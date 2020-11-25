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

            var lst1 = new List<Msg1>();
            var lst2 = new List<Msg2>();
            var lst3 = new List<Msg3>();
            var lst4 = new List<Msg4>();
            var lst5 = new List<Msg5>();

            var size = 10000;

            for (int i = 0; i < size; i++)
            {
                lst1.Add(new Msg1($"msg_{i}", "sample desc", i));
            }

            for (int i = 0; i < size; i++)
            {
                lst2.Add(new Msg2($"msg_{i}", "sample desc", i));
            }

            for (int i = 0; i < size; i++)
            {
                lst3.Add(new Msg3($"msg_{i}", "sample desc", i));
            }

            for (int i = 0; i < size; i++)
            {
                lst4.Add(new Msg4($"msg_{i}", "sample desc", i));
            }

            for (int i = 0; i < size; i++)
            {
                lst5.Add(new Msg5($"msg_{i}", "sample desc", i));
            }

            var t1 = Task.Factory.StartNew(() => Publish(dispatcher, lst1), TaskCreationOptions.LongRunning);
            var t2 = Task.Factory.StartNew(() => Publish(dispatcher, lst2), TaskCreationOptions.LongRunning);
            var t3 = Task.Factory.StartNew(() => Publish(dispatcher, lst3), TaskCreationOptions.LongRunning);
            var t4 = Task.Factory.StartNew(() => Publish(dispatcher, lst4), TaskCreationOptions.LongRunning);
            var t5 = Task.Factory.StartNew(() => Publish(dispatcher, lst5), TaskCreationOptions.LongRunning);

            Task.WaitAll(t1, t2, t3, t4);

            Console.WriteLine($"Publishing {size} msgs1 to Kafka in {t1.Result}. MEANS: {t1.Result / size}");
            Console.WriteLine($"Publishing {size} msgs2 to Kafka in {t2.Result}. MEANS: {t2.Result / size}");
            Console.WriteLine($"Publishing {size} msgs3 to Kafka in {t3.Result}. MEANS: {t3.Result / size}");
            Console.WriteLine($"Publishing {size} msgs4 to Kafka in {t4.Result}. MEANS: {t4.Result / size}");
            Console.WriteLine($"Publishing {size} msgs5 to Kafka in {t4.Result}. MEANS: {t5.Result / size}");
        }

        private static double Publish(IEventBusDispatcher publisher, List<Msg1> lst)
        {
            var sw = Stopwatch.StartNew();
            foreach (var model in lst)
            {
                publisher.Publish(model);
            }

            sw.Stop();

            return sw.Elapsed.TotalMilliseconds;
        }

        private static double Publish(IEventBusDispatcher publisher, List<Msg2> lst)
        {
            var sw = Stopwatch.StartNew();
            foreach (var model in lst)
            {
                publisher.Publish(model);
            }

            sw.Stop();

            return sw.Elapsed.TotalMilliseconds;
        }

        private static double Publish(IEventBusDispatcher publisher, List<Msg3> lst)
        {
            var sw = Stopwatch.StartNew();
            foreach (var model in lst)
            {
                publisher.Publish(model);
            }

            sw.Stop();

            return sw.Elapsed.TotalMilliseconds;
        }

        private static double Publish(IEventBusDispatcher publisher, List<Msg4> lst)
        {
            var sw = Stopwatch.StartNew();
            foreach (var model in lst)
            {
                publisher.Publish(model);
            }

            sw.Stop();

            return sw.Elapsed.TotalMilliseconds;
        }

        private static double Publish(IEventBusDispatcher publisher, List<Msg5> lst)
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