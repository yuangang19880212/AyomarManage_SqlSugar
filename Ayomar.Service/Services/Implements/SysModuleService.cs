using Ayomar.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ayomar.Service
{
    public class SysModuleService : Repository<SysModules>, ISysModuleService
    {
        /// <summary>
        /// 递归模块返回按级别排序
        /// </summary>
        /// <param name="Modules"></param>
        /// <returns></returns>
        public async Task<List<SysModules>> RecursiveModule(List<SysModules> Modules = null)
        {
            try
            {
                var newList = new List<SysModules>();

                Modules = Modules == null ? await GetAllAsync(p => p.GUID != "") : Modules;

                var InitParent_GUID = Modules == null ? "00000000-0000-0000-0000-000000000000" : Modules.OrderBy(p => p.Levels).FirstOrDefault().Parent_GUID;

                ChildModule(Modules, newList, InitParent_GUID);

                return newList;
            }
            catch (Exception e)
            {
                //await _sysLogService.LogAsync(Common.Enums.LoggerEnums.FATAL, Common.Enums.OperatorEnums.Remove, "System", "递归模块返回按级别排序（List<Modules>）", e.ToString());
                throw e;
            }
        }

        #region 私有方法
        /// <summary>
        /// 递归子模块，修改模块级别、显示状态
        /// </summary>
        /// <param name="Guid"></param>
        /// <param name=""></param>
        /// <param name="System_GUID"></param>
        /// <param name="Levels"></param>
        /// <param name="Modules"></param>
        /// <param name="ChildModules"></param>
        public void RecursiveChildModulesLevels(string Guid, int Levels, bool IsDisplay, List<SysModules> Modules, List<SysModules> ChildModules)
        {
            var Childs = Modules.FindAll(p => p.Parent_GUID == Guid);
            if (Childs.Count > 0)
            {
                foreach (var child in Childs)
                {
                    child.Levels = Levels + 1;
                    child.IsDisplay = IsDisplay;
                    ChildModules.Add(child);
                    RecursiveChildModulesLevels(child.GUID, child.Levels, IsDisplay, Modules, ChildModules);
                }
            }
        }
        /// <summary>
        /// 递归获取所有子模块
        /// </summary>
        /// <param name="Guid"></param>
        /// <param name="Modules"></param>
        /// <param name="ChildModules"></param>
        private void RecursiveChildModules(string Guid, List<SysModules> Modules, List<SysModules> ChildModules)
        {
            var Childs = Modules.FindAll(p => p.Parent_GUID == Guid);
            if (Childs.Count > 0)
            {
                foreach (var child in Childs)
                {
                    ChildModules.Add(child);
                    RecursiveChildModules(child.GUID, Modules, ChildModules);
                }
            }
        }
        /// <summary>
        /// 递归模块列表按级别排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="newlist"></param>
        /// <param name="parentId"></param>
        private void ChildModule(List<SysModules> list, List<SysModules> newlist, string parentGuid)
        {
            var result = list.Where(p => p.Parent_GUID == parentGuid).OrderBy(p => p.Levels).ThenBy(p => p.DisplayOrder).ToList();

            if (result.Count > 0)
                foreach (var child in result)
                {
                    newlist.Add(child);
                    ChildModule(list, newlist, child.GUID);
                }
        }
        /// <summary>
        ///  递归获取上级模块
        /// </summary>
        /// <param name="modules"></param>
        /// <param name="manageModules"></param>
        /// <param name="module"></param>
        private void RecursiveParentModule(List<SysModules> modules, List<SysModules> manageModules, SysModules module)
        {
            var newModule = modules.Find(p => p.GUID == module.Parent_GUID);
            if (newModule != null)
            {
                manageModules.Add(newModule);
                RecursiveParentModule(modules, manageModules, newModule);
            }
        }
        #endregion
    }
}
