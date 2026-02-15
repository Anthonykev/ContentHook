
using ContentHook.BL.Interfaces;
using ContentHook.BL.Services;
using ContentHook.DAL.Interfaces;
using ContentHook.DAL.Repositories;

namespace ContentHook.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // DAL
            services.AddScoped<ITranscriptRepository, TranscriptRepository>();

            // BL
            services.AddScoped<ITranscriptService, TranscriptService>();

            return services;
        }
    }
}