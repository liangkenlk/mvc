using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.ProtocolBuffers;
using com.gexin.rp.sdk.dto;
using com.igetui.api.openservice;
using com.igetui.api.openservice.igetui;
using com.igetui.api.openservice.igetui.template;
using com.igetui.api.openservice.payload;
using System.Net;

/**
 * 
 * 说明：
 *      此工程是一个测试工程，所用的相关.dll文件，都已经存在protobuffer文件里，需要加载到References里。
 *      工程中还有用到一个System.Web.Extensions文件，这个文件是用到Framework里V4.0版本的，
 *      一般路径如下：C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0，
 *      或如下路径：C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.5没有的也可以在protobuffer
 *      文件夹里找到。如再有问题，请直接联系技术客服，谢谢！
 *      GetuiServerApiSDK：此.dll文件为个推C#版本的SDK文件
 *      Google.ProtocolBuffers：此.dll文件为Google的数据交换格式文件
 *  注：
 *      新增一个连接超时时间设置，通过在环境变量--用户变量中增加名为：GETUI_TIMEOUT 的变量（修改环境变量，
       电脑重启后才能生效），值则是超时时间，如不设定，则默认为20秒。
 **/

namespace GetuiServerApiSDKDemo
{
    public class demo
    {
        //参数设置 <-----参数需要重新设置----->
        //http的域名
        private static String HOST = "http://sdk.open.api.igexin.com/apiex.htm";

        //https的域名
        //private static String HOST = "https://api.getui.com/apiex.htm";

        //定义常量, appId、appKey、masterSecret 采用本文档 "第二步 获取访问凭证 "中获得的应用配置
        private static String APPID = "djq4owrWrM9VhdHA5lrqn7";
        private static String APPKEY = "Lgs3aMAi7t9Sa3ApOZ73v2";
        private static String MASTERSECRET = "VLXvC6klHzAqxnTxtEHbUA";
        private static String CLIENTID = "a7f536a051965dec65ae7e1b59956f0d";
        private static String CLIENTID1 = "e605a0db5ce3cca9b76b012978064940";
        private static String CLIENTID2 = "e605a0db5ce3cca9b76b012978064940";
        private static String GroupName = "app推送";
        private static String Badge = "50";
        private static String TASKID = "OSA-0903_bWHwhpFPEC7i5nZwHmc6d";
        private static String ALIAS = "请输入别名";
        private static string PN = "13550347892";

        //static void Main(string[] args)
        //{
        //    //toList接口每个用户状态返回是否开启，可选
        //    Console.OutputEncoding = Encoding.GetEncoding(936);
        //    Environment.SetEnvironmentVariable("needDetails", "true");

        //    //下为消息推送的四种方式，单独使用时，请注释掉另外三种方法

        //    //对单个用户的推送
        //    PushMessageToSingle();

        //    //对指定列表用户推送
        //    PushMessageToList();

        //    //对指定应用群推送
        //    pushMessageToApp();

        //    //批量单推
        //    singleBatchDemo();
        //}

        public static void PushMessageToSingle(string cid,string taskid)
        {

            IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);



            //TransmissionTemplate template = TransmissionTemplateDemo();
            NotificationTemplate template = NotificationTemplateDemo(taskid);

            // 单推消息模型
            SingleMessage message = new SingleMessage();
            message.IsOffline = true;                         // 用户当前不在线时，是否离线存储,可选
            message.OfflineExpireTime = 1000 * 3600 * 12;            // 离线有效时间，单位为毫秒，可选
            message.Data = template;
            //判断是否客户端是否wifi环境下推送，2为4G/3G/2G，1为在WIFI环境下，0为不限制环境
            //message.PushNetWorkType = 1;  

            com.igetui.api.openservice.igetui.Target target = new com.igetui.api.openservice.igetui.Target();
            target.appId = APPID;
            target.clientId = cid;
            //target.alias = ALIAS;
            try
            {
                String pushResult = push.pushMessageToSingle(message, target);

                //System.Console.WriteLine("-----------------------------------------------");
                //System.Console.WriteLine("-----------------------------------------------");
                //System.Console.WriteLine("----------------服务端返回结果：" + pushResult);
            }
            catch (RequestException e)
            {
                String requestId = e.RequestId;
                //发送失败后的重发
                String pushResult = push.pushMessageToSingle(message, target, requestId);
                //System.Console.WriteLine("-----------------------------------------------");
                //System.Console.WriteLine("-----------------------------------------------");
                //System.Console.WriteLine("----------------服务端返回结果：" + pushResult);
            }
        }

        //PushMessageToList接口测试代码
        private static void PushMessageToList()
        {
            // 推送主类（方式1，不可与方式2共存）
            IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);
            // 推送主类（方式2，不可与方式1共存）此方式可通过获取服务端地址列表判断最快域名后进行消息推送，每10分钟检查一次最快域名
            //IGtPush push = new IGtPush("",APPKEY,MASTERSECRET);
            ListMessage message = new ListMessage();

            NotificationTemplate template = NotificationTemplateDemo("");
            // 用户当前不在线时，是否离线存储,可选
            message.IsOffline = true;
            // 离线有效时间，单位为毫秒，可选
            message.OfflineExpireTime = 1000 * 3600 * 12;
            message.Data = template;
            //message.PushNetWorkType = 0;        //判断是否客户端是否wifi环境下推送，1为在WIFI环境下，0为不限制网络环境。
            //设置接收者
            List<com.igetui.api.openservice.igetui.Target> targetList = new List<com.igetui.api.openservice.igetui.Target>();
            com.igetui.api.openservice.igetui.Target target1 = new com.igetui.api.openservice.igetui.Target();
            target1.appId = APPID;
            target1.clientId = CLIENTID;

            // 如需要，可以设置多个接收者
            //com.igetui.api.openservice.igetui.Target target2 = new com.igetui.api.openservice.igetui.Target();
            //target2.AppId = APPID;
            //target2.ClientId = "ddf730f6cabfa02ebabf06e0c7fc8da0";

            targetList.Add(target1);
            //targetList.Add(target2);

            String contentId = push.getContentId(message);
            String pushResult = push.pushMessageToList(contentId, targetList);
            System.Console.WriteLine("-----------------------------------------------");
            System.Console.WriteLine("服务端返回结果:" + pushResult);
        }


        //pushMessageToApp接口测试代码
        private static void pushMessageToApp()
        {
            // 推送主类（方式1，不可与方式2共存）
            IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);
            // 推送主类（方式2，不可与方式1共存）此方式可通过获取服务端地址列表判断最快域名后进行消息推送，每10分钟检查一次最快域名
            //IGtPush push = new IGtPush("",APPKEY,MASTERSECRET);

            AppMessage message = new AppMessage();

            // 设置群推接口的推送速度，单位为条/秒，仅对pushMessageToApp（对指定应用群推接口）有效
            message.Speed = 100;

            TransmissionTemplate template = TransmissionTemplateDemo();

            // 用户当前不在线时，是否离线存储,可选
            message.IsOffline = false;
            // 离线有效时间，单位为毫秒，可选  
            message.OfflineExpireTime = 1000 * 3600 * 12;
            message.Data = template;
            //message.PushNetWorkType = 0;        //判断是否客户端是否wifi环境下推送，1为在WIFI环境下，0为不限制网络环境。
            List<String> appIdList = new List<string>();
            appIdList.Add(APPID);

            //通知接收者的手机操作系统类型
            List<String> phoneTypeList = new List<string>();
            //phoneTypeList.Add("ANDROID");
            //phoneTypeList.Add("IOS");
            //通知接收者所在省份
            List<String> provinceList = new List<string>();
            //provinceList.Add("浙江");
            //provinceList.Add("上海");
            //provinceList.Add("北京");

            List<String> tagList = new List<string>();
            //tagList.Add("开心");

            message.AppIdList = appIdList;
            message.PhoneTypeList = phoneTypeList;
            message.ProvinceList = provinceList;
            message.TagList = tagList;


            String pushResult = push.pushMessageToApp(message);
            System.Console.WriteLine("-----------------------------------------------");
            System.Console.WriteLine("服务端返回结果：" + pushResult);
        }

        public static void singleBatchDemo()
        {
            IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);
            IBatch batch = new BatchImpl(APPKEY, push);
            //消息模版：TransmissionTemplate:透传模板
            TransmissionTemplate templateTrans = TransmissionTemplateDemo();

            // 单推消息模型
            SingleMessage messageTrans = new SingleMessage();
            messageTrans.IsOffline = true;                         // 用户当前不在线时，是否离线存储,可选
            messageTrans.OfflineExpireTime = 1000 * 3600 * 12;            // 离线有效时间，单位为毫秒，可选
            messageTrans.Data = templateTrans;
            //判断是否客户端是否wifi环境下推送，2为4G/3G/2G，1为在WIFI环境下，0为不限制环境
            //messageTrans.PushNetWorkType = 1;  

            com.igetui.api.openservice.igetui.Target targetTrans = new com.igetui.api.openservice.igetui.Target();
            targetTrans.appId = APPID;
            targetTrans.clientId = CLIENTID1;
            batch.add(messageTrans, targetTrans);

            NotificationTemplate templateNoti = NotificationTemplateDemo("");

            // 单推消息模型
            SingleMessage messageNoti = new SingleMessage();
            messageNoti.IsOffline = true;                         // 用户当前不在线时，是否离线存储,可选
            messageNoti.OfflineExpireTime = 1000 * 3600 * 12;            // 离线有效时间，单位为毫秒，可选
            messageNoti.Data = templateNoti;
            //判断是否客户端是否wifi环境下推送，2为4G/3G/2G，1为在WIFI环境下，0为不限制环境
            //messageNoti.PushNetWorkType = 1;  

            com.igetui.api.openservice.igetui.Target targetNoti = new com.igetui.api.openservice.igetui.Target();
            targetNoti.appId = APPID;
            targetNoti.clientId = CLIENTID2;
            batch.add(messageNoti, targetNoti);
            try
            {
                String ret = batch.submit();
                Console.Out.WriteLine(ret);
            }
            catch (Exception e)
            {
                batch.retry();
            }
        }


        //通知透传模板动作内容
        public static NotificationTemplate NotificationTemplateDemo(string taskid)
        {
            NotificationTemplate template = new NotificationTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            //通知栏标题
            template.Title = "您有一条新任务";
            //通知栏内容     
            template.Text = "请点击查看";
            //通知栏显示本地图片
            template.Logo = "";
            //通知栏显示网络图标
            template.LogoURL = "";
            //应用启动类型，1：强制应用启动  2：等待应用启动
            template.TransmissionType = 1;
            //透传内容  
            template.TransmissionContent = taskid;
            //接收到消息是否响铃，true：响铃 false：不响铃   
            template.IsRing = true;
            //接收到消息是否震动，true：震动 false：不震动   
            template.IsVibrate = true;
            //接收到消息是否可清除，true：可清除 false：不可清除    
            template.IsClearable = true;
            //设置通知定时展示时间，结束时间与开始时间相差需大于6分钟，消息推送后，客户端将在指定时间差内展示消息（误差6分钟）
            //String begin = "2015-03-06 14:36:10";
            //String end = "2015-03-06 14:46:20";
            //template.setDuration(begin, end);

            return template;
        }

        //透传模板动作内容
        public static TransmissionTemplate TransmissionTemplateDemo()
        {
            TransmissionTemplate template = new TransmissionTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            //应用启动类型，1：强制应用启动 2：等待应用启动
            template.TransmissionType = 1;
            //透传内容  
            template.TransmissionContent = "透传内容";
            //设置通知定时展示时间，结束时间与开始时间相差需大于6分钟，消息推送后，客户端将在指定时间差内展示消息（误差6分钟）
            //String begin = "2015-03-06 14:36:10";
            //String end = "2015-03-06 14:46:20";
            //template.setDuration(begin, end);

            return template;
        }

        //网页模板内容
        public static LinkTemplate LinkTemplateDemo()
        {
            LinkTemplate template = new LinkTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            //通知栏标题
            template.Title = "请填写通知标题";
            //通知栏内容 
            template.Text = "请填写通知内容";
            //通知栏显示本地图片 
            template.Logo = "";
            //通知栏显示网络图标，如无法读取，则显示本地默认图标，可为空
            template.LogoURL = "";
            //打开的链接地址    
            template.Url = "http://www.baidu.com";
            //接收到消息是否响铃，true：响铃 false：不响铃   
            template.IsRing = true;
            //接收到消息是否震动，true：震动 false：不震动   
            template.IsVibrate = true;
            //接收到消息是否可清除，true：可清除 false：不可清除
            template.IsClearable = true;
            return template;
        }

        //通知栏弹框下载模板
        public static NotyPopLoadTemplate NotyPopLoadTemplateDemo()
        {
            NotyPopLoadTemplate template = new NotyPopLoadTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            //通知栏标题
            template.NotyTitle = "请填写通知标题";
            //通知栏内容
            template.NotyContent = "请填写通知内容";
            //通知栏显示本地图片
            template.NotyIcon = "icon.png";
            //通知栏显示网络图标
            template.LogoURL = "http://www-igexin.qiniudn.com/wp-content/uploads/2013/08/logo_getui1.png";
            //弹框显示标题
            template.PopTitle = "弹框标题";
            //弹框显示内容    
            template.PopContent = "弹框内容";
            //弹框显示图片    
            template.PopImage = "";
            //弹框左边按钮显示文本    
            template.PopButton1 = "下载";
            //弹框右边按钮显示文本    
            template.PopButton2 = "取消";
            //通知栏显示下载标题
            template.LoadTitle = "下载标题";
            //通知栏显示下载图标,可为空 
            template.LoadIcon = "file://push.png";
            //下载地址，不可为空
            template.LoadUrl = "http://www.appchina.com/market/d/425201/cop.baidu_0/com.gexin.im.apk ";
            //应用安装完成后，是否自动启动
            template.IsActived = true;
            //下载应用完成后，是否弹出安装界面，true：弹出安装界面，false：手动点击弹出安装界面 
            template.IsAutoInstall = true;
            //接收到消息是否响铃，true：响铃 false：不响铃
            template.IsBelled = true;
            //接收到消息是否震动，true：震动 false：不震动   
            template.IsVibrationed = true;
            //接收到消息是否可清除，true：可清除 false：不可清除    
            template.IsCleared = true;
            return template;
        }

    }
}
