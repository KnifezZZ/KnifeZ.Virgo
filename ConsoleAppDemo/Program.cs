using Microsoft.Extensions.Hosting;
using System;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAppDemo
{
    class Program
    {
        public static ServiceProvider Provider { get; set; }
        static void Main(string[] args)
        {
            StartUp();
            Console.WriteLine("Hello World!");

            var oc = new OtherClassDo(Provider);
            oc.DoSomething();
        }

        static void StartUp()
        {
            var services = new ServiceCollection();
            services.AddVirgoContextForConsole();
            Provider = services.BuildServiceProvider();
        }

        static VirgoContext GetVirgoContext()
        {
            var res = Provider.GetRequiredService<VirgoContext>();
            return res;
        }
    }
}
