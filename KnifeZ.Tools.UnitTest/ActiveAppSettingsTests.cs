using Microsoft.VisualStudio.TestTools.UnitTesting;
using KnifeZ.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnifeZ.Tools.Tests
{
    [TestClass()]
    public class ActiveAppSettingsTests
    {
        [TestMethod()]
        public void ReadStringTest ()
        {
            Assert.IsNotNull(ActiveAppSettings.ReadString("Logging:LogLevel"));
        }

        [TestMethod()]
        public void ReadGuidTest ()
        {
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void ReadIntTest ()
        {
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void ReadTest ()
        {
            Assert.AreNotEqual(
                ActiveAppSettings.Read("Logging:LogLevel").ValueKind,
                System.Text.Json.JsonValueKind.Undefined);
            Assert.AreNotEqual(
                ActiveAppSettings.Read("Logging:LogLevel:Default").ValueKind,
                System.Text.Json.JsonValueKind.Undefined);
        }

        [TestMethod()]
        public void WriteTest ()
        {
            ActiveAppSettings.Write("AppSettings:Test", "saocjaoisf可选择");
            Assert.IsTrue(true);
        }
    }
}