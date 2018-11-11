using Ayomar.Web.Areas.BaseControllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;

namespace Ayomar.Web.Areas.SystemManage.Controllers
{
    [Area("SystemManage")]
    public class HomeController : BaseController
    {
        [ManageAuthorize(ModuleAlias = "NoVerify")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取管理员菜单Json
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetManageMenus()
        {
            //获取管理菜单
            StringBuilder sbMenu = new StringBuilder();
            var modules = AdminUser.Modules;
            sbMenu.Append("[");
            //一级菜单
            var spaces = modules.FindAll(p => p.Levels == 0).OrderBy(p => p.DisplayOrder);
            foreach (var space in spaces)
            {
                sbMenu.Append("{\"Id\":\"" + space.Alias + "\",\"Guid\":\"" + space.GUID + "\",\"Title\":\"" + space.Title + "\",\"Icon\":\"" + space.Icon + "\",\"Menus\":[");

                //二级菜单
                var manages = modules.FindAll(p => p.Levels == 1 && p.Parent_GUID == space.GUID).OrderBy(p => p.DisplayOrder);
                foreach (var manage in manages)
                {
                    sbMenu.Append("{\"Id\":\"" + manage.Alias + "\",\"Guid\":\"" + manage.GUID + "\",\"Title\":\"" + manage.Title + "\",\"Icon\":\"" + manage.Icon + "\",\"parent\":\"" + space.Alias + "\",\"Items\":[");

                    //三级菜单
                    var menus = modules.FindAll(p => p.Levels == 2 && p.Parent_GUID == manage.GUID).OrderBy(p => p.DisplayOrder);
                    foreach (var menu in menus)
                    {
                        sbMenu.Append("{\"Id\":\"" + menu.Alias + "\",\"Guid\":\"" + menu.GUID + "\",\"Title\":\"" + menu.Title + "\",\"Icon\":\"" + menu.Icon + "\",\"parent\":\"" + manage.Alias + "\",\"url\":\"" + menu.ModulePath + "\"},");
                    }
                    sbMenu.Length -= 1;
                    sbMenu.Append("]},");
                }
                sbMenu.Length -= 1;
                sbMenu.Append("]},");
            }
            sbMenu.Length -= 1;
            sbMenu.Append("]");

            return Json(Newtonsoft.Json.JsonConvert.DeserializeObject(sbMenu.ToString()));
        }
    }
}
