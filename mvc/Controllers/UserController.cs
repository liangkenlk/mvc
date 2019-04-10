using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TY.Core;
using WorkFlow;

namespace mvc.Controllers
{
    public class UserController : BaseController<Data.UserDataTable,Data.UserRow>
    {
        //public ActionResult login()
        //{
        //    var LoginID = Query<string>("LoginID");
        //    var PassWord = Query<string>("PassWord");
        //    string errMsg = "";
        //    string msg = UserAuth.GetAuthToken(LoginID, PassWord, out errMsg);

        //    if (msg == null)
        //    {
        //       return JsonOb(false, errMsg);
        //    }
        //    else
        //        return JsonOb(true, msg);

        //}



        public ActionResult Getlist() 
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string username = this.Query<string>("username");
            string sql = "select * from [user]";
            if (username != null)
                sql += " where LoginID like '%" + username + "%'";
                
            return JsonOb(bll.GetPagedDataTable(sql, pageSize, pageindex));
        }

        public ActionResult login()
        {
            string loginID = Query<string>("LoginID");
            string password = Query<string>("PassWord");
            string msg = "";
            string token =  UserAuth.GetAuthToken(loginID, password,out msg);
            HttpCookie c = new HttpCookie("token");
            c.Value = token;
            this.Response.SetCookie(c); 
            return JsonOb(true, msg, token);
        }

        public ActionResult Logout()
        {
            UserAuth.Logout();
            FormsAuthentication.SignOut();
            return JsonOb(true, "ok");
        }

        public ActionResult UpdatePushCid()
        {
            string cid = Query<string>("cid");
            var u =  UserAuth.User;
            u.OpenID = cid;
            bll.Update(u);
            return JsonOb(true, "ok");
        }
    }
}