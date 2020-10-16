using Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Attendance
{
    public class Methods
    {
        public void ImportDataToMySQL()
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
                    context.AddRange(attendanceInfoList);
                    var count = context.SaveChanges();

                    Console.WriteLine("此次录入 "+count+" 条数据");
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
            attendance.place = place.Equals("大门-进门")?1:place.Equals("大门-出门")?2:0;
            attendance.pass = row.GetCell(columnOrder[6]).ToString().Equals("1")?true:false;
            return attendance;
        }

        public void AnalysisResultsToMySQL()
        {
            //工具对象定义
            List<AttendanceInfo> attendanceInfoList = new List<AttendanceInfo>();
            AttendanceInfo AttendanceInfo_First;
            AttendanceInfo AttendanceInfo_Last;

            using (var context=new Context())
            {
                //获取数据存到内存
                attendanceInfoList = context.AttendanceInfo
                    .OrderBy(x => x.inout_time)
                    .AsNoTracking()
                    .ToList();

                AttendanceInfo_First = attendanceInfoList.First();
                AttendanceInfo_Last = attendanceInfoList.Last();

                //确认数据表里的数据是同一年同一个月的
                if (attendanceInfoList.First().inout_time.Year != attendanceInfoList.Last().inout_time.Year && attendanceInfoList.First().inout_time.Day != attendanceInfoList.Last().inout_time.Day)
                {
                    Console.WriteLine("此数据表横跨多个月份，请提供一个月份内的数据表");
                    return;
                }

                for(int i = 1; i <= 31; i++)
                {
                    DateTime datetime = new DateTime();
                }

                DateTime time = new DateTime();
                var result = attendanceInfoList.Where(info => info.emp_name == "龚恒海");
                Console.WriteLine(result);

            }
        }
    }
}
