using Ayomar.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ayomar.Service
{
    public interface ISysModuleService : IRepository<SysModules>
    {
        Task<List<SysModules>> RecursiveModule(List<SysModules> Modules = null);
        void RecursiveChildModulesLevels(string Guid, int Levels, bool IsDisplay, List<SysModules> Modules, List<SysModules> ChildModules);
    }
}
