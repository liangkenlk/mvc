<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="sjgzPic.aspx.cs" Inherits="Web.sjgzPic" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
 
    <link href="jquery-easyui-1.4.4/dist/main.css" rel="stylesheet" />
    <script src="jquery-easyui-1.4.4/dist/jquery-3.3.1.slim.min.js"></script>

    <link href="jquery-easyui-1.4.4/dist/viewer.min.css" rel="stylesheet" />
    <script src="jquery-easyui-1.4.4/dist/viewer.min.js"></script>
    <script>
        $(function () {
           
            $('#viewer').viewer()
        }
            );
    </script>
    <style>

        div {
            width: 100%;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Literal runat="server" ID="lt"></asp:Literal>
    </div>
    </form>
</body>
</html>
