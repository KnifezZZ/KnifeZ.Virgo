using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using KnifeZ.Extensions.DatabaseAccessor;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Json;
using KnifeZ.Virgo.Mvc.Binders;
using KnifeZ.Virgo.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace KnifeZ.Virgo.Mvc.Extensions
{
    public static class MvcOptionExtension
    {
        public static void UseSwaggerSecurityOptions(this SwaggerGenOptions options)
        {
            var bearer = new OpenApiSecurityScheme()
            {
                Description = "JWT Bearer",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey

            };
            options.AddSecurityDefinition("Bearer", bearer);
            var sr = new OpenApiSecurityRequirement();
            sr.Add(new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, Array.Empty<string>());
            options.AddSecurityRequirement(sr);
            options.SchemaFilter<SwaggerFilter>();
        }

        public static void UseVirgoMvcOptions (this MvcOptions options)
        {
            // ModelBinderProviders
            options.ModelBinderProviders.Insert(0, new StringBinderProvider());

            // Filters
            options.Filters.Add(new DataContextFilter());
            options.Filters.Add(new PrivilegeFilter());
            options.Filters.Add(new FrameworkFilter());
            options.ModelBindingMessageProvider.SetValueIsInvalidAccessor((x) => Core.CoreProgram.Callerlocalizer?["Sys.ValueIsInvalidAccessor", x]);
            options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => Core.CoreProgram.Callerlocalizer?["Sys.AttemptedValueIsInvalidAccessor", x, y]);
            options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor((x) => Core.CoreProgram.Callerlocalizer?["Sys.ValueIsInvalidAccessor", x]);
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(IModelStateService)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(IDataContext)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(VirgoContext)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(Configs)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(GlobalData)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(IDistributedCache)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(LoginUserInfo)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(ISessionService)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(IModelStateService)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(IStringLocalizer)));
            options.EnableEndpointRouting = true;

        }


        public static void UseVirgoJsonOptions (this JsonOptions options)
        {
            options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All);
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.IgnoreNullValues = true;
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            options.JsonSerializerOptions.Converters.Add(new StringIgnoreLTGTConverter());
            //枚举字段转string
            //options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new DateRangeConverter());
        }
        public static void UseVirgoApiOptions (this ApiBehaviorOptions options)
        {
            options.SuppressModelStateInvalidFilter = true;
            options.InvalidModelStateResponseFactory = (a) =>
            {
                return new BadRequestObjectResult(a.ModelState.GetErrorJson());
            };
        }

    }
}
