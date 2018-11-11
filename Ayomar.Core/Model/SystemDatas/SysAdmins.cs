using ProtoBuf;
using SqlSugar;
using System;

namespace Ayomar.Core.Model
{
    [ProtoContract]
    public class SysAdmins
    {
        /// <summary>
        /// Describe：系统管理员
        /// Author：YuanGang
        /// Date：2016/07/16
        /// Blogs:http://www.cnblogs.com/yuangang
        /// </summary>
        [ProtoMember(1)]
        [SugarColumn(IsPrimaryKey =true)]
        public string GUID { get; set; }

        [ProtoMember(2)]
        public string UserName { get; set; }

        [ProtoMember(3)]
        public string Account { get; set; }

        [ProtoMember(4)]
        public string Password { get; set; }

        [ProtoMember(5)]
        public bool IsCanLogin { get; set; }

        [ProtoMember(6)]
        public string EN_Name { get; set; }

        [ProtoMember(7)]
        public string EN_Nme_Simple { get; set; }

        [ProtoMember(8)]
        public string CreateUser { get; set; }

        [ProtoMember(9)]
        public DateTime CreateDate { get; set; }

        [ProtoMember(10)]
        public string UpdateUser { get; set; }

        [ProtoMember(11)]
        public DateTime UpdateDate { get; set; }

        [ProtoMember(12)]
        public bool IsSuper { get; set; } = false;

        [ProtoMember(13)]
        public string DepartmentGuid { get; set; }

        [ProtoMember(14)]

        public string DepartmentName { get; set; }

        [ProtoMember(15)]
        public string Avatar { get; set; }

        [ProtoMember(16)]
        public string Mobile { get; set; }

        [ProtoMember(17)]
        public string Email { get; set; }

        [ProtoMember(18)]
        public string SecondaryPassword { get; set; }
    }
}
