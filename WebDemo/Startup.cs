using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Core.Json;
using KnifeZ.Virgo.Core.Support.FileHandlers;
using KnifeZ.Virgo.Mvc;
using KnifeZ.Virgo.Mvc.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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
            services.AddVirgoSession(3600);
            services.AddVirgoCrossDomain();
            services.AddVirgoAuthentication();
            services.AddVirgoHttpClient();
            services.AddVirgoSwagger(new OpenApiInfo
            {
                Title = "WebDemo Api",
                Version = "v1"
            });
            services.AddVirgoMultiLanguages();

            services.AddMvc(options =>
            {
                options.UseKnifeMvcOptions();
            })
            .AddJsonOptions(options =>
            {
                options.UseKnifeJsonOptions();
            })
            .SetCompatibilityVersion(CompatibilityVersion.Latest)
            .ConfigureApiBehaviorOptions(options =>
            {
                options.UseKnifeApiOptions();
            })
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddVirgoDataAnnotationsLocalization(typeof(Program));

            services.AddVirgoContext(ConfigRoot, options =>
            {
                options.DataPrivileges = DataPrivilegeSettings();
                options.CsSelector = CSSelector;
                options.FileSubDirSelector = SubDirSelector;
                options.ReloadUserFunc = ReloadUser;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env)
        {
            var configs = app.ApplicationServices.GetRequiredService<IOptions<Configs>>().Value;

            if (configs == null)
            {
                throw new InvalidOperationException("Can not find Configs service, make sure you call AddVirgoContext at ConfigService");
            }

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
            //�Զ���תhttps
            app.UseHttpsRedirection();


            app.UseRouting();

            app.UseVirgoMultiLanguages();
            app.UseVirgoCrossDomain();

            app.UseAuthorization();
            app.UseAuthentication();
            app.UseSession();

            app.UseVirgoSwagger(false);
            app.UseVirgo();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                   name: "areaRoute",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapFallbackToFile("index.html");
            });

            app.UseVirgoContext();
        }


        /// <summary>
        /// Wtm will call this function to dynamiclly set connection string
        /// ��ܻ���������������̬�趨ÿ�η�����Ҫ���ӵ����ݿ�
        /// </summary>
        /// <param name="context">ActionContext</param>
        /// <returns>Connection string key name</returns>
        public string CSSelector (ActionExecutingContext context)
        {
            //To override the default logic of choosing connection string,
            //change this function to return different connection string key
            //����context���ز�ͬ�������ַ���������
            return null;
        }

        /// <summary>
        /// Set data privileges that system supports
        /// ����ϵͳ֧�ֵ�����Ȩ��
        /// </summary>
        /// <returns>data privileges list</returns>
        public List<IDataPrivilege> DataPrivilegeSettings ()
        {
            List<IDataPrivilege> pris = new List<IDataPrivilege>
            {
                //Add data privilege to specific type
                //ָ����Щģ����Ҫ����Ȩ��
                new DataPrivilegeInfo<FrameworkDomain>("��Ȩ��", m => m.Name)
            };
            return pris;
        }

        /// <summary>
        /// Set sub directory of uploaded files
        /// ��̬�����ϴ��ļ�����Ŀ¼
        /// </summary>
        /// <param name="fh">IWtmFileHandler</param>
        /// <returns>subdir name</returns>
        public string SubDirSelector (IVirgoFileHandler fh)
        {
            if (fh is VirgoLocalFileHandler)
            {
                return DateTime.Now.ToString("yyyy-MM-dd");
            }
            return null;
        }

        /// <summary>
        /// Custom Reload user process when cache is not available
        /// �����Զ���ķ������¶�ȡ�û���Ϣ��������������û�����ʧЧ��ʱ�����
        /// </summary>
        /// <param name="context"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public LoginUserInfo ReloadUser (VirgoContext context, string account)
        {
            return null;
        }
    }
}
