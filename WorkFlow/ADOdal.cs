namespace WorkFlow
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using TY.Core;
    

    public class ADOdal<T, R> : Idal<T, R> where T: DataTable where R: DataRow
    {
        
        private SqlDataAdapter adp;
        private SqlCommand cmd;
        private SqlConnection con;
        private DBContext db;
        public static DataTable dbInfoTable;
        public string PrimaryKeyName;
        public string TableName;

        public ADOdal()
        {
            T table = Activator.CreateInstance<T>();
            this.TableName = table.TableName;
            this.con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
            this.cmd = new SqlCommand("select * from [" + this.TableName + "]", this.con);
            this.adp = new SqlDataAdapter(this.cmd);
            if (table.PrimaryKey.Count()>0)
            PrimaryKeyName = table.PrimaryKey[0].ColumnName;
            //this. = this.GetKey(null);
        }

        public ADOdal(T table)
        {
            this.TableName = table.TableName;
            this.con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
            this.cmd = new SqlCommand("select * from [" + this.TableName + "]", this.con);
            this.adp = new SqlDataAdapter(this.cmd);
            if (table.PrimaryKey.Count() > 0)
                this.PrimaryKeyName = table.PrimaryKey[0].ColumnName;
        }

        public ADOdal(DBContext context)
        {
            T table = Activator.CreateInstance<T>();
            this.db = context;
            this.TableName = Activator.CreateInstance<T>().TableName;
            this.con = context.con;
            this.cmd = context.cmd;
            this.cmd.CommandText = "select * from [" + this.TableName + "]";
            this.adp = new SqlDataAdapter(this.cmd);
            if (table.PrimaryKey.Count() > 0)
                PrimaryKeyName = table.PrimaryKey[0].ColumnName;
        }

        internal PagedData GetNews(string sql)
        {
            throw new NotImplementedException();
        }

        public void Add(R row)
        {
            Type type = typeof(T);
            Type type2 = typeof(R);
            T table = row.Table as T;
            type.GetMethods().FirstOrDefault<MethodInfo>(p => (p.Name == ("Add" + table.TableName + "Row"))).Invoke(table, new object[] { row });
            this.Update(table);
        }

        public void Delete(string ids)
        {
            this.db.Open();
            this.cmd.CommandText = "Delete From [" + this.TableName + "] where " + this.PrimaryKeyName + " in (" + ids + ")";
            this.cmd.ExecuteNonQuery();
            this.db.End();
        }

        public void Delete(T table)
        {
            this.db.Open();
            foreach (DataRow row in table.Rows)
            {
                row.Delete();
            }
            SqlCommandBuilder builder = new SqlCommandBuilder(this.adp);
            this.adp.Update(table);
            this.db.End();
        }

        public void DeleteAll()
        {
            this.db.Open();
            this.cmd.CommandText = "delete from [" + this.TableName + "]";
            this.cmd.ExecuteNonQuery();
            this.db.End();
        }

        public void DeleteAll(string where)
        {
            this.db.Open();
            this.cmd.CommandText = "delete from [" + this.TableName + "] " + where;
            this.cmd.ExecuteNonQuery();
            this.db.End();
        }

        public void ExecuteNonQuery(string sql)
        {
            this.db.Open();
            this.db.cmd.CommandText = sql;
            this.db.cmd.ExecuteNonQuery();
            this.db.End();
        }

        public object ExecuteScalar(string sql)
        {
            this.db.Open();
            this.cmd.CommandText = sql;
            object obj2 = this.cmd.ExecuteScalar();
            this.db.End();
            return obj2;
        }

        public T GetByFilter(string sql, string filter)
        {
            this.db.Open();
            this.adp.SelectCommand.CommandText = sql + (!string.IsNullOrEmpty(filter) ? " where " : "") + filter;
            T dataTable = Activator.CreateInstance<T>();
            this.adp.Fill(dataTable);
            this.db.End();
            return dataTable;
        }

        public T GetByids(string ids)
        {
            this.db.Open();
            T dataTable = Activator.CreateInstance<T>();
            string columnName = dataTable.PrimaryKey[0].ColumnName;
            this.cmd.CommandText = "select * from [" + this.TableName + "] where " + columnName + " in (" + ids + ")";
            this.adp.Fill(dataTable);
            this.db.End();
            return dataTable;
        }

        public R GetByKey(object key)
        {
            if (key == null)
            {
                return default(R);
            }
            this.db.Open();
            T dataTable = Activator.CreateInstance<T>();
            string columnName = dataTable.PrimaryKey[0].ColumnName;
            this.cmd.CommandText = "select * from [" + this.TableName + "] where " + columnName + " = '" + key.ToString() + "'";
            this.adp.Fill(dataTable);
            this.db.End();
            if (dataTable.Rows.Count == 0)
            {
                return default(R);
            }
            return (dataTable.Rows[0] as R);
        }

        public T GetBySQL(string sql)
        {
            this.db.Open();
            this.adp.SelectCommand.CommandText = sql;
            T dataTable = Activator.CreateInstance<T>();
            this.adp.Fill(dataTable);
            this.db.End();
            return dataTable;
        }

        public T GetDataTable(string sql)
        {
            this.db.Open();
            this.adp.SelectCommand.CommandText = sql;
            T dataTable = Activator.CreateInstance<T>();
            this.adp.Fill(dataTable);
            this.db.End();
            return dataTable;
        }

        public string GetKey(string TableName = null)
        {

            //string tableName;
            //if (TableName == null)
            //{
            //    tableName = this.TableName;
            //}
            //else
            //{
            //    tableName = TableName;
            //}
            //string str2 = "select \r\n    [表名]=c.Name,\r\n    [表说明]=isnull(f.[value],''),\r\n    [列名]=a.Name,\r\n    [列序号]=a.Column_id,\r\n    [标识]=case when is_identity=1 then '1' else '' end,\r\n    [主键]=case when exists(select 1 from sys.objects where parent_object_id=a.object_id and type=N'PK' and name in\r\n                    (select Name from sys.indexes where index_id in\r\n                    (select indid from sysindexkeys where  colid=a.column_id)))\r\n                    then '1' else '' end,\r\n    [类型]=b.Name,\r\n    [字符数]=case when a.[max_length]=-1 and b.Name!='xml' then 'max/2G' \r\n            when b.Name='xml' then ' 2^31-1字節/2G'\r\n            else rtrim(a.[max_length]) end,\r\n    [长度]=ColumnProperty(a.object_id,a.Name,'Precision'),\r\n    [小时]=isnull(ColumnProperty(a.object_id,a.Name,'Scale'),0),\r\n    [可否为空]=case when a.is_nullable=1 then '1' else '' end,\r\n    [列说明]=isnull(e.[value],''),\r\n    [默认值]=isnull(d.text,'')    \r\nfrom \r\n    sys.columns a\r\nleft join\r\n    sys.types b on a.user_type_id=b.user_type_id\r\ninner join\r\n    sys.objects c on a.object_id=c.object_id and c.Type='U'\r\nleft join\r\n    syscomments d on a.default_object_id=d.ID\r\nleft join\r\n    sys.extended_properties e on e.major_id=c.object_id and e.minor_id=a.Column_id and e.class=1 \r\nleft join\r\n    sys.extended_properties f on f.major_id=c.object_id and f.minor_id=0 and f.class=1";
            //string str3 = this.ExecuteScalar(str2).ToString();

            //return str3;
            return PrimaryKeyName;
        }

        public DataTable GetNormalDataTable(string sql)
        {
            DataTable dataTable = new DataTable();
            this.db.Open();
            this.adp.SelectCommand.CommandText = sql;
            this.adp.Fill(dataTable);
            this.db.End();
            return dataTable;
        }

        public  PagedData GetPagedDataTable(string sql, int PageSize, int PageIndex)
        {
            this.db.Open();

            var a = sql.ToLower().IndexOf("order by");
            string countsql = "";
            if (a == -1)
                countsql = sql;
            else
                countsql = sql.Substring(0, a);
            int count = int.Parse(this.ExecuteScalar("select count(*) from (" + countsql + ") a").ToString());

            this.adp.SelectCommand.CommandText = sql;

            DataTable dataTable = new DataTable();
            this.adp.Fill(dataTable);

            if (PageSize == 0)
            {
                PageSize = count;
                PageIndex = 0;
            }

            DataTable local = new DataTable();
            this.adp.Fill(PageSize * PageIndex, PageSize,new DataTable[] { local });
       
            this.db.End();
            PagedData pd = new PagedData();
            pd.PageIndex = PageIndex;
            pd.PageSize = PageSize;
            pd.total = count;
            //pd.table = local;
            pd.rows = JsonHelper.DataTableToList( local);
            
            return pd;
        }

        public R NewRow()
        {
            Type type = typeof(T);
            Type type2 = typeof(R);
            T local = Activator.CreateInstance<T>();
            return (type.GetMethod("New" + local.TableName + "Row").Invoke(local, null) as R);
        }

        public void Update(R row)
        {
            this.Update(row.Table as T);
        }

        public void Update(T table)
        {
            this.db.Open();
            SqlCommandBuilder builder = new SqlCommandBuilder(this.adp);
            this.adp.SelectCommand.CommandText = "select * from [" + this.TableName + "]";
            this.adp.Update(table);
            this.db.End();
        }


    }

    public class PagedData
    {
        public int PageSize;
        public int PageIndex;
        public int total;
        public List<Dictionary<string, object>> rows;
        //public DataTable table;
 
        public string ToJson()
        {
            //Data[0]["Total"]=Total;
            return JsonHelper.ObjectToJSON(rows);
            
        }
        public string ToPagedJson()
        {

            return "{\"total\":" + total + ",\"rows\":" + ToJson() + "}";
        }
    }

}

