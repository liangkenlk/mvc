using System;
using System.Collections.Generic;
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
    public class ManualController : BaseController<Data.ManualDataTable,Data.ManualRow>
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

        public ActionResult GetByTypes()
        {

            var t = myGetByTypes();
            if (t == null)
                return JsonOb(false, "");
            //t.Content = t.Content.Replace("\n", "<br>").Replace(" ","&nbsp;");
            var dic = JsonHelper.DataRowToDictionary(t);
            dic["Content2"] = t.Content.Replace("\n", "<br>").Replace(" ", "&nbsp;");
            return JsonOb(true,"ok",dic);

        }

        Data.ManualRow myGetByTypes()
        {
            string BigType = this.Query<string>("BigType");
            string Type = this.Query<string>("Type");
            string SubType = this.Query<string>("SubType");
            var sql = "select * from manual where bigtype='" + BigType + "' and Type='" + Type + "' and SubType='" + SubType + "'";
            var t= bll.GetBySQL(sql);
            if (t.Count() > 0)
                return t[0];
            else
                return null;
        }
        [HttpPost]
        public new ActionResult Update()
        {
            Data.ManualRow row;
            var t = myGetByTypes();
            if (t== null)
                row = bll.NewRow();
            else
                row = t;

            this.FillObject<Data.ManualRow>(row);
            if (t == null)
            {
                row.id = Guid.NewGuid().ToString();
                bll.Add(row);
                
            }
            this.bll.Update(row);
            return JsonOb(true, "ok",row.id);
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

    }
}