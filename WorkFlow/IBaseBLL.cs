using System.Data;

namespace WorkFlow
{
    public interface IBaseBLL<T, R>
        where T : DataTable
        where R : DataRow
    {
        string TableName { get; }

        void Add(R row);
        void DeleteAll();
        void DeleteAll(string where);
        void Detele(string ids);
        void ExecuteNonQuery(string sql);
        object ExecuteScalar(string sql);
        T GetByFilter(string filter);
        R GetByKey(object key);
        T GetBySQL(string sql);
        object GetBySQL(string v1, object name, string v2);
        string GetKey(string TableName = null);
        PagedData GetNews(string sql);
        DataTable GetNormalDataTable(string sql);
        PagedData GetPagedDataTable(string sql, int PageSize, int PageIndex);
        R NewRow();
        void Update(T table);
        void Update(R row);
    }
}