using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EGIS.ShapeFileLib;
using WorkFlow;
using System.IO;
using System.Collections;
using System.Data;
using TY.Core;

namespace Web.lib
{
    public class EGIS
    {
        private ADOBaseBLL<Data.GeoDataTable, Data.GeoRow> geobll = new ADOBaseBLL<Data.GeoDataTable, Data.GeoRow>(new DBContext());
        public EGIS()
        {

            if (System.Configuration.ConfigurationSettings.AppSettings["SHPpath"] != null)
                SHPpath = System.Configuration.ConfigurationSettings.AppSettings["SHPpath"];
            else
                SHPpath = HttpContext.Current.Server.MapPath("~/shpfile");
        }

        public static string SHPpath = "";
        public void shpDirToDB()
        {
            string sql = "delete from geotable";
            geobll.ExecuteNonQuery(sql);
            var files = Directory.GetFiles(SHPpath).Where(p => p.ToLower().EndsWith(".shp") );
            foreach (string file in files)
            {
                ShpToDB(file);
            }

        }
        private void ShpToDB(string filePath)
        {
            ShapeFile sf = new ShapeFile(filePath);
            string[] fieldnames = sf.GetAttributeFieldNames();
            int addnameIdx = fieldnames.ToList().IndexOf("标准名称");
            if (addnameIdx == -1)
                addnameIdx = fieldnames.ToList().IndexOf("标准地名");
            for (int i = 0; i < sf.RecordCount; i++)
            {
                
                var s = sf.GetShapeData(i);
                string latlng = "";
                foreach (var p in s[0])
                {
                    if (latlng != "")
                        latlng += ";";
                    latlng += p.X + "," + p.Y;
                }
                string geo = LatlngToSqlGeometry(latlng, sf.ShapeType);

                
             
                Dictionary<string, string> dic = new Dictionary<string, string>();
                string[] values = sf.GetAttributeFieldValues(i);

                for (int j = 0; j < fieldnames.Length; j++)
                {
                    dic.Add(fieldnames[j], values[j]);
                }
                string keyvalues = JsonHelper.ObjectToJSON(dic).Replace(" ","").Replace("/","");
                string addname = values[addnameIdx];
                string geotype = "";
                if (sf.ShapeType == ShapeType.Polygon)
                    geotype = "polygon";
                if (sf.ShapeType == ShapeType.Point)
                    geotype = "point";
                if (sf.ShapeType == ShapeType.PolyLine)
                    geotype = "line";
                string sql = @"INSERT INTO geotable (geo,id,layername,KeyValue,addname,latlng,geotype) VALUES(" + geo+",'"+Guid.NewGuid().ToString()+"','"+Path.GetFileNameWithoutExtension(filePath) +"','"+keyvalues+"','"+ addname + "','"+ latlng + "','"+ geotype + "') ";
                geobll.ExecuteNonQuery(sql);
            }

        }

        public string LatlngToSqlGeometry(string latlng,ShapeType type)
        {
            string sql = "";
            if (type == ShapeType.Polygon)
                sql = "POLYGON";
            if (type == ShapeType.Point)
                sql = "POINT";
            if (type == ShapeType.PolyLine)
                sql = "LINESTRING";
            latlng = latlng.Replace(",", " ").Replace(";", ",");
            if(type==ShapeType.Polygon)
                sql = "geometry::STGeomFromText('"+sql+" (("+latlng+"))', 0)";
            else
                sql = "geometry::STGeomFromText('" + sql + " (" + latlng + ")', 0)";
            return sql;
        }

        public string sqlGeometryToLatlng()
        {
            return "";
        }

        public DataTable Query(string keyWord, string town, int PageIndex, int PageSize, out int total, bool isSpatial, string type)
        {
            string sql = "";
            if (!isSpatial)
            {
                sql = "select  id,  layername, KeyValue, addname, latlng, geotype from geotable where addname like '%" + keyWord + "%'";

            }
            else
            {
                
                sql = this.LatlngToSqlGeometry(keyWord,ShapeType.Polygon);
                sql = "select  id,  layername, KeyValue, addname, latlng, geotype  from geotable where  geo.STIntersection(" + sql + ").STAsText() <> 'GEOMETRYCOLLECTION EMPTY'";
            }
            if (type != null)
            {
                sql = "select  id,  layername, KeyValue, addname, latlng, geotype from geotable where keyvalue like '%" + "\"地名类别\":\""+type+"\"" + "%'";
            }
            PagedData pd = geobll.GetPagedDataTable(sql, PageSize, PageIndex);
            total = pd.Total;

            return pd.table;
        }
    }
}