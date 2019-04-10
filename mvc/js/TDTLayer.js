

/*
dojo.declare("basicTranLayer", esri.layers.TiledMapServiceLayer, { 
        constructor: function() { 
          this.spatialReference = new esri.SpatialReference({ wkid:4610 }); 
          this.initialExtent = (this.fullExtent = new esri.geometry.Extent(-180.0, -90.0, 180.0, 90.0, this.spatialReference)); 
  
          this.tileInfo = new esri.layers.TileInfo({ 
            "rows" : 256, 
            "cols" : 256, 
            "compressionQuality" : 0, 
            "origin" : { 
              "x" : -180, 
              "y" : 90 
            }, 
            "spatialReference" : { 
              "wkid" : 4610 
            }, 
            "lods" : [ 
             {"level" : 1, "resolution" : 0.703125, "scale" : 295497593.05875003}, 
              {"level" : 2, "resolution" : 0.3515625, "scale" : 147748796.52937502}, 
              {"level" : 3, "resolution" : 0.17578125, "scale" : 73874398.264687508}, 
              {"level" : 4, "resolution" : 0.087890625, "scale" : 36937199.132343754}, 
              {"level" : 5, "resolution" : 0.0439453125, "scale" : 18468599.566171877}, 
              {"level" : 6, "resolution" : 0.02197265625, "scale" : 9234299.7830859385}, 
              {"level" : 7, "resolution" : 0.010986328125, "scale" : 4617149.8915429693}, 
              {"level" : 8, "resolution" : 0.0054931640625, "scale" : 2308574.9457714846}, 
              {"level" : 9, "resolution" : 0.00274658203125, "scale" : 1154287.4728857423}, 
              {"level" : 10, "resolution" : 0.001373291015625, "scale" : 577143.73644287116}, 
              {"level" : 11, "resolution" : 0.0006866455078125, "scale" : 288571.86822143558}, 
              {"level" : 12, "resolution" : 0.00034332275390625, "scale" : 144285.93411071779}, 
              {"level" : 13, "resolution" : 0.000171661376953125, "scale" : 72142.967055358895}, 
              {"level" : 14, "resolution" : 8.58306884765625e-005, "scale" : 36071.483527679447}, 
              {"level" : 15, "resolution" : 4.291534423828125e-005, "scale" : 18035.741763839724}, 
              {"level" : 16, "resolution" : 2.1457672119140625e-005, "scale" : 9017.8708819198619}, 
              {"level" : 17, "resolution" : 1.0728836059570313e-005, "scale" : 4508.9354409599309}, 
              {"level" : 18, "resolution" : 5.3644180297851563e-006, "scale" : 2254.4677204799655},
              {"level" : 19, "resolution" : 2.68220901489e-006, "scale" : 1127.23386023},
              {"level" : 20, "resolution" : 1.34110450744e-006, "scale" : 563.616930115} 
            ] 
          }); 
  
          this.loaded = true; 
          this.onLoad(this); 
        }, 
  
        getTileUrl: function(level, row, col) {           
          return "images/basic.png";   
        } 
      });


*/


dojo.declare("TDTLayer", esri.layers.TiledMapServiceLayer, { 
        constructor: function() { 
          this.spatialReference = new esri.SpatialReference({ wkid:4326 }); 
          this.initialExtent = (this.fullExtent = new esri.geometry.Extent(-180.0, -90.0, 180.0, 90.0, this.spatialReference)); 
  
          this.tileInfo = new esri.layers.TileInfo({ 
            "rows" : 256, 
            "cols" : 256, 
            "compressionQuality" : 0, 
            "origin" : { 
              "x" : -180, 
              "y" : 90 
            }, 
            "spatialReference" : { 
              "wkid" : 4326 
            }, 
            "lods" : [ 
              {"level" : 1, "resolution" : 0.703125, "scale" : 295497593.05875003}, 
              {"level" : 2, "resolution" : 0.3515625, "scale" : 147748796.52937502}, 
              {"level" : 3, "resolution" : 0.17578125, "scale" : 73874398.264687508}, 
              {"level" : 4, "resolution" : 0.087890625, "scale" : 36937199.132343754}, 
              {"level" : 5, "resolution" : 0.0439453125, "scale" : 18468599.566171877}, 
              {"level" : 6, "resolution" : 0.02197265625, "scale" : 9234299.7830859385}, 
              {"level" : 7, "resolution" : 0.010986328125, "scale" : 4617149.8915429693}, 
              {"level" : 8, "resolution" : 0.0054931640625, "scale" : 2308574.9457714846}, 
              {"level" : 9, "resolution" : 0.00274658203125, "scale" : 1154287.4728857423}, 
              {"level" : 10, "resolution" : 0.001373291015625, "scale" : 577143.73644287116}, 
              {"level" : 11, "resolution" : 0.0006866455078125, "scale" : 288571.86822143558}, 
              {"level" : 12, "resolution" : 0.00034332275390625, "scale" : 144285.93411071779}, 
              {"level" : 13, "resolution" : 0.000171661376953125, "scale" : 72142.967055358895}, 
              {"level" : 14, "resolution" : 8.58306884765625e-005, "scale" : 36071.483527679447}, 
              {"level" : 15, "resolution" : 4.291534423828125e-005, "scale" : 18035.741763839724}, 
              {"level" : 16, "resolution" : 2.1457672119140625e-005, "scale" : 9017.8708819198619}, 
              {"level" : 17, "resolution" : 1.0728836059570313e-005, "scale" : 4508.9354409599309}, 
              {"level" : 18, "resolution" : 5.3644180297851563e-006, "scale" : 2254.4677204799655},
              {"level" : 19, "resolution" : 2.68220901489e-006, "scale" : 1127.23386023},
              {"level" : 20, "resolution" : 1.34110450744e-006, "scale" : 563.616930115} 
            ] 
          }); 
  
          this.loaded = true; 
          this.onLoad(this); 
        }, 
  
        getTileUrl: function(level, row, col) { 
                var levelMap = this.url; 
               /* if(level<10){ 
                        levelMap = "A0512_EMap"; 
                }else if(level<12){ 
                        levelMap = "B0627_EMap1112"; 
                }else if(level<17){ 
                        levelMap = "siwei0608"; 
                } */
          return "http://t1.tianditu.com/DataServer?T="+levelMap+"&"+"X=" + col + "&" + 
                  "Y=" + row + "&" + "L=" + level; 
  
        } 
      }); 
        
      
      
      
   
      
      
       
      
      
      dojo.declare("WMTSExLayer", esri.layers.TiledMapServiceLayer, { 
        constructor: function() { 
          this.spatialReference = new esri.SpatialReference({ wkid:4610 }); 
          this.initialExtent = (this.fullExtent = new esri.geometry.Extent(-180.0, -90.0, 180.0, 90.0, this.spatialReference)); 
  
          this.tileInfo = new esri.layers.TileInfo({ 
            "rows" : 256, 
            "cols" : 256, 
            "compressionQuality" : 0, 
            "origin" : { 
              "x" : -180, 
              "y" : 90 
            }, 
            "spatialReference" : { 
              "wkid" : 4610 
            }, 
            "lods" : [ 
              {"level" : 1, "resolution" : 0.703125, "scale" : 295497593.05875003}, 
              {"level" : 2, "resolution" : 0.3515625, "scale" : 147748796.52937502}, 
              {"level" : 3, "resolution" : 0.17578125, "scale" : 73874398.264687508}, 
              {"level" : 4, "resolution" : 0.087890625, "scale" : 36937199.132343754}, 
              {"level" : 5, "resolution" : 0.0439453125, "scale" : 18468599.566171877}, 
              {"level" : 6, "resolution" : 0.02197265625, "scale" : 9234299.7830859385}, 
              {"level" : 7, "resolution" : 0.010986328125, "scale" : 4617149.8915429693}, 
              {"level" : 8, "resolution" : 0.0054931640625, "scale" : 2308574.9457714846}, 
              {"level" : 9, "resolution" : 0.00274658203125, "scale" : 1154287.4728857423}, 
              {"level" : 10, "resolution" : 0.001373291015625, "scale" : 577143.73644287116}, 
              {"level" : 11, "resolution" : 0.0006866455078125, "scale" : 288571.86822143558}, 
              {"level" : 12, "resolution" : 0.00034332275390625, "scale" : 144285.93411071779}, 
              {"level" : 13, "resolution" : 0.000171661376953125, "scale" : 72142.967055358895}, 
              {"level" : 14, "resolution" : 8.58306884765625e-005, "scale" : 36071.483527679447}, 
              {"level" : 15, "resolution" : 4.291534423828125e-005, "scale" : 18035.741763839724}, 
              {"level" : 16, "resolution" : 2.1457672119140625e-005, "scale" : 9017.8708819198619}, 
              {"level" : 17, "resolution" : 1.0728836059570313e-005, "scale" : 4508.9354409599309}, 
              {"level" : 18, "resolution" : 5.3644180297851563e-006, "scale" : 2254.4677204799655},
              {"level" : 19, "resolution" : 2.68220901489e-006, "scale" : 1127.23386023},
              {"level" : 20, "resolution" : 1.34110450744e-006, "scale" : 563.616930115}  
            ] 
          }); 
  
          this.loaded = true; 
          this.onLoad(this); 
        }, 
  
        getTileUrl: function(level, row, col) { 
          var tempurl=this.url;
          tempurl= "http://map.maoming.gov.cn:8719/wmts/" + tempurl + "/Default/epsg:4610/";
          return tempurl+level+"/" + row + "/" + col +".png";   
        } 
      }); 
      
//url = this.url +"?SERVICE=" + this.options.service + "&REQUEST=" + this.options.request + "&VERSION=" + this.options.version + "&LAYER=" + this.options.layer + "&STYLE=" + this.options.style + "&FORMAT=" + this.options.formats + "&TILEMATRIXSET=" + this.options.tileMatrixSet + "&TILEMATRIX=" + tile_z + "&TILEROW=" + tile_y + "&TILECOL=" + tile_x;
      
      //专题图层
      dojo.declare("ThemeWMTSExLayer", esri.layers.TiledMapServiceLayer, { 
        constructor: function() { 
          this.spatialReference = new esri.SpatialReference({ wkid:4610 }); 
          this.initialExtent = (this.fullExtent = new esri.geometry.Extent(-180.0, -90.0, 180.0, 90.0, this.spatialReference)); 
  
          this.tileInfo = new esri.layers.TileInfo({ 
            "rows" : 256, 
            "cols" : 256, 
            "compressionQuality" : 0, 
            "origin" : { 
              "x" : -180, 
              "y" : 90 
            }, 
            "spatialReference" : { 
              "wkid" : 4610 
            }, 
            "lods" : [ 
              {"level" : 1, "resolution" : 0.703125, "scale" : 295497593.05875003}, 
              {"level" : 2, "resolution" : 0.3515625, "scale" : 147748796.52937502}, 
              {"level" : 3, "resolution" : 0.17578125, "scale" : 73874398.264687508}, 
              {"level" : 4, "resolution" : 0.087890625, "scale" : 36937199.132343754}, 
              {"level" : 5, "resolution" : 0.0439453125, "scale" : 18468599.566171877}, 
              {"level" : 6, "resolution" : 0.02197265625, "scale" : 9234299.7830859385}, 
              {"level" : 7, "resolution" : 0.010986328125, "scale" : 4617149.8915429693}, 
              {"level" : 8, "resolution" : 0.0054931640625, "scale" : 2308574.9457714846}, 
              {"level" : 9, "resolution" : 0.00274658203125, "scale" : 1154287.4728857423}, 
              {"level" : 10, "resolution" : 0.001373291015625, "scale" : 577143.73644287116}, 
              {"level" : 11, "resolution" : 0.0006866455078125, "scale" : 288571.86822143558}, 
              {"level" : 12, "resolution" : 0.00034332275390625, "scale" : 144285.93411071779}, 
              {"level" : 13, "resolution" : 0.000171661376953125, "scale" : 72142.967055358895}, 
              {"level" : 14, "resolution" : 8.58306884765625e-005, "scale" : 36071.483527679447}, 
              {"level" : 15, "resolution" : 4.291534423828125e-005, "scale" : 18035.741763839724}, 
              {"level" : 16, "resolution" : 2.1457672119140625e-005, "scale" : 9017.8708819198619}, 
              {"level" : 17, "resolution" : 1.0728836059570313e-005, "scale" : 4508.9354409599309}, 
              {"level" : 18, "resolution" : 5.3644180297851563e-006, "scale" : 2254.4677204799655},
              {"level" : 19, "resolution" : 2.68220901489e-006, "scale" : 1127.23386023},
              {"level" : 20, "resolution" : 1.34110450744e-006, "scale" : 563.616930115} 
            ] 
          }); 
  
          this.loaded = true; 
          this.onLoad(this); 
        }, 
  
        getTileUrl: function(level, row, col) { 
          var tempurl=eval('(' + this.url + ')');
          //tempurl=tempurl + "/Default/epsg:4610/";
          return tempurl.url+"?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER="+tempurl.name+"&STYLE=" + tempurl.name + "&TILEMATRIXSET=Matrix_0&TILEMATRIX="+level+"&TILEROW="+(row-256)+"&TILECOL="+(col+256*2)+"&FORMAT=image%2Fpng";
          //return tempurl+level1+"/" + row + "/" + col +".png";   
        } 
      }); 
      
      //切片图层
      dojo.declare("WallMapWMTSExLayer", esri.layers.TiledMapServiceLayer, { 
        constructor: function() { 
          this.spatialReference = new esri.SpatialReference({ wkid:4610 }); 
          this.initialExtent = (this.fullExtent = new esri.geometry.Extent(-180.0, -90.0, 180.0, 90.0, this.spatialReference)); 
  
          this.tileInfo = new esri.layers.TileInfo({ 
            "rows" : 256, 
            "cols" : 256, 
            "compressionQuality" : 0, 
            "origin" : { 
              "x" : -180, 
              "y" : 90 
            }, 
            "spatialReference" : { 
              "wkid" : 4610 
            }, 
            "lods" : [ 
              {"level" : 1, "resolution" : 0.703125, "scale" : 295497593.05875003}, 
              {"level" : 2, "resolution" : 0.3515625, "scale" : 147748796.52937502}, 
              {"level" : 3, "resolution" : 0.17578125, "scale" : 73874398.264687508}, 
              {"level" : 4, "resolution" : 0.087890625, "scale" : 36937199.132343754}, 
              {"level" : 5, "resolution" : 0.0439453125, "scale" : 18468599.566171877}, 
              {"level" : 6, "resolution" : 0.02197265625, "scale" : 9234299.7830859385}, 
              {"level" : 7, "resolution" : 0.010986328125, "scale" : 4617149.8915429693}, 
              {"level" : 8, "resolution" : 0.0054931640625, "scale" : 2308574.9457714846}, 
              {"level" : 9, "resolution" : 0.00274658203125, "scale" : 1154287.4728857423}, 
              {"level" : 10, "resolution" : 0.001373291015625, "scale" : 577143.73644287116}, 
              {"level" : 11, "resolution" : 0.0006866455078125, "scale" : 288571.86822143558}, 
              {"level" : 12, "resolution" : 0.00034332275390625, "scale" : 144285.93411071779}, 
              {"level" : 13, "resolution" : 0.000171661376953125, "scale" : 72142.967055358895}, 
              {"level" : 14, "resolution" : 8.58306884765625e-005, "scale" : 36071.483527679447}, 
              {"level" : 15, "resolution" : 4.291534423828125e-005, "scale" : 18035.741763839724}, 
              {"level" : 16, "resolution" : 2.1457672119140625e-005, "scale" : 9017.8708819198619}, 
              {"level" : 17, "resolution" : 1.0728836059570313e-005, "scale" : 4508.9354409599309}, 
              {"level" : 18, "resolution" : 5.3644180297851563e-006, "scale" : 2254.4677204799655},
              {"level" : 19, "resolution" : 2.68220901489e-006, "scale" : 1127.23386023},
              {"level" : 20, "resolution" : 1.34110450744e-006, "scale" : 563.616930115} 
            ] 
          }); 
  
          this.loaded = true; 
          this.onLoad(this); 
        }, 
  
        getTileUrl: function(level, row, col) { 
          var tempurl= this.url;
          //tempurl=tempurl + "/Default/epsg:4610/";
          if(level <10){
          return tempurl+"/wfs/L0"+level+"/" + row + "_" + col +".png";
          }else{
          return tempurl+"/wfs/L"+level+"/" + row + "_" + col +".png";
          }
        } 
      }); 
      
      
      
      
      
      
      
      
      
      
      
      