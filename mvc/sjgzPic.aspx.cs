using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WorkFlow;

namespace Web
{
    public partial class sjgzPic : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string id = this.Request.QueryString["id"];
                IBaseBLL<Data.ImagesDataTable, Data.ImagesRow> Bll = DBFactory<Data.ImagesDataTable, Data.ImagesRow>.GetBLL();
                string sql = "select * from images where outid='" + id + "'";
                //getfiles(Server.MapPath("~/荔湾区三旧改造标图成果"), id);
                Data.ImagesDataTable table = Bll.GetBySQL(sql);
                this.lt.Text = "<ul id='viewer' class='docs-pictures clearfix'>";
                //string root = this.MapPath("~/").Replace(" ","");
                foreach (Data.ImagesRow f in table)
                {
                    //string vpath=  f.Replace(root, "").Replace(@"\", @"/");
                    string vpath = "upload/case/" + id + "/" + f.FileName;

                    this.lt.Text += "<li><img src='" + vpath + "' alt='"+f.FileName+"' data-original='' /></li>";

                }
                this.lt.Text += "</ul>";

            }
        }

        public List<string> flist = new List<string>();
        public void getfiles(string path, string id)
        {
            var ds = Directory.GetDirectories(path);
            foreach (var dic in ds)
            {
                this.getfiles(dic,id);

            }
            var fs = Directory.GetFiles(path);
            foreach (var f in fs)
            {
                if(Path.GetFileNameWithoutExtension(f).IndexOf(id)>=0)
                  flist.Add(f);
            }

        }
    }
}