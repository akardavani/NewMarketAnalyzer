using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MarketAnalyzer.Api.Infrastructure
{
    internal class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
        /// </summary>
        public ConfigureSwaggerOptions()
        {
        }

        public void Configure(SwaggerGenOptions options)
        {
            options.DescribeAllParametersInCamelCase();

            options.MapType<FileResult>(() => new OpenApiSchema()
            {
                Type = "string",
                Format = "binary"
            });

            options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Fund API V1", Version = "v1.0" });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                    "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                    "Example: \"Bearer 12345abcdef\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            options.CustomSchemaIds(DefaultSchemaIdSelector);

            options.MapType(typeof(IFormFile), () =>
                new OpenApiSchema { Type = "file", Format = "binary" });
            options.MapType(typeof(IFormFileCollection), () =>
                new OpenApiSchema { Type = "file", Format = "binary" });
        }

        static string DefaultSchemaIdSelector(Type type)
        {
            if (type.IsNested)
            {
                return DefaultSchemaIdSelector(type.DeclaringType) + type.Name;
            }

            if (!type.IsConstructedGenericType)
            {
                return type.Name.Replace("[]", "Array");
            }

            var prefix = type
                .GetGenericArguments()
                .Select(DefaultSchemaIdSelector)
                .Aggregate((previous, current) => previous + current);

            return prefix + type.Name.Split('`').First();
        }
    }
}
