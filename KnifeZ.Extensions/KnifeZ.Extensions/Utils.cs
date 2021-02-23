using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace KnifeZ.Extensions
{
    public class UtilTools
    {
        private static List<Assembly> _allAssemblies;

        public static List<Assembly> GetAllAssembly()
        {

            if (_allAssemblies == null)
            {
                _allAssemblies = new List<Assembly>();
                string path = null;
                string singlefile = null;
                try
                {
                    path = Assembly.GetEntryAssembly()?.Location;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(path))
                {
                    singlefile = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                    path = Path.GetDirectoryName(singlefile);
                }
                var dir = new DirectoryInfo(Path.GetDirectoryName(path));

                var dlls = dir.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
                string[] systemdll = new string[]
                {
                "Microsoft.",
                "System.",
                "Swashbuckle.",
                "ICSharpCode",
                "Newtonsoft.",
                "Oracle.",
                "Pomelo.",
                "SQLitePCLRaw.",
                "Aliyun.OSS",
                "COSXML.dll",
                "BouncyCastle.",
                "FreeSql.",
                "Google.Protobuf.dll",
                "Humanizer.dll",
                "IdleBus.dll",
                "K4os.",
                "MySql.Data.",
                "Npgsql.",
                "NPOI.",
                "netstandard",
                "MySqlConnector",
                "VueCliMiddleware"
                };

                var filtered = dlls.Where(x => systemdll.Any(y => x.Name.StartsWith(y)) == false);

                var dlllist = new List<Assembly>();
                foreach (var dll in filtered)
                {
                    try
                    {
                        if(!systemdll.Any(y => dll.FullName.StartsWith(y)))
                        {
                            dlllist.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(dll.FullName));
                        }
                        else
                        {
                            AssemblyLoadContext.Default.LoadFromAssemblyPath(dll.FullName);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + ex.StackTrace);
                    }
                }
                //AssemblyLoadContext.Default.Assemblies.Where(x => systemdll.Any(y => x.FullName.StartsWith(y)) == false).ToList();
                _allAssemblies.AddRange(dlllist);
            }
            return _allAssemblies;
        }

    }
}
