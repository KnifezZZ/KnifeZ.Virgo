using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Mvc;
using KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkUserVms;
using KnifeZ.Virgo.Mvc.Auth;

namespace KnifeZ.Virgo.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ApiController]
    [Route("api/_[controller]")]
    [ActionDescription("Login")]
    [AllRights]
    public class AccountController : BaseApiController
    {
        private readonly ILogger _logger;
        private readonly ITokenService _authService;
        public AccountController (
            ILogger<AccountController> logger,
            ITokenService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Login ([FromForm] string userid, [FromForm] string password, [FromForm] bool rememberLogin = false, [FromForm] bool cookie = true)
        {

            var user = await KnifeVirgo.LoadUserFromDB(null, userid, password);

            //如果没有找到则输出错误
            if (user == null)
            {
                return BadRequest(Localizer["LoginFailed"].Value);
            }
            KnifeVirgo.LoginUserInfo = user;

            if (cookie) // cookie auth
            {
                AuthenticationProperties properties = null;
                if (rememberLogin)
                {
                    properties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30))
                    };
                }

                var principal = KnifeVirgo.LoginUserInfo.CreatePrincipal();
                // 在上面注册AddAuthentication时，指定了默认的Scheme，在这里便可以不再指定Scheme。
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
                List<SimpleMenu> ms = new List<SimpleMenu>();
                LoginUserInfo forapi = new LoginUserInfo();
                forapi.Id = user.Id;
                forapi.ITCode = user.ITCode;
                forapi.Name = user.Name;
                forapi.Roles = user.Roles;
                forapi.Groups = user.Groups;
                forapi.PhotoId = user.PhotoId;
                var menus = KnifeVirgo.DC.Set<FunctionPrivilege>()
                    .Where(x => x.UserId == user.Id || (x.RoleId != null && user.Roles.Select(x => x.ID).Contains(x.RoleId.Value)))
                    .Select(x => x.MenuItem)
                    .Where(x => x.MethodName == null)
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new SimpleMenu
                    {
                        Id = x.ID.ToString().ToLower(),
                        ParentId = x.ParentId.ToString().ToLower(),
                        Text = x.PageName,
                        Url = x.Url,
                        Icon = x.ICon
                    }).ToList();
                LocalizeMenu(menus);
                ms.AddRange(menus);

                List<string> urls = new List<string>();
                urls.AddRange(KnifeVirgo.DC.Set<FunctionPrivilege>()
                    .Where(x => x.UserId == user.Id || (x.RoleId != null && user.Roles.Select(x => x.ID).Contains(x.RoleId.Value)))
                    .Select(x => x.MenuItem)
                    .Where(x => x.MethodName != null)
                    .Select(x => x.Url)
                    );
                urls.AddRange(KnifeVirgo.GlobaInfo.AllModule.Where(x => x.IsApi == true).SelectMany(x => x.Actions).Where(x => (x.IgnorePrivillege == true || x.Module.IgnorePrivillege == true) && x.Url != null).Select(x => x.Url));
                forapi.Attributes = new Dictionary<string, object>();
                forapi.Attributes.Add("Menus", menus);
                forapi.Attributes.Add("Actions", urls);

                return Ok(forapi);
            }
            else // jwt auth
            {
                var authService = HttpContext.RequestServices.GetService(typeof(ITokenService)) as ITokenService;

                var token = await authService.IssueTokenAsync(KnifeVirgo.LoginUserInfo);
                return Content(JsonSerializer.Serialize(token), "application/json");
            }
        }


        private void LocalizeMenu (List<SimpleMenu> menus)
        {
            if (menus == null)
            {
                return;
            }
            //循环所有菜单项
            foreach (var menu in menus)
            {
                if (menu.Text?.StartsWith("MenuKey.") == true)
                {
                    menu.Text = Localizer[menu.Text];
                }
            }
        }


        [HttpPost("[action]")]
        [AllRights]
        [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
        public async Task<Token> RefreshToken (string refreshToken)
        {
            return await _authService.RefreshTokenAsync(refreshToken);
        }

        [AllRights]
        [HttpGet("[action]")]
        public IActionResult CheckUserInfo ()
        {
            if (KnifeVirgo.LoginUserInfo == null)
            {
                return BadRequest();
            }
            else
            {
                var forapi = new LoginUserInfo();
                forapi.Id = KnifeVirgo.LoginUserInfo.Id;
                forapi.ITCode = KnifeVirgo.LoginUserInfo.ITCode;
                forapi.Name = KnifeVirgo.LoginUserInfo.Name;
                forapi.Roles = KnifeVirgo.LoginUserInfo.Roles;
                forapi.Groups = KnifeVirgo.LoginUserInfo.Groups;
                forapi.PhotoId = KnifeVirgo.LoginUserInfo.PhotoId;

                var ms = new List<SimpleMenu>();
                var roleIDs = KnifeVirgo.LoginUserInfo.Roles.Select(x => x.ID).ToList();
                var data = KnifeVirgo.DC.Set<FrameworkMenu>().Where(x => x.MethodName == null).ToList();
                var topdata = data.Where(x => x.ParentId == null && x.ShowOnMenu).ToList().FlatTree(x => x.DisplayOrder).Where(x => (x.IsInside == false || x.FolderOnly == true || string.IsNullOrEmpty(x.MethodName)) && x.ShowOnMenu).ToList();
                var allowed = KnifeVirgo.DC.Set<FunctionPrivilege>()
                                .AsNoTracking()
                                .Where(x => x.UserId == KnifeVirgo.LoginUserInfo.Id || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                                .Select(x => new { x.MenuItem.ID, x.MenuItem.Url })
                                .ToList();

                var allowedids = allowed.Select(x => x.ID).ToList();
                foreach (var item in topdata)
                {
                    if (allowedids.Contains(item.ID))
                    {
                        ms.Add(new SimpleMenu
                        {
                            Id = item.ID.ToString().ToLower(),
                            ParentId = item.ParentId?.ToString()?.ToLower(),
                            Text = item.PageName,
                            Url = item.Url,
                            Icon = item.ICon,
                            Name = item.FolderOnly ?item.PageName: Utils.ToFirstLower(item.ClassName.Split(',')[1])
                        });
                    }
                }

                LocalizeMenu(ms);

                List<string> urls = new List<string>();
                urls.AddRange(allowed.Select(x => x.Url).Distinct());
                urls.AddRange(KnifeVirgo.GlobaInfo.AllModule.Where(x => x.IsApi == true).SelectMany(x => x.Actions).Where(x => (x.IgnorePrivillege == true || x.Module.IgnorePrivillege == true) && x.Url != null).Select(x => x.Url));
                forapi.Attributes = new Dictionary<string, object>();
                forapi.Attributes.Add("Menus", ms);
                forapi.Attributes.Add("Actions", urls);
                return Ok(forapi);
            }
        }


        [AllRights]
        [HttpPost("[action]")]
        public IActionResult ChangePassword (ChangePasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                vm.DoChange();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.GetErrorJson());
                }
                else
                {
                    return Ok();
                }
            }

        }

        [AllRights]
        [HttpGet("[action]/{id}")]
        public async Task Logout (Guid? id)
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Response.Redirect("/");
        }

    }

    public class SimpleMenu
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public string Name { get; set; }
    }
}
