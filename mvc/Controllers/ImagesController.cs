using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TY.Core;
using WorkFlow;

namespace mvc.Controllers
{
    public class ImagesController : BaseController<Data.ImagesDataTable, Data.ImagesRow>
    {
        public ActionResult GetList()
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string Address = this.Query<string>("Address");
            string sql = "select * from [Case] where 1=1 ";
            if (Address != null)
                sql += " and Address like '%" + Address + "%'";
            sql += " and status='新建' ";
            sql += " order by begintime desc";
            return JsonOb(bll.GetPagedDataTable(sql, pageSize, pageindex));
        }
        [HttpPost]
        public new ActionResult Update()
        {
            Data.ImagesRow row;
            if (Query<string>(bll.GetKey()) == null)
                row = bll.NewRow();
            else
                row = bll.GetByKey(Query<string>(bll.GetKey()));

            this.FillObject<Data.ImagesRow>(row);
            if (Query<string>(bll.GetKey()) == null)
            {
                //row.id = Guid.NewGuid().ToString();
                //row.BeginTime = DateTime.Now;
                //row.UploaderId = UserAuth.UserID.ToString();
                //row.Status = "新建";
                bll.Add(row);
            }
            this.bll.Update(row);
            return JsonOb(true, "ok");
        }

        public ActionResult GetListByOutId()
        {
            string outid = this.Query<string>("outid");
            string sql = "select * from [images] where 1=1 ";
            if (outid != null)
                sql += " and outid = '" + outid + "'";
            else
                return null;
            return JsonOb(JsonHelper.DataTableToList( bll.GetBySQL(sql)));
        }
        [HttpPost]
        public ActionResult UploadFile()
        {
            try
            {
                string outid = Query<string>("outid");
                HttpFileCollectionBase files = HttpContext.Request.Files;
                if (files.Count > 0)
                {
                    string fileid = Guid.NewGuid().ToString();

                    for (int i = 0; i < files.Count; i++)
                    {
                        var row =   bll.NewRow();
                        row.id = Guid.NewGuid().ToString();
                        row.UpTime = DateTime.Now.ToString();
                        row.Outid = outid;
                        row.FileName = files[i].FileName;
                        bll.Add(row);
                        string path = HttpContext.Server.MapPath("~/upload") + @"\Case\"+outid;
                        string filename = path + @"\" +  Path.GetFileName(files[i].FileName);
                        Directory.CreateDirectory(path);
                        files[i].SaveAs(filename);
                    }
                    return JsonOb(HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + "/upload/Case/" +  Path.GetFileName(files[0].FileName));

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

        public FileResult Get1stPic()
        {
            try
            {
                var id = Query<string>("id");
                var files = Directory.GetFiles(Server.MapPath("~/upload/case/" + id));
                if (files.Length == 0)
                    return null;
                //var root = System.Web.HttpContext.Current.Server.MapPath("~/App_Data");
                //string fileName = name;
                //string path = Path.Combine(root, fileName);
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                //var stream = new FileStream(files[0], FileMode.Open);
                //result.Content = new StreamContent(stream);
                //result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                //result.Content.Headers.ContentDisposition.FileName = Path.GetFileName(files[0]);
                ////result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                //result.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(Path.GetFileName(files[0])));
                //result.Content.Headers.ContentLength = stream.Length;


                //string root = Server.MapPath("~/App_Data");
                string fileName = Path.GetFileName(files[0]);
                string filePath = files[0];
                string s = MimeMapping.GetMimeMapping(fileName);

                return File(files[0], s, Path.GetFileName(filePath));
            }
            catch {
                return null;
            }
        }

        public ActionResult delImg()
        {
            string idlist = Query<string>("idlist");
            bll.ExecuteNonQuery("delete from images where id in (" + idlist + ")");
            return JsonOb(true, "ok");
        }
    }
}