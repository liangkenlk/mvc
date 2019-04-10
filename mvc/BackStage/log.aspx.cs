using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web.BackStage
{
    public partial class log : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id = this.Request.QueryString["id"];
            getfiles(Server.MapPath("~/log"), id);
            this.lt.Text = "<ul id='viewer' class='docs-pictures clearfix'>";
            string root = this.MapPath("~/").Replace(" ", "");
            foreach (var f in flist.OrderByDescending(p=>p.Key))
            {
                string vpath ="../"+ f.Value.Replace(root, "").Replace(@"\", @"/");

                this.lt.Text += "<li><a href='" + vpath + "' target='_blank'/>"+ Path.GetFileNameWithoutExtension(f.Value) + "</a>";

            }
            this.lt.Text += "</ul>";
        }

        public SortedList<string,string> flist = new SortedList<string,string>();
        public void getfiles(string path, string id)
        {
            var ds = Directory.GetDirectories(path);
            foreach (var dic in ds)
            {
                this.getfiles(dic, id);

            }
            var fs = Directory.GetFiles(path);
            foreach (var f in fs)
            {
   
                    flist.Add(f,f);
            }

        }
    }
}