using KnifeZ.Extensions.DatabaseAccessor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KnifeZ.Virgo.Core
{
    public static class VirgoLoggerExtensions
    {
        /// <summary>
        /// 添加一个名为“Virgo”的日志记录器
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddVirgoLogger (this ILoggingBuilder builder)
        {
            builder.AddConfiguration();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, VirgoLoggerProvider>());
            return builder;
        }
    }

    public class VirgoLoggerProvider : ILoggerProvider
    {
        private ConnectionStrings cs = null;
        private LoggerFilterOptions logConfig;

        public VirgoLoggerProvider (IOptions<Configs> _configs, IOptions<LoggerFilterOptions> _logConfig)
        {
            if (_configs.Value != null)
            {
                cs = _configs.Value.Connections.Where(x => x.Key.ToLower() == "defaultlog").FirstOrDefault();
                if (cs == null)
                {
                    cs = _configs.Value.Connections.Where(x => x.Key.ToLower() == "default").FirstOrDefault();
                }
                if (cs == null)
                {
                    cs = _configs.Value.Connections.FirstOrDefault();
                }
                logConfig = _logConfig.Value;
            }
        }

        public ILogger CreateLogger (string categoryName)
        {
            return new VirgoLogger(categoryName, cs, logConfig);
        }
        public void Dispose () { }
    }

    public class VirgoLogger : ILogger
    {
        private readonly string categoryName;
        private ConnectionStrings cs;
        private LoggerFilterOptions logConfig;

        public VirgoLogger (string categoryName, ConnectionStrings cs, LoggerFilterOptions logConfig)
        {
            this.categoryName = categoryName;
            this.cs = cs;
            this.logConfig = logConfig;
        }

        public bool IsEnabled (LogLevel logLevel)
        {
            if (logConfig == null)
            {
                return false;
            }
            var level = logConfig.Rules.Where(x =>
                x.ProviderName == "Virgo" &&
                    (
                      (x.CategoryName != null && categoryName.ToLower().StartsWith(x.CategoryName.ToLower())) ||
                      categoryName == "KnifeZ.Virgo.Core.ActionLog"
                    )
                )
                .Select(x => x.LogLevel).FirstOrDefault();
            if (level == null)
            {
                level = LogLevel.Error;
            }
            if (logLevel >= level)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Log<TState> (LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                ActionLog log = null;
                if (typeof(TState) != typeof(ActionLog))
                {
                    ActionLogTypesEnum ll = ActionLogTypesEnum.Normal;
                    if (logLevel == LogLevel.Trace || logLevel == LogLevel.Debug)
                    {
                        ll = ActionLogTypesEnum.Debug;
                    }
                    if (logLevel == LogLevel.Error || logLevel == LogLevel.Warning || logLevel == LogLevel.Critical)
                    {
                        ll = ActionLogTypesEnum.Exception;
                    }
                    log = new ActionLog
                    {
                        Remark = formatter?.Invoke(state, exception),
                        CreateTime = DateTime.Now,
                        ActionTime = DateTime.Now,
                        ActionName = "System Log",
                        ModuleName = "System Log",
                        LogType = ll
                    };
                }
                else
                {
                    log = state as ActionLog;
                }

                if (cs != null)
                {
                    using var dc = cs.CreateDC();
                    if (dc != null)
                    {
                        dc.AddEntity<ActionLog>(log);
                        dc.SaveChanges();
                    }
                }
            }
        }

        public IDisposable BeginScope<TState> (TState state) => null;
    }
}
