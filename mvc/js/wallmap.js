function wallMapTree()
{
    var url = "server/configHandler.ashx?method=GetAllRegion";
    j({}, url, wallMapTreeCallBack);

}
function wallMapTreeCallBack(data)
{
    $('#wallmaptree').tree({
        data: data,
        loadFilter: wallMapLoadFilter,
        onSelect: walltreeselect,
        formatter: wallformatter
        //lines: true,
        //checkbox:true
    });
  
    //var center = getPolygenCenter(data[0].latlng);
    

   // for(var i = 0;i<data.length;i++)
   // {
       // var LatlngData = LatlngParse(data[i].latlng, '面');
        //var polygonJson = { "rings": LatlngData, "spatialReference": { "wkid": 4610 } };
       // var polygon = new esri.geometry.Polygon(polygonJson);
       // addPolygonGeometry(LatlngData, { id: data[i].name }, null, null, wallmapLayer);
        
   // }
   // wallmap.setExtent(myMap.extent);
   // wallmap.addLayer(wallmapLayer);
    
   // wallmap.centerAndZoom(center, data[0].level);
}
function walltreeselect(node)
{
   // wallmapRemoveFlashGeo();
   // wallmapflashGeo(node.attributes["latlng"]);
   // var center = getPolygenCenter(node.attributes["latlng"]);
    // wallmap.centerAndZoom(center, node.attributes["level"]);
    var url = "server/wfsHandler.ashx?method=wallmapselect";
    //var fieldName = "TOWN";
    //if (node.attributes["latlng"] == "云安村界2000坐标_GK2_V2014")
    //    fieldName = "行政区名称";
    //j({ layerName: node.attributes["latlng"], fieldName: fieldName, name: node["text"] }, url, walltreeselectCallBack);
    j({id:node.id},url,walltreeselectCallBack);
}
function walltreeselectCallBack(re)
{
    removeFlashGeo();
    if (re.Success != false) {
       
       flashGeo(re.latlng);
        
        //var latlondata = LatlngParse(re["latlng"]);
        //addPolygonGeometry(latlondata, null, null, null, anlysisDrawPolygon, 0.5);
       
    }
    //var  = getPolygenCenter(re.latlng);
    
    var node = $('#wallmaptree').tree('getSelected');
    var x = parseFloat(node["attributes"].center.split(',')[0]);
    var y = parseFloat(node["attributes"].center.split(',')[1]);
    
    var center = new esri.geometry.Point(x, y, myMap.spatialReference);
    addLabelToLayer(center, node.attributes.name, { id: 'XXX' }, null, anlysisDrawPolygon);
    myMap.centerAndZoom(center, node["attributes"].level-startLevel);
    myMap.addLayer(anlysisDrawPolygon);
}

function wallformatter(node)
{
    var str = "<div style='width:150px;height:50px'><span>" + node.text + "</span><a style='float:right' href=javascript:wallmapwin('" + node.attributes.regionId + "')></a></div>";

    if (node.attributes.wallmap != null && node.attributes.wallmap != "")
     str = "<div style='width:150px;height:50px'><span>" + node.text + "</span><a style='float:right' href=javascript:wallmapwin('" + node.attributes.regionId + "')>查看挂图</a></div>";
    //str += '<br><input class="easyui-slider" value="12" style="width:100px,height:20px" data-options="showTip:false">';
    return str;
}

function wallmapwin(id)
{
    $("#iframe").attr("src", "wallmap.aspx?id=" + id);
    $('#win').window('open');

}
function wallMapLoadFilter(data)
{
    var treedata = [];
    for(var i=0;i<data.length;i++)
    {
        if (data[i].pid == 0)
        {
            var nodedata = {};
            nodedata["id"] = data[i].regionId;
            nodedata["text"] = data[i].name;
            nodedata["state"] = "closed";
            nodedata["attributes"] = data[i];
            nodedata["iconCls"] = "";
            treedata.push(nodedata);
            creatChildTree(treedata, data, nodedata)
        }
    }
    return treedata;
}

function creatChildTree(treedata,data,nodedata)
{
    for (var i = 0; i < data.length; i++) {
        
        if (data[i].pid == nodedata["attributes"]["regionId"]) {
            if (nodedata["children"] == null)
                nodedata["children"] = [];
            var childdata = {};
            childdata["id"] = data[i].regionId;
            childdata["text"] = data[i].name;
            childdata["state"] = "open";
            childdata["attributes"] = data[i];
            nodedata["children"].push(childdata);
            creatChildTree(treedata, data, childdata);
        }
    }
}

//取消高亮图斑
function wallmapRemoveFlashGeo() {
    for (var i = 0; i < wallmapLayer.graphics.length; i++) {
        if (wallmapLayer.graphics[i].attributes != null && wallmapLayer.graphics[i].attributes.id == "XXX")
            wallmapLayer.remove(wallmapLayer.graphics[i]);
    }
    myMap.infoWindow.hide();
}
//高亮图斑
function wallmapflashGeo(strPolygon)//空格隔开经纬度
{

    var LatlngData = LatlngParse(strPolygon, '面');
    addPolygonGeometry(LatlngData, { id: "XXX" }, null, null, wallmapLayer,1);

}