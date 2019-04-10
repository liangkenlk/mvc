
//加载专题图层
function themeList() {
    var url = "server/MapHandler.ashx?method=getThemeList";
    j({}, url, themeListCallBack);

}


var themeConfig;//存theme表的数据

function themeListCallBack(re) {
    re = JSON.parse(re.data); // --> parse error

    themeConfig = re;

    for (var i = 0; i < re.length; i++) {

        if (re[i].WFSonly != true) {
            
            var wmtsLayer1 = CreatWMTSlayer(re[i].wmts_tileMatrixSet, re[i].wmts_format, re[i].wmts_style, re[i].wmts_url, re[i].wmts_layer, re[i].wmts_type, re[i].name, re[i].wmts_levels, re[i].themeId, re[i].wmts_type);
            myMap.addLayer(wmtsLayer1);
            wmtsLayer1.setVisibility(false);
        }
        else {
            var layerDefinition = {
                "geometryType": "esriGeometryPolygon",
                "fields": [{
                    "name": "BUFF_DIST",
                    "type": "esriFieldTypeInteger",
                    "alias": "Buffer Distance"
                }]
            };
            var featureCollection = {
                layerDefinition: layerDefinition,
                featureSet: null
            };

            var wfslayer = new esri.layers.FeatureLayer(featureCollection, {
                mode: esri.layers.FeatureLayer.MODE_SNAPSHOT
            });
            wfslayer.id = re[i].themeId;
            myMap.addLayer(wfslayer);
            wfslayer.setVisibility(false);
        }
        //themeData += wmtslayerJsonArray[i].wmts_layer + "," + wmtslayerJsonArray[i].name + "," + wmtslayerJsonArray[i].wmts_type + "," + wmtslayerJsonArray[i].themeId + "," + wmtslayerJsonArray[i].groupname +","+wmtslayerJsonArray[i].wfs_url+","+wmtslayerJsonArray[i].wfs_layer+ ";"
    }
    getThemeListByConfig(re);

}



//弹出地图服务菜单窗口
function getThemeListByConfig1(re) {

    
    var htmlStr = "";
    for (var i = 0; i < re.length; i++) {
        var id = re[i].themeId;
        var name = re[i].name;
       

        htmlStr += "<li><div class='zt_text'><input name='subjectThemeCbx' id='" + id + "' type='checkbox' onclick=subjectThemeClickCbx('" + id + "')><a>" + name + "</a></div><div class='zt_kj'><input id='range~" + id + "' type='range' min='0' max='10' value='10' /></div></li>";

    }
    htmlStr += "</ul>";



    $("#centzt_list").html(htmlStr);

    for (var i = 0; i < re.length; i++) {

        

        //拖动活动条时修改图层透明度事件
        var key = "range~" + re[i].themeId;
        var x = document.getElementById(key);
        if (x == null)
            continue;
        x.onchange = function () {

            setLayerOpacity(this.id.split("~")[1], this.value / 10);
        };


    }

}

function getThemeListByConfig(re)
{
    $('#centzt_list').tree({
        data: re,
        loadFilter: themeTreeFilter,
        //onSelect: walltreeselect,
        formatter: themeTreeFormatter,
        lines: true,
        checkbox: true,
        cascadeCheck: false,
        onlyLeafCheck:true,
        onCheck: ThemeTreeOnCheck,
        onSelect: treeSelect
    });
}

function treeSelect(node)
{
    $('#centzt_list').tree("check", node.target);
}

function themeTreeFilter(data)
{
    var treedata = [];
    for (var i = 0; i < data.length; i++) {
       var rootnode =  findGroupNode(treedata, data[i].groupname);
        if (rootnode==null) {
            rootnode = {};
            rootnode["id"] = data[i].themeId+"root";
            rootnode["text"] = data[i].groupname;
            rootnode["state"] = "open";
            //rootnode["attributes"] = data[i];
            rootnode["iconCls"] = "";
            treedata.push(rootnode);
        }
        if (rootnode["children"] == null)
            rootnode["children"] = [];
        var childdata = {};
        childdata["id"] = data[i].themeId;
        childdata["text"] = data[i].name;
        childdata["state"] = "open";
        childdata["attributes"] = data[i];
        rootnode["children"].push(childdata);
        
    }
    return treedata;
}
function findGroupNode(treedata,groupname)
{
    for (var i = 0; i < treedata.length;i++)
    {
        if (treedata[i].text == groupname)
        {
            return treedata[i];
        }
    }
    return null;
}

function themeTreeFormatter(node) {
    if (node.attributes != null) {
        var str = "<div style='width:200px;height:50px'><span>" + node.text + "</span><a style='float:right;color:blue' onclick=setThemeWin(this,'" + node.id + "')>设置</a></div>";
        //str += '<br><input class="easyui-slider" value="12" style="width:100px,height:20px" data-options="showTip:false">';
        return str;
    }
    return node.text;
}
var setingId;
function setThemeWin(that,id)
{
    var theme;
    for (var i in themeConfig) {
        if (themeConfig[i].themeId == id)
            theme = themeConfig[i]
    }
    var x = getAbsoluteLeft(that);
    var y = getAbsoluteTop(that);
    //$("#aniframe").attr("src", "themeset.html?id=" + id);
    $('#anlyseWin').window({
        width: 300,
        height: 220,
        modal: false,
        maximizable: false,
        minimizable: false,
        collapsible: false,
        draggable: false,
        resizable: false,
        border: false,
        shadow:false,
        top: y,
        left: x + 30,
        title: theme.name+" 专题设置"
    });
    AnalysisResultWin = $('#anlyseWin').window("open");
   
    setingId = id;
    initThemeSet();
}

function initThemeSet() {
    //document.getElementById("slider").value = getOpacity(setingId) * 100;
    $("#slider").slider("setValue", getOpacity(setingId) * 100);
    if (setingId == tempId) {
        document.getElementById("check").checked = true;
    }
    else
        document.getElementById("check").checked = false;
    document.getElementById("table").style.display = "";
    for (var i = 0 ; i < themeConfig.length; i++) {
        if (themeConfig[i].themeId == setingId) {
            if (themeConfig[i].analyse == true)
            {
                document.getElementById("analyse").style.display = "";
            }
            else
                document.getElementById("analyse").style.display = "none";
            if (themeConfig[i].wmts_type == "arcserverquery") {
                document.getElementById("Example").style.display = "";
                
            }
            else
                document.getElementById("Example").style.display = "none";
            return;

        }
        

    }

}
function themeOnchange() {

    setLayerOpacity(setingId, this.value / 100);
}
function searchChange() {
    initKeyWordSelect(setingId);
}
//获取控件左绝对位置

function getAbsoluteLeft(o) {
    
    oLeft = o.offsetLeft
    while (o.offsetParent != null) {
        oParent = o.offsetParent
        oLeft += oParent.offsetLeft
        o = oParent
    }
    return oLeft
}
//获取控件上绝对位置
function getAbsoluteTop(o) {
    oTop = o.offsetTop;
    while (o.offsetParent != null) {
        oParent = o.offsetParent
        oTop += oParent.offsetTop  // Add parent top position
        o = oParent
    }
    return oTop
}


//图层透明度管理 点击复选框
function loadThemeLayer(id, type, value) {
    var wmtslayer = myMap.getLayer(id);
    if (type) {
        //wmtslayer.setOpacity(value);
        subjectThemeClickBox(id, type);
    } else {
        //wmtslayer.setOpacity(0);
        subjectThemeClickBox(id, type);
    }
}

var postParamofAnalysis;
var tempId;
//点击图层复选框事件  专题图层
function subjectThemeClickCbx(id) {

    var type = $("#"+id)[0].checked;

    tempId = id;

    
    //var layerIds = myMap.layerIds;
    //for (var i in layerIds) {
    //    var layer = myMap.getLayer(layerIds[i])
    //    if (layer == null)
    //        continue;
    //    if (id == layer.id && layer != undefined && type == true) {
    //        layer.setVisibility(true);
    //        initKeyWordSelect(id);
    //        DrawWFS(id);
    //    }
    //    if (id == layer.id && layer != undefined && type == false) {
    //        layer.setVisibility(false);
    //        tempId = null;
    //    }
    //}

    var layer = myMap.getLayer(id);
        if (layer != undefined && type == true) {
            layer.setVisibility(true);
            initKeyWordSelect(id);
            DrawWFS(id);
        }
        if (layer != undefined && type == false) {
            layer.setVisibility(false);
            tempId = null;
        }
        myMap.onClick = tempEvent;
        //myMap.on("click", function (evt) {
        //    clickSearch(evt);
        //});
        myMap.on("click", clickSearch);

}

function ThemeTreeOnCheck(node,type)
{
    var id = node.id;
    tempId = id;
    var layer = myMap.getLayer(id);
    if (layer != undefined && type == true) {
        layer.setVisibility(true);
        initKeyWordSelect(id);
        DrawWFS(id);
    }
    if (layer != undefined && type == false) {
        layer.setVisibility(false);
        if (id == tempId) {
            tempId = null;
            myMap.onClick = tempEvent;
        }
    }
    //showExample();

}

function DrawWFS(id)
{
    for (var i in themeConfig) {
        if (themeConfig[i].themeId == id)
        {
            if (themeConfig[i].WFSonly == true)
            {
                if (themeConfig[i].wmts_type == "arcserver")
                {
                    var layer = myMap.getLayer(tempId);
                    //layer.clear();
                    //layer.setVisibility(true);
                    myMap.removeLayer(layer);
                    layer = new esri.layers.ArcGISDynamicMapServiceLayer(themeConfig[i].wfs_url.replace("/WFSServer", "").replace("/services", "/rest/services"), {id:"XXdddX"});
                    layer.id = tempId;
                    myMap.addLayer(layer);
                }
                else if (themeConfig[i].wmts_type == "arcserverquery") {
                    var layer = myMap.getLayer(tempId);
                    //layer.clear();
                    //layer.setVisibility(true);
                    myMap.removeLayer(layer);
                    layer = new esri.layers.ArcGISDynamicMapServiceLayer(themeConfig[i].wmts_url, { id: "XXdddX" });
                    layer.id = tempId;
                    myMap.addLayer(layer);
                }
                else
                {
                    var url = "server/WFSHandler.ashx?method=DrawWFS";
                    j({ themeId: id }, url, DrawWFSCallBack);
                }
            }
        }
    }
}

function DrawWFSCallBack(re)
{
    var theme;
    for (var i in themeConfig) {
        if (themeConfig[i].themeId == tempId)
            theme = themeConfig[i]
    }
    var layer = myMap.getLayer(tempId);
    layer.clear();
    layer.setVisibility(true);
    for (var i = 0; i < re.length; i++)
    {
        if (re[i].geotype == "Point")
        {
            var latlngdata = LatlngParse(re[i].latlng, "point")
            addPointGeometry(latlngdata, null, null, layer, "images/point/" + theme.name + ".png");
        }
        if (re[i].geotype == "Polygon") {
            var latlngdata = LatlngParse(re[i].latlng, "polygon")
            addPolygonGeometry(latlngdata, null, null, null, layer, 0.5);
        }
    }
}

function initKeyWordSelect(id) {
    var theme;
    tempId = id;
    for (var i in themeConfig)
    {
        if (themeConfig[i].themeId == id)
            theme = themeConfig[i].wfs_searchfields;
    }
    //专题下拉框
    var select = $("#fields");
    select.html("");
    for (var i in theme.split(";")) {
        //if(fieldList[i]["@attributes"].type=="string")
        if (i == 'contains' || i == 'findById')
            continue;
        var field = theme.split(";")[i].split(",");
        select.append("<option value='" + field[1] + "'>" + field[0] + "</option>");
    }
    //myMap.onClick = null;
    //myMap.on("click", function (evt) {
    //    clickSearch(evt);
    //});
    //myMap.onClick = null;
    if (tempEvent==null)
    tempEvent = myMap.onClick;
    myMap.onClick =clickSearch;
}
var evtPoint;
function clickSearch(param) {
    var p1 = myMap.toMap(param.screenPoint.offset(-5, -5));
    var p2 = myMap.toMap(param.screenPoint.offset(5, -5));
    var p3 = myMap.toMap(param.screenPoint.offset(5, 5));
    var p4 = myMap.toMap(param.screenPoint.offset(-5, 5));

    evtPoint = param.mapPoint;
    var coordinateStr = p1.x + ',' + p1.y + ';' + p2.x + ',' + p2.y + ';' + p3.x + ',' + p3.y + ';' + p4.x + ',' + p4.y + ';' + p1.x + ',' + p1.y;
  
        var url = "server/wfsHandler.ashx?method=clickSearch";
        j({ themeid: tempId, coordinateStr: coordinateStr }, url, clickSearchCallBack);


}
//关键字搜索专题
function themesSearch() {
    if (tempId == null) {
        $.messager.alert("操作信息", "请先选择专题。。");
       
        return;
    }
    var field = $("#fields").find("option:selected").val();
    var keyword = $("#Themekeyword").val();
    if (keyword == '') {
        $.messager.alert("操作信息", "请输入专题信息。。");
    
        return;
    }
    $.messager.progress({ text: '搜索中，请稍后...' });
    var ztDIV = $('#centzt_list').hide();
    var searchDIV = $('#SeachList').show();
    var url = "server/wfsHandler.ashx?method=KeywordSearch";
    j({themeId:tempId,field:field,keyword:keyword}, url, themesSearchCallBack);


}

function themesSearchCallBack(re)
{

    if (re.length==0) {
        $.messager.show({ title: "温馨提示", msg: "未能查询到专题相关信息。。。" });
        $.messager.progress('close');
        return;
    } else {
        
        pageSize = 10;
        pageNumber = 1;
        themeQueryResult = re;
        BindThemeQueryResult();
    }
}

var pageSize = 10;
var pageNumber = 1;
var themeQueryResult;
function BindThemeQueryResult() {
    
    var namefield;
    for (var i in themeConfig)
    {
        if (themeConfig[i].themeId == tempId)
            namefield = themeConfig[i].wfs_namefield;
    }
    var template = "<li><a href='javascript:showInfoWindow(@index)' style='cursor:hand'>@field</a></li>";

  
    $('#ul').html("");
    var start = pageSize * (pageNumber - 1);

    var htmls = "";
    var polygonRings = new Array();//jiaru
    for (var i = start; i < themeQueryResult.length && i < start + pageSize; i++) {
        if (!themeQueryResult[i]) {
            $.messager.progress('close');
            $.messager.alert('Warning', "主要字段" + firstConfigField + "不存在,需要配置");
            return;
        }
        if (themeQueryResult["geotype"] == "Point") {

  
            var data = LatlngParse(themeQueryResult[i]["latlng"], "point");
            //addThemePointGeometry(pointRings, anlysisDrawPolygon, "0,255,0");

            addPointGeometry(data, null, "0,255,0", null, anlysisDrawPolygon);
        } else {
            var data = LatlngParse(themeQueryResult[i]["latlng"], "polygen");
            //addThemePolygonGeometry(data, anlysisDrawPolygon, "0,255,0");
            addPolygonGeometry(data, null, "0,255,0", null, anlysisDrawPolygon);
        } // end for
        myMap.addLayer(anlysisDrawPolygon);
        var html = template.replace("@field", themeQueryResult[i][namefield.split(',')[0]]).replace('@index', i);
        $('#ul').append(html);
    }

    $('#paging').pagination({
        total: themeQueryResult.length,
        pageSize: 10,
        onSelectPage: SelectPage,
        layout: ['links'],
        links: 8,
        displayMsg: ""
    });
    $.messager.progress('close');
}

//专题分页
function SelectPage(pageNumber, pageSize) {
    window.pageSize = pageSize;
    window.pageNumber = pageNumber;
    BindThemeQueryResult();
}
//专题返回列表
function unsearch() {
    var ztDIV = $('#centzt_list').show();
    var searchDIV = $('#SeachList').hide();
    clearAllThemeLayer();
    myMap.infoWindow.hide();
}
function showInfoWindow(i) {
    var infos = themeQueryResult;
    
    var strPolygon = themeQueryResult[i]["latlng"];
    var CurPos = getPolygenCenter(strPolygon);
    removeFlashGeo();
    flashGeo(strPolygon);
    var divStr = GetInfoWindowContent(infos[i]);
    myMap.infoWindow.setTitle("详细信息");
    myMap.infoWindow.setContent(divStr);
    myMap.infoWindow.show(CurPos);
    //myMap.infoWindow.show();
   

    myMap.centerAndZoom(CurPos, 13 - startLevel);
}
function getPolygenCenter(strPolygon) {

    
    var polygonJson = { "rings": LatlngParse(strPolygon, "polygon"), "spatialReference": myMap.spatialReference };
    var polygon = new esri.geometry.Polygon(polygonJson);
    var p = polygon.getCentroid();
    var CurPos = new esri.geometry.Point(Math.abs(p.x), Math.abs(p.y),myMap.spatialReference);
    return CurPos;
}

function getNowTheme()
{
    var theme;
    for (var i in themeConfig) {
        if (themeConfig[i].themeId == tempId)
            theme = themeConfig[i];
    }
    return theme;
}
//显示专题点查询结果
function clickSearchCallBack(result) {
    //var theme = getNowTheme();
    //if (theme.wmts_type == 'arcserverquery')//说明是arcserverquery
    //{
    //    var latlng = LatlngStringfy(result.features[0].geometry.rings);
    //        try {

    //            var strGeo = LatlngStringfy(result.features[0].geometry.rings);
    //            flashGeo(strGeo);
    //            myMap.addLayer(anlysisDrawPolygon);

    //        }


    //    catch (e) { removeFlashGeo(); }

    //        var dic = result.features[0].attributes;
    //        var showfield = theme.wfs_showfields.split(';');
    //        //for (var i = 0; i < showfield.length; i++)
    //        //{
    //        //    result[fieldAliases]
    //        //    var fkey =showfield[i].split(',')[0];
    //        //    var fvalue =showfield[i].split(',')[1];
    //        //    if (result.features[0].attributes[fvalue]!=null)
    //        //    {
    //        //        dic[fkey] = result.features[0].attributes[fvalue];
    //        //    }
    //        //}

        
    //    var divStr = GetInfoWindowContent(dic);
    //    var CurPos = new esri.geometry.Point(evtPoint.x, evtPoint.y, myMap.spatialReference);
    //    myMap.infoWindow.setTitle("详细信息");
    //    myMap.infoWindow.setContent(divStr);
    //    myMap.infoWindow.show(CurPos);
    //    myMap.infoWindow.show();
    //    $('.maximize').hide();

    //}
    //else {

        try {

            var strGeo = result["latlng"];
            flashGeo(strGeo);
            myMap.addLayer(anlysisDrawPolygon);
            result["latlng"] = null;
            delete result["latlng"];
        }


        catch (e) { removeFlashGeo(); }
        var divStr = GetInfoWindowContent(result);
        var CurPos = new esri.geometry.Point(evtPoint.x, evtPoint.y, myMap.spatialReference);
        myMap.infoWindow.setTitle("详细信息");
        myMap.infoWindow.setContent(divStr);
        myMap.infoWindow.show(CurPos);
        myMap.infoWindow.show();
        //隐藏进度条

        $('.maximize').hide();
    }
//}
//取消高亮图斑
function removeFlashGeo() {
    for (var i = 0; i < anlysisDrawPolygon.graphics.length; i++) {
        if (anlysisDrawPolygon.graphics[i].attributes != null && anlysisDrawPolygon.graphics[i].attributes.id == "XXX")
            anlysisDrawPolygon.remove(anlysisDrawPolygon.graphics[i]);
    }
    myMap.infoWindow.hide();
}
//高亮图斑
function flashGeo(strPolygon)//空格隔开经纬度
{
    removeFlashGeo();
    strPolygon = trim(strPolygon);
    strPolygon = strPolygon.replace(/[\n]/ig, '');
    strPolygon = trim(strPolygon);
    strPolygon = strPolygon.replaceAll(' ', ';');
    strPolygon = strPolygon.replace(new RegExp(';+', "gm"), ';');
    var LatlngData = LatlngParse(strPolygon, '面');
    addPolygonGeometry(LatlngData, { id: "XXX" }, null, null, anlysisDrawPolygon, 0.5);

}
function subjectThemeClickBox(key, type) {
    var layerIds = myMap.layerIds;
    for (var i in layerIds) {
        var layer = myMap.getLayer(layerIds[i])
        if (key == layer.themeId && layer != undefined && type == true) {
            layer.setVisibility(true);
        }
        if (key == layer.themeId && layer != undefined && type == false) {
            layer.setVisibility(false);
        }
    }
}

// 设置图层透明度
function setLayerOpacity(layerName, value) {
    var wmtslayer = myMap.getLayer(layerName);
    wmtslayer.setOpacity(value);
}

function getOpacity(layerName)
{
    var wmtslayer = myMap.getLayer(layerName);
    return  wmtslayer.opacity;
}


//清除 所有专题图层
function clearAllThemeLayer() {
    for (var i in themeArr)
        myMap.removeLayer(themeArr[i]);
    queryToolbar.deactivate();
    anlysisDrawPolygon.clear();
}

//根据专题分类加载专题列表
function getListByGroup() {
    // 清除所有专题列表
    $(".centzt_list").html("");

    var groupname = $("#groups").val();
    var url = "/server/MapHandler.ashx?method=getThemeListByGroupName";
    j({ groupname: groupname }, url, themeListCallBack);

    if (groupname == "全部") {
        themeList();
    }
}


//查询专题
function searchTheme() {
    var themeKeyWord = $("#Themekeyword").val();
    var groupname = $("#group").val();





    var url = "/server/MapHandler.ashx?method=themeQuery";
    j({ themeKeyWord: themeKeyWord, groupname: groupname }, url);


}

/********************************************************叠加分析**********************************************/








