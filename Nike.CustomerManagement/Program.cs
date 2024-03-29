﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Enexure.MicroBus.Messages;
using Enexure.MicroBus.MicrosoftDependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nike.CustomerManagement.Application.Customers.Commands;
using Nike.CustomerManagement.Application.Customers.IntegrationEvents;
using Nike.CustomerManagement.Domain.Customers;
using Nike.CustomerManagement.Infrastructure;
using Nike.CustomerManagement.Infrastructure.Services.Customers;
using Nike.EntityFramework;
using Nike.EntityFramework.Microsoft.DependencyInjection;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Kafka.AspNetCore;
using Nike.Framework.Domain;
using Nike.Framework.Domain.Persistence;
using Nike.Mediator.Handlers;
using Nike.Redis.Microsoft.DependencyInjection;

namespace Nike.CustomerManagement;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        // var scop = host.Services.CreateScope();
        // var bus = scop.ServiceProvider.GetService<IEventBusDispatcher>();
        // while (true)
        // {
        //      bus.PublishAsync(new ActiveCustomerIntegrationEvent());
        //     Thread.Sleep(1);
        // }

        // var bus = scop.ServiceProvider.GetService<IMicroMediator>();
        // var I = 1;
        // while (true)
        // {
        //     var command = new RegisterCustomerCommand
        //     {
        //         FirstName = $"Name {I}",
        //         LastName = $"Name {I}",
        //         NationalCode = I.ToString()
        //     };
        //     bus.SendAsync(command);
        //     Thread.Sleep(1);
        // }
        host.Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .ConfigureServices(Configuration);
    }

    private static void Configuration(HostBuilderContext hostContext, IServiceCollection services)
    {
        ConfigureRedis(hostContext, services);
        ConfigureElasticSearch(hostContext, services);
        ConfigureKafka(hostContext, services);
        // ConfigureEntityFrameWork(hostContext, services);
        ConfigureMicroBus(services);
        ConfigureStoreServices(services);

        services.AddSingleton<IClock, SystemClock>();
        //  services.AddHostedService<ConsumerHostedService>();
    }


    #region PrivateMethods

    private static void ConfigureRedis(HostBuilderContext hostContext, IServiceCollection services)
    {
        services.AddRedis(hostContext.Configuration.GetSection("Cache").Get<RedisConfig>());
    }

    private static void ConfigureElasticSearch(HostBuilderContext hostContext, IServiceCollection services)
    {
        var configuration = hostContext.Configuration.GetSection("ElasticSearch").Get<ElasticSearchConfiguration>();
        services.AddElasticSearch(configuration);
    }

    private static void ConfigureKafka(HostBuilderContext hostContext, IServiceCollection services)
    {
        var busConfig = hostContext.Configuration.GetSection("EventBus").Get<EventBusConfig>();
        services.AddKafkaProducer(busConfig.ConnectionString);
        services.AddKafkaConsumer(busConfig.ConnectionString, typeof(Program).Namespace);
    }

    private static void ConfigureEntityFrameWork(HostBuilderContext hostContext, IServiceCollection services)
    {
        services.AddEntityFrameworkSqlServer()
            .AddEntityFrameworkUnitOfWork()
            .AddEntityFrameworkDefaultRepository()
            .AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                    });
            })
            .AddScoped<IDbContextAccessor>(s => new DbContextAccessor(s.GetRequiredService<DatabaseContext>()));
    }

    private static void ConfigureMicroBus(IServiceCollection services)
    {
        services.RegisterMicroBus(new BusBuilder()
            .RegisterGlobalHandler<CacheInvalidationDelegatingHandler>()
            .RegisterEventHandler<NoMatchingRegistrationEvent, NoMatchingRegistrationEventHandler>()
            .RegisterHandlers(typeof(Program).Assembly));
    }

    private static void ConfigureStoreServices(IServiceCollection services)
    {
        services.AddTransient<ICustomerStoreService, CustomerStoreService>();
    }

    #endregion
}