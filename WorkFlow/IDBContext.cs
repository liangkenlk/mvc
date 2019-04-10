using System.Data;

namespace WorkFlow
{
    public interface IDBContext
    {
        void AddParam(string name, SqlDbType dbtype, object value);
        void BeginTransaction();
        void End();
        void Open();
        void RollBack();
        void TransCommit();
    }
}