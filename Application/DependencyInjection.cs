using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    /// <summary>
    /// Registers Application-layer services (MediatR handlers, validators, etc.).
    /// Called from Presentation/Program.cs.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
