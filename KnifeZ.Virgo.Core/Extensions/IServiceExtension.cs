using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnifeZ.Extensions.DatabaseAccessor;
using KnifeZ.Virgo.Core.Support.FileHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace KnifeZ.Virgo.Core.Extensions
{
    public static class IServiceExtension
    {

        public static IServiceCollection AddVirgoContextForConsole (this IServiceCollection services, string jsonFileDir = null, string jsonFileName = null, Func<IVirgoFileHandler, string> fileSubDirSelector = null)
        {
            var configBuilder = new ConfigurationBuilder();
            IConfigurationRoot ConfigRoot = configBuilder.VirgoConfig(null, jsonFileDir, jsonFileName).Build();
            var VirgoConfigs = ConfigRoot.Get<Configs>();
            services.Configure<Configs>(ConfigRoot);
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConfiguration(ConfigRoot.GetSection("Logging"))
                       .AddConsole()
                       .AddDebug()
                       .AddVirgoLogger();
            });
            var gd = GetGlobalData();
            services.AddHttpContextAccessor();
            services.AddSingleton(gd);
            VirgoFileProvider._subDirFunc = fileSubDirSelector;
            services.TryAddScoped<IDataContext, NullContext>();
            services.AddScoped<VirgoContext>();
            services.AddScoped<VirgoFileProvider>();
            services.AddHttpClient();
            if (VirgoConfigs.Domains != null)
            {
                foreach (var item in VirgoConfigs.Domains)
                {
                    services.AddHttpClient(item.Key, x =>
                    {
                        x.BaseAddress = new Uri(item.Value.Url);
                        x.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                        x.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
                    });
                }
            }
            services.AddDistributedMemoryCache();
            var cs = VirgoConfigs.Connections;
            foreach (var item in cs)
            {
                var dc = item.CreateDC();
                dc.Database.EnsureCreated();
            }
            VirgoFileProvider.Init(VirgoConfigs, gd);
            return services;
        }

        private static GlobalData GetGlobalData ()
        {
            GlobalData gd = new GlobalData
            {
                AllAssembly = Utils.GetAllAssembly()
            };
            return gd;
        }
    }
}
