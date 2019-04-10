using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TY.Core;
using TY.Utility;
using WorkFlow;

namespace mvc.Controllers
{
    public class BaseController<TB,R> :Controller  where TB : DataTable where R : DataRow 
    {

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            string error = filterContext.Exception.Message;
            this.Write(JsonHelper.OutResult(false, error));
           
        }
        public IBaseBLL<TB, R> bll = DBFactory<TB, R>.GetBLL();
        public void FormToObject<T>()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
        }

        public ActionResult GetCurrentUserInfo()
        {
            if (!UserAuth.IsLogined())
                return JsonOb(false, "您未登录");
            return Json(JsonHelper.DataRowToDictionary( UserAuth.User), JsonRequestBehavior.AllowGet);
        }



        public T Query<T>()
        {
            return this.Query<T>("");
        }
        public T Query<T>(Enum key)
        {
            return this.Query<T>(key.ToString(), default(T));
        }

        public T Query<T>(string key)
        {
            return this.Query<T>(key, default(T));
        }

        public T Query<T>(string key, T defaultValue)
        {
            return WebHelper.Query<T>(key, defaultValue, false);
        }
        public void FillObject<T>(T obj)
        {
            WebHelper.FillObject<T>(obj);
        }

        protected void Write(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Replace("\t", " ").Replace("\r", " ").Replace("\n", "<br/>");
            }
            //Access - Control - Allow - Credentials: true
            //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            HttpContext.Response.Write(result);
        }

        protected void WriteError(string msg)
        {
            this.Write(JsonHelper.OutResult(false, msg));
        }



        public ActionResult Add(R row)
        {
            this.bll.Add(row);
            return JsonOb(true, "ok");
        }

        public ActionResult DeleteAll()
        {
            this.bll.DeleteAll();
            return JsonOb(true, "ok");
        }

        public ActionResult DeleteAll(string where)
        {
            this.bll.DeleteAll(where);
            return JsonOb(true, "ok");
        }

        public ActionResult Delete()
        {
            string ids = this.Query<string>("selected");
            this.bll.Detele(ids);
            return JsonOb(true, "ok");
        }

        public ActionResult ExecuteNonQuery(string sql)
        {
            this.bll.ExecuteNonQuery(sql);
            return JsonOb( true, "ok");
        }

        public ActionResult ExecuteScalar(string sql)
        {
            return JsonOb( this.bll.ExecuteScalar(sql));
        }

     

        public ActionResult GetByKey()
        {
            var key = Query<string>("key");
            var a =JsonHelper.DataRowToDictionary(this.bll.GetByKey(key));
            return JsonOb (a);
            
        }

        public ActionResult GetBySQL(string sql)
        {
            return JsonOb(this.bll.GetBySQL(sql));
        }

        public string GetKey(string TableName = null)
        {
            return this.bll.GetKey(TableName);
        }

        public ActionResult GetNormalDataTable(string sql)
        {
            return JsonOb(true, "ok");
        }

 


        public ActionResult GetPagedDataTable(string sql, int PageSize, int PageIndex)
        {

            return Json(this.bll.GetPagedDataTable(sql, PageSize, PageIndex), JsonRequestBehavior.AllowGet);
        }

        public R NewRow()
        {
            return this.bll.NewRow();
        }



        public ActionResult Update(R row)
        {
            this.bll.Update(row);
            return JsonOb(true,"ok");
        }
        //[HttpPost]
        //public  ActionResult Update()
        //{
        //    R row;
        //    if (Query<string>(bll.GetKey()) == null)
        //        row = bll.NewRow();
        //    else
        //        row = bll.GetByKey(Query<string>(bll.GetKey()));

        //    this.FillObject<R>(row);
        //    if (Query<string>(bll.GetKey()) == null)
        //        bll.Add(row);
        //    this.bll.Update(row);
        //    return JsonOb(true, "ok");
        //}

        protected JsonResult JsonOb(bool success, string msg)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("Success", success);
            dictionary.Add("Message", msg);
            return Json(dictionary, JsonRequestBehavior.AllowGet);

        }

        protected JsonResult JsonOb(bool success, string msg,object data)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("Success", success);
            dictionary.Add("Message", msg);
            dictionary.Add("data", data);
            return Json(dictionary, JsonRequestBehavior.AllowGet);

        }

        protected JsonResult JsonOb(Object ob)
        {
            //JsonSerializerSettings setting = new JsonSerializerSettings()
            //{
            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //};

            //var ret = JsonConvert.SerializeObject(ob, setting);

            return Json(ob,JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActionList()
        {
            var t = this.GetType();
            MethodInfo [] mlist = t.GetMethods();

            string[] mstrlist = new string[mlist.Length];
            for (int i= 0;i< mlist.Length;i++)
            {
                mstrlist[i] = mlist[i].ToString();
            }
            return JsonOb(mstrlist);
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new ToJsonResult
            {
                Data = data,
                ContentEncoding = contentEncoding,
                ContentType = contentType,
                JsonRequestBehavior = behavior,
                FormateStr = "yyyy-MM-dd HH:mm:ss"
            };
        }

        /// <summary>
        /// 说明：转化为Jason
        /// 作者: CallmeYhz
        /// </summary>
        public class ToJsonResult : JsonResult
        {
            const string error = "该请求已被封锁，因为敏感信息透露给第三方网站，这是一个GET请求时使用的。为了可以GET请求，请设置JsonRequestBehavior AllowGet。";
            /// <summary>
            /// 格式化字符串
            /// </summary>
            public string FormateStr
            {
                get;
                set;
            }

            /// <summary>
            /// 说明：重写ExecueResult方法
            /// 作者：CallmeYhz    
            /// </summary>
            /// <param name="context"></param>
            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }
                if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                    String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(error);
                }

                HttpResponseBase response = context.HttpContext.Response;

                if (!String.IsNullOrEmpty(ContentType))
                {
                    response.ContentType = ContentType;
                }
                else
                {
                    response.ContentType = "application/json";
                }
                if (ContentEncoding != null)
                {
                    response.ContentEncoding = ContentEncoding;
                }
                if (Data != null)
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string jsonstring = serializer.Serialize(Data);


                    //string hashOldPassword = @"\\/Date\((\param+)\+\param+\)\\/";

                    string p = @"\\/Date\(\d+\)\\/";

                    MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);

                    Regex reg = new Regex(p);

                    jsonstring = reg.Replace(jsonstring, matchEvaluator);
                    response.Write(jsonstring);
                }
            }

            /// <summary>
            /// 说明：将Json序列化的时间由/Date(1294499956278+0800)转为字符串
            /// 作者：CallmeYhz   
            /// </summary>
            private string ConvertJsonDateToDateString(Match m)
            {

                string result = string.Empty;

                string p = @"\d";
                var cArray = m.Value.ToCharArray();
                StringBuilder sb = new StringBuilder();

                Regex reg = new Regex(p);
                for (int i = 0; i < cArray.Length; i++)
                {
                    if (reg.IsMatch(cArray[i].ToString()))
                    {
                        sb.Append(cArray[i]);
                    }
                }
                // reg.Replace(m.Value;

                DateTime dt = new DateTime(1970, 1, 1);

                dt = dt.AddMilliseconds(long.Parse(sb.ToString()));

                dt = dt.ToLocalTime();

                result = dt.ToString(this.FormateStr);

                return result;
            }
        }
    }
}