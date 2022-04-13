using Enexure.MicroBus;
using Enexure.MicroBus.Messages;
using Enexure.MicroBus.MicrosoftDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Nike.Api.Activators;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Kafka.AspNetCore;
using Nike.Mediator.Handlers;
using Nike.Redis.Microsoft.DependencyInjection;
using Nike.Swagger;
using Nike.Web.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using Nike.EventBus.Redis.Model;
using Nike.EventBus.Redis.Services;

namespace Nike.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddWrappingApiResult();
            ConfigureMicroBus(services);
            //ConfigureKafka(services);
            //ConfigureElasticSearch(services);
            ConfigureRedis(services);
            // ConfigureSwagger(services);
      
            // ConfigureCors(services);
           var sss =  services.BuildServiceProvider();
        var dispacher =    sss.GetService<IEventBusDispatcher>();
        dispacher.Publish(new SwitchScanResponseIntegrationEvent {});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseCors(policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyMethod();
                policy.AllowAnyHeader();
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    context.Response.StatusCode = (int) HttpStatusCode.Redirect;
                    context.Response.Redirect("/swagger");
                });
            });

            app.UseSwaggerDefault(swaggerUiOptions: (options =>
            {
                options.SwaggerEndpoint("api-v1/swagger.json", "Nike API V1");
                options.ConfigObject.DisplayOperationId = true;
            }));
        }

        #region PrivateMethods

        private void ConfigureKafka(IServiceCollection services)
        {
            var busConfig = Configuration.GetSection("EventBus").Get<EventBusConfig>();
            services.AddKafkaProducer(busConfig.ConnectionString);
            services.AddKafkaConsumer(busConfig.ConnectionString, Assembly.GetExecutingAssembly().GetName().Name);


            services.AddHostedService<ConsumerHostedService>();
        }

        private void ConfigureElasticSearch(IServiceCollection services)
        {
            services.AddElasticSearch(Configuration.GetSection("ElasticSearch")
                .Get<ElasticSearchExtensions.ElasticSearchConfiguration>());
        }

        private void ConfigureRedis(IServiceCollection services)
        {
           // var redisConfig = Configuration.GetSection("Cache").Get<RedisConfig>();
           // services.AddRedis(redisConfig.ConnectionString);
            var redis = new RedisSetting
            {
                ConnectionString = "192.168.101.22:6379,password=sdfhjg3457fgbSDFf"
            };
            services.AddRedisClientServiceWithConfig(redis);
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwagger(options =>
            {
                options.SwaggerDoc("api-v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = $"Nike API Gateway",
                    Description = "API Gateway for Sabacell's Nike project",
                });

                var filePath = Path.Combine(AppContext.BaseDirectory, "Nike.Api.xml");
                options.IncludeXmlComments(filePath);
                options.AddBearerToken();

                options.CustomOperationIds(apiDescription =>
                    apiDescription.TryGetMethodInfo(out var apiMethodInfo)
                        ? apiMethodInfo.Name.Replace("Async", "")
                        : "OperationIdNotSet");
            });
        }

        private static void ConfigureMicroBus(IServiceCollection services)
        {
            var busBuilder = new BusBuilder()
                .RegisterGlobalHandler<CacheInvalidationDelegatingHandler>()
                .RegisterEventHandler<NoMatchingRegistrationEvent, NoMatchingRegistrationEventHandler>()
                .RegisterHandlers(typeof(Startup).Assembly);


            services.RegisterMicroBus(busBuilder);
        }

        private static void ConfigureCors(IServiceCollection services)
        {
            services.AddCors();
        }

        #endregion
    }
}