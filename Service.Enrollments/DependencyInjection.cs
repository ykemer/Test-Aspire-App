using Library.AsyncMessages;
using Service.Enrollments.AsyncDataServices;
using Service.Enrollments.Middleware;

namespace Service.Enrollments;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddGrpc();
        services.AddScoped<ApplicationDbContextInitializer>();
        services.AddScoped<IMessageBusClient, MessageBusClient>();
        services.AddSingleton<IEventProcessor, EventProcessor>();
        services.AddHostedService<MessageBusSubscriber>();
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));
            options.AddOpenRequestPostProcessor(typeof(LoggingBehaviour<,>));
            options.AutoRegisterRequestProcessors = false;
        });
        
        
        return services;
    }
}