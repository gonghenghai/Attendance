using Attendance.Model.DataBase;
using Attendance.Model.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Attendance.Controller.Analysis
{
    class analysisUtil
    {
        /// <summary>
        /// 获取某个月份的工作日列表
        /// </summary>
        /// <param name="Year">目标年份</param>
        /// <param name="Month">目标月份</param>
        /// <returns></returns>
        public static List<int> GetWorkDayListOfMonth(int year, int month)
        {
            DateTime firstDay = new DateTime(year, month, 1);

            //获取本月节假日，调班等特殊日期对象
            List<HolidayChanges> holidays = new List<HolidayChanges>();
            List<HolidayChanges> overtimes = new List<HolidayChanges>();
            using (var context = new Context())
            {
                holidays = context.HolidayChanges.Where(x => x.type == "放假" && x.day >= firstDay.Date && x.day < firstDay.Date.AddMonths(1)).OrderBy(x => x.day).AsNoTracking().ToList();
                overtimes = context.HolidayChanges.Where(x => x.type == "加班" && x.day >= firstDay.Date && x.day < firstDay.Date.AddMonths(1)).OrderBy(x => x.day).AsNoTracking().ToList();
            }

            //将特殊日期值存入列表
            List<int> holidays_day_list = holidays.Select(t => t.day.Day).ToList();
            List<int> overtimes_day_list = overtimes.Select(t => t.day.Day).ToList();

            //工作日List
            List<int> workDayList = new List<int>();
            //用来遍历本月的工具对象
            DateTime day = firstDay;

            //获取理论上的工作日(仅排除周末)
            for (int i = 1; i <= 31; i++)
            {
                //利用AddDays方法可以确保遍历本月所有的日期而不会报错，但是有可能会出现
                //下个月的日期，而下个月的日期由于星期的不同会污染数据，所以用Month加以甄别
                if (day.Month == month)
                {
                    if (day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday)
                        workDayList.Add(i);
                }
                day = day.AddDays(1);
            }

            //删除所有假期日期
            workDayList.RemoveAll(x => holidays_day_list.Contains(x));
            //添加所有加班日期
            foreach (var overtime in overtimes_day_list)
            {
                workDayList.Add(overtime);
            }

            //理顺日期列表并返回
            workDayList.Sort();
            return workDayList;
        }

        /// <summary>
        /// 获取某年某月里所有周末的日期列表(去掉周末加班的)
        /// </summary>
        /// <param name="year">目标年份</param>
        /// <param name="month">目标月份</param>
        /// <returns></returns>
        public static List<int> GetWeekendDayListOfMonth(int year, int month)
        {
            List<int> weekend_day_list = new List<int>();
            DateTime day = new DateTime(year, month, 1);

            for (int i = 1; i <= 31; i++)
            {
                //避免统计到下个月的数据
                if (day.Month == month)
                {
                    if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                        weekend_day_list.Add(i);
                }
                day = day.AddDays(1);
            }

            List<HolidayChanges> overtimes = new List<HolidayChanges>();
            using (var context = new Context())
            {
                overtimes = context.HolidayChanges.Where(x => x.type == "加班" && x.day >= day.Date && x.day < day.Date.AddMonths(1)).OrderBy(x => x.day).AsNoTracking().ToList();
            }
            List<int> overtimes_list = overtimes.Select(t => t.day.Day).ToList();

            //去掉加班的日期列表
            weekend_day_list.Except(overtimes_list);
            return weekend_day_list;
        }

    }
}
