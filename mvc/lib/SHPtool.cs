using System;
using System.Collections.Generic;

using System.IO;

using System.Data;
using CLeopardZip;
using EGIS.ShapeFileLib;
using System.Drawing;
namespace topo.com
{

    public class SHPtool
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            public SHPtool()
            {

                dic["P_Type"] = "门牌类型";
                dic["P_Region"] = "采集区域";
                dic["P_Class"] = "门牌种类";
                dic["PC_Value"] = "门牌种类值";
                dic["YesOrNoOD"] = "是否有原门牌";
                dic["OP_Num"] = "原门牌号";
                dic["BLGX"] = "是否保留或更新";
                dic["YesOrNo_BZ"] = "是否编制";
                dic["BZ_Values"] = "编制属性值";
                dic["Direction"] = "街道方向";
                dic["R_Name"] = "区域名称";
                dic["P_Adrress"] = "城市门牌地址";
                dic["NC_Address"] = "农村门牌地址";
                dic["MPWZ_Pic"] = "门牌安装位置";
                dic["LegalIden"] = "法定标识";
                dic["Addr_Code"] = "地址代码";
                dic["Lon"] = "经度";
                dic["Lat"] = "纬度";
                dic["MPDWMC"] = "门牌单位名称";
                dic["WZXM"] = "屋主姓名";
                dic["ZhuZhi"] = "住址";
                dic["ID_card"] = "身份证";
                dic["Phone"] = "联系电话";
                dic["Time"] = "采集时间";
                dic["Remark"] = "备注";
            }
            public void export(DataTable table,string strShapeFolder, string strShapeFile)
            {
                  string rootDir =strShapeFolder;
                  string shapefileName = strShapeFile;
            
                FileInfo fFile = new FileInfo(strShapeFolder + @"\" + strShapeFile);
                if (!Directory.Exists(strShapeFolder))
                    Directory.CreateDirectory(strShapeFolder);
                                //是否重命名
                if (fFile.Exists)
                {

               
                        DirectoryInfo fold = new DirectoryInfo(strShapeFolder);
                        FileInfo[] files = fold.GetFiles();
                        foreach (FileInfo f in files)
                        {
                            f.Delete();
                        }
       
                }
            //create a new ShapeFileWriter
            ShapeFileWriter sfw;
            ShapeType shapeType = ShapeType.Point;
            List<DbfFieldDesc> fieldDescs = new List<DbfFieldDesc>();
       
            foreach (DataColumn col in table.Columns)
            {
         
                    DbfFieldDesc fd1 = new DbfFieldDesc();
                    fd1.FieldName = col.Caption;
                    fd1.FieldType = DbfFieldType.Character;
                    fd1.FieldLength = 64;
                    fd1.RecordOffset = 1;

                    fieldDescs.Add(fd1);
            }
      
             
            sfw = ShapeFileWriter.CreateWriter(rootDir, shapefileName, shapeType, fieldDescs.ToArray());
            
                
             foreach (DataRow row in table.Rows)
             {

                 PointD point = new PointD();
                 point.X = double.Parse(row["lon"].ToString());
                 point.Y = double.Parse(row["lat"].ToString());
                 string [] Values = new string[table.Columns.Count];
                 List<string> alfields = new List<string>();
                 foreach (DataColumn col in table.Columns)
                 {
                     
                     if(row[col.Caption]!=null)
                     {
                         alfields.Add(row[col.Caption].ToString());
                     }
                     else
                         alfields.Add("");
                 }
               
                 sfw.AddRecord(new PointD[] { point }, 1, alfields.ToArray());

                }
          

            sfw.Close(); 







        
             
                //打包文件夹
            ZipHelper.ZipDirectory(strShapeFolder.TrimEnd('\\'), strShapeFolder.TrimEnd('\\') + ".zip");
    
            }


            public void ImportShapeFile(string path,DataTable table,string TaskTypeId,int NameIndex)
            {
                ShapeFile.MapFilesInMemory = true;
                EGIS.ShapeFileLib.ShapeFile sf = new EGIS.ShapeFileLib.ShapeFile(path);
                sf.RenderSettings = new EGIS.ShapeFileLib.RenderSettings(path, "", new Font("宋体", 9, FontStyle.Bold));
                EGIS.ShapeFileLib.DbfReader dbfr = sf.RenderSettings.DbfReader;
                                      EGIS.ShapeFileLib.ShapeFileEnumerator sfEnum = sf.GetShapeFileEnumerator();
                        int recordIndex = 0;

                        while (sfEnum.MoveNext())
                        {
                            DataRow row = table.NewRow();
                            //if (recordIndex >= 100) break;
                            //writer.WriteLine(string.Format("Record:{0}", recordIndex));
                            
                            //writer.WriteLine(dbfr.GetField(recordIndex,int.Parse(this.textBox1.Text)));
                            row["BelongType"] = TaskTypeId;
                            row["Name"] = recordIndex.ToString("000000");
                            row["id"] = Guid.NewGuid().ToString();
                            row["Status"] = "未分配";
                            row["GeometryType"] = "面";
                            row["StartTime"] = DateTime.Now;
                                System.Collections.ObjectModel.ReadOnlyCollection<PointD[]> pointRecords = sfEnum.Current;
                                
                                foreach (PointD[] pts in pointRecords)
                                {
                                    string OrilayerLatlng = "";
                                    if (pts.Length < 50)
                                    {
                                        //writer.Write(string.Format("[NumPoints:{0}]", pts.Length));
                                        
                                        for (int n = 0; n < pts.Length; ++n)
                                        {
                                            //if (n > 0) writer.Write(',');
                                            //writer.Write(pts[n].ToString());
                                            //pts[n].X
                                            OrilayerLatlng+=pts[n].X+","+pts[n].Y+";";
                                           
                                        }
                                        OrilayerLatlng = OrilayerLatlng.TrimEnd(';');
                                        //writer.WriteLine();
                                    }
                                    row["OrilayerLatlng"] = OrilayerLatlng;
                                }
                                table.Rows.Add(row);
                                recordIndex++;
                            }
            }

        }
    }