dojo.require("dijit.layout.ContentPane");
dojo.require("esri.map");
dojo.require("esri.layers.WMTSLayerInfo");
dojo.require("esri.layers.WMTSLayer");
dojo.require("esri.layers.TileInfo");
dojo.require("esri.geometry.Extent");
dojo.require("esri.toolbars.draw");
dojo.require("esri.toolbars.edit");
dojo.require("esri.toolbars.navigation"); //地图工具
dojo.require("esri.tasks.query");
dojo.require("esri.tasks.QueryTask");
dojo.require("esri.layers.FeatureLayer");
dojo.require("esri.dijit.Scalebar");
dojo.require("esri.layers.GraphicsLayer");
dojo.require("esri.InfoTemplate");
dojo.require("dijit.registry");
dojo.require("dojo.parser");
dojo.require("dojo.domReady!");
dojo.require("esri.symbols.TextSymbol");
dojo.require("esri.symbols.Font");
dojo.require("esri.symbols.SimpleFillSymbol");
dojo.require("esri.dijit.NavigationBar");
dojo.require("esri.layers.agstiled");
dojo.require("esri.layers.agsdynamic");
dojo.addOnLoad(init);
var center = { x: sysConfig.中心经度, y: sysConfig.中心纬度 };
var startLevel = 0;

var myMap;
var myMap2;
var featureLayer;
var basemap;	//矢量图层
var annolayer;  //矢量注记
var img_c;		//影像图层
var cia_c;		//影像注记
var dgmap;	//桥头矢量地图
var dgimage;	//桥头影像地图
var themelayer_1, themelayer_2, themelayer_3;
var curentShowMapType = "vector";		//当前图层名称
//	var cityservices="vec,天地图底图,vector;cva,天地图底图注记,vector;img,天地图影像,image;cia,天地图影像注记,image;dgmap,茂名电子地图,vector;dgimage,茂名地图,image;";
var cityservices = "";
var themeData = "";
var nowView = null;	//窗口切换时的中间变量
var toolbar;
var editToolbar;
var gpsLayer;
var analysisToolbar;
//天地图 侧面 测距
var polygonTool;
var map_tianditu;
//测面 测距图层
var countyLayer;
//专题注记图层
var textLayer;
//挂图镇级图层
var wallmap4Zhenlayer;
//挂图村级图层
var wallmap4Cunlayer;
//挂图切片图层
var wallmaplayer;
var wallmapToolbar;

//选择专题面要素
var themeSelectGraphic;
//标记图层
var drawLabelLayer;
var addLabelLayer;
//叠加分析图层
var anlysisDrawPolygon;
//地名查询 点标签图层
var queryAdressLayer;
//子窗口
var childWin;
var navToolbar;
var vrLayer;
var dragToolbar;
//分屏对比 当前拖动的地图
var tempMymap;
var celianLayer;

var tempId;

var tileExtent, tileInfo, scale_JSON;

//var spatialReference = { "wkid": 4490 };
var spatialReference;// = new esri.SpatialReference(4326);
var symbol;
var token;
var tempEvent;
function init() {
    // navigation();//添加镇界导航
    //updateDiv();
    //initUI();
    //buildTree();
    symbol = new esri.symbol.SimpleFillSymbol(esri.symbol.SimpleFillSymbol.STYLE_SOLID,
            new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID,
            new dojo.Color([255, 0, 0]), 2), new dojo.Color([255, 255, 0, 0.25])
        );
    spatialReference = new esri.SpatialReference(4490);
    //map_tianditu = new TMap("map_tianditu");
    // map_tianditu=new TMap(""); 

    var config = {
        // strokeColor:"blue",	//折线颜色 
        // fillColor:"#FFFFFF",	//填充颜色。当参数为空时，折线覆盖物将没有填充效果 
        // strokeWeight:"3px",	//折线的宽度，以像素为单位 
        // strokeOpacity:0.5,	//折线的透明度，取值范围0 - 1 
        // fillOpacity:0.5	    //填充的透明度，取值范围0 - 1 
    };

    //创建测面工具对象 
    //polygonTool = new TPolygonTool(map_tianditu, config);



    var lods = [
            { "level": 0, "resolution": 0.703125, "scale": 295497593.05875003 },
            { "level": 1, "resolution": 0.3515625, "scale": 147748796.52937502 },
            { "level": 2, "resolution": 0.17578125, "scale": 73874398.264687508 },
            { "level": 3, "resolution": 0.087890625, "scale": 36937199.132343754 },
            { "level": 4, "resolution": 0.0439453125, "scale": 18468599.566171877 },
            { "level": 5, "resolution": 0.02197265625, "scale": 9234299.7830859385 },
            { "level": 6, "resolution": 0.010986328125, "scale": 4617149.8915429693 },
            { "level": 7, "resolution": 0.0054931640625, "scale": 2308574.9457714846 },
            { "level": 8, "resolution": 0.00274658203125, "scale": 1154287.4728857423 },
            { "level": 9, "resolution": 0.001373291015625, "scale": 577143.73644287116 },
            { "level": 10, "resolution": 0.0006866455078125, "scale": 288571.86822143558 },
            { "level": 11, "resolution": 0.00034332275390625, "scale": 144285.93411071779 },
            { "level": 12, "resolution": 0.000171661376953125, "scale": 72142.967055358895 },
            { "level": 13, "resolution": 8.58306884765625e-005, "scale": 36071.483527679447 },
            { "level": 14, "resolution": 4.291534423828125e-005, "scale": 18035.741763839724 },
            { "level": 15, "resolution": 2.1457672119140625e-005, "scale": 9017.8708819198619 },
            { "level": 16, "resolution": 1.0728836059570313e-005, "scale": 4508.9354409599309 },
            { "level": 17, "resolution": 5.3644180297851563e-006, "scale": 2254.4677204799655 },
            { "level": 18, "resolution": 2.68220901489e-006, "scale": 1127.23386023 },
            { "level": 19, "resolution": 1.34110450744e-006, "scale": 563.616930115 }//arc 533.1823959646343
    ];


    myMap = new esri.Map("map", { logo: false, slider: true, nav: false, sliderOrientation: "vertical", sliderStyle: "large", navigationMode: 'classic', lods: lods });
    //myMap2 = new esri.Map("map2", { logo: false, slider: true, nav: false, sliderOrientation: "vertical", sliderStyle: "large", navigationMode: 'classic', lods: lods });


    //基础图层
    //	    var basiclayer=new basicTranLayer();
    //	    myMap.setBasemap(basiclayer);

    baseMapList();

    //loadLabelList();

    myMap.centerAndZoom(new esri.geometry.Point({ "x": center.x, "y": center.y, "spatialReference": spatialReference }), 14);
    //myMap.onExtentChange = setCenter;
    //比例尺 hl
    var scalebar = new esri.dijit.Scalebar({
        map: myMap,
        scalebarUnit: "metric",
        scalebarStyle: "line"
    });
    //         dojo.connect(myMap,"onZoomEnd",function(evt){
    //        	showTDT();
    //        });

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

    featureLayer = new esri.layers.FeatureLayer(featureCollection, {
        mode: esri.layers.FeatureLayer.MODE_SNAPSHOT
    });
    //叠加分析图层
    anlysisDrawPolygon = new esri.layers.FeatureLayer(featureCollection, {
        mode: esri.layers.FeatureLayer.MODE_SNAPSHOT
    });

    //挂图图层
    wallmapLayer = new esri.layers.FeatureLayer(featureCollection, {
        mode: esri.layers.FeatureLayer.MODE_SNAPSHOT
    });
    //标记图层
    drawLabelLayer = new esri.layers.FeatureLayer(featureCollection, {
        mode: esri.layers.FeatureLayer.MODE_SNAPSHOT
    });

    addLabelLayer = new esri.layers.FeatureLayer(featureCollection, {
        mode: esri.layers.FeatureLayer.MODE_SNAPSHOT
    });

    celianLayer = new esri.layers.FeatureLayer(featureCollection, {
        mode: esri.layers.FeatureLayer.MODE_SNAPSHOT
    });
    //地名地址查询图层
    queryAdressLayer = new esri.layers.FeatureLayer(featureCollection, {
        mode: esri.layers.FeatureLayer.MODE_SNAPSHOT
    });
    //测面测距图层
    countyLayer = new esri.layers.GraphicsLayer();
    gpsLayer = new esri.layers.GraphicsLayer();
    // textLayer = new esri.layers.GraphicsLayer();

    // queryAdressLayer = new esri.layers.FeatureLayer(featureCollection, {
    // mode: esri.layers.FeatureLayer.MODE_SNAPSHOT
    // });
    vrLayer = new esri.layers.FeatureLayer(featureCollection, {
        mode: esri.layers.FeatureLayer.MODE_SNAPSHOT
    });
    //地图基本操作 工具
    navToolbar = new esri.toolbars.Navigation(myMap);
    navToolbar.on("extent-history-change", extentHistoryChangeHandler);

    if (showPointsData != null)
        showPoints(showPointsData);

}

function showSelectPoint()
{
    $("#lblSelectPoint").show();
    $("#divPostion").show();
    setCenter();
   

    
}

function setCenter()
{
    var divPostion = document.getElementById("divPostion");
    var divmap = document.getElementById("map");
    divPostion.style.left = parseInt(divmap.clientWidth / 2 - 10) + 'px';
    divPostion.style.top = parseInt(divmap.clientHeight / 2 - 10) + 'px';
}

var x, y;
function pointSelected()
{
    x = myMap.extent.getCenter().x;
    y = myMap.extent.getCenter().y;
    $("#lblSelectPoint").hide();
    $("#divPostion").hide();
    //alert(x + "," + y);
    $("#win iframe").attr("src", "CaseEdit.html");
    $("#win").window('open');
    
}

function editCaseWinOpen(id)
{
    var url = $("#win iframe").attr("src");
    $("#win iframe").attr("src","CaseEdit.html?id="+id);
    $("#win").window('open');

}

function winReload() {
    var url = $(this).find("iframe").attr("src");
    $(this).find("iframe").attr("src", url);
}
function closeEditWin() {
    $("#win").window("close");
}

function showPoints(data)
{
    if (!anlysisDrawPolygon)
        return;
    anlysisDrawPolygon.clear();
    for (var i = 0; i < data.length; i++) {
        if (data[i].X == null)
            continue;
        var d = [data[i].X, data[i].Y];
        addPointGeometry(d, { id: data[i].id }, null, anlysisDrawPolygon, "images/yhd.png");
    }
    myMap.addLayer(anlysisDrawPolygon);
}

function HightLight(data)
{
    if (anlysisDrawPolygon == undefined)
        return;
    for (var i = 0; i < anlysisDrawPolygon.graphics.length; i++) {
        if (anlysisDrawPolygon.graphics[i].attributes != null && anlysisDrawPolygon.graphics[i].attributes.id.indexOf("BIG")>0)
            anlysisDrawPolygon.remove(anlysisDrawPolygon.graphics[i]);
    }
    for (var i = 0; i < data.length; i++)
    {
        if (data[i].X == null)
            continue;
        var d = [data[i].X, data[i].Y];
        addPointGeometry(d, { id: data[i].id + "BIG" }, null, anlysisDrawPolygon, "images/yhd.png", 25);
        if (i == data.length - 1) {
            myMap.centerAndZoom(new esri.geometry.Point({ "x": parseFloat(d[0]), "y": parseFloat(d[1]), "spatialReference": spatialReference }), 14);
        }
    }
    

}

//矢量影像地图切换
var curentShowMapType = "vector";
function changeMapHandler() {
    if (this.curentShowMapType == "vector")
        curentShowMapType = "image";
    else
        curentShowMapType = "vector";

    showTDT();

}



//是否切换天地图切片
function showTDT() {

    //var layers = myMap.getLayersVisibleAtScale();
    $("#imagebtn").toggle();
    $("#vectorbtn").toggle();
    var layerIds = myMap.layerIds;
    for (var i in layerIds) {
        var layer = myMap.getLayer(layerIds[i])
        if (curentShowMapType == "image" && layer != undefined) {
            if (layer.maptype == "vector")
                layer.setVisibility(false);
            if (layer.maptype == "image")
                layer.setVisibility(true);
        }
        if (curentShowMapType == "vector" && layer != undefined) {
            if (layer.maptype == "vector")
                layer.setVisibility(true);
            if (layer.maptype == "image")
                layer.setVisibility(false);
        }

    }
}