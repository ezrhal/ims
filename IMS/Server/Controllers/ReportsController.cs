using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IMS.Client.Pages.Maintenance;
using IMS.Client.Pages.PO;
using IMS.Server.Services;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Mvc;
// using Telerik.Reporting;
// using Telerik.Reporting.Services;
// using Telerik.Reporting.Services.AspNetCore;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.Pdf;
using Syncfusion.DocIORenderer;
using HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment;
using Tab = Spire.Doc.Tab;
using VerticalAlignment = Spire.Doc.Documents.VerticalAlignment;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IMS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        IMongoDBService _db;
        public ReportsController(IWebHostEnvironment webHostEnvironment, IMongoDBService db)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF5cWWdCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXxdcXRSR2BeVE12VkI=");
            _webHostEnvironment = webHostEnvironment;
            _db = db;
        }


        // [HttpGet("generatereport")]
        // public async Task<string> GenerateReport()
        // {
        //
        //     string webRootPath = _webHostEnvironment.WebRootPath;
        //     string contentRootPath = _webHostEnvironment.ContentRootPath;
        //
        //     //string path = "";
        //     //path = Path.Combine(contentRootPath, "Reports");
        //
        //     //doc.LoadFromFile(path + "/Templates/" + "workitem.docx");
        //
        //     //Table materials = doc.Sections[0].Tables[0] as Table;
        //
        //
        //     //doc.SaveToFile(path + "/Output/" + "workitem1.docx" );
        //
        //     try
        //     {
        //         var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
        //
        //         // set any deviceInfo settings if necessary
        //         var deviceInfo = new System.Collections.Hashtable();
        //
        //         var reportSource = new Telerik.Reporting.UriReportSource();
        //
        //         reportSource.Uri = contentRootPath + "/Reports/workitem.trdp";
        //
        //         Telerik.Reporting.Processing.RenderingResult result = reportProcessor.RenderReport("PDF", reportSource, deviceInfo);
        //
        //         var b64 = Convert.ToBase64String(result.DocumentBytes);
        //         return b64;
        //     }
        //     catch(Exception e)
        //     {
        //         return e.ToString();
        //     }
        //    
        //
        //
        // }

        // [HttpGet("generatereport1")]
        // public IActionResult GenerateReportPDF()
        // {
        //     //var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
        //
        //     //// set any deviceInfo settings if necessary
        //     //var deviceInfo = new System.Collections.Hashtable();
        //
        //     //var reportSource = new Telerik.Reporting.UriReportSource();
        //
        //     //reportSource.Uri = @"https://servicereport1.azurewebsites.net/api/reprots/workitem.trdp";
        //
        //     //Telerik.Reporting.Processing.RenderingResult result = reportProcessor.RenderReport("PDF", reportSource, deviceInfo);
        //
        //     try
        //     {
        //
        //         var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
        //         var deviceInfo = new System.Collections.Hashtable();
        //
        //         string webRootPath = _webHostEnvironment.WebRootPath;
        //         var reportPackager = new ReportPackager();
        //         Report report;
        //         InstanceReportSource instanceReportSource = new InstanceReportSource();
        //
        //         string contentRootPath = System.IO.Directory.GetCurrentDirectory();
        //
        //
        //
        //         using (var sourceStream = System.IO.File.OpenRead(contentRootPath + "/Reports/workitem.trdp"))
        //         {
        //             report = (Report)reportPackager.UnpackageDocument(sourceStream);
        //         };
        //
        //         instanceReportSource.ReportDocument = report;
        //
        //         Telerik.Reporting.Processing.RenderingResult result = reportProcessor.RenderReport("PDF", instanceReportSource, deviceInfo);
        //
        //         var b64 = Convert.ToBase64String(result.DocumentBytes);
        //         return new ContentResult() { Content = b64 };
        //     }
        //     catch(Exception e)
        //     {
        //         return new ContentResult() { Content = e.ToString() };
        //     }
        //     
        // }
        
        [HttpGet("printworkitem")]
        public async Task<IActionResult> PrintWorkitem(string projectid, string workitemid)
        {
            Document doc = new Document();
            
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;

            string path = "";
            path = Path.Combine(contentRootPath, "Reports");

            doc.LoadFromFile(path + "/Templates/" + "workitem.docx");

            Section section = doc.Sections[0];
            

            ProjectModel project = await _db.GetProject(projectid);

            if (project != null)
            {
                WorkItemModel workitem = project.workitems.Find(q => q.Id.Equals(workitemid));
                List<MaterialsModel> materials = workitem.materials;

                if (materials != null)
                {
                    Table table = doc.Sections[0].Tables[1] as Spire.Doc.Table;
                    List<string[]> materialsList = new();
                    int x = 1;
                    foreach (MaterialsModel material in materials)
                    {
                        materialsList.Add([x.ToString(), material.item + " " + material.description, material.unit, string.Format("{0:n2}", material.unitcost), string.Format("{0:n2}", material.quantity), string.Format("{0:n2}", material.unitcost * material.quantity)]);
                        x++;
                    }
                    
                    for (int i = 0; i < materialsList.Count; i++)
                    {
                        TableRow newRow = table.AddRow(false, true);
                        newRow.Height = 12.5f;
                    
                        for (int c = 0; c < materialsList[i].Length; c++)
                        {
                            TableCell cell = newRow.Cells[c];
                            cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            Paragraph cp = newRow.Cells[c].AddParagraph();
                            cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;

                            if (c == 1)
                            {
                                cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                            }
                            else if (c == 5)
                            {
                                cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Right;
                            }

                            TextRange txtRange = cp.AppendText(materialsList[i][c]);
                            txtRange.CharacterFormat.Bold = false;
                            txtRange.CharacterFormat.FontSize = 9;
                        }
                    }
                
                    TableRow totalRow = table.AddRow(false, true);
                    totalRow.Height = 12.5f;
                    
                    Paragraph pTotal = totalRow.Cells[4].AddParagraph();
                    
                    TableCell pAmountCell = totalRow.Cells[5];
                    pAmountCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    Paragraph pAmount = totalRow.Cells[5].AddParagraph();

                    pTotal.AppendText("TOTAL").CharacterFormat.FontSize = 9;
                    pAmount.AppendText(string.Format("{0:n2}", materials.Sum(q => q.unitcost * q.quantity))).CharacterFormat.FontSize = 9;
                    pTotal.Format.HorizontalAlignment = HorizontalAlignment.Right;
                    pAmount.Format.HorizontalAlignment = HorizontalAlignment.Right;
      
                
                    //table.Rows.Add(totalRow);
                    table.ApplyHorizontalMerge(table.Rows.Count -1,0, 4);
                    
                    TableCell pTotalRowCell = totalRow.Cells[0];
                    pTotalRowCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                }
                
                List<EquipmentModel> equipment = workitem.equipment;

                if (equipment != null)
                {
                    Table table1 = doc.Sections[0].Tables[2] as Spire.Doc.Table;
                    List<string[]> equipmentList = new();
                    int x = 1;
                    
                    foreach (EquipmentModel equip in equipment)
                    {
                        equipmentList.Add([x.ToString(), equip.item + " " + equip.description, equip.unit, string.Format("{0:n2}", equip.unitcost), equip.hours.ToString(), string.Format("{0:n2}", equip.quantity), string.Format("{0:n2}", equip.unitcost * equip.quantity * equip.hours)]);
                        x++;
                    }
                    
                    for (int i = 0; i < equipmentList.Count; i++)
                    {
                        TableRow newRow = table1.AddRow(false, true);
                        newRow.Height = 12.5f;
                        
                        for (int c = 0; c < equipmentList[i].Length; c++)
                        {
                            TableCell cell = newRow.Cells[c];
                            cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            Paragraph cp = newRow.Cells[c].AddParagraph();
                            cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;

                            if (c == 1)
                            {
                                cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                            }
                            else if (c == 6)
                            {
                                cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Right;
                            }

                            TextRange txtRange = cp.AppendText(equipmentList[i][c]);
                            txtRange.CharacterFormat.Bold = false;
                            txtRange.CharacterFormat.FontSize = 9;
                        }
                    }
                
                    TableRow totalRow = table1.AddRow(false, true);
                    totalRow.Height = 12.5f;
                    
                    Paragraph pTotal = totalRow.Cells[5].AddParagraph();
                    TableCell pAmountCell = totalRow.Cells[6];
                    pAmountCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    Paragraph pAmount = totalRow.Cells[6].AddParagraph();

                    pTotal.AppendText("TOTAL").CharacterFormat.FontSize = 9;
                    pAmount.AppendText(string.Format("{0:n2}", equipment.Sum(q => q.unitcost * q.quantity * q.hours))).CharacterFormat.FontSize = 9;
                    pTotal.Format.HorizontalAlignment = HorizontalAlignment.Right;
                    pAmount.Format.HorizontalAlignment = HorizontalAlignment.Right;
      
                
                    //table.Rows.Add(totalRow);
                    table1.ApplyHorizontalMerge(table1.Rows.Count -1,0, 5);
                    TableCell pTotalCell = totalRow.Cells[0];
                    pTotalCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                }
                
                List<LaborModel> labors = workitem.labor;

                if (equipment != null)
                {
                    Table table12= doc.Sections[0].Tables[3] as Spire.Doc.Table;
                    List<string[]> laborList = new();
                    int x = 1;
                    
                    foreach (LaborModel labor in labors)
                    {
                        laborList.Add([x.ToString(), labor.item + " " + labor.description, labor.unit, string.Format("{0:n2}", labor.unitcost), labor.days.ToString(), string.Format("{0:n2}", labor.quantity), string.Format("{0:n2}", labor.unitcost * labor.quantity * labor.days)]);
                        x++;
                    }
                    
                    for (int i = 0; i < laborList.Count; i++)
                    {
                        TableRow newRow = table12.AddRow(false, true);
                        newRow.Height = 12.5f;
                        
                        for (int c = 0; c < laborList[i].Length; c++)
                        {
                            TableCell cell = newRow.Cells[c];
                            cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            Paragraph cp = cell.AddParagraph();
                            cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;

                            if (c == 1)
                            {
                                cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                            }
                            else if (c == 6)
                            {
                                cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Right;
                            }

                            TextRange txtRange = cp.AppendText(laborList[i][c]);
                            txtRange.CharacterFormat.Bold = false;
                            txtRange.CharacterFormat.FontSize = 9;
                        }
                    }
                
                    TableRow totalRow = table12.AddRow(false, true);
                    totalRow.Height = 12.5f;
                    Paragraph pTotal = totalRow.Cells[5].AddParagraph();

                    TableCell pAmountCell = totalRow.Cells[6];
                    pAmountCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    Paragraph pAmount = totalRow.Cells[6].AddParagraph();

                    pTotal.AppendText("TOTAL").CharacterFormat.FontSize = 9;
                    pAmount.AppendText(string.Format("{0:n2}", labors.Sum(q => q.unitcost * q.days * q.quantity))).CharacterFormat.FontSize = 9;
                    pTotal.Format.HorizontalAlignment = HorizontalAlignment.Right;
                    pAmount.Format.HorizontalAlignment = HorizontalAlignment.Right;
      
                
                    //table.Rows.Add(totalRow);
                    table12.ApplyHorizontalMerge(table12.Rows.Count -1,0, 5);
                    totalRow.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                }
                
                doc.Replace("<<PROJECT>>", project.projectname, false, true);
                doc.Replace("<<WORKITEM>>", workitem.itemno + " " + workitem.description, false, true);
                doc.Replace("<<AMOUNT>>", string.Format("{0:n2}", workitem.materials.Sum(q => q.quantity * q.unitcost) +
                                                                  workitem.equipment.Sum(q => q.unitcost * q.quantity * q.hours) + 
                                                                  workitem.labor.Sum(q => q.unitcost * q.quantity * q.days)), false, true);
                
                using (MemoryStream stream = new MemoryStream())
                {
                    doc.SaveToStream(stream, FileFormat.Docx);
                    WordDocument wordDocument = new WordDocument(stream, Syncfusion.DocIO.FormatType.Automatic);
                    DocIORenderer render = new DocIORenderer();

                    PdfDocument pdfDocument = render.ConvertToPDF(wordDocument);

                    render.Dispose();
                    wordDocument.Dispose();

                    MemoryStream outputStream = new MemoryStream();
                    pdfDocument.Save(outputStream);
                    pdfDocument.Close();

                    Response.ContentType = "application/pdf";
                    Response.Headers.Add("content-length", outputStream.GetBuffer().Length.ToString());
                    Response.BodyWriter.Write(outputStream.GetBuffer());
                }
                
                
                
                //doc.SaveToFile(path + "/Output/" + "workitem1.docx");
            }



            return Ok();

        }
        
        [HttpGet("printproject")]
        public async Task<IActionResult> PrintProject(string projectid)
        {
            Document doc = new Document();
            
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;

            string path = "";
            path = Path.Combine(contentRootPath, "Reports");

            doc.LoadFromFile(path + "/Templates/" + "project.docx");

            Section section = doc.Sections[0];
            

            ProjectModel project = await _db.GetProject(projectid);

            if (project != null)
            {
                List<WorkItemModel> workitems = project.workitems;

                if (workitems != null)
                {
                    Table table = doc.Sections[0].Tables[1] as Spire.Doc.Table;
                    List<string[]> workitemList = new();
                    int x = 1;
                    
                    foreach (WorkItemModel workitem in workitems)
                    {
                        workitemList.Add([x.ToString(), workitem.itemno, workitem.description, string.Format("{0:n2}", workitem.totalmaterials), string.Format("{0:n2}", workitem.totalequipment), 
                            string.Format("{0:n2}", workitem.totallabor), string.Format("{0:n2}", workitem.totalmaterials + workitem.totalequipment + workitem.totallabor)]);
                        x++;
                    }
                    
                    for (int i = 0; i < workitemList.Count; i++)
                    {
                        TableRow newRow = table.AddRow(false, true);
                        newRow.Height = 12.5f;
                        
                        for (int c = 0; c < workitemList[i].Length; c++)
                        {
                            TableCell tcell = newRow.Cells[c];
                            tcell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            Paragraph cp =tcell.AddParagraph();
                            cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;

                            if (c == 2)
                            {
                                cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                            }
                            else if (c > 2)
                            {
                                cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Right;
                            }

                            TextRange txtRange = cp.AppendText(workitemList[i][c]);
                            txtRange.CharacterFormat.Bold = false;
                            txtRange.CharacterFormat.FontSize = 9;
                        }
                    }
                
                    TableRow totalRow = table.AddRow(false, true);
                    totalRow.Height = 12.5f;
                    
                    Paragraph pTotal = totalRow.Cells[2].AddParagraph();

                    TableCell mTotalCell = totalRow.Cells[3];
                    mTotalCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    Paragraph mTotal = mTotalCell.AddParagraph();

                    TableCell eTotalCell = totalRow.Cells[4];
                    eTotalCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    Paragraph eTotal = eTotalCell.AddParagraph();

                    TableCell lTotalCell = totalRow.Cells[5];
                    lTotalCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    Paragraph lTotal = lTotalCell.AddParagraph();

                    TableCell tTotalCell = totalRow.Cells[6];
                    tTotalCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    Paragraph tTotal = tTotalCell.AddParagraph();

                    pTotal.AppendText("TOTAL").CharacterFormat.FontSize = 9;
                    mTotal.AppendText(string.Format("{0:n2}", workitems.Sum(q => q.totalmaterials))).CharacterFormat.FontSize = 9;
                    eTotal.AppendText(string.Format("{0:n2}", workitems.Sum(q => q.totalequipment))).CharacterFormat.FontSize = 9;
                    lTotal.AppendText(string.Format("{0:n2}", workitems.Sum(q => q.totallabor))).CharacterFormat.FontSize = 9;
                    tTotal.AppendText(string.Format("{0:n2}", workitems.Sum(q => q.totalmaterials + q.totalequipment + q.totallabor))).CharacterFormat.FontSize = 9;
                    pTotal.Format.HorizontalAlignment = HorizontalAlignment.Right;
                    mTotal.Format.HorizontalAlignment = HorizontalAlignment.Right;
                    eTotal.Format.HorizontalAlignment = HorizontalAlignment.Right;
                    lTotal.Format.HorizontalAlignment = HorizontalAlignment.Right;
                    tTotal.Format.HorizontalAlignment = HorizontalAlignment.Right;
                    
      
                
                    //table.Rows.Add(totalRow);
                    table.ApplyHorizontalMerge(table.Rows.Count -1,0, 2);

                    TableCell MergeCell = totalRow.Cells[0];
                    MergeCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                }
                
                doc.Replace("<<PROJECT>>", project.projectname, false, true);
                doc.Replace("<<LOCATION>>", project.geodata.address, false, true);
                doc.Replace("<<AMOUNT>>", string.Format("{0:n2}", project.workitems.Sum(q => q.totalmaterials + q.totalequipment + q.totallabor)), false, true);
                
                using (MemoryStream stream = new MemoryStream())
                {
                    doc.SaveToStream(stream, FileFormat.Docx);
                    stream.Position = 0;
                    WordDocument wordDocument = new WordDocument(stream, Syncfusion.DocIO.FormatType.Automatic);
                    using (DocIORenderer renderer = new DocIORenderer())
                    {
                        // Convert the Word document to a PDF
                        using (PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument))
                        {
                            // Create an output stream for the PDF
                            using (MemoryStream outputStream = new MemoryStream())
                            {
                                // Save the PDF to the output stream
                                pdfDocument.Save(outputStream);

                                // Reset the position of the output stream
                                outputStream.Position = 0;

                                // Set the response content type and headers
                                Response.ContentType = "application/pdf";
                                Response.Headers["Content-Length"] = outputStream.Length.ToString();

                                // Write the PDF data to the response body
                                await Response.Body.WriteAsync(outputStream.ToArray());
                            }
                        }
                    }
                    wordDocument.Dispose();
                }
                
                // using (MemoryStream stream = new MemoryStream())
                // {
                //     doc.SaveToStream(stream, FileFormat.Docx);
                //     WordDocument wordDocument = new WordDocument(stream, Syncfusion.DocIO.FormatType.Automatic);
                //     DocIORenderer render = new DocIORenderer();
                //
                //     using (DocIORenderer renderer = new DocIORenderer())
                //     {
                //         //Converts Word document into PDF document.
                //         using (PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument))
                //         {
                //             //Saves the PDF file to file system.    
                //             using (FileStream outputStream = new FileStream(path + "/Output/" + projectid  + ".pdf", FileMode.Create, FileAccess.ReadWrite))
                //             {
                //                 pdfDocument.Save(outputStream);
                //             }
                //         }
                //     }
                //     
                //     
                //     render.Dispose();
                //     wordDocument.Dispose();
                //     
                //     string filePath = Path.Combine(path, "/Output/" + projectid  + ".pdf");
                //
                //     // Read the file bytes
                //     byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                //
                //     // Return the file with the appropriate MIME type
                //     return File(fileBytes, "application/pdf", "project.pdf");
                //     
                //     //Response.ContentType = "application/pdf";
                //     //Response.Headers.Add("content-length", outputStream.GetBuffer().Length.ToString());
                //     //Response.BodyWriter.Write(outputStream.GetBuffer());
                // }
                
                
                
                //doc.SaveToFile(path + "/Output/" + "workitem1.docx");
            }



            return Ok();

        }
        
        [HttpGet("printpr")]
        public async Task<IActionResult> PrintPR(string prid)
        {
            Document doc = new Document();
            
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;

            string path = "";
            path = Path.Combine(contentRootPath, "Reports");

            doc.LoadFromFile(path + "/Templates/" + "Purchase Request.docx");

            Section section = doc.Sections[0];
            

            PRModel pr = await _db.GetPR(prid);

            if (pr != null)
            {
                List<PRItemModel> items = pr.items;

                if (items != null)
                {
                    Table table = doc.Sections[0].Tables[1] as Spire.Doc.Table;
                    List<string[]> itemList = new();
                    int x = 1;
                    
                    foreach (PRItemModel item in items)
                    {
                        itemList.Add([x.ToString(), item.item + " "  + item.description, item.unit, string.Format("{0:n2}", item.unitcost), item.quantity.ToString(), string.Format("{0:n2}", item.unitcost * item.quantity)]);
                        x++;
                    }
                    
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        TableRow newRow = table.AddRow(false, true);
                        newRow.Height = 12.0f;
                    
                        for (int c = 0; c < itemList[i].Length; c++)
                        {
                            TableCell tcell = newRow.Cells[c];
                            tcell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            Paragraph cp = tcell.AddParagraph();
                            cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                            

                            if (c == 1)
                            {
                                cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                            }
                            else if (c == 5)
                            {
                                cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Right;
                            }

                            TextRange txtRange = cp.AppendText(itemList[i][c]);
                            txtRange.CharacterFormat.Bold = false;
                            txtRange.CharacterFormat.FontSize = 9;
                        }
                    }
                
                    
                    
                }
                
                doc.Replace("<<PROJECT>>", pr.projectname, false, true);
                doc.Replace("<<WORKITEM>>", pr.workitem, false, true);
                doc.Replace("<<PRNO>>", pr.transactionno, false, true);
                doc.Replace("<<AMOUNT>>", string.Format("{0:n2}", pr.items.Sum(q => q.unitcost * q.quantity)) , false, true);
               
                using (MemoryStream stream = new MemoryStream())
                {
                    doc.SaveToStream(stream, FileFormat.Docx);
                    stream.Position = 0;
                    WordDocument wordDocument = new WordDocument(stream, Syncfusion.DocIO.FormatType.Automatic);
                    using (DocIORenderer renderer = new DocIORenderer())
                    {
                        // Convert the Word document to a PDF
                        using (PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument))
                        {
                            // Create an output stream for the PDF
                            using (MemoryStream outputStream = new MemoryStream())
                            {
                                // Save the PDF to the output stream
                                pdfDocument.Save(outputStream);

                                // Reset the position of the output stream
                                outputStream.Position = 0;

                                // Set the response content type and headers
                                Response.ContentType = "application/pdf";
                                Response.Headers["Content-Length"] = outputStream.Length.ToString();

                                // Write the PDF data to the response body
                                await Response.Body.WriteAsync(outputStream.ToArray());
                            }
                        }
                    }
                    wordDocument.Dispose();
                }
                
                // using (MemoryStream stream = new MemoryStream())
                // {
                //     doc.SaveToStream(stream, FileFormat.Docx);
                //     stream.Position = 0;
                //     WordDocument wordDocument = new WordDocument(stream, Syncfusion.DocIO.FormatType.Automatic);
                //     using (DocIORenderer renderer = new DocIORenderer())
                //     {
                //         // Convert the Word document to a PDF
                //         using (PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument))
                //         {
                //             // Create an output stream for the PDF
                //             using (MemoryStream outputStream = new MemoryStream())
                //             {
                //                 // Save the PDF to the output stream
                //                 pdfDocument.Save(outputStream);
                //
                //                 // Reset the position of the output stream
                //                 outputStream.Position = 0;
                //
                //                 // Set the response content type and headers
                //                 Response.ContentType = "application/pdf";
                //                 Response.Headers["Content-Length"] = outputStream.Length.ToString();
                //
                //                 // Write the PDF data to the response body
                //                 await Response.Body.WriteAsync(outputStream.ToArray());
                //             }
                //         }
                //     }
                //     wordDocument.Dispose();
                // }
                
                
                
                //doc.SaveToFile(path + "/Output/" + "workitem1.docx");
            }



            return Ok();

        }
        
        [HttpGet("printpr1")]
        public async Task<IActionResult> PrintPR1(string prid)
        {
            Document doc = new Document();
            
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;

            string path = "";
            path = Path.Combine(contentRootPath, "Reports");

            doc.LoadFromFile(path + "/Templates/" + "Purchase Request1.docx");

            Section section = doc.Sections[0];
            

            PRModel pr = await _db.GetPR(prid);

            if (pr != null)
            {
                List<PRItemModel> items = pr.items;

                if (items != null)
                {
                    Table table = doc.Sections[0].Tables[1] as Spire.Doc.Table;
                    List<string[]> itemList = new();
                    int x = 1;
                    
                    foreach (PRItemModel item in items)
                    {
                        itemList.Add([x.ToString(), item.item + " "  + item.description, item.unit, string.Format("{0:n2}", item.unitcost), item.quantity.ToString(), string.Format("{0:n2}", item.unitcost * item.quantity)]);
                        x++;
                    }
                    
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        TableRow newRow = table.AddRow(false, true);
                        newRow.Height = 12.0f;
                    
                        for (int c = 0; c < itemList[i].Length; c++)
                        {
                            TableCell tcell = newRow.Cells[c];
                            tcell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            Paragraph cp = tcell.AddParagraph();
                            cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                            

                            if (c == 1)
                            {
                                cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                            }
                            else if (c == 5)
                            {
                                cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Right;
                            }

                            TextRange txtRange = cp.AppendText(itemList[i][c]);
                            txtRange.CharacterFormat.Bold = false;
                            txtRange.CharacterFormat.FontSize = 9;
                        }
                    }
                
                    
                    
                }
                
                doc.Replace("<<CHARGES>>", pr.charges, false, true);
                doc.Replace("<<PRNO>>", pr.transactionno, false, true);
                doc.Replace("<<AMOUNT>>", string.Format("{0:n2}", pr.items.Sum(q => q.unitcost * q.quantity)) , false, true);
               
                using (MemoryStream stream = new MemoryStream())
                {
                    doc.SaveToStream(stream, FileFormat.Docx);
                    stream.Position = 0;
                    WordDocument wordDocument = new WordDocument(stream, Syncfusion.DocIO.FormatType.Automatic);
                    using (DocIORenderer renderer = new DocIORenderer())
                    {
                        // Convert the Word document to a PDF
                        using (PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument))
                        {
                            // Create an output stream for the PDF
                            using (MemoryStream outputStream = new MemoryStream())
                            {
                                // Save the PDF to the output stream
                                pdfDocument.Save(outputStream);

                                // Reset the position of the output stream
                                outputStream.Position = 0;

                                // Set the response content type and headers
                                Response.ContentType = "application/pdf";
                                Response.Headers["Content-Length"] = outputStream.Length.ToString();

                                // Write the PDF data to the response body
                                await Response.Body.WriteAsync(outputStream.ToArray());
                            }
                        }
                    }
                    wordDocument.Dispose();
                }
                
                // using (MemoryStream stream = new MemoryStream())
                // {
                //     doc.SaveToStream(stream, FileFormat.Docx);
                //     WordDocument wordDocument = new WordDocument(stream, Syncfusion.DocIO.FormatType.Automatic);
                //     DocIORenderer render = new DocIORenderer();
                //
                //     PdfDocument pdfDocument = render.ConvertToPDF(wordDocument);
                //
                //     render.Dispose();
                //     wordDocument.Dispose();
                //
                //     MemoryStream outputStream = new MemoryStream();
                //     pdfDocument.Save(outputStream);
                //     pdfDocument.Close();
                //
                //     Response.ContentType = "application/pdf";
                //     Response.Headers.Add("content-length", outputStream.GetBuffer().Length.ToString());
                //     Response.BodyWriter.Write(outputStream.GetBuffer());
                // }
                
                
                
                //doc.SaveToFile(path + "/Output/" + "workitem1.docx");
            }



            return Ok();

        }
        
        [HttpGet("printpo")]
        public async Task<IActionResult> PrintPO(string prid, string poid)
        {
            Document doc = new Document();
            
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;

            string path = "";
            path = Path.Combine(contentRootPath, "Reports");

            doc.LoadFromFile(path + "/Templates/" + "Purchase Order.docx");

            Section section = doc.Sections[0];

            PRModel pr = await _db.GetPR(prid);
            POModel po = pr.PO.Find(q => q.Id.Equals(poid));

            if (po != null)
            {
                List<POItemModel> items = po.items;

                if (items != null)
                {
                    Table table = doc.Sections[0].Tables[1] as Spire.Doc.Table;
                    List<string[]> itemList = new();
                    int x = 1;
                    
                    foreach (POItemModel item in items)
                    {
                        itemList.Add([x.ToString(), item.item + " "  + item.description, item.unit, string.Format("{0:n2}", item.price), item.quantity.ToString(), string.Format("{0:n2}", item.price * item.quantity)]);
                        x++;
                    }
                    
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        TableRow newRow = table.AddRow(false, true);
                        newRow.Height = 12.0f;
                    
                        for (int c = 0; c < itemList[i].Length; c++)
                        {
                            TableCell tcell = newRow.Cells[c];
                            tcell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            Paragraph cp = tcell.AddParagraph();
                            cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                            

                            if (c == 1)
                            {
                                cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                            }
                            else if (c == 5)
                            {
                                cp.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Right;
                            }

                            TextRange txtRange = cp.AppendText(itemList[i][c]);
                            txtRange.CharacterFormat.Bold = false;
                            txtRange.CharacterFormat.FontSize = 9;
                        }
                    }
                }
                
                doc.Replace("<<PONO>>", po.pono, false, true);
                doc.Replace("<<AMOUNT>>", string.Format("{0:n2}", po.items.Sum(q => q.price * q.quantity)), false, true);
                doc.Replace("<<SUPPLIER>>", po.supplier, false, true);
                doc.Replace("<<PRNO>>", pr.transactionno, false, true);
               
                using (MemoryStream stream = new MemoryStream())
                {
                    doc.SaveToStream(stream, FileFormat.Docx);
                    stream.Position = 0;
                    WordDocument wordDocument = new WordDocument(stream, Syncfusion.DocIO.FormatType.Automatic);
                    using (DocIORenderer renderer = new DocIORenderer())
                    {
                        // Convert the Word document to a PDF
                        using (PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument))
                        {
                            // Create an output stream for the PDF
                            using (MemoryStream outputStream = new MemoryStream())
                            {
                                // Save the PDF to the output stream
                                pdfDocument.Save(outputStream);

                                // Reset the position of the output stream
                                outputStream.Position = 0;

                                // Set the response content type and headers
                                Response.ContentType = "application/pdf";
                                Response.Headers["Content-Length"] = outputStream.Length.ToString();

                                // Write the PDF data to the response body
                                await Response.Body.WriteAsync(outputStream.ToArray());
                            }
                        }
                    }
                    wordDocument.Dispose();
                }
                
                
                
                //doc.SaveToFile(path + "/Output/" + "workitem1.docx");
            }



            return Ok();

        }
        
        [HttpGet("printcv")]
        public async Task<IActionResult> PrintCV(string poid)
        {
            Document doc = new Document();
            
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;

            string path = "";
            path = Path.Combine(contentRootPath, "Reports");

            doc.LoadFromFile(path + "/Templates/" + "Voucher.docx");

            Section section = doc.Sections[0];


            POModel cv = await _db.GetCV(poid);

            if (cv != null)
            {
               
                
                doc.Replace("<<CVDATE>>", string.Format("{0:MMMM dd, yyyy}", cv.cvdate), false, true);
                doc.Replace("<<AMOUNT>>", string.Format("{0:n2}", cv.amount), false, true);
                doc.Replace("<<PAYEE>>", cv.payee, false, true);
                doc.Replace("<<PROJECTNAME>>", cv.projectname, false, true);
                doc.Replace("<<CHECKNO>>", cv.checkno, false, true);
                doc.Replace("<<CHECKDATE>>", cv.checkdate, false, true);
                doc.Replace("<<ITEM>>", "Payment of " + cv.items.First().item + " " + cv.items.First().description, false, true);
               
                using (MemoryStream stream = new MemoryStream())
                {
                    doc.SaveToStream(stream, FileFormat.Docx);
                    WordDocument wordDocument = new WordDocument(stream, Syncfusion.DocIO.FormatType.Automatic);
                    DocIORenderer render = new DocIORenderer();

                    PdfDocument pdfDocument = render.ConvertToPDF(wordDocument);

                    render.Dispose();
                    wordDocument.Dispose();

                    MemoryStream outputStream = new MemoryStream();
                    pdfDocument.Save(outputStream);
                    pdfDocument.Close();

                    Response.ContentType = "application/pdf";
                    Response.Headers.Add("content-length", outputStream.GetBuffer().Length.ToString());
                    Response.BodyWriter.Write(outputStream.GetBuffer());
                }
                
                
                
                //doc.SaveToFile(path + "/Output/" + "workitem1.docx");
            }



            return Ok();

        }

    }
}
