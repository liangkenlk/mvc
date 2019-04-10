using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using WxPayAPI;

namespace WorkFlow.Utility
{
    public class ctHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.Write("xxxxx");
            string exname = Path.GetExtension(context.Request.PhysicalPath).ToLower();
            string filename = System.IO.Path.GetFileName(context.Request.PhysicalPath).ToLower();
            string filepath = context.Request.CurrentExecutionFilePath.ToLower();
            if (filename == "login.ct")
            {
                string LoginName = context.Request["LoginName"];
                string LoginPwd = context.Request["LoginPwd"];
                context.Response.Redirect("~/server/userhandler.ashx?method=Login4old&LoginID=" + LoginName + "&PassWord=" + LoginPwd);
            }

            if (filename == "getwallmappointconf.ct")
            {
                string wallmap = context.Request["wallmap"];
                context.Response.Redirect("~/server/maphandler.ashx?method=GetWallMapUrl&wallmap=" + wallmap);
            }
            
            if (filename == "moblile_deleteLabel.ct")
            {
                string selected = context.Request["uuid"];
                context.Response.Redirect("~/server/maphandler.ashx?method=deleteLabel&selected=" + selected);
            }

            if (filename == "loginmove.ct")
            {
                string LoginName = context.Request["user.loginName"];
                string LoginPwd = context.Request["user.loginPwd"];
                context.Response.Redirect("~/server/userhandler.ashx?method=Login4old&LoginID=" + LoginName + "&PassWord=" + LoginPwd);
            }
            if (filename == "uploadLableInfo.ct".ToLower())
            {

                string id = context.Request["label.id"];
                string name = context.Request["label.name"];
                string type = context.Request["label.type"];
                string describe = context.Request["label.describe"];
                string latlng = context.Request["label.latlng"];
                string createTime = context.Request["label.createTime"];
                context.Response.Redirect("~/server/maphandler.ashx?method=uploadLableInfo&id=" + id + "&name=" + name + "&type=" + type + "&describe=" + describe + "&latlng=" + latlng + "&createTime=" + createTime );
            }
            if (filename == "getLableListByUserIdForJson.ct".ToLower())
            {
                string userid = context.Request["user.id"];
                context.Response.Redirect("~/server/maphandler.ashx?method=getLableList4old&userid="+userid);
            }
            if (filename == "deleteLabel.ct".ToLower())
            {
                string id = context.Request["id"];
                context.Response.Redirect("~/server/maphandler.ashx?method=getLableList&selected=" + id);
            }
            if (filename == "origImage.jpg")
            {
                
            }
        }
    }
}
