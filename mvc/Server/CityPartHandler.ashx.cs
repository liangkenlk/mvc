using System;
using System.Web;
using TY.Core;
using TY.UI.Ajax;
using WorkFlow;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using Web.lib;

namespace Web.Server
{
    /// <summary>
    /// TraceHandler 的摘要说明
    /// </summary>
    public class CityPartHandler : AjaxHandler
    {
        private ADOBaseBLL<Data.CityPartDataTable, Data.CityPartRow> bll = new ADOBaseBLL<Data.CityPartDataTable, Data.CityPartRow>(new DBContext());
        public void GetList()
        {
            addUserId();
            string keyword = Query<string>("keyword");
            string type = Query<string>("type");
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string sql = "select * from citypart";
            string where = " where 1=1";
            if (UserAuth.User.UserType != "管理员")
                where = " where UserId="+UserAuth.UserID;
            if (keyword != null)
                where += " and name like '%" + keyword + "%'";
            if(type!=null)
                where += " and type = '" + type + "'";
            jsonResult = bll.GetPagedDataTable(sql+where, pageSize, pageindex).ToPagedJson();
        }

        public void addUserId()
        {
            var t = bll.GetNormalDataTable("select * from [CityPart]");
            if (!t.Columns.Contains("UserId"))
                this.bll.ExecuteNonQuery("alter table [CityPart] add UserId int");
        }

        public void GetTypeList()
        {
            string sql = "select distinct type from citypart";
            
            jsonResult = JsonHelper.DataTableToJSON(bll.GetNormalDataTable(sql));
        }



        [Description("保存 参数详见数据库，id为guid，不填或新建都视为新增")]
        public void Update()
        {
            string id = Query<string>("id");
            var row = bll.GetByKey(id);
            if (row == null)
            {
                row = bll.NewRow();
                row.id = id;
                row.UserId = UserAuth.UserID;
                bll.Add(row);

            }
            this.FillObject<Data.CityPartRow>(row);
            bll.Update(row);
            jsonResult = JsonHelper.OutResult(true, "ok");

        }

        public void DeleteSelect()
        {
            string selected = this.Query<string>("selected");
            this.bll.ExecuteNonQuery("delete  from  [citypart] where id in (" + selected + ")");
            jsonResult = JsonHelper.OutResult(true, "ok");
        }

        public void ExportPointExcel()
        {
            string keyword = Query<string>("keyword");
            string type = Query<string>("type");
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string sql = "select * from citypart";
            string where = " where 1=1";
            if (UserAuth.User.UserType != "管理员")
                where = " where UserId=" + UserAuth.UserID;
            if (keyword != null)
                where += " and name like '%" + keyword + "%'";
            if (type != null)
                where += " and type = '" + type + "'";
            var table = bll.GetBySQL(sql);

            string filename = "城市部件数据" + DateTime.Now.Ticks + ".xls";
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic["name"] = "名称";
            dic["type"] = "类型";
            dic["latlng"] = "位置";
            dic["json"] = "属性";
            //dic["dX"] = "检测地址";
            //dic["dY"] = "检测时间";
            //dic["dS"] = "检测单位地址";


            //var templatePath = HttpContext.Current.Server.MapPath("~/temp/pointTemplate.xls");
            new ExcelTool(dic).GridToExcelByNPOI(table, HttpContext.Current.Server.MapPath("~/temp/" + filename), "", 2, 0);

            jsonResult = JsonHelper.OutResult(true, filename);

        }

    }
}