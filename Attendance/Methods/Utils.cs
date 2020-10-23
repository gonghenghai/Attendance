using System;
using System.Collections.Generic;
using System.Linq;
using Attendance.Models.DataBaseModels;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Methods
{
    public class Utils
    {
        /// <summary>
        /// 获取某个月份的工作日列表
        /// </summary>
        /// <param name="Year">目标年份</param>
        /// <param name="Month">目标月份</param>
        /// <returns></returns>
        public static List<int> GetWorkDayList(int year, int month)
        {
            DateTime firstDay = new DateTime(year,month,1);

            //获取本月节假日，调班等特殊日期
            List<HolidayChanges> holidays = new List<HolidayChanges>();
            List<HolidayChanges> overTimes = new List<HolidayChanges>();
            using (var contex = new Context())
            {
                holidays = contex.HolidayChanges.Where(x => x.type == "放假" && x.day >= firstDay.Date && x.day < firstDay.Date.AddMonths(1)).OrderBy(x => x.day).AsNoTracking().ToList();
                overTimes = contex.HolidayChanges.Where(x => x.type == "加班" && x.day >= firstDay.Date && x.day < firstDay.Date.AddMonths(1)).OrderBy(x => x.day).AsNoTracking().ToList();
            }
            List<int> holidays_DayList = new List<int>();
            List<int> overTimes_DayList = new List<int>();
            holidays_DayList = holidays.Select(t => t.day.Day).ToList();
            overTimes_DayList = overTimes.Select(t => t.day.Day).ToList();

            //工作日List
            List<int> workDayList = new List<int>();

            //获取理论上的工作日
            for (int i = 1; i <= 31; i++)
            {
                if (date.Month == attendanceInfoFirstDateTime.Month)
                {
                    if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                        workDayList.Add(i);
                }
                date = date.AddDays(1);
            }
            workDayList.RemoveAll(x => holidayChangesList_Holiday_All_Days.Contains(x));
            foreach (var overtime in holidayChangesList_OverTime_All_Days)
            {
                if (!workDayList.Contains(overtime))
                {
                    workDayList.Add(overtime);
                }
            }
            //理论工作日列表和数量
            workDayList.Sort();
            int workDayCount = workDayList.Count();
            return workDayList;
        }
    }
}
