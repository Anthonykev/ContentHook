using ContentHook.BL.Interfaces;
using ContentHook.BL.Queue;
using ContentHook.BL.Services;
using ContentHook.BL.Workers;
using ContentHook.DAL.Interfaces;
using ContentHook.DAL.Repositories;
using ContentHook.API.Services;

namespace ContentHook.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // DAL
            services.AddScoped<ITranscriptRepository, TranscriptRepository>();
            services.AddScoped<IJobRepository, JobRepository>();         

            // BL
            services.AddScoped<ITranscriptService, TranscriptService>();
            services.AddScoped<IFFmpegService, FFmpegService>();                    
            services.AddScoped<IVideoStorageService, TempFileVideoStorageService>(); 

            // Whisper HttpClient
            services.AddHttpClient<ITranscriptionService, WhisperTranscriptionService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            //GPT Generation
            services.AddSingleton<IRuleProvider, RuleProvider>();
            services.AddScoped<IPromptBuilder, PromptBuilder>();
            services.AddScoped<IGenerationService, GenerationService>();
            services.AddScoped<IGenerationRepository, GenerationRepository>();

            services.AddHttpClient<IGptService, GptService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(2);
            });

            // Queue — Singleton: ein Channel für die gesamte App-Laufzeit
            services.AddSingleton<IJobQueue, JobChannel>();

            // SignalR Notifier — API implementiert das BL-Interface
            services.AddSingleton<IProgressNotifier, SignalRProgressNotifier>();

            // Background Worker — wird automatisch gestartet
            services.AddHostedService<VideoProcessingWorker>();

            // History (API-Layer — aggregiert Job + Transcript + Generations)
            services.AddScoped<HistoryService>();

            return services;
        }
    }
}