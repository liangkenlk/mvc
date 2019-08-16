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

        public ActionResult GetCurrentUserInfo()
        {
            if (!UserAuth.IsLogined())
                return JsonOb(false, "您未登录");
            return Json(JsonHelper.DataRowToDictionary(UserAuth.User), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDeptTree()
        {
            //Dictionary<ob, ob> dic =
            string username = this.Query<string>("username");

            string ParentId = this.Query<string>("ParentId");
            if (ParentId == null)
            {
                ADOBaseBLL<Data.DeptDataTable, Data.DeptRow> deptbll = new ADOBaseBLL<Data.DeptDataTable, Data.DeptRow>(new DBContext());
                string sql = "SELECT 'D' + id AS UserId,'' as LoginID, DeptName AS UserName, '' AS PassWord, '' AS CellPhone, '' AS Email, '' AS DataPower, '' AS BtnPower, '部门' AS UserType,'' as DeptId ,'D' + ParentId as parentid FROM Dept UNION SELECT convert(varchar(10),UserId) as UserId, LoginID, UserName, PassWord, CellPhone, Email, DataPower, BtnPower, UserType, DeptId, 'D'+ DeptId AS parentid FROM [User] ";
                if (username != null)
                    sql += " where UserName like '%" + username + "%'";
                var t = deptbll.GetNormalDataTable(sql);
                return JsonOb(t);
                
            }
            else
            {
                string sql = "select * from [user] where parentid='"+ParentId+"'";

                return Json(bll.GetBySQL(sql));
            }
        }
        [HttpPost]
        public  ActionResult Update()
        {
            Data.UserRow row;
            if (Query<string>(bll.GetKey()) == "-1" || Query<string>(bll.GetKey()) == null)
                row = bll.NewRow();
            else
                row = bll.GetByKey(Query<string>(bll.GetKey()));

            this.FillObject<Data.UserRow>(row);
            row.DeptId = row.DeptId.Replace("D", "");
            if (Query<string>(bll.GetKey()) == null || Query<string>(bll.GetKey()) == "-1")
            {
                bll.Add(row);
            }
            this.bll.Update(row);
            return JsonOb(true, "ok", row.UserId);
        }
    }
}