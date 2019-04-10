using System;
using System.Web;
using System.IO;
using TY.Core;

namespace WorkFlow.Utility
{
    public class myPowerModule : IHttpModule
    {
        /// <summary>
        /// 您将需要在网站的 Web.config 文件中配置此模块
        /// 并向 IIS 注册它，然后才能使用它。有关详细信息，
        /// 请参见下面的链接: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpModule Members

        public void Dispose()
        {
            //此处放置清除代码。
        }

        public void Init(HttpApplication context)
        {
            // 下面是如何处理 LogRequest 事件并为其 
            // 提供自定义日志记录实现的示例
            context.LogRequest += new EventHandler(OnLogRequest);
            context.AcquireRequestState += context_AcquireRequestState;
        }

        void context_AcquireRequestState(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            string exname = Path.GetExtension(application.Request.PhysicalPath).ToLower();
            string filename = System.IO.Path.GetFileName(application.Request.PhysicalPath).ToLower();
            string filepath = application.Request.CurrentExecutionFilePath.ToLower();
            string director = "";
            if(filename!="")
            director= filepath.Replace(filename, "").Trim('/').ToLower();
            else
                director = filepath.Trim('/').ToLower();
            director = director.Split('/')[director.Split('/').Length - 1];
            
            if (application.Request.Cookies["token"] == null)
            {
                //if (exname != ".ashx" && filename != "login.html")
                //{
                //    if (exname == ".aspx" || exname == ".html" || exname == "")
                //        application.Response.Redirect("~/"+ wechat+"login.html?returnUrl=" + application.Request.Url);
                //}
               if(director=="pages" || director == "workpage" || director== "backstage" || filename=="default.aspx")
                    application.Response.Redirect("~/" +  "login.html?returnUrl=" + HttpContext.Current.Request.Url.OriginalString);
            }
            else
            {
                if (filename == "login.html")
                {
                    string returnUrl = application.Request.QueryString["returnUrl"];
                    if (returnUrl != null)
                        //application.Response.Redirect(returnUrl);
                        HttpContext.Current.Response.Redirect("default.aspx");
                }
            }
            if (filename == "mobile_login")
            {
                string LoginName = application.Request["user.loginName"];
                string LoginPwd = application.Request["user.loginPwd"];
                application.Response.Redirect("~/server/userhandler.ashx?method=mobile_login&LoginID=" + LoginName + "&PassWord=" + LoginPwd);
            }
            //只允许管理员进后台
            if (director == "backstage" && (UserAuth.User.IsUserTypeNull() || UserAuth.User.UserType != "管理员"))
            {
                application.Response.Redirect("~/" + "login.html");

            }



        }

        #endregion

        public void OnLogRequest(Object source, EventArgs e)
        {
            //可以在此处放置自定义日志记录逻辑
        }
    }
}
