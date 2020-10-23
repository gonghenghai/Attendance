using Attendance.Models;
using Attendance.Models.DataBaseModels;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Attendance.Methods
{
    class ImporDatatFromExcelToMySQL
    {
        public void ImportAttendanceInfoToMySQL()
        {
            //工具对象定义
            List<string> list = new List<string>();
            List<AttendanceInfo> attendanceInfoList = new List<AttendanceInfo>();
            AttendanceInfo attendanceInfo;
            ISheet sheet;
            IRow row = null;

            //打开EXCEL对象,提取数据
            using (var stream = new FileStream("D:\\test.xlsx", FileMode.Open))
            {
                stream.Position = 0;
                XSSFWorkbook xSSFWorkbook = new XSSFWorkbook(stream);
                sheet = xSSFWorkbook.GetSheetAt(0);
                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;
                for (int j = 0; j < cellCount; j++)
                {
                    ICell cell = headerRow.GetCell(j);
                    if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                    {
                        list.Add(cell.ToString());
                    }
                }

                //确保此EXCEL含有以下列名,否则停止数据录入
                bool rightHead = list.Contains("序号") && list.Contains("卡号") && list.Contains("工号") &&
                                list.Contains("姓名") && list.Contains("时间") && list.Contains("地点") && list.Contains("通过");
                if (!rightHead)
                {
                    Console.WriteLine("请确保excel文件包含以下列:序号,卡号,工号,姓名,时间,地点,通过");
                    return;
                }

                //确定列名顺序，使得即使不按照固定顺序也可以录入数据
                List<int> columnOrder = new List<int>() {list.IndexOf("序号"), list.IndexOf("卡号"), list.IndexOf("工号"),
                    list.IndexOf("姓名"), list.IndexOf("时间"), list.IndexOf("地点"), list.IndexOf("通过")};

                //清空工具对象
                list.Clear();

                using (var context = new Context())
                {
                    //防止EXCEL表格多次录入
                    IRow FirstInfoRow = sheet.GetRow(sheet.FirstRowNum + 1);
                    attendanceInfo = GetAttendanceFromExcel(FirstInfoRow, columnOrder);
                    var ExistCount = context.AttendanceInfo
                        .Where(x => x.inout_time == attendanceInfo.inout_time && x.emp_name == attendanceInfo.emp_name && x.place == attendanceInfo.place)
                        .Count();
                    if (ExistCount > 0)
                    {
                        Console.WriteLine("EXCEL内的第一条数据就已经重复，该EXCEL内的数据应该已经导入过了，请不要重复导入相同的数据。");
                        return;
                    }

                    //数据录入
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        //这里之所以加上Count的判断，是因为有些无效数据，例如门长时间未关报警(有六列数据)
                        if (row.Cells.Count >= 7)
                        {
                            attendanceInfo = GetAttendanceFromExcel(row, columnOrder);
                            attendanceInfoList.Add(attendanceInfo);
                        }
                    }
                    //将数据存储到数据库中
                    context.AttendanceInfo.AddRange(attendanceInfoList);
                    var count = context.SaveChanges();

                    Console.WriteLine("此次录入 " + count + " 条数据");
                }
            }
        }

        private AttendanceInfo GetAttendanceFromExcel(IRow row, List<int> columnOrder)
        {
            AttendanceInfo attendance = new AttendanceInfo();
            attendance.seq_num = int.Parse(row.GetCell(columnOrder[0]).ToString());
            attendance.card_id = row.GetCell(columnOrder[1]).ToString();
            attendance.job_num = row.GetCell(columnOrder[2]).ToString().Trim();
            attendance.emp_name = row.GetCell(columnOrder[3]).ToString();
            attendance.inout_time = Convert.ToDateTime(row.GetCell(columnOrder[4]).ToString().Substring(0, 19));
            string place = row.GetCell(columnOrder[5]).ToString();
            attendance.place = 0;
            switch (place)
            {
                case "大门-进门":
                    attendance.place = 51;
                    break;
                case "大门-出门":
                    attendance.place = 52;
                    break;
                case "6F大门-进门":
                    attendance.place = 61;
                    break;
                case "6F大门-出门":
                    attendance.place = 62;
                    break;
                case "7F大门-进门":
                    attendance.place = 71;
                    break;
                case "7F大门-出门":
                    attendance.place = 72;
                    break;
            }
            attendance.pass = row.GetCell(columnOrder[6]).ToString().Equals("1") ? true : false;
            return attendance;
        }

        public void ImportSkipEmployeeToMySQL()
        {
            using (var stream = new FileStream("D:\\skip.xlsx", FileMode.Open))
            {
                stream.Position = 0;
                List<string> list = new List<string>();
                XSSFWorkbook xSSFWorkbook = new XSSFWorkbook(stream);
                ISheet sheet = xSSFWorkbook.GetSheetAt(0);
                IRow headRow = sheet.GetRow(0);
                int cellCount = headRow.LastCellNum;
                for (int i = 0; i < cellCount; i++)
                {
                    ICell cell = headRow.GetCell(i);
                    if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                    {
                        list.Add(cell.ToString());
                    }
                }

                IRow row = null;
                List<SkipEmployee> skipEmployeeListOriginal = new List<SkipEmployee>();
                List<SkipEmployee> skipEmployeeList = new List<SkipEmployee>();
                SkipEmployee skipEmployee = new SkipEmployee();


                bool rightHead = list.Contains("卡号") && list.Contains("工号") && list.Contains("姓名");
                if (!rightHead)
                {
                    Console.WriteLine("请确保EXCEL文件包含以下列:卡号，工号，姓名");
                    return;
                }

                List<int> columnOrder = new List<int>() { list.IndexOf("卡号"), list.IndexOf("工号"), list.IndexOf("姓名") };

                using (var context = new Context())
                {
                    skipEmployeeListOriginal = context.SkipEmployee.AsNoTracking().ToList();

                    int alreadyHasCount = 0;

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        skipEmployee = GetSkipEmployeeFromExcel(row, columnOrder);
                        //假如已经录入了，就避免再次录入
                        int alreadyExists = skipEmployeeListOriginal.Where(x => x.card_id == skipEmployee.card_id && x.job_num == skipEmployee.job_num && x.emp_name == skipEmployee.emp_name).Count();
                        if (alreadyExists == 0)
                            skipEmployeeList.Add(skipEmployee);
                        else
                            alreadyHasCount++;
                    }
                    context.SkipEmployee.AddRange(skipEmployeeList);
                    var count = context.SaveChanges();
                    Console.WriteLine("此次录入 " + count + " 条数据," + ",重复数据 " + alreadyHasCount + " 条");
                }
            }
        }

        public void ImportHolidayChangesToMySQL()
        {
            using (var stream = new FileStream("D:\\holiday.xlsx", FileMode.Open))
            {
                stream.Position = 0;
                List<string> list = new List<string>();
                XSSFWorkbook xSSFWorkbook = new XSSFWorkbook(stream);
                ISheet sheet = xSSFWorkbook.GetSheetAt(0);
                IRow headRow = sheet.GetRow(0);
                int cellCount = headRow.LastCellNum;
                for (int i = 0; i < cellCount; i++)
                {
                    ICell cell = headRow.GetCell(i);
                    if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                    {
                        list.Add(cell.ToString());
                    }
                }

                IRow row = null;
                List<HolidayChanges> HolidayChangesListOriginal = new List<HolidayChanges>();
                List<HolidayChanges> HolidayChangesList = new List<HolidayChanges>();
                HolidayChanges holidayChanges = new HolidayChanges();


                bool rightHead = list.Contains("日期") && list.Contains("类型");
                if (!rightHead)
                {
                    Console.WriteLine("请确保EXCEL文件包含以下列:日期，类型");
                    return;
                }

                List<int> columnOrder = new List<int>() { list.IndexOf("日期"), list.IndexOf("类型") };

                using (var context = new Context())
                {
                    HolidayChangesListOriginal = context.HolidayChanges.AsNoTracking().ToList();

                    int alreadyHasCount = 0;

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        holidayChanges = GetHolidayChangesFromExcel(row, columnOrder);

                        //假如已经录入了，就避免再次录入
                        int alreadyExists = HolidayChangesListOriginal.Where(x => x.day == holidayChanges.day && x.type == holidayChanges.type).Count();
                        if (alreadyExists == 0)
                            HolidayChangesList.Add(holidayChanges);
                        else
                            alreadyHasCount++;
                    }
                    context.HolidayChanges.AddRange(HolidayChangesList);
                    var count = context.SaveChanges();
                    Console.WriteLine("此次录入 " + count + " 条数据," + ",重复数据 " + alreadyHasCount + " 条");
                }
            }
        }

        public HolidayChanges GetHolidayChangesFromExcel(IRow row, List<int> columnOrder)
        {
            HolidayChanges holidayChanges = new HolidayChanges();
            holidayChanges.day = DateTime.Parse(row.GetCell(columnOrder[0]).DateCellValue.ToString()).Date;
            holidayChanges.type = row.GetCell(columnOrder[1]).ToString();
            return holidayChanges;
        }

        public SkipEmployee GetSkipEmployeeFromExcel(IRow row, List<int> columnOrder)
        {
            SkipEmployee skipEmployee = new SkipEmployee();
            skipEmployee.card_id = row.GetCell(columnOrder[0]).ToString();
            skipEmployee.job_num = row.GetCell(columnOrder[1]).ToString().Trim();
            skipEmployee.emp_name = row.GetCell(columnOrder[2]).ToString();
            return skipEmployee;
        }

    }
}
