/***********************************************************添加注记*********************************************/
// 项目加载数据 
var isLabelLoaded = false;
function onLabelListShow()
{
    if (!isLabelLoaded)
    {
        loadLabelList();
        isLabelLoaded = true;
    }

}

function loadLabelList() {
    var keyword = $('#labelkeyword').val();
    var shareType = $('#shareType').find("option:selected").val();

    $("#labellist").show();
    $('#labelGrid').datagrid({
        height: 450,
        width: 290,
        url: rootUrl + 'server/MapHandler.ashx',
        method: 'POST',
        queryParams: { method: 'getLabelList', keyword: keyword, token: token, isshare: shareType },
        idField: 'id',
        striped: true,
        //fitColumns: true,
        singleSelect: false,
        rownumbers: true,
        pagination: true,
        pageSize: 10,
        toolbar: '#tb',
        nowrap: true,
        showFooter: false,
        columns: [[

                { field: 'id', width: 30, checkbox: true },
                { field: 'name', title: '名称', width: 90, align: 'left' },
                { field: 'UserName', title: '用户', width: 50, align: 'left' },
                { field: 'isShare', title: '类型', width: 50, align: 'left', formatter: shareFormatter }
        ]],
        //onLoadSuccess: loaded,
        onCheck: labelCellclick,
        onUncheck: labelCellclick,
        onCheckAll: labelCellclick,
        onUncheckAll: labelCellclick
    });
    var pager = $("#labelGrid").datagrid("getPager");
    pager.pagination({
        layout: ['first', 'links', 'last'],
        displayMsg: ""
    });
}
function shareFormatter(value, row, index)
{
    if (value == 1) {
        return '共享';
    }
    else
        return '私有';
}
function addLabel() {
    showaddLabelTypeWin();
}


var addLabelWin;
var typeName = "";
var type;
function showaddLabelTypeWin() {
    xw = document.body.offsetWidth - 10;

    var addLabelStr = '&nbsp;&nbsp;&nbsp;<a onclick=javascript:toDrawShape(' + '\'point\'' + ');><input type="radio" name="css" id=' + 'ccc_1' + ' /></a><label for="' + 'ccc_1' + '">' + '点' + '</label><br><br>' +
        '&nbsp;&nbsp;&nbsp;<a onclick=javascript:toDrawShape(' + '\'line\'' + ');><input type="radio" name="css" id=' + 'ccc_2' + ' /></a><label for="' + 'ccc_2' + '">' + '线' + '</label><br><br>' +
        '&nbsp;&nbsp;&nbsp;<a onclick=javascript:toDrawShape(' + '\'polygon\'' + ');><input type="radio" name="css" id=' + 'ccc_3' + ' /></a><label for="' + 'ccc_3' + '">' + '面' + '</label><br><br>' +
        '&nbsp;&nbsp;&nbsp;<a onclick=javascript:toDrawShape(' + '\'rectangle\'' + ');><input type="radio" name="css" id=' + 'ccc_4' + ' /></a><label for="' + 'ccc_4' + '">' + '矩形' + '</label><br><br>' +
        '&nbsp;&nbsp;&nbsp;<a onclick=javascript:toDrawShape(' + '\'circle\'' + ');><input type="radio" name="css" id=' + 'ccc_5' + ' /></a><label for="' + 'ccc_5' + '">' + '圆' + '</label><br/><br>' +
        // '&nbsp;&nbsp;&nbsp;<a onclick=javascript:changCheck('+'\'工业用地\''+');><input type="radio" name="cs" id='+'cc_2'+' /></a><label for="'+'cc_2'+'">'+'多边形'+'</label>'+
        '&nbsp;&nbsp<input type="button" value="取消" onclick="javascript:addLabelWinCancel();"/>'
    closeNowWin();
    $("#addLabel").html(addLabelStr);
    var addLabelWin = $("#addLabel").window('open');


    nowView = addLabelWin;
    dragWindow(addLabelWin);
}

function toDrawShape(shape) {

    //var dragToolbar;
    //添加面标签工具
    dragToolbar = new esri.toolbars.Draw(myMap, { showTooltips: true });
    dragToolbar.respectDrawingVertexOrder = true;
    dragToolbar.on("draw-end", mouseDragEnd);
    dragToolbar.lineSymbol = new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID, new dojo.Color([0, 0, 255]), 2);
    dragToolbar.fillSymbol = new esri.symbol.SimpleFillSymbol(esri.symbol.SimpleFillSymbol.STYLE_SOLID,
            new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID,
        new dojo.Color([0, 0, 255]), 2), new dojo.Color([0, 0, 255, 0.15]));

    if (shape == "point") {
        typeName = shape;
        type = 1;
        dragToolbar.activate(esri.toolbars.Draw.POINT);

    } else if (shape == "line") {
        typeName = shape;
        type = 2;
        dragToolbar.activate(esri.toolbars.Draw.POLYLINE);
    } else if (shape == "polygon") {
        typeName = shape;
        type = 3;
        dragToolbar.activate(esri.toolbars.Draw.POLYGON);
    } else if (shape == "rectangle") {
        typeName = "polygon";
        type = 3;
        dragToolbar.activate(esri.toolbars.Draw.RECTANGLE);

    } else if (shape == "circle") {
        typeName = "polygon";
        type = 3;
        dragToolbar.activate(esri.toolbars.Draw.CIRCLE);
    }
    closeNowWin();
}
var nowView;
//关闭当前的窗口
function closeNowWin() {
    if (nowView) {
        nowView.window('close');
    }
}

var latlng = "";
function mouseDragEnd(evt) {
    latlng = "";
    // dragToolbar.activate(esri.toolbars.Draw.RECTANGLE);
    // $(".tooltip").remove();
    var symbol;
    dragToolbar.deactivate();
    myMap.showZoomSlider();
    switch (evt.geometry.type) {
        case "point":
        case "multipoint":
            // symbol = new esri.symbol.SimpleMarkerSymbol();
            // var CurPos = new esri.geometry.Point(data[0], data[1], myMap.spatialReference);
            symbol = new esri.symbol.PictureMarkerSymbol("images/point/default.png",16,16);
            break;
        case "polyline":
            symbol = new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID, new dojo.Color([0, 0, 255]), 2);
            break;
        default:
            symbol = new esri.symbol.SimpleFillSymbol(esri.symbol.SimpleFillSymbol.STYLE_SOLID,
              new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID,
          new dojo.Color([0, 0, 255]), 2), new dojo.Color([0, 0, 255, 0.15]));
            break;
    }
    if (evt.geometry.type == "point") {
        latlng = evt.geometry.x + "," + evt.geometry.y;
    }
    else if (evt.geometry.type == "polyline") {
        var points = evt.geometry.paths;
        //for (var i = 0; i < points.length; i++) {
        //    latlng += "[" + points[i][0] + "," + points[i][1] + "],";
        //}
        // latlng = latlng.substr(0, latlng.length - 1);
        latlng = LatlngStringfy(points);
    }
    else {
        var points = evt.geometry.rings;
        //for (var i = 0; i < points.length; i++) {
        //    latlng += "[" + points[i][0] + "," + points[i][1] + "],";
        //}
        //latlng = latlng.substr(0, latlng.length - 1);
        latlng = LatlngStringfy(points);
    }
    //latlng = "[" + latlng + "]";

    var graphic = new esri.Graphic(evt.geometry, symbol);
    addLabelLayer.add(graphic);
    myMap.addLayer(addLabelLayer);
    showAddLabelWin();
}

function addLabelWinCancel() {

    $("#addLabel").window('close');
}

//画完标注 显示添加标记窗口


var addLabelWin;
function showAddLabelWin() {
    var name = "";
    var labelD
    var labelDescribe="";

    var checked="";
    if(editingLabel!=null)
    {
        type = editingLabel.type;
        name = editingLabel.name;
        labelDescribe = editingLabel.labelDescribe;
        latlng = editingLabel.latlng;
        checked = editingLabel.isShare == 1 ? "checked" : "";
    }
    if (type == 1) {
        typeName = "点";
    } else if (type == 2) {
        typeName = "线";
    } else if (type == 3) {
        typeName = "面";
    }

    
    var addLabelPanelStr = '<br>标签名称： <input class="easyui-textbox" type="text" id="name" value="'+name+'" style="width:300px;"/><br/>' +
        '类&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;型： <input class="easyui-textbox" type="text" id="type" value="' + typeName + '"style="width:300px;"/><br/>' +
        '描&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;述： <input class="easyui-textbox" type="text" id="labelDescribe" value="' + labelDescribe + '"  style="width:300px;"/><br/>' +
        '坐&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;标： <input class="easyui-textbox" type="text" id="latlng" style="width:300px;" value="' + latlng + '"/><br/>' +
        '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;是否共享：<input type="checkbox" id = "isshare" ' + checked + '/><br/>' +
        '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type="button" id="saveLabel" value="保存" onclick="saveLabel()"&nbsp;&nbsp;&nbsp;/>' +
        '<input type="button" id="cancelAdd" value="取消" onclick="cancelAdd()"/>';
    $("#addLabelPanel").html(addLabelPanelStr);
    $("#addLabelPanel").window('open');

}

function delLabel() {
    if (!confirm("确定要删除吗？")) {
        return;
    }
    var a = $('#labelGrid').datagrid("getChecked");
    var selected = "";
    for (var i = 0; i < a.length; i++) {
        var id = "'" + a[i].id + "'";
        if (selected != "")
            selected += "," + id;
        else
            selected += id;
    }

    var url = "server/MapHandler.ashx?method=deleteLabel";
    var data = { selected: selected };
    j(data, url, delLabelCallBack);

}
function delLabelCallBack(re) {
    if (re.Success == true)
        $('#labelGrid').datagrid("reload");
    else
        $.messager.show({ title: "提示", msg: "请勾选需要删除的选项" });

}


//保存标签
function saveLabel() {
    // var id = Math.random().toString();
    var url = "server/MapHandler.ashx?method=saveLabel";
    var name = $("#name").val();
    if (name == "") {
        $.messager.alert("提示","请输入名称。");
        return;
    }
    
    var typeName = $("#type").val();
    //var type = $("#type").val();
    var labelDescribe = $("#labelDescribe").val();
    var latlng = $("#latlng").val();
    var isshare = $("#isshare").is(':checked') == true?1:0;
    //var userId = $("#UserId").val();
    
    data = { name: name, typeName: typeName, labelDescribe: labelDescribe, latlng: latlng, isshare: isshare };
    if (editingLabel != null)
        data["id"] = editingLabel.id;
    editingLabel = null;
    j(data, url, saveLabelCallBack);
    

}

function saveLabelCallBack(re) {
    if (re.Success == true) {
        $.messager.show({ title: "提示", msg: "保存成功!" });
        cancelAdd();
        loadLabelList();
    } else {

        $.messager.show({ title: "提示", msg: re.Message });
    }
}

//取消添加标签
function cancelAdd() {
    $("#addLabelPanel").window('close');
    if (addLabelLayer)
        addLabelLayer.clear();
}


//显示标签
function showLabel() {

}

//清除标记图层
function clearDisplayLabelLayer() {
    drawLabelLayer.clear();
}

//标签列表
function LabelList() {
    $('#labelGrid').datagrid("reload");
    $("#labelList").window("open");
}
//显示标记
var temppAnnotationGraphic;
var tempRecord;
function displayLabel(records) {
    myMap.infoWindow.hide();
    tempRecord = records;
    var center;
    for (var i = 0; i < records.length; i++) {
        var data = records[i].latlng;
        var type = records[i].type;
        //var data = LatlngParse(data);
        var typeName;
        if (type == 1) {
            typeName = "点";
        } else if (type == 2) {
            typeName = "线";
        } else if (type == 3) {
            typeName = "面";
        }
        var content = '标签名称： '+records[i].name+'<br/>' +
    '类&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;型： ' + typeName + '<br/>' +
    '描&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;述： ' + records[i].labelDescribe + '<br/>' +
    '时&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;间： ' + records[i].createTime + '<br/>' +
    //'坐&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;标： ' + data + '<br/>';
    '<a href="#" onclick="showPhotoWin('+i+')"><img width=25px src="images/photo.jpg"/>查看照片</a>';
        var infoTemplate = new esri.InfoTemplate("自定义标注", content);
        
        if (type == 1) {
            data = LatlngParse(records[0].latlng, "point");
            addPointGeometry(data, null, infoTemplate, drawLabelLayer);
            center = new esri.geometry.Point(data[0], data[1], myMap.spatialReference);
        }
        else if (type == 2) {
            data = LatlngParse(records[0].latlng, "polyline");
            center = new esri.geometry.Point(data[0][0], data[0][1], myMap.spatialReference);
            addPolyLineGeometry(data, null, null, infoTemplate, drawLabelLayer);
        }
        else {
            data = LatlngParse(records[0].latlng, "polygon");
            center = new esri.geometry.Point(data[0][0], data[0][1], myMap.spatialReference);
            addPolygonGeometry(data, null, null, infoTemplate, drawLabelLayer);
        }
    }
    //var center = new esri.geometry.Point(x, y, myMap.spatialReference);
    //addLabelToLayer(center, node.attributes.name, { id: 'XXX' }, null, anlysisDrawPolygon);
    myMap.centerAndZoom(center);

    //addLabelGeometry(type, latlng, drawLabelLayer, "0,0,255");
    myMap.addLayer(drawLabelLayer);
    $(".maximize").hide();
    $(".action").hide();
}

function labelCellclick() {
    //var records = labelGrid.getSelectionModel().getSelection();
    var records = $('#labelGrid').datagrid("getChecked");
    clearDisplayLabelLayer();
    displayLabel(records);
}

function addLabelGeometry(type, data, layer, color) {
    var r = parseInt(color.split(",")[0]);
    var g = parseInt(color.split(",")[1]);
    var b = parseInt(color.split(",")[2]);
    if (type == 1) {
    
        var CurPos = new esri.geometry.Point(data[0], data[1], myMap.spatialReference);
        myMap.centerAt(CurPos);
        var sms = new esri.symbol.PictureMarkerSymbol("images/point/default.png", 16, 16);
        var graphic = new esri.Graphic(CurPos, sms, null, null);
        temppAnnotationGraphic = graphic;
        layer.add(graphic);
    } else if (type == 2) {

        var CurPos = new esri.geometry.Point(data[0][0], data[0][1], myMap.spatialReference);
        myMap.centerAt(CurPos);
        var polylineJson = { "paths": [data], "spatialReference": spatialReference };
        var polyline = new esri.geometry.Polyline(polylineJson);
        var symbol = new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID, new dojo.Color([r, g, b]), 2);

        var graPolyline = new esri.Graphic(polyline, symbol, null, null);
        // editToolbar.activate(esri.toolbars.Edit.EDIT_VERTICES ,graPolygon);
        temppAnnotationGraphic = graPolyline;
        layer.add(graPolyline);

    } else if (type == 3) {
        var CurPos = new esri.geometry.Point(data[0][0], data[0][1], myMap.spatialReference);
        myMap.centerAt(CurPos);
        var polygonJson = { "rings": [data], "spatialReference": spatialReference };
        var polygon = new esri.geometry.Polygon(polygonJson);

        var symbol = new esri.symbol.SimpleFillSymbol(esri.symbol.SimpleFillSymbol.STYLE_SOLID,
                new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID,
                new dojo.Color([r, g, b]), 2), new dojo.Color([r, g, b, 0.25])
            );
        var graPolygon = new esri.Graphic(polygon, symbol, null, null);
        temppAnnotationGraphic = graPolygon;
        layer.add(graPolygon);

    }

}

//清除
function unShow() {
    graPolygon.clear();
}



//返回编辑后的标注坐标
function getAnnotationLatlng() {
    latlng = "";
    editToolbar.deactivate();
    if (temppAnnotationGraphic.geometry.type == "point") {
        latlng = temppAnnotationGraphic.geometry.x + "," + temppAnnotationGraphic.geometry.y;
    } else if (temppAnnotationGraphic.geometry.type == "polyline") {
        var points = temppAnnotationGraphic.geometry.paths[0];
        for (var i = 0; i < points.length; i++) {
            latlng += "[" + points[i][0] + "," + points[i][1] + "],";
        }
        latlng = latlng.substr(0, latlng.length - 1);
    } else {
        var points = temppAnnotationGraphic.geometry.rings[0];
        for (var i = 0; i < points.length; i++) {
            latlng += "[" + points[i][0] + "," + points[i][1] + "],";
        }
        latlng = latlng.substr(0, latlng.length - 1);

    }
    latlng = "[" + latlng + "]";
    return latlng;
}

var currentRecord;
function showPhotoWin(i)
{
    currentRecord = tempRecord[i];
    currentRecord["tableName"] = "Label";
    $("#iframe").attr("src", "photos.html");
        $('#win').window('setTitle', "现场照片");
        $('#win').window('open');
    
}
//编辑标记--------------------------------------------------------------------------------------
var editingLabel;
function editLabel()
{
    var records = $('#labelGrid').datagrid("getChecked");
    if (records.length != 1) {
        $.messager.show({ title: '提示', msg: '请只选择一条记录' });
    }
    editingLabel = records[0];
    showAddLabelWin();
}

