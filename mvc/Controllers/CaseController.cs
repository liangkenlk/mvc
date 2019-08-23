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
    public class CaseController : BaseController<Data.CaseDataTable,Data.CaseRow>
    {
        public ActionResult GetList()
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string Address = this.Query<string>("Address");
            string KeyWord = this.Query<string>("keyword");
            string status = this.Query<string>("status");
            string sql = "select * from [Case] where 1=1 ";
            if (Address != null)
                sql += " and Address like '%" + Address + "%'";
            if (KeyWord != null)
                sql += " and ( Address like '%" + KeyWord + "%' or remark like '%" + KeyWord + "%')";
            if(status != null && status !="null")
                sql += " and status='"+ status + "' ";
            sql += " order by begintime desc";
            return JsonOb(bll.GetPagedDataTable(sql, pageSize, pageindex));
        }
        [HttpPost]
        public new ActionResult Update()
        {
            Data.CaseRow row;
            if (Query<string>(bll.GetKey()) == null)
                row = bll.NewRow();
            else
                row = bll.GetByKey(Query<string>(bll.GetKey()));

            this.FillObject<Data.CaseRow>(row);
            if (Query<string>(bll.GetKey()) == null)
            {
                row.id = Guid.NewGuid().ToString();
                row.BeginTime = DateTime.Now;
                row.Status = "需处理";
                row.UploaderId = UserAuth.UserID.ToString();
                row.Uploader = UserAuth.UserName;
                bll.Add(row);
                
            }
            this.bll.Update(row);
            return JsonOb(true, "ok",row.id);
        }

        public ActionResult GetCount()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string sql = "select count(*) from [case] where status='审核'";
            var ob1 = bll.ExecuteScalar(sql).ToString();
            sql = "select count(*) from [case] where status='立案'";
            var ob2 = bll.ExecuteScalar(sql).ToString();
            sql = "select count(*) from [case] where status='处置'";
            var ob3 = bll.ExecuteScalar(sql).ToString();
            sql = "select count(*) from task where userid='"+UserAuth.UserID+"' and  result='未处理'";
            var ob4 = bll.ExecuteScalar(sql).ToString();
            dic["审核"] = ob1;
            dic["立案"] = ob2;
            dic["处置"] = ob3;
            dic["任务"] = ob4;
            return JsonOb(dic);
        }
        public ActionResult UploadFile()
        {
            try
            {
                string postId = Query<string>("id");
                HttpFileCollectionBase files = HttpContext.Request.Files;
                if (files.Count > 0)
                {
                    string fileid = Guid.NewGuid().ToString();

                    for (int i = 0; i < files.Count; i++)
                    {
                        string path = HttpContext.Server.MapPath("~/upload") + @"\Case";
                        string filename = path + @"\" + fileid + Path.GetExtension(files[i].FileName);
                        Directory.CreateDirectory(path);
                        files[i].SaveAs(filename);
                        
                    }
                    return JsonOb(HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + "/upload/Case/" + fileid + Path.GetExtension(files[0].FileName));

                }
                else
                {
                    return JsonOb(false, "没有收到文件！");
                }
            }
            catch (Exception exception)
            {
                return JsonOb(false,exception.Message);
            }
        }
        public ActionResult AppVersion()
        {
            string ver = System.Configuration.ConfigurationSettings.AppSettings["appversion"].ToString();
            return JsonOb(true, ver);
        }

        public ActionResult Count()
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
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
            return JsonOb(bll.GetPagedDataTable(sql, pageSize, pageindex));

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

        public DataTable typeCount()
        {
            var start = Query<string>("start");
            var end = Query<string>("end");
            var sql = @"select a.CaseBigType,a.casetype,a.casesubtype,casecount,casecount1,casecount2,casecount3,casecount4  from (select Casebigtype,casetype,CaseSubType,count(*) as casecount from [case] where @where1 group by CaseBigType,CaseType,CaseSubType) as  a
                        left  join 
                        (select Casebigtype,casetype,CaseSubType,count(*) as casecount1 from [case] where Status='审核' and @where1  group by CaseBigType,CaseType,CaseSubType) as b
                        on a.casebigtype=b.casebigtype and a.casetype=b.casetype and a.casesubtype=b.casesubtype
                        left join 
                        (select Casebigtype,casetype,CaseSubType,count(*) as casecount2 from [case] where Status='立案' and @where1   group by CaseBigType,CaseType,CaseSubType) as c
                        on a.casebigtype=c.casebigtype and a.casetype=c.casetype and a.casesubtype=c.casesubtype
                        left join
                        (select Casebigtype,casetype,CaseSubType,count(*) as casecount3 from [case] where Status='处置' and @where1   group by CaseBigType,CaseType,CaseSubType) as d
                        on a.casebigtype=d.casebigtype and a.casetype=d.casetype and a.casesubtype=d.casesubtype
                        left join
                        (select Casebigtype,casetype,CaseSubType,count(*) as casecount4 from [case] where Status='结案' and @where1 group by CaseBigType,CaseType,CaseSubType) as e
                        on a.casebigtype=e.casebigtype and a.casetype=e.casetype and e.casesubtype=d.casesubtype";
            var where1 = " begintime <'" + end + "' and begintime >='" + start + "'";
            sql = sql.Replace("@where1", where1);
            var table = bll.GetNormalDataTable(sql);
            return table;
        }

        public ActionResult GetTypeCount()
        {
            return JsonOb(this.typeCount());
        }
        public ActionResult ExportTypeCount()
        {
            var t = this.typeCount();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["CaseBigType"] = "大类";
            dic["casetype"] = "类型";
            dic["casesubtype"] = "小类";
            dic["casecount"] = "总数";
            dic["casecount1"] = "需审核";
            dic["casecount2"] = "已立案";
            dic["casecount3"] = "已处理";
            dic["casecount4"] = "已结案";
            string filename = "事件问题分类统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string path = Server.MapPath("~/temp/") + filename;
            new ExcelTool(dic).GridToExcelByNPOI(t, path);
            return JsonOb(true, "ok", filename);
        }
    }
}