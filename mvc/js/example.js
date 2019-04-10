
//var exampleList = "土地利用现状,土地利用规划,城市总体规划".split(",");
var colorwin;
function showExample(id) {

    closeExample();
    var theme;
    for (var i in themeConfig) {
        if (themeConfig[i].themeId == tempId)
            theme = themeConfig[i]
    }

    var url = theme.wmts_url + "/legend?f=pjson";
    j({}, url, showExampleCallBack);
    
    //for (var i = 0; i < exampleList.length; i++) {
    //    if (exampleList[i] != theme.name)
    //        return;
    //    colorwin = $.messager.show({
    //        timeout: 0,
    //        width: 350,
    //        height: 350,
    //        title: '图例',
    //        msg: "<img style='width:100%' src='images/colorExample/" + exampleList[i] + ".png'/>"
    //    });
    //}
}

function showExampleCallBack(e)
{
    var theme;
    for (var i in themeConfig) {
        if (themeConfig[i].themeId == tempId)
            theme = themeConfig[i]
    }
    if (!e)
        return;
    var str = "";
    for (var i = 0; i < e.layers[0].legend.length; i++)
    {
        var temp = "<tr><td><img style='width:20px,height:20px' src='@src'/></td><td>@label</td></tr>";
        var src = theme.wmts_url + "/" + e.layers[0].layerId + "/images/" + e.layers[0].legend[i].url;
        var label = e.layers[0].legend[i].label;
        str += temp.replace("@src", src).replace("@label", label);
        
    }
    str = "<table>" + str + "</table>";
        colorwin = $.messager.show({
            timeout: 0,
            width: 350,
            height: 350,
            title: '图例',
            msg: str
        });

    
}

function closeExample() {
    if (colorwin != null) {
        try {
            colorwin.window('close');
            colorwin = null;

        }
        catch (err) { }

    }
}