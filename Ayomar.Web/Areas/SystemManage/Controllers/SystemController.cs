using Ayomar.Core.Model;
using Ayomar.Service;
using Ayomar.Web.Areas.BaseControllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Ayomar.Web.Areas.SystemManage.Controllers
{
    [Area("SystemManage")]
    public class SystemController : BaseController
    {
        public readonly ISysSystemService sysSystemService;

        public SystemController(ISysSystemService sysSystemService)
        {
            this.sysSystemService = sysSystemService;
        }

        [ManageAuthorize(ModuleAlias = "System", Operatinos = "Save")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取系统配置
        /// </summary>
        /// <param name="_sysSystemConfigService"></param>
        /// <returns></returns>
        [HttpGet]
        [ManageAuthorize(ModuleAlias = "System", Operatinos = "View")]
        public async Task<object> GetAsync()
        {
            return Json(await sysSystemService.GetAsync(p => p.GUID == "00000000-0000-0000-0000-000000000000"));
        }

        /// <summary>
        /// 修改系统配置
        /// </summary>
        /// <param name="_sysSystemConfigService"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "System", Operatinos = "Save")]
        public async Task<object> PutAsync(SysSystem entity)
        {
            if (ModelState.IsValid)
            {
                var json = new ResJson() { success = false };

                try
                {
                    entity.Date = DateTime.Now;

                    if (await sysSystemService.UpdateAsync(entity))
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
            else
                return BadRequest();
        }
    }
}
