// All material copyright ESRI, All Rights Reserved, unless otherwise specified.
// See http://js.arcgis.com/3.14/esri/copyright.txt for details.
//>>built
define("esri/dijit/metadata/editor/EditorResizeMixin","dojo/_base/declare dojo/_base/lang dojo/dom-geometry dojo/dom-style dojo/has dojo/window ../../../kernel".split(" "),function(e,g,d,f,k,h,l){e=e(null,{constructor:function(a){g.mixin(this,a)},_getMaxCanvasHeight:function(a){var b=0,c;c=this.validationPane.domNode;this.dialogBroker?(a=d.getMarginBox(this.domNode),b=d.getMarginBox(this.primaryToolbar.domNode),b=a.h-b.h,"none"!==c.style.display&&(c=d.getMarginBox(c),b-=c.h)):(c=h.getBox(this.ownerDocument),
a=d.getMarginBox(a),b=c.h-a.t-10);return b},resizeDocument:function(a){a=a.domNode;var b=this._getMaxCanvasHeight(a);10<b&&f.set(a,"maxHeight",b-10+"px")},resizeXmlPane:function(a){a=this.xmlPane.textAreaNode;var b=this._getMaxCanvasHeight(a);this.dialogBroker||(b-=10);10<b&&f.set(a,"height",b-10+"px")},resize:function(){if(this.dialogBroker){var a=h.getBox(this.ownerDocument);a.w*=0.9;a.h*=0.9;var b=d.getMarginBox(this.domNode),b=a.h-b.t-b.l-30;f.set(this.domNode,"width",a.w-100+"px");f.set(this.domNode,
"height",b+"px")}this.resizeDocument(this.editDocumentPane);this.resizeDocument(this.viewDocumentPane);this.resizeXmlPane()}});k("extend-esri")&&g.setObject("dijit.metadata.editor.EditorResizeMixin",e,l);return e});