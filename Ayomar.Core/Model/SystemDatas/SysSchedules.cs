using SqlSugar;
using System;

namespace Ayomar.Core.Model
{
    /// <summary>
    /// 调度任务
    /// </summary>
    public class SysSchedules
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string GUID { get; set; }
        /// <summary>
        /// 任务组
        /// </summary>
        public string JobGroup { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 执行周期表达式
        /// </summary>
        public string Cron { get; set; }
        /// <summary>
        /// 开始运行时间
        /// </summary>
        public DateTime StarRunTime { get; set; }
        /// <summary>
        /// 结束运行时间
        /// </summary>
        public DateTime EndRunTime { get; set; }
        /// <summary>
        /// 上次时间
        /// </summary>
        public DateTime? PreviousRunTime { get; set; }
        /// <summary>
        /// 下次执行时间
        /// </summary>
        public DateTime? NextRunTime { get; set; }
        /// <summary>
        /// 要执行的任务
        /// </summary>
        public string JobService { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDescription { get; set; }
    }
}
