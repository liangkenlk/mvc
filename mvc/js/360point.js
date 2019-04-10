
/*********************************************360vr***360度全景图相关**********************************************************/
function coninfo(Str) {
    if (console.info) {
        console.info(Str);
    }
}
var show360Open = false;
function show360Point()
{
    var url = "../Server/MapHandler.ashx?method=get360PointList";
    j({}, url, show360PointCallBack);
}

function show360PointCallBack(re) {

    if (show360Open) {
        $("#IDvr360").css("background", "rgb(235,235,235)");
        show360Open = false;
        if (vrLayer) {
            vrLayer.clear();
        }
    } else {
        $("#IDvr360").css("background", "#ccc");
        show360Open = true;
        if (vrLayer.graphics.length > 0) {
            return;
        }
       // $.post("getVr360ConfForWeb.ct", function (result) {
            if (console.info) {
                console.info(re.data);
            }
            var jsonPolygonArr =JSON.parse(re.data);
            for (var i = 0; i < jsonPolygonArr.length; i++) {
                var data = jsonPolygonArr[i];
                var id = data.id;
                var latlng = data.latlng;
                var text = data.text;
                //var address = data.address;
                //var thumbnail = data.thumbnail;
                $('#map_graphics_layer > path').remove();
                myMap.centerAt(new esri.geometry.Point({ "x": latlng[0], "y": latlng[1], "spatialReference": { "wkid": 4610 } }));
                var CurPos = new esri.geometry.Point(latlng[0], latlng[1], myMap.spatialReference);
                var sms = new esri.symbol.PictureMarkerSymbol("images/markerQueryVr3.png", 25, 41);
                var json = { title: "详细信息", content: uuid }
                var infoTemplate = new esri.InfoTemplate(json);
                // var graphic = new esri.Graphic(CurPos, sms, {"id":uuid},infoTemplate);
                var graphic = new esri.Graphic(CurPos, sms, { "id": id, "name": text, "latlng": latlng }, null);

                if (console.info) {
                    console.info(graphic);
                }
                vrLayer.add(graphic);
                myMap.addLayer(vrLayer);
                $(".action").remove();

            }
            if (vrLayer != null) {
                vrLayer.on("click", function (evt) {
                    var uuid = evt.graphic.attributes.id;
                    window.open('vr360/Pano.swf?id=' + id, "_blank");
                });
                vrLayer.on("mouse-over", function (evt) {
                    var uuid = evt.graphic.attributes.id;
                    var name = evt.graphic.attributes.name;
                    var address = evt.graphic.attributes.address;
                    var thumbnail = evt.graphic.attributes.thumbnail;
                    var latlng = evt.graphic.attributes.latlng;
                    // Ext.Msg.alert('提示', '请先选择专题。。。'+id);

                    if (console.info) {
                        console.info(evt);
                    }

                    var str1 = "<div>" +
				   	"<table>" +
				   	"<tr><td>名称:" + name + "</td></tr>" +
				   	"<tr><td>详细地址:" + address + "</td></tr>" +
				   	"</table>" +
				   //	"<img width='100%' height='50%' src='vr360/" + uuid + "/" + thumbnail + "' alt='" + thumbnail + "'>";
                   	"<img width='100%' height='50%' src='vr360/" + id+"'>";
                    var CurPos = new esri.geometry.Point(latlng[0], latlng[1], myMap.spatialReference);
                    myMap.infoWindow.setTitle("详细信息");
                    myMap.infoWindow.setContent(str1);
                    myMap.infoWindow.show(CurPos);
                    $('.maximize').hide();

                    // window.open('vr360/Pano.swf?uuid='+uuid,"_blank");
                });
            }
      //  })
    }
}

/*
var show360Open = false;
function show360Point() {
    if (show360Open) {
        $("#IDvr360").css("background", "rgb(235,235,235)");
        show360Open = false;
        if (vrLayer) {
            vrLayer.clear();
        }
    } else {
        $("#IDvr360").css("background", "#ccc");
        show360Open = true;
        if (vrLayer.graphics.length > 0) {
            return;
        }
        $.post("getVr360ConfForWeb.ct", function (result) {
            if (console.info) {
                console.info(result);
            }
            var jsonPolygonArr = eval("(" + result + ")");
            for (var i = 0; i < jsonPolygonArr.length; i++) {
                var data = jsonPolygonArr[i];
                var uuid = data.uuid;
                var latlng = data.latlng;
                var name = data.name;
                var address = data.address;
                var thumbnail = data.thumbnail;
                $('#map_graphics_layer > path').remove();
                myMap.centerAt(new esri.geometry.Point({ "x": latlng[0], "y": latlng[1], "spatialReference": { "wkid": 4610 } }));
                var CurPos = new esri.geometry.Point(latlng[0], latlng[1], myMap.spatialReference);
                var sms = new esri.symbol.PictureMarkerSymbol("images/markerQueryVr3.png", 25, 41);
                var json = { title: "详细信息", content: uuid }
                var infoTemplate = new esri.InfoTemplate(json);
                // var graphic = new esri.Graphic(CurPos, sms, {"id":uuid},infoTemplate);
                var graphic = new esri.Graphic(CurPos, sms, { "id": uuid, "name": name, "latlng": latlng, "address": address, "thumbnail": thumbnail }, null);

                if (console.info) {
                    console.info(graphic);
                }
                vrLayer.add(graphic);
                myMap.addLayer(vrLayer);
                $(".action").remove();

            }
            if (vrLayer != null) {
                vrLayer.on("click", function (evt) {
                    var uuid = evt.graphic.attributes.id;
                    window.open('vr360/Pano.swf?uuid=' + uuid, "_blank");
                });
                vrLayer.on("mouse-over", function (evt) {
                    var uuid = evt.graphic.attributes.id;
                    var name = evt.graphic.attributes.name;
                    var address = evt.graphic.attributes.address;
                    var thumbnail = evt.graphic.attributes.thumbnail;
                    var latlng = evt.graphic.attributes.latlng;
                    // Ext.Msg.alert('提示', '请先选择专题。。。'+id);

                    if (console.info) {
                        console.info(evt);
                    }

                    var str1 = "<div>" +
				   	"<table>" +
				   	"<tr><td>名称:" + name + "</td></tr>" +
				   	"<tr><td>详细地址:" + address + "</td></tr>" +
				   	"</table>" +
				   	"<img width='100%' height='50%' src='vr360/" + uuid + "/" + thumbnail + "' alt='" + thumbnail + "'>";

                    var CurPos = new esri.geometry.Point(latlng[0], latlng[1], myMap.spatialReference);
                    myMap.infoWindow.setTitle("详细信息");
                    myMap.infoWindow.setContent(str1);
                    myMap.infoWindow.show(CurPos);
                    $('.maximize').hide();

                    // window.open('vr360/Pano.swf?uuid='+uuid,"_blank");
                });
            }
        })
    }
}*/
var add360Is = false;
function add360Point() {
    if (!add360Is) {
        toolbar.activate(esri.toolbars.Draw.POINT);
        // line 515
        add360Is = true;
    } else {
        // if(console.info){
        // 	console.info("22222:"+geometry);
        // }		
        coninfo(geometry);
        // window.open("vr360/addvr.ct?x="+geometry.x+"&y="+geometry.y);
        htmlStr = "<div style='width: 100%;height: 100%;border:1px solid #A6C9E2;overflow:hidden;OVERFLOW-Y: auto; OVERFLOW-X:hidden'>" +
                    "<IFRAME id='vr360_frame' NAME='vr360_frame' width=100% height=100% SRC='vr360/addvr.ct?x=" + geometry.x + "&y=" + geometry.y + "' ></IFRAME>" +
                  "</div>";
        var vrWindow = Ext.create('Ext.window.Window', {
            title: '添加360度全景图',
            height: 250,
            width: 300,
            //x			: 100,
            //y			: 230,
            layout: 'absolute',
            // autoShow	: true,
            // closeAction	: 'destroy',
            modal: true,
            autoScroll: false,
            html: htmlStr,
            listeners: {
                close: function () {
                    // clearmeasure();
                    if (vrLayer) {
                        vrLayer.clear();
                    }
                }
            }
        }).show();

        add360Is = false;
    }
    // window.open("vr360/addvr.ct?x="+geometry.x+"&y="+geometry.y);
}

function vrlist() {

    htmlStr = "<div style='width: 100%;height: 100%;border:1px solid #A6C9E2;overflow:hidden;OVERFLOW-Y: auto; OVERFLOW-X:hidden'>" +
                    "<IFRAME id='vr360_frame' NAME='vr360_frame' width=100% height=100% SRC='vr360/vrlist.ct' ></IFRAME>" +
                  "</div>";

    // var vrWindow =  Ext.create('Ext.window.Window', {
    // 					title		: "360度全景图管理"，
    // 					height		: 230,
    // 					width		: 300,
    // 					x			: 100,
    // 					y			: 230,	
    // 					layout		: 'absolute',		
    // 					modal		: true,
    // 					autoScroll	: true,
    // 					html		: htmlStr,
    // 					listeners	: {
    // 						close	: function(){

    // 						}
    // 					}
    // 				}).show();

    var vrWindow = Ext.create('Ext.window.Window', {
        title: '添加360度全景图',
        height: 500,
        width: 1200,
        x: 300,
        y: 120,
        layout: 'absolute',
        // autoShow	: true,
        // closeAction	: 'destroy',
        modal: true,
        autoScroll: false,
        maximized: true,
        maximizable: true,
        html: htmlStr,
        listeners: {
            close: function () {
            }
        }
    }).show();
}


