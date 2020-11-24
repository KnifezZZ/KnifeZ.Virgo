using System;
using System.Collections.Generic;
using System.Text;

namespace KnifeZ.Virgo.Core
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class ValidateFormItemOnlyAttribute : Attribute
    {
    }
}
