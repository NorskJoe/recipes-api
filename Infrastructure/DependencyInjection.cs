using Application.Common.Interfaces;
using Application.Features.Recipes.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found."
                );

            services.AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(
                connectionString
            ));

            services.AddScoped<IRecipeReadRepository, RecipeReadRepository>();
            services.AddScoped<IRecipeWriteRepository, RecipeWriteRepository>();

            services.AddScoped<DatabaseInitializer>();

            return services;
        }
    }
}
