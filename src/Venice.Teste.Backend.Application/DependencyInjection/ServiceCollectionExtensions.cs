using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Venice.Teste.Backend.Application.Mappings;
using Venice.Teste.Backend.Infrastructure.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Venice.Teste.Backend.Application.UseCases.Product.Create;

namespace Venice.Teste.Backend.Application.DependencyInjection
{
    [ExcludeFromCodeCoverage]
	public static class ServiceCollectionExtensions
	{
		public static void AddDependencyApplicationSetup(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddApplicationLayer();
			services.AddValidation();
			services.AddInfrastructure(configuration);
		}

		private static void AddApplicationLayer(this IServiceCollection services)
		{
			services.AddAutoMapper(typeof(ManagerProfile));
			services.AddMediatR(c => c.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
		}

		private static void AddValidation(this IServiceCollection services)
		{
			services.AddFluentValidationAutoValidation();
			services.AddFluentValidationClientsideAdapters();
			services.AddValidatorsFromAssemblyContaining<CommandValidator>();
		}
        
        private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddPersistenceContexts(configuration);
			services.AddRepositories();
            services.AddCache(configuration);
            services.AddMessaging(configuration);
			services.AddHealthChecks();
		}
    }
}