using Attendance.Model.DataBase;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace Attendance.Controller.ExcelToSQL
{
    class excelToSQLUtil
    {
        /// <summary>
        /// 将Excel内的数据转化为AttendanceInfo对象
        /// </summary>
        /// <param name="row">Excel行</param>
        /// <param name="columnOrder">列名顺序列表</param>
        /// <returns>AttendanceInfo对象</returns>
        public static AttendanceInfo GetAttendanceFromExcel(IRow row, List<int> columnOrder)
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

        /// <summary>
        /// 将Excel内的数据转化为SkipEmployee对象
        /// </summary>
        /// <param name="row">Excel行</param>
        /// <param name="columnOrder">列名顺序列表</param>
        /// <returns>SkipEmployee对象</returns>
        public static SkipEmployee GetSkipEmployeeFromExcel(IRow row, List<int> columnOrder)
        {
            SkipEmployee skipEmployee = new SkipEmployee();
            skipEmployee.card_id = row.GetCell(columnOrder[0]).ToString();
            skipEmployee.job_num = row.GetCell(columnOrder[1]).ToString().Trim();
            skipEmployee.emp_name = row.GetCell(columnOrder[2]).ToString();
            return skipEmployee;
        }

        /// <summary>
        /// 将Excel内的数据转化为HolidayChanges对象
        /// </summary>
        /// <param name="row">Excel行</param>
        /// <param name="columnOrder">列名顺序列表</param>
        /// <returns>HolidayChanges对象</returns>
        public static HolidayChanges GetHolidayChangesFromExcel(IRow row, List<int> columnOrder)
        {
            HolidayChanges holidayChanges = new HolidayChanges();
            holidayChanges.day = DateTime.Parse(row.GetCell(columnOrder[0]).DateCellValue.ToString()).Date;
            holidayChanges.type = row.GetCell(columnOrder[1]).ToString();
            return holidayChanges;
        }

        /// <summary>
        /// 获取ISheet对象
        /// </summary>
        /// <param name="file_name">文件名</param>
        /// <param name="stream">数据流</param>
        /// <returns>ISheet对象</returns>
        public static ISheet GetSheet(string file_name,FileStream stream)
        {
            ISheet sheet=null;

            //文件扩展名
            string file_extension = Path.GetExtension(file_name).ToLower();
            //不同的文件扩展名需要用不同的读取类
            if (file_extension == ".xls")
            {
                HSSFWorkbook hSSFWorkbook = new HSSFWorkbook(stream);
                sheet = hSSFWorkbook.GetSheetAt(0);
            }
            else if (file_extension == ".xlsx")
            {
                XSSFWorkbook xSSFWorkbook = new XSSFWorkbook(stream);
                sheet = xSSFWorkbook.GetSheetAt(0);
            }

            return sheet;
        }

        /// <summary>
        /// 获取指定列名名称的顺序
        /// </summary>
        /// <param name="sheet">数据流</param>
        /// <param name="column_name_list">列名名称列表</param>
        /// <returns></returns>
        public static List<int> GetColumnOrderList(ISheet sheet, List<string> column_name_list)
        {
            //列名行
            IRow headerRow = sheet.GetRow(0);
            //列名列表
            List<string> list = new List<string>();
            for (int j = 0; j < headerRow.LastCellNum; j++)
            {
                ICell cell = headerRow.GetCell(j);
                if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                {
                    list.Add(cell.ToString());
                }
            }

            //确保Excel含有指定列名,否则停止数据录入
            foreach(var x in column_name_list)
            {
                if (!list.Contains(x))
                {
                    Console.WriteLine("请确保excel文件包含以下列:" + string.Join(",", column_name_list.ToArray()));
                    return null;
                }
            }

            //确定列名顺序，使得即使不按照固定顺序也可以录入数据
            List<int> column_order_list = new List<int>();
            foreach (var x in column_name_list)
            {
                column_order_list.Add(list.IndexOf(x));
            }

            return column_order_list;
        }

        /// <summary>
        /// 检测文件是否存在
        /// </summary>
        /// <param name="path_name">文件地址</param>
        public static bool FileExistsOrNot(string path_name)
        {
            bool exists = File.Exists(path_name);
            if (!exists)
            {
                Console.WriteLine("此文件不存在,请输入正确的文件夹和文件名");
            }
            return exists;
        }
    }
}
