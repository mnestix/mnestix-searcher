using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MnestixSearcher.ApiServices.Contracts;
using MnestixSearcher.ApiServices.Contracts.Repository;
using MnestixSearcher.ApiServices.HttpClient;
using MnestixSearcher.ApiServices.Services;
using MnestixSearcher.ApiServices.Services.Repository;
using MnestixSearcher.ApiServices.Services.Shared;
using MnestixSearcher.ApiServices.Settings;
using MnestixSearcher.ApiServices.Visitors;
using static AasCore.Aas3_0.Visitation;

namespace MnestixSearcher.ApiServices
{
    public static class ServicesRegistration
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services, IConfiguration configuration)
        {
            var baseUrlSettings = configuration.GetSection("BaseUrlSettings").Get<BaseUrlSettings>();

            if (baseUrlSettings == null) { 
                throw new ArgumentNullException(nameof(baseUrlSettings));
            }

            services.AddSingleton<IAasHttpClient>(sp =>
                 new HttpRepoClient(baseUrlSettings.AasRepositoryBaseUrl));

            services.AddSingleton<ISubmodelHttpClient>(sp =>
                new HttpRepoClient(baseUrlSettings.SubmodelRepositoryBaseUrl));

            services.AddSingleton<IConceptHttpClient>(sp =>
                new HttpRepoClient(baseUrlSettings.ConceptDescriptionRepositoryBaseUrl));
            services.AddScoped<IAasRepoService, AasRepoService>();
            services.AddScoped<ISubmodelRepoService, SubmodelRepoService>();

            return services;
        }

        public static IServiceCollection AddMnestixServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IBase64Service, Base64Service>();
            services.AddScoped<IAasSearcherService, AasSearcherService>();
            services.AddScoped<IAasService, AasService>();
            services.AddScoped<ISubmodelService, SubmodelService>();
            services.AddScoped<IFilterService, FilterService>();
            services.AddScoped<IVisitorFactory, VisitorFactory>();
            return services;
        }
    }
}
