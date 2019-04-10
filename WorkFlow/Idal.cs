using System.Data;

namespace WorkFlow
{
    public interface Idal<T, R>
        where T : DataTable
        where R : DataRow
    {
        void Add(R row);
        void Delete(T table);
        void Delete(string ids);
        void DeleteAll();
        void DeleteAll(string where);
        void ExecuteNonQuery(string sql);
        object ExecuteScalar(string sql);
        T GetByFilter(string sql, string filter);
        T GetByids(string ids);
        R GetByKey(object key);
        T GetBySQL(string sql);
        T GetDataTable(string sql);
        string GetKey(string TableName = null);
        DataTable GetNormalDataTable(string sql);
        PagedData GetPagedDataTable(string sql, int PageSize, int PageIndex);
        R NewRow();
        void Update(T table);
        void Update(R row);
    }
}