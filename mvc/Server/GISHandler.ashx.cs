using System;
using System.Collections.Generic;
using System.Web;
using TY.Core;
using TY.UI.Ajax;
using WorkFlow;
using Web.lib;
using System.Data;
using System.ComponentModel;
using System.IO;
using CLeopardZip;
using System.Net;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using EGIS.ShapeFileLib;

namespace Web.Server
{
    /// <summary>
    /// GISHandler 的摘要说明
    /// </summary>
    public class GISHandler : AjaxHandler
    {
        private ADOBaseBLL<Data.GeoDataTable, Data.GeoRow> geobll = new ADOBaseBLL<Data.GeoDataTable, Data.GeoRow>(new DBContext());
        [Description("普通查询：参数 keyword地名关键字,page页面（从1开始）,rows页大小,town所属县街、道（空就是全区）")]
        public void AddQuery()
        {

            string keyword = Query<string>("keyword");
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string town = this.Query<string>("town");
            string type = this.Query<string>("type");
            //GIS gis = new GIS();
            Web.lib.EGIS gis = new Web.lib.EGIS();
            int total = 0;
            DataTable table = gis.Query(keyword, town, pageindex, pageSize, out total, false, type);
            PagedData pd = new PagedData();
            pd.Total = total;

            pd.Data = JsonHelper.DataTableToList(table);
            jsonResult = pd.ToPagedJson();
        }
        [Description("普通查询：参数 ring空间查询的面经纬度(\"121,22;123,22;121,22\"),page页面（从1开始）,rows页大小")]
        public void SpatialQuery()
        {
            string ring = Query<string>("ring");
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            //GIS gis = new GIS();
            Web.lib.EGIS gis = new Web.lib.EGIS();
            int total = 0;
            DataTable table = gis.Query(ring, null, pageindex, pageSize, out total, true, null);
            PagedData pd = new PagedData();
            pd.Total = total;

            pd.Data = JsonHelper.DataTableToList(table);
            jsonResult = pd.ToPagedJson();
        }

        public void GetLayers()
        {
            GIS gis = new GIS();
            var re = gis.GetLayers();
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

            foreach (string name in re)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("id", name);
                dic.Add("name", name);
                list.Add(dic);
            }

            jsonResult = JsonHelper.ObjectToJSON(list);
        }
    }
}