namespace TY.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Web;
    using System.Web.SessionState;
    using TY.Utility;
    using WxPayAPI;

    public abstract class AjaxBase : IHttpHandler, IRequiresSessionState, IBase
    {
      
        private string _OutputExcelName;
        protected bool CancelInvoke = false;
        protected MemoryStream excelStream = null;
        public string jsonResult = JsonHelper.OutResult(false, "未初始化");
        private Dictionary<string, string> keyValue = new Dictionary<string, string>();
        private static Dictionary<string, MemoryStream> streamList = new Dictionary<string, MemoryStream>();

        protected AjaxBase()
        {
        }

        public abstract void BeforeInvoke();
        public void FormToObject<T>()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
        }

        public void GetCurrentUserInfo()
        {
            this.jsonResult = JsonHelper.ObjectToJSON(UserAuth.User);
        }

        public string GetMethodList()
        {
            MethodInfo[] methods = base.GetType().GetMethods();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string str = "";
            foreach (MethodInfo info in methods)
            {
                DescriptionAttribute[] customAttributes = (DescriptionAttribute[]) info.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (customAttributes.Length != 0)
                {
                    string description = customAttributes[0].Description;
                    string str4 = str;
                    str = str4 + "</b>\n" + info.Name + ": " + description;
                }
            }
            return str;
        }

        protected string GetOrderBy(string defaultSort)
        {
            string sort = this.Sort;
            if (string.IsNullOrEmpty(sort))
            {
                sort = defaultSort;
            }
            if (!string.IsNullOrEmpty(sort))
            {
                return string.Format(" order by {0} {1} ", sort, this.Order);
            }
            return "";
        }

        public string GetWhereIn(string primaryKey, string requestKey = null)
        {
            string getID = string.Empty;
            if (!string.IsNullOrEmpty(requestKey))
            {
                getID = this.Query<string>(requestKey);
            }
            else
            {
                getID = this.GetID;
            }
            if (string.IsNullOrEmpty(getID))
            {
                return getID;
            }
            string[] strArray = getID.Split(new char[] { ',' });
            getID = string.Empty;
            foreach (string str2 in strArray)
            {
                getID = getID + "'" + str2.Trim(new char[] { '\'' }) + "',";
            }
            return string.Format("[{0}] in ({1})", primaryKey, getID.TrimEnd(new char[] { ',' }));
        }

        public void MethodInvoke()
        {
            string str = this.Query<string>("method");
            if (str == null)
                str = this.Query<string>("REQUEST");
            if (string.IsNullOrEmpty(str))
            {
                this.jsonResult = JsonHelper.OutResult(false, "method can't be empty");
                this.jsonResult = this.jsonResult + this.GetMethodList();
                this.Write(this.jsonResult);
            }
            else
            {
                MethodInfo method = base.GetType().GetMethod(str);
                if (method == null)
                {
                    this.jsonResult = JsonHelper.OutResult(false, "not found the method : " + str);
                    this.jsonResult = this.jsonResult + this.GetMethodList();
                    this.Write(this.jsonResult);
                }
                else
                {
                    //if ((!(UserAuth.IsExistsAppToken() || (",Login,Register,VersionRequest,CreateMsgCode,VaildateMsgCode,".IndexOf("," + method.Name + ",") != -1)) && HttpContext.Current.Request.Url.ToString().ToLower().IndexOf("consolehandler.ashx") == -1))
                    //{
                    //    this.jsonResult = JsonHelper.OutResult(false, "你未登录！");
                    //    this.Write(this.jsonResult);
                    //    return;
                    //}
                    try
                    {
       
                     
                        object obj2 = method.Invoke(this, null);
                        //Log.WriteLog("用户操作", UserAuth.UserName,method.Name);
                    }
                    catch (ThreadAbortException)
                    {
                        return;
                    }
                    catch (Exception exception)
                    {
                        //Log.Error(method.Name, exception.InnerException.Message);
                       this.jsonResult = JsonHelper.OutResult(false, exception.Message);
                    }
                }
                if (this.excelStream != null)
                {
                    string key = this.OutputExcelName + "#" + DateTime.Now.Ticks;
                    streamList.Add(key, this.excelStream);
                    this.jsonResult = "{\"downcode\":\"" + key + "\"," + this.jsonResult.Substring(1);
                }
                this.Write(this.jsonResult);
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            this.BeforeInvoke();
            this.MethodInvoke();
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
        protected void SetError(string msg)
        {
            this.jsonResult = JsonHelper.OutResult(false, msg);
        }

        public void SetKeyValue(Enum key, string value)
        {
            this.SetKeyValue(key.ToString(), value);
        }

        public void SetKeyValue(string key, string value)
        {
            if (!this.keyValue.ContainsKey(key))
            {
                this.keyValue.Add(key, value);
            }
        }

        protected void Write(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Replace("\t", " ").Replace("\r", " ").Replace("\n", "<br/>");
            }
            //Access - Control - Allow - Credentials: true
            //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            HttpContext.Current.Response.Write(result);
        }

        protected void WriteError(string msg)
        {
            this.Write(JsonHelper.OutResult(false, msg));
        }

        public string GetID
        {
            get
            {
                string str = this.Query<string>("id");
                if (string.IsNullOrEmpty(str) && (HttpContext.Current.Request.QueryString.Keys.Count > 0))
                {
                    string key = HttpContext.Current.Request.QueryString.Keys[0];
                    if (key.ToLower().Contains("id"))
                    {
                        return this.Query<string>(key, string.Empty);
                    }
                }
                return str;
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }



        public string Order
        {
            get
            {
                return this.Query<string>("order", "desc");
            }
        }

        protected string OutputExcelName
        {
            get
            {
                if (string.IsNullOrEmpty(this._OutputExcelName))
                {
                    if (HttpContext.Current.Request.Files.Count > 0)
                    {
                        this._OutputExcelName = HttpContext.Current.Request.Files[0].FileName.Replace(".xls", "_错误信息.xls");
                        this._OutputExcelName = Path.GetFileName(this._OutputExcelName);
                    }
                    else
                    {
                        this._OutputExcelName = "Excel导入错误信息.xls";
                    }
                }
                return this._OutputExcelName;
            }
            set
            {
                this._OutputExcelName = value;
            }
        }

        public int PageIndex
        {
            get
            {
                return this.Query<int>("page");
            }
        }

        public int PageSize
        {
            get
            {
                return this.Query<int>("rows");
            }
        }

        public string Sort
        {
            get
            {
                return this.Query<string>("sort", "");
            }
        }


    }
}

