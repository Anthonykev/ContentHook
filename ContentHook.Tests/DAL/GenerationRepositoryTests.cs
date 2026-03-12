using ContentHook.DAL.Entities;
using ContentHook.DAL.ORMapper;
using ContentHook.DAL.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ContentHook.Tests.DAL
{
    public class GenerationRepositoryTests : IDisposable
    {
       

        private readonly AppDbContext _db;
        private readonly GenerationRepository _sut;

        public GenerationRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique per test
                .Options;

            _db = new AppDbContext(options);
            _sut = new GenerationRepository(_db);
        }

        public void Dispose() => _db.Dispose();

        private static Generation BuildGeneration(
            string userId = "auth0|user1",
            Guid? transcriptId = null,
            string platform = "tiktok",
            int regenerationIndex = 1)
        {
            return new Generation(
                userId,
                transcriptId ?? Guid.NewGuid(),
                platform,
                title: "Test Title",
                hook: "Test Hook",
                hashtags: "#test #unit",
                modelUsed: "gpt-4o-mini",
                promptVersion: "v1.0",
                regenerationIndex: regenerationIndex,
                tonality: "Auto");
        }


        [Fact]
        public async Task AddAsync_SavesGenerationToDatabase()
        {
            // Arrange
            var generation = BuildGeneration();

           
            var result = await _sut.AddAsync(generation);

        
            result.Should().NotBeNull();
            result.Id.Should().NotBe(Guid.Empty);

            var inDb = await _db.Generations.FindAsync(result.Id);
            inDb.Should().NotBeNull();
        }

   
        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsGeneration()
        {
            // Arrange
            var generation = BuildGeneration();
            await _sut.AddAsync(generation);

           
            var result = await _sut.GetByIdAsync(generation.Id);

        
            result.Should().NotBeNull();
            result!.Id.Should().Be(generation.Id);
        }

        

        [Fact]
        public async Task GetByIdAsync_UnknownId_ReturnsNull()
        {
           
            var result = await _sut.GetByIdAsync(Guid.NewGuid());

        
            result.Should().BeNull();
        }


        [Fact]
        public async Task CountByTranscriptAndPlatformAsync_ReturnsCorrectCount()
        {
            // Arrange
            var transcriptId = Guid.NewGuid();

            await _sut.AddAsync(BuildGeneration(transcriptId: transcriptId, platform: "tiktok", regenerationIndex: 1));
            await _sut.AddAsync(BuildGeneration(transcriptId: transcriptId, platform: "tiktok", regenerationIndex: 2));
            await _sut.AddAsync(BuildGeneration(transcriptId: transcriptId, platform: "instagram", regenerationIndex: 1));

           
            var tiktokCount = await _sut.CountByTranscriptAndPlatformAsync(transcriptId, "tiktok");
            var instagramCount = await _sut.CountByTranscriptAndPlatformAsync(transcriptId, "instagram");

        
            tiktokCount.Should().Be(2);
            instagramCount.Should().Be(1);
        }

       

        [Fact]
        public async Task CountByTranscriptAndPlatformAsync_NoGenerations_ReturnsZero()
        {
           
            var count = await _sut.CountByTranscriptAndPlatformAsync(Guid.NewGuid(), "tiktok");

        
            count.Should().Be(0);
        }

     

        [Fact]
        public async Task CountByTranscriptAndPlatformAsync_At3_ExceedsLimit()
        {
            // Arrange
            var transcriptId = Guid.NewGuid();

            for (int i = 1; i <= 3; i++)
                await _sut.AddAsync(BuildGeneration(transcriptId: transcriptId, platform: "tiktok", regenerationIndex: i));

           
            var count = await _sut.CountByTranscriptAndPlatformAsync(transcriptId, "tiktok");

        
            count.Should().BeGreaterThanOrEqualTo(3);
        }


        [Fact]
        public async Task GetByTranscriptIdAsync_ReturnsAllForTranscript()
        {
            // Arrange
            var transcriptId = Guid.NewGuid();
            var otherTranscriptId = Guid.NewGuid();

            await _sut.AddAsync(BuildGeneration(transcriptId: transcriptId, platform: "tiktok"));
            await _sut.AddAsync(BuildGeneration(transcriptId: transcriptId, platform: "instagram"));
            await _sut.AddAsync(BuildGeneration(transcriptId: otherTranscriptId, platform: "tiktok")); // andere Transcript

           
            var results = await _sut.GetByTranscriptIdAsync(transcriptId);

        
            results.Should().HaveCount(2);
            results.Should().AllSatisfy(g => g.TranscriptId.Should().Be(transcriptId));
        }

   
        [Fact]
        public async Task DeleteByTranscriptIdAsync_RemovesAllGenerations()
        {
            // Arrange
            var transcriptId = Guid.NewGuid();

            await _sut.AddAsync(BuildGeneration(transcriptId: transcriptId, regenerationIndex: 1));
            await _sut.AddAsync(BuildGeneration(transcriptId: transcriptId, regenerationIndex: 2));

            // Verify they exist
            var before = await _sut.GetByTranscriptIdAsync(transcriptId);
            before.Should().HaveCount(2);

           
            await _sut.DeleteByTranscriptIdAsync(transcriptId);

        
            var after = await _sut.GetByTranscriptIdAsync(transcriptId);
            after.Should().BeEmpty();
        }


        [Fact]
        public async Task GetByIdForUserAsync_CorrectUser_ReturnsGeneration()
        {
            // Arrange
            var userId = "auth0|user1";
            var generation = BuildGeneration(userId: userId);
            await _sut.AddAsync(generation);

           
            var result = await _sut.GetByIdForUserAsync(generation.Id, userId);

        
            result.Should().NotBeNull();
            result!.UserId.Should().Be(userId);
        }



        [Fact]
        public async Task GetByIdForUserAsync_WrongUser_ReturnsNull()
        {
            // Arrange
            var generation = BuildGeneration(userId: "auth0|user1");
            await _sut.AddAsync(generation);

           
            var result = await _sut.GetByIdForUserAsync(generation.Id, "auth0|intruder");

        
            result.Should().BeNull();
        }
    }
}