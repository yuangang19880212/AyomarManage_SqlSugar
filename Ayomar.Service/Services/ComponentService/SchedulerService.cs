using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Ayomar.Service.Services.ComponentService
{
    public class SchedulerService
    {
        /// <summary>
        /// 任务调度对象
        /// </summary>
        public static IScheduler scheduler = null;

        private async Task<IScheduler> SchedulerAsync()
        {
            if (scheduler != null)
            {
                return scheduler;
            }
            else
            {
                ISchedulerFactory factory = new StdSchedulerFactory();
                return await factory.GetScheduler();
            }
        }

        /// <summary>
        /// 添加一个任务调度
        /// </summary>
        /// <param name="JobName"></param>
        /// <param name="JobGroup"></param>
        /// <param name="JobService"></param>
        /// <param name="Cron"></param>
        /// <param name="StarRunTime"></param>
        /// <param name="EndRunTime"></param>
        /// <returns>最近一次开始执行时间</returns>
        public async Task<DateTimeOffset> AddJobAsync(string JobName, string JobGroup, string JobService, string Cron, DateTimeOffset StarRunTime, DateTimeOffset EndRunTime)
        {
            scheduler = await SchedulerAsync();
            //开启
            await scheduler.Start();

            var jobKey = new JobKey(JobName, JobGroup);
            //检查任务是否已存在 存在删除
            if (await scheduler.CheckExists(jobKey))
                await scheduler.DeleteJob(jobKey);

            //反射获取任务执行服务
            var jobTask = Assembly.Load(new AssemblyName("Ayomar.Service")).GetType(JobService);

            IJobDetail job = new JobDetailImpl(JobName, JobGroup, jobTask);

            //创建触发器
            ITrigger trigger = TriggerBuilder.Create()
                .StartAt(StarRunTime)
                .EndAt(EndRunTime)
                .WithIdentity(JobName, JobGroup)
                .WithCronSchedule(Cron)
                .ForJob(JobName, JobGroup)
                .Build();

            //使用触发器安排作业
           return await scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// 暂停指定任务计划
        /// </summary>
        /// <returns></returns>
        public async Task<bool> PauseJobAsync(string JobName, string JobGroup)
        {
            try
            {
                scheduler = await SchedulerAsync();
                //使任务暂停
                var job = new JobKey(JobName, JobGroup);
                if (await scheduler.CheckExists(job))
                {
                    await scheduler.PauseJob(job);
                    return true;
                }
                else { return false; }
            }
            catch
            {               
                return false;
            }
        }


        /// <summary>
        /// 恢复暂停的任务计划
        /// </summary>
        /// <param name="GUID"></param>
        /// <param name="JobName"></param>
        /// <param name="JobGroup"></param>
        /// <param name="operUser"></param>
        /// <returns></returns>
        public async Task<bool> ReStartJobAsync(string JobName, string JobGroup)
        {

            try
            {

                scheduler = await SchedulerAsync();

                //使暂停任务恢复
                var job = new JobKey(JobName,JobGroup);
                if (await scheduler.CheckExists(job))
                {
                    await scheduler.ResumeJob(job);
                    return true;
                }
                else { return false; }
                
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 删除指定任务计划
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteJobAsync(string JobName, string JobGroup)
        {
            try
            {
                scheduler = await SchedulerAsync();
                //删除任务
                var job = new JobKey(JobName, JobGroup);
                if (await scheduler.CheckExists(job))
                    await scheduler.DeleteJob(job);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 删除多个任务计划
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeletesJobAsync(List<Core.Model.SysSchedules> dys)
        {
            try
            {
                scheduler = await SchedulerAsync();
                List<JobKey> jobKeys = new List<JobKey>();
                //删除多个任务
                foreach(var dy in dys)
                {
                    jobKeys.Add(new JobKey(dy.JobName, dy.JobGroup));
                }
                await scheduler.DeleteJobs(jobKeys);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 任务是否存在
        /// </summary>
        /// <param name="JobName"></param>
        /// <param name="JobGroup"></param>
        /// <returns></returns>
        public async Task<bool> IsAnyAsync(string JobName,string JobGroup)
        {
            var job = new JobKey(JobName, JobGroup);
            return await scheduler.CheckExists(job);
        }


        /// <summary>
        /// cron表达式是否正确
        /// </summary>
        /// <param name="cron"></param>
        /// <returns></returns>
        public bool IsValidateCron(string cron)
        {
            return CronExpression.IsValidExpression(cron);
        }
    }
}
