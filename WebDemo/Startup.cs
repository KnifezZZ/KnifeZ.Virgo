using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Core.Json;
using KnifeZ.Virgo.Mvc;
using KnifeZ.Virgo.Mvc.Binders;
using KnifeZ.Virgo.Mvc.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace WebDemo
{
    public class Startup
    {
        public IConfigurationRoot ConfigRoot { get; }
        public Startup (IWebHostEnvironment env)
        {
            var configBuilder = new ConfigurationBuilder();
            ConfigRoot = configBuilder.VirgoConfig(env).Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services)
        {

            services.AddDistributedMemoryCache();
            services.AddVirgoAuthentication();
            services.AddVirgoCrossDomain();
            services.AddVirgoHttpClient();
            services.AddVirgoSwagger(new OpenApiInfo
            {
                Title = "WebDemo Api",
                Version = "v1"
            });
            services.AddVirgoSession(3600);
            services.AddVirgoMultiLanguages();

            services.AddMvc(options =>
            {
                // ModelBinderProviders
                options.ModelBinderProviders.Insert(0, new StringBinderProvider());

                options.EnableEndpointRouting = false;
                // Filters
                //options.Filters.Add(new AuthorizeFilter());
                options.Filters.Add(new DataContextFilter());
                options.Filters.Add(new PrivilegeFilter());
                options.Filters.Add(new FrameworkFilter());

                options.ModelBindingMessageProvider.SetValueIsInvalidAccessor((x) => KnifeZ.Virgo.Core.Program._localizer["ValueIsInvalidAccessor", x]);
                options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => KnifeZ.Virgo.Core.Program._localizer["AttemptedValueIsInvalidAccessor", x, y]);
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor((x) => KnifeZ.Virgo.Core.Program._localizer["ValueIsInvalidAccessor", x]);
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All);
                options.JsonSerializerOptions.NumberHandling =
                    JsonNumberHandling.AllowReadingFromString |
                    JsonNumberHandling.WriteAsString;
                //options.JsonSerializerOptions.Converters.Add(new StringIgnoreLTGTConverter());
                options.JsonSerializerOptions.Converters.Add(new DateRangeConverter());
            })
            .SetCompatibilityVersion(CompatibilityVersion.Latest)
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
                options.InvalidModelStateResponseFactory = (a) =>
                {
                    return new BadRequestObjectResult(a.ModelState.GetErrorJson());
                };
            })
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddVirgoDataAnnotationsLocalization(typeof(Program));

            services.AddVirgoContext(ConfigRoot, x => x.DataPrivileges = DataPrivilegeSettings());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebDemo API V1");
                });
            }
            app.UseStaticFiles();
            app.UseFileServer();
            //自动跳转https
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseSession();


            app.UseVirgoMultiLanguages();
            app.UseVirgoCrossDomain();

            app.UseVirgoSwagger(false);
            app.UseVirgo();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseVirgoContext();
        }
        /// <summary>
        /// Set data privileges that system supports
        /// 设置系统支持的数据权限
        /// </summary>
        /// <returns>data privileges list</returns>
        public List<IDataPrivilege> DataPrivilegeSettings ()
        {
            List<IDataPrivilege> pris = new List<IDataPrivilege>();
            //Add data privilege to specific type
            //指定哪些模型需要数据权限
            pris.Add(new DataPrivilegeInfo<FrameworkDomain>("域权限", m => m.Name));
            return pris;
        }
    }
}
