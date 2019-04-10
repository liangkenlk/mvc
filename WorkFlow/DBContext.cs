namespace WorkFlow
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;

    public class DBContext : IDBContext
    {
        public SqlDataAdapter adp;
        public SqlCommand cmd;
        public System.Data.CommandType CommandType;
        public SqlConnection con;
        //private string TableName;
        public SqlTransaction trans;

        public DBContext()
        {
            this.CommandType = System.Data.CommandType.Text;
            this.con = new SqlConnection(ConfigurationManager.ConnectionStrings["Conn"].ConnectionString);
            this.cmd = new SqlCommand();
            this.cmd.Connection = this.con;
            this.adp = new SqlDataAdapter(this.cmd);
        }

        public DBContext(string ConnectionStringName)
        {
            this.CommandType = System.Data.CommandType.Text;
            this.con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionStringName"].ConnectionString);
            this.cmd = new SqlCommand();
            this.cmd.Connection = this.con;
            this.adp = new SqlDataAdapter(this.cmd);
        }

        public void AddParam(string name, SqlDbType dbtype, object value)
        {
            SqlParameter parameter = new SqlParameter(name, dbtype) {
                Value = value
            };
            this.cmd.Parameters.Add(parameter);
        }

        public void BeginTransaction()
        {
            this.con.Open();
            this.trans = this.con.BeginTransaction();
            this.cmd.Transaction = this.trans;
        }

        public void End()
        {
            if (this.trans == null)
            {
                this.cmd.CommandType = System.Data.CommandType.Text;
                this.con.Close();
            }
        }

        public void Open()
        {
            if (this.con.State == ConnectionState.Closed)
            {
                this.con.Open();
            }
        }

        public void RollBack()
        {
            try
            {
                if(this.trans!=null)
                this.trans.Rollback();
            }
            catch
            {
            }
        }

        public void TransCommit()
        {
            try
            {
                if (this.trans != null)
                {
                    this.trans.Commit();
                }
                this.con.Close();
                this.trans = null;
            }
            catch (Exception exception)
            {
                this.trans.Rollback();
                this.con.Close();
                throw exception;
            }
        }
    }
}

