

function MenuSelect(node)
{
    if (node.url == null)
        return;
   
    if ($('#tabs').tabs('exists', node.text)) {
        $('#tabs').tabs('select', node.text);
        var tab = $('#tabs').tabs('getSelected');
        var str = OpenMenuUrl(node.url);
        $('#tabs').tabs('update', {
            tab: tab,
            options: {
                title: node.text,
                content: str,  // the new content URL
                closable: true
            }
        });
    }
    
    else {
        var str = OpenMenuUrl(node.url);
        $('#tabs').tabs('add', {
            title: node.text,
            content: str,
            closable: true
        });
    }
}

function OpenMenuUrl(url)
{
    var str = "<iframe frameborder=0 scrolling='no' width=100% height=100% src='" + url + "'></iframe>";
    return str;
}
$(function () {
    $("#menu").tree({
        onSelect: MenuSelect
    });
    var tab = $('#tabs').tabs('getTab', 0);
    var str = OpenMenuUrl("userlist.html");
    $('#tabs').tabs('update', {
        tab: tab,
        options: {
            title: "用户管理",
            content: str,  // the new content URL
            closable: true
        }
    });


}

);