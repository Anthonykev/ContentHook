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

            // Queue — Singleton: ein Channel für die gesamte App-Laufzeit
            services.AddSingleton<IJobQueue, JobChannel>();

            // SignalR Notifier — API implementiert das BL-Interface
            services.AddSingleton<IProgressNotifier, SignalRProgressNotifier>();

            // Background Worker — wird automatisch gestartet
            services.AddHostedService<VideoProcessingWorker>();           

            return services;
        }
    }
}