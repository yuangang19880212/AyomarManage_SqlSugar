using Ayomar.Core.Model;
using Ayomar.Web.Areas.BaseControllers;
using Microsoft.AspNetCore.Mvc;

namespace Ayomar.Web.Areas.Components.Controllers
{
    [Area("Components")]
    public class ValidateIdentityController : BaseController
    {       
        [HttpPost]
        public IActionResult VerffyPassword(string password)
        {
            if (new Common.CryptHelper.AESCrypt().Decrypt(AdminUser.User.SecondaryPassword).Equals(password))
                return Json(new ResJson());
            else
                return Json(new ResJson() { success = false, message = "验证失败" });
        }
    }
}
