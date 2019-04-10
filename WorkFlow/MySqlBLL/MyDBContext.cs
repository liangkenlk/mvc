namespace WorkFlow
{
    using System;
    using System.Configuration;
    using System.Data;
    using MySql.Data.MySqlClient;

    public class MyDBContext:IDBContext
    {
        public MySqlDataAdapter adp;
        public MySqlCommand cmd;
        public System.Data.CommandType CommandType;
        public MySqlConnection con;
        //private string TableName;
        public MySqlTransaction trans;

        public MyDBContext()
        {
            this.CommandType = System.Data.CommandType.Text;
            this.con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Conn"].ConnectionString);
            this.cmd = new MySqlCommand();
            this.cmd.Connection = this.con;
            this.adp = new MySqlDataAdapter(this.cmd);
        }

        public MyDBContext(string ConnectionStringName)
        {
            this.CommandType = System.Data.CommandType.Text;
            this.con = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionStringName"].ConnectionString);
            this.cmd = new MySqlCommand();
            this.cmd.Connection = this.con;
            this.adp = new MySqlDataAdapter(this.cmd);
        }

        public void AddParam(string name, SqlDbType dbtype, object value)
        {
            MySqlParameter parameter = new MySqlParameter(name, dbtype) {
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

