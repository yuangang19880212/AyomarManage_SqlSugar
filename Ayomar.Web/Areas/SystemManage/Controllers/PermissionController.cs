using Ayomar.Core.Model;
using Ayomar.Service;
using Ayomar.Web.Areas.BaseControllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ayomar.Web.Areas.SystemManage.Controllers
{
    [Area("SystemManage")]
    public class PermissionController : BaseController
    {
        private readonly ISysPermissionService sysPermissionService;

        public PermissionController(ISysPermissionService sysPermissionService)
        {
            this.sysPermissionService = sysPermissionService;
        }

        [ManageAuthorize(ModuleAlias = "Permission", Operatinos = "View")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 添加修改
        /// </summary>
        /// <param name="_sysSystemConfigService"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "Permission", Operatinos = "Save")]
        public async Task<object> PutAsync(SysPermissions entity)
        {
            var json = new ResJson() { success = false };

            try
            {
                bool IsSave = string.IsNullOrEmpty(entity.GUID);

                if (IsSave)
                {
                    // 权限动作不能重复
                    if (await sysPermissionService.IsAnyAsync(p => p.PermissionValue.ToLower() == entity.PermissionValue.ToLower() && p.Module_GUID == entity.Module_GUID))
                    {
                        json.message = "权限值不能重复";
                        return json;
                    }

                    // Add 初始参数
                    entity.CreateUser = AdminUser.User.Account;
                    entity.CreateDate = DateTime.Now;
                    entity.GUID = Guid.NewGuid().ToString();
                }

                // Add、Update 默认参数
                entity.UpdateUser = AdminUser.User.Account;
                entity.UpdateDate = DateTime.Now;

                //保存权限
                if (await sysPermissionService.SaveOrUpdateAsync(entity,IsSave))
                {

                    json.message = "操作成功！";
                    json.success = true;
                }
                else
                    json.message = "操作失败！";              
            }
            catch (Exception ex)
            {
                json.message = "网络超时.";                
                throw ex;
            }

            return json;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [ManageAuthorize(ModuleAlias = "Permission", Operatinos = "Delete")]
        [HttpPost]
        public async Task<object> Deletes(string[] values)
        {
            var json = new ResJson() { success = false };

            try
            {
                if (values != null && values.Length > 0)
                {
                    if (await sysPermissionService.DeleteAsync(values))
                    {
                        json.message = "删除成功！";
                        json.success = true;
                    }
                    else { json.message = "删除失败!"; }

                }
                else
                    json.message = "删除失败!";
            }
            catch (Exception ex)
            {
                json.message = "网络超时.";             
                throw ex;
            }

            return json;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "Permission", Operatinos = "Detail")]
        public async Task<object> GetAllAsync(string Module_GUID)
        {
            return Json(await sysPermissionService.GetAllAsync(p => p.Module_GUID == Module_GUID));           
        }

        /// <summary>
        /// 获取所有模块数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ManageAuthorize(ModuleAlias = "Module", Operatinos = "View")]
        public async Task<object> GetMenus([FromServices]ISysModuleService sysModuleService)
        {
            StringBuilder sbMenu = new StringBuilder();
            var modules = await sysModuleService.GetAllAsync(p => p.GUID != null);
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
