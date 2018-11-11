using Ayomar.Common.ResultHelper;
using Ayomar.Core.Model;
using Ayomar.Service;
using Ayomar.Web.Areas.BaseControllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ayomar.Web.Areas.SystemManage.Controllers
{
    [Area("SystemManage")]
    public class ModuleController : BaseController
    {
        private readonly ISysModuleService sysModuleService;

        public ModuleController(ISysModuleService sysModuleService)
        {
            this.sysModuleService = sysModuleService;
        }

        #region 视图
        [ManageAuthorize(ModuleAlias = "Module", Operatinos = "View")]
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// 添加修改
        /// </summary>
        /// <param name="_sysSystemConfigService"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "Module", Operatinos = "Save")]
        public async Task<object> PutAsync(SysModules entity)
        {
            var json = new ResJson() { success = false };

            try
            {
                bool IsSave = string.IsNullOrEmpty(entity.GUID);

                //初始化子模块
                List<SysModules> ChildModules = new List<SysModules>();

                if (IsSave)
                {
                    // 模块别名（权限标示名称）不能重复
                    if (await sysModuleService.IsAnyAsync(p => p.Alias == entity.Alias))
                    {
                        json.message = "模块别名不能重复";
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

                //设置模块级别
                if (entity.Parent_GUID.Equals("00000000-0000-0000-0000-000000000000"))
                    entity.Levels = 0;
                else
                {
                    var ParentModule = await sysModuleService.GetAsync(p => p.GUID == entity.Parent_GUID);
                    entity.Levels = ParentModule.Levels + 1;
                }

                //批量修改下级模块（模块级别、显示状态）
                if (!IsSave)
                {
                    ChildModules.Add(entity);

                    var OldModules = await sysModuleService.GetAsync(p => p.GUID == entity.GUID);

                    //是否改变
                    if (OldModules.Levels != entity.Levels || OldModules.IsDisplay != entity.IsDisplay)
                    {
                        //递归修改子模块
                        sysModuleService.RecursiveChildModulesLevels(entity.GUID, entity.Levels, entity.IsDisplay, await sysModuleService.GetAllAsync(p => p.GUID != ""), ChildModules);
                    }
                }

                //保存模块
                if (IsSave ? await sysModuleService.SaveAsync(entity) : await sysModuleService.UpdateAsync(ChildModules))
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
        [ManageAuthorize(ModuleAlias = "Module", Operatinos = "Delete")]
        [HttpPost]
        public object Deletes(string[] values)
        {
            var json = new ResJson() { success = false };

            try
            {
                if (values != null && values.Length > 0)
                {
                    List<SysModules> ChildModules = new List<SysModules>();

                    //查询出所有模块及其子模块
                    var Modules = sysModuleService.Sql(@"SELECT * FROM `sysmodules` WHERE GUID IN (
                                                         SELECT DISTINCT CASE _id WHEN 0 THEN p0.GUID WHEN 1 THEN p1.GUID WHEN 2 THEN p2.GUID END as GUID
                                                         FROM `sysmodules` p0
                                                         LEFT JOIN `sysmodules` p1 ON p1.Parent_GUID = p0.GUID
                                                         LEFT JOIN `sysmodules` p2 ON p2.Parent_GUID = p1.GUID
                                                         CROSS JOIN( SELECT 0 as _id UNION ALL SELECT 1 UNION ALL SELECT 2 ) p
                                                         WHERE p0.GUID IN ('" + string.Join("','", values) + "')) ORDER BY Levels,DisplayOrder ASC");

                    var db = sysModuleService.GetDb();

                    try
                    {
                        db.Ado.BeginTran();
                        db.Deleteable(Modules).ExecuteCommand();
                        var moduleguids = Modules.Select(m => m.GUID).ToArray();
                        db.Deleteable<SysPermissions>(p => moduleguids.Contains(p.Module_GUID)).ExecuteCommand();
                        db.Ado.CommitTran();
                        json.message = "删除成功！";
                        json.success = true;
                    }
                    catch (Exception ex)
                    {
                        db.Ado.RollbackTran();
                        json.message = "删除失败!";
                        throw ex;
                    }

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
        /// 获取所有模块
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "Module", Operatinos = "Detail")]
        public async Task<object> GetLevelsAllAsync(string GUID)
        {
            var datas = await sysModuleService.RecursiveModule();

            datas = string.IsNullOrEmpty(GUID) ? datas : datas.FindAll(p => p.GUID != GUID);

            return Json(datas);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "Module", Operatinos = "Detail")]
        public async Task<object> GetAllAsync(string keyWords)
        {
            var datas = await sysModuleService.RecursiveModule();

            return Json(string.IsNullOrEmpty(keyWords) ? datas : datas.FindAll(p => p.Title.ToLower().Contains(keyWords.ToLower()) || p.Alias.ToLower().Contains(keyWords.ToLower()) || p.ModulePath.ToLower().Contains(keyWords.ToLower())));
        }
    }
}
