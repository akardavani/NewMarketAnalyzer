using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Messaging;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Infrastructure;
using MarketAnalyzer.Infrastructure.Authentication;
using MarketAnalyzer.Infrastructure.Authentication.ApiKey;
using MarketAnalyzer.Infrastructure.Cqrs;
using MarketAnalyzer.Infrastructure.ExcelParser;
using MarketAnalyzer.Infrastructure.Json;
using MarketAnalyzer.Infrastructure.Messaging;
using MarketAnalyzer.Infrastructure.Messaging.Asiatech;
using MarketAnalyzer.Infrastructure.Messaging.Candoo;
using MarketAnalyzer.Infrastructure.Messaging.Email;
using MarketAnalyzer.Infrastructure.OperationRequirements;
using MarketAnalyzer.Infrastructure.RabbitMQ;
using MarketAnalyzer.Infrastructure.Reports;
using MarketAnalyzer.Infrastructure.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddFundInfrastructure(this IServiceCollection services, IConfiguration configuration, bool useRouteAuthorization)
        {
            services.Configure<JsonTokenOptions>(configuration.GetSection(nameof(JsonTokenOptions)));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer()
                .AddApiKey(); //NOTE: Implement IApiKeyValidator in any Api.

            services.Configure<MvcOptions>(options =>
            {
                options.Conventions.Add(new AuthorizationActionModelConvention());
                options.ModelBinderProviders.Insert(0, new DateOnlyModelBinderProvider());
            });
            services.AddAuthorizationBuilder()
                .AddPolicy(AuthenticationConstants.RequiredApiKeyPolicy, builder =>
                {
                    ConfigPolicy(builder, ApiKeyDefaults.AuthenticationScheme);
                })
                .AddPolicy(AuthenticationConstants.DefaultPolicy, builder =>
                {
                    ConfigPolicy(builder, JwtBearerDefaults.AuthenticationScheme);
                    if (useRouteAuthorization)
                    {
                        builder.AddRequirements(RouteAuthorizationRequirement.Instance);
                    }
                });

            services.PostConfigureAll<JsonOptions>(options =>
            {
                options.JsonSerializerOptions.Converters.Insert(0, new DateOnlyJsonConverter());
                options.JsonSerializerOptions.Converters.Insert(0, new DateTimeJsonConverter());
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            });

            services.Configure<ReportsOptions>(configuration.GetSection(nameof(ReportsOptions)));
            services.Configure<RabbitMQMessageBrokerOptions>(configuration.GetSection(nameof(RabbitMQMessageBrokerOptions)));

            services.AddHttpContextAccessor();
            services.TryAddSingleton<ICacheHelper, CacheHelper>();
            services.TryAddSingleton<IPasswordHasher, PasswordHasher>();
            services.TryAddSingleton<ISessionManager, SessionManager>();
            services.TryAddSingleton<IOperatorProvider, OperatorProvider>();
            services.TryAddSingleton<IApplicationLock, ApplicationLock>();
            services.TryAddSingleton<IJsonTokenManager, JsonTokenManager>();
            services.AddSingleton<IBinarySerializer, JsonBinarySerializer>();
            services.TryAddSingleton<IMessageBroker, RabbitMQMessageBroker>();
            services.AddSingleton<IObjectHashGenerator, ObjectHashGenerator>();
            services.TryAddSingleton<IExcelDocumentFactory, ExcelDocumentFactory>();
            services.AddSingleton<IReportRendererService, StimulReportRendererService>();
            services.TryAddSingleton<IRouteDescriptorProvider, RouteDescriptorProvider>();
            services.TryAddSingleton<IRouteDescriptorProvider, RouteDescriptorProvider>();
            services.TryAddSingleton<IOtpAuthenticationService, OtpAuthenticationService>();
            services.TryAddSingleton<IOperationRequirementService, OperationRequirementService>();
            services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, JwtBearerOptionsConfiguration>();

            if (useRouteAuthorization)
            {
                services.TryAddEnumerable(ServiceDescriptor.Scoped<IAuthorizationHandler, RouteAuthorizationHandler>());
            }

            //Messaging
            services.AddHostedService<MessagingHostedService>();
            services.TryAddSingleton<IMessageSender, DefaultMessageSender>();
            services.Configure<MessageBalancerOptions>(configuration.GetSection(nameof(MessageBalancerOptions)));
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IMessageServiceProviderFactory, EmailMessageServiceProviderFactory>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IMessageServiceProviderFactory, CandooMessageServiceProviderFactory>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IMessageServiceProviderFactory, AsiatechMessageServiceProviderFactory>());

            return services;
        }

        public static IServiceCollection AddMediator(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            services.TryAddScoped<IMediator, Mediator>();

            var requestHandlers = assemblies
                .SelectMany(e => e.DefinedTypes)
                .Where(IsRequestHandler);

            foreach (var handlerType in requestHandlers)
            {
                var @interface = handlerType
                    .GetInterfaces()
                    .First(t => t.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

                services.TryAddScoped(@interface, handlerType);
            }

            var notificationHandlers = assemblies
                .SelectMany(e => e.DefinedTypes)
                .Where(IsNotificationHandler);

            foreach (var handlerType in notificationHandlers)
            {
                var @interface = handlerType
                    .GetInterfaces()
                    .First(t => t.GetGenericTypeDefinition() == typeof(INotificationHandler<>));

                services.AddScoped(@interface, handlerType);
            }

            return services;
        }

        private static void ConfigPolicy(AuthorizationPolicyBuilder builder, string scheme)
        {
            builder.AuthenticationSchemes = [scheme];
            builder.RequireAuthenticatedUser()
                .RequireClaim(JwtTokenPayloads.ApplicationClaim);
        }

        private static bool IsRequestHandler(Type type)
        {
            return type.IsClass &&
                !type.IsAbstract &&
                type.GetInterfaces()
                    .Any(i => i.IsGenericType && !i.IsGenericTypeDefinition && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));
        }

        private static bool IsNotificationHandler(Type type)
        {
            return type.IsClass &&
                !type.IsAbstract &&
                type.GetInterfaces()
                    .Any(i => i.IsGenericType && !i.IsGenericTypeDefinition && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>));
        }

        public static IServiceCollection AddFundCoreInfrastructure(this IServiceCollection services)
        {
            services.TryAddSingleton<IApplicationLock, ApplicationLock>();

            return services;
        }
    }
}
