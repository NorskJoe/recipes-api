using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    /// <summary>
    /// Registers Infrastructure-layer services (Dapper connection factory,
    /// database initializer). Called from Presentation/Program.cs.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found.");

            services.AddSingleton<IDbConnectionFactory>(
                _ => new SqlConnectionFactory(connectionString));

            services.AddScoped<DatabaseInitializer>();

            return services;
        }
    }
}
