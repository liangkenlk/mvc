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
    public class ArrangeController : BaseController<Data.ArrangeDataTable, Data.ArrangeRow>
    {
        IBaseBLL<Data.UserDataTable, Data.UserRow> userBll = new ADOBaseBLL<Data.UserDataTable, Data.UserRow>(new DBContext());
        IBaseBLL<Data.ArrangeRulesDataTable, Data.ArrangeRulesRow> ruleBll = new ADOBaseBLL<Data.ArrangeRulesDataTable, Data.ArrangeRulesRow>(new DBContext());
        string a = "08:00";
        string b = "12:00";
        string c = "15:00";
        string d = "18:00";
        string e = "21:30";
        public ActionResult GetList()
        {
            var start = this.Query<string>("start");
            var end = this.Query<string>("end");
            start = DateTime.Parse(start).ToString("yyyy-MM-dd");
            end = DateTime.Parse(end).ToString("yyyy-MM-dd");
            string sql = "select * from arrange where date>='" + start + "' and date<='" + end + "' order by date,username";
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
        public JsonResult AutoArrange()
        {
            var start = Query<string>("start");
            var end = Query<string>("end");
            start = DateTime.Parse(start).ToString("yyyy-MM-dd");
            end = DateTime.Parse(end).ToString("yyyy-MM-dd");
            for (var i = start; DateTime.Parse(i) <= DateTime.Parse(end);i= DateTime.Parse(i).AddDays(1).ToString("yyyy-MM-dd"))
            {
                ArrangeByDate(DateTime.Parse(i).ToString("yyyy-MM-dd"));
            }
            return JsonOb(true, "ok");
        }
        /// <summary>
        /// 2019-8-19是第一天
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Date"></param>
        void ArrangeByDate(string Date)
        {
            Date = DateTime.Parse(Date).ToString("yyyy-MM-dd");
            DateTime start = Convert.ToDateTime("2019-8-19");
            DateTime end = Convert.ToDateTime(Date);
            TimeSpan sp = start.Subtract(end);
            int dayIndex = sp.Days % 6;
            var users = userBll.GetBySQL("select * from [user]");
            var rules = ruleBll.GetBySQL("select * from arrangerules");
            var exist = bll.GetBySQL("select * from arrange where date='"+Date+"' and worktime!='休息'");
            Data.ArrangeDataTable t = new Data.ArrangeDataTable();
            foreach (var u in users)
            {
                if (u.IsDutyTypeNull())
                    continue;
                if (u.DutyType == "四班")
                {
                    var rule = rules.Where(p => (p.DayIndex == dayIndex) && (p.DepId == u.DeptId)).FirstOrDefault();
                    if (rule == null)
                        continue;
                    foreach (string worktime in rule.WorkTime.Split('、'))
                    {
                        if (exist.Where(p => p.UserId == u.UserId.ToString() && p.WorkTime == worktime.Substring(0, 1)).Count() > 0)
                            continue;
                        var newrow = t.NewArrangeRow();
                        newrow.id = Guid.NewGuid().ToString();
                        newrow.Date = Date;
                        newrow.UserId = u.UserId.ToString();
                        newrow.UserName = u.UserName;
                        newrow.WorkTime = worktime.Substring(0, 1);
                        t.AddArrangeRow(newrow);
                    }
                }
                if (u.DutyType == "两班")//特勤中队
                {
                    var datetime = DateTime.Parse(Date);
                    if (datetime.DayOfWeek != DayOfWeek.Saturday && datetime.DayOfWeek != DayOfWeek.Sunday)
                    {
                        if (exist.Where(p => p.UserId == u.UserId.ToString() && p.WorkTime == "早").Count() == 0)
                        {
                            var newrow = t.NewArrangeRow();
                            newrow.id = Guid.NewGuid().ToString();
                            newrow.Date = Date;
                            newrow.UserId = u.UserId.ToString();
                            newrow.UserName = u.UserName;
                            newrow.WorkTime = "早";
                            t.AddArrangeRow(newrow);
                        }
                        if (exist.Where(p => p.UserId == u.UserId.ToString() && p.WorkTime == "下").Count() == 0)
                        {
                            var newrow = t.NewArrangeRow();
                            newrow.id = Guid.NewGuid().ToString();
                            newrow.Date = Date;
                            newrow.UserId = u.UserId.ToString();
                            newrow.UserName = u.UserName;
                            newrow.WorkTime = "下";
                            t.AddArrangeRow(newrow);
                        }
                    }
                }

               
            }
            bll.Update(t);    
        }
        [HttpPost]
        public  ActionResult Update()
        {
            Data.ArrangeRow row;
            if (Query<string>(bll.GetKey()) == null)
                row = bll.NewRow();
            else
                row = bll.GetByKey(Query<string>(bll.GetKey()));

            this.FillObject<Data.ArrangeRow>(row);
            if (Query<string>(bll.GetKey()) == null)
            {
                row.id = Guid.NewGuid().ToString();
                row.SignOnTime = DateTime.Now;
                row.UserId = UserAuth.UserID.ToString();
                row.UserName = UserAuth.UserName;
                bll.Add(row);
                
            }
            this.bll.Update(row);
            return JsonOb(true, "ok",row.id);
        }



        public ActionResult GetSignOnState()
        {
            //string re = "";
            //DateTime AMonT = DateTime.Parse( DateTime.Now.ToString("yyyy-MM-dd") + AMon);
            //DateTime AMoffT = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + AMoff);
            //DateTime PMonT = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + PMon);
            //DateTime PMoffT = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + PMoff);
            //var now = DateTime.Now;
            //Dictionary<string, bool> dic = new Dictionary<string, bool>();
            //if (now <= AMonT)
            //{
            //    re = "上班,正常";
            //}
            //if (now > AMoffT && now<PMonT)
            //{ }
            //if (now > PMonT)
            //{ }
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


        public ActionResult Exchange()
        {
            var id1 = this.Query<string>("id1");
            var id2 = this.Query<string>("id2");
            var row1 = bll.GetByKey(id1);
            var row2 = bll.GetByKey(id2);
            var tempid = row1.UserId;
            var tempname = row1.UserName;
            row1.UserId = row2.UserId;
            row1.UserName = row2.UserName;
            row2.UserName = tempname;
            row2.UserId = tempid;
            bll.Update(row1);
            bll.Update(row2);
            return JsonOb(true, "ok");

        }

        public DataTable RepDutyList1()
        {
            var start = Query<string>("start");
            var end = Query<string>("end");
            start = DateTime.Parse(start).ToString("yyyy-MM-dd");
            end = DateTime.Parse(end).ToString("yyyy-MM-dd");
            string sql = @"select b.UserName, @cols from vsignonbase b ";
            string cols = "";
            for (var i = start; DateTime.Parse(i) <= DateTime.Parse(end); i = DateTime.Parse(i).AddDays(1).ToString("yyyy-MM-dd"))
            {
                var j= i.Replace("-","");
                cols += "a"+j+ ".WorkTime,a" + j + ".SignOnTime,a" + j + ".SignAdd,";
                sql += @" left join  (select * from arrange where [Date] = '"+i+"') a"+j+" on b.userid=a"+j+ ".userid  and b.type=a" + j + ".WorkTime  ";
            }
            sql = sql.Replace("@cols", cols.TrimEnd(',')+" ")+ "order by b.userid";
            return bll.GetNormalDataTable(sql);
        }

        public DataTable RepDutyList()
        {
            var start = Query<string>("start");
            var end = Query<string>("end");
            start = DateTime.Parse(start).ToString("yyyy-MM-dd");
            end = DateTime.Parse(end).ToString("yyyy-MM-dd");
            string sql = "select * from arrange where"+ " Date < '" + end + "' and Date >= '" + start + "' order by date,userid";
            return bll.GetNormalDataTable(sql);
        }

            public ActionResult GetRepDutyList()
        {
            return JsonOb(this.RepDutyList());
        }

        public ActionResult Export()
        {
            var t = RepDutyList();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["Date"] = "日期";
            dic["UserName"] = "执勤人员";
            dic["WorkTime"] = "班次";
            dic["SignOnTime"] = "签到时间";
            dic["SignAdd"] = "签到地点";
            string filename = "执勤签到" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string path = Server.MapPath("~/temp/") + filename;
            new ExcelTool(dic).GridToExcelByNPOI(t, path);
            return JsonOb(true, "ok", filename);

        }


        public DataTable RepDutyCount()
        {
            var start = Query<string>("start");
            var end = Query<string>("end");
            start = DateTime.Parse(start).ToString("yyyy-MM-dd");
            end = DateTime.Parse(end).ToString("yyyy-MM-dd");
            string sql = @"select * from 
                    (select userid,username, COUNT(*) as countall  from Arrange where @where group by userid,username) a
                    left join 
                    (select userid,username,COUNT(*) as countsign from Arrange where @where and signontime is not null group by userid,username) b
                    on a.userid = b.userid
                    left join
                    (select userid,username,COUNT(*) as countnotsign from Arrange where  @where and signontime is  null group by userid,username) c
                    on a.userid = c.userid";
            var where = "  Date <'" + end + "' and Date >='" + start + "'";
            sql = sql.Replace("@where", where);
            return bll.GetNormalDataTable(sql);
        }

        public ActionResult GetRepDutyCount()
        {
            return JsonOb(this.RepDutyCount());
        }

        public ActionResult ExportRepDutyCount()
        {
            var t = RepDutyList();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["UserName"] = "执勤人员";
            dic["countall"] = "排班次数";
            dic["countsign"] = "签到次数";
            dic["countnotsign"] = "未签到次数";
            string filename = "执勤签到汇总" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string path = Server.MapPath("~/temp/") + filename;
            new ExcelTool(dic).GridToExcelByNPOI(t, path);
            return JsonOb(true, "ok", filename);

        }
    }
}