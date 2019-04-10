function CityPart_dosearch() {

    var keyword = $('#CityPartkeyword').val();
    var type=$("#CityPartType").find("option:selected").val();
    if (type == "")
        type = null;
    $('#CityPart').datagrid("load", { keyword: keyword, type: type });
}

function initCityPartTypeSelect() {
    var url = 'server/CityPartHandler.ashx?method=GetTypeList';
        j(null,url,initCityPartTypeSelectCallBack);
}
function initCityPartTypeSelectCallBack(re)
{

    //专题下拉框
    var select = $("#CityPartType");
    select.html("");
    for (var i = 0; i < re.length;i++) {

        select.append("<option value='" + re[i].type + "'>" + re[i].type + "</option>");
    }
}
// 项目加载数据
function loadCityPart() {
    if ($('#CityPart').length == 0)
        return;
    initCityPartTypeSelect();
    $('#CityPart').datagrid({
        height: 450,
        width: 270,
        url: 'server/CityPartHandler.ashx?method=GetList',
        method: 'POST',
        //queryParams: { 'id': id },
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
                { field: 'name', title: '名称', width: 90, align: 'left' },
                { field: 'type', title: '类型', width: 90, align: 'left' },

        ]],

        onCheck: CityPart_check,
        onUncheck: CityPart_uncheck,
        onCheckAll: CityPart_check,
        onUncheckAll: CityPart_unallcheck,
        onLoadSuccess: function () { $(".nav ul li:eq(0)").click(); }
    });
    var pager = $("#CityPart").datagrid("getPager");
    pager.pagination({ 
        layout: ['first', 'links', 'last'],
        displayMsg:""
    }); 
    
    //隐藏该列显示
    // $("#projectInfo").datagrid("hideColumn", "Geometry");
    //$("#projectInfo").datagrid("hideColumn", "geoType");
}


function CityPart_check(rowIndex, rowData)
{
    var a = $('#CityPart').datagrid("getChecked");




  
    for (var i = 0; i < a.length; i++) {
        

        var infoTemplate = new esri.InfoTemplate();
        infoTemplate.setTitle("详细信息");

        var nodeData = a[i];
        var dic = JSON.parse(nodeData.json);

        var divStr = GetInfoWindowContent(dic);
        infoTemplate.setContent(divStr);
        var geotype;
        if (nodeData.geotype == "1")
            geotype = "point";
        if (nodeData.geotype == "2")
            geotype = "polygon";
        if (nodeData.geotype == "3")
            geotype = "polygon";


        var LatlngData = LatlngParse(nodeData.latlng, geotype);
        var attr = { id: nodeData.id };
        if (nodeData.geotype == null || nodeData.geotype == "1") {
            addPointGeometry(LatlngData, attr, infoTemplate, featureLayer);
            panto(LatlngData[1], LatlngData[0]);
        }
        if (nodeData.geotype == "2") {
            addPolyLineGeometry(LatlngData, attr, null, infoTemplate, featureLayer);
            panto(LatlngData[0][0][1], LatlngData[0][0][0]);
        }
        if (nodeData.geotype == "3") {
            addPolygonGeometry(LatlngData, attr, null, infoTemplate, featureLayer);
            panto(LatlngData[0][0][1], LatlngData[0][0][0]);
        }
        myMap.addLayer(featureLayer);
        $(".action").remove();
        $(".maximize").remove();
    }
}



function CityPart_uncheck(rowIndex, rowData)
{
    for (var i = 0; i < featureLayer.graphics.length; i++) {
        if (featureLayer.graphics[i].attributes.id == rowData.id) {
            featureLayer.remove(featureLayer.graphics[i]);
            i--;
        }
    }
    return;
}
function CityPart_unallcheck(rowIndex, rowData) {
    for (var i = 0; i < featureLayer.graphics.length; i++) {
        
            featureLayer.remove(featureLayer.graphics[i]);
            i--;
        
    }
    return;
}

