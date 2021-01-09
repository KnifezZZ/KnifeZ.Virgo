using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var virgoConfigs = ConfigRoot.Get<Configs>();
            services.Configure<Configs>(ConfigRoot);
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConfiguration(ConfigRoot.GetSection("Logging"))
                       .AddConsole()
                       .AddDebug()
                       .AddVirgoLogger();
            });
            VirgoFileProvider._subDirFunc = fileSubDirSelector;
            services.TryAddScoped<IDataContext, NullContext>();
            services.AddScoped<VirgoContext>();
            services.AddSingleton<VirgoFileProvider>();
            services.AddHttpClient();
            if (virgoConfigs.Domains != null)
            {
                foreach (var item in virgoConfigs.Domains)
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
            return services;
        }

    }
}
