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
    public class TaskController : BaseController<Data.TaskDataTable,Data.TaskRow>
    {
        public ActionResult GetListByCaseId()
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string result = this.Query<string>("result");
            string caseid = this.Query<string>("caseid");
            if (string.IsNullOrEmpty(caseid))
                return null;
            string sql = "select * from [task] where caseid='"+caseid+"'";
            if ( !string.IsNullOrEmpty(result))
                sql += " and result = '" + result + "'";
            sql += " order by begintime desc";
            return JsonOb(bll.GetPagedDataTable(sql, pageSize, pageindex));
        }
        public ActionResult GetListByUser()
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string result = this.Query<string>("result");
            string caseid = this.Query<string>("caseid");

            string sql = "select Task.id, Task.Caseid, Task.Userid, Task.BeginTime, Task.EndTime, Task.Result, Task.Assigner, Task.remark, Task.UserName, [Case].id AS Expr1, [Case].CaseType, [Case].CaseSubType,[Case].Address, [Case].Status, [Case].X, [Case].Y, [Case].BeginTime AS Expr2, [Case].EndTime AS Expr3, [Case].Remark AS Expr4, [Case].Zone, [Case].UploaderId, [Case].Isdel,[Case].Uploader FROM Task LEFT OUTER JOIN [Case] ON[Case].id = Task.Caseid WHERE (1 = 1)";
           
                sql += " and [task].Userid = '" + UserAuth.UserID + "'";
            if (string.IsNullOrEmpty(result))
                sql += " and [task].result = '未处理'";
            else
                sql += " and [task].result = '" + result + "'";
            sql += " order by [task].begintime desc";
            return JsonOb(bll.GetPagedDataTable(sql, pageSize, pageindex));
        }

        [HttpPost]
        public new ActionResult Update()
        {
            Data.TaskRow row;
            if (Query<string>(bll.GetKey()) == null)
                row = bll.NewRow();
            else
                row = bll.GetByKey(Query<string>(bll.GetKey()));

            this.FillObject<Data.TaskRow>(row);
            if (Query<string>(bll.GetKey()) == null)
            {

                row.id = Guid.NewGuid().ToString();
                row.BeginTime = DateTime.Now;
                row.Result = "未处理";
                row.Assigner = UserAuth.UserName;
                //row. = UserAuth.UserName;
                bll.Add(row);
                

            }
            this.bll.Update(row);
            if (!row.IsUseridNull())
            {
                IBaseBLL<Data.UserDataTable, Data.UserRow> ubll = DBFactory<Data.UserDataTable, Data.UserRow>.GetBLL();
                var u = ubll.GetByKey(row.Userid);
                try
                {
                    GetuiServerApiSDKDemo.demo.PushMessageToSingle(u.OpenID, row.id);
                }
                catch { }
            }
            return JsonOb(true, "ok",row.id);
        }
    }
}