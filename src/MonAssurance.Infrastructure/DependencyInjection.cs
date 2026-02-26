using Microsoft.Extensions.DependencyInjection;
using MonAssurance.Application.Shared;
using MonAssurance.Application;
using MonAssurance.Infrastructure.CQRS;

namespace MonAssurance.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers a single command or query handler.
    /// </summary>
    public static IServiceCollection AddHandler<THandler>(this IServiceCollection services)
        where THandler : class
    {
        var handlerType = typeof(THandler);
        var handlerInterfaces = handlerType.GetInterfaces()
            .Where(i => i.IsGenericType && 
                   (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                    i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));

        foreach (var @interface in handlerInterfaces)
            services.AddScoped(@interface, handlerType);

        return services;
    }

    /// <summary>
    /// Registers all command and query handlers from the Application assembly.
    /// Uses reflection to discover all types implementing handler interfaces.
    /// </summary>
    public static IServiceCollection AddApplicationHandlers(
        this IServiceCollection services)
    {
        var applicationAssembly = typeof(IApplicationMarker).Assembly;
        
        var allTypes = applicationAssembly.GetTypes()
            .Where(t => !t.IsInterface && !t.IsAbstract);
            
        foreach (var type in allTypes)
        {
            var handlerInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && 
                       (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                        i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                        i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));
                        
            foreach (var @interface in handlerInterfaces)
                services.AddScoped(@interface, type);
        }
        
        return services;
    }

    /// <summary>
    /// Registers Infrastructure services (repositories, external services, DbContext).
    /// Note: Handlers should be registered individually using AddHandler<THandler>().
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        // Register CQRS senders
        services.AddScoped<ICommandSender, CommandSender>();
        services.AddScoped<IQuerySender, QuerySender>();
        
        // Register handlers individually (recommended approach)
        // services.AddHandler<MyCommandHandler>();
        // services.AddHandler<MyQueryHandler>();
        
        // OR use automatic discovery (scans entire Application assembly)
        // services.AddApplicationHandlers();
        
        // TODO: Add EF Core DbContext
        // services.AddDbContext<AppDbContext>(options => ...);
        
        // TODO: Add repositories
        // services.AddScoped<IOrderRepository, OrderRepository>();
        
        return services;
    }
}
