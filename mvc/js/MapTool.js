
//拖动窗口事件
function dragWindow(nowWin) {
    nowWin.on("dragstart", function () {
        $('#divJeepDiv').css("display", "block");
    });
    nowWin.on("dragend", function () {
        $('#divJeepDiv').css("display", "none");
    });

}



//关闭子窗口
function closeChildWin() {
    if (childWin) {
        childWin.close();
    }
}
// 删除左右两端的空格
function trim(str) {
    return str.replace(/(^\s*)|(\s*$)/g, "");
}

function deactivate() {
    navToolbar.deactivate();
    // dragToolbar.deactivate();
}

//图层上加点
function addPointGeometry(data, attr, infoTemplate, layer, image,size) {
    var lat;
    var lon;
    lat = data[1];
    lon = data[0];
    if (image == null)
        image = "images/point/default.png";
    if (size == null)
        size = 18;
    var point = new esri.geometry.Point(lon, lat, spatialReference);
    var symbol = new esri.symbol.PictureMarkerSymbol(image, size, size);
    var graPoint = new esri.Graphic(point, symbol, attr, infoTemplate);
    graPoint.setInfoTemplate(infoTemplate);
    layer.add(graPoint);

}
//图层上加面
function addPolygonGeometry(data, attr, color, infoTemplate, layer,tmd) {
    var polygonJson = { "rings": data, "spatialReference": spatialReference };
    var polygon = new esri.geometry.Polygon(polygonJson);
    if (color == null)
        color = "0, 0, 255";
    var r = parseInt(color.split(",")[0]);
    var g = parseInt(color.split(",")[1]);
    var b = parseInt(color.split(",")[2]);
    if (tmd == null)
        tmd = 0;
    var symbol = new esri.symbol.SimpleFillSymbol(esri.symbol.SimpleFillSymbol.STYLE_SOLID,
             new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID,
             new dojo.Color([r, g, b]), 2), new dojo.Color([r, g, b,tmd])
         );
    var graPolygon = new esri.Graphic(polygon, symbol, attr, infoTemplate);
    layer.add(graPolygon);
}
//图层上加线
function addPolyLineGeometry(data, attr, color, infoTemplate, layer) {
    var lineJson = { "paths": data, "spatialReference": spatialReference };
    var polyline = new esri.geometry.Polyline(lineJson);
    if (color == null)
        color = "0, 0, 255";
    var r = parseInt(color.split(",")[0]);
    var g = parseInt(color.split(",")[1]);
    var b = parseInt(color.split(",")[2]);

    var symbol = new esri.symbol.SimpleFillSymbol(esri.symbol.SimpleFillSymbol.STYLE_SOLID,
             new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID,
             new dojo.Color([r, g, b]), 2), new dojo.Color([r, g, b, 0])
         );
    var graPolygon = new esri.Graphic(polyline, symbol, attr, infoTemplate);
    layer.add(graPolygon);
}

//图层上加文字
function addLabelToLayer(point,text, attr, color,  layer)
{
    if (color == null)
        color = "0, 0, 255";
    var r = parseInt(color.split(",")[0]);
    var g = parseInt(color.split(",")[1]);
    var b = parseInt(color.split(",")[2]);
    var font = new esri.symbol.Font("20px", esri.symbol.Font.STYLE_NORMAL, esri.symbol.Font.VARIANT_NORMAL, esri.symbol.Font.WEIGHT_BOLDER);

    var symbol = new esri.symbol.TextSymbol(text, font, new dojo.Color([r, g, b]), 2);
    var graPoint = new esri.Graphic(point, symbol, attr,null);
    layer.add(graPoint);
}

var currentDic;
//生成详细信息窗口内容
function GetInfoWindowContent( dic,fun_detailWin) {
    if (dic == null)
    {
    dic = {};
    dic["VILLAGE"] = "所属村";
    dic["HOUSEHOLDERNAME"] = "户主名称";
    dic["A18"] = "贫困户属性";

    }
    if (fun_detailWin == null)
        fun_detailWin = "detailWin";
    themeValueNameList = dic;
    //var divStr = "<table cellpadding='0' cellspacing='0' class='msgTable'><tr><td>门牌位置<br><img width='95%' src='upload/user/pic/" + info.MPWZ_Pic + "'/></td><td>相片<br><img width='95%' src='upload/user/pic/" + info.Picture + "'/></td><tr></tr></table>";
    var divStr = "";
    divStr += "<table cellpadding='0' cellspacing='0' class='msgTable'>";
    if (dic["项目地块编号"] != null)//三旧改造专用s
    {
        divStr += "<tr><td width='80' valign='top'>相关图片</td><td align='left'><a href='javascript:sjgzPic(\"" + dic["项目地块编号"] + "\")'>点击查看>>></a></td></tr>";
        currentDic = dic;
    }
    
    for (var i in dic) {
        //if (themeValueNameList[i] != null) {
        //    if (info[i] == null)
        //        info[i] = "";
        //divStr += "<tr><td width='80' valign='top'>" + themeValueNameList[i] + "</td><td align='left'>" + info[i] + "</td></tr>";
        if (i != "latlng" && i != "geotype") {
            if (i == "规划地类")
            {
                divStr += "<tr><td width='80' valign='top'>" + i + "</td><td align='left'>" + dlmcDic[dic[i]] + "</td></tr>";
            }
            else
                divStr += "<tr><td width='80' valign='top'>" + i + "</td><td align='left'>" + keep4Decimal(dic[i]) + "</td></tr>";
        }
        //}
    }

    if (dic == null || dic.length < 1) {
        divStr += "<tr><td width='150'>此区域无专题属性</td></tr>";
    }
    divStr += "</table>";
    // divStr += "<div style='float:right;color:blue'><a href='javascript:// sjgzPic()' onclick=" + fun_detailWin + "('" + info.BSM + "')>详细>>></a></div>";
    return divStr;


}
//四舍五入保留2位小数（若第4位小数为0，则保留一位小数）
function keep4Decimal(num) {
    var result = parseFloat(num);
    if (isNaN(result)) {
        //alert('传递参数错误，请检查！');
        return num;
    }
    result = Math.round(num * 10000) / 10000;
    return result;
}
function sjgzPic(id) {
    $("#iframe").attr("src", "sjgzPic.aspx?id="+id);
    $('#win').window('setTitle',currentDic["权属单位名称"]);
    $('#win').window('open');
}
//经纬度精确浮点型封装
function LatlngParse(strlatlng, type) {
    var latlngs = strlatlng.split(";");
    var result = [];
    result[0] = [];
    if (type == "point" || type==null) {
        //return strlatlng.split(',')[0] + ',' + strlatlng.split(',')[1];
        return [parseFloat(strlatlng.split(',')[0]), parseFloat(strlatlng.split(',')[1])];
    }
    for (var i = 0; i < latlngs.length; i++) {

        result[0].push([parseFloat(latlngs[i].split(',')[0]), parseFloat(latlngs[i].split(',')[1])]);

    }
    if (type == "polygon") {
        result[0].push(result[0][0]);
    }
    return result;
}
//经纬度字符型封装
function LatlngStringfy(arrlatlng, type) {
    var result = "";
    if (type == "点") {
        return arrlatlng[0] + "," + arrlatlng[1];
    }
    for (var i = 0; i < arrlatlng[0].length; i++) {

        result += arrlatlng[0][i][0] + ',' + arrlatlng[0][i][1]
        if (i != arrlatlng[0].length - 1)
            result += ";";
    }
    return result;
}

/************************************************放大 缩小***********************************************************/
function extentHistoryChangeHandler() {
    navToolbar.isFirstExtent();
    navToolbar.isLastExtent();
}
	

function moveTo(x,y,z){	
    //		console.info("x:"+x+" y:"+y+" z:"+z);
    z = z==undefined?13:z;
    myMap.setZoom(z);
    myMap.centerAt(new esri.geometry.Point({ "x": x, "y": y, "spatialReference": spatialReference }));
}
	
//全屏
function fullext(){
    // navToolbar.zoomToFullExtent();
    //111.0094495561685 21.516663074469026 
    // deactivate();
    myMap.setZoom(13-startLevel);
    myMap.centerAt(new esri.geometry.Point({ "x": center.x, "y": center.y, "spatialReference": spatialReference }));
    // $.post("dianbai.cainiaorj.com/getWallmapPointConf.ct",{"wallmap":"电白区"},function(e){
    // console.info(e);
    // })
		
}
//放大
function zoomIn(){
    // navToolbar.activate(esri.toolbars.Navigation.ZOOM_IN);
    // dragToolbar.activate(esri.toolbars.Draw.RECTANGLE);
    // $(".tooltip").remove();
    myMap.setZoom(myMap.getZoom()+1);
}
//缩小
function zoomOut(){
    // navToolbar.activate(esri.toolbars.Navigation.ZOOM_OUT);
    // dragToolbar.activate(esri.toolbars.Draw.RECTANGLE);
    // $(".tooltip").remove();
    myMap.setZoom(myMap.getZoom()-1);
}
	
//移动
function pan(){
    navToolbar.activate(esri.toolbars.Navigation.PAN);
		
}
	
//
function deactivate(){
    navToolbar.deactivate();
    // dragToolbar.deactivate();
}
	
/*******************************************************************************************************************/
/****************************************************************************************测面  测距****************************************************/
//侧面测距 结果数组
var measureArr = [];
//测距
function measureLength(){    	
    toolbar.activate(esri.toolbars.Draw.POLYLINE);
		  
}
//测面
function measureArea(){
    toolbar.activate(esri.toolbars.Draw.POLYGON);		  
}
var geometry;
function measureEnd(evt){
    if(console.info){
        console.info("L:514");
    }
    toolbar.deactivate();
    geometry =evt.geometry;
    // evtGeometry = evt.geometry;
    //if(add360Is){
    //    add360Point();
    //}
    var symbol;
    var point;
    var value;
    switch (geometry.type) {
        //添加360vr点全景点
        case "point":
            {
                //坐标
                latlng = evt.geometry.x + "," + evt.geometry.y ;
                var CurPos = new esri.geometry.Point(evt.geometry.x, evt.geometry.y, myMap.spatialReference);
                var sms = new esri.symbol.PictureMarkerSymbol("images/markerQueryVr3.png", 25, 41);
                var graphic = new esri.Graphic(CurPos, sms, null, null);
                vrLayer.add(graphic);
                myMap.addLayer(vrLayer);
                // var point = new esri.geometry.Point( {"x": evt.geometry.x, "y": evt.geometry.y, "spatialReference": {"wkid": 4326 } });
                // var symbol = new esri.symbol.SimpleMarkerSymbol().setStyle(
                // esri.symbol.SimpleMarkerSymbol.STYLE_CIRCLE).setColor(
                // new dojo.Color([0,0,255,0.5]));
                // Ext.Msg.alert("提示信息", "请填写标签名称！" + latlng);
                break;}
        case "polyline":{
            symbol = new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID, new dojo.Color([0, 0, 255]), 2);
            var points=geometry.paths[0];
            var length = getDistanceForJs(points);
            var pointArray=points[points.length-1];
            var x=pointArray[0];
            var y=pointArray[1];					
            point=new esri.geometry.Point(x,y,myMap.spatialReference);	
            
            if (length > 1000)
                value = "距离值" + (length / 1000).toFixed(3) + " km";
            else
                value = "距离值" + length.toFixed(3) + " m";
            break;
        }
        case "polygon":{
            symbol = new esri.symbol.SimpleFillSymbol(esri.symbol.SimpleFillSymbol.STYLE_SOLID,
                new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID,
                new dojo.Color([0, 0, 255]), 2), new dojo.Color([0, 0, 255, 0.15])); 
		           			           	
            var points=geometry.rings[0];
            var area = getArea(points);
            var pointArray=points[points.length-2];
            var x=pointArray[0];
            var y=pointArray[1];					
            point = new esri.geometry.Point(x, y, myMap.spatialReference);
            if (area > 1000)
                value = "面积值" + (area/1000000).toFixed(3) + " km²";
            else
                value = "面积值" + area.toFixed(3) + " m²";
            break;
        }
        default:{
			  	
            break;
        } 
    } 
			
    var graphic = new esri.Graphic(geometry,symbol);
    // myMap.graphics.add(graphic);
    countyLayer.add(graphic);		  
    var textSymbol = new esri.symbol.TextSymbol(value);
    textSymbol.setColor(new dojo.Color([255, 0, 0]));		
    textSymbol.setOffset(0, 10);
    var graphicText = esri.Graphic(point, textSymbol);
    // myMap.graphics.add(graphicText);
    countyLayer.add(graphicText);
    myMap.addLayer(countyLayer);
    countyLayer.show();
    // measureArr.push(countyLayer);

}

//js代码计算距离 
function getDistanceForJs(params){
    var dd = 0;
    for (var i = 0; i < params.length - 1; i++) {
        var lat = [params[i][1], params[i+1][1]];
        var lng = [params[i][0], params[i+1][0]];     //var R = 6371; // km (change this constant to get miles)
        var R = 6378137; // In meters
        var dLat = (lat[1] - lat[0]) * Math.PI / 180;
        var dLng = (lng[1] - lng[0]) * Math.PI / 180;
        var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) + Math.cos(lat[0] * Math.PI / 
	
180) * Math.cos(lat[1] * Math.PI / 180) * Math.sin(dLng / 2) * Math.sin(dLng / 2);
        var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        var d = R * c;
        dd += d;
    }
    return dd;
    //return (dd/1000).toFixed(3);
}
	
//计算面积
function getArea(geometry){
    var arr = new Array();      
    for(var i=0;i<geometry.length;i++){
        var pt1 = new TLngLat(geometry[i][0],geometry[i][1]);
        arr.push(pt1); 
    }
    //document.getElementById("measure_resultdiv").innerHTML="面积："+(polygonTool.getArea(arr)/1000000).toFixed(3)+" km<sup>2</sup>";
    //return (polygonTool.getArea(arr)/1000000).toFixed(3);
    /** dialog =  new NDialog("null", new NLatLng(0,0),new NSize(point.x,point.y),"NewMapServer API",true);
     dialog.setContentHTML(diaglogContent);
     map.addDialog(dialog);  
     dialog.show(); */
    return polygonTool.getArea(arr);
}
	
//清除 测面测距
function clearmeasure(){
    // for(var m in measureArr){
    // measureArr[m].clear();
    // }
    //隐藏浮动窗口
    myMap.infoWindow.hide();
    //清除测面 测距图层
    countyLayer.clear();
    measureArr = [];
    //取消地图平移 放大缩小 
    deactivate();
    //清除挂图图层
    if(wallmap4Cunlayer){
        page =1;
        wallmap4Cunlayer.clear();
        wallmap4Zhenlayer.clear();
    }
    if(vrLayer){
        vrLayer.clear();
    }
    if(celianLayer){
        celianLayer.clear();
    }
    //清除地名地址查询标签
    clearDisplayLabelLayer();
}
	
/*****************************************************************************************************************************************************/
		
// htmlStr = '';
		



// document.getElementById("snapwindow").style.position="absolute";
// document.getElementById("snapwindow").style.left="-10000px";
// document.getElementById("snapwindow").style.display="none";
// document.getElementById("ext-gen1077").style.display="none";

// $("#snapwindow").css({"display":"none"});
// $(".x-css-shadow").css({"display":"none"});
// $(".x-ie-shadow").css({"display":"none"});

// $(".x-css-shadow").css({"opacity":"0.1"});
// $(".x-css-shadow").css({"opacity":"0.1"});

// snapwindow.setVisible = false;
// snapwindow.hide();
//保存图片
function saveImageAs(imgOrURL) {
    if (typeof imgOrURL == 'object')
        imgOrURL = imgOrURL.src;
    window.win = open (imgOrURL);
    setTimeout('win.document.execCommand("SaveAs","aa.jpg")', 500);
}

//打印图片
function printS(src) 
{  
    Ext.Msg.confirm('提示', '确定打印吗？', function(e){
        if(e == 'yes'){
            var newstr = '<img id="snapimg" src="'+src+'" alt="截图" ;>';
            // console.log(newstr);
            printWindow = window.open();
            printWindow.document.write(newstr);
            setTimeout(function(){
                if (!window.ActiveXObject){
                }else{
                    printWindow.location.reload();
                } 
						
                setTimeout(function(){
                    printWindow.print();
                },300);
            },300);
            return false;
        }
    });
    // if(confirm('确定打印吗？')){
			
    // 　}
} 	
	 
	 


function printdiv(printpage) {
    //var headstr = "<html><head><title></title></head><body>";
    //var footstr = "</body>";
    //var newstr = document.all.item(printpage).innerHTML;
    //var oldstr = document.body.innerHTML;
    ////document.body.innerHTML = headstr + newstr + footstr;
    //var w = window.open();
    //w.document.body.innerHTML = headstr + newstr + footstr;
    //w.print();
    //document.body.innerHTML = oldstr;
    window.print();
    return false;
}




// 删除左右两端的空格
function trim(str) {
    return str.replace(/(^\s*)|(\s*$)/g, "");
}




/****************************************************分屏对比**************************************************/
function maptoSplit() {
    if ($("#divhid").css('display') == "none") {
        //	      var centerPt=new esri.geometry.Point({"x":(myMap.extent.xmin+myMap.extent.xmax)/2,"y":(myMap.extent.ymin+myMap.extent.ymax)/2,"spatialReference": {"wkid": 4610 }});
        //	      myMap2.centerAndZoom(centerPt,myMap.getLevel());
        //	        if(myMap2) myMap2.resize();
        //	       myMap2.setExtent(myMap.extent);
        $("#map").css('width', '50%');
        $("#divhid").css({ height: "100%", width: "50%", overflow: "hidden", position: "absolute", display: "block", right: "0px" });
        $("#map2").css({ width: "100%", height: "100%", display: "inline" });

        //		  $("#map2").css('z-index', '99');
        if (myMap) myMap.resize();
        myMap2.setExtent(myMap.extent);
        if (myMap2) myMap2.resize();



    } else {
        $("#divhid").css('display', 'none')
        $("#map").css('width', '100%');
        if (myMap) myMap.resize();
        if (myMap2) myMap2.resize();
    }

    setTimeout("resizeMap2()", 1000);
}
function resizeMap2() {
    myMap2.setExtent(myMap.extent);
    if (myMap2) myMap2.resize();
}
/********************************************************卷帘效果******************************************/
function curtain() {

    var dragging = false;//拖拉控制
    if ($("#divhid").css('display') == "none") {
        //地图1 的宽度
        var wid = document.getElementById("map").scrollWidth;
        var hei = document.getElementById("map").scrollHeight;
        $("#map , #map2").css({ width: "100%", height: "100%", position: "absolute", top: "0", right: "0" });

        var arrowtop = ($("body").height() / 2 - 30) + "px";
        $("#map2").css({ width: wid + "px", height: "100%", 'z-index': "20", display: "inline" });
        $("#divhid").css({ height: "100%", width: "0", overflow: "hidden", position: "absolute", top: "0", right: "0", display: "block" });
        $("#arrow").css({ top: arrowtop, right: "-25", "z-index": "21", display: "inline" });

        setTimeout("resizeMap2()", 1000);
        //mobiletouch 这个库，它在触屏事件上层做了封装，你可以用 swipeStart ，swipeProgress 和 swipe 来替代 touchstart, touchmove 和 touchend.
        $(document).bind("mousemove", function (e) {
            e.preventDefault(); //阻止触摸事件的默认行为，即阻止滚屏
            if (dragging) {
                var e = e || window.event;
                var map2Width;
                //原地图的宽度
                var mapWidth = $("#map").width();
                var marginRightOfArrow = $(document.body).width() - e.pageX;
                if (marginRightOfArrow > 0) {
                    if (marginRightOfArrow <= mapWidth) {
                        map2Width = marginRightOfArrow;
                    } else {
                        map2Width = mapWidth;
                    }
                } else {
                    map2Width = 0;
                }
                $("#arrow").css({ right: (map2Width - 25 < -25 ? -25 : map2Width - 25) + "px" });
                $("#divhid").css({ width: map2Width + "px" });
                // fraction = dwid/wid;
                return false;
            }
        });
        $("#arrow").bind("mousedown", function (e) {
            myMap.disablePan();
            myMap2.disablePan();
            dragging = true;
            var centerPt = new esri.geometry.Point({ "x": (myMap.extent.xmin + myMap.extent.xmax) / 2, "y": (myMap.extent.ymin + myMap.extent.ymax) / 2, "spatialReference": spatialReference });
            myMap2.centerAndZoom(centerPt, myMap.getLevel());
            if (myMap2) myMap2.resize();
            setTimeout("myMap2.resize()", 1000);
            // map2.setCenter(map.getCenter(),map.getZoomLevel());
        });
        $(document).bind("mouseup", function (e) {
            if (dragging) {
                myMap2.setExtent(myMap.extent);
                if (myMap2) myMap2.resize();
                myMap.enablePan();
                myMap2.enablePan();
                //两个地图在DIV中的比例
                $("#divhid").css("width", Math.round(($("#divhid").width() / $("#map").width()) * 10000) / 100 + "%");
                //		     console.info("百分比： " + Math.round(($("#divhid").width()/$("#map").width()) * 10000) / 100);
            }
            dragging = false;
        });

     

    } else {
        var dragging = false;
        $("#divhid").css('display', 'none');
        $("#divhid").css('width', '0px');
        $("#arrow").css('display', 'none');
        $("#map2").css('display', 'none');

        //	      document.onmousemove = function(e) {
        //	        return false;
        //	      }
    }
}
/*************************************************************************************************************/
function splitGotoMap() {

}

//构造切片

function CreatWMTSlayer(tileMatrixSet, format, style, url, layeridfilter, type, layername,levels,id,serverType) {
    esri.layers.WMTSLayer.prototype.getTileUrl = function (level, row, column) {
        return this.url + "?SERVICE=WMTS&VERSION=1.0.0&REQUEST=GetTile&LAYER=" + this.layerInfo.identifier + "&STYLE=" + this.layerInfo.style + "&FORMAT=" + this.layerInfo.format + "&TILEMATRIXSET=" + this.layerInfo.tileMatrixSet + "&TILEMATRIX=" + (level+1) + "&TILEROW=" + row + "&TILECOL=" + column;
    }
    //$.post('getMapServiceWmtslayerList.ct',function(result){
    tileExtent = new esri.geometry.Extent(-180.0, -90.0, 180.0, 90.0, spatialReference);
    //tileExtent = new esri.geometry.Extent(107.05128777659374, 22.853843645606837, 109.91395558923273, 24.180445802683437, spatialReference);
    tileInfo = new esri.layers.TileInfo({
        "format": "image/png",
        //"dpi": 90.71428571428571,
        "dpi": 96,
        "compressionQuality": 0,
        "spatialReference": spatialReference,
        "rows": 256,
        "cols": 256,
        "origin": {
            "x": -180,
            "y": 90
        },
        "lods": [
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

        ]
    });
    scale_JSON = {
        1: 295497593.05875003,
        2: 147748796.52937502,
        3: 73874398.264687508,
        4: 36937199.132343754,
        5: 18468599.566171877,
        6: 9234299.7830859385,
        7: 4617149.8915429693,
        8: 2308574.9457714846,
        9: 1154287.4728857423,
        10: 577143.73644287116,
        11: 288571.86822143558,
        12: 144285.93411071779,
        13: 72142.967055358895,
        14: 36071.483527679447,
        15: 18035.741763839724,
        16: 9017.8708819198619,
        17: 4508.9354409599309,
        18: 2254.4677204799655,
        19: 1127.23386023,
        20: 563.616930115
    };

    var layerInfo = new esri.layers.WMTSLayerInfo({
        tileInfo: tileInfo,
        fullExtent: tileExtent,
        initialExtent: tileExtent,
        //			          identifier: "vec",
        //			          tileMatrixSet: "c",
        //			          format: "tiles",
        //			          style: "Default"
        identifier: layeridfilter,
        tileMatrixSet: tileMatrixSet,
        format: format,
        style: style

    });


    var resourceInfo = {
        version: "1.0.0",
        layerInfos: [layerInfo],
        copyright: ""
    };

    var options = {
        serviceMode: "KVP",
        resourceInfo: resourceInfo,
        layerInfo: layerInfo,
        layerInfos: [layerInfo]
    };
    //var jsonOfResolutions = {
    //    "1": 0.703125,
    //    "2": 0.3515625,
    //    "3": 0.17578125,
    //    "4": 0.087890625,
    //    "5": 0.0439453125,
    //    "6": 0.02197265625,
    //    "7": 0.010986328125,
    //    "8": 0.0054931640625,
    //    "9": 0.00274658203125,
    //    "10": 0.001373291015625,
    //    "11": 0.0006866455078125,
    //    "12": 0.00034332275390625,
    //    "13": 0.000171661376953125,
    //    "14": 0.0000858306884765625,
    //    "15": 0.00004291534423828125,
    //    "16": 0.000021457672119140625,
    //    "17": 0.000010728836059570312,
    //    "18": 0.000005364418029785156,
    //    "19": 0.0000026822090148925775,
    //    "20": 0.00000134110450744628875
    //};

    //var r = 0;
    //for (var key in jsonOfResolutions) {
    //    if (r == 0) {
    //        minR = key - 1;
    //    }
    //    maxR = key;
    //    r++;
    //}
    minR = levels.split(',')[0];
    maxR = levels.split(',')[levels.split(',').length - 1];
    if (type == "TDTServer") {
        //由于 format 总是会在前面加了"image/" 所以源码被我改了源码路径 Path=".\arcgis_js_api\library\3.9\3.9\js\esri\layers\WMTSLayer.js" 原文件重命名为"WMTSLayer_1_0731.js"
        wmtsLayer1 = new esri.layers.WMTSLayer(url, options);
        //			        wmtsLayer1.UrlTemplate=wmtsLayer1.UrlTemplate.replace("image/","");

    } else {
        wmtsLayer1 = new esri.layers.WMTSLayer(url, options);
    }

    wmtsLayer1 = new esri.layers.WMTSLayer(url, options);
    wmtsLayer1.id = id;
    wmtsLayer1.themeId = id;
    wmtsLayer1.description = type;
    wmtsLayer1.maptype = type;

    //wmtsLayer1.setMinScale(scale_JSON[parseInt(minR)]);
    //wmtsLayer1.setMaxScale(scale_JSON[parseInt(maxR)]);


    if (serverType == "arcserver" || serverType == "arcserverquery")
    {
        if (url == "")
            return wmtsLayer1;
        wmtsLayer1 = new esri.layers.ArcGISTiledMapServiceLayer(url, { displayLevels: [8, 9, 10, 11, 12,13,14,15,16,17,18,19] });
        wmtsLayer1.id = id;
        wmtsLayer1.themeId = id;
        wmtsLayer1.maptype = type;
        //wmtsLayer1.initialExtent = tileExtent;
        //wmtsLayer1.fullExtent = tileExtent;
        //wmtsLayer1.tileInfo = tileInfo;
        wmtsLayer1.spatialReference = spatialReference;
        wmtsLayer1.description = type;
        wmtsLayer1.setScaleRange(scale_JSON[0], 533);

    }
    if (wmtsLayer1.description == "image") {
        wmtsLayer1.setVisibility(false);
    }
    return wmtsLayer1;

    //cityservices += layeridfilter + "," + layername + "," + type + "," + mapId + ";";

}


/****************************************************分屏对比**************************************************/
var tosplit = false;	//设置分屏默认关闭状态 
var curtainbool = false; //打开卷帘时,设置分屏默认关闭状态
function maptoSplit() {
    tosplit = tosplit == false ? true : false;
    if (tosplit) {
        //	      var centerPt=new esri.geometry.Point({"x":(myMap.extent.xmin+myMap.extent.xmax)/2,"y":(myMap.extent.ymin+myMap.extent.ymax)/2,"spatialReference": {"wkid": 4610 }});
        //	      myMap2.centerAndZoom(centerPt,myMap.getLevel());
        //	        if(myMap2) myMap2.resize();
        //	       myMap2.setExtent(myMap.extent);
        $("#map").css('width', '50%');
        $("#divhid").css({ height: "100%", width: "50%", overflow: "hidden", position: "absolute", display: "block", right: "0px" });
        $("#map2").css({ width: "100%", height: "100%", display: "inline" });

        //		  $("#map2").css('z-index', '99');
        if (myMap) myMap.resize();
        myMap2.setExtent(myMap.extent);
        if (myMap2) myMap2.resize();
        $('#arrow').hide();
        boolcurtain = false; //设置分屏打开时，卷帘打开状态
        curtainbool = true; //设置分屏打开时的,卷帘打开状态
    } else {
        $("#divhid").css('display', 'none');
        $("#map").css('width', '100%');
        if (myMap) myMap.resize();
        if (myMap2) myMap2.resize();
        $('#arrow').hide();
        boolcurtain = false;  //设置分屏关闭时的,卷帘关闭状态
        curtainbool = false; //设置分屏关闭时的,卷帘关闭状态
    }
    setTimeout("resizeMap2()", 1000);
}
function resizeMap2() {
    myMap2.setExtent(myMap.extent);
    if (myMap2) myMap2.resize();
}



//GPS定位
//function gpslocation() {
//    if (navigator.geolocation) {
//        navigator.geolocation.getCurrentPosition(zoomToLocation, locationError);
//        navigator.geolocation.watchPosition(showLocation, locationError);
//    }
//}
var watchId;
var owatchid;
var isFirstTime;

function gpslocation() {
    //var data = [108.35610959749374, 22.813999489808694];
    //addPointGeometry(data, null, null, drawLabelLayer);
    //myMap.centerAndZoom(new esri.geometry.Point({ "x": data[0], "y": data[1], "spatialReference": spatialReference }), 13 - startLevel);
    if (window.plus)
    { 
    	isFirstTime=true;
    	if(watchId)
    	{
    		plus.geolocation.clearWatch(watchId);
    		watchId=null;
    		return;
    	}
    	owatchid=plus.orientation.watchOrientation( Orient, function(e){
		alert('Geolocation error: ' + e.message);
	} );
    	watchId = plus.geolocation.watchPosition(zoomToLocation,function(e){
//		alert('Geolocation error: ' + e.message);
	},{provider:'amap',coordsType:'gcj02',enableHighAccuracy:true}); 
    }

}
function zoomToLocation(p) {
//    var data = [p.coords.longitude, p.coords.latitude];
    //var data = [22.813999489808694, 108.35610959749374];
    
  var data = gcj02towgs84(p.coords.longitude, p.coords.latitude);
    
    myMap.infoWindow.hide();
    gpsLayer.id="gpsLayer";
     gpsLayer.clear();
    gpsLayer.setOpacity(0);
    var infoTemplate = new esri.InfoTemplate("当前位置", p.addresses + "<br>" + data[0] + "," + data[1] );
    addPointGeometry(data, {id:"posi"}, infoTemplate, gpsLayer,'images/point/or.png');

    if(isFirstTime)
    {

    	isFirstTime=false;
//	    myMap.addLayer(anlysisDrawPolygon);
	    myMap.centerAt(new esri.geometry.Point({ "x": data[0], "y": data[1], "spatialReference": spatialReference }));
	     mui.toast("当前位置:"+p.addresses + "\n" + data[0] + "," + data[1] );
    }
    //  myMap.infoWindow.setTitle("当前位置");
    //myMap.infoWindow.setContent(p.addresses+"<br>"+data[0]+","+data[1]+"<br>"+p.coordsType);
    ////  myMap.infoWindow.setContent(data[0]+","+data[1]);
    //   myMap.infoWindow.show(new esri.geometry.Point({ "x": data[0], "y": data[1], "spatialReference": spatialReference }));
}

function latlngpost()
{
    var data = JSON.parse('[' + $("#txtlatlng").val() + ']');
    anlysisDrawPolygon.clear();
    addPointGeometry(data, null, null, anlysisDrawPolygon);
    myMap.addLayer(anlysisDrawPolygon);
    myMap.centerAt(new esri.geometry.Point({ "x": data[0], "y": data[1], "spatialReference": spatialReference }));
}

function toggleLatlngWin()
{
    $('#latlngWin').window({
        width: 300,
        height: 100,
        modal: false,
        maximizable: false,
        minimizable: false,
        collapsible: false,
        draggable: false,
        resizable: false,
        border: false,
        shadow: false,
        title: '输入经纬度'
    });
    AnalysisResultWin = $('#latlngWin').window("open");
}

/**
 * GCJ02 转换为 WGS84
 * @param lng
 * @param lat
 * @returns {*[]}
 */
function gcj02towgs84(lng, lat) {
    if (out_of_china(lng, lat)) {
        return [lng, lat]
    }
    else {
        var dlat = transformlat(lng - 105.0, lat - 35.0);
        var dlng = transformlng(lng - 105.0, lat - 35.0);
        var radlat = lat / 180.0 * PI;
        var magic = Math.sin(radlat);
        magic = 1 - ee * magic * magic;
        var sqrtmagic = Math.sqrt(magic);
        dlat = (dlat * 180.0) / ((a * (1 - ee)) / (magic * sqrtmagic) * PI);
        dlng = (dlng * 180.0) / (a / sqrtmagic * Math.cos(radlat) * PI);
        mglat = lat + dlat;
        mglng = lng + dlng;
        return [lng * 2 - mglng, lat * 2 - mglat]
    }
}

function transformlat(lng, lat) {
    var ret = -100.0 + 2.0 * lng + 3.0 * lat + 0.2 * lat * lat + 0.1 * lng * lat + 0.2 * Math.sqrt(Math.abs(lng));
    ret += (20.0 * Math.sin(6.0 * lng * PI) + 20.0 * Math.sin(2.0 * lng * PI)) * 2.0 / 3.0;
    ret += (20.0 * Math.sin(lat * PI) + 40.0 * Math.sin(lat / 3.0 * PI)) * 2.0 / 3.0;
    ret += (160.0 * Math.sin(lat / 12.0 * PI) + 320 * Math.sin(lat * PI / 30.0)) * 2.0 / 3.0;
    return ret
}

function transformlng(lng, lat) {
    var ret = 300.0 + lng + 2.0 * lat + 0.1 * lng * lng + 0.1 * lng * lat + 0.1 * Math.sqrt(Math.abs(lng));
    ret += (20.0 * Math.sin(6.0 * lng * PI) + 20.0 * Math.sin(2.0 * lng * PI)) * 2.0 / 3.0;
    ret += (20.0 * Math.sin(lng * PI) + 40.0 * Math.sin(lng / 3.0 * PI)) * 2.0 / 3.0;
    ret += (150.0 * Math.sin(lng / 12.0 * PI) + 300.0 * Math.sin(lng / 30.0 * PI)) * 2.0 / 3.0;
    return ret
}

/**
 * 判断是否在国内，不在国内则不做偏移
 * @param lng
 * @param lat
 * @returns {boolean}
 */
function out_of_china(lng, lat) {
    return (lng < 72.004 || lng > 137.8347) || ((lat < 0.8293 || lat > 55.8271) || false);
}
//定义一些常量
var x_PI = 3.14159265358979324 * 3000.0 / 180.0;
var PI = 3.1415926535897932384626;
var a = 6378245.0;
var ee = 0.00669342162296594323;

//设置方向
function Orient(p)
{
//	var g= getPosiGraphics();
//	if(g!=null)
//	   g.hide();
	
	myMap.addLayer(gpsLayer);
	var img = $("#graphicsLayer8_layer image");
    
	if(img.length==0)
	return;
	
	
	var d=  p.magneticHeading-window.orientation;
	var x = parseInt(img.attr("x"))+8;
	var y = parseInt(img.attr("y"))+8;
    img.attr("transform","rotate("+d+","+x+" "+y+")");
    gpsLayer.setOpacity(1);

    
//  myMap.addLayer(anlysisDrawPolygon);
    //plus.orientation.clearWatch(owatchid);
//img.css("stransform","rotate("+d+"deg)" );
	
}

function getPosiGraphics()
{
    for(var i = 0;i<gpsLayer.graphics.length;i++)
	{
		gpsLayer.graphics[i].attributes["id"]=="posi";
		return gpsLayer.graphics[i];
	}
}
