// All material copyright ESRI, All Rights Reserved, unless otherwise specified.
// See http://js.arcgis.com/3.14/esri/copyright.txt for details.
//>>built
require({cache:{"url:esri/dijit/metadata/base/templates/TabRadio.html":'\x3cdiv class\x3d"gxeTabButton"\x3e\r\n  \x3cinput id\x3d"${id}_rad" type\x3d"radio" name\x3d"${radioName}" ${checkedAttr} \r\n    data-dojo-attach-point\x3d"radioNode" \r\n    data-dojo-attach-event\x3d"onclick: _onClick"/\x3e\r\n  \x3clabel for\x3d"${id}_rad"  data-dojo-attach-point\x3d"labelNode"\x3e${label}\x3c/label\x3e\r\n\x3c/div\x3e'}});
define("esri/dijit/metadata/base/TabRadio","dojo/_base/declare dojo/_base/lang dojo/dom-attr dojo/has ./TabButton dojo/text!./templates/TabRadio.html ../../../kernel".split(" "),function(a,b,c,d,e,f,g){a=a([e],{checkedAttr:"",radioName:null,templateString:f,postCreate:function(){this.inherited(arguments)},setChecked:function(a){c.set(this.radioNode,"checked",a)}});d("extend-esri")&&b.setObject("dijit.metadata.base.TabRadio",a,g);return a});