using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow;

namespace WorkFlow
{
    public  class DBFactory<T, R> where T : DataTable where R : DataRow
    {
        static public IBaseBLL<T,R>  GetBLL()
        {
            if (ConfigurationManager.ConnectionStrings["Conn"].ProviderName!= "System.Data.SqlClient")
            {
                return new MYSQLBLL<T, R>(new MyDBContext());
            }
            else
                return new ADOBaseBLL<T, R>(new DBContext());
        }

        static public IDBContext GetDBContext()
        {
            if (ConfigurationManager.ConnectionStrings["Conn"].ProviderName != "System.Data.SqlClient")
            {
                return new MyDBContext();
            }
            else
                return new DBContext();
        }
            
    }
}
