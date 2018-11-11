using Ayomar.Core.Model;
using Ayomar.Service;
using Ayomar.Web.Areas.BaseControllers;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ayomar.Web.Areas.AdminManage.Controllers
{
    [Area("AdminManage")]
    public class AdminController : BaseController
    {
        private readonly ISysAdminService sysAdminService;

        public AdminController(ISysAdminService sysAdminService)
        {
            this.sysAdminService = sysAdminService;
        }

        [ManageAuthorize(ModuleAlias = "Admin", Operatinos = "View")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 添加修改
        /// </summary>
        /// <param name="sysAdminService"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "Admin", Operatinos = "Save")]
        public async Task<object> PutAsync(SysAdmins entity)
        {
            var json = new ResJson() { success = false };

            try
            {
                bool IsSave = string.IsNullOrEmpty(entity.GUID);

                if (IsSave)
                {
                    if (await sysAdminService.IsAnyAsync(p => p.Account == entity.Account || p.Mobile==entity.Mobile))
                    {
                        json.message = "账号或手机号码已存在";
                        return json;
                    }

                    // Add 初始参数
                    entity.CreateUser = AdminUser.User.Account;
                    entity.CreateDate = DateTime.Now;
                    entity.GUID = Guid.NewGuid().ToString();
                    entity.IsCanLogin = true;
                    entity.Password = new Common.CryptHelper.AESCrypt().Encrypt(entity.Mobile);
                    entity.SecondaryPassword = new Common.CryptHelper.AESCrypt().Encrypt("123456");
                }

                // Add、Update 默认参数
                if (string.IsNullOrEmpty(entity.UserName))
                    entity.UserName = "匿名";
                if (string.IsNullOrEmpty(entity.EN_Name))
                    entity.EN_Name = Common.ToolHelper.ConvertHzToPz.Convert(entity.UserName);
                if (string.IsNullOrEmpty(entity.EN_Nme_Simple))
                    entity.EN_Nme_Simple = Common.ToolHelper.ConvertHzToPz.ConvertFirst(entity.UserName);               

                entity.UpdateUser = AdminUser.User.Account;
                entity.UpdateDate = DateTime.Now;
                entity.IsSuper = false;

                //保存模块
                if (await sysAdminService.SaveOrUpdateAsync(entity,IsSave))
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
        [ManageAuthorize(ModuleAlias = "Admin", Operatinos = "Delete")]
        [HttpPost]
        public async Task<object> Deletes(string[] values)
        {
            var json = new ResJson() { success = false };

            try
            {
                if (values != null && values.Length > 0)
                {
                    if (await sysAdminService.DeleteAsync(values))
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
        /// 锁定/解锁 账户
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [ManageAuthorize(ModuleAlias = "Admin", Operatinos = "Lock,UnLock")]
        [HttpPost]
        public object Locks(string[] values,bool IsLock)
        {
            var json = new ResJson() { success = false };

            try
            {
                if (values != null && values.Length > 0)
                {
                    var locked = IsLock ? 0 : 1;
                    if (sysAdminService.ExecuteSql("UPDATE SYSADMINS SET IsCanLogin = "+locked+" WHERE GUID IN ('"+ string.Join("','", values) + "')")>0)
                    {
                        json.message = IsLock?"锁定成功！":"解锁成功!";
                        json.success = true;
                    }
                    else { json.message = IsLock? "锁定失败!":"解锁失败!"; }

                }
                else
                    json.message = "操作失败!";
            }
            catch (Exception ex)
            {
                json.message = "网络超时.";
                throw ex;
            }

            return json;
        }

        /// <summary>
        /// 分配角色
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [ManageAuthorize(ModuleAlias = "Admin", Operatinos = "Allocation")]
        [HttpPost]
        public object Allocation(string guid,string[] values)
        {
            var json = new ResJson() { success = false };

            try
            {
                var db = sysAdminService.GetDb();

                try
                {
                    db.Ado.BeginTran();
                    db.Deleteable<SysRoleAssignments>(p => p.Admin_GUID == guid).ExecuteCommand();
                    if(values!=null && values.Length > 0)
                    {
                        var datas = new List<SysRoleAssignments>();
                        foreach(var value in values)
                        {
                            datas.Add(new SysRoleAssignments() {GUID=Guid.NewGuid().ToString(),Admin_GUID=guid,Role_GUID=value });
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
        [ManageAuthorize(ModuleAlias = "Admin", Operatinos = "Detail")]
        public async Task<object> GetAllAsync(int page, int pagesize, string keyWords)
        {

            var exp = Expressionable.Create<SysAdmins>()
                .And(p => !p.IsSuper)
                .AndIF(!string.IsNullOrEmpty(keyWords), p => p.Account.ToLower().Contains(keyWords.ToLower()) || p.UserName.ToLower().Contains(keyWords.ToLower()) || p.Mobile.ToLower().Contains(keyWords.ToLower()) || p.Email.ToLower().Contains(keyWords.ToLower())).ToExpression();

            return Json(await sysAdminService.GetPageAsync(page, pagesize, exp), new Newtonsoft.Json.JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd HH:mm:ss", Formatting = Newtonsoft.Json.Formatting.Indented });
        }

        /// <summary>
        /// 获取角色数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "Admin", Operatinos = "Detail")]
        public async Task<object> GetRoles([FromServices]ISysRoleServcie sysRoleServcie)
        {
            return Json(await sysRoleServcie.GetAllAsync(p => p.GUID != ""));
        }
        /// <summary>
        /// 获取用户所属角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "Admin", Operatinos = "Detail")]
        public async Task<object> GetRoleAssignments([FromServices]ISysRoleAssignmentServcie sysRoleAssignmentServcie,  string GUID)
        {
            var RoleAssignments = await sysRoleAssignmentServcie.GetAllAsync(p => p.Admin_GUID == GUID);

            return Json(RoleAssignments.Select(p => p.Role_GUID).ToArray());
        }
    }
}
