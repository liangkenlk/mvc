using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TY.Core;
using Web.lib;
using WorkFlow;

namespace mvc.Controllers
{
    public class SignOnController : BaseController<Data.SignOnDataTable, Data.SignOnRow>
    {
        string AMon = "08:00";
        string AMoff = "12:00";
        string PMon = "15:00";
        string PMoff = "18:00";
        public ActionResult GetList()
        {
            var start = this.Query<DateTime>("start");
            var end = this.Query<DateTime>("end");
            string sql = "select * from signon where signtime<='" + start + "' and signtime<'" + end + "' order by signtime,username";
            var table= bll.GetBySQL(sql);
            //DataTable re = new DataTable();
            //re.Columns.Add("UserName");
            //re.PrimaryKey = new DataColumn[] { re.Columns[0] };
            //for (var i = start; i < end; i=i.AddDays(1))
            //{
            //    re.Columns.Add(i.ToString("yyyy-MM-dd"));
            //}
            //foreach (var row in table)
            //{
            //    DataView view = re.AsDataView();
            //    view.RowFilter = "UserName='" + row.UserName + "'";
            //    var reRows = view.FindRows(row.UserName);
            //    if (reRows.Length > 0)
            //    {
            //         reRows[0][row.SignTime.ToString("yyyy-MM-dd")] = row.SignTime;
            //    }
            //}
            return JsonOb(bll.GetBySQL(sql));
        }
        [HttpPost]
        public  ActionResult Update()
        {
            Data.SignOnRow row;
            if (Query<string>(bll.GetKey()) == null)
                row = bll.NewRow();
            else
                row = bll.GetByKey(Query<string>(bll.GetKey()));

            this.FillObject<Data.SignOnRow>(row);
            if (Query<string>(bll.GetKey()) == null)
            {
                row.id = Guid.NewGuid().ToString();
                row.SignTime = DateTime.Now;
                row.UserId = UserAuth.UserID.ToString();
                row.UserName = UserAuth.UserName;
                bll.Add(row);
                
            }
            this.bll.Update(row);
            return JsonOb(true, "ok",row.id);
        }



        public ActionResult GetSignOnState()
        {
            string re = "";
            DateTime AMonT = DateTime.Parse( DateTime.Now.ToString("yyyy-MM-dd") + AMon);
            DateTime AMoffT = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + AMoff);
            DateTime PMonT = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + PMon);
            DateTime PMoffT = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + PMoff);
            var now = DateTime.Now;
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            if (now <= AMonT)
            {
                re = "上班,正常";
            }
            if (now > AMoffT && now<PMonT)
            { }
            if (now > PMonT)
            { }
            return JsonOb(null);
        }

        public ActionResult ExportExcel()
        {
            var start = Query<string>("start");
            var end = Query<string>("end");
            var sql = @"  select [User].username,[User].userid,casecount,checkcount from[User] left join
                    (select Uploader as username, UploaderId as userid, COUNT(*) as casecount from[Case] @where1 group by Uploader, UploaderId ) aa
                     on[user].UserId = aa.userid
                    left join
                    (select UserName, UserId, COUNT(*) as checkcount from[Check] @where2 group by UserId, UserName ) b
                     on[user].UserId = b.UserId";
            var where1 = "where begintime <'" + end + "' and begintime >='" + start + "'";
            var where2 = "where checktime <'" + end + "' and checktime >='" + start + "'";
            sql = sql.Replace("@where1", where1).Replace("@where2", where2);
            var table = bll.GetNormalDataTable(sql);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["username"] = "人员";
            dic["casecount"] = "上报数";
            dic["checkcount"] = "处理数";
            string filename = "统计结果" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string path = Server.MapPath("~/temp/") + filename;
            new ExcelTool(dic).GridToExcelByNPOI(table, path);
            return JsonOb(true, "ok", filename);

        }

    }
}