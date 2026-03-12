using ContentHook.API.Controllers;
using ContentHook.API.DTOs;
using ContentHook.BL.Interfaces;
using ContentHook.DAL.Entities;
using ContentHook.DAL.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace ContentHook.Tests.API
{
    public class JobsControllerTests
    {
       

        private static (
            Mock<IJobRepository> jobRepo,
            Mock<ITranscriptService> transcriptService,
            Mock<IGenerationService> generationService,
            Mock<IProgressNotifier> notifier,
            Mock<IGenerationRepository> generationRepo,
            JobsController sut
        ) BuildController(string userId = "auth0|testuser")
        {
            var jobRepo = new Mock<IJobRepository>();
            var transcriptService = new Mock<ITranscriptService>();
            var generationService = new Mock<IGenerationService>();
            var notifier = new Mock<IProgressNotifier>();
            var generationRepo = new Mock<IGenerationRepository>();

            var sut = new JobsController(
                jobRepo.Object,
                transcriptService.Object,
                generationService.Object,
                notifier.Object,
                generationRepo.Object);

          
            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            return (jobRepo, transcriptService, generationService, notifier, generationRepo, sut);
        }

        private static StartGenerationRequest BuildRequest(
            string platform = "tiktok",
            string tonality = "Auto")
            => new StartGenerationRequest { Platform = platform, Tonality = tonality };


        [Fact]
        public async Task Generate_JobNotFound_Returns404()
        {
         
            var (jobRepo, _, _, _, _, sut) = BuildController();
            var jobId = Guid.NewGuid();

            jobRepo.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync((Job?)null);

           
            var result = await sut.Generate(jobId, BuildRequest(), CancellationToken.None);

            
            result.Should().BeOfType<NotFoundResult>();
        }

       

        [Fact]
        public async Task Generate_JobBelongsToOtherUser_Returns404()
        {
          
            var (jobRepo, _, _, _, _, sut) = BuildController(userId: "auth0|user-A");

            var job = new Job("auth0|user-B", "tiktok", "test.mp4", "storage-key");

            jobRepo.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);

            
            var result = await sut.Generate(job.Id, BuildRequest(), CancellationToken.None);

          
            result.Should().BeOfType<NotFoundResult>();
        }


        [Fact]
        public async Task Generate_StatusQueued_Returns400()
        {
            
            var userId = "auth0|testuser";
            var (jobRepo, _, _, _, _, sut) = BuildController(userId);

            var job = new Job(userId, "tiktok", "test.mp4", "key");
            

            jobRepo.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);

           
            var result = await sut.Generate(job.Id, BuildRequest(), CancellationToken.None);

            
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Generate_StatusTranscribing_Returns400()
        {
            
            var userId = "auth0|testuser";
            var (jobRepo, _, _, _, _, sut) = BuildController(userId);

            var job = new Job(userId, "tiktok", "test.mp4", "key");
            job.MarkAsTranscribing();

            jobRepo.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);

           
            var result = await sut.Generate(job.Id, BuildRequest(), CancellationToken.None);

            
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Generate_StatusGenerating_Returns400()
        {
           
            var userId = "auth0|testuser";
            var (jobRepo, _, _, _, _, sut) = BuildController(userId);

            var transcriptId = Guid.NewGuid();
            var job = new Job(userId, "tiktok", "test.mp4", "key");
            job.MarkAsTranscribed(transcriptId);
            job.MarkAsGenerating(transcriptId);

            jobRepo.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);

         
            var result = await sut.Generate(job.Id, BuildRequest(), CancellationToken.None);

          
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Generate_StatusFailed_Returns400()
        {
           
            var userId = "auth0|testuser";
            var (jobRepo, _, _, _, _, sut) = BuildController(userId);

            var job = new Job(userId, "tiktok", "test.mp4", "key");
            job.MarkAsFailed("Whisper error");

            jobRepo.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);

         
            var result = await sut.Generate(job.Id, BuildRequest(), CancellationToken.None);

         
            result.Should().BeOfType<BadRequestObjectResult>();
        }

     
        [Fact]
        public async Task Generate_TranscribedStatus_CallsGenerationService()
        {
            var userId = "auth0|testuser";
            var (jobRepo, transcriptService, generationService, notifier, _, sut) = BuildController(userId);

            var transcriptId = Guid.NewGuid();
            var job = new Job(userId, "tiktok", "test.mp4", "key");
            job.MarkAsTranscribed(transcriptId);

            var transcript = new Transcript(userId, "This is the video transcript text.", "de", "test.mp4");
        

            var generation = new Generation(userId, transcriptId, "tiktok",
                "Test Title", "Test Hook", "#test #hook", "gpt-4o-mini", "v1.0", 1, "Auto");

            jobRepo.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);
            jobRepo.Setup(r => r.UpdateAsync(It.IsAny<Job>())).ReturnsAsync((Job j) => j);

            
            transcriptService.Setup(t => t.GetByIdAsync(transcriptId)).ReturnsAsync(transcript);

            generationService.Setup(g => g.GenerateAsync(
                userId, transcript.Id, transcript.Text, "tiktok", "Auto", It.IsAny<CancellationToken>()))
                .ReturnsAsync(generation);

            notifier.Setup(n => n.NotifyAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<object>()))
                    .Returns(Task.CompletedTask);
            notifier.Setup(n => n.NotifyAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                    .Returns(Task.CompletedTask);

            
            var result = await sut.Generate(job.Id, BuildRequest("tiktok", "Auto"), CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            generationService.Verify(g => g.GenerateAsync(
                userId, transcript.Id, transcript.Text, "tiktok", "Auto",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        

        [Fact]
        public async Task Generate_DoneStatus_IsAllowedToRegenerate()
        {
           
            var userId = "auth0|testuser";
            var (jobRepo, transcriptService, generationService, notifier, _, sut) = BuildController(userId);

            var transcriptId = Guid.NewGuid();
            var job = new Job(userId, "tiktok", "test.mp4", "key");
            job.MarkAsTranscribed(transcriptId);
            job.MarkAsGenerating(transcriptId);
            job.MarkAsDone(Guid.NewGuid());

            var transcript = new Transcript(userId, "Transcript text.", "de", "test.mp4");

            var generation = new Generation(userId, transcriptId, "tiktok",
                "Regen Title", "Regen Hook", "#regen", "gpt-4o-mini", "v1.0", 2, "Auto");

            jobRepo.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);
            jobRepo.Setup(r => r.UpdateAsync(It.IsAny<Job>())).ReturnsAsync((Job j) => j);
            transcriptService.Setup(t => t.GetByIdAsync(transcriptId)).ReturnsAsync(transcript);
            generationService.Setup(g => g.GenerateAsync(
                It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(generation);
            notifier.Setup(n => n.NotifyAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<object>()))
                    .Returns(Task.CompletedTask);
            notifier.Setup(n => n.NotifyAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                    .Returns(Task.CompletedTask);

            
            var result = await sut.Generate(job.Id, BuildRequest(), CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
        }

       

        [Fact]
        public async Task Generate_TranscriptNotFound_Returns404()
        {
            
            var userId = "auth0|testuser";
            var (jobRepo, transcriptService, _, notifier, _, sut) = BuildController(userId);

            var transcriptId = Guid.NewGuid();
            var job = new Job(userId, "tiktok", "test.mp4", "key");
            job.MarkAsTranscribed(transcriptId);

            jobRepo.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);
            jobRepo.Setup(r => r.UpdateAsync(It.IsAny<Job>())).ReturnsAsync((Job j) => j);
            transcriptService.Setup(t => t.GetByIdAsync(transcriptId)).ReturnsAsync((Transcript?)null);
            notifier.Setup(n => n.NotifyAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                    .Returns(Task.CompletedTask);

            
            var result = await sut.Generate(job.Id, BuildRequest(), CancellationToken.None);

            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}