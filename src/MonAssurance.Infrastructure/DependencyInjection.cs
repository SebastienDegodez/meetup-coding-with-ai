using Microsoft.Extensions.DependencyInjection;
using MonAssurance.Application._Contracts;
using MonAssurance.Application;

namespace MonAssurance.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers all application layer services using convention-based discovery.
    /// Handlers are discovered by naming: *CommandHandler, *QueryHandler.
    /// </summary>
    public static IServiceCollection AddApplicationHandlers(
        this IServiceCollection services)
    {
        var applicationAssembly = typeof(IApplicationMarker).Assembly;
        
        // Discover all *CommandHandler classes
        var commandHandlers = applicationAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("CommandHandler") && 
                       !t.IsInterface && 
                       !t.IsAbstract);
            
        foreach (var handler in commandHandlers)
        {
            var interfaces = handler.GetInterfaces()
                .Where(i => i.IsGenericType && 
                       (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                        i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)));
                        
            foreach (var @interface in interfaces)
                services.AddScoped(@interface, handler);
        }
        
        // Discover all *QueryHandler classes
        var queryHandlers = applicationAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("QueryHandler") && 
                       !t.IsInterface && 
                       !t.IsAbstract);
            
        foreach (var handler in queryHandlers)
        {
            var interfaces = handler.GetInterfaces()
                .Where(i => i.IsGenericType && 
                       i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));
                        
            foreach (var @interface in interfaces)
                services.AddScoped(@interface, handler);
        }
        
        return services;
    }

    /// <summary>
    /// Registers Infrastructure services (repositories, external services, DbContext).
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        // Register handlers
        services.AddApplicationHandlers();
        
        // TODO: Add EF Core DbContext
        // services.AddDbContext<AppDbContext>(options => ...);
        
        // TODO: Add repositories
        // services.AddScoped<IOrderRepository, OrderRepository>();
        
        return services;
    }
}
