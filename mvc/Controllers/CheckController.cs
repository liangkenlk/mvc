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
    public class CheckController : BaseController<Data.CheckDataTable,Data.CheckRow>
    {
        public ActionResult GetListByCaseId()
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");

            string caseid = this.Query<string>("caseid");
            if (string.IsNullOrEmpty(caseid))
                return null;
            string sql = "select * from [check] where caseid='" + caseid + "'";

            sql += " order by checktime desc";
            return JsonOb(bll.GetPagedDataTable(sql, pageSize, pageindex));
        }
        [HttpPost]
        public new ActionResult Update()
        {
            Data.CheckRow row;
            if (Query<string>(bll.GetKey()) == null)
                row = bll.NewRow();
            else
                row = bll.GetByKey(Query<string>(bll.GetKey()));

            this.FillObject<Data.CheckRow>(row);
            if (Query<string>(bll.GetKey()) == null)
            {

                row.id = Guid.NewGuid().ToString();
                row.CheckTime = DateTime.Now;
                
                row.UserId = UserAuth.UserID.ToString();
                row.UserName = UserAuth.UserName;
                //row. = UserAuth.UserName;
                bll.Add(row);

                //IBaseBLL<Data.TaskDataTable, Data.TaskRow> taskBll = DBFactory<Data.TaskDataTable, Data.TaskRow>.GetBLL();
                string sql = "update task set result='已处理',endtime=GETDATE() where userid='" + row.UserId + "' and caseid='" + row.Caseid + "' and result='未处理'";
                string sql2 = "update [case] set status='已处理' where  id='" + row.Caseid + "'";
                bll.ExecuteNonQuery(sql);
                bll.ExecuteNonQuery(sql2);

            }
            this.bll.Update(row);
            return JsonOb(true, "ok",row.id);
        }
    }
}