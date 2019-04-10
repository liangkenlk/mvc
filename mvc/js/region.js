function changeTown() {
    url = "Server/MapHandler.ashx?method=getRegionList";
    j({}, url, changeTownCallBack);
}

function changeTownCallBack(re) {
    var town = JSON.parse(re.data);
   
    for(var i=0;i<town.length;i++){
        town[town[i].name] = [town[i].latlng];
    }
    var val = $("#town").find("option:selected").val();
    panto(town[val][1], town[val][0]);
}
