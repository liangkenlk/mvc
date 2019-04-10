namespace WorkFlow
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class DBInfo
    {
        public List<string> Tables;

        public List<ColInfo> GetColsInfo(string TableName)
        {
            List<ColInfo> list = new List<ColInfo>();
            string sql = "SELECT\r\n            表名=objs.name ,\r\n            字段名=cols.name,\r\n            字段说明=props.[value]\r\n            FROM syscolumns cols\r\n            inner join sysobjects objs on cols.id= objs.id and  objs.xtype='U' \r\n            left join sys.extended_properties props on cols.id=props.major_id and cols.colid=props.minor_id\r\n            where  objs.name='" + TableName + "'";
            DataTable dataTable = new ADOdal<DataTable, DataRow>().GetDataTable(sql);
            foreach (DataRow row in dataTable.Rows)
            {
                ColInfo item = new ColInfo {
                    name = row["字段名"].ToString(),
                    TableName = row["表名"].ToString(),
                    Description = row["字段说明"].ToString()
                };
                list.Add(item);
            }
            return list;
        }

        public List<ColInfo> GetSqlColInfo(string sql)
        {
            DataTable dataTable = new ADOdal<DataTable, DataRow>().GetDataTable(sql);
            return null;
        }
    }
}

