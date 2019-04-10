//加载专题图层
function getAnalyseList() {
    var url = "server/MapHandler.ashx?method=getThemeList";
    j({}, url, getAnalyseListCallBack);

}


var analyseList = [];
function getAnalyseListCallBack(re)
{
    analyseList = JSON.parse(re.data);
    var htmlStr = "<ul>";
  
    for (var i = 0; i < analyseList.length; i++) {
   
        if (analyseList[i].wfs_classifyfield==null) {
            continue;
        }
        var wmtsLayer1 = CreatWMTSlayer(analyseList[i].wmts_tileMatrixSet, analyseList[i].wmts_format, analyseList[i].wmts_style, analyseList[i].wmts_url, analyseList[i].wmts_layer, "analysis", analyseList[i].name, analyseList[i].themeId + "_fx");
        myMap.addLayer(wmtsLayer1);
        wmtsLayer1.setVisibility(false);
    
        htmlStr += "<div class='tc_text'><table><tr><td><a href='#' onclick=loadAnalysisLayer('" + analyseList[i].themeId + "')><input type='radio' name='analysis' id='analysis" + analyseList[i].themeId + "' /></a>&nbsp;&nbsp;<label for='analysis" + analyseList[i].themeId + "' style='font-family:微软雅黑;font-size: 14px;'>" + analyseList[i].name + "</label></td></tr></table></div>";
        htmlStr += "<div class='tc_kj'><input id='range~" + analyseList[i].themeId + "_fx' type='range' min='0' max='10' value='10'/></div>";
      
    }
    htmlStr += "</ul>";
    $(".centfx_list").html(htmlStr);
    for (var i = 0; i < analyseList.length; i++) {
        var key = "range~" + analyseList[i].themeId + "_fx";
        var x = document.getElementById(key);
        if (x == null)
            continue;
        x.onchange = function () {
            //setLayerOpacity(this.id.split("~")[1],this.value/10);
            setLayerOpacity(this.id.split("~")[1], this.value / 10);
        }

    }
}

function loadAnalysisLayer(id)
{
    var type = $("#analysis" + id)[0].checked;

    tempId = id;
    var layerIds = myMap.layerIds;
    for (var i in layerIds) {
        var layer = myMap.getLayer(layerIds[i])
        if (layer == null)
            continue;
        if (id + "_fx" == layer.id && layer != undefined && type == true) {
            layer.setVisibility(true);
      
        }
        if (id + "_fx" != layer.id && layer.description == "analysis") {
            layer.setVisibility(false);

        }
    }
}

//手绘
function handPainted() {
    if (!tempId) {
        $.messager.alert('提示', '请先选择专题!');
        return;
    }
    //myMap.removeEventListener("click", clickSearch);
    myMap.onClick = tempEvent;
    
    if(window.mui)
        mui.toast('点击绘制多边形，双击结束。');
    else
        $.messager.alert('提示', '请用点击，双击结束。');
    if (AnalysisResultWin) {
        AnalysisResultWin.window("close");
    }

    toCancelPainted();
    analysisToolbar.activate(esri.toolbars.Draw.POLYGON);
    $("#btnEndDraw").show();
}

//取消
function toCancelPainted() {
    if (anlysisDrawPolygon) {
        anlysisDrawPolygon.clear();
    }
    if (AnalysisResultWin) {
        AnalysisResultWin.window("close");
    }
}

function EndDraw()
{
    analysisToolbar.finishDrawing();
    endSelect();
}

//手绘结束
function analysisEnd(evt) {
    analysisToolbar.deactivate();
    var geometry = evt.geometry;
    //在图层上添加手绘面
    var symbol = new esri.symbol.SimpleFillSymbol(esri.symbol.SimpleFillSymbol.STYLE_SOLID,
            new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID,
            new dojo.Color([255, 0, 255]), 2), new dojo.Color([255, 0, 255, 0.25])
        );
    var graPolygon = new esri.Graphic(geometry, symbol, null, null);
    anlysisDrawPolygon.add(graPolygon);
    myMap.addLayer(anlysisDrawPolygon);

    var latlon = geometry.rings;
    $.messager.progress({ title: "请稍后", msg: "分析中..." });
    drawFeatureEndFunc(latlon);
    
    
}
var latlonAll = "";
function drawFeatureEndFunc(latlon) {
   latlonAll = LatlngStringfy(latlon,"面");
   $("#btnEndDraw").hide();
   movePointList = [];
    var url = "server/wfsHandler.ashx?method=OverlayAnalysis";
    data = { "tempId": tempId, "coordinateStr": latlonAll };

    j(data, url, drawFeatureEndFuncCallBack);

}


var jsonCal = [];
var field = "";
function drawFeatureEndFuncCallBack(result) {
    
    var theme = getNowTheme();

  

    //手绘面的面积
    $.messager.progress("close");
    if (result.length == 0) {
        $.messager.alert('提示', '此区域没有分析到数据!');
        return;
    }
    var jsonFeatures = result;
    jsonCal = {};
    field = jsonFeatures[0].key;
    for (var j = 0; j < jsonFeatures.length; j++) {
        
        //var keyName = jsonFeatures[j].attribute[postParamofAnalysis.split(",")[2]];
        var keyName = jsonFeatures[j].value;

        var data = LatlngParse(jsonFeatures[j].latlng,"polygon");
        addPolygonGeometry(data, null, "0,255,0", null, anlysisDrawPolygon);
        var polygonJson = { "rings": data, "spatialReference": myMap.spatialReference };
        var eachPolygon = new esri.geometry.Polygon(polygonJson);
        polygonJson = { "rings": LatlngParse(latlonAll, "polygon"), "spatialReference": myMap.spatialReference };
        var orPolygon = new esri.geometry.Polygon(polygonJson);

        var intersectPolygon = Engine.intersect(eachPolygon, orPolygon);
		if(intersectPolygon==null)
			continue;

        //		     console.info("areaValue: " + areaValue);
        //var intersectArray = [];
        //for (var i = 0; i < intersectPolygon.rings[0].length; i++) {
        //    var p = new TLngLat(intersectPolygon.rings[0][i][0], intersectPolygon.rings[0][i][1]);
        //    intersectArray.push(p);
        //}
        var intersectArray = [];
        for (var i = 0; i < intersectPolygon.rings[0].length; i++) {
            var p = new TLngLat(intersectPolygon.rings[0][i][0], intersectPolygon.rings[0][i][1]);
            intersectArray.push(p);
        }
        var intersectArea = (polygonTool.getArea(intersectArray)).toFixed(3);
       
        if (jsonCal[keyName]) {
            var tempvalue = parseFloat(jsonCal[keyName]);
            jsonCal[keyName] = tempvalue + parseFloat(intersectArea);
        } else {
            jsonCal[keyName] = parseFloat(intersectArea);
        }

        //每次都减去交集的面积，将会得到其他的面积
        //originalArea = parseFloat(originalArea) - parseFloat(areaValue);
    }
    //var nameValue = postParamofAnalysis.split(",")[3];
    //console.info("nameValue: " + nameValue);
    //jsonCal["其他"]=Math.abs(originalArea);
    polygonTool.close();
    showAnalysisResultWin(jsonCal);
    myMap.onClick = clickSearch;
}





//显示叠加分析结果
var AnalysisResultWin;
function showAnalysisResultWin(jsonCal) {
    $("#iframe").attr("src", "analyse.html"); 
    $('#win').window('setTitle','分析结果');
    $('#win').window('open');
}

function readFilePolygons(strPolygons) {
    
    $('#win').window('close');
    $('#win').attr("src", "import.html");
    var strRings = strPolygons.split('@');
    for (var i = 0; i < strRings.length; i++) {
        var LatlngData = LatlngParse(strRings[i], '面');
        addPolygonGeometry(LatlngData, null, null, null, anlysisDrawPolygon, 0.2);
    }
    myMap.addLayer(anlysisDrawPolygon);
    //myMap.onClick=null;
    // myMap.on("click",anerlys);
    $.messager.progress({ title: "请稍后", msg: "分析中..." });
    drawFeatureEndFunc(LatlngData);
}

function importTxt() {
    if (!tempId) {
        $.messager.alert('提示', '请先选择专题。。。');
        return;
    }
    $("#iframe").attr("src", "import.html");
    $('#win').window('open');
    AnalysisResultWin.window('close');
}
//拖动手绘
var panEndEvent;
function movePainted()
{
    if (!tempId) {
        $.messager.alert('提示', '请先选择专题!');
        return;
    }
    $('#anlyseWin').window("close");
    myMap.onClick = tempEvent;
    //$.messager.alert('提示', '请用手指在地图上绘制多边形。');
    //myMap.on("mouse-drag,mouse-drag-end,mouse-move,pan", showPointList);
    $("#lblSelectPoint").show();
    panEndEvent = myMap.onExtentChange;
    myMap.onExtentChange = showPointList;
    $("#divPostion").show();
    var divPostion = document.getElementById("divPostion");
    var divmap = document.getElementById("map");
    divPostion.style.left = parseInt(divmap.clientWidth / 2 - 20) + 'px';
    divPostion.style.top = parseInt(divmap.clientHeight / 2 - 90) + 'px';
}
var movePointList = [];
function selectPoint() {

    var x = myMap.extent.getCenter().x;
    var y = myMap.extent.getCenter().y;
    movePointList.push([x, y]);
    $("#lblSelectPoint").text("已选择" + (movePointList.length) + "个点");
    if(movePointList.length>1)
        $("#btnEndDraw").show();

}
function showPointList(e) {
    var x = myMap.extent.getCenter().x;
    var y = myMap.extent.getCenter().y;



    var polygonList = movePointList.slice();
    polygonList.push([x, y]);
    polygonList.push(movePointList[0]);
    if (polygonList.length > 2) {
        anlysisDrawPolygon.clear();
        addPolygonGeometry([polygonList], { id: "xxx" }, null, null, anlysisDrawPolygon);

    }
    myMap.addLayer(anlysisDrawPolygon);
}

function endSelect() {
    var x = myMap.extent.getCenter().x;
    var y = myMap.extent.getCenter().y;
    var polygonList = movePointList.slice();
    polygonList.push([x, y]);
    polygonList.push(movePointList[0]);
    movePointList = [];
    if (polygonList.length > 2) {
        anlysisDrawPolygon.clear();
        addPolygonGeometry([polygonList], { id: "xxx" }, null, null, anlysisDrawPolygon);
    }
    $("#divPostion").hide();
    $("#lblSelectPoint").hide();
    $("#btnEndDraw").hide();
    $("#lblSelectPoint").text("请选择点");
    myMap.onExtentChange = panEndEvent;
    $.messager.progress({ title: "请稍后", msg: "分析中..." });
    drawFeatureEndFunc([polygonList]);
}