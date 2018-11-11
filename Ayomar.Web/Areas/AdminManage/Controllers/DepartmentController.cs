using Ayomar.Core.Model;
using Ayomar.Service;
using Ayomar.Web.Areas.BaseControllers;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Threading.Tasks;

namespace Ayomar.Web.Areas.AdminManage.Controllers
{
    [Area("AdminManage")]
    public class DepartmentController : BaseController
    {
        private readonly ISysDepartmentService sysDepartmentService;

        public DepartmentController(ISysDepartmentService sysDepartmentService)
        {
            this.sysDepartmentService = sysDepartmentService;
        }

        [ManageAuthorize(ModuleAlias = "Department", Operatinos = "View")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 添加修改
        /// </summary>
        /// <param name="sysRoleServcie"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "Department", Operatinos = "Save")]
        public async Task<object> PutAsync(SysDepartments entity)
        {
            var json = new ResJson() { success = false };

            try
            {
                bool IsSave = string.IsNullOrEmpty(entity.GUID);

                if (IsSave)
                {
                    if (await sysDepartmentService.IsAnyAsync(p => p.DepartmentName == entity.DepartmentName))
                    {
                        json.message = "部门名称已存在";
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

                //保存模块
                if (await sysDepartmentService.SaveOrUpdateAsync(entity, IsSave))
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
        [ManageAuthorize(ModuleAlias = "Department", Operatinos = "Delete")]
        [HttpPost]
        public async Task<object> Deletes(string[] values)
        {
            var json = new ResJson() { success = false };

            try
            {
                if (values != null && values.Length > 0)
                {
                    if (await sysDepartmentService.DeleteAsync(values))
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
        [ManageAuthorize(ModuleAlias = "Department", Operatinos = "Detail")]
        public async Task<object> GetAllAsync()
        {
            return Json(await sysDepartmentService.GetAllAsync(p => p.GUID != null));
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "Department", Operatinos = "Detail")]
        public async Task<object> GetAllPageAsync(int page, int pagesize, string keyWords)
        {

            var exp = Expressionable.Create<SysDepartments>()
                .AndIF(!string.IsNullOrEmpty(keyWords), p => p.DepartmentName.ToLower().Contains(keyWords.ToLower())).ToExpression();

            return Json(await sysDepartmentService.GetPageAsync(page, pagesize, exp), new Newtonsoft.Json.JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd HH:mm:ss", Formatting = Newtonsoft.Json.Formatting.Indented });
        }
    }
}
