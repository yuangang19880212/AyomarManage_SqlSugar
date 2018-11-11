using ProtoBuf;
using SqlSugar;
using System;

namespace Ayomar.Core.Model
{
    ///<summary>
    ///
    ///</summary>
    [ProtoContract]
    public partial class SysPermissions
    {
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true)]
        [ProtoMember(1)]
        public string GUID { get; set; }
        [ProtoMember(2)]
        public string Module_GUID { get; set; }
        [ProtoMember(3)]
        public string Title { get; set; }
        [ProtoMember(4)]
        public string PermissionValue { get; set; }
        [ProtoMember(5)]
        public int DisplayOrder { get; set; }
        [ProtoMember(6)]
        public string CreateUser { get; set; }
        [ProtoMember(7)]
        public DateTime CreateDate { get; set; }
        [ProtoMember(8)]
        public string UpdateUser { get; set; }
        [ProtoMember(9)]
        public DateTime UpdateDate { get; set; }

    }
}
