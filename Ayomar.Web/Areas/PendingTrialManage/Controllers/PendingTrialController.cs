using Ayomar.Core.Model;
using Ayomar.Service;
using Ayomar.Web.Areas.BaseControllers;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ayomar.Web.Areas.PendingTrialManage.Controllers
{
    [Area("PendingTrialManage")]
    public class PendingTrialController : BaseController
    {
        private readonly ISysPendingTrialService sysPendingTrialService;

        public PendingTrialController(ISysPendingTrialService sysPendingTrialService)
        {
            this.sysPendingTrialService = sysPendingTrialService;
        }

        [ManageAuthorize(ModuleAlias = "PendingTrial", Operatinos = "View")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="sysAdminService"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "PendingTrial", Operatinos = "Audit")]
        public async Task<object> PutAsync(SysPendingTrials entity)
        {
            var json = new ResJson() { success = false };

            try
            {
                entity.Auditor = AdminUser.User.Account;
                entity.AuditDate = DateTime.Now;

                if (await sysPendingTrialService.UpdateAsync(entity))
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
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "PendingTrial", Operatinos = "Detail")]
        public async Task<object> GetAllAsync(int page, int pagesize, string keyWords)
        {

            var exp = Expressionable.Create<SysPendingTrials>()
                .AndIF(!string.IsNullOrEmpty(keyWords), p => p.CreateUser.ToLower().Contains(keyWords.ToLower()) || p.Auditor.ToLower().Contains(keyWords.ToLower()) || p.PendingTrialMessage.Contains(keyWords)).ToExpression();

            return Json(await sysPendingTrialService.GetPageAsync(page, pagesize, exp, "Auditor ASC,AuditDate DESC"), new Newtonsoft.Json.JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd HH:mm:ss", Formatting = Newtonsoft.Json.Formatting.Indented });
        }
    }
}
