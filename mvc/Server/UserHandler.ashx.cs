namespace iParking.Server
{
    using iParking;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Security;
    using TY.Core;
    using TY.UI.Ajax;
    using Web.Server;
    using WorkFlow;

    public class UserHandler : AjaxHandler
    {
        private IBaseBLL<Data.UserDataTable, Data.UserRow> userbll = DBFactory<Data.UserDataTable, Data.UserRow>.GetBLL();
        //[Description("批准用户的司机身份：LoginID")]
        public void BecomeDriver()
        {
            Data.UserDataTable bySQL = this.userbll.GetBySQL("select * from [user] where loginid='" + base.Query<string>("LoginID") + "'");
            try
            {
                Data.UserRow row = bySQL.Rows[0] as Data.UserRow;
                row.IsParkDriver = true;
                this.userbll.Update(row);
                base.jsonResult = JsonHelper.OutResult(true, "更新成功");
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError(exception.Message);
            }
        }
        
        public void DeleteSelect()
        {
            string selected = this.Query<string>("selected");
            this.userbll.ExecuteNonQuery("delete  from  [user] where userid in (" + selected + ")");
            jsonResult = JsonHelper.OutResult(true, "ok");
        }
        public  void GetUserList()
        {            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
             string username = this.Query<string>("username");
            string sql = "select * from [user]";
            if (username != null)
                sql += " where LoginID like '%" + username + "%'";
           // var list = userbll.GetBySQL(sql);
            jsonResult =  userbll.GetPagedDataTable(sql, pageSize, pageindex).ToPagedJson();
        }
        public void GetUserListForDropDonw()
        {
            int pageindex = this.Query<int>("page") - 1;
            int pageSize = this.Query<int>("rows");
            string username = this.Query<string>("username");
            string sql = "select * from [user]";
            if (username != null)
                sql += " where username like '%" + username + "%'";
            var list = userbll.GetBySQL(sql);
            var nullrow = list.NewUserRow();
            
            nullrow.UserName="全部";
            list.Rows.InsertAt(nullrow, 0);
            jsonResult = JsonHelper.DataTableToJSON(list);
        }
        [Description("删除用户信息：参数 LoginID")]
        public void Delete()
        {
            try
            {
                this.userbll.ExecuteNonQuery("delete from [user] where loginid='" + base.Query<string>("LoginID") + "'");
                base.jsonResult = JsonHelper.OutResult(true, "删除成功");
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError("删除失败：" + exception.Message);
            }
        }

        //[Description("获得空闲司机")]
        public void GetFreeDriver()
        {
            try
            {
                string sql = "SELECT [user].userid,Longitude,Latitude FROM [user] LEFT JOIN (\r\nSELECT * FROM dbo.DriverTrace WHERE TraceID IN (\r\nSELECT MAX(TraceID) FROM dbo.DriverTrace GROUP BY userid)) AS a ON a.UserID = dbo.[User].UserId  where [User].IsParkDriver=1 and [User].DriverState='空闲' and isOnline=1";
                DataTable normalDataTable = this.userbll.GetNormalDataTable(sql);
                base.jsonResult = JsonHelper.DataTableToJSON(normalDataTable);
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError(exception.Message);
            }
        }
        //[Description("获得所有在线司机")]
        public void GetOnlineDriver()
        {
            try
            {
                string sql = @"SELECT  [User].UserId ,
        a.Longitude ,
        a.Latitude ,
        dbo.ParkOrder.FlowState,
        [user].DriverState,[user].UserName,[user].LoginID,[user].Mark
FROM    [User]
        LEFT JOIN ( SELECT  *
                    FROM    dbo.DriverTrace
                    WHERE   TraceID IN ( SELECT MAX(TraceID)
                                         FROM   dbo.DriverTrace
                                         GROUP BY UserID )
                  ) AS a ON a.UserID = dbo.[User].UserId
                  LEFT JOIN dbo.ParkOrder ON ParkOrder.OrderID = a.OrderID
WHERE   [User].IsParkDriver = 1
        AND IsOnline = 1";
                DataTable normalDataTable = this.userbll.GetNormalDataTable(sql);
                //foreach (DataRow user in normalDataTable.Rows)
                ////{
                ////    if (!SocketListen.SocketList.ContainsKey(user["UserId"].ToString()))
                ////    {
                ////        user.Delete();
                ////    }
                //}
                normalDataTable.AcceptChanges();
                base.jsonResult = JsonHelper.DataTableToJSON(normalDataTable);
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError(exception.Message);
            }
        }


        [Description("获得用户信息:参数LoginID 或 UserID")]
        public void GetUserInfo()
        {
            try
            {
                string str = base.Query<string>("LoginID");
                string str2 = base.Query<string>("UserID");
                string sql = "select * from [user] where UserID='" + str2 + "'";
                if (str2 == null)
                {
                    sql = "select * from [user] where LoginID='" + str + "'";
                }
                Data.UserDataTable bySQL = this.userbll.GetBySQL(sql);
                base.jsonResult = JsonHelper.ObjectToJSON(JsonHelper.DataTableToList(bySQL)[0]);
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError(exception.Message);
            }
        }

        [Description("判断用户名是否可以使用 参数 loginid")]
        public void IsLoginIdExist(string loginid)
        {
            if (this.userbll.ExecuteScalar("select * from [user] where loginid='" + loginid + "'") != null)
            {
                base.jsonResult = JsonHelper.OutResult(false, "用户名已存在。");
            }
            else
            {
                base.jsonResult = JsonHelper.OutResult(true, "可以使用。");
            }
        }

        public void Login4old()
        {
            var dic = this.Login();

            jsonResult = JsonHelper.ObjectToJSON(dic);
        }

        public void mobile_login()
        {
            var dic = this.Login();
            jsonResult = JsonHelper.ObjectToJSON(dic);
        }

        [Description("登录  参数Loginid,Password 成功返回令牌Token")]
        public Dictionary<string,object> Login()
        {
            string LoginID = base.Query<string>("LoginID");
            string PassWord = base.Query<string>("PassWord");
            string MsgCode = base.Query<string>("MsgCode");
            Dictionary<string, object> dic = new Dictionary<string, object>();

              string errMsg = "";
            if (LoginID == null)
            {
                base.jsonResult = JsonHelper.OutResult(false, "登录名不能为空。");
                dic["result"] = false;
                dic["reason"] = "登录名不能为空。";
                dic["success"] = false;
                
                return dic;
            }
             string msg =null;
            //if(MsgCode!=null)
            //     msg = UserAuth.GetAuthTokenByMsgCode(loginID, MsgCode, out errMsg);
            //else
             msg = UserAuth.GetAuthToken(LoginID, PassWord, out errMsg);

            if (msg == null)
            {
                
                base.jsonResult = JsonHelper.OutResult(false, errMsg);
                dic["result"] = false;
                dic["reason"] = errMsg;
                dic["success"] = false;
                return dic;
            }
            else
            {
                HttpCookie cookie = new HttpCookie("token", msg);
                cookie.Expires = DateTime.Now.AddDays(1000);
                cookie.Path = "/";
                HttpContext.Current.Response.SetCookie(cookie);
                //填充微信用的openid
                var user = userbll.GetByKey(UserAuth.UserID);
                if ( HttpContext.Current.Session["openid"] != null)
                {
                    user.OpenID = HttpContext.Current.Session["openid"].ToString();
                    userbll.Update(user);
                   
                }
                //创建身份验证票   
                FormsAuthenticationTicket AuTicket = new FormsAuthenticationTicket(2, LoginID, DateTime.Now, DateTime.Now.AddDays(1000), true, "user");
                ////将票据加密  
                string encTicket = FormsAuthentication.Encrypt(AuTicket);
                HttpCookie Acookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                Acookie.HttpOnly = true;
                Acookie.Secure = FormsAuthentication.RequireSSL;
                Acookie.Domain = FormsAuthentication.CookieDomain;
                Acookie.Path = FormsAuthentication.FormsCookiePath;
                Acookie.Expires = DateTime.Now.AddDays(1000);
              
                ////加入新cookie  
                HttpContext.Current.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);

                HttpContext.Current.Response.Cookies.Add(Acookie);
                //FormsAuthentication.SetAuthCookie("user", true);
                base.jsonResult = JsonHelper.OutResult(true, msg,user.UserId.ToString());
                dic["result"] = true;
                dic["success"] = true;
                dic["city"] = new XMLHandler().getConfigValueByName("地区拼音");
                dic["userId"] = UserAuth.UserID;
                dic["statistics"] = "";
                return dic;
            }
        }

        [Description("登出")]
        public void Logout()
        {
            UserAuth.Logout();
            FormsAuthentication.SignOut();
        }





        [Description("注册：参数 UserName,Password,LoginID,IDCard,BankCard,CellPhone,Address,Email,DriverAge")]
        public void Register()
        {
            string str = base.Query<string>("loginid");
            if (str == null)
            {
                base.jsonResult = JsonHelper.OutResult(false, "登录名不能为空。");
            }
            else if (this.userbll.ExecuteScalar("select * from [user] where loginid='" + str + "'") != null)
            {
                base.jsonResult = JsonHelper.OutResult(false, "用户名已存在。");
            }
            else
            {
                try
                {
                    Data.UserRow row = this.userbll.NewRow();
                    if (base.Query<string>("UserName") == null)
                    {
                        throw new Exception("用户名不能为空。");
                    }
                    row.UserName = base.Query<string>("UserName");
                    if (base.Query<string>("PassWord") != null)
                    {
                        row.PassWord = base.Query<string>("PassWord");
                    }
                    row.RegTime = DateTime.Now;
                    if (base.Query<string>("LoginID") != null)
                    {
                        row.LoginID = base.Query<string>("LoginID");
                    }
                    row.IsOnline = true;
                    if (base.Query<string>("IDCard") != null)
                    {
                        row.IDCard = base.Query<string>("IDCard");
                    }
                    row.IsParkDriver = false;
                    if (base.Query<string>("BankCard") != null)
                    {
                        row.BankCard = base.Query<string>("BankCard");
                    }
                    if (base.Query<string>("CellPhone") != null)
                    {
                        row.CellPhone = base.Query<string>("CellPhone");
                    }
                    if (base.Query<string>("Address") != null)
                    {
                        row.Address = base.Query<string>("Address");
                    }
                    if (base.Query<string>("DriverAge")!=null)
                    {
                        row.DriverAge = base.Query<int>("DriverAge");
                    }
                   // if(HttpContext.Current.Session[])
                    row.DriverState = "空闲";
                    this.userbll.Add(row);
                    base.jsonResult = JsonHelper.OutResult(true, "注册成功");
                }
                catch (Exception exception)
                {
                    base.jsonResult = JsonHelper.OutError(exception.Message);
                }
            }
        }

        [Description("发送验证码：参数 CellPhone")]
        public void SendValidateCode()
        {
        }

        public void Save()
        { 
            Data.UserRow data ;
            if (this.Query<int>("UserId") != 0)
                data = this.userbll.GetByKey(this.Query<string>("UserId"));
            else
                data = this.userbll.NewRow();
        
            data.UserName = this.Query<string>("UserName");
            data.LoginID = this.Query<string>("LoginID");
            if (this.Query<int>("UserId") == 0)
            {
                if (this.userbll.GetBySQL("select * from [user] where Loginid='" + data.LoginID + "'").Count > 0)
                    
                {
                    jsonResult = JsonHelper.OutResult(false, "登录名已存在！");
                    return;
                }
            }
            
    
            data.PassWord = this.Query<string>("PassWord");
            data.CellPhone = this.Query<string>("CellPhone");
            data.DataPower = this.Query<string>("DataPower");
            data.UserType = this.Query<string>("UserType");

            if (this.Query<int>("UserId") != 0)
                userbll.Update(data);
            else
                userbll.Add(data);
            var a = this.userbll.GetBySQL("select * from [user] where Loginid='" + data.LoginID + "'")[0];
            jsonResult = JsonHelper.OutResult(true, a.UserId.ToString());

        }

        [Description("修改用户信息：参数 UserName,Password,LoginID,IDCard,BankCard,CellPhone,Address,Email,DriverAge")]
        public void Update()
        {
            Data.UserDataTable bySQL = this.userbll.GetBySQL("select * from [user] where loginid='" + base.Query<string>("LoginID") + "'");
            try
            {
                Data.UserRow row = bySQL.Rows[0] as Data.UserRow;
                if (base.Query<string>("UserName") != null)
                {
                    row.UserName = base.Query<string>("UserName");
                }
                if (base.Query<string>("PassWord") != null)
                {
                    row.PassWord = base.Query<string>("PassWord");
                }
                //row.RegTime = DateTime.Now;
                if (base.Query<string>("LoginID") != null)
                {
                    row.LoginID = base.Query<string>("LoginID");
                }
                //row.IsOnline = true;
                if (base.Query<string>("IDCard") != null)
                {
                    row.IDCard = base.Query<string>("IDCard");
                }
               // row.IsParkDriver = false;
                if (base.Query<string>("BankCard") != null)
                {
                    row.BankCard = base.Query<string>("BankCard");
                }
                if (base.Query<string>("Email") != null)
                {
                    row.Email = base.Query<string>("Email");
                }
                if (base.Query<string>("CellPhone") != null)
                {
                    row.CellPhone = base.Query<string>("CellPhone");
                }
                if (base.Query<string>("Address") != null)
                {
                    row.Address = base.Query<string>("Address");
                }
                if (base.Query<int>("DriverAge")!=0)
                {
                    row.DriverAge = base.Query<int>("DriverAge");
                }
                this.userbll.Update(row);
                base.jsonResult = JsonHelper.OutResult(true, "更新成功");
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError(exception.Message);
            }
        }

        //[Description("文件上传 参数：type(上传图片的命名定义：\r\n头像:Face.png\r\n身份证正面:IdentityCardFront.png\r\n身份证背面:IdentityCardBack.png\r\n驾驶证:DriverLicense.png\r\n泊车位:ParkingLot.png\r\n油表盘:OilDial.png\r\n多张用逗号隔开)")]
        public void UploadFile()
        {
            try
            {
                HttpFileCollection files = HttpContext.Current.Request.Files;
                if (files.Count > 0)
                {

                    for (int i = 0; i < files.Count; i++)
                    {
                        string path = HttpContext.Current.Server.MapPath("~/upload") + @"\user\" + UserAuth.UserID;
                        string filename = path + @"\" + base.Query<string>("type").Split(',')[i] + Path.GetExtension(files[i].FileName);
                        Directory.CreateDirectory(path);
                        files[i].SaveAs(filename);

                    }
                    base.jsonResult = JsonHelper.OutResult(true, "上传成功。");
                }
                else
                {
                    base.jsonResult = JsonHelper.OutResult(false, "没有收到文件！");
                }
            }
            catch (Exception exception)
            {
                base.jsonResult = JsonHelper.OutError(exception.Message);
            }
        }

        [Description("验证手机：参数 ValidateCode,CellPhone")]
        public void ValidateCellPhone()
        {
            string str = base.Query<string>("ValidateCode");
            string str2 = base.Query<string>("CellPhone");
        }
        //[Description("司机信息是否完整")]
        public void isDriverInfoComplete()
        { 
            
            //var user = userbll.GetByKey(userid);

            string path =  HttpContext.Current.Server.MapPath("~/upload/user/"+UserAuth.UserID+"/");
            if (File.Exists(path + "Face.png") && File.Exists(path + "IdentityCardFront.png") && File.Exists(path + "IdentityCardBack.png") && File.Exists(path + "DriverLicense.png"))
            {
                base.jsonResult = JsonHelper.OutResult(true, "头像，身份证正反面，驾照齐全！");
            }
            
            else
            {
                base.jsonResult = JsonHelper.OutResult(false, "头像，身份证正反面，驾照不齐全！");
            }

            
        }

        public void getCurrentUserInfo()
        {
            var user = userbll.GetByKey(UserAuth.UserID);
            jsonResult = JsonHelper.DataRowToJSON(user);
        }

        //获取当前用户的用户名(登录成功后存放在在cookie中的用户名信息）
        public String getCurrentUserName()
        {
           

            return null;
        }
  

 

    }
}

