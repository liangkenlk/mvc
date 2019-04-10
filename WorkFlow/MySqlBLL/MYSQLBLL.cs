using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow
{
    public class MYSQLBLL<T, R> : IBaseBLL<T, R> where T : DataTable where R : DataRow
    {
        public MYSQLdal<T, R> dal;
        public string TableName
        { get { return this.dal.TableName; } }

        public MYSQLBLL(MyDBContext context)
        {
            this.dal = new MYSQLdal<T, R>();
        }

        public void Add(R row)
        {
            this.dal.Add(row);
        }

        public void DeleteAll()
        {
            this.dal.DeleteAll();
        }

        public void DeleteAll(string where)
        {
            this.dal.DeleteAll(where);
        }

        public void Detele(string ids)
        {
            this.dal.Delete(ids);
        }

        public void ExecuteNonQuery(string sql)
        {
            this.dal.ExecuteNonQuery(sql);
        }

        public object ExecuteScalar(string sql)
        {
            return this.dal.ExecuteScalar(sql);
        }

        public T GetByFilter(string filter)
        {
            return this.dal.GetByFilter("select * from " + this.dal.TableName, filter);
        }

        public R GetByKey(object key)
        {
            return this.dal.GetByKey(key);
        }

        public T GetBySQL(string sql)
        {
            return this.dal.GetBySQL(sql);
        }

        public string GetKey(string TableName = null)
        {
            return this.dal.GetKey(TableName);
        }

        public DataTable GetNormalDataTable(string sql)
        {
            return this.dal.GetNormalDataTable(sql);
        }

        public PagedData GetNews(string sql)
        {

            return this.dal.GetNews(sql);
        }


        public PagedData GetPagedDataTable(string sql, int PageSize, int PageIndex)
        {

            return this.dal.GetPagedDataTable(sql, PageSize, PageIndex);
        }

        public R NewRow()
        {
            return this.dal.NewRow();
        }

        public object GetBySQL(string v1, object name, string v2)
        {
            throw new NotImplementedException();
        }

        public void Update(R row)
        {
            this.dal.Update(row);
        }

        public void Update(T table)
        {
            this.dal.Update(table);
        }
    }
}
