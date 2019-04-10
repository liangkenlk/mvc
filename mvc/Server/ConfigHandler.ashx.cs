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
using TY.Core;
using TY.UI.Ajax;
using WorkFlow;

namespace Web.Server
{
    /// <summary>
    /// ConfigHandler 的摘要说明
    /// </summary>
    public class ConfigHandler : AjaxHandler
    {
        private IBaseBLL<Data.BaseMapDataTable, Data.BaseMapRow> mapbll =DBFactory<Data.BaseMapDataTable, Data.BaseMapRow>.GetBLL();
        private IBaseBLL<Data.themeDataTable, Data.themeRow> themebll =  DBFactory<Data.themeDataTable, Data.themeRow>.GetBLL();
        private IBaseBLL<Data.regionDataTable, Data.regionRow> regionbll =  DBFactory<Data.regionDataTable, Data.regionRow>.GetBLL();
        private IBaseBLL<Data.ConfigDataTable, Data.ConfigRow> configbll =  DBFactory<Data.ConfigDataTable, Data.ConfigRow>.GetBLL();

        //删除选中的配置
        public void DeleteMapConfigSelect()
        {
            string selected = this.Query<string>("selected");
            this.mapbll.ExecuteNonQuery("delete  from  [BaseMap] where mapId in (" + selected + ")");
            jsonResult = JsonHelper.OutResult(true, "ok");
        }

        public void DeleteThemeConfigSelect()
        {
            string selected = this.Query<string>("selected");
            this.themebll.ExecuteNonQuery("delete  from  [theme] where themeId in (" + selected + ")");
            jsonResult = JsonHelper.OutResult(true, "ok");
        }

        public void DeleteRegionConfigSelect()
        {
            string selected = this.Query<string>("selected");
            this.regionbll.ExecuteNonQuery("delete  from  [region] where regionId in (" + selected + ")");
            jsonResult = JsonHelper.OutResult(true, "ok");
        }

        public void DeleteConfigSelect()
        {
            string selected = this.Query<string>("selected");
            this.configbll.ExecuteNonQuery("delete  from  [Config] where ConfigId in (" + selected + ")");
            jsonResult = JsonHelper.OutResult(true, "ok");
        }


        //修改分类字段
        public void UpdateShowFields()
        {
            string themeId = this.Query<string>("themeId");
            string classTxt = this.Query<string>("classTxt").Trim(';') ;
            this.themebll.ExecuteNonQuery("update [theme] set wfs_showfields='"+classTxt+"' where themeId='"+ themeId+"'");
            jsonResult = JsonHelper.OutResult(true, "保存成功！");
        }

        //修改分类字段
        public void UpdateDetailFields()
        {
            string themeId = this.Query<string>("themeId");
            string detailTxt = this.Query<string>("detailTxt").Trim(';');
            this.themebll.ExecuteNonQuery("update [theme] set wfs_searchfields='" + detailTxt + "' where themeId='" + themeId+"'");
            jsonResult = JsonHelper.OutResult(true, "保存成功！");
        }

        //获取配置列表
        public void GetMapConfigList()
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string layername = this.Query<string>("layername");
            string sql = "select * from [BaseMap]";
            if (layername != null)
                sql += " where layername like '%" + layername + "%'";
            // var list = userbll.GetBySQL(sql);
            sql += " order by idx";
            jsonResult = mapbll.GetPagedDataTable(sql, pageSize, pageindex).ToPagedJson();
        }

        public void GetThemeConfigList()
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string name = this.Query<string>("name");
            string sql = "select * from [theme]";
            if (name != null)
                sql += " where name like '%" + name + "%'";
            // var list = userbll.GetBySQL(sql);
            sql += " order by idx";
            jsonResult = themebll.GetPagedDataTable(sql, pageSize, pageindex).ToPagedJson();
        }

        public void GetRegionConfigList()
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string name = this.Query<string>("name");
            string sql = "select * from [region] where sysname = (select top 1 ConfigValue from [config] where configname='地区名')";
            if (regionbll.GetType().ToString().ToLower().Contains("mysql"))
                sql = "select * from [region] where sysname = (select ConfigValue from [config] where configname='地区名' limit 1)";
            if (name != null)
                sql += " and name like '%" + name + "%'";
            // var list = userbll.GetBySQL(sql);
            sql += " order by pid";
            jsonResult = regionbll.GetPagedDataTable(sql, pageSize, pageindex).ToPagedJson();
        }

        public void GetConfigList()
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string configName = this.Query<string>("configName");
            string sql = "select * from [Config]";
            if (configName != null)
                sql += " where configName like '%" + configName + "%'";
            // var list = userbll.GetBySQL(sql);
            jsonResult = configbll.GetPagedDataTable(sql, pageSize, pageindex).ToPagedJson();
        }

        public void GetMapConfigListForDropDonw()
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string layername = this.Query<string>("layername");
            string sql = "select * from [BaseMap]";
            if (layername != null)
                sql += " where layername like '%" + layername + "%'";
            var list = mapbll.GetBySQL(sql);
            var nullrow = list.NewBaseMapRow();

            nullrow.layername = "全部";
            list.Rows.InsertAt(nullrow, 0);
            jsonResult = JsonHelper.DataTableToJSON(list);
        }

        public void GetThemeConfigListForDropDonw()
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string name = this.Query<string>("name");
            string sql = "select * from [theme]";
            if (name != null)
                sql += " where name like '%" + name + "%'";
            var list = themebll.GetBySQL(sql);
            var nullrow = list.NewthemeRow();

            nullrow.name = "全部";
            list.Rows.InsertAt(nullrow, 0);
            jsonResult = JsonHelper.DataTableToJSON(list);
        }

        public void GetRegionConfigListForDropDonw()
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string name = this.Query<string>("name");
            string sql = "select * from [region]";
            if (name != null)
                sql += " where name like '%" + name + "%'";
            var list = regionbll.GetBySQL(sql);
            var nullrow = list.NewregionRow();

            nullrow.name = "全部";
            list.Rows.InsertAt(nullrow, 0);
            jsonResult = JsonHelper.DataTableToJSON(list);
        }

        public void GetAllRegion()
        {
            if (regionbll.GetType().ToString().ToLower().Contains("mysql"))
                jsonResult = JsonHelper.DataTableToJSON(regionbll.GetNormalDataTable("SELECT    regionId, name, [level], center, pid, wallmap, sysname FROM region where sysname = (select  ConfigValue from [config] where configname='地区名' limit 1)"));
            else
            jsonResult = JsonHelper.DataTableToJSON(regionbll.GetNormalDataTable("SELECT    regionId, name, [level], center, pid, wallmap, sysname FROM region where sysname = (select top 1 ConfigValue from [config] where configname='地区名')"));
        }

        public void GetTreeRegion()
        {
            jsonResult = JsonHelper.DataTableToJSON(regionbll.GetNormalDataTable("SELECT    regionId, name, [level], center, pid, wallmap, sysname FROM region"));
        }

        public void GetConfigListForDropDonw()
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string configName = this.Query<string>("configName");
            string sql = "select * from [BaseMap]";
            if (configName != null)
                sql += " where ConfigName like '%" + configName + "%'";
            var list = configbll.GetBySQL(sql);
            var nullrow = list.NewConfigRow();

            nullrow.ConfigName = "全部";
            list.Rows.InsertAt(nullrow, 0);
            jsonResult = JsonHelper.DataTableToJSON(list);
        }

        //删除配置
        public void DeleteMapConfig()
        {
            try
            {
                this.mapbll.ExecuteNonQuery("delete from [BaseMap] where mapId='" + base.Query<string>("mapId") + "'");
                base.jsonResult = JsonHelper.OutResult(true, "删除成功");
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError("删除失败：" + exception.Message);
            }
        }

        public void DeleteThemeConfig()
        {
            try
            {
                this.themebll.ExecuteNonQuery("delete from [theme] where themeId='" + base.Query<string>("themeId") + "'");
                base.jsonResult = JsonHelper.OutResult(true, "删除成功");
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError("删除失败：" + exception.Message);
            }
        }

        public void DeleteRegionConfig()
        {
            try
            {
                this.regionbll.ExecuteNonQuery("delete from [region] where regionId='" + base.Query<string>("regionId") + "'");
                base.jsonResult = JsonHelper.OutResult(true, "删除成功");
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError("删除失败：" + exception.Message);
            }
        }

        public void DeleteConfig()
        {
            try
            {
                this.configbll.ExecuteNonQuery("delete from [Config] where ConfigId='" + base.Query<string>("configId") + "'");
                base.jsonResult = JsonHelper.OutResult(true, "删除成功");
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError("删除失败：" + exception.Message);
            }
        }

        //获取配置信息
        public void GetMapConfigInfo()
        {
            try
            {
                string mapId = base.Query<string>("mapId");
                string sql = "select * from [BaseMap] where mapId='" + mapId + "'";
                Data.BaseMapDataTable bySQL = this.mapbll.GetBySQL(sql);
                base.jsonResult = JsonHelper.ObjectToJSON(JsonHelper.DataTableToList(bySQL)[0]);
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError(exception.Message);
            }
        }

        public void GetThemeConfigInfo()
        {
            try
            {
                string themeId = base.Query<string>("themeId");
                string sql = "select * from [theme] where themeId='" + themeId + "'";
                Data.themeDataTable bySQL = this.themebll.GetBySQL(sql);
                base.jsonResult = JsonHelper.ObjectToJSON(JsonHelper.DataTableToList(bySQL)[0]);
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError(exception.Message);
            }
        }

        public void GetRegionConfigInfo()
        {
            try
            {
                string regionId = base.Query<string>("regionId");
                string sql = "select * from [region] where regionId='" + regionId + "'";
                Data.regionDataTable bySQL = this.regionbll.GetBySQL(sql);
                base.jsonResult = JsonHelper.ObjectToJSON(JsonHelper.DataTableToList(bySQL)[0]);
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError(exception.Message);
            }
        }

        public void GetConfigInfo()
        {
            try
            {
                string configId = base.Query<string>("configId");
                string sql = "select * from [Config] where ConfigId='" + configId + "'";
                Data.ConfigDataTable bySQL = this.configbll.GetBySQL(sql);
                base.jsonResult = JsonHelper.ObjectToJSON(JsonHelper.DataTableToList(bySQL)[0]);
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError(exception.Message);
            }
        }

        //保存配置
        public void SaveMapConfig()
        {
            try
            {
                Data.BaseMapRow data;
                if (this.Query<string>("mapId") != null)
                    data = this.mapbll.GetByKey(this.Query<string>("mapId"));
                else
                    data = this.mapbll.NewRow();
                
              
       
                if (this.Query<string>("mapId") == null)
                {
                    this.FillObject<Data.BaseMapRow>(data);
                    if (this.mapbll.GetBySQL("select * from [BaseMap] where layername='" + data.layername + "'").Count > 0)

                    {
                        jsonResult = JsonHelper.OutResult(false, "地图配置已存在！");
                        return;
                    }


                }

                this.FillObject<Data.BaseMapRow>(data);
                if (this.Query<string>("mapId") != null)
                {

                    mapbll.Update(data);
                }

                else
                {
                    data.mapId = Guid.NewGuid().ToString();
                    mapbll.Add(data);
                }
               
                jsonResult = JsonHelper.OutResult(true, data.mapId);
            }
            catch (Exception e)
            {
                base.jsonResult = JsonHelper.OutError(e.Message);
            }

        }

        public void SaveThemeConfig()
        {
            try
            {
                Data.themeRow data;
                if (this.Query<string>("themeId") != null)
                    data = this.themebll.GetByKey(this.Query<string>("themeId"));
                else
                    data = this.themebll.NewRow();

                data.name = this.Query<string>("name");
                data.idx = this.Query<int>("idx");
                if (this.Query<string>("themeId") == null)
                {

                    if (this.mapbll.GetBySQL("select * from [theme] where name='" + data.name + "'").Count > 0)

                    {
                        jsonResult = JsonHelper.OutResult(false, "专题配置已存在！");
                        return;
                    }
                    
                }

                this.FillObject<Data.themeRow>(data);
                if (this.Query<string>("themeId") != null)
                {
                    
                    themebll.Update(data);
                }

                else
                {
                    data.themeId = Guid.NewGuid().ToString();
                    themebll.Add(data);
                }
                //var a = this.themebll.GetBySQL("select * from [theme] where name='" + data.name + "'")[0];
                jsonResult = JsonHelper.OutResult(true, data.themeId.ToString());
            }
            catch (Exception e)
            {
                base.jsonResult = JsonHelper.OutError(e.Message);
            }

        }

        public void SaveRegionConfig()
        {
            try
            {
                Data.regionRow data;
                if (this.Query<string>("regionId") != null)
                    data = this.regionbll.GetByKey(this.Query<string>("regionId"));
                else
                    data = this.regionbll.NewRow();

                this.FillObject<Data.regionRow>(data);
                if (this.Query<string>("regionId") == null)
                {

                    if (this.regionbll.GetBySQL("select * from [region] where name='" + data.name + "'").Count > 0)

                    {
                        jsonResult = JsonHelper.OutResult(false, "区域配置已存在！");
                        return;
                    }
                    
                }


                if (this.Query<string>("regionId") != null)
                {

                    regionbll.Update(data);
                }

                else
                {
                    data.regionId = Guid.NewGuid().ToString();
                    regionbll.Add(data);

                }
                jsonResult = JsonHelper.OutResult(true, data.regionId.ToString());
            }
            catch (Exception e)
            {
                base.jsonResult = JsonHelper.OutError(e.Message);
            }

        }

        public void SaveConfig()
        {
            try
            {
                Data.ConfigRow data;
                if (this.Query<string>("ConfigId") != null)
                    data = this.configbll.GetByKey(this.Query<string>("ConfigId"));
                else
                    data = this.configbll.NewRow();


                this.FillObject<Data.ConfigRow>(data);
                if (this.Query<string>("ConfigId") == null)
                {

                    if (this.configbll.GetBySQL("select * from [Config] where ConfigName='" + data.ConfigName + "'").Count > 0)

                    {
                        jsonResult = JsonHelper.OutResult(false, "系统配置已存在！");
                        return;
                    }
                
                }


                if (this.Query<string>("ConfigId") != null)
                {


                    configbll.Update(data);
                }

                else
                {
                    data.ConfigId = Guid.NewGuid().ToString();
                    configbll.Add(data);
                }
                this.SetsysConfigJSON();
               jsonResult = JsonHelper.OutResult(true, data.ConfigId.ToString());
            }
            catch (Exception e)
            {
                base.jsonResult = JsonHelper.OutError(e.Message);
            }

        }

        public void UpdateMapConfig()
        {
            Data.BaseMapDataTable bySQL = this.mapbll.GetBySQL("select * from [BaseMap] where layername='" + base.Query<string>("layername") + "'");
            try
            {
                Data.BaseMapRow row = bySQL.Rows[0] as Data.BaseMapRow;
                if (base.Query<string>("mapId") != null)
                {
                    row.mapId = base.Query<string>("mapId");
                }
                if (base.Query<string>("layername") != null)
                {
                    row.layername = base.Query<string>("layername");
                }
                if (base.Query<string>("type") != null)
                {
                    row.type = base.Query<string>("type");
                }
                //row.RegTime = DateTime.Now;
                if (base.Query<string>("url") != null)
                {
                    row.url = base.Query<string>("url");
                }
                //row.IsOnline = true;
                if (base.Query<string>("layeridfilter") != null)
                {
                    row.layeridfilter = base.Query<string>("layeridfilter");
                }
                // row.IsParkDriver = false;
                if (base.Query<string>("style") != null)
                {
                    row.style = base.Query<string>("style");
                }
                if (base.Query<string>("format") != null)
                {
                    row.format = base.Query<string>("format");
                }
                if (base.Query<string>("tileMatrixSet") != null)
                {
                    row.tileMatrixSet = base.Query<string>("tileMatrixSet");
                }
                if (base.Query<string>("levels") != null)
                {
                    row.levels = base.Query<string>("levels");
                }
                if (base.Query<string>("offline_url_android") != null)
                {
                    row.offline_url_android = base.Query<string>("offline_url_android");
                }
                if (base.Query<int>("idx") != 0)
                {
                    row.idx = base.Query<int>("idx");
                }
                this.mapbll.Update(row);
                base.jsonResult = JsonHelper.OutResult(true, "更新成功");
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError(exception.Message);
            }
        }

        public void UpdateThemeConfig()
        {
            Data.themeDataTable bySQL = this.themebll.GetBySQL("select * from [theme] where name='" + base.Query<string>("name") + "'");
            try
            {
                Data.themeRow row = bySQL.Rows[0] as Data.themeRow;
                if (base.Query<string>("themeId") != null)
                {
                    row.themeId = base.Query<string>("themeId");
                }
                if (base.Query<string>("name") != null)
                {
                    row.name = base.Query<string>("name");
                }
                if (base.Query<string>("groupname") != null)
                {
                    row.groupname = base.Query<string>("groupname");
                }
                //row.RegTime = DateTime.Now;
                if (base.Query<string>("wfs_layer") != null)
                {
                    row.wfs_layer = base.Query<string>("wfs_layer");
                }
                //row.IsOnline = true;
                if (base.Query<string>("wfs_searchfields") != null)
                {
                    row.wfs_searchfields = base.Query<string>("wfs_searchfields");
                }
                // row.IsParkDriver = false;
                if (base.Query<string>("wfs_showfields") != null)
                {
                    row.wfs_showfields = base.Query<string>("wfs_showfields");
                }
                if (base.Query<string>("wfs_url") != null)
                {
                    row.wfs_url = base.Query<string>("wfs_url");
                }
                if (base.Query<string>("wmts_format") != null)
                {
                    row.wmts_format = base.Query<string>("wmts_format");
                }
                if (base.Query<string>("wmts_layer") != null)
                {
                    row.wmts_layer = base.Query<string>("wmts_layer");
                }
                if (base.Query<string>("wmts_levels") != null)
                {
                    row.wmts_levels = base.Query<string>("wmts_levels");
                }
                if (base.Query<string>("wmts_style") != null)
                {
                    row.wmts_style = base.Query<string>("wmts_style");
                }
                if (base.Query<string>("wmts_tileMatrixSet") != null)
                {
                    row.wmts_tileMatrixSet = base.Query<string>("wmts_tileMatrixSet");
                }
                if (base.Query<string>("wmts_url") != null)
                {
                    row.wmts_url = base.Query<string>("wmts_url");
                }
                if (base.Query<int>("idx") != 0)
                {
                    row.idx = base.Query<int>("idx");
                }
                this.themebll.Update(row);
                base.jsonResult = JsonHelper.OutResult(true, "更新成功");
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError(exception.Message);
            }
        }

        public void UpdateRegionConfig()
        {
            Data.regionDataTable bySQL = this.regionbll.GetBySQL("select * from [region] where name='" + base.Query<string>("name") + "'");
            try
            {
                Data.regionRow row = bySQL.Rows[0] as Data.regionRow;
                if (base.Query<string>("regionId") != null)
                {
                    row.regionId = base.Query<string>("regionId");
                }
                if (base.Query<string>("name") != null)
                {
                    row.name = base.Query<string>("name");
                }
                if (base.Query<string>("latlng") != null)
                {
                    row.latlng = base.Query<string>("latlng");
                }
                //row.RegTime = DateTime.Now;
                if (base.Query<string>("level") != null)
                {
                    row.level = base.Query<string>("level");
                }

                this.regionbll.Update(row);
                base.jsonResult = JsonHelper.OutResult(true, "更新成功");
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError(exception.Message);
            }
        }

        public void UpdateConfig()
        {
            Data.ConfigDataTable bySQL = this.configbll.GetBySQL("select * from [Config] where ConfigName='" + base.Query<string>("configName") + "'");
            try
            {
                Data.ConfigRow row = bySQL.Rows[0] as Data.ConfigRow;
                if (base.Query<string>("ConfigId") != null)
                {
                    row.ConfigId = base.Query<string>("ConfigId");
                }
                if (base.Query<string>("ConfigName") != null)
                {
                    row.ConfigName = base.Query<string>("ConfigName");
                }
                if (base.Query<string>("ConfigKey") != null)
                {
                    row.ConfigKey = base.Query<string>("ConfigKey");
                }
                //row.RegTime = DateTime.Now;
                if (base.Query<string>("ConfigValue") != null)
                {
                    row.ConfigValue = base.Query<string>("ConfigValue");
                }

                this.configbll.Update(row);
                SetsysConfigJSON();
                base.jsonResult = JsonHelper.OutResult(true, "更新成功");
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError(exception.Message);
            }
        }

        public void GetSysConfig()
        {


            string sql = "select * from [Config]";
;
             var list = this.configbll.GetBySQL(sql);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var row in list)
            {
                dic[row.ConfigName] = row.ConfigValue;
            }
            jsonResult = JsonHelper.ObjectToJSON(dic);
        }

        public void SetsysConfigJSON()
        {
            var table = configbll.GetBySQL("select * from [config]");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var row in table)
            {
                dic[row.ConfigName] = row.ConfigValue;
            }
            dic["username"] = UserAuth.User.UserName;
            string sysConfig = "var sysConfig ="+ JsonHelper.ObjectToJSON(dic);


            File.WriteAllText(HttpContext.Current.Server.MapPath("~/js/sysConfig.js"),sysConfig);
        }

        public void getVersion()
        {
            string sql = "select configvalue from [Config] where configname='版本号'";
            object ob = configbll.ExecuteScalar(sql);
            if (ob != null)
                jsonResult = "{\"data\":" + ob.ToString() + "}";
        }

    }
}