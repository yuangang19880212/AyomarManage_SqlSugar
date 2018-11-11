using Ayomar.Core.Model;
using Ayomar.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ayomar.Web.Areas.BaseControllers
{
    /// <summary>
    /// BaseController 
    /// 所有后台Controller均继承BaseController
    /// 负责：系统过滤、管理员认证、权限认证
    /// </summary>
    public class BaseController : Controller
    {
        public Admins AdminUser
        {
            get
            {
                var _httpContextAccessor = new HttpContextAccessor();
                byte[] byteArray = null;
                if (_httpContextAccessor.HttpContext.Session.TryGetValue("AdminsAuthentication", out byteArray))
                {
                    return Common.ProtobufHelper.ProtobufHelper.DeSerialize<Admins>(byteArray);
                }
                else
                {
                    string CookieValue = string.Empty;
                    if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("CookiesAuthentication", out CookieValue))
                    {
                        var systemsJson = new Common.CryptHelper.AESCrypt().Decrypt(CookieValue);
                        if (!Common.JsonHelper.JsonSplit.IsJson(systemsJson))
                            return null;
                        var admins = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(systemsJson);
                        string account = admins.account, password = admins.password;
                        var sysAdminService = _httpContextAccessor.HttpContext.RequestServices.GetService(typeof(ISysAdminService)) as ISysAdminService;
                        var User = sysAdminService.ValidateLogin(account, password);
                        if (User!=null && User.User.IsCanLogin)
                        {
                            _httpContextAccessor.HttpContext.Session.Set("AdminsAuthentication", Common.ProtobufHelper.ProtobufHelper.Serialize(User));
                            return User;
                        }
                        else
                            return null;
                    }
                    return null;
                }
            }
        }       
    }
    /// <summary>
    /// 权限认证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ManageAuthorize : ActionFilterAttribute
    {
        // 模块别名
        public string ModuleAlias { get; set; }
        // 权限动作
        public string Operatinos { get; set; }
        // 实例化控制器基类
        BaseController baseController = new BaseController();
        // 耗时检测
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        //权限集合
        List<SysPermissions> Permissions = new List<SysPermissions>();


        /// <summary>
        ///  验证操作权限
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            stopwatch.Restart();
            // 管理员存在性验证
            if (baseController.AdminUser == null)
            {
                context.Result = new RedirectResult("/AuthorizeManage/Authorize/UserExpired");
                return;
            }

            if (ModuleAlias != "NoVerify")
            {
                var _adminVerifyService = context.HttpContext.RequestServices.GetService(typeof(ISysAdminService)) as ISysAdminService;
                Permissions = _adminVerifyService.VerifyPermission(ModuleAlias, baseController.AdminUser.Permissions,baseController.AdminUser.Modules);

                // 模块与动作验证
                if (Permissions == null || Permissions.Count <= 0 || Permissions.Find(p => Operatinos.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(m=>m.ToLower()).ToArray().Contains(p.PermissionValue.ToLower())) == null)
                {
                    if (context.HttpContext.Request.Method.ToLower() == "post" || context.HttpContext.Request.Method.ToLower() == "put" || context.HttpContext.Request.Method.ToLower() == "delete")
                        context.Result = new JsonResult(new ResJson() { success = false, statusCode = 500, message = "权限验证失败，无操作权限！", returnUrl = "/authorizemanage/authorize/authenticationfails" });
                    else
                        context.Result = new RedirectResult("/authorizemanage/authorize/authenticationfails");
                    return;
                }
            }

        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            //生成权限操作json
            context.HttpContext.Items["PermissJson"] =Newtonsoft.Json.JsonConvert.SerializeObject(Permissions.OrderBy(p => p.DisplayOrder).ThenBy(p => p.UpdateDate).Select(p => p.PermissionValue.ToLower()).Distinct());
        }
    }
}
