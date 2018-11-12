using Ayomar.Common;
using Ayomar.Core.Model;
using Ayomar.Service;
using Ayomar.Service.Services.ComponentService;
using Ayomar.Web.Areas.BaseControllers;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Ayomar.Web.Areas.SchedulingManage.Controllers
{
    [Area("SchedulingManage")]
    public class SchedulingController : BaseController
    {
        private readonly ISysScheduleService sysScheduleService;
        private readonly SchedulerService schedulerService;

        public SchedulingController(ISysScheduleService sysScheduleService, SchedulerService schedulerService)
        {
            this.sysScheduleService = sysScheduleService;
            this.schedulerService = schedulerService;
        }

        [ManageAuthorize(ModuleAlias = "Scheduling", Operatinos = "View")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取可执行的Quartz任务
        /// </summary>
        /// <returns></returns>
        [ManageAuthorize(ModuleAlias = "Scheduling", Operatinos = "Detail")]
        public object GetTaskJobs()
        {
            return Json(Assembly.Load(new AssemblyName("Ayomar.Service")).DefinedTypes.Where(p => p.Namespace == "Ayomar.Service.Services.QuartzService" && !p.Name.Contains("Execute")).Select(p=>new { p.FullName,p.Name}));
        }

        /// <summary>
        /// 添加修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ManageAuthorize(ModuleAlias = "Scheduling", Operatinos = "Save")]
        public async Task<object> PutAsync(SysSchedules entity)
        {
            var json = new ResJson() { success = false };

            try
            {
                //检测Cron表达式是否正确
                if (!schedulerService.IsValidateCron(entity.Cron))
                {
                    json.message = "Cron表达式不正确";
                    return json;
                }

                bool IsSave = string.IsNullOrEmpty(entity.GUID);

                if (IsSave)
                {
                    if (await sysScheduleService.IsAnyAsync(p => p.JobName == entity.JobName || p.JobService == entity.JobService))
                    {
                        json.message = "任务已存在";
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
                entity.Status = 1;
                //添加任务计划
                var dateOffset = await schedulerService.AddJobAsync(entity.JobName, entity.JobGroup, entity.JobService, entity.Cron, Unitls.DateTimeToDateTimeOffset(entity.StarRunTime), Unitls.DateTimeToDateTimeOffset(entity.EndRunTime));
                entity.NextRunTime = dateOffset.DateTime;

                if (await sysScheduleService.SaveOrUpdateAsync(entity, IsSave))
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
        [ManageAuthorize(ModuleAlias = "Scheduling", Operatinos = "Delete")]
        [HttpPost]
        public async Task<object> Deletes(string[] values)
        {
            var json = new ResJson() { success = false };

            try
            {
                if (values != null && values.Length > 0)
                {

                    if(await schedulerService.DeletesJobAsync(await sysScheduleService.GetAllAsync(p => values.Contains(p.GUID))))
                    {
                        if (await sysScheduleService.DeleteAsync(values))
                        {
                            json.message = "删除成功！";
                            json.success = true;
                        }
                        else { json.message = "删除失败!"; }
                    }
                    else
                        json.message = "删除失败!";

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
        /// 启动任务
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [ManageAuthorize(ModuleAlias = "Scheduling", Operatinos = "Start")]
        [HttpPost]
        public async Task<object> StartAsync(string GUID,string JobName,string JobGroup)
        {
            var json = new ResJson() { success = false };

            try
            {
                if (await schedulerService.ReStartJobAsync(JobName, JobGroup))
                {
                    if (sysScheduleService.ExecuteSql("UPDATE SysSchedules SET STATUS = 1 WHERE GUID ='" + GUID + "'") > 0)
                    {
                        json.message = "操作成功";
                        json.success = true;
                    }
                    else
                        json.message = "操作失败";
                }
                else json.message = "操作失败";
            }
            catch (Exception ex)
            {
                json.message = "网络超时.";
                throw ex;
            }

            return json;

        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [ManageAuthorize(ModuleAlias = "Scheduling", Operatinos = "Stop")]
        [HttpPost]
        public async Task<object> PauseAsync(string GUID, string JobName, string JobGroup)
        {
            var json = new ResJson() { success = false };

            try
            {
                if (await schedulerService.PauseJobAsync(JobName, JobGroup))
                {
                    if (sysScheduleService.ExecuteSql("UPDATE SysSchedules SET STATUS = 2 WHERE GUID ='" + GUID + "'") > 0)
                    {
                        json.message = "操作成功";
                        json.success = true;
                    }
                    else
                        json.message = "操作失败";
                }
                else json.message = "操作失败";
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
        [ManageAuthorize(ModuleAlias = "Scheduling", Operatinos = "Detail")]
        public async Task<object> GetAllPageAsync(int page, int pagesize, string keyWords)
        {

            var exp = Expressionable.Create<SysSchedules>()
                .AndIF(!string.IsNullOrEmpty(keyWords), p => p.JobName.ToLower().Contains(keyWords.ToLower())).ToExpression();

            return Json(await sysScheduleService.GetPageAsync(page, pagesize, exp), new Newtonsoft.Json.JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd HH:mm:ss", Formatting = Newtonsoft.Json.Formatting.Indented });
        }
    }
}
