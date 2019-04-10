namespace TY.Utility
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.UI;

    public static class ScriptHelper
    {
        public static void Alert(Page page, string msg)
        {
            RegisterScript(page, JS_Alert(msg));
        }

        public static void AlertAndClose(Page page, string info, bool refreshParentWindow)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(JS_Alert(info)).Append(JS_Close(refreshParentWindow));
            WriteScript(page, builder.ToString());
        }

        public static void AlertAndCloseDialog(Page page, string info, string returnValue)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(JS_Alert(info)).Append(JS_CloseDialog(returnValue));
            WriteScript(page, builder.ToString());
        }

        public static void AlertAndCloseDialogOld(Page page, string info, string returnValue)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(JS_Alert(info)).Append(JS_CloseDialogOld(returnValue));
            WriteScript(page, builder.ToString());
        }

        public static void Close(Page page, bool refreshParentWindow)
        {
            RegisterScript(page, JS_Close(refreshParentWindow));
        }

        public static void CloseDialog(Page page, string returnValue)
        {
            RegisterScript(page, JS_CloseDialog(returnValue));
        }

        public static void CloseDialogOld(Page page, string returnValue)
        {
            RegisterScript(page, JS_CloseDialogOld(returnValue));
        }

        public static void Confirm(Page page, string msg, string isTrueDo, string isFalseDo)
        {
            RegisterScript(page, JS_Confirm(msg, isTrueDo, isFalseDo));
        }

        public static string EscapeString(string str, bool escapeDoubleQuote)
        {
            string pattern = string.Format(@"('|\\{0}|(?:\r?\n))", escapeDoubleQuote ? "|\"" : string.Empty);
            str = Regex.Replace(str, pattern, @"\$1");
            return str;
        }

        private static string FormatJsString(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                s = s.Replace(@"\", @"\\").Replace("'", @"\'").Replace("\r", "").Replace("\n", @"\n");
            }
            return s;
        }

        public static void Goto(Page page, string urlPage)
        {
            RegisterScript(page, JS_Goto(urlPage));
        }

        private static string JS_Alert(string msg)
        {
            return string.Format("alert('{0}');", FormatJsString(msg));
        }

        private static string JS_Close(bool refreshParentWindow)
        {
            string str = string.Empty;
            if (refreshParentWindow)
            {
                str = "try{window.opener.location.href+='';window.opener=null;}catch(e){}";
            }
            return (str + "window.close();");
        }

        private static string JS_CloseDialog(string returnValue)
        {
            if (returnValue != "")
            {
                if (returnValue == "true")
                {
                    return "var dg = frameElement.lhgDG;var tvalue=dg.getArgs();if(tvalue=='1'){dg.curWin.location.reload();}else{dg.curDoc.forms['form1'].__EVENTTARGET.value = dg.getArgs();dg.curDoc.forms['form1'].__EVENTARGUMENT.value = '';dg.curDoc.forms['form1'].submit();}dg.cancel();";
                }
                return "var dg = frameElement.lhgDG;dg.cancel();";
            }
            return "var dg = frameElement.lhgDG;dg.cancel();";
        }

        private static string JS_CloseDialogOld(string returnValue)
        {
            return string.Format("window.returnValue='{0}';window.close();", FormatJsString(returnValue));
        }

        private static string JS_Confirm(string msg, string isTrueDo, string isFalseDo)
        {
            return ("if(confirm('" + FormatJsString(msg) + "')){" + isTrueDo + "} else {" + isFalseDo + "}");
        }

        private static string JS_Goto(string urlPage)
        {
            return string.Format("top.location.href='{0}';", FormatJsString(urlPage));
        }

        private static string JS_Open(string url)
        {
            return string.Format("window.open(encodeURI('{0}'));", FormatJsString(url));
        }

        private static string JSRedirect(string urlPage)
        {
            return string.Format("location.href='{0}';", FormatJsString(urlPage));
        }

        private static string JSTopRedirect(string urlPage)
        {
            return string.Format("top.location.href='{0}';", FormatJsString(urlPage));
        }

        public static void Open(Page page, string url)
        {
            RegisterScript(page, JS_Open(url));
        }

        public static void Redirect(Page page, string urlPage)
        {
            RegisterScript(page, JSRedirect(urlPage));
        }

        public static void RegisterScript(Page page, string script)
        {
            string str = "(function(){" + script + "})();";
            page.ClientScript.RegisterStartupScript(page.GetType(), Guid.NewGuid().ToString("N"), str, true);
        }

        public static void TopRedirect(Page page, string urlPage)
        {
            RegisterScript(page, JSTopRedirect(urlPage));
        }

        public static void WriteScript(Page page, string script)
        {
            string s = "<script type=\"text/javascript\">(function(){" + script + "})();</script>";
            page.Response.Write(s);
        }
    }
}

