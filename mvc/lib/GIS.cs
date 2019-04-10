using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using System.IO;
using System.Collections;
using System.Data;
using TY.Core;
using WorkFlow;

namespace Web.lib
{
    public class GIS
    {
        public static bool isArcGisOk;
        private ADOBaseBLL<Data.GeoDataTable, Data.GeoRow> geobll = new ADOBaseBLL<Data.GeoDataTable, Data.GeoRow>(new DBContext());

        public  GIS()
        {
            if(!isArcGisOk)
               isArcGisOk = new Web.lib.LicenseInitializer().InitializeApplication();
            if (System.Configuration.ConfigurationSettings.AppSettings["SHPpath"] != null)
                SHPpath= System.Configuration.ConfigurationSettings.AppSettings["SHPpath"];
            else
            SHPpath= HttpContext.Current.Server.MapPath("~/shpfile");
        }

        public static string SHPpath = "";
   

        public ArrayList GetLayers()
        {
            
            string [] files =  Directory.GetFiles(SHPpath);
            ArrayList layers = new ArrayList();
            foreach (string filename in files)
            {
                if (System.IO.Path.GetExtension(filename).ToLower() == ".shp")
                    layers.Add(System.IO.Path.GetFileNameWithoutExtension(filename));
                
                    
            }
            return layers;

            
        }
        public DataTable Query(string keyWord,string town,int PageIndex,int PageSize,out int total,bool isSpatial,string type)
        {
            total = 0;
            DataTable table = new DataTable();
            table.Columns.Add("latlng");
            table.Columns.Add("addname");
            //table.Columns.Add("add");
            //table.Columns.Add("fid");
            table.Columns.Add("geotype");
            table.Columns.Add("KeyValue");

            //table.Columns.Add("bigtype");
            //table.Columns.Add("type");
            table.Columns.Add("id");
            //table.Columns.Add("his");
            //table.Columns.Add("means");
            //table.Columns.Add("time");
            //table.Columns.Add("comefrom");
            var Layers = this.GetLayers();
            if (string.IsNullOrEmpty(keyWord) && string .IsNullOrEmpty(type))
                return table;

            int skip = PageIndex * PageSize;
            int take = PageSize;
            ESRI.ArcGIS.Geometry.Polygon polygon = new ESRI.ArcGIS.Geometry.Polygon();
            if (isSpatial)
            {
                string[] lnlas = keyWord.Split(';');
                foreach (string lnla in lnlas)
                {
                    Point p = new Point();
                    p.X = double.Parse(lnla.Split(',')[0]);
                    p.Y = double.Parse(lnla.Split(',')[1]);
                    polygon.AddPoint(p);
                }
            }
            foreach (string layer in Layers)
            {


                    QueryOneLayer(layer, keyWord, town, ref skip,ref take, table,ref total,isSpatial, polygon,type);
                
            }

            return table;
        }
        public void QueryOneLayer(string LayerName,string KeyWord,string town,ref int skip,ref int take, DataTable table,ref int total,bool isSpatial, ESRI.ArcGIS.Geometry.Polygon polygon,string type)
        {
            IFeatureClass featureClass = null;
            IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
            IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(SHPpath, 0);
            //string LayerName = "点状";
            IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
            featureClass = pFeatureWorkspace.OpenFeatureClass(LayerName);
            string nameField = "标准名称";
            if (featureClass.ShapeType != esriGeometryType.esriGeometryPoint && isSpatial)
            {
                return;
            }

            if (featureClass.FindField(nameField) == -1)
                //nameField = "标准地名";
                return;

            IFeatureCursor featureCursor;
            if (!isSpatial)
            {
                

                IQueryFilter queryFilter = new QueryFilterClass();
                if (!string.IsNullOrEmpty(town))
                {
                    if (featureClass.FindField("所在镇街") == -1)
                        return;
                    queryFilter.WhereClause = nameField + " like '%" + KeyWord + "%' and 所在镇街='"+town+"'";
                }
                else
                {
                    queryFilter.WhereClause = nameField + " like '%" + KeyWord + "%'";
                }

                if (!string.IsNullOrEmpty(type))
                {
                    queryFilter.WhereClause = "地名类别 = '" + type + "'";
                }

                featureCursor = featureClass.Search(queryFilter, true);
            }
            else
            {

                //point.X = 100;
                //point.Y = 200;
                ISpatialFilter spatialFilter = new SpatialFilter();
                spatialFilter.Geometry = polygon as IGeometry;
                spatialFilter.GeometryField = "shape";
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                featureCursor = featureClass.Search(spatialFilter, true);
            }
            IFeature feature = featureCursor.NextFeature();
           
            while (feature != null)
            {
                //this.Label1.Text = feature.get_Value(index).ToString();
                total++;
                if (skip <= 0)
                {
                    if (take == 0)
                    {
                        feature = featureCursor.NextFeature();
                        continue;
                    }
                    DataRow row = table.NewRow();
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    for (int i = 0; i < feature.Fields.FieldCount; i++)
                    {
                        string fieldName = feature.Fields.Field[i].Name;
                        dic.Add(fieldName, getFieldValue(fieldName, feature));
                    }
                    if (featureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                    {
                        IPoint p = (IPoint)feature.Shape;
                        
                        row["latlng"] = p.X + "," + p.Y;
                        row["geotype"] = "point";
                        dic["Shape"] = "point";
                      
                    }
                    if (featureClass.ShapeType == esriGeometryType.esriGeometryPolyline)
                    {
                        string latlng = "";
                        IPointCollection p = (IPointCollection)feature.Shape;
                        for (int i = 0; i < p.PointCount; i++)
                        {
                            latlng += p.Point[i].X + "," + p.Point[i].Y + ";";
                        }
                        latlng = latlng.TrimEnd(';');
                        row["latlng"] = latlng;
                    }
                    if (featureClass.ShapeType == esriGeometryType.esriGeometryPolyline)
                    {
                        row["geotype"] = "line";
                        dic["Shape"] = "line";
                    }
                    if (featureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        row["geotype"] = "polygon";
                        dic["Shape"] = "polygon";
                    }

                    row["KeyValue"] = JsonHelper.ObjectToJSON(dic);
                    //row["fid"] = getFieldValue("fid", feature);
                    row["id"] = Guid.NewGuid().ToString();
                    //row["bigtype"] = getFieldValue("大类", feature);
                    //row["type"] = getFieldValue("地名类别", feature);
                    //row["comefrom"] = getFieldValue("地名的来历", feature);
                    //row["means"] = getFieldValue("地名的含义", feature);
                    //row["his"] = getFieldValue("地名的历史", feature);
                    //row["time"] = getFieldValue("登记时间", feature);
                    row["addname"] = getFieldValue(nameField, feature);
                    //row["add"] = getFieldValue(nameField, feature);
                    //IFields fields = feature.Fields;
                    //fields.Field[0].c
                    table.Rows.Add(row);
                    take--;
                }
                else
                    skip--;
                feature = featureCursor.NextFeature();
                
                
            }
        }

        public string getFieldValue(string fieldName, IFeature feature)
        {
            int index = feature.Fields.FindField(fieldName);
            if (index == -1)
                return "";
            return feature.Value[index].ToString();
        }

        public string CheckData()
        {
            new EGIS().shpDirToDB();
            return "";
            geobll.ExecuteNonQuery("delete from geo");
            var Layers = this.GetLayers();
            foreach (string layer in Layers)
            {
                
                LayerToDB(layer);
                

            }
            string r = "检查完成";
            string sql = @"SELECT     namecount, name
FROM(SELECT     COUNT(*) AS namecount, name
                       FROM          Geo
                       GROUP BY name) AS a
WHERE(namecount > 1)";
            var data = geobll.GetNormalDataTable(sql);
            if (data.Rows.Count == 0)
                r += "<br>没有检查到一名多用";
            else
            {
                foreach (DataRow row in data.Rows)
                {
                    r += "<br>检查到一名多用:" + row["name"].ToString() + row["namecount"].ToString() + "个";
                }
            }
            sql = @"SELECT     Geo.id, Geo.name, Geo.geoType, Geo.layer, Geo.typeValues, Geo.latlng, b.namecount, b.latlng AS Expr1
FROM         Geo INNER JOIN
                          (SELECT     namecount, latlng
                            FROM          (SELECT     COUNT(*) AS namecount, latlng
                                                    FROM          Geo AS Geo_1
                                                    GROUP BY latlng) AS a
                            WHERE      (namecount > 1)) AS b ON Geo.latlng = b.latlng";
            data = geobll.GetNormalDataTable(sql);

            if (data.Rows.Count == 0)
                r += "<br>没有检查到一地多名";
            else
            {
                foreach (DataRow row in data.Rows)
                {
                    r += "<br>检查到一地多名:" + row["name"].ToString();
                }
            }

            sql = @"SELECT     namecount, PinYin
FROM(SELECT     COUNT(*) AS namecount, PinYin
                       FROM          Geo
                       GROUP BY PinYin) AS a
WHERE(namecount > 1)";
            data = geobll.GetNormalDataTable(sql);
            if (data.Rows.Count == 0)
                r += "<br>没有检查到重名同音";
            else
            {
                foreach (DataRow row in data.Rows)
                {
                    r += "<br>检查到重名同音:" + row["PinYin"].ToString() + row["namecount"].ToString() + "个";
                }
            }
            return r;

        }

        void LayerToDB(string LayerName)
        {
            IFeatureClass featureClass = null;
            IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
            IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(SHPpath, 0);
          
            IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
            featureClass = pFeatureWorkspace.OpenFeatureClass(LayerName);
            string nameField = "标准名称";

            if (featureClass.FindField(nameField) == -1)
           
                return;
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = nameField + " like '%%'";
            IFeatureCursor featureCursor;
            featureCursor = featureClass.Search(queryFilter, true);
            IFeature feature = featureCursor.NextFeature();
            while (feature != null)
            {


                var row = geobll.NewRow();
                Dictionary<string, string> dic = new Dictionary<string, string>();

                if (featureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                {
                    IPoint p = (IPoint)feature.Shape;

                    row.geoType = "点";
                    row.latlng = p.X + "," + p.Y;


                }
                if (featureClass.ShapeType == esriGeometryType.esriGeometryPolyline)
                {
                    string latlng = "";
                    IPointCollection p = (IPointCollection)feature.Shape;
                    for (int i = 0; i < p.PointCount; i++)
                    {
                        latlng += p.Point[i].X + "," + p.Point[i].Y + ";";
                    }
                    latlng = latlng.TrimEnd(';');
                    //row["latlng"] = latlng;
                    row.geoType = "线";
                    row.latlng = latlng;
                    
                }
  
                if (featureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                {
                    row.geoType = "面";
                }
                row.name = getFieldValue(nameField, feature);
                row.id = Guid.NewGuid().ToString();
                row.layer = LayerName; 
                row.PinYin = getFieldValue("罗马字母拼", feature);
                //row["KeyValue"] = JsonHelper.ObjectToJSON(dic);
                //row["fid"] = getFieldValue("fid", feature);
                //row["id"] = Guid.NewGuid().ToString();
                //row["bigtype"] = getFieldValue("大类", feature);
                //row["type"] = getFieldValue("地名类别", feature);
                //row["comefrom"] = getFieldValue("地名的来历", feature);
                //row["means"] = getFieldValue("地名的含义", feature);
                //row["his"] = getFieldValue("地名的历史", feature);
                //row["time"] = getFieldValue("登记时间", feature);
                //row["addname"] = getFieldValue(nameField, feature);
                geobll.Add(row);

                feature = featureCursor.NextFeature();


            }
        }
    }


}