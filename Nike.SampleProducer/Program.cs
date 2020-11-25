using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Enexure.MicroBus.Messages;
using Enexure.MicroBus.MicrosoftDependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Kafka.AspNetCore;
using Nike.Framework.Domain;
using Nike.Mediator.Handlers;
using Nike.Redis.Microsoft.DependencyInjection;
using Nike.SampleProducer.Model;

namespace Nike.SampleProducer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            Test(host.Services);
            host.Run();
        }


        private static void Test(IServiceProvider serviceProvider)
        {
            var dispatcher = serviceProvider.GetRequiredService<IEventBusDispatcher>();

            var lst0 = new List<MyMessage2Part>();
            var lst = new List<bncMsgIntegrationEvent>();
            var lst2 = new List<bncMsg2IntegrationEvent>();
            var lst3 = new List<bncMsg3IntegrationEvent>();
            var lst4 = new List<bncMsg4IntegrationEvent>();

            var size = 100;

            for (int i = 0; i < size; i++)
            {
                lst0.Add(new MyMessage2Part($"msg_{i}", "sample desc", i));
            }

            for (int i = 0; i < size; i++)
            {
                lst.Add(new bncMsgIntegrationEvent($"msg_{i}", "sample desc", i));
            }

            for (int i = 0; i < size; i++)
            {
                lst2.Add(new bncMsg2IntegrationEvent($"msg_{i}", "sample desc", i));
            }

            for (int i = 0; i < size; i++)
            {
                lst3.Add(new bncMsg3IntegrationEvent($"msg_{i}", "sample desc", i));
            }

            for (int i = 0; i < size; i++)
            {
                lst4.Add(new bncMsg4IntegrationEvent($"msg_{i}", "sample desc", i));
            }

            Publish(dispatcher, lst0);
            //var t0 = Task.Factory.StartNew(() => Publish(dispatcher, lst0), TaskCreationOptions.LongRunning);
            // var t1 = Task.Factory.StartNew(() => Publish(dispatcher, lst), TaskCreationOptions.LongRunning);
            //
            // var t2 = Task.Factory.StartNew(() => Publish(dispatcher, lst2), TaskCreationOptions.LongRunning);
            // var t3 = Task.Factory.StartNew(() => Publish(dispatcher, lst3), TaskCreationOptions.LongRunning);
            // var t4 = Task.Factory.StartNew(() => Publish(dispatcher, lst4), TaskCreationOptions.LongRunning);
            //
            // Task.WaitAll(t1, t2, t3, t4);
            //
            // Console.WriteLine($"Publishing {size} msgs to Kafka in {t1.Result}. MEANS: {t1.Result / size}");
            // Console.WriteLine($"Publishing {size} msgs to Kafka in {t2.Result}. MEANS: {t2.Result / size}");
            // Console.WriteLine($"Publishing {size} msgs to Kafka in {t3.Result}. MEANS: {t3.Result / size}");
            // Console.WriteLine($"Publishing {size} msgs to Kafka in {t4.Result}. MEANS: {t4.Result / size}");
        }

        private static double Publish(IEventBusDispatcher publisher, List<MyMessage2Part> lst)
        {
            var sw = Stopwatch.StartNew();
            foreach (var model in lst)
            {
                publisher.Publish(model);
            }

            sw.Stop();

            return sw.Elapsed.TotalMilliseconds;
        }  private static double Publish(IEventBusDispatcher publisher, List<bncMsgIntegrationEvent> lst)
        {
            var sw = Stopwatch.StartNew();
            foreach (var model in lst)
            {
                publisher.Publish(model);
            }

            sw.Stop();

            return sw.Elapsed.TotalMilliseconds;
        }

        private static double Publish(IEventBusDispatcher publisher, List<bncMsg2IntegrationEvent> lst)
        {
            var sw = Stopwatch.StartNew();
            foreach (var model in lst)
            {
                publisher.Publish(model);
            }

            sw.Stop();

            return sw.Elapsed.TotalMilliseconds;
        }

        private static double Publish(IEventBusDispatcher publisher, List<bncMsg3IntegrationEvent> lst)
        {
            var sw = Stopwatch.StartNew();
            foreach (var model in lst)
            {
                publisher.Publish(model);
            }

            sw.Stop();

            return sw.Elapsed.TotalMilliseconds;
        }

        private static double Publish(IEventBusDispatcher publisher, List<bncMsg4IntegrationEvent> lst)
        {
            var sw = Stopwatch.StartNew();
            foreach (var model in lst)
            {
                publisher.Publish(model);
            }

            sw.Stop();

            return sw.Elapsed.TotalMilliseconds;
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
            ConfigureKafka(hostContext, services);
            ConfigureMicroBus(services);
            services.AddSingleton<IClock, SystemClock>();
        }


        #region PrivateMethods

        private static void ConfigureRedis(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddRedis(hostContext.Configuration.GetSection("Cache").Get<RedisConfig>());
        }

        private static void ConfigureKafka(HostBuilderContext hostContext, IServiceCollection services)
        {
            var busConfig = hostContext.Configuration.GetSection("EventBus").Get<EventBusConfig>();
            services.AddKafkaProducer(busConfig.ConnectionString);
        }

        private static void ConfigureMicroBus(IServiceCollection services)
        {
            services.RegisterMicroBus(new BusBuilder()
                .RegisterEventHandler<NoMatchingRegistrationEvent, NoMatchingRegistrationEventHandler>()
                .RegisterHandlers(typeof(Program).Assembly));
        }

        #endregion
    }
}