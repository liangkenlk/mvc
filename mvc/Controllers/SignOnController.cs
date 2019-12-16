using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TY.Core;
using Web.lib;
using WorkFlow;

namespace mvc.Controllers
{
    public class SignOnController : BaseController<Data.SignOnDataTable, Data.SignOnRow>
    {
        public ActionResult Getgrant_code()
        {
           var code =   this.Query<string>("code");
            this.HttpContext.Application["code"] = code;
            return JsonOb(code);
                
        }
        public ActionResult Showgrant_code()
        {
            var code = this.HttpContext.Application["code"].ToString();
            return JsonOb(code);

        }
    }
}