
var CurrentOrder;
var userinfo;
var NowPrise;//当期停车时长和计费
var STATE;
var ws;//websocket
var serverIP = "ws://192.168.1.10:8885";
//var rootUrl = "http://localhost/GisBaseForH5/";
//var rootUrl = "http://192.168.43.236/GisBaseForH5/";
//var rootUrl = "http://192.168.2.80/GisBaseForH5/";
var rootUrl = "";

function freshState(s)
{
    if(s!=null)
    {
        STATE = s;
    }
    $.ajax({
        type: "POST",
        url: "../Server/ParkOrderHandler.ashx?Method=OrderState&r=" + Math.random() + Date.parse(new Date()),
   
        success: function (data) {
            
            var a = STATE.split(',');
            for (var i = 0; i < a.length; i++) {
                if (data == a[i]) {
                    setTimeout(freshState(), 3000);
                    $('#p').text(data);
                    return;
                    
                }

            }
            window.open("index.aspx?r=" + Math.random() + Date.parse(new Date()), "_self");
        },
        error: function () {
            
            setTimeout(freshState(), 3000);
        }
    });
}
/*
function freshState(s)
{
    
    if (s != null) {
        STATE = s;
    }
    ws = new WebSocket(serverIP);
    ws.onopen = function () {
       
        ws.send("token:" + encodeURIComponent(getCookie("token")));
    };
    ws.onmessage = function (evt) {
        if (evt.data == "change" || evt.data == "token正确")
        {
            bindInfo();
            return;
        }
        var a = STATE.split(',');
        for (var i = 0; i < a.length; i++) {
            if (evt.data == a[i]) {
               
                $('#p').text(evt.data);
                return;

            }

        }
        window.open("index.aspx?r=" + Math.random() + Date.parse(new Date()), "_self");
    };
    ws.onclose = function (evt) {
       
    };
    ws.onerror = function (evt) {
        //freshState(s);
    };
}*/
function sleep(numberMillis) {
    var now = new Date();
    var exitTime = now.getTime() + numberMillis;
    while (true) {
        now = new Date();
        if (now.getTime() > exitTime)
            return;
    }
}

function setBtnPower() {
    //if (IsSetBtnPower) {
    //    return;
    //}
    var url = "user/getCurrentUserInfo";

    j({}, url, setBtnPowerCallBack);
}
function setBtnPowerCallBack(re) {


    let list = re.BtnPower.split(",");
    for (var i = 0; i < list.length; i++) {
        if (list[i] == "")
            continue;
        $(".easyui-linkbutton :contains('"+list[i]+"')").hide()
    }

}
function show_msg(msg, url) {
    var d = dialog({ content: msg }).show();
    setTimeout(function () {
        d.close().remove();
        if (url) { window.location.href = url; }
    }, 2000);
}

function GetUserInfo(userid, async)
{
    $.ajax({
        type: "POST",
        url: "/Server/UserHandler.ashx?Method=GetUserInfo",
        data: { userid: userid },
        async:async,
        success: function (data) {
            userinfo = eval('('+data+')');
            }
    });
}

function GetUserCurrentOrder(async) {
    $.ajax({
        type: "POST",
        url: "../Server/ParkOrderHandler.ashx?Method=GetUserCurrentOrder",
        async: async,
        success: function (data) {
            data = eval('(' + data + ')');
            CurrentOrder = data;
        }
    });
}
function SetCookie(name, value)//两个参数，一个是cookie的名子，一个是值
{
    var Days = 30; //此 cookie 将被保存 30 天
    var exp = new Date();    //new Date("December 31, 9998");
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + unescape(value) + ";expires=" + exp.toGMTString();
}
function GetNowPrise(async) {
    $.ajax({
        type: "POST",
        url: "../Server/ParkOrderHandler.ashx?Method=GetNowPrise",
        async: async,
        success: function (data) {
            data = eval('(' + data + ')');
            NowPrise = data;
        }
    });
}

function cancel_order() {
    var d = dialog({
        title: '提示',
        content: '确定取消召唤代泊员吗?',
        okValue: '确定',
        quickClose: false,
        ok: function () {
            _dobj = this;
            _dobj.title('提交中'); _dobj.content('正在取消，请稍等…');
            $.ajax({
                type: "POST",
                url: "../Server/ParkOrderHandler.ashx?Method=UserAbortOrder",
                success: function (msg) {
                    msg = eval('(' + msg + ')');
                    if (msg.Success == true) {
                        show_msg(msg.Message, "index.aspx?r=" + Math.random() + Date.parse(new Date()));
                    } else {
                        show_msg(msg.Message);
                    }
                    d.close().remove();
                }
            });
            return false;
        },
        cancelValue: '取消',
        cancel: function () { }
    }).width(180).height(20).show();
}

function getCookie(name)//取cookies函数        
{
    var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
    if (arr != null) return unescape(arr[2]); return null;

}
function delCookie(name)//删除cookie
{
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null) document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
}

//根据QueryString参数名称获取值

function getQueryStringByName(name) {

    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));

    if (result == null || result.length < 1) {

        return null;

    }

    return decodeURIComponent(result[1]);

}
function j(data, url, callBack, async, ob)
{
    GetObjectByMethod(ob, data, url, callBack, async);
}

function GetObjectByMethod(ob, data, url,callBack, async)
{
    var myRootUrl ="" ;

    if (url.toLowerCase().indexOf("http://") != 0)
    {
        myRootUrl = rootUrl;
        if (window.plus)
            data["token"] = plus.storage.getItem("token");//说明是外部网站
    }
       
    $.ajax({
        type: "POST",
        url: myRootUrl + url.replaceAll("../", ""),
        data: data,
        dataType: 'json',
        //async: async,
        xhrFields: {

            withCredentials: false

        },

        crossDomain: true,
        //crossDomain: true,
        success: function (msg) {
            // ob.set(eval('(' + msg + ')'));
            //ob.set(msg);
           // alert(msg.data);
            if (!msg.Success && msg.Message == "你未登录！")
              location.href = "../login.html";
            callBack(msg);
        },
        error: function (e) {

            //alert(e);
        }

    });
}

var ajaxObject = function ()
{
    this.value = {};
    this.set = function (v) { this.value = v; }
    
}

//扩展
String.prototype.replaceAll = function (target, replacement) {
    return this.split(target).join(replacement);
}

Array.prototype.contains = function (value)
{
    for (x in this)
    {
        this[x] == value;
        return true;
    }
    return false;
}

Array.prototype.findById = function (value) {
    for (x in this) {
        if(this[x].id == value)
        return this[x];
    }
    return null;
}

function jUp(formData, url, onprogress, callBack) {
    $.ajax({
        type: "POST",
        url: url,
        data: formData,　　//这里上传的数据使用了formData 对象
        processData: false,
        //必须false才会自动加上正确的Content-Type 
        contentType: false,

        //这里我们先拿到jQuery产生的 XMLHttpRequest对象，为其增加 progress 事件绑定，然后再返回交给ajax使用
        xhr: function () {
            var xhr = $.ajaxSettings.xhr();
            if (onprogress && xhr.upload) {
                xhr.upload.addEventListener("progress", onprogress, false);
                return xhr;
            }
        },
        success: function (msg) {
            // ob.set(eval('(' + msg + ')'));
            //ob.set(msg);
            if (!msg.Success && msg.Message == "你未登录！")
                location.href = "../login.html";
            callBack(msg);
        }
    });
}

$(function () {
    document.onkeydown = function (event) {
        var e = event || window.event || arguments.callee.caller.arguments[0];
        if (e && e.keyCode == 13) { // 按 Esc 
            //要做的事情
            $("[Enter=true]:visible").click();
        }
    }
})

function myPro(title)
{ $.messager.progress({ title: title }); }

function myProClose()
{ $.messager.progress("close"); }

function myAlert(msg,title)
{
    if (title == null)
        title = "提示";
    $.messager.alert({ title: title, msg: msg });
}
$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

function jUp(formData, url, onprogress, callBack) {
    $.ajax({
        type: "POST",
        url: url,
        data: formData,　　//这里上传的数据使用了formData 对象
        processData: false,
        //必须false才会自动加上正确的Content-Type 
        contentType: false,

        //这里我们先拿到jQuery产生的 XMLHttpRequest对象，为其增加 progress 事件绑定，然后再返回交给ajax使用
        xhr: function () {
            var xhr = $.ajaxSettings.xhr();
            if (onprogress && xhr.upload) {
                xhr.upload.addEventListener("progress", onprogress, false);
                return xhr;
            }
        },
        success: function (msg) {
            // ob.set(eval('(' + msg + ')'));
            //ob.set(msg);
            if (!msg.Success && msg.Message == "你未登录！")
                location.href = "../login.html";
            callBack(msg);
        }
    });
}