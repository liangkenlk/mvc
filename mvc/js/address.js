function addsearch() {

    var keyword = $('#inputkeyword').val();
    var town = $('#town').find("option:selected").val();
    //$("#treelist").hide();
    $("#addlist").show();

    
    // $('#addInfo').datagrid("load", { keyword: keyword, method: "addsearch", town: town });
    loadaddInfo(keyword);
}

function addunsearch()
{
    //$("#addlist").show();
    $("#addlist").hide();
}

// 项目加载数据
function loadaddInfo(keyword) {
    $('#addInfo').datagrid({
        height: 550,
        width: 290,
        url: rootUrl+'Server/wfsHandler.ashx',
        method: 'POST',
        queryParams: { method: 'addsearch', keyword: keyword },
        idField: 'id',
        striped: true,
        fitColumns: true,
        singleSelect: true,
        rownumbers: true,
        pagination: true,
        pageSize: 20,
        nowrap: true,
        showFooter: false,
        columns: [[

                { field: 'id', width: 30, checkbox: true },
                { field: 'addname', title: '地名', width: 90, align: 'left' },
                //{ field: 'add', title: '地址', width: 90, align: 'left' }

        ]],
        onLoadSuccess:loaded,
        onCheck: Pro_check,
        onUncheck:Pro_uncheck,
        onCheckAll:Pro_check,
        onUncheckAll: Pro_unallcheck
    });
    var pager = $("#addInfo").datagrid("getPager");
    pager.pagination({ 
        layout: ['first', 'links', 'last'],
        displayMsg:""
    }); 

    //隐藏该列显示
    // $("#projectInfo").datagrid("hideColumn", "Geometry");
    //$("#projectInfo").datagrid("hideColumn", "geoType");
    $("#typetree").tree({
        data: treeData,
        onClick:typeSelect
    })
    
}

function typeSelect(node)
{
    //$("#treelist").hide();
    $("#SeachList").show();


    $('#addInfo').datagrid("load", { type: node.text, method: "AddQuery" });
}
var treeData = [];
function loaded(data)
{
    //return;
    anlysisDrawPolygon.clear();
    var dic = {};
    //dic["addname"] = "地名";
    //dic["type"] = "地名类别";
    //dic["bigtype"] = "大类";
    //dic["comefrom"] = "地名来历";
    //dic["means"] = "地名的含义";
    //dic["his"] = "地名的历史";
    //dic["time"] = "登记时间";

    for (var i = 0; i < data.rows.length; i++) {
        var infoTemplate = new esri.InfoTemplate();
        infoTemplate.setTitle("详细信息");
        //dic = JSON.parse(data.rows[i]["KeyValue"]);
        dic["地名"] = data.rows[i]["addname"];
        var nodeData = data.rows[i];

        var divStr = GetInfoWindowContent(dic, dic, "Pro_detailWin");
        infoTemplate.setContent(divStr);

        //var infoTemplate = null;
        nodeData["geotype"] = "point";
        var LatlngData = LatlngParse(nodeData.latlng, nodeData.geotype);
        var attr = { id: nodeData.id };
        if (null === nodeData.geotype || "point" === nodeData.geotype) {
            addPointGeometry(LatlngData, attr, infoTemplate, anlysisDrawPolygon);
            panto(LatlngData[1], LatlngData[0]);
        }
        if (nodeData.geotype == "line") {
            addPolyLineGeometry(LatlngData, attr, null, infoTemplate, anlysisDrawPolygon);
            panto(LatlngData[0][0][1], LatlngData[0][0][0]);
        }
        if (nodeData.geotype == "polygon") {
            addPolygonGeometry(LatlngData, attr, null, infoTemplate, anlysisDrawPolygon);
            panto(LatlngData[0][0][1], LatlngData[0][0][0]);
        }
        myMap.addLayer(anlysisDrawPolygon);
        $(".action").remove();
        $(".maximize").remove();
    }
}

function Pro_check(rowIndex,rowData)
{
    var a = $('#addInfo').datagrid("getChecked");
    var dic = {};
    //dic["addname"] = "地名";
    //dic["type"] = "地名类别";
    //dic["bigtype"] = "大类";
    //dic["comefrom"] = "地名来历";
    //dic["means"] = "地名的含义";
    //dic["his"] = "地名的历史";
    //dic["time"] = "登记时间";
    //dic = JSON.parse(rowData["KeyValue"]);
  
    dic["地名"] = rowData["addname"];
    ////dic["地址"] = rowData["add"];
    for (var i = 0; i < a.length; i++) {
 
        var nodeData = a[i];
        var divStr = GetInfoWindowContent (dic,"Pro_detailWin");
        myMap.infoWindow.setTitle("详细信息");
        myMap.infoWindow.setContent(divStr);



        var LatlngData = LatlngParse(nodeData.latlng, "point");
        var attr = { id: nodeData.id };
        if (nodeData.geotype == null || nodeData.geotype == "point") {
            var CurPos = new esri.geometry.Point(LatlngData[0], LatlngData[1], myMap.spatialReference);
            myMap.infoWindow.show(CurPos);
            myMap.infoWindow.show();
            panto(LatlngData[1], LatlngData[0]);
        }
        if (nodeData.geotype == "line") {
            var CurPos = new esri.geometry.Point(LatlngData[0][0][0], LatlngData[0][0][1], myMap.spatialReference);
            myMap.infoWindow.show(CurPos);
            myMap.infoWindow.show();
            panto(LatlngData[0][0][1], LatlngData[0][0][0]);
        }
        if (nodeData.geotype == "polygon") {
            var polygonJson = { "rings": nodeData, "spatialReference": myMap.spatialReference };
            var polygon = new esri.geometry.Polygon(polygonJson);
            var CurPos = polygon.getCentroid();
            myMap.infoWindow.show(CurPos);
            myMap.infoWindow.show();
            panto(LatlngData[0][0][1], LatlngData[0][0][0]);
        }
        myMap.addLayer(featureLayer);
        $(".action").remove();
        $(".maximize").remove();
    }
}

function Pro_uncheck(rowIndex, rowData)
{
    //for (var i = 0; i < featureLayer.graphics.length; i++) {
    //    if (featureLayer.graphics[i].attributes.id == rowData.id) {
    //        featureLayer.remove(featureLayer.graphics[i]);
    //        i--;
    //    }
    //}
    //return;
}
function Pro_unallcheck(rowIndex, rowData) {
    //for (var i = 0; i < featureLayer.graphics.length; i++) {
        
    //        featureLayer.remove(featureLayer.graphics[i]);
    //        i--;
        
    //}
    //return;
}

function Pro_detailWin(id) {
    $("#iframe").attr("src", 'pages/projectinfo.html?id=' + id);
    $('#win').window({
        width: 800,
        height: 400,
        modal: true
    });
    $('#win').window('open');
    //$('#win').window('refresh', 'templatedefine.html?id='+id);
}