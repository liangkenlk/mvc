
//function baseMapList()
//{
//    var url = "server/MapHandler.ashx?method=getBaseMapList";
//    j({}, url, baseMapListCallBack);

//}


function baseMapList() {

    var re = mapconfig; // --> parse error

    for (var i = 0; i < re.length; i++) {
       //continue;
        var wmtsLayer1 = CreatWMTSlayer(re[i].tileMatrixSet, re[i].format, re[i].style, re[i].url, re[i].layeridfilter, re[i].type, re[i].layername, re[i].levels, re[i].mapId,re[i].offline_url_android);
        //var wmtsLayer2 = CreatWMTSlayer(re[i].tileMatrixSet, re[i].format, re[i].style, re[i].url, re[i].layeridfilter, re[i].type, re[i].layername, re[i].levels, re[i].mapId, re[i].offline_url_android);
        
        myMap.addLayer(wmtsLayer1);
        //if(re[i].type=="image")
        //    myMap2.addLayer(wmtsLayer2);
        //wmtsLayer2.setVisibility(true);
        
    }
    //anlysisDrawPolygon = new esri.layers.ArcGISDynamicMapServiceLayer("http://121.33.234.44:6080/arcgis/rest/services/LW_SJGZ_PUB_440103/MapServer");
    //myMap.addLayer(anlysisDrawPolygon);
    //anlysisDrawPolygon.setVisibility(true);


    //分屏对比 地图的拖动事件
    //myMap.on("mouse-down,mouse-wheel", function () {
    //    tempMymap = "map";


    //});
    //myMap2.on("mouse-down,mouse-wheel", function () {
    //    tempMymap = "map2";
    //});
    //$("#map")[0].addEventListener("touchstart", function () {
    //    tempMymap = "map";

    //});
    //$("#map2")[0].addEventListener("touchstart", function () {
    //    tempMymap = "map2";

    //});
    //myMap.on("pan-end,zoom-end", function (e) {
        
    //    		   var centerPt=new esri.geometry.Point({"x":(e.extent.xmin+e.extent.xmax)/2,"y":(e.extent.ymin+e.extent.ymax)/2,"spatialReference": {"wkid": 4610 }});
    //    if (tempMymap == "map") {
    //        myMap2.setExtent(myMap.extent);
    //    }
    //});
    //myMap2.on("pan-end,zoom-end", function (e) {
    //    if (tempMymap == "map2") {
    //        myMap.setExtent(e.extent);
    //    }

    //});
    //showMapList(re);
    //themeList();
    //getAnalyseList();
 
}



//获取图层透明度
function getMapType() {
    return curentShowMapType;
}

function getCityservices() {
    return cityservices;
}


// 设置图层透明度
function setLayerOpacity(layerName, value) {
    var wmtslayer = myMap.getLayer(layerName);
    wmtslayer.setOpacity(value);
}

//图层透明度管理 点击复选框
function loadSubjectLayer(id, type, value) {
    var wmtslayer = myMap.getLayer(id);
    if (type) {
        //wmtslayer.setOpacity(value);
        subjectClickBox(id, type);
    } else {
        //wmtslayer.setOpacity(0);
        subjectClickBox(id, type);
    }
}


//点击图层复选框事件
function subjectClickCbx(id) {
    var subjectCbxId = document.getElementById(id);
    var rangeId = "range~" + id;
    var rangeObj = document.getElementById(rangeId);
    loadSubjectLayer(id, subjectCbxId.checked, rangeObj.value / 5);
}

function mapSelect(id) {
    var layerIds = myMap.layerIds;
    var type = $("#" + id)[0].checked;
    for (var i in layerIds) {
        var layer = myMap.getLayer(layerIds[i])
        if (layer == null)
            continue;
        if (id == layer.id && layer != undefined && type == true) {
            layer.setVisibility(true);
        }
        if (id == layer.id && layer != undefined && type == false) {
            layer.setVisibility(false);
        }
    }
}
var mapType = "vector";
function showMapList(re) {

    var htmlStr = "<ul>";
    for (var i = 0; i < re.length; i++) {
        var id = re[i].mapId;
        var name = re[i].layername;
       // htmlStr += "<li><div class='tc_text'><input name='subjectCbx' id='" + id + "' type='checkbox' onclick=mapSelect('" + id + "')><a>" + name + "</a></div><div class='tc_kj'><input id='range~" + id + "' type='range' min='0' max='10' value='10' /></div></li>";
        htmlStr += "<li><div class='tc_text'><input name='subjectCbx' id='" + id + "' type='checkbox' onclick=mapSelect('" + id + "')><a>" + name + "</a></div><input class='easyui-slider'value=100 id='range~" + id + "' style='width:200px' data-options='step:1,showTip:false,max:100,onChange:baseMapOnchange'></li>";

    }
    htmlStr += "</ul>";



    $(".centtc_list").html(htmlStr);
    
    for (var i = 0; i < re.length; i++) {

        //初始化复选框勾选
        if (re[i].type == mapType) {
            var cbxId = re[i].mapId;
            var obj = document.getElementById(cbxId);
            obj.checked = true;
            //	alert(mapType);
        }

        //拖动活动条时修改图层透明度事件
        //var key = "range~" + re[i].mapId;
        //var x = document.getElementById(key);
        //if (x == null)
        //    continue;
        //x.onchange = function () {

        //    setLayerOpacity(this.id.split("~")[1], this.value / 10);
        //};

        $.parser.parse();
    };
}

function baseMapOnchange()
{
    setLayerOpacity(this.id.split("~")[1], this.value / 100);
}


//矢量影像地图切换
function changeMapHandler(type) {
    curentShowMapType = type;
    showTDT();
    countyLayer.clear();
    //矢量影像地图切换时勾选复选框
    updateSubjectCheckbox(curentShowMapType);
}



//是否切换天地图切片
function showTDT() {

    //var layers = myMap.getLayersVisibleAtScale();
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


//矢量影像地图切换时勾选复选框
function updateSubjectCheckbox(mapType) {
    //		if(subjectToolWin!=null&&subjectToolWin.hidden!=true){
    var cservices = getCityservices();
    var cservices = cityservices.split("-")[0];
    for (var i = 0; i < cityservices.split(";").length - 1; i++) {
        var cbxId = cityservices.split(";")[i].split(",")[0];
        var obj = document.getElementById(cbxId);
        if (obj) {
            if (cityservices.split(";")[i].split(",")[2] == mapType) {
                obj.checked = true;
            } else {
                obj.checked = false;
            }
        }

    }
    //		}

}