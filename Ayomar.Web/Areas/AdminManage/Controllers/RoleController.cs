using Ayomar.Web.Areas.BaseControllers;
using Microsoft.AspNetCore.Mvc;
using Ayomar.Service;
using System.Threading.Tasks;
using SqlSugar;
using Ayomar.Core.Model;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Ayomar.Web.Areas.AdminManage.Controllers
{
    [Area("AdminManage")]
    public class RoleController : BaseController
    {
        private readonly ISysRoleServcie sysRoleServcie;

        public RoleController(ISysRoleServcie sysRoleServcie)
        {
            this.sysRoleServcie = sysRoleServcie;
        }

        [ManageAuthorize(ModuleAlias = "Role", Operatinos = "View")]
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
        [ManageAuthorize(ModuleAlias = "Role", Operatinos = "Save")]
        public async Task<object> PutAsync(SysRoles entity)
        {
            var json = new ResJson() { success = false };

            try
            {
                bool IsSave = string.IsNullOrEmpty(entity.GUID);

                if (IsSave)
                {
                    if (await sysRoleServcie.IsAnyAsync(p => p.RoleName == entity.RoleName))
                    {
                        json.message = "角色名称已存在";
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
                if (await sysRoleServcie.SaveOrUpdateAsync(entity, IsSave))
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
        [ManageAuthorize(ModuleAlias = "Role", Operatinos = "Delete")]
        [HttpPost]
        public async Task<object> Deletes(string[] values)
        {
            var json = new ResJson() { success = false };

            try
            {
                if (values != null && values.Length > 0)
                {
                    if (await sysRoleServcie.DeleteAsync(values))
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
        /// 获取权限数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "Role", Operatinos = "Detail")]
        public object GetPermissions()
        {
            var db = sysRoleServcie.GetDb();           
            return Json(db.Queryable<SysModules>().Where(p=>p.Levels==2).ToList().Select(p=>new { p.Title,Permissions=db.Queryable<SysPermissions>().Where(m=>m.Module_GUID==p.GUID).ToList()}).ToList());
        }

        /// <summary>
        /// 获取角色权限数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "Role", Operatinos = "Detail")]
        public async Task<object> GetRolePermissionsAsync([FromServices]ISysRolePermissionService sysRolePermissionService,string GUID)
        {
            var data = await sysRolePermissionService.GetAllAsync(p => p.Role_GUID == GUID);
            return Json(data.Select(p=>p.Permission_GUID).ToArray());
        }

        /// <summary>
        /// 分配权限
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [ManageAuthorize(ModuleAlias = "Role", Operatinos = "Allocation")]
        [HttpPost]
        public object Allocation(string guid, string[] values)
        {
            var json = new ResJson() { success = false };

            try
            {
                var db = sysRoleServcie.GetDb();

                try
                {
                    db.Ado.BeginTran();
                    db.Deleteable<SysRolePermissions>(p => p.Role_GUID == guid).ExecuteCommand();
                    if (values != null && values.Length > 0)
                    {                        
                        var datas = new List<SysRolePermissions>();
                        foreach (var value in values)
                        {
                            datas.Add(new SysRolePermissions() { GUID = Guid.NewGuid().ToString(), Role_GUID = guid, Permission_GUID = value });
                        }
                        db.Insertable(datas).ExecuteCommand();
                    }
                    db.Ado.CommitTran();
                    json.message = "分配成功！";
                    json.success = true;
                }
                catch (Exception ex)
                {
                    db.Ado.RollbackTran();
                    json.message = "分配失败!";
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                json.message = "网络超时.";
                throw ex;
            }

            return Json(json);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "Role", Operatinos = "Detail")]
        public async Task<object> GetAllAsync(int page, int pagesize, string keyWords)
        {

            var exp = Expressionable.Create<SysRoles>()
                .AndIF(!string.IsNullOrEmpty(keyWords), p => p.RoleName.ToLower().Contains(keyWords.ToLower())).ToExpression();

            return Json(await sysRoleServcie.GetPageAsync(page, pagesize, exp), new Newtonsoft.Json.JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd HH:mm:ss", Formatting = Newtonsoft.Json.Formatting.Indented });
        }
    }

}
