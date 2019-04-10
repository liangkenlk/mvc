namespace TY.UI.Ajax
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using TY.Core;

    public class AjaxHandler : AjaxBase
    {
        public override void BeforeInvoke()
        {
        }

        public bool ValidateString(string type, string input)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["经度"] = @"^-?((0|[1-9]\d?|1[1-7]\d)(\.\d{1,7})?|180(\.0{1,7})?)?$";
            dictionary["纬度"] = @"^-?((0|[1-8]\d|)(\.\d{1,7})?|90(\.0{1,7})?)?$";
            dictionary["电子邮箱"] = @"^(\w)+(\.\w+)*@(\w)+((\.\w{2,3}){1,3})$";
            dictionary["手机"] = @"^((\d{3,4}\s+)?\d{7,8}$|(\d{3,4}-)?\d{7,8}$|1[3|4|5|8][0-9]\d{4,8})$";
            dictionary["工作电话"] = @"^((\d{3,4}\s+)?\d{7,8}$|(\d{3,4}-)?\d{7,8}$|1[3|4|5|8][0-9]\d{4,8})$";
            Regex regex = new Regex(dictionary[type]);
            return regex.IsMatch(input);
        }
    }
}

