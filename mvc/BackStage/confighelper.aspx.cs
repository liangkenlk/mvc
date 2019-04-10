using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using TY.Core;
using Web.Server;

namespace Web.BackStage
{
    public partial class confighelper : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string post(string txt, string url)
        {
            WebClient c = new WebClient();
            var r = c.UploadData(url, System.Text.Encoding.UTF8.GetBytes(txt));
            string str = System.Text.Encoding.UTF8.GetString(r);
            return str;
        }
        public string get(string url)
        {
            WebClient c = new WebClient();
            var r = c.DownloadData(url);
            string str = System.Text.Encoding.UTF8.GetString(r);
            return str;
        }


        public XmlNamespaceManager getxnm(XmlNameTable table)
        {
            XmlNamespaceManager xnm = new XmlNamespaceManager(table);
            xnm.AddNamespace("schemaLocation", "http://schemas.opengeospatial.net/gml/2.1.2/feature.xsd");
            xnm.AddNamespace("version", "1.0.0");
            xnm.AddNamespace("elementFormDefault", "qualified");
            xnm.AddNamespace("targetNamespace", "http://www.geostar.com.cn/geoglobe");
            xnm.AddNamespace("gml", "http://www.opengis.net/gml");
            xnm.AddNamespace("geoglobe", "http://www.geostar.com.cn/geoglobe");
            xnm.AddNamespace("d", "http://www.w3.org/2001/XMLSchema");
            return xnm;
        }
        protected void url_TextChanged(object sender, EventArgs e)
        {




        }



        Dictionary<string, ArrayList> getDic()
        {
            Dictionary<string, ArrayList> dic = new Dictionary<string, ArrayList>();
            string url = this.url.Text + "?VERSION=1.0.0&REQUEST=DescribeFeatureType";
            var txt = get(url);
            XmlDocument xml = new XmlDocument();
            if (this.ddlType.SelectedValue == "geoserver")
            {
                xml.LoadXml(txt);
                var xnm = getxnm(xml.NameTable);
                var nodes = xml.SelectNodes("//d:complexType", xnm);
                foreach (XmlNode node in nodes)
                {

                    string layername = node.Attributes["name"].Value.Replace("_Type", "");

                    var fnodes = xml.SelectNodes("//d:complexType[@name='" + layername + "_Type']//d:element", xnm);
                    //var fnodes = xml.SelectNodes("/d:element [@name='" + layername + "_Type']//d:element", xnm);
                    ArrayList ar = new ArrayList();
                    foreach (XmlNode fnode in fnodes)
                    {
                        ar.Add(fnode.Attributes["name"].Value);
                    }
                    dic[layername] = ar;

                }
                return dic;
            }
            else
            {
                //xml.LoadXml(DelXmlNSP(txt));
                xml.LoadXml(txt);
                var xnm = getxnm(xml.NameTable);
                var nodes = xml.SelectNodes("//d:complexType", xnm);
                foreach (XmlNode node in nodes)
                {

                    string layername = node.Attributes["name"].Value.Replace("Type", "");

                    var fnodes = xml.SelectNodes("//d:complexType[@name='" + layername + "Type']//d:element", xnm);
                    //var fnodes = xml.SelectNodes("/d:element [@name='" + layername + "_Type']//d:element", xnm);
                    ArrayList ar = new ArrayList();
                    foreach (XmlNode fnode in fnodes)
                    {
                        ar.Add(fnode.Attributes["name"].Value);
                    }
                    dic[layername] = ar;

                }
                return dic;

            }
        }
        private string DelXmlNSP(string r)
        {
            r = Regex.Replace(r, "</\\w+?:", "</");
            r = Regex.Replace(r, "<\\w+?:", "<");
            r = Regex.Replace(r, "<FeatureCollection.+?>", "<FeatureCollection>");
            r = Regex.Replace(r, "\\w+?:", "");
            return r;
        }
        protected void btnTextwfs_Click(object sender, EventArgs e)
        {
            var dic = this.getDic();
            //foreach (string key in dic.Keys)
            //{
            //    if (this.layers.Text == "")
            //        this.layers.Text += key;
            //    else
            //        this.layers.Text += "\n" + key;
            //}
            this.ListBox1.DataSource = dic.Keys;
            
            ListBox1.DataBind();
            if (ListBox1.Items.Count > 0)
                ListBox1.Items[0].Selected=true;
        }

        Dictionary<string, Dictionary<string, string>> getWmtsDic()
        {
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            string url = this.url2.Text + "?SERVICE=WMTS&VERSION=1.0.0&REQUEST=GetCapabilities";
            var txt = get(url);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(txt);
            XmlNamespaceManager xnm = new XmlNamespaceManager(xml.NameTable);
            xnm.AddNamespace("schemaLocation", "http://www.opengis.net/wmts/1.0 http://schemas.opengis.net/wmts/1.0.0/wmtsGetCapabilities_response.xsd");
            xnm.AddNamespace("version", "1.0.0");
            xnm.AddNamespace("xlink", "http://www.w3.org/1999/xlink");
            xnm.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xnm.AddNamespace("gml", "http://www.opengis.net/gml");
            xnm.AddNamespace("ows", "http://www.opengis.net/ows/1.1");
            xnm.AddNamespace("d", "http://www.opengis.net/wmts/1.0");

            var nodes = xml.SelectNodes("//d:Layer", xnm);
            foreach (XmlNode node in nodes)
            {
                Dictionary<string, string> sdic = new Dictionary<string, string>();
                sdic["Identifier"] = node.SelectSingleNode("descendant::ows:Identifier", xnm).InnerText;
                sdic["Format"] = node.SelectSingleNode("descendant::d:Format", xnm).InnerText;
                sdic["TileMatrixSet"] = node.SelectSingleNode("descendant::d:TileMatrixSet", xnm).InnerText;
                var tnode = node.SelectSingleNode("//ows:Identifier[text()='" + sdic["TileMatrixSet"] + "']", xnm);//../ d:TileMatrix / ows:Identifier
                var lnodes = tnode.ParentNode.SelectNodes("descendant::d:TileMatrix/ows:Identifier", xnm);
                string levels = "";
                foreach (XmlNode lnode in lnodes)
                {
                    if (levels == "")
                        levels += lnode.InnerText;
                    else
                        levels += "," + lnode.InnerText;
                }
                sdic["levels"] = levels;

                dic[sdic["Identifier"]] = sdic;
            }



            return dic;
        }
        protected void btnTextwfs0_Click(object sender, EventArgs e)
        {
            this.txtfields.Text = "";
            var dic = this.getDic();
            Dictionary<string, object> alldic = new Dictionary<string, object>();
            string json = File.ReadAllText(Server.MapPath("~/data/PropertyDic.json"));
            alldic = JsonHelper.JsonToDictionary(json);

            //ArrayList ar = dic[this.layers.Text];
            
            ArrayList ar = dic[this.ListBox1.SelectedValue];
            foreach (string i in ar)
            {
                string cn = i;
                if (alldic.ContainsKey(i))
                    cn = alldic[i].ToString();

                if (this.txtfields.Text == "")
                    this.txtfields.Text += cn + "," + i;
                else
                    this.txtfields.Text += "\n" + cn + "," + i;
            }
        }

        protected void tstUrl_Click(object sender, EventArgs e)
        {
            //this.layers0.Text = "";
            var dic = this.getWmtsDic();
            //foreach (string i in dic.Keys)
            //{
            //    if (this.layers0.Text == "")
            //        this.layers0.Text += i;
            //    else
            //        this.layers0.Text += "\n" + i;

            //}
            this.ListBox2.DataSource = dic.Keys;
            
            ListBox2.DataBind();
            if (ListBox2.Items.Count > 0)
                ListBox2.Items[0].Selected = true;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            txtfields0.Text = "";
            var dic = this.getWmtsDic();
            var sdic = dic[this.ListBox2.SelectedItem.Text];
            foreach (string k in sdic.Keys)
            {
                this.txtfields0.Text += k + ":" + sdic[k] + "\n";
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            XMLHandler h = new XMLHandler();

            h.SetAllConfig();
        }

        protected void btnRegion_Click(object sender, EventArgs e)
        {
            new WFSHandler().ImportRegionLatlngYC();//云浮导法
            //new WFSHandler().ImortShpRegion();//电白导法
        }
        protected void btnRegion0_Click(object sender, EventArgs e)
        {
            new WFSHandler().ImportRegionLatlngYA();//云安导法
        }

        void dbimporRegion()
        { }

        /// <summary>
        /// 对图片进行压缩优化，始终保持原宽高比，限制长边长度，常用场景：相片
        /// </summary>
        /// <param name="destPath">目标保存路径</param>
        /// <param name="srcPath">源文件路径</param>
        /// <param name="max_Length">压缩后的图片边（宽或者高）长变不大于这值，为0表示不限制</param>  
        /// <param name="quality">1~100整数,无效值，则取默认值95</param>
        /// <param name="mimeType">如 image/jpeg</param>
        public bool GetCompressImage(string destPath, string srcPath, int maxLength, int quality, out string error, string mimeType = "image/jpeg")
        {
            bool retVal = false;
            error = string.Empty;
            //最大边长不能小于0
            if (maxLength < 0)
            {
                error = "最大边长不能小于0";
                return retVal;
            }
            System.Drawing.Image srcImage = null;
            System.Drawing.Image destImage = null;
            Graphics graphics = null;
            try
            {
                //获取源图像
                srcImage = System.Drawing.Image.FromFile(srcPath, false);
                FileInfo fileInfo = new FileInfo(srcPath);
                //目标宽度
                var destWidth = srcImage.Width;
                //目标高度
                var destHeight = srcImage.Height;
                //如果限制
                if (maxLength > 0)
                {
                    //原高宽比
                    float srcD = (float)srcImage.Height / srcImage.Width;
                    //如果宽>高，且大于 限制
                    if (destWidth > destHeight && destWidth > maxLength)
                    {
                        destWidth = maxLength;
                        destHeight = Convert.ToInt32(destWidth * srcD);
                    }
                    else
                    {
                        if (destHeight > maxLength)
                        {
                            destHeight = maxLength;
                            destWidth = Convert.ToInt32(destHeight / srcD);
                        }
                    }
                }
                //如果维持原宽高，则判断是否需要优化
                if (destWidth == srcImage.Width && destHeight == srcImage.Height && fileInfo.Length < destWidth * destHeight * sizePerPx)
                {
                    error = "图片不需要压缩优化";
                    return retVal;
                }
                //定义画布
                destImage = new Bitmap(destWidth, destHeight);
                //获取高清Graphics
                graphics = GetGraphics(destImage);
                //将源图像画到画布上，注意最后一个参数GraphicsUnit.Pixel
                graphics.DrawImage(srcImage, new Rectangle(0, 0, destWidth, destHeight), new Rectangle(0, 0, srcImage.Width, srcImage.Height), GraphicsUnit.Pixel);
                //如果是覆盖则先释放源资源
                if (destPath == srcPath)
                {
                    srcImage.Dispose();
                }
                //保存到文件，同时进一步控制质量
                SaveImage2File(destPath, destImage, quality, mimeType);
                retVal = true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                if (srcImage != null)
                    srcImage.Dispose();
                if (destImage != null)
                    destImage.Dispose();
                if (graphics != null)
                    graphics.Dispose();
            }
            return retVal;
        }

        //优化良好的图片每个像素平均占用文件大小，经验值，可根据需要修改
        private static readonly double sizePerPx = 0.18;
        /// <summary>
        /// 获取高清的Graphics
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public Graphics GetGraphics(System.Drawing.Image img)
        {
            var g = Graphics.FromImage(img);
            //设置质量
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            //InterpolationMode不能使用High或者HighQualityBicubic,如果是灰色或者部分浅色的图像是会在边缘处出一白色透明的线
            //用HighQualityBilinear却会使图片比其他两种模式模糊（需要肉眼仔细对比才可以看出）
            g.InterpolationMode = InterpolationMode.Default;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            return g;
        }
        /// <summary>
        /// 将Image实例保存到文件,注意此方法不执行 img.Dispose()
        /// 图片保存时本可以直接使用destImage.Save(path, ImageFormat.Jpeg)，但是这种方法无法进行进一步控制图片质量
        /// </summary>
        /// <param name="path"></param>
        /// <param name="img"></param>
        /// <param name="quality">1~100整数,无效值，则取默认值95</param>
        /// <param name="mimeType"></param>
        public void SaveImage2File(string path, System.Drawing.Image destImage, int quality, string mimeType = "image/jpeg")
        {
            if (quality <= 0 || quality > 100) quality = 95;
            //创建保存的文件夹
            FileInfo fileInfo = new FileInfo(path);
            if (!Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }
            //设置保存参数，保存参数里进一步控制质量
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qua = new long[] { quality };
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            //获取指定mimeType的mimeType的ImageCodecInfo
            var codecInfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(ici => ici.MimeType == mimeType);
            destImage.Save(path, codecInfo, encoderParams);
        }


        public List<string> flist = new List<string>();
        public void getfiles(string path)
        {
            var ds =  Directory.GetDirectories(path);
            foreach (var dic in ds)
            {
                this.getfiles(dic);
            }
            var fs = Directory.GetFiles(path);
            foreach (var f in fs)
            {
                flist.Add(f);
            }

        }
        protected void zipimage_Click(object sender, EventArgs e)
        {
            string orDir = this.ordimage.Text;
            string destDir = this.destimage.Text;
            string error = "";
            //GetCompressImage(dest, or, 500, 80,out error);
            this.getfiles(orDir);
            foreach (var f in flist)
            {
                string zhenName = Path.GetDirectoryName(f).TrimEnd('\\');
                zhenName = zhenName.Split('\\')[zhenName.Split('\\').Length - 1];
                string destDir2 = destDir +"\\"+zhenName+ Path.GetFileNameWithoutExtension(f).Replace("影像地图", "");
                if(zhenName== Path.GetFileNameWithoutExtension(f).Replace("影像地图", ""))
                    destDir2 = destDir + "\\" + Path.GetFileNameWithoutExtension(f).Replace("影像地图", "");
                if (!Directory.Exists(destDir2))
                    Directory.CreateDirectory(destDir2);
                string destF = destDir2 + "\\small.jpg";
                GetCompressImage(destF, f, 500, 80, out error);
                File.Copy(f, destDir2 + "\\origin.jpg");
                string xmlstr = File.ReadAllText(this.MapPath("~/wallmap/WallmapPointConf.xml"));
                xmlstr = xmlstr.Replace("@name", Path.GetFileNameWithoutExtension(f).Replace("影像地图", ""));
                File.WriteAllText(destDir2 + @"\WallmapPointConf.xml",xmlstr);
                
            }

        }

        protected void btnRegion1_Click(object sender, EventArgs e)
        {
            new WFSHandler().ImortShpRegionLW();
            this.Response.Write("<script>alert('ok');</script>");
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            //string where = namefield + " like '%" + keyword + "%'";
           
            var r = get(this.txtQueryurl.Text + "/query?where=1<>1&text=&objectIds=&time=&geometry=&geometryType=esriGeometryEnvelope&inSR=&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=*&returnGeometry=true&maxAllowableOffset=&geometryPrecision=&outSR=&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&gdbVersion=&returnDistinctValues=false&f=pjson");
            r = r.Replace("\n", "").Replace(" ", "");
            var rdic = JsonHelper.JsonToDictionary(r);
            



            var fieldAliases = rdic["fieldAliases"] as Dictionary<string, object>;
            this.txtQueryResult.Text = "";
            txtTranslate.Text = "";

            string json = File.ReadAllText(Server.MapPath("~/data/PropertyDic.json"));
            var alldic = JsonHelper.JsonToDictionary(json);

            foreach (var f in fieldAliases)
            {
                txtQueryResult.Text+=f.Value.ToString()+","+f.Key + "\n";
                string cn = f.Key;
                if (alldic.ContainsKey(f.Key))
                    cn = alldic[f.Key].ToString();
                txtTranslate.Text += cn + "," + f.Key + "\n";
            }

        }
    }
}