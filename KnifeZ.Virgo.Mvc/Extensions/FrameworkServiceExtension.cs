using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Auth;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Core.Support.FileHandlers;
using KnifeZ.Virgo.Core.Support.Json;
using KnifeZ.Virgo.Mvc.Auth;
using KnifeZ.Virgo.Mvc.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SpaServices.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace KnifeZ.Virgo.Mvc
{
    public static class FrameworkServiceExtension
    {

        private static Configs _virgoconfigs;
        private static Configs VirgoConfigs
        {
            get
            {
                if (_virgoconfigs == null)
                {
                    ConfigurationBuilder cb = new ConfigurationBuilder();
                    _virgoconfigs = cb.VirgoConfig(null).Build().Get<Configs>();
                }
                return _virgoconfigs;
            }
        }




        private static GlobalData GetGlobalData ()
        {
            var gd = new GlobalData();

            return gd;
        }

        private static List<SimpleMenu> GetAllMenus (List<SimpleModule> allModule, Configs ConfigInfo)
        {
            var localizer = new ResourceManagerStringLocalizerFactory(Options.Create<LocalizationOptions>(new LocalizationOptions { ResourcesPath = "Resources" }), new Microsoft.Extensions.Logging.LoggerFactory()).Create(typeof(KnifeZ.Virgo.Core.CoreProgram));
            var menus = new List<SimpleMenu>();

            if (ConfigInfo.IsQuickDebug)
            {
                menus = new List<SimpleMenu>();
                var areas = allModule.Where(x => x.NameSpace != "KnifeZ.Virgo.Admin.Api").Select(x => x.Area?.AreaName).Distinct().ToList();
                foreach (var area in areas)
                {
                    var modelmenu = new SimpleMenu
                    {
                        ID = Guid.NewGuid(),
                        PageName = area ?? localizer["Sys.DefaultArea"]
                    };
                    menus.Add(modelmenu);
                    var pages = allModule.Where(x => x.NameSpace != "KnifeZ.Virgo.Admin.Api" && x.Area?.AreaName == area).SelectMany(x => x.Actions).Where(x => x.MethodName.ToLower() == "index").ToList();
                    foreach (var page in pages)
                    {
                        var url = page.Url;
                        menus.Add(new SimpleMenu
                        {
                            ID = Guid.NewGuid(),
                            ParentId = modelmenu.ID,
                            PageName = page.Module.ActionDes == null ? page.Module.ModuleName : page.Module.ActionDes.Description,
                            Url = url
                        });
                    }
                }
            }
            else
            {
                try
                {
                    using (var dc = ConfigInfo.Connections.Where(x => x.Key.ToLower() == "default").FirstOrDefault().CreateDC())
                    {
                        menus.AddRange(dc?.Set<FrameworkMenu>()
                                .OrderBy(x => x.DisplayOrder)
                                .Select(x => new SimpleMenu
                                {
                                    ID = x.ID,
                                    ParentId = x.ParentId,
                                    PageName = x.PageName,
                                    Url = x.Url,
                                    DisplayOrder = x.DisplayOrder,
                                    ShowOnMenu = x.ShowOnMenu,
                                    Icon = x.ICon,
                                })
                                .ToList());
                    }
                }
                catch { }
            }
            return menus;
        }

        /// <summary>
        /// 获取所有模块
        /// </summary>
        /// <param name="controllers"></param>
        /// <returns></returns>
        private static List<SimpleModule> GetAllModules (List<Type> controllers)
        {
            var modules = new List<SimpleModule>();

            foreach (var ctrl in controllers)
            {
                var pubattr1 = ctrl.GetCustomAttributes(typeof(PublicAttribute), false);
                var pubattr12 = ctrl.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                var rightattr = ctrl.GetCustomAttributes(typeof(AllRightsAttribute), false);
                var debugattr = ctrl.GetCustomAttributes(typeof(DebugOnlyAttribute), false);
                var areaattr = ctrl.GetCustomAttributes(typeof(AreaAttribute), false);
                var model = new SimpleModule
                {
                    ClassName = ctrl.Name.Replace("Controller", string.Empty)
                };
                if (ctrl.Namespace == "KnifeZ.Virgo.Mvc")
                {
                    continue;
                }
                if (areaattr.Length == 0 && model.ClassName == "Home")
                {
                    continue;
                }
                if(areaattr.Length==0&& model.ClassName == "Account")
                {
                    continue;
                }
                if (pubattr1.Length > 0 || pubattr12.Length > 0 || rightattr.Length > 0 || debugattr.Length > 0)
                {
                    model.IgnorePrivillege = true;
                }
                if (typeof(BaseApiController).IsAssignableFrom(ctrl))
                {
                    model.IsApi = true;
                }
                model.NameSpace = ctrl.Namespace;
                //获取controller上标记的ActionDescription属性的值
                var attrs = ctrl.GetCustomAttributes(typeof(ActionDescriptionAttribute), false);
                if (attrs.Length > 0)
                {
                    var ada = attrs[0] as ActionDescriptionAttribute;
                    var nameKey = ada.GetDescription(ctrl);
                    model.ModuleName = nameKey;
                    ada.SetLoccalizer(ctrl);
                    model.ActionDes = ada;
                }
                else
                {
                    model.ModuleName = model.ClassName;
                }
                //获取该controller下所有的方法
                var methods = ctrl.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
                //过滤掉/Login/Login方法和特殊方法
                if (model.ClassName.ToLower() == "login")
                {
                    methods = methods.Where(x => x.IsSpecialName == false && x.Name.ToLower() != "login").ToArray();
                }
                else
                {
                    methods = methods.Where(x => x.IsSpecialName == false).ToArray();
                }
                model.Actions = new List<SimpleAction>();
                //循环所有方法
                foreach (var method in methods)
                {
                    var pubattr2 = method.GetCustomAttributes(typeof(PublicAttribute), false);
                    var pubattr22 = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                    var arattr2 = method.GetCustomAttributes(typeof(AllRightsAttribute), false);
                    var debugattr2 = method.GetCustomAttributes(typeof(DebugOnlyAttribute), false);
                    var postAttr = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
                    //如果不是post的方法，则添加到controller的action列表里
                    if (postAttr.Length == 0)
                    {
                        var action = new SimpleAction
                        {
                            Module = model,
                            MethodName = method.Name
                        };
                        if (pubattr2.Length > 0 || pubattr22.Length > 0 || arattr2.Length > 0 || debugattr2.Length > 0)
                        {
                            action.IgnorePrivillege = true;
                        }

                        var attrs2 = method.GetCustomAttributes(typeof(ActionDescriptionAttribute), false);
                        if (attrs2.Length > 0)
                        {
                            var ada = attrs2[0] as ActionDescriptionAttribute;
                            ada.SetLoccalizer(ctrl);
                            action.ActionDes = ada;
                        }
                        else
                        {
                            action.ActionName = action.MethodName;
                        }
                        var pars = method.GetParameters();
                        if (pars != null && pars.Length > 0)
                        {
                            action.ParasToRunTest = new List<string>();
                            foreach (var par in pars)
                            {
                                action.ParasToRunTest.Add(par.Name);
                            }
                        }
                        model.Actions.Add(action);
                    }
                }
                //再次循环所有方法
                foreach (var method in methods)
                {
                    var pubattr2 = method.GetCustomAttributes(typeof(PublicAttribute), false);
                    var pubattr22 = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                    var arattr2 = method.GetCustomAttributes(typeof(AllRightsAttribute), false);
                    var debugattr2 = method.GetCustomAttributes(typeof(DebugOnlyAttribute), false);

                    var postAttr = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
                    //找到post的方法且没有同名的非post的方法，添加到controller的action列表里
                    if (postAttr.Length > 0 && model.Actions.Where(x => x.MethodName.ToLower() == method.Name.ToLower()).FirstOrDefault() == null)
                    {
                        if (method.Name.ToLower().StartsWith("dobatch"))
                        {
                            if (model.Actions.Where(x => "do" + x.MethodName.ToLower() == method.Name.ToLower()).FirstOrDefault() != null)
                            {
                                continue;
                            }
                        }
                        var action = new SimpleAction
                        {
                            Module = model,
                            MethodName = method.Name
                        };
                        if (pubattr2.Length > 0 || pubattr22.Length > 0 || arattr2.Length > 0 || debugattr2.Length > 0)
                        {
                            action.IgnorePrivillege = true;
                        }
                        var attrs2 = method.GetCustomAttributes(typeof(ActionDescriptionAttribute), false);
                        if (attrs2.Length > 0)
                        {
                            var ada = attrs2[0] as ActionDescriptionAttribute;
                            ada.SetLoccalizer(ctrl);
                            action.ActionDes = ada;
                        }
                        else
                        {
                            action.ActionName = action.MethodName;
                        }
                        var pars = method.GetParameters();
                        if (pars != null && pars.Length > 0)
                        {
                            action.ParasToRunTest = new List<string>();
                            foreach (var par in pars)
                            {
                                action.ParasToRunTest.Add(par.Name);
                            }
                        }
                        model.Actions.Add(action);
                    }
                }
                if (model.Actions != null && model.Actions.Count > 0)
                {
                    if (areaattr.Length > 0)
                    {
                        string areaName = (areaattr[0] as AreaAttribute).RouteValue;
                        var existArea = modules.Where(x => x.Area?.AreaName == areaName).Select(x => x.Area).FirstOrDefault();
                        if (existArea == null)
                        {
                            model.Area = new SimpleArea
                            {
                                AreaName = (areaattr[0] as AreaAttribute).RouteValue,
                                Prefix = (areaattr[0] as AreaAttribute).RouteValue,
                            };
                        }
                        else
                        {
                            model.Area = existArea;
                        }
                    }
                    modules.Add(model);
                }
            }

            return modules;
        }

        /// <summary>
        /// 获取所有无需权限验证的链接
        /// </summary>
        /// <param name="controllers"></param>
        /// <returns></returns>
        private static List<string> GetAllAccessUrls (List<Type> controllers)
        {
            var rv = new List<string>();
            foreach (var ctrl in controllers)
            {
                if (typeof(BaseApiController).IsAssignableFrom(ctrl))
                {
                    continue;
                }
                var area = string.Empty;
                var ControllerName = ctrl.Name.Replace("Controller", string.Empty);
                var includeAll = false;
                //获取controller上标记的ActionDescription属性的值
                var attrs = ctrl.GetCustomAttributes(typeof(AllRightsAttribute), false);
                var attrs2 = ctrl.GetCustomAttributes(typeof(PublicAttribute), false);
                var attrs22 = ctrl.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                var areaAttr = ctrl.GetCustomAttribute(typeof(AreaAttribute), false);
                if (areaAttr != null)
                {
                    area = (areaAttr as AreaAttribute).RouteValue;
                }
                if (attrs.Length > 0 || attrs2.Length > 0 || attrs22.Length > 0)
                {
                    includeAll = true;
                }

                //获取该controller下所有的方法
                var methods = ctrl.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
                //过滤掉特殊方法
                methods = methods.Where(x => x.IsSpecialName == false).ToArray();
                var ActionName = string.Empty;
                //循环所有方法
                foreach (var method in methods)
                {
                    var postAttr = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
                    //如果不是post的方法，则添加到controller的action列表里
                    if (postAttr.Length == 0)
                    {
                        ActionName = method.Name;
                        var url = ControllerName + "/" + ActionName;
                        if (!string.IsNullOrEmpty(area))
                        {
                            url = area + "/" + url;
                        }
                        if (includeAll == true)
                        {
                            rv.Add(url);
                        }
                        else
                        {
                            var attrs3 = method.GetCustomAttributes(typeof(AllRightsAttribute), false);
                            var attrs4 = method.GetCustomAttributes(typeof(PublicAttribute), false);
                            var attrs42 = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                            if (attrs3.Length > 0 || attrs4.Length > 0 || attrs42.Length > 0)
                            {
                                rv.Add(url);
                            }
                        }
                    }
                }
                //再次循环所有方法
                foreach (var method in methods)
                {
                    var postAttr = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
                    //找到post的方法且没有同名的非post的方法，添加到controller的action列表里
                    if (postAttr.Length > 0 && !rv.Contains(ControllerName + "/" + method.Name))
                    {
                        ActionName = method.Name;
                        var url = ControllerName + "/" + ActionName;
                        if (!string.IsNullOrEmpty(area))
                        {
                            url = area + "/" + url;
                        }
                        if (includeAll == true)
                        {
                            rv.Add(url);
                        }
                        else
                        {
                            var attrs5 = method.GetCustomAttributes(typeof(AllRightsAttribute), false);
                            var attrs6 = method.GetCustomAttributes(typeof(PublicAttribute), false);
                            var attrs62 = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                            if (attrs5.Length > 0 || attrs6.Length > 0 || attrs62.Length > 0)
                            {
                                rv.Add(url);
                            }
                        }
                    }
                }
            }
            return rv;
        }

        private static Assembly GetRuntimeAssembly (string name)
        {
            var path = Assembly.GetEntryAssembly().Location;
            var library = DependencyContext.Default.RuntimeLibraries.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();
            if (library == null)
            {
                return null;
            }
            var r = new CompositeCompilationAssemblyResolver(new ICompilationAssemblyResolver[]
        {
            new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(path)),
            new ReferenceAssemblyPathResolver(),
            new PackageCompilationAssemblyResolver()
        });

            var wrapper = new CompilationLibrary(
                library.Type,
                library.Name,
                library.Version,
                library.Hash,
                library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                library.Dependencies,
                library.Serviceable);

            var assemblies = new List<string>();
            r.TryResolveAssemblyPaths(wrapper, assemblies);
            if (assemblies.Count > 0)
            {
                return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblies[0]);
            }
            else
            {
                return null;
            }
        }

        public static IServiceCollection AddVirgoContext (this IServiceCollection services, IConfigurationRoot config, Action<VirgoContextOption> options = null)
        {
            VirgoContextOption op = new VirgoContextOption();
            options?.Invoke(op);
            services.Configure<Configs>(config);
            var gd = GetGlobalData();
            services.AddHttpContextAccessor();
            services.AddSingleton(gd);
            services.AddSingleton(op.DataPrivileges ?? new List<IDataPrivilege>());
            DataContextFilter._csfunc = op.CsSelector;
            VirgoFileProvider._subDirFunc = op.FileSubDirSelector;
            VirgoContext.ReloadUserFunc = op.ReloadUserFunc;
            services.TryAddScoped<IDataContext, NullContext>();
            services.AddScoped<VirgoContext>();
            services.AddScoped<VirgoFileProvider>();
            services.Configure<FormOptions>(y =>
            {
                y.ValueCountLimit = 5000;
                y.ValueLengthLimit = int.MaxValue - 20480;
                y.MultipartBodyLengthLimit = VirgoConfigs.FileUploadOptions.UploadLimit;
            });
            return services;
        }
        public static IServiceCollection AddVirgoCrossDomain (this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                if (VirgoConfigs.CorsOptions?.Policy?.Count > 0)
                {
                    foreach (var item in VirgoConfigs.CorsOptions.Policy)
                    {
                        string[] domains = item.Domain?.Split(',');
                        options.AddPolicy(item.Name,
                           builder =>
                           {
                               builder.WithOrigins(domains)
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                           });
                    }
                }
                else
                {
                    options.AddPolicy("_donotusedefault",
                        builder =>
                        {
                            builder.SetIsOriginAllowed((a) => true)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                        });
                }
            });
            return services;
        }
        public static IServiceCollection AddVirgoSession (this IServiceCollection services, int timeout)
        {
            services.AddSession(options =>
            {
                options.Cookie.Name = VirgoConfigs.CookiePre + ".Session";
                options.IdleTimeout = TimeSpan.FromSeconds(timeout);
            });
            return services;
        }
        public static IServiceCollection AddVirgoAuthentication (this IServiceCollection services)
        {
            services.AddSingleton<ITokenService, TokenService>();

            var jwtOptions = VirgoConfigs.JwtOption;

            var cookieOptions = VirgoConfigs.CookieOption;

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            NameClaimType = AuthConstants.JwtClaimTypes.Name,
                            RoleClaimType = AuthConstants.JwtClaimTypes.Role,

                            ValidateIssuer = true,
                            ValidIssuer = jwtOptions.Issuer,

                            ValidateAudience = true,
                            ValidAudience = jwtOptions.Audience,

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey)),

                            ValidateLifetime = true
                        };
                    })
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.Cookie.Name = CookieAuthenticationDefaults.CookiePrefix + AuthConstants.CookieAuthName;
                        options.Cookie.HttpOnly = true;
                        options.Cookie.SameSite = SameSiteMode.Strict;

                        options.ClaimsIssuer = cookieOptions.Issuer;
                        options.SlidingExpiration = cookieOptions.SlidingExpiration;
                        options.ExpireTimeSpan = TimeSpan.FromSeconds(cookieOptions.Expires);
                        // options.SessionStore = new MemoryTicketStore();

                        options.LoginPath = cookieOptions.LoginPath;
                        options.LogoutPath = cookieOptions.LogoutPath;
                        options.ReturnUrlParameter = cookieOptions.ReturnUrlParameter;
                        options.AccessDeniedPath = cookieOptions.AccessDeniedPath;
                    });
            return services;
        }

        public static IServiceCollection AddVirgoHttpClient (this IServiceCollection services)
        {
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
            return services;
        }

        public static IServiceCollection AddVirgoSwagger (this IServiceCollection services,OpenApiInfo apiInfo)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(apiInfo.Version, apiInfo);
                var bearer = new OpenApiSecurityScheme()
                {
                    Description = "JWT Bearer",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey

                };
                c.AddSecurityDefinition("Bearer", bearer);
                var sr = new OpenApiSecurityRequirement();
                sr.Add(new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }, Array.Empty<string>());
                c.AddSecurityRequirement(sr);
                c.SchemaFilter<SwaggerFilter>();
            });
            return services;
        }

        public static IServiceCollection AddVirgoMultiLanguages (this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            List<CultureInfo> supportedCultures = new List<CultureInfo>();
            var lans = VirgoConfigs.Languages.Split(",");
            foreach (var lan in lans)
            {
                supportedCultures.Add(new CultureInfo(lan));
            }

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(supportedCultures[0]);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            return services;
        }

        public static IMvcBuilder AddVirgoDataAnnotationsLocalization (this IMvcBuilder builder, Type programType)
        {
            builder.AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    //if (Core.CoreProgram.Buildindll.Any(x => type.FullName.StartsWith(x)))
                    //{
                    //    return factory.Create(typeof(KnifeZ.Virgo.Core.CoreProgram));
                    //}
                    //else
                    //{
                    return factory.Create(programType);
                    //}
                };
            });
            return builder;
        }

        public static IApplicationBuilder UseVirgoContext (this IApplicationBuilder app)
        {
            var configs = app.ApplicationServices.GetRequiredService<IOptionsMonitor<Configs>>().CurrentValue;
            var lg = app.ApplicationServices.GetRequiredService<LinkGenerator>();
            var gd = app.ApplicationServices.GetRequiredService<GlobalData>();
            var localfactory = app.ApplicationServices.GetRequiredService<IStringLocalizerFactory>();
            //获取所有程序集
            gd.AllAssembly = Utils.GetAllAssembly();

            //set Core's _Callerlocalizer to use localizer point to the EntryAssembly's Program class
            var programType = Assembly.GetCallingAssembly()?.GetTypes()?.Where(x => x.Name == "Program").FirstOrDefault();
            var coredll = gd.AllAssembly.Where(x => x.GetName().Name == "KnifeZ.Virgo.Core.dll" || x.GetName().Name == "KnifeZ.Virgo.Core").FirstOrDefault();
            var programLocalizer = localfactory.Create(programType);
            coredll.GetType("KnifeZ.Virgo.Core.CoreProgram").GetProperty("Callerlocalizer").SetValue(null, programLocalizer);


            var controllers = gd.GetTypesAssignableFrom<IBaseController>();
            gd.AllModule = GetAllModules(controllers);
            gd.AllAccessUrls = GetAllAccessUrls(controllers);

            gd.SetMenuGetFunc(() =>
            {
                var menus = new List<SimpleMenu>();
                var cache = app.ApplicationServices.GetRequiredService<IDistributedCache>();
                var menuCacheKey = "FFMenus";
                if (cache.TryGetValue(menuCacheKey, out List<SimpleMenu> rv) == false)
                {
                    var data = GetAllMenus(gd.AllModule, configs);
                    cache.Add(menuCacheKey, data, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = new TimeSpan(1, 0, 0) });
                    menus = data;
                }
                else
                {
                    menus = rv;
                }

                return menus;
            });
            foreach (var m in gd.AllModule)
            {
                foreach (var a in m.Actions)
                {
                    string u = null;
                    if (a.ParasToRunTest != null && a.ParasToRunTest.Any(x => x.ToLower() == "id"))
                    {
                        u = lg.GetPathByAction(a.MethodName, m.ClassName, new { id = 0, area = m.Area?.AreaName });
                    }
                    else
                    {
                        u = lg.GetPathByAction(a.MethodName, m.ClassName, new { area = m.Area?.AreaName });
                    }
                    if (u != null && u.EndsWith("/0"))
                    {
                        u = u.Substring(0, u.Length - 2);
                        if (m.IsApi == true)
                        {
                            u = u + "/{id}";
                        }
                    }
                    a.Url = u;
                }
            }

            var test = app.ApplicationServices.GetService<ISpaStaticFileProvider>();
            VirgoFileProvider.Init(configs, gd);
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var fixdc = scope.ServiceProvider.GetRequiredService<IDataContext>();
                if (fixdc is NullContext)
                {
                    var cs = configs.Connections;
                    foreach (var item in cs)
                    {
                        var dc = item.CreateDC();
                        dc.DataInit(gd.AllModule, true).Wait();
                    }
                }
                else
                {
                    fixdc.DataInit(gd.AllModule, true).Wait();
                }

            }
            return app;
        }
        public static IApplicationBuilder UseVirgoMultiLanguages (this IApplicationBuilder app)
        {
            var configs = app.ApplicationServices.GetRequiredService<IOptionsMonitor<Configs>>().CurrentValue;
            if (string.IsNullOrEmpty(configs.Languages) == false)
            {
                List<CultureInfo> supportedCultures = new List<CultureInfo>();
                var lans = configs.Languages.Split(",");
                foreach (var lan in lans)
                {
                    supportedCultures.Add(new CultureInfo(lan));
                }

                app.UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture(supportedCultures[0]),
                    SupportedCultures = supportedCultures,
                    SupportedUICultures = supportedCultures
                });
                System.Threading.Thread.CurrentThread.CurrentCulture = supportedCultures[0];
                System.Threading.Thread.CurrentThread.CurrentUICulture = supportedCultures[0];
            }
            return app;
        }
        public static IApplicationBuilder UseVirgoCrossDomain (this IApplicationBuilder app)
        {
            var configs = app.ApplicationServices.GetRequiredService<IOptionsMonitor<Configs>>().CurrentValue;
            if (configs.CorsOptions.EnableAll == true)
            {
                if (configs.CorsOptions?.Policy?.Count > 0)
                {
                    app.UseCors(configs.CorsOptions.Policy[0].Name);
                }
                else
                {
                    app.UseCors("_donotusedefault");
                }
            }
            return app;
        }
        public static IApplicationBuilder UseVirgoSwagger (this IApplicationBuilder app, bool showInDebugOnly = true)
        {
            var configs = app.ApplicationServices.GetRequiredService<IOptions<Configs>>().Value;
            if (configs.IsQuickDebug == true || showInDebugOnly == false)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "KnifeZ.Virgo API V1");
                });
            }
            return app;
        }

    }


}
