<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="confighelper.aspx.cs" Inherits="Web.BackStage.confighelper" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        wfs:<br />
        url:<asp:TextBox ID="url" runat="server" AutoPostBack="True" OnTextChanged="url_TextChanged"></asp:TextBox>
        <asp:Button ID="btnTextwfs" runat="server" OnClick="btnTextwfs_Click" Text="测试" />
        layers:<asp:ListBox ID="ListBox1" runat="server" Height="130px" style="margin-top: 0px" Width="179px"></asp:ListBox>
        <asp:Button ID="btnTextwfs0" runat="server" OnClick="btnTextwfs0_Click" Text="测试" />
        fields:<asp:TextBox ID="txtfields" runat="server" Rows="10" TextMode="MultiLine"></asp:TextBox>
    
    </div>
        <div>

            wmts<br />
            url:<asp:TextBox ID="url2" runat="server" Height="16px"></asp:TextBox>
            <asp:Button ID="tstUrl" runat="server" OnClick="tstUrl_Click" Text="测试" />
            layers:<asp:ListBox ID="ListBox2" runat="server" Height="130px" style="margin-top: 0px" Width="179px"></asp:ListBox>
            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="测试" />
            <asp:TextBox ID="txtfields0" runat="server" Rows="10" TextMode="MultiLine"></asp:TextBox>
    
        </div>
        arcserverquery<br />
        url:<asp:TextBox ID="txtQueryurl" runat="server" Height="16px"></asp:TextBox><asp:Button ID="Button3" runat="server"  Text="测试" OnClick="Button3_Click" /><asp:TextBox ID="txtQueryResult" runat="server" Rows="10" TextMode="MultiLine"></asp:TextBox><asp:TextBox ID="txtTranslate" runat="server" Rows="10" TextMode="MultiLine"></asp:TextBox>
        <div>

        </div>
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Visible="false" Text="配置人员xml" />
        <asp:Button ID="btnRegion" runat="server" OnClick="btnRegion_Click" Text="导入地区(云城）" Visible="false" />
        <asp:Button ID="btnRegion0" runat="server" OnClick="btnRegion0_Click" Text="导入地区(云安)"  Visible="false"/>
        <asp:Button ID="btnRegion1" runat="server" OnClick="btnRegion1_Click" Text="导入地区(荔湾)"  Visible="false"/>
        <br />
        <asp:TextBox ID="ordimage" runat="server"></asp:TextBox>
        <asp:TextBox ID="destimage" runat="server"></asp:TextBox>
        <asp:Button ID="zipimage" runat="server" OnClick="zipimage_Click" Text="压缩图片" />
        <asp:DropDownList ID="ddlType" runat="server">
            <asp:ListItem>geoserver</asp:ListItem>
            <asp:ListItem Selected="True">arcserver</asp:ListItem>
        </asp:DropDownList>
    </form>
</body>
</html>
