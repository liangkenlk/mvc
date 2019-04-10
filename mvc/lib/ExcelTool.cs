using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data;
using System.IO;

namespace Web.lib
{
    public class ExcelTool
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        public ExcelTool()
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
            dic["UserName"] = "采集单位";
        }
        public ExcelTool(Dictionary<string, string> dic)
        {
            this.dic = dic;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt">数据集</param>
        /// <param name="strExcelFileName">导出文件名</param>
        /// <param name="templatePath">模板文件名</param>
        /// <param name="isAutoHead">是否自动生成表头</param>
        /// <param name="startRow">开始填充的行</param>
        /// <param name="startCol">开始填充的列</param>
        public void GridToExcelByNPOI(DataTable dt, string strExcelFileName, string templatePath = "", int startRow = 1, int startCol = 0)
        {

            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");
            if (templatePath == "")
            {
                ICellStyle HeadercellStyle = workbook.CreateCellStyle();
                HeadercellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                //字体
                NPOI.SS.UserModel.IFont headerfont = workbook.CreateFont();
                headerfont.Boldweight = (short)FontBoldWeight.Bold;
                HeadercellStyle.SetFont(headerfont);


                //用column name 作为列名
                //int icolIndex = 0;
                //IRow headerRow = sheet.CreateRow(0);
                //foreach (DataColumn item in dt.Columns)
                //{
                //    ICell cell = headerRow.CreateCell(icolIndex);
                //    string title = item.ColumnName;
                //    if (dic.ContainsKey(item.ColumnName))
                //        title = dic[item.ColumnName];

                //    cell.SetCellValue(title);
                //    cell.CellStyle = HeadercellStyle;
                //    icolIndex++;
                //}

                int icolIndex = 0;
                IRow headerRow = sheet.CreateRow(0);
                foreach (string title in dic.Values)
                {
                    ICell cell = headerRow.CreateCell(icolIndex);

                    cell.SetCellValue(title);
                    cell.CellStyle = HeadercellStyle;
                    icolIndex++;

                }
            }
            else
            {
                using (FileStream filee = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    workbook = new HSSFWorkbook(filee);
                    sheet = workbook.GetSheetAt(0);
                }
            }

            ICellStyle cellStyle = workbook.CreateCellStyle();

            //为避免日期格式被Excel自动替换，所以设定 format 为 『@』 表示一率当成text來看
            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
            cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;


            NPOI.SS.UserModel.IFont cellfont = workbook.CreateFont();
            cellfont.Boldweight = (short)FontBoldWeight.Normal;
            cellStyle.SetFont(cellfont);


            //foreach (DataRow Rowitem in dt.Rows)
            //{
            //    IRow DataRow = sheet.CreateRow(iRowIndex);
            //    foreach (DataColumn Colitem in dt.Columns)
            //    {

            //        ICell cell = DataRow.CreateCell(iCellIndex);
            //        cell.SetCellValue(Rowitem[Colitem].ToString());
            //        cell.CellStyle = cellStyle;
            //        iCellIndex++;
            //    }
            //    iCellIndex = 0;
            //    iRowIndex++;
            //}

            //建立内容行
            int iRowIndex = startRow;
            int iCellIndex = startCol;
            foreach (DataRow row in dt.Rows)
            {
                IRow DataRow = sheet.CreateRow(iRowIndex);
                foreach (string key in dic.Keys)
                {
                    ICell cell = DataRow.CreateCell(iCellIndex);
                    cell.SetCellValue(row[key].ToString().Replace(" 0:00:00", ""));
                    cell.CellStyle = cellStyle;
                    iCellIndex++;
                }
                iCellIndex = startCol;
                iRowIndex++;
            }

            //自适应列宽度
            for (int i = 0; i < dic.Count(); i++)
            {
                sheet.AutoSizeColumn(i);
            }

            //写Excel
            FileStream file = new FileStream(strExcelFileName, FileMode.OpenOrCreate);
            workbook.Write(file);
            file.Flush();
            file.Close();
        }


    }
}