using ProtoBuf;
using SqlSugar;
using System;

namespace Ayomar.Core.Model
{
    [ProtoContract]
    public partial class SysModules
    {
        [ProtoMember(1)]
        [SugarColumn(IsPrimaryKey=true)]
           public string GUID {get;set;}

        [ProtoMember(2)]
        public string Parent_GUID {get;set;}

        [ProtoMember(3)]
        public string Title {get;set;}

        [ProtoMember(4)]
        public string Alias {get;set;}

        [ProtoMember(5)]
        public string Icon {get;set;}

        [ProtoMember(6)]
        public string ModulePath {get;set;}

        [ProtoMember(7)]
        public bool IsDisplay {get;set;}

        [ProtoMember(8)]
        public int DisplayOrder {get;set;}

        [ProtoMember(9)]
        public int Levels {get;set;}

        [ProtoMember(10)]
        public string CreateUser {get;set;}

        [ProtoMember(11)]
        public DateTime CreateDate {get;set;}

        [ProtoMember(12)]
        public string UpdateUser {get;set;}

        [ProtoMember(13)]
        public DateTime UpdateDate {get;set;}

    }
}
