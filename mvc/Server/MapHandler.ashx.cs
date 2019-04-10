using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Xml;
using TY.Core;
using TY.UI.Ajax;
using WorkFlow;


namespace Web.Server
{
    /// <summary>
    /// MapHandler 的摘要说明
    /// </summary>
    public class MapHandler : AjaxHandler
    {
        private IBaseBLL<Data.BaseMapDataTable, Data.BaseMapRow> mapbll = DBFactory<Data.BaseMapDataTable, Data.BaseMapRow>.GetBLL();
        private IBaseBLL<Data.themeDataTable, Data.themeRow> themebll =  DBFactory<Data.themeDataTable, Data.themeRow>.GetBLL();
        private IBaseBLL<Data.regionDataTable, Data.regionRow> regionbll =  DBFactory<Data.regionDataTable, Data.regionRow>.GetBLL();

        private IBaseBLL<Data.LabelDataTable, Data.LabelRow> labelbll =  DBFactory<Data.LabelDataTable, Data.LabelRow>.GetBLL();
        private IBaseBLL<Data.label_imageDataTable, Data.label_imageRow> labelImagebll =  DBFactory<Data.label_imageDataTable, Data.label_imageRow>.GetBLL();
        private IBaseBLL<Data.UploadImageDataTable, Data.UploadImageRow> imgBll =  DBFactory<Data.UploadImageDataTable, Data.UploadImageRow>.GetBLL();
        public void getBaseMapList()
        {
            string sql = "select * from [BaseMap] order by idx";
            var list = mapbll.GetBySQL(sql);
            string mapData = JsonHelper.DataTableToJSON(list);

            base.jsonResult = JsonHelper.OutResult(true,"",mapData);
        }

        //加载所有专题列表
        public void getThemeList()
        {
            string sql = "select * from [theme] order by idx";
            var list = themebll.GetBySQL(sql);
            string mapData = JsonHelper.DataTableToJSON(list);

            base.jsonResult = JsonHelper.OutResult(true, "", mapData);

        }

        //根据专题类别加载专题
        public void getThemeListByGroupName()
        {
            string groupname = this.Query<string>("groupname");
            string sql = "select * from [theme] where groupname='"+groupname+"'";
            var list = themebll.GetBySQL(sql);
            string mapData = JsonHelper.DataTableToJSON(list);

            base.jsonResult = JsonHelper.OutResult(true, "", mapData);

        }

        public void getRegionList()
        {
            string sql = "select * from [region] where sysname = (select top 1 ConfigValue from [config] where configname='地区名')";
            var list = regionbll.GetBySQL(sql);
            string mapData = JsonHelper.DataTableToJSON(list);

            base.jsonResult = JsonHelper.OutResult(true, "", mapData);

        }

        //添加标签
        public void saveLabel()
        {
            try {
                Data.LabelRow data;


                if (Query<string>("id") == null)
                {
                    data = this.labelbll.NewRow();
                    data.id = Guid.NewGuid().ToString();
                }
                else
                {
                    data = this.labelbll.GetByKey(Query<string>("id"));
                }
                
                data.UserId = UserAuth.UserID;
                data.status = 1;
                data.isShare=0;
                int type = 0;
                string typeName= this.Query<string>("typeName");
                if("点"==typeName)
                {
                    type =1;
                }else if("线"==typeName)
                {
                    type = 2;
                }else if("面"==typeName)
                {
                    type = 3;
                }
                else if("矩形"==typeName)
                {
                    type = 4;
                }
                else if ("圆" == typeName)
                {
                    type =5;
                }
                data.name= this.Query<string>("name");
                data.type = type;
                data.latlng= this.Query<string>("latlng");
                data.labelDescribe= this.Query<string>("labelDescribe");
                data.createTime = DateTime.Now;
                data.isShare = this.Query<int>("isshare");
                if (Query<string>("id") == null)
                {
                    labelbll.Add(data);
                }
                else
                {
                    labelbll.Update(data);
                }
                
                jsonResult = JsonHelper.OutResult(true,"保存成功！");
            }
            catch (Exception e)
            {
                base.jsonResult = JsonHelper.OutError(e.Message);
            }
          
        }
       public void uploadLableInfo()
        {
            try {
                Data.LabelRow data;

                data = this.labelbll.NewRow();

                data.id = Guid.NewGuid().ToString();
               // data.UserId= this.Query<int>("userId");
                data.UserId = UserAuth.UserID;
                data.status = 1;
                data.isShare=1;
                int type = 0;
                type = this.Query<int>("type");
           
                data.name= this.Query<string>("name");
                data.type = type;
                data.latlng= this.Query<string>("latlng");
                data.labelDescribe= this.Query<string>("labelDescribe");
                data.createTime = DateTime.Now;
                
                labelbll.Add(data);
                jsonResult = JsonHelper.OutResult(true,"保存成功！");
            }
            catch (Exception e)
            {
                base.jsonResult = JsonHelper.OutError(e.Message);
            }
          
        }
        //编辑标签保存
        public void editLabelSave()
        {
            try {
                Data.LabelRow data;
                data = this.labelbll.GetByKey(this.Query<string>("id"));
                this.FillObject<Data.LabelRow>(data);
                labelbll.Update(data);

                jsonResult = JsonHelper.OutResult(true,"保存成功！");
            } catch(Exception e)
            {
                base.jsonResult = JsonHelper.OutError(e.Message);

            }
        }

        //删除标签
        public void deleteLabel()
        {
            string selected = this.Query<string>("selected");
            if (UserAuth.User.UserType != "管理员")
            {
                string sql = "select count(*) from [label]  where id in (" + selected + ") and userid<>"+UserAuth.UserID;
                if (labelbll.ExecuteScalar(sql).ToString() != "0")
                {
                    jsonResult = JsonHelper.OutResult(false, "只能删本人的。");
                    return;
                }
            }
            this.labelbll.ExecuteNonQuery("delete  from  [label] where id in (" + selected + ")");
            jsonResult = JsonHelper.OutResult(true, "删除成功!");
        }

        //标签列表
        public void getLabelList()
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string keyword = this.Query<string>("keyword");
            string isShare= this.Query<string>("isshare");
            if (keyword == null)
                keyword = "";
            string where = "";
            if (isShare == "")
            {
                where = "(isShare = 1 or [label].UserId = '" + UserAuth.UserID + "')";
            }
            if (isShare == "1")
            {
                where = "isShare = 1";
            }
            if (isShare == "0")
            {
                where = " [label].UserId = '" + UserAuth.UserID + "'";
            }
            string sql = "select * from [label] left join [user] on [label].UserId=[user].UserId where name like '%" + keyword + "%' and  "+where+" order by createTime desc";
            PagedData ob = labelbll.GetPagedDataTable(sql, pageSize, pageindex);
            jsonResult = ob.ToPagedJson();
            //return labelbll.GetBySQL(sql);

        }
        public void getLableList4old()
        {
            string userid = UserAuth.UserID.ToString();
            if (this.Query<string>("userid") != null)
            {
                userid = this.Query<string>("userid");
            }
            string sql = "select * from [label] where isshare=0 and userid='"+ userid + "' order by createtime desc";
            var table = labelbll.GetBySQL(sql);
            string sql2 = "select * from [label] where isshare=1 order by createtime desc";
            var table2 = labelbll.GetBySQL(sql);
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["labelArray"] = JsonHelper.DataTableToList(table);
            Dictionary<string, object> dic2 = new Dictionary<string, object>();
            dic["labelShareArray"] = JsonHelper.DataTableToList(table2);
            jsonResult = JsonHelper.ObjectToJSON(dic);
        }

        public void get360PointList()
        {
            string sql = "select * from [360point]";
            var list = regionbll.GetBySQL(sql);
            string mapData = JsonHelper.DataTableToJSON(list);

            base.jsonResult = JsonHelper.OutResult(true, "", mapData);

        }


   

        /// <summary>
        /// //将XML格式数据转换成JSON格式
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="groupname"></param>
        /// <returns></returns>
        public String readGeogisFromContentForAnalysis(String strContent, string groupname)
        {

            //JSONObject mainObject = new JSONObject();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strContent);
            string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc);
            return json;

        }


        /// <summary>
        /// 过滤请求回来的xml，将报错信息和报错地块去掉
        /// </summary>
        /// <param name="sTotalString"></param>
        /// <returns></returns>
        private String filterXML(String sTotalString)
        {
            if (sTotalString.IndexOf("</gml:featureMember>") != -1)
            {
                return sTotalString.Substring(0, sTotalString.IndexOf("</gml:featureMember>") + 20) + filterXML(sTotalString.Substring(sTotalString.IndexOf("</gml:featureMember>") + 20));
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public string submitPost(string strUrl, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            //request.CookieContainer = Cookie;
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //response.Cookies = Cookie.GetCookies(response.ResponseUri);
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        //public void ImportRegionLatlng()
        //{
        //    string filePath = HttpContext.Current.Server.MapPath("~/upload/区划/区划.shp");
        //    ADOBaseBLL<Data.regionDataTable, Data.regionRow> regionbll = new ADOBaseBLL<Data.regionDataTable, Data.regionRow>(new DBContext());
        //    var data = regionbll.GetBySQL("select * from region");
        //    ShapeFile sf = new ShapeFile(filePath);
        //    for (int i = 0; i < sf.RecordCount; i++)
        //    {
        //        if (i == data.Count)
        //            break;
        //        var s = sf.GetShapeData(i);
        //        string latlng = "";
        //        foreach (var p in s[0])
        //        {
        //            if (latlng != "")
        //                latlng += ";";
        //            latlng += p.X + "," + p.Y;
        //        }
        //        data[i].latlng = latlng;
        //    }
        //    regionbll.Update(data);
        //    jsonResult = JsonHelper.OutResult(true, "ok");
        //}

        public void GetWallMapUrl()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            string wallmap = this.Query<string>("wallmap").Replace("居委会", "社区");
            var table = regionbll.GetBySQL("select * from region where wallmap = '" + wallmap + "'");
            if (table.Rows.Count == 0)
            {

                dic["reason"] = "没有找到";
            }
            else
            {
                dic["result"] = true;
                
                string AppPath = "";
                string UrlAuthority = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                if (HttpContext.Current.Request.ApplicationPath == null || HttpContext.Current.Request.ApplicationPath == "/")
                    //直接安装在   Web   站点   
                    AppPath = UrlAuthority;
                else
                    //安装在虚拟子目录下   
                    //AppPath = UrlAuthority + HttpContext.Current.Request.ApplicationPath+":"+ HttpContext.Current.Request.Url.Port;
                dic["url"] = "/wallmap/"+table[0].wallmap+"/WallmapPointConf.xml";
            }
            jsonResult = JsonHelper.ObjectToJSON(dic);
        }

        public void UploadPhoto()
        {
            string outid = Query<string>("outid");
            string tableName = Query<string>("tableName");
            string imageName = Query<string>("imageName");
            string imageDescribe = Query<string>("imageDescribe");
            string latlng = Query<string>("latlng");

            
            
            



            string file = "";
            int count = HttpContext.Current. Request.Files.Count;
            

            for (int i = 0; i < count; i++)
            {

                var newid = Guid.NewGuid().ToString();
                int l = HttpContext.Current.Request.Files["uploadkey" + (i + 1)].ContentLength;
                byte[] buffer = new byte[l];
                Stream s = HttpContext.Current.Request.Files["uploadkey" + (i + 1)].InputStream;
                System.Drawing.Bitmap image = new System.Drawing.Bitmap(s);
                string imgname = newid+ ".jpg";
                string path = "~/upload/Images/" ;
                if (!Directory.Exists(HttpContext.Current.Server.MapPath(path)))
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));
                }
                image.Save(HttpContext.Current.Server.MapPath(path + imgname));


                var row = imgBll.NewRow();
                row.id = newid;
                row.tableName = tableName;
                row.outid = outid;
                row.latlng = latlng;
                row.uploadTime = DateTime.Now.ToString();
                row.userid = UserAuth.UserID.ToString();
                row.imageName = imageName;
                row.imageDescribe = imageDescribe;
                row.imageSize = image.Width + "X" + image.Height;
                imgBll.Add(row);
            }

        }

        public void getPhotoList()
        {
            string outid = Query<string>("outid");
            var data =  imgBll.GetBySQL("select * from uploadimage where outid='" + outid + "' order by uploadtime desc");
            jsonResult = JsonHelper.DataTableToJSON(data);
        }

        public void delImg()
        {
            string idlist = Query<string>("idlist");
            imgBll.ExecuteNonQuery("delete from uploadimage where id in (" + idlist + ")");
            jsonResult = JsonHelper.OutResult(true, "ok");
        }
    }
}