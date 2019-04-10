
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Xml;
using TY.Core;
using TY.UI.Ajax;
using WorkFlow;
using System.Linq;

namespace Web.Server
{
    /// <summary>
    /// WFSHandler 的摘要说明
    /// </summary>
    public class WFSHandler : AjaxHandler
    {
        private IBaseBLL<Data.themeDataTable, Data.themeRow> themebll = DBFactory<Data.themeDataTable, Data.themeRow>.GetBLL();
        private IBaseBLL<Data.regionDataTable, Data.regionRow> regionbll = DBFactory<Data.regionDataTable, Data.regionRow>.GetBLL();

        public string post(string txt, string url)
        {
            WebClient c = new WebClient();
            var r = c.UploadData(url, System.Text.Encoding.UTF8.GetBytes(txt));
            string str = System.Text.Encoding.UTF8.GetString(r);
            return str;
        }

        public string get(string url)
        {
            WebClient c = new WebClient();
            var r = c.DownloadData(url);
            string str = System.Text.Encoding.UTF8.GetString(r);
            return str;
        }


        public void addsearch()
        {

            string keyword = Query<string>("keyword");
            XMLHandler h = new XMLHandler();
            string namefield = h.getConfigValueByName("地名地址地名字段");
            string addfield = h.getConfigValueByName("地名地址地址字段");
            string replacestr = h.getConfigValueByName("地名地址替换文字");
            string strLayerName = h.getConfigValueByName("地名地址图层名");
            string url = h.getConfigValueByName("地名地址服务");
            string servertype = h.getConfigValueByName("服务类型");
            string querystr = getFilter(keyword, namefield, strLayerName, servertype);
            if (servertype.ToLower() == "arcserver")
            {
                url = url + "?request=Getfeature&TypeName="+ strLayerName + "&version=1.0.0&MAXFEATURES=300&Filter="+ querystr;
            }

            string r = "";
            DataTable table = new DataTable();
            table.Columns.Add("id");
            table.Columns.Add("addname");
            table.Columns.Add("add");
            table.Columns.Add("addcode");
            table.Columns.Add("pic");
            table.Columns.Add("latlng");

            if (servertype.ToLower() == "geoserver")
            {
                r = post(querystr, url);
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(r);
                XmlNamespaceManager xnm = new XmlNamespaceManager(xml.NameTable);
                //xnm.AddNamespace("xmlns", "http://quartznet.sourceforge.net/JobSchedulingData1");
                xnm.AddNamespace("gml", "http://www.opengis.net/gml");
                xnm.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                xnm.AddNamespace("wfs", "http://www.opengis.net/wfs");
                xnm.AddNamespace("schemaLocation", "http://www.geostar.com.cn/geoglobe http://192.168.1.182:9010/POI_PUB_441781/wfs?service=WFS&amp;version=1.0.0&amp;request=DescribeFeatureType http://www.opengis.net/wfs http://192.168.1.182:9010/POI_PUB_441781/WFS-basic.xsd");
                xnm.AddNamespace("d", "http://www.geostar.com.cn/geoglobe");

                var namelist = xml.SelectNodes("//d:" + strLayerName + "//d:" + namefield, xnm);
                var addlist = xml.SelectNodes("//d:" + strLayerName + "//d:" + addfield, xnm);
                var latlnglist = xml.SelectNodes("//d:" + strLayerName + "//gml:coordinates", xnm);
                for (int i = 0; i < namelist.Count; i++)
                {
                    var row = table.NewRow();
                    row["id"] = namelist[i].InnerText;
                    row["addname"] = namelist[i].InnerText;
                    row["add"] = addlist[i].InnerText.Replace(replacestr, "");
                    row["latlng"] = latlnglist[i].InnerText;
                    table.Rows.Add(row);


                }
            }
            else if (servertype.ToLower() == "arcserver")
            {
                r = get(url);
                r = Regex.Replace(r, "</\\w+?:", "</");
                r = Regex.Replace(r, "<\\w+?:", "<");
                r = Regex.Replace(r, "<FeatureCollection.+?>", "<FeatureCollection>");
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(r);
                //XmlNamespaceManager xnm = new XmlNamespaceManager(xml.NameTable);
                //xnm.AddNamespace("xmlns", "http://quartznet.sourceforge.net/JobSchedulingData1");
                //xnm.AddNamespace("gml", "http://www.opengis.net/gml");
                //xnm.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                //xnm.AddNamespace("wfs", "http://www.opengis.net/wfs");
                //xnm.AddNamespace("schemaLocation", "http://www.geostar.com.cn/geoglobe http://192.168.1.182:9010/POI_PUB_441781/wfs?service=WFS&amp;version=1.0.0&amp;request=DescribeFeatureType http://www.opengis.net/wfs http://192.168.1.182:9010/POI_PUB_441781/WFS-basic.xsd");
                ////xnm.AddNamespace("d", "http://www.geostar.com.cn/geoglobe");

                var namelist = xml.SelectNodes("//" + strLayerName + "//" + namefield);
                var addlist = xml.SelectNodes("//" + strLayerName + "//" + addfield);
                var latlnglist = xml.SelectNodes("//" + strLayerName + "//coordinates");
                for (int i = 0; i < namelist.Count; i++)
                {
                    var row = table.NewRow();
                    row["id"] = namelist[i].InnerText;
                    row["addname"] = namelist[i].InnerText;
                    row["add"] = addlist[i].InnerText.Replace(replacestr, "");
                    row["latlng"] = latlnglist[i].InnerText;
                    table.Rows.Add(row);


                }
            }
            else if (servertype.ToLower() == "arcserverquery")
            {
               // http://192.168.2.146:6080/arcgis/rest/services/%E5%B8%82%E6%B0%91%E6%94%BF%E5%B1%80/%E5%9C%B0%E5%90%8D%E5%9C%B0%E5%9D%80/MapServer/0/query?where=ADDCODE+like+%27%25120%25%27&text=&objectIds=&time=&geometry=&geometryType=esriGeometryEnvelope&inSR=&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=*&returnGeometry=true&maxAllowableOffset=&geometryPrecision=&outSR=&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&gdbVersion=&returnDistinctValues=false&f=pjson
                string where = namefield+" like '%"+ keyword + "%'";
                r = get(url + "/query?where=" + HttpUtility.UrlEncode(where) + "&text=&objectIds=&time=&geometry=&geometryType=esriGeometryEnvelope&inSR=&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=*&returnGeometry=true&maxAllowableOffset=&geometryPrecision=&outSR=&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&gdbVersion=&returnDistinctValues=false&f=pjson");
                r = r.Replace("\n", "").Replace(" ", "");
                var rdic = JsonHelper.JsonToDictionary(r);

               

                var fieldAliases = rdic["fieldAliases"] as Dictionary<string, object>;
                var features = rdic["features"] as ArrayList;
                foreach (object ob in features)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    var feature = ob as Dictionary<string, object>;
                    var geometry = feature["geometry"] as Dictionary<string, object>;
                    var attr = feature["attributes"] as Dictionary<string, object>;



                    var row = table.NewRow();
                    row["id"] = attr[namefield].ToString();
                     row["addname"] = attr[namefield].ToString();
                    row["add"] = attr[addfield].ToString();
                    row["latlng"] = geometry["x"].ToString()+","+geometry["y"].ToString();
                    table.Rows.Add(row);
                }
                
            }
            jsonResult = JsonHelper.DataTableToJSON(table);
        }

        private  string getFilter(string keyword, string namefield, string strLayerName,string servertype)
        {
            if (servertype.ToLower() == "arcserver")
                return "<ogc:Filter><ogc:PropertyIsLike wildCard='*' singleChar='.' escape='!'><ogc:PropertyName>" + namefield + "</ogc:PropertyName><ogc:Literal>*"+ keyword + "*</ogc:Literal></ogc:PropertyIsLike></ogc:Filter>";
            else

                return @"<?xml version='1.0' encoding='UTF-8'?><wfs:GetFeature maxFeatures='300'"

                    + @" service='WFS' version='1.0.0'  xsi:schemaLocation='http://www.opengis.net/wfs http://schemas.opengis.net/wfs/1.0.0/wfs.xsd' xmlns:wfs='http://www.opengis.net/wfs' xmlns:gml='http://www.opengis.net/gml' xmlns:ogc='http://www.opengis.net/ogc' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>"
                + "  <wfs:Query typeName='"
                        + strLayerName + "' srsName='EPSG:4326'>"
                + "   <ogc:Filter>"
                + "      <ogc:And>"
                + "       <ogc:Or>"
                + "        <ogc:PropertyIsLike wildCard='*' singleChar='.' escape='!'>"
                + "          <ogc:PropertyName>"
                        + namefield + "</ogc:PropertyName>"
                + "            <ogc:Literal>*"
                        + keyword + "*</ogc:Literal>"
               + "          </ogc:PropertyIsLike>"

               + "          </ogc:Or>"
                + "          </ogc:And>"
                + "         </ogc:Filter>"
                + "     </wfs:Query>"
                + "</wfs:GetFeature>";
        }

        public void clickSearch()
        {
            var themeId = this.Query<string>("themeId");
            var coordinateStr = this.Query<string>("coordinateStr");
            var row = themebll.GetByKey(themeId);

            if (row.wmts_type.ToLower() == "" || row.wmts_type.ToLower() == "geoserver")
            {
                string queryStr = "<?xml version='1.0' encoding='UTF-8'?><wfs:GetFeature maxFeatures='1' service='WFS' version='1.0.0' xsi:schemaLocation='http://www.opengis.net/wfs http://schemas.opengis.net/wfs/1.0.0/wfs.xsd' xmlns:wfs='http://www.opengis.net/wfs' xmlns:gml='http://www.opengis.net/gml' xmlns:ogc='http://www.opengis.net/ogc' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>";

                // 多图层查询
                foreach (String strTypeName in row.wfs_layer.Split(','))
                {
                    queryStr += "  <wfs:Query typeName='" + strTypeName + "'>" //  srsName='EPSG:4326'
                    + "    <ogc:Filter>"
                    + "      <ogc:And>"
                    + "        <ogc:Intersects>"
                    + "          <ogc:PropertyName>geometry</ogc:PropertyName>"
                    + "          <gml:Polygon>"
                    + "            <gml:outerBoundaryIs>"
                    + "              <gml:LinearRing>"
                    + "                <gml:coordinates decimal='.' cs=',' ts=';'>" + coordinateStr.Replace(";", " ") + "</gml:coordinates>"
                    + "              </gml:LinearRing>"
                    + "            </gml:outerBoundaryIs>"
                    + "          </gml:Polygon>"
                    + "        </ogc:Intersects>"
                    + "      </ogc:And>"
                    + "    </ogc:Filter>"
                    + "  </wfs:Query>";
                }

                queryStr += "</wfs:GetFeature>";
                var txt = this.post(queryStr, row.wfs_url);
                Dictionary<string, string> dic = XmltoDic(row, txt);
                jsonResult = JsonHelper.ObjectToJSON(dic);
            }
            else if (row.wmts_type.ToLower() == "arcserver")
            {
                string newcoord = "";
                foreach (string i in coordinateStr.Split(';'))
                {
                    newcoord += i.Split(',')[1] + "," + i.Split(',')[0] + " ";
                }
                newcoord = newcoord.Trim();
                string querystr = "<ogc:Filter xmlns:gml='http://www.opengis.net/gml' xmlns:ogc='http://www.opengis.net/ogc'><ogc:Intersects><ogc:PropertyName>SHAPE</ogc:PropertyName><gml:Polygon><gml:outerBoundaryIs><gml:LinearRing><gml:coordinates srsName='urn:ogc:def:crs:EPSG:6.9:4490'>" + newcoord + "</gml:coordinates></gml:LinearRing></gml:outerBoundaryIs></gml:Polygon></ogc:Intersects></ogc:Filter>";
                var r = this.get(row.wfs_url + "?request=Getfeature&service=WFS&TypeName=" + row.wfs_layer + "&MAXFEATURES=1&Filter=" + querystr);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                r = DelXmlNSP(r);
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(r);
                if (xml.SelectSingleNode("//featureMember//coordinates") != null)
                {
                    dic["latlng"] = xml.SelectSingleNode("//featureMember//coordinates").InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
                }
                else if (xml.SelectSingleNode("//featureMember//posList") != null)
                {
                    string latlng = xml.SelectSingleNode("//featureMember//posList").InnerText.Trim().Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
                    var latlngs = latlng.Split(';');
                    dic["latlng"] = "";
                    for (int i = 0; i < latlngs.Length - 1; i = i + 2)
                    {
                        if (dic["latlng"] != "")
                            dic["latlng"] += ";";
                        dic["latlng"] += latlngs[i + 1] + "," + latlngs[i];

                    }



                }
                else
                {
                    dic["无数据"] = "";
                    jsonResult = JsonHelper.ObjectToJSON(dic);
                    return;
                }

                //dic["latlng"] = xml.SelectSingleNode("//member//coordinates").InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');

                var node = xml.SelectSingleNode("//featureMember");
                foreach (string str in row.wfs_showfields.Split(';'))
                {
                    string key = str.Split(',')[0];
                    string value = str.Trim(';').Split(',')[1];
                    if (node.SelectSingleNode("//" + value) != null)
                    {
                        dic[key] = node.SelectSingleNode("//" + value).InnerText;
                        var geoName = node.SelectSingleNode("//Shape|//SHAPE").FirstChild.Name;
                        if (geoName == "Polygon" || geoName == "MultiSurface")
                            dic["geotype"] = "面";
                        if (geoName == "Point")
                            dic["geotype"] = "点";

                    }
                }
                jsonResult = JsonHelper.ObjectToJSON(dic);
            }
            else if (row.wmts_type.ToLower() == "arcserverquery")
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                string newcoord = "";
                foreach (string i in coordinateStr.Split(';'))
                {
                    newcoord += "["+i.Split(',')[0] + "," + i.Split(',')[1] + "],";
                }
                newcoord = newcoord.Trim(',');
                string querystr = row.wfs_url + "/query?where=&text=&objectIds=&time=&geometry="+HttpUtility.UrlEncode("{\"rings\" : [["+ newcoord + "]], \"spatialReference\" : {\"wkid\" : 4490}}")+"&geometryType=esriGeometryPolygon&inSR=4490&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=*&returnGeometry=true&maxAllowableOffset=&geometryPrecision=&outSR=4490&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&gdbVersion=&returnDistinctValues=false&f=pjson";
                var r = this.get(querystr).Replace("\n","").Replace(" ","");
                var rdic = JsonHelper.JsonToDictionary(r);

                var fieldAliases = rdic["fieldAliases"] as Dictionary<string, object>;
                var features = rdic["features"] as ArrayList;
                if(features.Count==0)
                {
                    dic["无数据"] = "";
                    jsonResult = JsonHelper.ObjectToJSON(dic);
                    return;
                }
                var feature = features[0] as Dictionary<string, object>;
                var geometry = feature["geometry"] as Dictionary<string, object>;

                var geoName = rdic["geometryType"].ToString();
                if (geoName == "esriGeometryPolygon")
                {
                    dic["geotype"] = "面";
                    var rings = JsonHelper.ObjectToJSON(geometry["rings"]).Replace("\n", "").Replace("[", "").Replace("],", ";").Replace("]","");
                    dic["latlng"] = rings;
                }
                if (geoName == "esriGeometryPoint")
                {
                    dic["geotype"] = "点";
                }
                if (geoName == "esriGeometryPolyline")
                {
                    dic["geotype"] = "线";
                }
      
                foreach (string str in row.wfs_showfields.Split(';'))
                {

                    string cn = str.Split(',')[0];
                    string en = str.Split(',')[1];
                    //var keyvalue = fieldAliases.Where(p => p.Value.ToString() == en).FirstOrDefault();

                    var attr = feature["attributes"] as Dictionary<string, object>;
                    //if (!default(KeyValuePair<string, string>).Equals(keyvalue) && attr.ContainsKey(keyvalue.Key))
                    //    dic[cn] = attr[keyvalue.Key]==null?null: attr[keyvalue.Key].ToString();
                    if (attr.ContainsKey(en))
                    {
                        if (attr[en] != null)
                            dic[cn] = attr[en].ToString();
                        else
                            dic[cn] = "";
                    }

                }
                jsonResult = JsonHelper.ObjectToJSON(dic);
            }





        }

        private string DelXmlNSP(string r)
        {
            r = Regex.Replace(r, "</\\w+?:", "</");
            r = Regex.Replace(r, "<\\w+?:", "<");
            r = Regex.Replace(r, "<FeatureCollection.+?>", "<FeatureCollection>");
            r = Regex.Replace(r, "\\w+?:", "");
            return r;
        }

        private static Dictionary<string, string> XmltoDic(Data.themeRow row, string txt)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(txt);
            XmlNamespaceManager xnm = new XmlNamespaceManager(xml.NameTable);
            xnm.AddNamespace("gml", "http://www.opengis.net/gml");
            xnm.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xnm.AddNamespace("wfs", "http://www.opengis.net/wfs");
            xnm.AddNamespace("schemaLocation", "http://www.geostar.com.cn/geoglobe http://192.168.1.182:9010/POI_PUB_441781/wfs?service=WFS&amp;version=1.0.0&amp;request=DescribeFeatureType http://www.opengis.net/wfs http://192.168.1.182:9010/POI_PUB_441781/WFS-basic.xsd");
            xnm.AddNamespace("d", "http://www.geostar.com.cn/geoglobe");


            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (xml.SelectSingleNode("//gml:featureMember//gml:coordinates", xnm) == null)
            {
                dic["无数据"] = "";
                return dic;
            }

            dic["latlng"] = xml.SelectSingleNode("//gml:featureMember//gml:coordinates", xnm).InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');

            var node = xml.SelectSingleNode("//gml:featureMember", xnm);
            foreach (string str in row.wfs_showfields.Split(';'))
            {
                string key = str.Split(',')[0];
                string value = str.Trim(';').Split(',')[1];
                if (node.SelectSingleNode("//d:" + value, xnm) != null)
                {
                    dic[key] = node.SelectSingleNode("//d:" + value, xnm).InnerText;
                    var geoName = node.SelectSingleNode("//d:GEOMETRY", xnm).FirstChild.Name;
                    if (geoName == "gml:Polygon")
                        dic["geotype"] = "面";
                    if (geoName == "gml:Point")
                        dic["geotype"] = "点";

                }
            }

            return dic;
        }

        public void KeywordSearch()
        {
            var themeId = this.Query<string>("themeId");
            var field = this.Query<string>("field");
            var keyword = this.Query<string>("keyword");
            var row = themebll.GetByKey(themeId);

            string queryStr = "<?xml version=\"1.0\" encoding=\"utf-8\"?><wfs:GetFeature xmlns:wfs=\"http://www.opengis.net/wfs\" xmlns:gml=\"http://www.opengis.net/gml\" xmlns:ogc=\"http://www.opengis.net/ogc\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" maxFeatures=\"50\" service=\"WFS\" version=\"1.0.0\" outputFormat=\"text/xml; subtype=gml/3.1.1\" xsi:schemaLocation=\"http://www.opengis.net/wfs http://schemas.opengis.net/wfs/1.0.0/wfs.xsd\">  <wfs:Query typeName=\"@typeName\"><ogc:Filter><ogc:And><ogc:Or><ogc:PropertyIsLike wildCard=\"*\" singleChar=\".\" escape=\"!\"><ogc:PropertyName>@PropertyName</ogc:PropertyName><ogc:Literal>*@Literal*</ogc:Literal></ogc:PropertyIsLike></ogc:Or></ogc:And></ogc:Filter></wfs:Query></wfs:GetFeature>";
            queryStr = queryStr.Replace("@typeName", row.wfs_layer);
            queryStr = queryStr.Replace("@PropertyName", field);
            queryStr = queryStr.Replace("@Literal", keyword);
            if (row.wmts_type == "geoserver" || row.wmts_type == "")
            {
                var txt = this.post(queryStr, row.wfs_url);


                XmlDocument xml = new XmlDocument();
                xml.LoadXml(txt);
                XmlNamespaceManager xnm = new XmlNamespaceManager(xml.NameTable);
                xnm.AddNamespace("gml", "http://www.opengis.net/gml");
                xnm.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                xnm.AddNamespace("wfs", "http://www.opengis.net/wfs");
                xnm.AddNamespace("schemaLocation", "http://www.geostar.com.cn/geoglobe http://192.168.1.182:9010/POI_PUB_441781/wfs?service=WFS&amp;version=1.0.0&amp;request=DescribeFeatureType http://www.opengis.net/wfs http://192.168.1.182:9010/POI_PUB_441781/WFS-basic.xsd");
                xnm.AddNamespace("d", "http://www.geostar.com.cn/geoglobe");
                var nodes = xml.SelectNodes("//gml:featureMember//d:" + row.wfs_layer, xnm);
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

                foreach (XmlNode node in nodes)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic["latlng"] = node.SelectSingleNode("descendant::gml:coordinates", xnm).InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
                    foreach (string str in row.wfs_showfields.Split(';'))
                    {
                        string key = str.Split(',')[0];
                        string value = str.Split(',')[1];
                        if (node.SelectSingleNode("descendant::d:" + value, xnm) != null)
                        {
                            dic[key] = node.SelectSingleNode("descendant::d:" + value, xnm).InnerText;
                            var geoName = node.SelectSingleNode("descendant::d:GEOMETRY", xnm).FirstChild.Name;
                            if (geoName == "gml:Polygon")
                                dic["geotype"] = "Polygon";
                            if (geoName == "gml:Point")
                                dic["geotype"] = "Point";
                        }
                    }
                    list.Add(dic);
                }



                jsonResult = JsonHelper.ObjectToJSON(list);
            }
            else if (row.wmts_type == "arcserver")
            {
                queryStr = "<ogc:Filter><ogc:PropertyIsLike wildCard='*' singleChar='.' escape='!'><ogc:PropertyName>" + field + "</ogc:PropertyName><ogc:Literal>*" + keyword + "*</ogc:Literal></ogc:PropertyIsLike></ogc:Filter>";
                var url = row.wfs_url + "?request=Getfeature&TypeName=" + row.wfs_layer + "&version=1.0.0&MAXFEATURES=300&Filter=" + queryStr;
                var txt = get(url);
                txt = DelXmlNSP(txt);
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(txt);
                var nodes = xml.SelectNodes("//featureMember//" + row.wfs_layer);
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

                foreach (XmlNode node in nodes)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic["latlng"] = node.SelectSingleNode("descendant::coordinates").InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
                    foreach (string str in row.wfs_showfields.Split(';'))
                    {
                        string key = str.Split(',')[0];
                        string value = str.Split(',')[1];
                        if (node.SelectSingleNode("descendant::" + value) != null)
                        {
                            dic[key] = node.SelectSingleNode("descendant::" + value).InnerText;
                            var geoName = node.SelectSingleNode("descendant::SHAPE").FirstChild.Name;
                            if (geoName == "Polygon")
                                dic["geotype"] = "Polygon";
                            if (geoName == "Point")
                                dic["geotype"] = "Point";
                        }
                    }
                    list.Add(dic);
                }
                jsonResult = JsonHelper.ObjectToJSON(list);
            }
            else if (row.wmts_type == "arcserverquery")
            {
                string where = field + " like '%" + keyword + "%'";
                var r = get(row.wfs_url + "/query?where=" + HttpUtility.UrlEncode(where) + "&text=&objectIds=&time=&geometry=&geometryType=esriGeometryEnvelope&inSR=&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=*&returnGeometry=true&maxAllowableOffset=&geometryPrecision=&outSR=&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&gdbVersion=&returnDistinctValues=false&f=pjson");
                r = r.Replace("\n", "").Replace(" ", "");
                var rdic = JsonHelper.JsonToDictionary(r);


                var geoName = rdic["geometryType"].ToString();
                var fieldAliases = rdic["fieldAliases"] as Dictionary<string, object>;
                var features = rdic["features"] as ArrayList;
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                foreach (object ob in features)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    var feature = ob as Dictionary<string, object>;
                    var geometry = feature["geometry"] as Dictionary<string, object>;
                    if (geoName == "esriGeometryPolygon")
                    {
                        dic["geotype"] = "面";
                        var rings = JsonHelper.ObjectToJSON(geometry["rings"]).Replace("\n", "").Replace("[", "").Replace("],", ";").Replace("]", "");
                        dic["latlng"] = rings;
                    }
                    if (geoName == "esriGeometryPoint")
                    {
                        dic["geotype"] = "点";
                        row["latlng"] = geometry["x"].ToString() + "," + geometry["y"].ToString();
                    }
                    if (geoName == "esriGeometryPolyline")
                    {

                        dic["geotype"] = "线";
                        var lines = JsonHelper.ObjectToJSON(geometry["lines"]).Replace("\n", "").Replace("[", "").Replace("],", ";").Replace("]", "");
                        dic["latlng"] = lines;
                    }

                    foreach (string str in row.wfs_showfields.Split(';'))
                    {
                        string cn = str.Split(',')[0];
                        string en = str.Split(',')[1];
                        var attr = feature["attributes"] as Dictionary<string, object>;
                        if (attr.ContainsKey(en))
                        {
                            if (attr[en] != null)
                                dic[cn] = attr[en].ToString();
                            else
                                dic[cn] = "";
                        }

 
                        
                        //if (node.SelectSingleNode("descendant::" + value) != null)
                        //{
                        //    dic[key] = node.SelectSingleNode("descendant::" + value).InnerText;
                        //    var geoName = node.SelectSingleNode("descendant::SHAPE").FirstChild.Name;
                        //    if (geoName == "Polygon")
                        //        dic["geotype"] = "Polygon";
                        //    if (geoName == "Point")
                        //        dic["geotype"] = "Point";
                        //}
                    }
                    list.Add(dic);


                    //var row = table.NewRow();
                    //row["id"] = feature[namefield].ToString();
                    //row["addname"] = feature[namefield].ToString();
                    //row["add"] = feature[addfield].ToString();
                    //row["latlng"] = geometry["x"].ToString() + "," + geometry["y"].ToString();
                    //table.Rows.Add(row);
                }
                jsonResult = JsonHelper.ObjectToJSON(list);
            }
        }

        public void DrawWFS()
        {
            var themeId = this.Query<string>("themeId");


            var row = themebll.GetByKey(themeId);
            string query = "?VERSION=1.0.0&REQUEST=GetFeature&TypeName=" + row.wfs_layer + "&MaxFeatures=300";

            WebClient c = new WebClient();
            var r = c.DownloadData(row.wfs_url + query);
            string txt = System.Text.Encoding.UTF8.GetString(r);



            XmlDocument xml = new XmlDocument();
            
            if (row.wmts_type == "geoserver" || row.wmts_type =="")
            {
                xml.LoadXml(txt);
                XmlNamespaceManager xnm = new XmlNamespaceManager(xml.NameTable);
                xnm.AddNamespace("gml", "http://www.opengis.net/gml");
                xnm.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                xnm.AddNamespace("wfs", "http://www.opengis.net/wfs");
                xnm.AddNamespace("schemaLocation", "http://www.geostar.com.cn/geoglobe http://192.168.1.182:9010/POI_PUB_441781/wfs?service=WFS&amp;version=1.0.0&amp;request=DescribeFeatureType http://www.opengis.net/wfs http://192.168.1.182:9010/POI_PUB_441781/WFS-basic.xsd");
                xnm.AddNamespace("d", "http://www.geostar.com.cn/geoglobe");

                var nodes = xml.SelectNodes("//gml:featureMember//d:" + row.wfs_layer, xnm);
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

                foreach (XmlNode node in nodes)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic["latlng"] = node.SelectSingleNode("descendant::gml:coordinates", xnm).InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
                    foreach (string str in row.wfs_showfields.Split(';'))
                    {
                        string key = str.Split(',')[0];
                        string value = str.Split(',')[1];
                        if (node.SelectSingleNode("descendant::d:" + value, xnm) != null)
                        {
                            dic[key] = node.SelectSingleNode("descendant::d:" + value, xnm).InnerText;
                            var geoName = node.SelectSingleNode("descendant::d:GEOMETRY", xnm).FirstChild.Name;
                            if (geoName == "gml:Polygon")
                                dic["geotype"] = "Polygon";
                            if (geoName == "gml:Point")
                                dic["geotype"] = "Point";

                        }

                    }
                    list.Add(dic);
                }



                jsonResult = JsonHelper.ObjectToJSON(list);
            }
            else if(row.wmts_type =="arcserver")
            {
                txt = DelXmlNSP(txt);
                xml.LoadXml(txt);
                var nodes = xml.SelectNodes("//featureMember//" + row.wfs_layer);
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

                foreach (XmlNode node in nodes)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic["latlng"] = node.SelectSingleNode("descendant::coordinates").InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
                    foreach (string str in row.wfs_showfields.Split(';'))
                    {
                        string key = str.Split(',')[0];
                        string value = str.Split(',')[1];
                        if (node.SelectSingleNode("descendant::" + value) != null)
                        {
                            dic[key] = node.SelectSingleNode("descendant::" + value).InnerText;
                            var geoName = node.SelectSingleNode("descendant::SHAPE").FirstChild.Name;
                            if (geoName == "Polygon")
                                dic["geotype"] = "Polygon";
                            if (geoName == "Point")
                                dic["geotype"] = "Point";

                        }

                    }
                    list.Add(dic);
                }



                jsonResult = JsonHelper.ObjectToJSON(list);
            }
            //else if (row.wmts_type == "arcserverquery")
            //{
            //    txt = DelXmlNSP(txt);
            //    xml.LoadXml(txt);
            //    var nodes = xml.SelectNodes("//featureMember//" + row.wfs_layer);
            //    List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

            //    foreach (XmlNode node in nodes)
            //    {
            //        Dictionary<string, string> dic = new Dictionary<string, string>();
            //        dic["latlng"] = node.SelectSingleNode("descendant::coordinates").InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
            //        foreach (string str in row.wfs_showfields.Split(';'))
            //        {
            //            string key = str.Split(',')[0];
            //            string value = str.Split(',')[1];
            //            if (node.SelectSingleNode("descendant::" + value) != null)
            //            {
            //                dic[key] = node.SelectSingleNode("descendant::" + value).InnerText;
            //                var geoName = node.SelectSingleNode("descendant::SHAPE").FirstChild.Name;
            //                if (geoName == "Polygon")
            //                    dic["geotype"] = "Polygon";
            //                if (geoName == "Point")
            //                    dic["geotype"] = "Point";

            //            }

            //        }
            //        list.Add(dic);
            //    }



            //    jsonResult = JsonHelper.ObjectToJSON(list);
            //}
        }

        /// <summary>
        /// 叠加分析
        /// </summary>
        public void OverlayAnalysis()
        {

            string themeId = this.Query<String>("tempId");

            string coordinateStr = this.Query<String>("coordinateStr");
            var row = themebll.GetByKey(themeId);

            if (row.wmts_type.ToLower() == "geoserver" || row.wmts_type.ToLower() == "")
            {
                String sTotalString = "";


                StringBuilder sBuilderPostXml = new StringBuilder("");

                sBuilderPostXml.Append("<wfs:GetFeature maxFeatures='50' service='WFS' version='1.0.0' outputFormat='text/xml; subtype=gml/3.1.1' xsi:schemaLocation='http://www.opengis.net/wfs http://schemas.opengis.net/wfs/1.0.0/wfs.xsd'  xmlns:wfs='http://www.opengis.net/wfs' xmlns:gml='http://www.opengis.net/gml' xmlns:ogc='http://www.opengis.net/ogc' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>");
                sBuilderPostXml.Append("  <wfs:Query typeName='" + row.wfs_layer + "'>");
                sBuilderPostXml.Append("    <ogc:Filter><ogc:Intersects>");
                sBuilderPostXml.Append("      <ogc:PropertyName>geometry</ogc:PropertyName><gml:Polygon>");
                sBuilderPostXml.Append("        <gml:outerBoundaryIs><gml:LinearRing>");
                sBuilderPostXml.Append("          <gml:coordinates decimal='.' cs=',' ts=' '>" + coordinateStr.Replace(";", " ") + "</gml:coordinates>");
                sBuilderPostXml.Append("          </gml:LinearRing></gml:outerBoundaryIs></gml:Polygon>");
                sBuilderPostXml.Append("          </ogc:Intersects></ogc:Filter></wfs:Query></wfs:GetFeature>");
                //请求数据，xml格式的结果

                //		
                var txt = this.post(sBuilderPostXml.ToString(), row.wfs_url);

                XmlDocument xml = new XmlDocument();
                xml.LoadXml(txt);
                XmlNamespaceManager xnm = new XmlNamespaceManager(xml.NameTable);
                xnm.AddNamespace("gml", "http://www.opengis.net/gml");
                xnm.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                xnm.AddNamespace("wfs", "http://www.opengis.net/wfs");
                xnm.AddNamespace("schemaLocation", "http://www.geostar.com.cn/geoglobe http://192.168.1.182:9010/POI_PUB_441781/wfs?service=WFS&amp;version=1.0.0&amp;request=DescribeFeatureType http://www.opengis.net/wfs http://192.168.1.182:9010/POI_PUB_441781/WFS-basic.xsd");
                xnm.AddNamespace("d", "http://www.geostar.com.cn/geoglobe");






                var nodes = xml.SelectNodes("//gml:featureMember//d:" + row.wfs_layer, xnm);
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

                foreach (XmlNode node in nodes)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic["latlng"] = node.SelectSingleNode("descendant::gml:outerBoundaryIs//gml:coordinates", xnm).InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
                    string str = row.wfs_classifyfield;
                    string key = str.Split(',')[0];
                    string value = str.Split(',')[1];
                    dic["key"] = key;
                    dic["value"] = node.SelectSingleNode("descendant::d:" + value, xnm).InnerText;

                    list.Add(dic);
                }


                jsonResult = JsonHelper.ObjectToJSON(list);
            }
            else if (row.wmts_type.ToLower() == "arcserver")
            {
                string newcoord = "";
                foreach (string i in coordinateStr.Split(';'))
                {
                    newcoord += i.Split(',')[1] + "," + i.Split(',')[0] + " ";
                }
                newcoord = newcoord.Trim();
                string querystr = "<ogc:Filter xmlns:gml='http://www.opengis.net/gml' xmlns:ogc='http://www.opengis.net/ogc'><ogc:Intersects><ogc:PropertyName>SHAPE</ogc:PropertyName><gml:Polygon><gml:outerBoundaryIs><gml:LinearRing><gml:coordinates srsName='urn:ogc:def:crs:EPSG:6.9:4490'>" + newcoord + "</gml:coordinates></gml:LinearRing></gml:outerBoundaryIs></gml:Polygon></ogc:Intersects></ogc:Filter>";
                var r = this.get(row.wfs_url + "?request=Getfeature&service=WFS&TypeName=" + row.wfs_layer + "&MAXFEATURES=10&Filter=" + querystr);

                r = DelXmlNSP(r);
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(r);
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                var nodes = xml.SelectNodes("//featureMember//" + row.wfs_layer);
                foreach (XmlNode node in nodes)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    //dic["latlng"] = node.SelectSingleNode("descendant::gml:outerBoundaryIs//gml:coordinates").InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
                    if (xml.SelectSingleNode("//featureMember//coordinates") != null)
                    {
                        dic["latlng"] = xml.SelectSingleNode("//member//coordinates").InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
                    }
                    else if (xml.SelectNodes("//featureMember//posList") != null)
                    {
                        string latlng = xml.SelectSingleNode("//featureMember//posList").InnerText.Trim().Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
                        var latlngs = latlng.Split(';');
                        dic["latlng"] = "";
                        for (int i = 0; i < latlngs.Length - 1; i = i + 2)
                        {
                            if (dic["latlng"] != "")
                                dic["latlng"] += ";";
                            dic["latlng"] += latlngs[i + 1] + "," + latlngs[i];

                        }


                    }

                    string str = row.wfs_classifyfield;
                    string key = str.Split(',')[0];
                    string value = str.Split(',')[1];
                    dic["key"] = key;
                    dic["value"] = node.SelectSingleNode("descendant::" + value).InnerText;

                    list.Add(dic);
                }

                jsonResult = JsonHelper.ObjectToJSON(list);
            }
            else if (row.wmts_type.ToLower() == "arcserverquery")
            {
                
                string newcoord = "";
                foreach (string i in coordinateStr.Split(';'))
                {
                    newcoord += "[" + i.Split(',')[0] + "," + i.Split(',')[1] + "],";
                }
                newcoord = newcoord.Trim(',');
                string querystr = row.wfs_url + "/query?where=&text=&objectIds=&time=&geometry=" + HttpUtility.UrlEncode("{\"rings\" : [[" + newcoord + "]], \"spatialReference\" : {\"wkid\" : 4490}}") + "&geometryType=esriGeometryPolygon&inSR=4490&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=*&returnGeometry=true&maxAllowableOffset=&geometryPrecision=&outSR=4490&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&gdbVersion=&returnDistinctValues=false&f=pjson";
                var r = this.get(querystr).Replace("\n", "").Replace(" ","");
                string str = row.wfs_classifyfield;
                string cn = str.Split(',')[0];
                string en = str.Split(',')[1];
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

                var rdic = JsonHelper.JsonToDictionary(r);

                var fieldAliases = rdic["fieldAliases"] as Dictionary<string, object>;
                var features = rdic["features"] as ArrayList;
                foreach (object ob in features)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    var feature = ob as Dictionary<string, object>;
                    var geometry = feature["geometry"] as Dictionary<string, object>;

                    var rings = JsonHelper.ObjectToJSON(geometry["rings"]).Replace("\n", "").Replace("[", "").Replace("],", ";").Replace("]", "");
                    dic["latlng"] = rings;
                    dic["key"] = cn;//cn

                    var attr = feature["attributes"] as Dictionary<string, object>;
                    //var keyvalue = fieldAliases.Where(p => p.Value.ToString() == dic["key"].ToString()).FirstOrDefault();
                    //if (!default(KeyValuePair<string, string>).Equals(keyvalue) && attr.ContainsKey(keyvalue.Key))
                    //    dic["value"] = attr[keyvalue.Key] == null ? null : attr[keyvalue.Key].ToString();
                    if(attr[en]!=null)
                    dic["value"] = attr[en].ToString();
                    
                    list.Add(dic);
                }

                jsonResult = JsonHelper.ObjectToJSON(list);
            }
        }

        public void importTxt()
        {
            try
            {
                HttpFileCollection files = HttpContext.Current.Request.Files;


                if (files.Count > 0)
                {


                    for (int i = 0; i < files.Count; i++)
                    {
                        string path = HttpContext.Current.Server.MapPath("~/temp/");

                        string id = Guid.NewGuid().ToString();
                        Directory.CreateDirectory(path);
                        files[i].SaveAs(path + id + ".txt");
                        string txt = File.ReadAllText(path + id + ".txt");
                        HttpContext.Current.Session["temptxt"] = txt;

                    }


                    jsonResult = JsonHelper.OutResult(true, "上传成功！");
                    HttpContext.Current.Response.Redirect("~/import.html?success=1");

                }
                else
                {
                    base.jsonResult = JsonHelper.OutResult(false, "没有收到文件！");
                }

            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError(exception.Message);
            }
        }

        public void readAnFile()
        {
            jsonResult = "{\"str\":\"" + HttpContext.Current.Session["temptxt"].ToString() + "\"}";
        }

        public string Formatlatlng(string wfs_latlng)
        {
            return wfs_latlng.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
        }
        #region 地区导入

        public void addsysname()
        {
            var t = regionbll.GetNormalDataTable("select * from [region]");
            if(!t.Columns.Contains("sysname"))
               this.regionbll.ExecuteNonQuery("alter table [region] add sysname varchar(50)");
        }
        public void ImportRegionLatlngYC()
        {
            addsysname();
            //string filePath = HttpContext.Current.Server.MapPath("~/upload/区划/区划.shp");
            //ADOBaseBLL<Data.regionDataTable, Data.regionRow> regionbll = new ADOBaseBLL<Data.regionDataTable, Data.regionRow>(new DBContext());
            var sysname = regionbll.ExecuteScalar("select top 1 ConfigValue from [config] where configname='地区名'").ToString();
            regionbll.ExecuteNonQuery("delete from region where sysname = (select top 1 ConfigValue from [config] where configname='地区名') or sysname is null");
            XmlDocument xml = new XmlDocument();
            //xml.Load(HttpContext.Current.Server.MapPath("~/data/云安界线.xml"));
            xml.Load(HttpContext.Current.Server.MapPath("~/data/界线.xml"));
            XmlNamespaceManager xnm = new XmlNamespaceManager(xml.NameTable);
            xnm.AddNamespace("gml", "http://www.opengis.net/gml");
            xnm.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xnm.AddNamespace("wfs", "http://www.opengis.net/wfs");
            xnm.AddNamespace("schemaLocation", "http://www.geostar.com.cn/geoglobe http://192.168.1.182:9010/POI_PUB_441781/wfs?service=WFS&amp;version=1.0.0&amp;request=DescribeFeatureType http://www.opengis.net/wfs http://192.168.1.182:9010/POI_PUB_441781/WFS-basic.xsd");
            xnm.AddNamespace("d", "http://www.geostar.com.cn/geoglobe");

            var data = regionbll.GetBySQL("select * from region where sysname = (select top 1 ConfigValue from [config] where configname='地区名')");
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            //var nodes = xml.SelectNodes("//gml:featureMember//d:云安镇界2000坐标_GK2_V2014", xnm);
            var nodes = xml.SelectNodes("//gml:featureMember//d:YCZJ_ZW2_V2014", xnm);
            list = new List<Dictionary<string, string>>();

            foreach (XmlNode node in nodes)
            {

                string latlng = node.SelectSingleNode("descendant::gml:coordinates", xnm).InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
                string center = getCenterOfGravityPoint(latlng);
                //string name = node.SelectSingleNode("descendant::d:TOWN", xnm).InnerText;
                //string id = node.SelectSingleNode("descendant::d:镇代码", xnm).InnerText;
                string name = node.SelectSingleNode("descendant::d:镇名", xnm).InnerText;
                string id = node.SelectSingleNode("descendant::d:镇级代码", xnm).InnerText;
                var row = regionbll.NewRow();
                row.name = name;
                //row.latlng = latlng;
                row.level = "11";
                row.regionId = id;
                row.pid = "0";
                //row.latlng = "云安镇界2000坐标_GK2_V2014";
                row.latlng = latlng;
                row.center = center;
                row.wallmap = name;
                row.sysname = sysname;

                try
                {
                    regionbll.Add(row);
                }
                catch { continue; }
            }


            //nodes = xml.SelectNodes("//gml:featureMember//d:云安村界2000坐标_GK2_V2014", xnm);

            nodes = xml.SelectNodes("//gml:featureMember//d:YCCJ_ZW2_V2014", xnm);
            foreach (XmlNode node in nodes)
            {

                //string latlng = node.SelectSingleNode("descendant::gml:coordinates", xnm).InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
                string latlng = node.SelectSingleNode("descendant::gml:coordinates", xnm).InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
                string center = getCenterOfGravityPoint(latlng);
                //string name = node.SelectSingleNode("descendant::d:行政区名称", xnm).InnerText;
                //string id = node.SelectSingleNode("descendant::d:行政区代码", xnm).InnerText;
                //string pid = node.SelectSingleNode("descendant::d:镇代码", xnm).InnerText;
                string name = node.SelectSingleNode("descendant::d:行政区名称", xnm).InnerText;
                string id = node.SelectSingleNode("descendant::d:行政区代码", xnm).InnerText;
                string pid = node.SelectSingleNode("descendant::d:镇级代码", xnm).InnerText;
                object ob = regionbll.ExecuteScalar("select name from region where regionid='" + pid + "'");
                string pname = "";
                if (ob != null)
                    pname = ob.ToString();

                var row = regionbll.NewRow();
                row.name = name.Replace("居委会", "社区");
                //row.latlng = latlng;
                row.level = "13";
                row.pid = pid;
                row.center = center;
                //row.latlng = "云安村界2000坐标_GK2_V2014";
                row.latlng = latlng;
                row.regionId = id;
                row.sysname = sysname;
                row.wallmap = pname + row.name;
                try
                {
                    regionbll.Add(row);
                }
                catch { continue; }
            }



        }

        public void ImportRegionLatlngYA()
        {
            addsysname();
            //string filePath = HttpContext.Current.Server.MapPath("~/upload/区划/区划.shp");
            //ADOBaseBLL<Data.regionDataTable, Data.regionRow> regionbll = new ADOBaseBLL<Data.regionDataTable, Data.regionRow>(new DBContext());
            var sysname = regionbll.ExecuteScalar("select top 1 ConfigValue from [config] where configname='地区名'").ToString();
            regionbll.ExecuteNonQuery("delete from region where sysname = (select top 1 ConfigValue from [config] where configname='地区名') or sysname is null");
            XmlDocument xml = new XmlDocument();
            xml.Load(HttpContext.Current.Server.MapPath("~/data/云安界线.xml"));
            //xml.Load(HttpContext.Current.Server.MapPath("~/data/界线.xml"));
            XmlNamespaceManager xnm = new XmlNamespaceManager(xml.NameTable);
            xnm.AddNamespace("gml", "http://www.opengis.net/gml");
            xnm.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xnm.AddNamespace("wfs", "http://www.opengis.net/wfs");
            xnm.AddNamespace("schemaLocation", "http://www.geostar.com.cn/geoglobe http://192.168.1.182:9010/POI_PUB_441781/wfs?service=WFS&amp;version=1.0.0&amp;request=DescribeFeatureType http://www.opengis.net/wfs http://192.168.1.182:9010/POI_PUB_441781/WFS-basic.xsd");
            xnm.AddNamespace("d", "http://www.geostar.com.cn/geoglobe");

            var data = regionbll.GetBySQL("select * from region where sysname = (select top 1 ConfigValue from [config] where configname='地区名')");
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            var nodes = xml.SelectNodes("//gml:featureMember//d:云安镇界2000坐标_GK2_V2014", xnm);
            //var nodes = xml.SelectNodes("//gml:featureMember//d:YCZJ_ZW2_V2014", xnm);
            list = new List<Dictionary<string, string>>();

            foreach (XmlNode node in nodes)
            {

                string latlng = node.SelectSingleNode("descendant::gml:coordinates", xnm).InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
                string center = getCenterOfGravityPoint(latlng);
                string name = node.SelectSingleNode("descendant::d:TOWN", xnm).InnerText;
                string id = node.SelectSingleNode("descendant::d:镇代码", xnm).InnerText;
                //string name = node.SelectSingleNode("descendant::d:镇名", xnm).InnerText;
                //string id = node.SelectSingleNode("descendant::d:镇级代码", xnm).InnerText;
                var row = regionbll.NewRow();
                row.name = name;
                //row.latlng = latlng;
                row.level = "11";
                row.regionId = id;
                row.pid = "0";
                //row.latlng = "云安镇界2000坐标_GK2_V2014";
                row.latlng = latlng;
                row.center = center;
                row.wallmap = name;
                row.sysname = sysname;

                try
                {
                    regionbll.Add(row);
                }
                catch { continue; }
            }


            nodes = xml.SelectNodes("//gml:featureMember//d:云安村界2000坐标_GK2_V2014", xnm);

            //nodes = xml.SelectNodes("//gml:featureMember//d:YCCJ_ZW2_V2014", xnm);
            foreach (XmlNode node in nodes)
            {

                string latlng = node.SelectSingleNode("descendant::gml:coordinates", xnm).InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
                //string latlng = node.SelectSingleNode("descendant::gml:coordinates", xnm).InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
                string center = getCenterOfGravityPoint(latlng);
                string name = node.SelectSingleNode("descendant::d:行政区名称", xnm).InnerText;
                string id = node.SelectSingleNode("descendant::d:行政区代码", xnm).InnerText;
                string pid = node.SelectSingleNode("descendant::d:镇代码", xnm).InnerText;
                //string name = node.SelectSingleNode("descendant::d:行政区名称", xnm).InnerText;
                //string id = node.SelectSingleNode("descendant::d:行政区代码", xnm).InnerText;
                //string pid = node.SelectSingleNode("descendant::d:镇级代码", xnm).InnerText;
                object ob = regionbll.ExecuteScalar("select name from region where regionid='" + pid + "'");
                string pname = "";
                if (ob != null)
                    pname = ob.ToString();

                var row = regionbll.NewRow();
                row.name = name.Replace("居委会", "社区");
                //row.latlng = latlng;
                row.level = "13";
                row.pid = pid;
                row.center = center;
                //row.latlng = "云安村界2000坐标_GK2_V2014";
                row.latlng = latlng;
                row.regionId = id;
                row.sysname = sysname;
                row.wallmap = pname+  row.name;
                try
                {
                    regionbll.Add(row);
                }
                catch { continue; }
            }



        }
        #endregion
        public void ImortShpRegion()
        {
            var sysname = regionbll.ExecuteScalar("select top 1 ConfigValue from [config] where configname='地区名'").ToString();
            regionbll.ExecuteNonQuery("delete from region where sysname = (select top 1 ConfigValue from [config] where configname='地区名')");

            string path = HttpContext.Current.Server.MapPath("~/data/电白行政区/电白行政区镇.shp");
            ShapeFile.MapFilesInMemory = true;
            EGIS.ShapeFileLib.ShapeFile sf = new EGIS.ShapeFileLib.ShapeFile(path);
            
            sf.RenderSettings = new EGIS.ShapeFileLib.RenderSettings(path, "", new Font("宋体", 9, FontStyle.Bold));
  
            Array fieldnames = sf.GetAttributeFieldNames();
            ArrayList al = new ArrayList();
            foreach (var i in fieldnames)
            {
                al.Add(i);
            }
                
            for(int i=0;i<sf.RecordCount;i++)
            {
                string[] values = sf.GetAttributeFieldValues(i);
                var row = regionbll.NewRow();
                row.sysname = sysname;
                row.wallmap = values[al.IndexOf("NAME")].Trim();
                row.regionId = values[al.IndexOf("PAC")].Trim();
                row.name = values[al.IndexOf("NAME")].Trim();
                row.pid = "0";
                row.level = "11";
                var s = sf.GetShapeData(i);
                string latlng = "";
                foreach (var p in s[0])
                {
                    if (latlng != "")
                        latlng += ";";
                    latlng += p.X + "," + p.Y;
                }
                row.latlng = latlng;
                string center = getCenterOfGravityPoint(latlng);
                row.center = center;
                try
                {
                    regionbll.Add(row);
                }
                catch (Exception e){
                    string ei = e.ToString();
                }
            }

            path = HttpContext.Current.Server.MapPath("~/data/电白行政区/电白村级行政区划.shp");
            ShapeFile.MapFilesInMemory = true;
            sf = new EGIS.ShapeFileLib.ShapeFile(path);

            sf.RenderSettings = new EGIS.ShapeFileLib.RenderSettings(path, "", new Font("宋体", 9, FontStyle.Bold));

            fieldnames = sf.GetAttributeFieldNames();
            al = new ArrayList();
            foreach (var i in fieldnames)
            {
                al.Add(i);
            }

            for (int i = 0; i < sf.RecordCount; i++)
            {
                string[] values = sf.GetAttributeFieldValues(i);
                var row = regionbll.NewRow();
                row.sysname = sysname;
                //row.wallmap = values[al.IndexOf("wallmap")].Trim();

                row.regionId = values[al.IndexOf("PAC")].Trim();
                row.name = values[al.IndexOf("NAME")].Trim();
                row.pid = row.regionId.Substring(0,9);
                row.level = "13";
                object ob = regionbll.ExecuteScalar("select name from region where regionid='" + row.pid + "'");
                string pname = "";
                if (ob != null)
                    pname = ob.ToString();
                row.wallmap = pname + row.name;
                var s = sf.GetShapeData(i);
                string latlng = "";
                foreach (var p in s[0])
                {
                    if (latlng != "")
                        latlng += ";";
                    latlng += p.X + "," + p.Y;
                }
                row.latlng = latlng;
                string center = getCenterOfGravityPoint(latlng);
                row.center = center;
                try
                {
                    regionbll.Add(row);
                }
                catch (Exception e)
                {
                    string ei = e.ToString();
                }
            }
        }

        public void ImortShpRegionLW()
        {
            var sysname = regionbll.ExecuteScalar("select top 1 ConfigValue from [config] where configname='地区名'").ToString();
            regionbll.ExecuteNonQuery("delete from region where sysname = (select top 1 ConfigValue from [config] where configname='地区名')");

            string path = HttpContext.Current.Server.MapPath("~/data/荔湾出图界线0904/荔湾界线0904.shp");
            ShapeFile.MapFilesInMemory = true;
            EGIS.ShapeFileLib.ShapeFile sf = new EGIS.ShapeFileLib.ShapeFile(path);

            sf.RenderSettings = new EGIS.ShapeFileLib.RenderSettings(path, "", new Font("宋体", 9, FontStyle.Bold));

            Array fieldnames = sf.GetAttributeFieldNames();
            ArrayList al = new ArrayList();
            foreach (var i in fieldnames)
            {
                al.Add(i);
            }

            for (int i = 0; i < sf.RecordCount; i++)
            {
                string[] values = sf.GetAttributeFieldValues(i);
                var row = regionbll.NewRow();
                row.sysname = sysname;
                if (values[al.IndexOf("Village")].Trim() == "")
                {
                    row.wallmap = values[al.IndexOf("NAME")].Trim();
                    row.regionId = row.wallmap;
                    row.name = values[al.IndexOf("NAME")].Trim();
                    row.pid = "0";
                    row.level = "11";
                    var s = sf.GetShapeData(i);
                    string latlng = "";
                    foreach (var p in s[0])
                    {
                        if (latlng != "")
                            latlng += ";";
                        latlng += p.X + "," + p.Y;
                    }
                    row.latlng = latlng;
                    string center = getCenterOfGravityPoint(latlng);
                    row.center = center;
                    try
                    {
                        regionbll.Add(row);
                    }
                    catch (Exception e)
                    {
                        string ei = e.ToString();
                    }
                }
                else
                {
                    row.wallmap = values[al.IndexOf("Town")].Trim()+ values[al.IndexOf("NAME")].Trim();
                    row.regionId = row.wallmap;
                    row.name = values[al.IndexOf("NAME")].Trim();
                    row.pid = values[al.IndexOf("Town")].Trim();
                    row.level = "13";
                    var s = sf.GetShapeData(i);
                    string latlng = "";
                    foreach (var p in s[0])
                    {
                        if (latlng != "")
                            latlng += ";";
                        latlng += p.X + "," + p.Y;
                    }
                    row.latlng = latlng;
                    string center = getCenterOfGravityPoint(latlng);
                    row.center = center;
                    try
                    {
                        regionbll.Add(row);
                    }
                    catch (Exception e)
                    {
                        string ei = e.ToString();
                    }
                }
            }

            //path = HttpContext.Current.Server.MapPath("~/data/荔湾出图界线0904.shp");
            //ShapeFile.MapFilesInMemory = true;
            //sf = new EGIS.ShapeFileLib.ShapeFile(path);

            //sf.RenderSettings = new EGIS.ShapeFileLib.RenderSettings(path, "", new Font("宋体", 9, FontStyle.Bold));

            //fieldnames = sf.GetAttributeFieldNames();
            //al = new ArrayList();
            //foreach (var i in fieldnames)
            //{
            //    al.Add(i);
            //}

            //for (int i = 0; i < sf.RecordCount; i++)
            //{
            //    string[] values = sf.GetAttributeFieldValues(i);
            //    var row = regionbll.NewRow();
            //    row.sysname = sysname;
            //    //row.wallmap = values[al.IndexOf("wallmap")].Trim();

            //    row.regionId = values[al.IndexOf("PAC")].Trim();
            //    row.name = values[al.IndexOf("NAME")].Trim();
            //    row.pid = row.regionId.Substring(0, 9);
            //    row.level = "13";
            //    object ob = regionbll.ExecuteScalar("select name from region where regionid='" + row.pid + "'");
            //    string pname = "";
            //    if (ob != null)
            //        pname = ob.ToString();
            //    row.wallmap = pname + row.name;
            //    var s = sf.GetShapeData(i);
            //    string latlng = "";
            //    foreach (var p in s[0])
            //    {
            //        if (latlng != "")
            //            latlng += ";";
            //        latlng += p.X + "," + p.Y;
            //    }
            //    row.latlng = latlng;
            //    string center = getCenterOfGravityPoint(latlng);
            //    row.center = center;
            //    try
            //    {
            //        regionbll.Add(row);
            //    }
            //    catch (Exception e)
            //    {
            //        string ei = e.ToString();
            //    }
            //}
        }
        public string getCenterOfGravityPoint(string strPolygen)
        {
            double area = 0.0;//多边形面积  
            double Gx = 0.0, Gy = 0.0;// 重心的x、y  
            string[] xys = strPolygen.Split(';');
            for (int i = 1; i <= strPolygen.Split(';').Length; i++)
            {
                double iLat = double.Parse(xys[i % xys.Length].Split(',')[0]);
                double iLng = double.Parse(xys[i % xys.Length].Split(',')[1]);
                double nextLat = double.Parse(xys[i - 1].Split(',')[0]);
                double nextLng = double.Parse(xys[i - 1].Split(',')[1]);
                double temp = (iLat * nextLng - iLng * nextLat) / 2.0;
                area += temp;
                Gx += temp * (iLat + nextLat) / 3.0;
                Gy += temp * (iLng + nextLng) / 3.0;
            }
            Gx = Gx / area;
            Gy = Gy / area;
            return Gx + "," + Gy;
        }


        //public void wallmapselect()
        //{
        //    string layerName = this.Query<string>("layerName");
        //    string fieldName = this.Query<string>("fieldName");
        //    string name = this.Query < string > ("name").Replace("社区","居委会");
        //    XmlDocument xml = new XmlDocument();
        //    xml.Load(HttpContext.Current.Server.MapPath("~/data/云安界线.xml"));
        //    XmlNamespaceManager xnm = new XmlNamespaceManager(xml.NameTable);
        //    xnm.AddNamespace("gml", "http://www.opengis.net/gml");
        //    xnm.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
        //    xnm.AddNamespace("wfs", "http://www.opengis.net/wfs");
        //    xnm.AddNamespace("schemaLocation", "http://www.geostar.com.cn/geoglobe http://192.168.1.182:9010/POI_PUB_441781/wfs?service=WFS&amp;version=1.0.0&amp;request=DescribeFeatureType http://www.opengis.net/wfs http://192.168.1.182:9010/POI_PUB_441781/WFS-basic.xsd");
        //    xnm.AddNamespace("d", "http://www.geostar.com.cn/geoglobe");

        //    var node = xml.SelectSingleNode("//gml:featureMember//d:"+layerName+"/d:"+fieldName+"[text()='"+name+"']", xnm);
        //    var latlngnode = node.ParentNode.SelectSingleNode("descendant::gml:coordinates",xnm);
        //    Dictionary<string, string> dic = new Dictionary<string, string>();
        //    dic["latlng"] = latlngnode.InnerText.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
        //    jsonResult = JsonHelper.ObjectToJSON(dic); 
        //}
        public void wallmapselect()
        {
            string id = this.Query<string>("id");
            string latlng = regionbll.ExecuteScalar("select latlng from region where regionId = '" + id + "'").ToString() ;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["latlng"] = latlng.Replace("\n", "").Replace("  ", "").Replace(" ", ";").Trim(';');
            jsonResult = JsonHelper.ObjectToJSON(dic);
        }

       
    }
}