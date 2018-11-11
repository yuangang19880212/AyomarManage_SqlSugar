using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ayomar.Core.Model
{
    [ProtoContract]
    public class Admins
    {
        [ProtoMember(1)]
        public SysAdmins User { get; set; }
        [ProtoMember(2)]
        public List<SysModules> Modules { get; set; }
        [ProtoMember(3)]
        public List<SysPermissions> Permissions { get; set; }
    }
}
