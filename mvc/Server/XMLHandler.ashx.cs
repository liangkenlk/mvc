using iParking;

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
    /// XMLHandler 的摘要说明
    /// </summary>
    public class XMLHandler : AjaxHandler
    {
        private IBaseBLL<Data.UserDataTable, Data.UserRow> userbll = DBFactory<Data.UserDataTable, Data.UserRow>.GetBLL();
        private IBaseBLL<Data.BaseMapDataTable, Data.BaseMapRow> baseMapbll = DBFactory<Data.BaseMapDataTable, Data.BaseMapRow>.GetBLL();
        private IBaseBLL<Data.ConfigDataTable, Data.ConfigRow> configbll = DBFactory<Data.ConfigDataTable, Data.ConfigRow>.GetBLL();
        private IBaseBLL<Data.regionDataTable, Data.regionRow> regionsbll = DBFactory<Data.regionDataTable, Data.regionRow>.GetBLL();
        private IBaseBLL<Data.themeDataTable, Data.themeRow> themebll = DBFactory<Data.themeDataTable, Data.themeRow>.GetBLL();
        public string getConfigValueByName(string name)
        {
            var r = this.configbll.GetBySQL("select * from config where configname ='" + name + "'");
            if (r.Count > 0) 
            return r[0].ConfigValue;
            return null;
        }
        public void getConfigValueByNameJS()
        {
            string name = Query<string>("name");
            jsonResult = JsonHelper.OutResult(true, getConfigValueByName(name));
        }


        public void DB2cityservice_config(string userid)
        {
            string path = HttpContext.Current.Server.MapPath("~/data/") + "cityservice_config_" + userid+ ".xml";
            if (File.Exists(path))
                File.Delete(path);
            XmlDocument xml = new XmlDocument();

            var configuration =  xml.CreateElement("configuration");
            xml.AppendChild(configuration);

            var citylist = xml.CreateElement("citylist");
            configuration.AppendChild(citylist);

            var city =  xml.CreateElement("city");
            city.SetAttribute("value", getConfigValueByName("地区名"));
            city.SetAttribute("name", getConfigValueByName("地区拼音"));
            citylist.AppendChild(city);

            var location = xml.CreateElement("location");
            location.SetAttribute("lon", getConfigValueByName("中心经度"));
            location.SetAttribute("lat", getConfigValueByName("中心纬度"));
            location.SetAttribute("level", getConfigValueByName("中心等级"));
            city.AppendChild(location);

            var regions = xml.CreateElement("regions");
            regions.SetAttribute("value", "市镇");
            city.AppendChild(regions);
            var regiondata = new Data.regionDataTable();
            string sql = "select * from region where pid='0' and sysname = (select top 1 ConfigValue from [config] where configname='地区名')";
            if (regionsbll.GetType().ToString().ToLower().Contains("mysql"))
            {
                sql = "select * from region where pid='0' and sysname = (select ConfigValue from [config] where configname='地区名' limit 1)";
            }
            regiondata =  regionsbll.GetBySQL(sql);
            foreach (var row in regiondata)
            {
                var Property = xml.CreateElement("Property");
                Property.SetAttribute("value", row.name);
                Property.SetAttribute("lon", row.center.Split(',')[0].Trim());
                Property.SetAttribute("lat", row.center.Split(',')[1].Trim());
                Property.SetAttribute("level", row.level);
                regions.AppendChild(Property);
            }

            var wmtslayer = xml.CreateElement("wmtslayer");
            city.AppendChild(wmtslayer);

            var wmtslayerData= baseMapbll.GetBySQL("select * from BaseMap order by idx");
 
                for (int i=0;i< wmtslayerData.Rows.Count; i++)
                {
                    var layer = xml.CreateElement("layer");
                    layer.SetAttribute("type", wmtslayerData[i].type);
                    layer.SetAttribute("servicetype", "GeoGlobe");
                    layer.SetAttribute("layercontrol", "false");
                    layer.SetAttribute("id",wmtslayerData[i].mapId);

                var wmtslayerProperty1 = xml.CreateElement("Property");
                var wmtslayerProperty2 = xml.CreateElement("Property");
                var wmtslayerProperty3 = xml.CreateElement("Property");
                var wmtslayerProperty4 = xml.CreateElement("Property");
                var wmtslayerProperty5 = xml.CreateElement("Property");
                var wmtslayerProperty6 = xml.CreateElement("Property");
                var wmtslayerProperty7 = xml.CreateElement("Property");
                var wmtslayerProperty8 = xml.CreateElement("Property");
                var wmtslayerProperty9 = xml.CreateElement("Property");
                var wmtslayerProperty10 = xml.CreateElement("Property");
                var wmtslayerProperty11 = xml.CreateElement("Property");
                var wmtslayerProperty12 = xml.CreateElement("Property");
                var wmtslayerProperty13 = xml.CreateElement("Property");
                var wmtslayerProperty14 = xml.CreateElement("Property");
                wmtslayerProperty1.SetAttribute("name", "layername");
                wmtslayerProperty1.SetAttribute("value", wmtslayerData[i].layername);

                wmtslayerProperty2.SetAttribute("name","url");
                wmtslayerProperty2.SetAttribute("value", wmtslayerData[i].url);

                wmtslayerProperty3.SetAttribute("name", "levels");
                wmtslayerProperty3.SetAttribute("value", wmtslayerData[i].levels);

                wmtslayerProperty4.SetAttribute("name", "layeridfilter");
                wmtslayerProperty4.SetAttribute("value", wmtslayerData[i].layeridfilter);

                wmtslayerProperty5.SetAttribute("name", "serviceMode");
                wmtslayerProperty5.SetAttribute("value", "KVP");

                wmtslayerProperty6.SetAttribute("name", "style");
                wmtslayerProperty6.SetAttribute("value", wmtslayerData[i].style);

                wmtslayerProperty7.SetAttribute("name", "format");
                wmtslayerProperty7.SetAttribute("value", wmtslayerData[i].format);

                wmtslayerProperty8.SetAttribute("name", "projection");
                wmtslayerProperty8.SetAttribute("value", "");

                wmtslayerProperty9.SetAttribute("name", "tileMatrixSet");
                wmtslayerProperty9.SetAttribute("value", wmtslayerData[i].tileMatrixSet);

                wmtslayerProperty10.SetAttribute("name", "offline_url_android");
                wmtslayerProperty10.SetAttribute("value", "digitalmapgis/" + getConfigValueByName("地区拼音") + "/" + wmtslayerData[i].layeridfilter);

                wmtslayerProperty11.SetAttribute("name", "offline_url_ios");
                wmtslayerProperty11.SetAttribute("value", "digitalmapgis/" + getConfigValueByName("地区拼音") + "/" + wmtslayerData[i].layeridfilter);

                wmtslayerProperty12.SetAttribute("name", "version");
                wmtslayerProperty12.SetAttribute("value", "1.0.0");

                wmtslayerProperty13.SetAttribute("name", "service");
                wmtslayerProperty13.SetAttribute("value", "WMTS");

                wmtslayerProperty14.SetAttribute("name", "request");
                wmtslayerProperty14.SetAttribute("value", "GetTile");

                layer.AppendChild(wmtslayerProperty1);
                layer.AppendChild(wmtslayerProperty2);
                layer.AppendChild(wmtslayerProperty3);
                layer.AppendChild(wmtslayerProperty4);
                layer.AppendChild(wmtslayerProperty5);
                layer.AppendChild(wmtslayerProperty6);
                layer.AppendChild(wmtslayerProperty7);
                layer.AppendChild(wmtslayerProperty8);
                layer.AppendChild(wmtslayerProperty9);
                layer.AppendChild(wmtslayerProperty10);
                layer.AppendChild(wmtslayerProperty11);
                layer.AppendChild(wmtslayerProperty12);
                layer.AppendChild(wmtslayerProperty13);
                layer.AppendChild(wmtslayerProperty14);
                wmtslayer.AppendChild(layer);
                }


            var addressquery= xml.CreateElement("addressquery");
            addressquery.SetAttribute("type", "GeoGisServer");
            addressquery.SetAttribute("url", getConfigValueByName("地名地址服务"));
            addressquery.SetAttribute("layer", getConfigValueByName("地名地址图层名"));
            addressquery.SetAttribute("paraWhereByCode", "false");
            addressquery.SetAttribute("paraWhereKeyName", getConfigValueByName("地名地址地名字段"));
            addressquery.SetAttribute("paraReplace", getConfigValueByName("地名地址替换文字"));
            addressquery.SetAttribute("paraName", getConfigValueByName("地名地址地名字段"));
            addressquery.SetAttribute("paraAddress", getConfigValueByName("地名地址地址字段"));

            city.AppendChild(addressquery);

            xml.Save(path);

        }


        /// <summary>
        /// 数据库专题图数据转换为xml文件
        /// </summary>
        public void DB2themeservice_config(string userid)
        {
            string path = HttpContext.Current.Server.MapPath("~/data/") + "themeData_config_" + userid + ".xml";
            if (File.Exists(path))
                File.Delete(path);
            XmlDocument xml = new XmlDocument();

            var configuration = xml.CreateElement("configuration");
            xml.AppendChild(configuration);

            var citylist = xml.CreateElement("citylist");
            configuration.AppendChild(citylist);

            var city = xml.CreateElement("city");
            city.SetAttribute("value", getConfigValueByName("地区名"));
            city.SetAttribute("name", getConfigValueByName("地区拼音"));
            citylist.AppendChild(city);

            var themeDataList = xml.CreateElement("themeDataList");
            city.AppendChild(themeDataList);

            var themeData = themebll.GetBySQL("select * from theme order by idx");

            for(int i=0;i< themeData.Rows.Count; i++)
            {
                var themeDataLayer = xml.CreateElement("themeDataLayer");
                themeDataLayer.SetAttribute("type", "geogis");
                themeDataLayer.SetAttribute("key", themeData[i].name);
                themeDataLayer.SetAttribute("describe", themeData[i].name);
                themeDataLayer.SetAttribute("islocal","false");
                themeDataLayer.SetAttribute("id", themeData[i].themeId);
                themeDataList.AppendChild(themeDataLayer);

                var wfslayer = xml.CreateElement("wfslayer");
                wfslayer.SetAttribute("url", themeData[i].wfs_url);
                wfslayer.SetAttribute("layer", themeData[i].wfs_layer);
                wfslayer.SetAttribute("geometryType", "Polygon");
                themeDataLayer.AppendChild(wfslayer);


                var mainFieldList = xml.CreateElement("mainFieldList");
                mainFieldList.SetAttribute("key", "OID");
                string[] mainFieldData = themeData[i].wfs_showfields.Split(';');
       
                    for (int j = 0; j < mainFieldData.Length - 1 && j<7; j++)
                    {
                        var Property = xml.CreateElement("Property");
                        string[] mainField = mainFieldData[j].Split(',');

                        if (mainField.Length == 2)
                        {
                            Property.SetAttribute("value", mainField[0]);
                            Property.SetAttribute("name", mainField[1]);
                            //Property.SetAttribute("id", mainField[2]);

                            mainFieldList.AppendChild(Property);
                        }
                    }
                
                themeDataLayer.AppendChild(mainFieldList);


                var showDetailFieldList = xml.CreateElement("showDetailFieldList");
                string[] detailFieldData = themeData[i].wfs_showfields.Split(';');
           
                for (int j = 0; j < detailFieldData.Length - 1; j++)
                {
                    var Property = xml.CreateElement("Property");
                    string[] detailField = detailFieldData[j].Split(',');
                    if (detailField.Length == 2)
                    {
                        Property.SetAttribute("value", detailField[0]);
                        Property.SetAttribute("name", detailField[1]);
                        //Property.SetAttribute("id", detailField[2]);

                        showDetailFieldList.AppendChild(Property);
                    }

                }
                
                themeDataLayer.AppendChild(showDetailFieldList);

                var classifyField = xml.CreateElement("classifyField");

                if (themeData[i].wfs_classifyfield.Split(',').Length == 2)
                {
                    var classProperty = xml.CreateElement("Property");
                    classProperty.SetAttribute("name", themeData[i].wfs_classifyfield.Split(',')[1]);
                    classProperty.SetAttribute("value", themeData[i].wfs_classifyfield.Split(',')[0]);
                    classifyField.AppendChild(classProperty);
                }
                themeDataLayer.AppendChild(classifyField);


                var superimposedField = xml.CreateElement("superimposedField");
                superimposedField.SetAttribute("superimposedLayer", "false");
                if (themeData[i].wfs_classifyfield.Split(',').Length == 2)
                {
                    var superimposedProperty = xml.CreateElement("Property");
                    superimposedProperty.SetAttribute("name", themeData[i].wfs_classifyfield.Split(',')[1]);
                    superimposedProperty.SetAttribute("value", themeData[i].wfs_classifyfield.Split(',')[0]);
                    superimposedField.AppendChild(superimposedProperty);
                }
                themeDataLayer.AppendChild(superimposedField);


                var wmtslayer = xml.CreateElement("wmtslayer");
                wmtslayer.SetAttribute("type", "feature");
                wmtslayer.SetAttribute("servicetype", "GeoGlobe");
                wmtslayer.SetAttribute("layercontrol", "false");

                var wmtslayerProperty1 = xml.CreateElement("Property");
                var wmtslayerProperty2 = xml.CreateElement("Property");
                var wmtslayerProperty3 = xml.CreateElement("Property");
                var wmtslayerProperty4 = xml.CreateElement("Property");
                var wmtslayerProperty5 = xml.CreateElement("Property");
                var wmtslayerProperty6 = xml.CreateElement("Property");
                var wmtslayerProperty7 = xml.CreateElement("Property");
                var wmtslayerProperty8 = xml.CreateElement("Property");
                var wmtslayerProperty9 = xml.CreateElement("Property");
                var wmtslayerProperty10 = xml.CreateElement("Property");
                var wmtslayerProperty11 = xml.CreateElement("Property");
                var wmtslayerProperty12 = xml.CreateElement("Property");
                var wmtslayerProperty13 = xml.CreateElement("Property");
                var wmtslayerProperty14 = xml.CreateElement("Property");
                wmtslayerProperty1.SetAttribute("name","layername");
                wmtslayerProperty1.SetAttribute("value", themeData[i].name);

                wmtslayerProperty2.SetAttribute("name", "url");
                wmtslayerProperty2.SetAttribute("value", themeData[i].wmts_url);

                wmtslayerProperty3.SetAttribute("name", "levels");
                wmtslayerProperty3.SetAttribute("value", themeData[i].wmts_levels);

                wmtslayerProperty4.SetAttribute("name", "layeridfilter");
                wmtslayerProperty4.SetAttribute("value", themeData[i].wmts_layer);

                wmtslayerProperty5.SetAttribute("name", "serviceMode");
                wmtslayerProperty5.SetAttribute("value", "KVP");

                wmtslayerProperty6.SetAttribute("name", "style");
                wmtslayerProperty6.SetAttribute("value", themeData[i].wmts_style);

                wmtslayerProperty7.SetAttribute("name", "format");
                wmtslayerProperty7.SetAttribute("value", themeData[i].wmts_format);

                wmtslayerProperty8.SetAttribute("name", "projection");
                wmtslayerProperty8.SetAttribute("value", "");

                wmtslayerProperty9.SetAttribute("name", "tileMatrixSet");
                wmtslayerProperty9.SetAttribute("value", themeData[i].wmts_tileMatrixSet);

                wmtslayerProperty10.SetAttribute("name", "offline_url_android");
                wmtslayerProperty10.SetAttribute("value", "digitalmapgis/" + getConfigValueByName("地区拼音") + "/" + themeData[i].wmts_layer);

                wmtslayerProperty11.SetAttribute("name", "offline_url_ios");
                wmtslayerProperty11.SetAttribute("value", "digitalmapgis/" + getConfigValueByName("地区拼音") + "/" + themeData[i].wmts_layer);

                wmtslayerProperty12.SetAttribute("name", "version");
                wmtslayerProperty12.SetAttribute("value", "1.0.0");

                wmtslayerProperty13.SetAttribute("name", "service");
                wmtslayerProperty13.SetAttribute("value", "WMTS");

                wmtslayerProperty14.SetAttribute("name", "request");
                wmtslayerProperty14.SetAttribute("value", "GetTile");

                wmtslayer.AppendChild(wmtslayerProperty1);
                wmtslayer.AppendChild(wmtslayerProperty2);
                wmtslayer.AppendChild(wmtslayerProperty3);
                wmtslayer.AppendChild(wmtslayerProperty4);
                wmtslayer.AppendChild(wmtslayerProperty5);
                wmtslayer.AppendChild(wmtslayerProperty6);
                wmtslayer.AppendChild(wmtslayerProperty7);
                wmtslayer.AppendChild(wmtslayerProperty8);
                wmtslayer.AppendChild(wmtslayerProperty9);
                wmtslayer.AppendChild(wmtslayerProperty10);
                wmtslayer.AppendChild(wmtslayerProperty11);
                wmtslayer.AppendChild(wmtslayerProperty12);
                wmtslayer.AppendChild(wmtslayerProperty13);
                wmtslayer.AppendChild(wmtslayerProperty14);

                themeDataLayer.AppendChild(wmtslayer);

            }

            xml.Save(path);

        }

        public void PoiConf(string loginid)
        {
            string path = HttpContext.Current.Server.MapPath("~/data/file_") + loginid+ "\\PoiConf.xml";
            if (File.Exists(path))
                File.Delete(path);
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            XmlDocument xml = new XmlDocument();
            var configuration = xml.CreateElement("configuration");
            xml.AppendChild(configuration);

            var attributes = xml.CreateElement("attributes");
            attributes.SetAttribute("platform", "GeoGlobe");
            attributes.SetAttribute("url", this.getConfigValueByName("地名地址服务"));
            attributes.SetAttribute("layer", this.getConfigValueByName("地名地址图层名"));
            attributes.SetAttribute("type", "Point");
            attributes.SetAttribute("title", "地名地址查询");
            attributes.SetAttribute("addressDelete", this.getConfigValueByName("地名地址替换文字"));
            attributes.SetAttribute("keyName", this.getConfigValueByName("地名地址地名字段"));
            attributes.SetAttribute("keyAddress", this.getConfigValueByName("地名地址地址字段"));
            configuration.AppendChild(attributes);
            xml.Save(path);

        }
        public void LocalConf(string loginid)
        {
            string path = HttpContext.Current.Server.MapPath("~/data/file_") + loginid + "\\LocalConf.xml";
            if (File.Exists(path))
                File.Delete(path);
            XmlDocument xml = new XmlDocument();
            var configuration = xml.CreateElement("configuration");
            xml.AppendChild(configuration);

            var city = xml.CreateElement("city");
            city.SetAttribute("key", this.getConfigValueByName("地区拼音"));
            city.SetAttribute("title", this.getConfigValueByName("地区名"));
            configuration.AppendChild(city);

            var major = xml.CreateElement("major");
            major.SetAttribute("name", this.getConfigValueByName("地区名"));
            major.SetAttribute("anchor", "Point("+ this.getConfigValueByName("中心经度") +" "+this.getConfigValueByName("中心纬度") + ")");
            major.SetAttribute("scale", "36111.9818670124");
            major.SetAttribute("nId", "1");
            city.AppendChild(major);

            var table = this.regionsbll.GetBySQL("select * from region where pid='0'  and sysname = (select top 1 ConfigValue from [config] where configname='地区名')");
            foreach(var row in table)
            {
                var minor = xml.CreateElement("minor");
                minor.SetAttribute("name", row.name);
                minor.SetAttribute("anchor", "Point(" + row.center.Replace(","," ") + ")");
                minor.SetAttribute("scale", "18055.9909335062");
                minor.SetAttribute("nId",row.regionId);
                major.AppendChild(minor);

            }
            xml.Save(path);
        }

        public void FeatureConf(string loginid)
        {
            string path = HttpContext.Current.Server.MapPath("~/data/file_") + loginid + "\\FeatureConf.xml";
            if (File.Exists(path))
                File.Delete(path);
            XmlDocument xml = new XmlDocument();
            var configuration = xml.CreateElement("configuration");
            xml.AppendChild(configuration);

            var city = xml.CreateElement("city");
            city.SetAttribute("key", this.getConfigValueByName("地区拼音"));
            city.SetAttribute("title", this.getConfigValueByName("地区名"));
            configuration.AppendChild(city);
            var table = themebll.GetBySQL("select * from theme order by idx");

            foreach (var row in table)
            {
                var feature = xml.CreateElement("feature");
                feature.SetAttribute("title", row.name);
                feature.SetAttribute("uid",row.themeId);
                feature.SetAttribute("id", row.themeId);
                city.AppendChild(feature);

                var wfs = xml.CreateElement("wfs");
                wfs.SetAttribute("type", "Polygen");
                wfs.SetAttribute("platform", "GeoGlobe");
                wfs.SetAttribute("url", row.wfs_url);
                wfs.SetAttribute("layer", row.wfs_layer);
                wfs.SetAttribute("domain", "");
                wfs.SetAttribute("token", "");
                feature.AppendChild(wfs);

                var wmts = xml.CreateElement("wmts");
             
                wmts.SetAttribute("type", "feature");
                wmts.SetAttribute("platform", "GeoServer");
                wmts.SetAttribute("title", row.name);
                wmts.SetAttribute("uid", row.themeId);
                wmts.SetAttribute("domain", "");
                wmts.SetAttribute("token", "");
                feature.AppendChild(wmts);
                if (!row.Iswmts_urlNull() || row.wmts_url.Trim() != "")
                {
                    var envelope = xml.CreateElement("envelope");
                    envelope.InnerText = "-180,-90 180,90";
                    wmts.AppendChild(envelope);

                    var levels = xml.CreateElement("levels");
                    levels.InnerText = row.wmts_levels;
                    wmts.AppendChild(levels);

                    var offline = xml.CreateElement("offline");
                    offline.InnerText = "digitalmapgis/" + this.getConfigValueByName("地区拼音") + "/";
                    wmts.AppendChild(offline);


                    var url = xml.CreateElement("url");
                    url.InnerText = row.wmts_url;
                    wmts.AppendChild(url);

                    var layer = xml.CreateElement("layer");
                    layer.InnerText = row.wmts_layer;
                    wmts.AppendChild(layer);

                    var style = xml.CreateElement("style");
                    style.InnerText = row.wmts_style;
                    wmts.AppendChild(style);

                    var tileMatrixSet = xml.CreateElement("tileMatrixSet");
                    tileMatrixSet.InnerText = row.wmts_tileMatrixSet;
                    wmts.AppendChild(tileMatrixSet);

                    var serviceMode = xml.CreateElement("serviceMode");
                    serviceMode.InnerText = "KVP";
                    wmts.AppendChild(serviceMode);

                    var format = xml.CreateElement("format");
                    format.InnerText = row.wmts_format;
                    wmts.AppendChild(format);

                    var version = xml.CreateElement("version");
                    version.InnerText = "1.0.0";
                    wmts.AppendChild(version);


                    var service = xml.CreateElement("service");
                    service.InnerText = "WMTS";
                    wmts.AppendChild(service);

                    var request = xml.CreateElement("request");
                    request.InnerText = "GetTile";
                    wmts.AppendChild(request);
                }
                var attributes = xml.CreateElement("attributes");

                feature.AppendChild(attributes);
                Dictionary<string, XmlNode> dic = new Dictionary<string, XmlNode>();
                foreach (string str in row.wfs_showfields.Split(';'))
                {
                    string key = str.Split(',')[1];
                    string name = str.Split(',')[0];
                    if (!dic.ContainsKey(key))
                    {
                        var attribute = xml.CreateElement("attribute");
                        attribute.SetAttribute("key", key);
                        attribute.SetAttribute("title", name);
                        attribute.SetAttribute("type", "string");
                        attribute.SetAttribute("SearchShowName", "yes");
                        dic[key] = attribute;
                          
                    }
                   
                }
                foreach (var att in dic.Values)
                {
                    attributes.AppendChild(att);
                }
                
                var statistics = xml.CreateElement("statistics");
                if (row.wfs_classifyfield.Split(',').Length == 2)
                {
                    statistics.SetAttribute("key", row.wfs_classifyfield.Split(',')[1]);
                    feature.AppendChild(statistics);
                }


            }


            xml.Save(path);
        }

        public void CityConf(string loginid)
        {
            string path = HttpContext.Current.Server.MapPath("~/data/file_") + loginid + "\\CityConf.xml";
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            if (File.Exists(path))
                File.Delete(path);
            XmlDocument xml = new XmlDocument();
            var configuration = xml.CreateElement("configuration");
            xml.AppendChild(configuration);

            var city = xml.CreateElement("city");
            city.SetAttribute("key", this.getConfigValueByName("地区拼音"));
            city.SetAttribute("title", this.getConfigValueByName("地区名"));
            city.SetAttribute("anchor", "Point(" + this.getConfigValueByName("中心经度") + " " + this.getConfigValueByName("中心纬度") + ")");
            city.SetAttribute("scale", "36111.9818670124");
            configuration.AppendChild(city);
            var table = baseMapbll.GetBySQL("select * from baseMap order by idx");

            foreach (var row in table)
            {
    


                var wmts = xml.CreateElement("wmts");
                wmts.SetAttribute("type", row.type);
                wmts.SetAttribute("platform", "GeoServer");
                wmts.SetAttribute("title", row.layername);
                wmts.SetAttribute("uid", row.mapId);
                wmts.SetAttribute("domain", "");
                wmts.SetAttribute("token", "");
                city.AppendChild(wmts);

                var envelope = xml.CreateElement("envelope");
                envelope.InnerText = "-180,-90 180,90";
                wmts.AppendChild(envelope);

                var levels = xml.CreateElement("levels");
                levels.InnerText = row.levels;
                wmts.AppendChild(levels);

                var offline = xml.CreateElement("offline");
                offline.InnerText = "digitalmapgis/" + this.getConfigValueByName("地区名") + "/";
                wmts.AppendChild(offline);


                var url = xml.CreateElement("url");
                url.InnerText = row.url;
                wmts.AppendChild(url);

                var layer = xml.CreateElement("layer");
                layer.InnerText = row.layeridfilter;
                wmts.AppendChild(layer);

                var style = xml.CreateElement("style");
                style.InnerText = row.style;
                wmts.AppendChild(style);

                var tileMatrixSet = xml.CreateElement("tileMatrixSet");
                tileMatrixSet.InnerText = row.tileMatrixSet;
                wmts.AppendChild(tileMatrixSet);

                var serviceMode = xml.CreateElement("serviceMode");
                serviceMode.InnerText = "KVP";
                wmts.AppendChild(serviceMode);

                var format = xml.CreateElement("format");
                format.InnerText = row.format;
                wmts.AppendChild(format);

                var version = xml.CreateElement("version");
                version.InnerText = "1.0.0";
                wmts.AppendChild(version);


                var service = xml.CreateElement("service");
                service.InnerText = "WMTS";
                wmts.AppendChild(service);

                var request = xml.CreateElement("request");
                request.InnerText = "GetTile";
                wmts.AppendChild(request);



            }
                xml.Save(path);
        }

        public void SetAllConfig()
        {
            ADOBaseBLL<Data.UserDataTable, Data.UserRow> userbll = new ADOBaseBLL<Data.UserDataTable, Data.UserRow>(new DBContext());
            var table = userbll.GetBySQL("select * from [user]");
            foreach (var row in table)
            {
                DB2themeservice_config(row.UserId.ToString());
                DB2cityservice_config(row.UserId.ToString());
                PoiConf(row.LoginID);
                LocalConf(row.LoginID);
                FeatureConf(row.LoginID);
                CityConf(row.LoginID);

            }

        }

    }
}