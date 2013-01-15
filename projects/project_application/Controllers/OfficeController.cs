using System.Web;
using System.Web.Mvc;
using System;
using System.Collections;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using project_application.Models;
//引入操作 xml 的包
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Office.Interop.Word;

namespace project_application.Controllers
{
    public class OfficeController : Controller
    {
        private ProjectsContext db = new ProjectsContext();
        // excel  导入  并写入数据库
        public void importExcel(string fileName)
        {
            string phyPath = HttpContext.Request.MapPath("/");
            fileName = phyPath + "test.xls";
            FileStream file = null;
            try
            {

                file = new FileStream(fileName, FileMode.Open);
                IWorkbook workbook = new HSSFWorkbook(file);
                ISheet sheet = workbook.GetSheetAt(0);//取第一个表   
                {
                    IRow headerRow = sheet.GetRow(0);//第一行为标题行   
                    int cellCount = headerRow.LastCellNum;//LastCellNum = PhysicalNumberOfCells   
                    int rowCount = sheet.LastRowNum;//LastRowNum = PhysicalNumberOfRows - 1   
                    for (int i = (sheet.FirstRowNum + 2); i <= rowCount; i++)
                    {
                        IRow row = sheet.GetRow(i);

                        if (row != null)
                        {
                            //for (int j = row.FirstCellNum; j < cellCount; j++)
                            //{
                            //    if (row.GetCell(j) != null)
                            //        Console.WriteLine(row.GetCell(j));
                            BasicProject bp = new BasicProject();
                            bp.Name = row.GetCell(2).ToString();
                            //项目类型
                            String typeName = row.GetCell(3).ToString();
                            //项目资金来源
                            String moneyresource = row.GetCell(4).ToString();
                            if (typeName.Equals("")) break;
                            //var  moneys=db.
                            var type = db.ProjectTypes.First(d => d.Name == typeName);
                            bp.Type = type;
                            //}
                        }

                    }
                }
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }
        //  excel  导出

        public string exportExcel(string fileName)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            string phyPath = HttpContext.Request.MapPath("/");
            FileStream file = new FileStream(phyPath + "/" + fileName, FileMode.Create);
            try
            {
                ISheet sheet = workbook.CreateSheet("A");
                workbook.CreateSheet("B");
                workbook.CreateSheet("D");
                IRow dataRow = sheet.CreateRow(0);
                dataRow.CreateCell(0).SetCellValue("haha");
                dataRow = sheet.CreateRow(1);
                dataRow.CreateCell(0).SetCellValue("hoho");

                //   if (File.Exists(fileName)) File.Delete(fileName);   

                workbook.Write(ms);
                ms.WriteTo(file);
                ms.Flush();

            }
            finally
            {
                ms.Close();
                file.Close();
            }
            return "sucess";
        }
        //准备生成word
        private void AddContent(Microsoft.Office.Interop.Word.Document Worddoc, string TextContent, float FontSize, string FontName, Microsoft.Office.Interop.Word.WdParagraphAlignment AlignStysle, int bold)
        {
            Object Nothing = System.Reflection.Missing.Value;


            Microsoft.Office.Interop.Word.Paragraph oPara1;

            oPara1 = Worddoc.Content.Paragraphs.Add(ref  Nothing);
            oPara1.Range.Text = TextContent;
            oPara1.Range.Font.Bold = bold;
            oPara1.Range.Font.Size = FontSize;
            oPara1.Range.Font.Name = FontName;
            oPara1.Format.SpaceAfter = 0;    //24 pt spacing after paragraph.
            oPara1.Format.LineSpacing = 25;
            oPara1.Format.LineSpacingRule = Microsoft.Office.Interop.Word.WdLineSpacing.wdLineSpaceExactly;
            oPara1.Format.Alignment = AlignStysle;

            oPara1.Range.InsertParagraphAfter();

        }
        //
        private void insertline(int j, Microsoft.Office.Interop.Word.Document Worddoc)
        {
            Object Nothing = System.Reflection.Missing.Value;
            for (int i = 1; i <= j; i++)
            {
                Worddoc.Content.Paragraphs.Add(ref  Nothing);
            }
        }

        public String BuildWord(ApplyProject applyproject, String phyPath)
        {

            object filename = phyPath + "test.doc";
            //创建Word文档
            Object Nothing = System.Reflection.Missing.Value;
            object oPageBreak = Microsoft.Office.Interop.Word.WdBreakType.wdPageBreak;

            Microsoft.Office.Interop.Word.Application WordApp = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document WordDoc = WordApp.Documents.Add(ref Nothing, ref Nothing, ref Nothing, ref Nothing);
            //添加页眉
            WordApp.ActiveWindow.View.Type = WdViewType.wdOutlineView;
            WordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekPrimaryHeader;
            WordApp.ActiveWindow.ActivePane.Selection.InsertAfter("[页眉内容]");
            WordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;//设置右对齐
            WordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekMainDocument;//跳出页眉设置
            WordApp.Selection.ParagraphFormat.LineSpacing = 15f;//设置文档的行间距
            AddContent(WordDoc, "附件二：", 15, "黑体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 0);
            AddContent(WordDoc, "", 17, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter, 0);

            AddContent(WordDoc, "", 17, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter, 0);
            AddContent(WordDoc, "", 17, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter, 0);
            AddContent(WordDoc, "贵州民族大学基本建设投资项目", 18, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter, 1);
            AddContent(WordDoc, "", 17, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter, 0);
            insertline(1, WordDoc);
            AddContent(WordDoc, "申请书", 18, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter, 1);
            AddContent(WordDoc, "", 14, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter, 0);
            insertline(6, WordDoc);
            AddContent(WordDoc, "项  目  名   称：_____________________", 16, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter, 1);
            AddContent(WordDoc, "申  请  学   校：贵州民族学院 （公章）", 16, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter, 1);
            AddContent(WordDoc, "项 目  负 责 人：             （签名）", 16, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter, 1);
            AddContent(WordDoc, "申  报  日   期：       年　  月    日", 16, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter, 1);
            insertline(6, WordDoc);
            AddContent(WordDoc, "贵州民族学院发展规划处制", 16, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter, 1);
            WordDoc.Paragraphs.Last.Range.InsertBreak(ref oPageBreak);//插入了一页 
            AddContent(WordDoc, "一、项目概况", 16, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, "1、项目名称：" + applyproject.ProjectNameTitle, 12, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, "2、项目类型：" + applyproject.ProjectType, 12, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, "3、项目负责人基本情况:" + applyproject.ProjectLeaderDetail, 12, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, "4、项目简介：" + applyproject.ProjectAbstract, 12, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, "二、项目建设的意义和可行性", 16, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, applyproject.ProjectMeaning, 12, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, "三、项目建设目标", 16, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, "1、建设目标" + applyproject.BulidTarget, 12, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, "2、建设内容" + applyproject.BulidContent, 12, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, "四、项目建设任务", 16, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, applyproject.BulidTask, 12, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, "五、项目预期成效", 16, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, applyproject.ExpectEffect, 12, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, "六、建设项目实施组织及进度安排", 16, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, applyproject.ProjectSchedule, 12, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, "七、保障措施", 16, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, applyproject.ProjectEnsure, 12, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, "八、项目支出预算及安排（附设备采购清单)", 16, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);
            AddContent(WordDoc, applyproject.ProjectPay, 12, "宋体", Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft, 1);


            // myRange.Font.Size = 24;
            object FileFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocument97;
            //文件保存
            WordDoc.SaveAs(ref filename, ref FileFormat, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing);
            WordDoc.Close(ref Nothing, ref Nothing, ref Nothing);
            WordApp.Quit(ref Nothing, ref Nothing, ref Nothing);

            return "sucess";
        }



        public ActionResult Index()
        {
            return View();
        }

    }
}
