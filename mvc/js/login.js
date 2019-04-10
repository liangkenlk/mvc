/**Ext.onReady(function() {
            var LoginForm = new Ext.FormPanel({
                //el: 'hello-tabs',
                id: 'LoginForm',
                name: 'LoginForm',
                layout: 'fit',
                autoTabs: true,
                activeTab: 0,
                //deferredRender: false,
                //waitMsgTarget: true, 
                border: false,
                items: {
                    xtype: 'tabpanel',
                    activeTab: 0,
                    defaults: { height: 100, bodyStyle: 'padding:20px' },
                    items: [{
                        title: '用户登录',
                        contentEl: 'loginInfo',
                        layout: 'form',
                       defaults: { width: 160 },
               //         labelWidth: 73,
                        labelAlign: "right",
                        defaultType: 'textfield',
                        border: false,
                        items: [{
                            id: 'loginName',
                            fieldLabel: '账 号',
                            name: 'loginName',
                             labelWidth:40,     //Label的宽度
                              fieldWidth: 10,     //输入框宽度
                            style: 'font-size: 15px'
                        }, {
                            fieldLabel: '密 码',
                            id: 'UserPwd',
                            name: 'UserPwd',
                            labelWidth:40,     //Label的宽度
                            fieldWidth: 20,     //输入框宽度
                            style: 'font-size: 15px',
                            inputType: 'password'
                        }
                        ]
                    }]
                    }
                });
                
                
                
                var win = new Ext.Window({
                    title: '系统登录',
                    //el: 'hello-win',
                    layout: 'fit',
                    width: 330,
                    height: 200,
                    closeAction: 'close',
                    plain: true,
                    frame: true,
                    modal: true,
                    collapsible: false,
                    draggable: true, //是否允许拖动
                    maximizable: false,
                    closable: false,
                    resizable: false,
                    bodyStyle: 'padding:0px 0px 0px 0px;',
                    items: LoginForm,
                    //2010.5.24 jr 设置enter快捷键
                    keys: {
                        key: [13],
                        fn: function() { login(); },
                        scope: this
                    },
                    defaultButton: 0,
                    buttonAlign: 'center',
                    buttons: [{
                        text: '确定',

                        handler: login
                    }, {
                        text: '重置',
                        handler: function() {
                            win.getComponent('LoginForm').form.reset();
                        }
                    }]
                    }

            );
                    win.show();

                    function login() {
                        if (LoginForm.form.isValid()) {
                        	var loginName = Ext.getCmp("loginName").lastValue;
                        	var userPwd = Ext.getCmp("UserPwd").lastValue;
                        	if(loginName==""||loginName==null){
                        		Ext.Msg.alert("提示","登录账号不能为空！");
                        		return false;
                        	}
                        	if(userPwd==""||userPwd==null){
                        		Ext.Msg.alert("提示","登录密码不能为空！");
                        		return false;
                        	}
                        	
                            LoginForm.form.doAction('submit', {
                                url: 'login.ct',
                                waitMsg: '正在验证用户...',
                                waitTitle: '提示:',
                                method: 'post',
                                params: {'user.loginName':loginName,'user.loginPwd':userPwd},
                                success: function(form, action) {
                                	if (action.result.success == true) {
                                        var mymask = new Ext.LoadMask(Ext.getBody(), { msg: "正在登录，请稍后......" });
                                        mymask.show();
                                        window.location.href = "index.ct";
                                    }
                                },
                                failure: function() {
                                    Ext.Msg.alert('提示:', '登录失败！请检查您输入的用户名、密码是否正确！');
                                    Ext.getCmp("loginName").setValue();
                                    Ext.getCmp("UserPwd").setValue();
                                    Ext.getCmp("loginName").focus();
                                }
                            });
                        }



                    }
                });**/

$(document).ready(function(){
	$("#content").height($(window).height()-$("#header").height()-$("#footer").height()-1);
	window.onresize=function(){
		$("#content").height($(window).height()-$("#header").height()-$("#footer").height()-1);
	}
	$("#username").live({focus:function(){
		$(this).removeClass("required-number");
	},blur:function(){
		if($.trim($(this).val())==""){
			$(this).addClass("required-number");
		}
	}})
	$("#password").live({focus:function(){
		$(this).removeClass("required-password");
	},blur:function(){
		if($.trim($(this).val())==""){
			$(this).addClass("required-password");
			
		}
	}})

	$(".login_but").live({click:function(){
		if($.trim($("#username").val())==""){
            $("#username").addClass("required-number");
            alert("用户名或密码不能为空！！！");
        }
        else if($.trim($("#password").val())==""){
            $("#password").addClass("required-password");
            alert("用户名或密码不能为空！！！");
        }
        else{
      //点击 登陆
     //checkUserPwd checkUserName
     
     var data = { 
        "user.loginName": $('#username').val(), 
        "user.loginPwd": $('#password').val()
    }
//提交数据给Login.ashx页面处理 
$.post("login.ct",data,function(result){ 
if(result == "success") //登录成功 
{ 
    $("#hh").html("<div id='status'  class='succ'>正在登录中......</div>");
// 关闭模拟窗口 
window.location.href = "index.ct";
} else 
{ 
    alert("登录失败！请重试"); 
}                           
},"text")

}

}})
	
	$("#username").focus();
})





/**

		 


	

$(document).ready(function(){ 

//关键的代码 
$("#btnLogin").click(function(){ 
if(checkUserName() && checkUserPwd()) 
{ 
var data = { 
"user.loginName": $('#txtUserName').val(), 
"user.loginPwd": $('#txtUserPwd').val(), 
}; 
alert(data);
//提交数据给Login.ashx页面处理 
$.post("login.ct",data,function(result){ 
alert(result);
if(result == "success") //登录成功 
{ 
alert("登录成功！您可以进行其他操作了！"); 
// 关闭模拟窗口 
window.parent.window.jBox.close(); 
 window.location.href = "index.ct";
} 

else 
{ 
alert("登录失败！请重试"); 
} 
}); 
} 
else 
{ 
checkUserName(); 
checkUserPwd(); 
checkCheckCode(); 
} 
}); 
}); **/

//check the userName 
/**function checkUserName(){ 


if($("#username").val().length == 0){
$("#status").addClass("errors"); 
$("#status").next("span").css("color","red").text("*用户名不为空"); 
return false; 
} else { 
var reg = /^\d{9}$/; 
if(!reg.test($('#username').val())) { 
$('#username').next("span").css("color","red").text("*正确的格式 如：030602888"); 
return false; 
} else { 
$("#username").next("span").css("color","red").text(""); 
return true; 
} }
 } 
//check the pwd 
function checkUserPwd() { 
if($('#password').val().length == 0) { 
$('#password').next("span").css("color","red").text("*密码不为空"); 
return false; 
} 
else { 
$('#password').next("span").css("color","red").text(""); 
return true; 
} 
}*/


