namespace TY.Core
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Web;
    using TY.Utility;
    using WorkFlow;
    using System.Net;
    using System.Net.Sockets;

    public class UserAuth
    {
        private static Dictionary<string, DateTime> appTokenList = new Dictionary<string, DateTime>();
        private static Dictionary<string, string> pcTokenList = new Dictionary<string, string>();
        private static Dictionary<int, bool> FreshList = new Dictionary<int, bool>();
        public static Dictionary<string, Dictionary<string, object>> Sessions = new Dictionary<string, Dictionary<string, object>>();
        public static Dictionary<int, Socket> SocketList = new Dictionary<int, Socket>();
        public static string access_token = "none";
        public static string jsapi_ticket = "none";
        public static DateTime getTokenTime = DateTime.Now;
        public static string GetAuthToken(string loginID, string password, out string errMsg)
        {
            string token = null;
            errMsg = string.Empty;
            string sql = "select * from [user] where loginid='" + loginID + "' and password='" + password + "'";
            Data.UserDataTable bySQL = new ADOBaseBLL<Data.UserDataTable, Data.UserRow>(new DBContext()).GetBySQL(sql);
            if (bySQL.Rows.Count == 0)
            {
                errMsg = "用户名或密码错误！";
                return token;
            }
            Data.UserRow row = bySQL.Rows[0] as Data.UserRow;
            
            token = EncrpytHelper.Encrypt(string.Concat(new object[] { row.UserId, ",", row.LoginID, ",", DateTime.Now.Day }));
            SetAppToken(token);
            token = HttpContext.Current.Server.UrlEncode(token);
            
            
            return token;
        }

        
        public static Dictionary<string,object> Session
        {
            get
            {
                if (HttpContext.Current.Session["openid"] == null)
                {
                    return new Dictionary<string, object>();
                }
                if (!Sessions.ContainsKey(HttpContext.Current.Session["openid"].ToString()))
                    Sessions[HttpContext.Current.Session["openid"].ToString()] = new Dictionary<string,object>();
                    return Sessions[HttpContext.Current.Session["openid"].ToString()];
        
            }
            set
            {
                
                Sessions[HttpContext.Current.Session["openid"].ToString()] = value;
            }
        }

        


        private static string GetCookieValue(string name)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null)
            {
                return cookie.Value;
            }
            return string.Empty;
        }

        public static int GetOnlineAppCount(int minutes)
        {
            int num = 0;
            try
            {
                if (appTokenList.Count <= 0)
                {
                    return num;
                }
                foreach (DateTime time in appTokenList.Values)
                {
                    if (time.AddMinutes((double) minutes) > DateTime.Now)
                    {
                        num++;
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return num;
        }

        public  static string GetTokenValue(int index,string intoken=null)
        {
            string token ;
            if (intoken != null)
                token = intoken;
            else
                token = Token;
            if (!string.IsNullOrEmpty(token))
            {
                string str2 = EncrpytHelper.Decrypt(token);
                if (!string.IsNullOrEmpty(str2))
                {
                    string[] strArray = str2.Split(new char[] { ',' });
                    if (strArray.Length > index)
                    {
                        return strArray[index];
                    }
                }
            }
            return string.Empty;
        }

        public static bool IsExistsAppToken()
        {
            string token = Token;
            if (!string.IsNullOrEmpty(token))
            {
                if (!appTokenList.ContainsKey(token))
                {
                    SetAppToken(token);
                    return true;
                }
                return true;
            }
            return false;
        }

        public static bool IsExistsToken(bool isRedirect)
        {
            string token = Token;
            if (!string.IsNullOrEmpty(token))
            {
                if (!pcTokenList.ContainsValue(token))
                {
                    SetToken(token, LoginID);
                    return true;
                }
                return true;
            }
            if (isRedirect)
            {
                HttpContext.Current.Response.Redirect("/Login.aspx");
            }
            return false;
        }
        public static bool NeedFresh
        {
            get
            {
                if(FreshList.ContainsKey(UserID))
                return FreshList[UserID];
                return false;
            }
            set
            {
                FreshList[UserID] = value;
            }
        }
        public static bool Login(string loginID, string password, bool isRemember, out string errMsg)
        {
            string str = GetAuthToken(loginID, password, out errMsg);
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            SetToken(str, loginID);
            WriteCookie(str, loginID, isRemember);
            HttpContext.Current.Response.Redirect("/Index.aspx");
            return true;
        }

        public static void Logout()
        {
            string loginID = LoginID;
            HttpCookie cookie = new HttpCookie("token", "");
            cookie.Expires = DateTime.Now.AddDays(-1000);
            HttpContext.Current.Response.SetCookie(cookie);
            if (!string.IsNullOrEmpty(loginID))
            {
                try
                {
                    if (pcTokenList.ContainsKey(loginID))
                    {
                        pcTokenList.Remove(loginID);
          
                    }
                }
                finally
                {
                    Token = null;
                }
            }
            if (HttpContext.Current.Request.Url.LocalPath.EndsWith(".aspx"))
            {
               // HttpContext.Current.Response.Redirect("/Login.aspx");
            }
        }

        private static void SetAppToken(string token)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    if (appTokenList.ContainsKey(token))
                    {
                        appTokenList[token] = DateTime.Now;
                    }
                    else
                    {
                        appTokenList.Add(token, DateTime.Now);
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private static void SetToken(string token, string loginID)
        {
            try
            {
                if (!pcTokenList.ContainsKey(loginID))
                {
                    pcTokenList.Add(loginID, token);
                }
                else
                {
                    pcTokenList[loginID] = token;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private static void WriteCookie(string token, string loginID, bool isRemember)
        {
            bool flag = HttpContext.Current.Request.Url.Host == "localhost";
            HttpCookie cookie = new HttpCookie("token", token);
            HttpCookie cookie2 = new HttpCookie("remember", isRemember.ToString().ToLower());
            HttpCookie cookie3 = new HttpCookie("loginID", loginID);
            if (isRemember || flag)
            {
                DateTime time = DateTime.Now.AddDays(7.0);
                if (flag)
                {
                    cookie.Expires = time;
                }
                cookie2.Expires = time;
                cookie3.Expires = time;
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
            HttpContext.Current.Response.Cookies.Add(cookie2);
            HttpContext.Current.Response.Cookies.Add(cookie3);
        }

        public static int CompanyID
        {
            get
            {
                int num;
                int.TryParse(GetTokenValue(3), out num);
                return num;
            }
        }

        internal static int Day
        {
            get
            {
                int num;
                int.TryParse(GetTokenValue(4), out num);
                return num;
            }
        }

        public static bool IsAdmin
        {
            get
            {
                return LoginID.ToLower().EndsWith("admin");
            }
        }

        public static bool IsProvince
        {
            get
            {
                return (CompanyID == 300);
            }
        }

        public static bool IsRemember
        {
            get
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies["remember"];
                return ((cookie != null) && (cookie.Value == "true"));
            }
        }

        public static string LoginID
        {
            get
            {
                string tokenValue = GetTokenValue(1);
                if (string.IsNullOrEmpty(tokenValue))
                {
                    tokenValue = GetCookieValue("loginID");
                }
                return tokenValue;
            }
        }

        public static bool IsLogined(string token=null)
        {
            try
            {

                string str2;
                if (token != null)
                    str2 = EncrpytHelper.Decrypt(token);
                else
                    str2= EncrpytHelper.Decrypt(Token);
                string[] array = str2.Split(',');
                if (array.Length != 3)
                {
                    return false;
                }
                int userid = int.Parse(array[0]);
                string loginid = array[1];
                return true;
            }
            catch
            { return false; }

        }

        public static int OnlineAppCount
        {
            get
            {
                return appTokenList.Count;
            }
        }

        public static int OnlineCount
        {
            get
            {
                return pcTokenList.Count;
            }
        }

        public static string Token
        {
            get
            {
                string str = HttpContext.Current.Request["token"];
                if ((str != null) && (str.IndexOf('%') != -1))
                {
                    str = HttpContext.Current.Server.UrlDecode(str);
                }
                return str;
            }
            private set
            {
                //HttpCookie cookie = HttpContext.Current.Request.Cookies["token"];
                //if (cookie != null)
                //{
                //    cookie.HttpOnly = false;
                //    cookie.Expires = DateTime.Now.AddDays(30);
                //    HttpContext.Current.Response.Cookies.Add(cookie);
                //}
            }
        }

        public static Data.UserRow User
        {
            get
            {
                return DBFactory<Data.UserDataTable, Data.UserRow>.GetBLL().GetByKey(GetTokenValue(0));
            }
        }

        public static int UserID
        {
            get
            {
                int num;
                int.TryParse(GetTokenValue(0), out num);
                return num;
            }
        }

        public static string UserName
        {
            get
            {
                return GetTokenValue(1);
            }
        }

        public static void  UpdateLastRequest()
        {
            ADOBaseBLL<Data.UserDataTable, Data.UserRow> bll = new ADOBaseBLL<Data.UserDataTable, Data.UserRow>(new DBContext());
             var row = bll.GetByKey(UserAuth.UserID);
             if (row == null)
                 return;
             row.LastRequest = DateTime.Now;
             bll.Update(row);
        }
    }
}

