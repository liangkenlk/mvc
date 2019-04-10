
/**********************************************************地名地址查询*****************************************/

//地名地址查询
var newLayerNameWin;
function queryLayerNameListWin() {

    if (newLayerNameWin != null) {
        if (!newLayerNameWin.rendered) {

        } else if (newLayerNameWin.hidden) {

        } else {
            return false;
        }
    }

    //地名地址查询
    Ext.define('layer', {
        extend: 'Ext.data.Model',
        fields: [{
            name: 'id',
            mapping: 'id'
        },
	    {
	        name: 'name',
	        mapping: 'DOMAINNAME'
	    },
	    {
	        name: "address",
	        mapping: 'ADDNAME'
	    },
	    {
	        name: "lat",
	        mapping: 'lat'
	    },
	    {
	        name: "lon",
	        mapping: 'lon'
	    }]
    });

    // 创建数据源
    var layerStore = Ext.create('Ext.data.Store', {
        model: 'layer',

        // 设置分页大小
        pageSize: 10,
        proxy: {
            type: 'ajax',
            url: '',
            reader: {
                // 数据格式为json
                type: 'json',
                root: 'rows',
                // 获取数据总数
                totalProperty: 'records'
            }
        },
        autoLoad: true
    });

    layerStore.on("load", function () {
        if (layerStore.getAt(0) != null) {
            boardMaster = layerStore.getAt(0).get('id');
            if (boardMaster == -1) {
                alert("连接超时！！！请重新登陆");
                top.location = "login.html";
            };
        }
    });

    /*mystore.each(function(result){ 
        alert(result.get('age'));   // 遍历出mystore中age的值 
    }) 
 */

    //地名地址查询panel
    var grid = new Ext.grid.Panel({
        width: 290,
        id: "grid_layer",
        height: 570,
        renderTo: Ext.getBody(),
        store: layerStore,
        tbar: [
        {
            xtype: 'textfield',
            fieldLabel: '地名',
            id: 'nameTxtId',//字段组件id   
            labelSeparator: '：',//分隔符   
            labelWidth: 40,//标签宽度   
            width: 200
        },

        {
            xtype: 'button',
            text: '查询',
            handler: function () {
                grid.store.proxy.url = encodeURI("getAdressResult.ct?queryCondition=" + Ext.getCmp("nameTxtId").value);
                grid.store.reload();		// 重新加载grid

            }
        }
        ],
        columns: [{
            text: '地名',
            dataIndex: 'name',
            sortable: false,
            width: 100
        }, {
            text: '地址',
            dataIndex: 'address',
            sortable: false,
            width: 250
        }]
    });

    grid.addListener('cellclick', gridCellclick);

    newLayerNameWin = new Ext.Window({
        name: 'layerNameWin',
        id: 'layerNameWin',
        autoScroll: true,
        layout: 'fit',
        width: 290,
        height: 570,
        x: 0,
        y: 71,
        modal: false,// 4.12
        title: '地名地址查询',
        items: grid
    });
    //点击”关闭“		
    newLayerNameWin.on("close", function () {
        clearQueryLabelLayer();
    });
    var i = 0;
    //显示地"名地址查询"窗口
    closeNowWin();
    newLayerNameWin.show();
    //关闭其他窗口

    nowView = newLayerNameWin;
    // dragWindow(nowView);
}

//单击地名地址列表，定位
function gridCellclick(d) {
    var records = d.getSelectionModel().getSelection();
    layerNamePan(records);
}

function layerNamePan(records) {
    // var xy = new NXY(records[0].data.lon, records[0].data.lat);
    myMap.infoWindow.hide();
    $('#map_graphics_layer > path').remove();
    // 设置标注信息提示窗体内容
    str1 = "" + "名称:" + records[0].data.name + "<br>地址:"
            + records[0].data.address;
    var x = parseFloat(records[0].data.lon);
    var y = parseFloat(records[0].data.lat);
    var CurPos = new esri.geometry.Point(x, y, myMap.spatialReference);
    myMap.infoWindow.setTitle("详细信息");
    myMap.infoWindow.setContent(str1);
    myMap.infoWindow.show(CurPos);
    myMap.setZoom(13);//15	
    myMap.centerAt(new esri.geometry.Point({ "x": x, "y": y, "spatialReference": { "wkid": 4610 } }));

    var sms = new esri.symbol.PictureMarkerSymbol("images/markerQuery.png", 25, 41);
    var json = { title: "详细信息", content: str1 }
    var infoTemplate = new esri.InfoTemplate(json);
    var graphic = new esri.Graphic(CurPos, sms, { "id": "110" }, infoTemplate);
    queryAdressLayer.add(graphic);
    // queryAdressLayer.on("click", function(evt) {
    // $('#map_graphics_layer > path').remove();
    // });
    myMap.addLayer(queryAdressLayer);
    myMap.infoWindow.show();
    $(".action").remove();
}
//关闭地名地址查询窗口
function clearQueryLabelLayer() {
    queryAdressLayer.clear();
    myMap.infoWindow.hide();
}