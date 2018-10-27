using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Inventory.Microservices.Net.Core.Api.Base.IntegrationEvents.EventHandling;
using Inventory.Microservices.Net.Core.Api.Base.IntegrationEvents.Events;
using Inventory.Microservices.Net.Core.EventBus;
using Inventory.Microservices.Net.Core.EventBus.Abstractions;
using Inventory.Microservices.Net.Core.EventBusRabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Swashbuckle.AspNetCore.Swagger;

namespace Api.Base
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

            
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = Configuration.GetValue<string>("RabbitMQ:HostName")
                };

                if (Configuration.GetValue<int>("RabbitMQ:Port") != 0)
                {
                    factory.Port = Configuration.GetValue<int>("RabbitMQ:Port");
                }

                if (!string.IsNullOrEmpty(Configuration.GetValue<string>("RabbitMQ:UserName")))
                {
                    factory.UserName = Configuration.GetValue<string>("RabbitMQ:UserName");
                }

                if (!string.IsNullOrEmpty(Configuration.GetValue<string>("RabbitMQ:Password")))
                {
                    factory.Password = Configuration.GetValue<string>("RabbitMQ:Password");
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });
            RegisterEventBus(services);
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Info
                {
                    Title = "Base HTTP API",
                    Version = "v1",
                    Description = "The Base Service HTTP API",
                    TermsOfService = "Terms Of Service"
                });

            });


            var container = new ContainerBuilder();
            container.Populate(services);
            services.AddSingleton<ILifetimeScope>(container.Build());

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseMvc();
            app.UseSwagger()
            .UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Basket.API V1");
                c.OAuthClientId("baseswaggerui");
                c.OAuthAppName("Base Swagger UI");
            });
            ConfigureEventBus(app);
        }



        private void RegisterEventBus(IServiceCollection services)
        {
            var subscriptionClientName = Configuration["SubscriptionClientName"];
            //
            services.AddTransient<Itest, Test>();
            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                var test = sp.GetRequiredService<Itest>();
                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddTransient<ProductChangedIntegrationEventHandler>();
            services.AddTransient<ProductAddIntegrationEventHandler>();
            //services.AddTransient<ProductPriceChangedIntegrationEventHandler>();
            //services.AddTransient<OrderStartedIntegrationEventHandler>();
        }


        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<ProductChangedIntegrationEvent, ProductChangedIntegrationEventHandler>();
            eventBus.Subscribe<ProductAddIntegrationEvent, ProductAddIntegrationEventHandler>();
        }
    }
}
