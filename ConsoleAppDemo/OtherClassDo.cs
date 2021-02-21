using KnifeZ.Virgo.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppDemo
{
    public class OtherClassDo
    {
        public VirgoContext vc;
        public OtherClassDo(IServiceProvider service)
        {
            vc = service.GetRequiredService<VirgoContext>();
        }

        public void DoSomething()
        {
            Console.WriteLine(vc.CurrentCS);
        }
    }
}
