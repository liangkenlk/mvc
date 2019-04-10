namespace TY.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text.RegularExpressions;
    using System.Web.Script.Serialization;

    public class JsonHelper
    {
        public static Dictionary<string, object> DataRowFromJSON(string jsonText)
        {
            return JSONToObject<Dictionary<string, object>>(jsonText);
        }

        public static string DataRowToJSON(DataRow row)
        {
            return ObjectToJSON(DataRowToDictionary(row));
        }


        public static Dictionary<string,object> DataRowToDictionary(DataRow row)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (DataColumn col in row.Table.Columns)
            {
                dictionary.Add(col.ColumnName, row[col.ColumnName]);
            }
            return dictionary;
        }

        public static Dictionary<string, List<Dictionary<string, object>>> DataSetToDic(DataSet ds)
        {
            Dictionary<string, List<Dictionary<string, object>>> dictionary = new Dictionary<string, List<Dictionary<string, object>>>();
            foreach (DataTable table in ds.Tables)
            {
                dictionary.Add(table.TableName, DataTableToList(table));
            }
            return dictionary;
        }

        public static string DataTableToJSON(DataTable dt)
        {
            return DataTimeConvert(ObjectToJSON(DataTableToList(dt)));
        }

        public static List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, object> item = new Dictionary<string, object>();
                foreach (DataColumn column in dt.Columns)
                {
                    item.Add(column.ColumnName, row[column.ColumnName]);
                }
                list.Add(item);
            }
            return list;
        }

        public static string DataTimeConvert(string json)
        {
            json = Regex.Replace(json, @"\\/Date\((\d+)\)\\/", match => new DateTime(0x7b2, 1, 1).AddMilliseconds((double) long.Parse(match.Groups[1].Value)).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            return json;
        }

        public static T JSONToObject<T>(string jsonText)
        {
            T local;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            try
            {
                local = serializer.Deserialize<T>(jsonText);
            }
            catch (Exception exception)
            {
                throw new Exception("JSONHelper.JSONToObject(): " + exception.Message);
            }
            return local;
        }

        public static string ObjectToJSON(object obj)
        {
            string str;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            try
            {
                serializer.MaxJsonLength = Int32.MaxValue;
                str = DataTimeConvert(serializer.Serialize(obj));
            }
            catch (Exception exception)
            {
                throw new Exception("JSONHelper.ObjectToJSON(): " + exception.Message);
            }
            return str;
        }

        public static string OutError(string error)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("Success", false);
            dictionary.Add("Error", error);
            return ObjectToJSON(dictionary);
        }

        public static string OutResult(bool success, string msg)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("Success", success);
            dictionary.Add("Message", msg);
            return ObjectToJSON(dictionary);
        }

        public static object ResultOb(bool success, string msg)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("Success", success);
            dictionary.Add("Message", msg);
            return dictionary;
        }

        public static string OutResult(bool success, string msg,string data)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("Success", success);
            dictionary.Add("Message", msg);
            dictionary.Add("data", data);
            return ObjectToJSON(dictionary);
        }

        public static Dictionary<string, List<Dictionary<string, object>>> TablesDataFromJSON(string jsonText)
        {
            return JSONToObject<Dictionary<string, List<Dictionary<string, object>>>>(jsonText);
        }

        /// <summary>
        /// 将json数据反序列化为Dictionary
        /// </summary>
        /// <param name="jsonData">json数据</param>
        /// <returns></returns>
        public static Dictionary<string, object> JsonToDictionary(string jsonData)
        {
            //实例化JavaScriptSerializer类的新实例
            JavaScriptSerializer jss = new JavaScriptSerializer();
            try
            {
                jss.MaxJsonLength = Int32.MaxValue;
                //将指定的 JSON 字符串转换为 Dictionary<string, object> 类型的对象
                return jss.Deserialize<Dictionary<string, object>>(jsonData);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

