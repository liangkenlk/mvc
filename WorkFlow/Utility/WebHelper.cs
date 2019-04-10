namespace TY.Utility
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Web;

    public static class WebHelper
    {
        public static string FilterValue(string strFilter)
        {
            if (strFilter == null)
            {
                return "";
            }
            string str = strFilter;
            string[] strArray = new string[] { "'", ",", "(", ")", ";", "\"" };
            for (int i = 0; i < strArray.Length; i++)
            {
                str = str.Replace(strArray[i], "");
            }
            return str.Trim(new char[] { ' ' });
        }

        public static string HttpGet(string url, string postDataStr, Encoding getEncoding = null)
        {
            string str2;
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url + ((postDataStr == "") ? "" : "?") + postDataStr);
            request.Method = "GET";
            if (getEncoding == null)
            {
                getEncoding = Encoding.GetEncoding("gb2312");
            }
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream, getEncoding))
                {
                    str2 = reader.ReadToEnd();
                }
            }
            return str2;
        }

        public static string HttpPost(string url, string postDataStr, CookieContainer cookie, Encoding requestCoding = null, Encoding getCoding = null)
        {
            string str2;
            if (requestCoding == null)
            {
                requestCoding = Encoding.GetEncoding("gb2312");
            }
            if (getCoding == null)
            {
                getCoding = Encoding.UTF8;
            }
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "POST";
            if (requestCoding == Encoding.UTF8)
            {
                request.ContentType = "application/x-www-form-urlencoded;";
            }
            else
            {
                request.ContentType = "application/x-www-form-urlencoded;charset=gb2312";
            }
            byte[] bytes = requestCoding.GetBytes(postDataStr);
            request.ContentLength = bytes.Length;
            request.CookieContainer = cookie;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                if (cookie != null)
                {
                    response.Cookies = cookie.GetCookies(response.ResponseUri);
                }
                using (Stream stream2 = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream2, getCoding))
                    {
                        str2 = reader.ReadToEnd();
                    }
                }
            }
            return str2;
        }

        public static bool IsEmpty(params string[] strArr)
        {
            foreach (string str in strArr)
            {
                if (string.IsNullOrWhiteSpace(str))
                {
                    return true;
                }
            }
            return false;
        }
        public static T Query<T>()
        {
            return Query<T>("");
        }
        public static T Query<T>(string key)
        {
            return Query<T>(key, default(T), false);
        }

 

        public static T Query<T>(string key, T defaultValue, bool filter)
        {
            object obj2;
           
            if (HttpContext.Current.Request[key] == null)
            {
                return defaultValue;
            }
            if (typeof(T).Name == "Int32")
            {
                
                int result = 0;
                if (!int.TryParse(HttpContext.Current.Request[key], out result))
                {
                    return defaultValue;
                }
                obj2 = result;
            }
            else if (typeof(T).Name.ToLower() == "double")
            {
                double num2 = 0.0;
                if (!double.TryParse(HttpContext.Current.Request[key], out num2))
                {
                    return defaultValue;
                }
                obj2 = num2;
            }
            else if (typeof(T).Name == "Boolean")
            {
                if ((HttpContext.Current.Request[key].ToLower() == "true") || (HttpContext.Current.Request[key] == "1"))
                {
                    obj2 = true;
                }
                else
                {
                    obj2 = false;
                }
            }
            else if (typeof(T).Name == "DateTime")
            {

                DateTime result = DateTime.Now;
                if (!DateTime.TryParse(HttpContext.Current.Request[key], out result))
                {
                    return defaultValue;
                }
                obj2 = result;
            }

            else
            {
                obj2 = HttpContext.Current.Request[key].Trim();
            }

            return (T) obj2;
        }

        public static void FillObject<T>(T obj2)
        {
            try
            {
                if (typeof(T).IsClass)
                {
                    Type type = typeof(T);
                    System.Reflection.PropertyInfo[] infos = type.GetProperties();


                    foreach (var info in infos)
                    {

                        if (HttpContext.Current.Request[info.Name] == null || !info.CanWrite)
                        {
                            continue;


                        }
                        if (info.PropertyType.Name == "Int32")
                        {

                            int result = 0;
                            if (!int.TryParse(HttpContext.Current.Request[info.Name], out result))
                            {
                                continue;
                            }
                            try
                            {
                                info.SetValue(obj2, result);
                            }
                            catch { continue; }
                        }
                        else if (info.PropertyType.Name.ToLower() == "double")
                        {
                            double num2 = 0.0;
                            if (!double.TryParse(HttpContext.Current.Request[info.Name], out num2))
                            {
                                continue;
                            }
                            info.SetValue(obj2, num2);
                        }
                        else if (info.PropertyType.Name == "Boolean")
                        {
                            if ((HttpContext.Current.Request[info.Name].ToLower() == "true") || (HttpContext.Current.Request[info.Name] == "1"))
                            {
                                info.SetValue(obj2, true);
                            }
                            else
                            {
                                info.SetValue(obj2, false);
                            }
                        }
                        else if (info.PropertyType.Name.ToLower() == "datetime")
                        {
                            DateTime date;
                            if (!DateTime.TryParse(HttpContext.Current.Request[info.Name], out date))
                            {
                                continue;
                            }
                            info.SetValue(obj2, date);
                        }
                        else
                        {
                            info.SetValue(obj2, HttpContext.Current.Request[info.Name].Trim());

                        }


                    }

                }
            }
            catch { }
        }
        public static void ResponceImage(FileStream fs)
        {
            using (Image image = Image.FromStream(fs))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Jpeg);
                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.BinaryWrite(stream.ToArray());
                    HttpContext.Current.Response.ContentType = "image/jpeg";
                }
            }
        }

        public static void SendFile(string fileName, string saveText)
        {
            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(saveText);
            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length, true, true);
            SendFile(fileName, ms, Encoding.UTF8);
        }

        public static void SendFile(string fileName, MemoryStream ms, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            if ((ms != null) && !string.IsNullOrEmpty(fileName))
            {
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.AddHeader("Content-Type", "application/octet-stream");
                response.Charset = encoding.BodyName;
                if (!(HttpContext.Current.Request.UserAgent.Contains("Firefox") || HttpContext.Current.Request.UserAgent.Contains("Chrome")))
                {
                    fileName = HttpUtility.UrlEncode(fileName, encoding);
                }
                response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
                response.BinaryWrite(ms.GetBuffer());
                ms.Close();
                ms = null;
                response.Flush();
                response.End();
            }
        }

        public static void SendImage(FileStream fs)
        {
            using (Image image = Image.FromStream(fs))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Jpeg);
                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.BinaryWrite(stream.ToArray());
                    HttpContext.Current.Response.ContentType = "image/jpeg";
                }
            }
        }
    }
}

