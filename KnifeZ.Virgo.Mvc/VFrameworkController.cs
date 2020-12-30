using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;
using Microsoft.AspNetCore.Mvc;

namespace KnifeZ.Virgo.Mvc
{
    [AllRights]
    [ActionDescription("VFramework")]
    [Route("api/_[controller]")]
    public class VFrameworkController : BaseApiController
    {


    }
}
