﻿using Library.AsyncMessages;
using Service.Students.AsyncDataServices;
using Service.Students.Middleware;

namespace Service.Students;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddGrpc();
        services.AddScoped<ApplicationDbContextInitializer>();
        services.AddTransient<IMessageBusClient, MessageBusClient>();
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