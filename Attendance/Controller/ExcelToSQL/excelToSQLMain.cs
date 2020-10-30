using Attendance.Model.DataBase;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Attendance.Controller.ExcelToSQL
{
    class excelToSQLMain
    {
        /// <summary>
        /// 将员工打卡信息从Excel表格导入到Mysql数据库
        /// </summary>
        /// <param name="folder_name">文件夹路径</param>
        /// <param name="file_name">文件名</param>
        public void ImportAttendanceInfoToMySQL(string folder_name, string file_name)
        {
            string path_name = Path.Combine(folder_name, file_name);

            //检测文件是否存在
            if (!excelToSQLUtil.FileExistsOrNot(path_name)) return;

            //打开EXCEL对象,提取数据
            using (var stream = new FileStream(path_name, FileMode.Open))
            {
                stream.Position = 0;

                //获取sheet
                ISheet sheet=excelToSQLUtil.GetSheet(file_name,stream);
                if (sheet == null) return;

                //获取列名顺序列表
                List<int> column_order_list = excelToSQLUtil.GetColumnOrderList(sheet,new List<string> {"序号","卡号","工号","姓名","时间","地点","通过"});
                if (column_order_list == null) return;

                IRow row = null;
                List<AttendanceInfo> attendance_info_list = new List<AttendanceInfo>();

                using (var context = new Context())
                {
                    //防止EXCEL表格多次录入
                    IRow first_info_row = sheet.GetRow(sheet.FirstRowNum + 1);
                    AttendanceInfo attendance_info = excelToSQLUtil.GetAttendanceFromExcel(first_info_row, column_order_list);
                    var exist_count = context.AttendanceInfo
                        .Where(x => x.inout_time == attendance_info.inout_time && x.emp_name == attendance_info.emp_name && x.place == attendance_info.place)
                        .Count();
                    if (exist_count > 0)
                    {
                        Console.WriteLine("EXCEL内的第一条数据已存在,该EXCEL内的数据应该已经导入过了,请不要重复导入相同的Excel文件.");
                        return;
                    }

                    //数据录入
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        //这里之所以加上Count的判断,是因为有些无效数据,例如门长时间未关报警只有六列数据
                        if (row.Cells.Count >= 7)
                        {
                            attendance_info = excelToSQLUtil.GetAttendanceFromExcel(row, column_order_list);
                            attendance_info_list.Add(attendance_info);
                        }
                    }

                    //将数据存储到数据库中
                    context.AttendanceInfo.AddRange(attendance_info_list);
                    var count = context.SaveChanges();

                    Console.WriteLine("此次录入 " + count + " 条数据");
                }
            }
        }

        /// <summary>
        /// 将需要跳过的员工信息从Excel表格导入到Mysql数据库
        /// </summary>
        /// <param name="folder_name">文件夹路径</param>
        /// <param name="file_name">文件名</param>
        public void ImportSkipEmployeeToMySQL(string folder_name,string file_name)
        {
            string path_name = Path.Combine(folder_name, file_name);

            //检测文件是否存在
            if (!excelToSQLUtil.FileExistsOrNot(path_name)) return;

            using (var stream = new FileStream(path_name, FileMode.Open))
            {
                stream.Position = 0;

                //获取sheet
                ISheet sheet = excelToSQLUtil.GetSheet(file_name, stream);
                if (sheet == null) return;

                //获取列名顺序列表
                List<int> column_order_list = excelToSQLUtil.GetColumnOrderList(sheet, new List<string> {"卡号", "工号", "姓名"});
                if (column_order_list == null) return;

                IRow row = null;
                List<SkipEmployee> skip_employee_list_original = new List<SkipEmployee>();
                List<SkipEmployee> skip_employee_list = new List<SkipEmployee>();

                using (var context = new Context())
                {
                    skip_employee_list_original = context.SkipEmployee.AsNoTracking().ToList();
                    //数据库里已经有的数据数量
                    int already_has_count = 0;

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        SkipEmployee skip_employee = excelToSQLUtil.GetSkipEmployeeFromExcel(row, column_order_list);

                        //假如已经录入了，就避免再次录入
                        int already_exists = skip_employee_list_original.Where(x => x.card_id == skip_employee.card_id && x.job_num == skip_employee.job_num && x.emp_name == skip_employee.emp_name).Count();
                        if (already_exists == 0)
                            skip_employee_list.Add(skip_employee);
                        else
                            already_has_count++;
                    }

                    context.SkipEmployee.AddRange(skip_employee_list);
                    var count = context.SaveChanges();
                    Console.WriteLine("此次录入 " + count + " 条数据," + ",重复数据 " + already_has_count + " 条");
                }
            }
        }

        /// <summary>
        /// 将特殊日期信息从Excel表格导入到Mysql数据库
        /// </summary>
        /// <param name="folder_name">文件夹路径</param>
        /// <param name="file_name">文件名</param>
        public void ImportHolidayChangesToMySQL(string folder_name, string file_name)
        {
            string path_name = Path.Combine(folder_name, file_name);

            //检测文件是否存在
            if (!excelToSQLUtil.FileExistsOrNot(path_name)) return;

            using (var stream = new FileStream(path_name, FileMode.Open))
            {
                stream.Position = 0;

                //获取sheet
                ISheet sheet = excelToSQLUtil.GetSheet(file_name, stream);
                if (sheet == null) return;

                //获取列名顺序列表
                List<int> column_order_list = excelToSQLUtil.GetColumnOrderList(sheet, new List<string> { "日期", "类型" });
                if (column_order_list == null) return;

                IRow row = null;
                List<HolidayChanges> holiday_changes_list_original = new List<HolidayChanges>();
                List<HolidayChanges> holiday_changes_list = new List<HolidayChanges>();

                using (var context = new Context())
                {
                    holiday_changes_list_original = context.HolidayChanges.AsNoTracking().ToList();
                    //数据库里已经有的数据数量
                    int alreadyHasCount = 0;

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        HolidayChanges holiday_changes = excelToSQLUtil.GetHolidayChangesFromExcel(row, column_order_list);

                        //假如已经录入了，就避免再次录入
                        int alreadyExists = holiday_changes_list_original.Where(x => x.day == holiday_changes.day && x.type == holiday_changes.type).Count();
                        if (alreadyExists == 0)
                            holiday_changes_list.Add(holiday_changes);
                        else
                            alreadyHasCount++;
                    }

                    context.HolidayChanges.AddRange(holiday_changes_list);
                    var count = context.SaveChanges();
                    Console.WriteLine("此次录入 " + count + " 条数据," + ",重复数据 " + alreadyHasCount + " 条");
                }
            }
        }

        

    }
}
