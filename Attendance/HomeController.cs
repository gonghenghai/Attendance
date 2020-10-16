//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using WuxiOA.Models;
//using NPOI.XSSF.UserModel;
//using NPOI.HSSF.UserModel;
//using NPOI.SS.UserModel;
//using Microsoft.AspNetCore.Hosting;
//using WuxiOA.Data;

//namespace WuxiOA.Controllers
//{
//    public class HomeController : Controller
//    {
//        private readonly ILogger<HomeController> _logger;
//        private readonly IHostingEnvironment _hostingEnvironment;
//        private readonly OAContext _context;


//        public HomeController(ILogger<HomeController> logger, IHostingEnvironment hostingEnvironment, OAContext context)
//        {
//            _logger = logger;
//            _hostingEnvironment = hostingEnvironment;
//            _context = context;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }

//        //导入Excel
//        public ActionResult Import()
//        {
//            IFormFile file = Request.Form.Files[0];
//            string folderName = "UploadExcel";
//            string webRootPath = _hostingEnvironment.WebRootPath;
//            string newPath = Path.Combine(webRootPath, folderName);
//            StringBuilder sb = new StringBuilder();
//            if (!Directory.Exists(newPath))
//            {
//                Directory.CreateDirectory(newPath);
//            }
//            if (file.Length > 0)
//            {
//                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
//                ISheet sheet;
//                string fullPath = Path.Combine(newPath, file.FileName);
//                using (var stream = new FileStream(fullPath, FileMode.Create))
//                {
//                    file.CopyTo(stream);
//                    stream.Position = 0;
//                    if (sFileExtension == ".xls")
//                    {
//                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
//                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
//                    }
//                    else
//                    {
//                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
//                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   

//                        clearDB();//清空数据库
//                        importEmployees(hssfwb.GetSheetAt(0));//导入Excel表内的Employee表到数据库
//                        importJobs(hssfwb.GetSheetAt(3));//导入Excel表内的Job表到数据库
//                    }

//                    //展示Employee数据
//                    IRow headerRow = sheet.GetRow(0); //Get Header Row
//                    int cellCount = headerRow.LastCellNum;


//                    sb.Append("<table class='table table-bordered'><tr>");
//                    for (int j = 0; j < cellCount; j++)
//                    {
//                        NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
//                        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
//                        sb.Append("<th>" + cell.ToString() + "</th>");
//                    }
//                    sb.Append("</tr>");
//                    sb.AppendLine("<tr>");
//                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
//                    {
//                        IRow row = sheet.GetRow(i);
//                        if (row == null) continue;
//                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
//                        for (int j = row.FirstCellNum; j < cellCount; j++)
//                        {
//                            if (row.GetCell(j) != null)
//                                sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
//                        }
//                        sb.AppendLine("</tr>");
//                    }
//                    sb.Append("</table>");
//                }
//            }
//            return this.Content(sb.ToString());
//        }

//        //清空数据库
//        private void clearDB()
//        {
//            _context.Employee.RemoveRange(_context.Employee.ToList());
//            _context.Job.RemoveRange(_context.Job.ToList());
//            _context.SaveChanges();
//        }

//        private void importEmployees(ISheet sheet)
//        {
//            IRow headerRow = sheet.GetRow(0); //Get Header Row
//            int cellCount = headerRow.LastCellNum;

//            for (int i = (sheet.FirstRowNum + 2); i <= sheet.LastRowNum; i++) //Read Excel File
//            {
//                IRow row = sheet.GetRow(i);
//                if (row == null) continue;
//                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

//                _context.Employee.Add(GetEmployeeFromExcelRow(row));

//            }
//            _context.SaveChanges();
//        }

//        //Convert each datarow into Job object
//        private Employee GetEmployeeFromExcelRow(IRow row)
//        {
//            Employee employee = new Employee();

//            employee.SeatNo = row.GetCell(0).ToString();
//            employee.ChineseName = row.GetCell(1).ToString();
//            employee.EnglishName = row.GetCell(2).ToString();
//            employee.MSAlias = row.GetCell(3).ToString();
//            employee.WSAlias = row.GetCell(4).ToString();
//            employee.Team = row.GetCell(5).ToString();
//            employee.LOB1 = row.GetCell(6).ToString();
//            employee.TLorTM = row.GetCell(7).ToString();
//            employee.WSManager = row.GetCell(8).ToString();
//            employee.MSManager = row.GetCell(9).ToString();
//            employee.SupportTag = row.GetCell(10).ToString();
//            employee.CheckInTime = row.GetCell(11).ToString();
//            employee.Smartcardtime = row.GetCell(12).ToString();
//            employee.TrainingCompleteTime = row.GetCell(13).ToString();

//            employee.BackgroundInvestigationID = row.GetCell(14).ToString();
//            employee.D4O = row.GetCell(15).ToString();
//            employee.Intern = row.GetCell(16) != null ? row.GetCell(16).ToString() : "";
//            employee.UserID = row.GetCell(17) != null ? row.GetCell(17).ToString() : "";
//            employee.LOB2 = row.GetCell(18) != null ? row.GetCell(18).ToString() : "";

//            try
//            {
//                employee.job = row.GetCell(20) != null ? _context.Job.Find((int)row.GetCell(20).NumericCellValue) : null;
//            }
//            catch (InvalidOperationException e)
//            {
//            }

//            employee.EmailGroup = row.GetCell(22) != null ? row.GetCell(22).ToString() : "";

//            try
//            {
//                employee.Floor = row.GetCell(23) != null ? row.GetCell(23).NumericCellValue.ToString() : "";
//            }
//            catch (InvalidOperationException e)
//            {
//            }

//            try
//            {
//                employee.Month = row.GetCell(24) != null ? row.GetCell(24).NumericCellValue.ToString() : "";
//            }
//            catch (InvalidOperationException e)
//            {
//            }

//            try
//            {
//                employee.Day = row.GetCell(25) != null ? row.GetCell(25).NumericCellValue.ToString() : "";
//            }
//            catch (InvalidOperationException e)
//            {
//            }

//            try
//            {
//                employee.Year = row.GetCell(26) != null ? row.GetCell(26).NumericCellValue.ToString() : "";
//            }
//            catch (InvalidOperationException e)
//            {
//            }


//            employee.PhoneLOB = row.GetCell(27) != null ? row.GetCell(27).ToString() : "";
//            employee.PhoneTeam = row.GetCell(28) != null ? row.GetCell(28).ToString() : "";
//            employee.ExtensionNumber = row.GetCell(29) != null ? row.GetCell(29).ToString() : "";
//            employee.PhoneNumber = row.GetCell(30) != null ? row.GetCell(30).ToString() : "";
//            employee.Installed = row.GetCell(31) != null ? row.GetCell(31).ToString() : "";

//            try
//            {
//                employee.MonthlyTest = row.GetCell(32) != null ? row.GetCell(32).NumericCellValue.ToString() : "";
//            }
//            catch (InvalidOperationException e)
//            {
//            }

//            try
//            {
//                employee.MonthlyTestDate = row.GetCell(33) != null ? row.GetCell(33).StringCellValue : "";
//            }
//            catch (InvalidOperationException e)
//            {
//            }


//            employee.WS = row.GetCell(34) != null ? row.GetCell(34).ToString() : "";
//            employee.MS = row.GetCell(35) != null ? row.GetCell(35).ToString() : "";

//            return employee;
//        }

//        private void importJobs(ISheet sheet)
//        {
//            IRow headerRow = sheet.GetRow(0); //Get Header Row
//            int cellCount = headerRow.LastCellNum;

//            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
//            {
//                IRow row = sheet.GetRow(i);
//                if (row == null) continue;
//                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

//                _context.Job.Add(GetJobFromExcelRow(row));

//            }
//            _context.SaveChanges();

//        }

//        //Convert each datarow into Job object
//        private Job GetJobFromExcelRow(IRow row)
//        {
//            return new Job
//            {
//                Id = int.Parse(row.GetCell(0).ToString()),
//                Description = row.GetCell(1).ToString(),
//                Column1 = row.GetCell(2).ToString(),
//                FamilyID = row.GetCell(3).ToString(),
//                FamilyDescription = row.GetCell(4).ToString()
//            };
//        }



//        public ActionResult Download()
//        {
//            string Files = "wwwroot/UploadExcel/WS Site Seat list.xlsx";
//            byte[] fileBytes = System.IO.File.ReadAllBytes(Files);
//            System.IO.File.WriteAllBytes(Files, fileBytes);
//            MemoryStream ms = new MemoryStream(fileBytes);
//            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "WS Site Seat list.xlsx");
//        }


//        public IActionResult Privacy()
//        {
//            return View();
//        }

//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//        }
//    }
//}
