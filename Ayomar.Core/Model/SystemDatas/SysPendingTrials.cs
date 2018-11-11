using SqlSugar;
using System;

namespace Ayomar.Core.Model
{
    public class SysPendingTrials
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string GUID { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string Genre { get; set; }

        /// <summary>
        /// 待审信息
        /// </summary>

        public string PendingTrialMessage { get; set; }

        /// <summary>
        /// 待审内容
        /// </summary>
        public string PendingTrialContent { get; set; }

        /// <summary>
        /// 提交者
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 审核者
        /// </summary>
        public string Auditor { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime AuditDate { get; set; }
        /// <summary>
        /// 审核信息
        /// </summary>

        public string AduitMessage { get; set; }

        /// <summary>
        /// 是否通过审核
        /// </summary>
        public bool IsPass { get; set; }

        
    }
}
