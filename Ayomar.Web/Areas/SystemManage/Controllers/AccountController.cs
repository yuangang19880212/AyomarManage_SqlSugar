using Ayomar.Core.Model;
using Ayomar.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Ayomar.Web.Areas.SystemManage.Controllers
{
    [Area("SystemManage")]
    public class AccountController : Controller
    {
        private readonly ISysAdminService sysAdminService;

        public AccountController(ISysAdminService sysAdminService)
        {
            this.sysAdminService = sysAdminService;
        }

        public IActionResult Index()
        {
            //Remove Session Authentication  and Cookie Authentication 
            HttpContext.Session.Remove("AdminsAuthentication");
            HttpContext.Response.Cookies.Delete("CookiesAuthentication");
            return View();
        }


        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="_Systemservices"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult ValidateAccount(LoginData entity)
        {
            var json = new ResJson() { success = false };

            if (ModelState.IsValid)
            {
                var Admins = sysAdminService.ValidateLogin(entity.account, entity.password);
                if (Admins != null)
                {
                    if (Admins.User.IsCanLogin)
                    {
                        json.success = true;
                        json.returnUrl = "/systemmanage/home";

                        // Add Session Authentication
                        HttpContext.Session.Set("AdminsAuthentication", Common.ProtobufHelper.ProtobufHelper.Serialize(Admins));

                        //Add Cookie Authentication 
                        HttpContext.Response.Cookies.Append("CookiesAuthentication", new Common.CryptHelper.AESCrypt().Encrypt("{\"account\":\"" + entity.account +
                                                 "\",\"password\":\"" + entity.password + "\"}"), new CookieOptions() { Expires = DateTime.Now + TimeSpan.FromDays(3) });
                    }
                    else
                        json.message = "账号已锁定，禁止登录";

                }
                else
                    json.message = "验证失败：账号和密码不正确！";
            }
            else
                json.message = "验证失败";

            return Json(json);
        }
    }

    public class LoginData
    {
        public string account { get; set; }
        public string password { get; set; }
    }
}
