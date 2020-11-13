using Attendance.Model.DataBase;
using Attendance.Model.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Attendance.Controller.Analysis
{
    class analysisMain
    {
        /// <summary>
        /// 入口方法
        /// </summary>
        public void AnalysisResultsToMySQL()
        {
            using (var context = new Context())
            {
                //获取一个月内所有员工的打卡信息
                List<AttendanceInfo> attendance_info_all_list = context.AttendanceInfo
                    .OrderBy(x => x.inout_time)
                    .AsNoTracking()
                    .ToList();

                if (attendance_info_all_list.Count == 0)
                {
                    Console.WriteLine("数据表数据为空,停止分析");
                    return;
                }

                //确认数据表里的数据是同年同月的
                if (attendance_info_all_list.First().inout_time.Year != attendance_info_all_list.Last().inout_time.Year && attendance_info_all_list.First().inout_time.Month != attendance_info_all_list.Last().inout_time.Month)
                {
                    Console.WriteLine("此数据表横跨多个月份，请提供一个月的员工打卡数据");
                    return;
                }

                int year = attendance_info_all_list.First().inout_time.Year;
                int month = attendance_info_all_list.First().inout_time.Month;

                //获取工作日列表
                List<int> work_day_list = analysisUtil.GetWorkDayListOfMonth(year, month);
                //获取周末日期列表(排除周末加班)
                List<int> weekend_day_list = analysisUtil.GetWeekendDayListOfMonth(year, month);
                //获取所有不需要分析的人员
                List<SkipEmployee> skip_employee_list = context.SkipEmployee.AsNoTracking().ToList();

                AnalysisResult analysis_result = GetAnalysisResult(attendance_info_all_list,skip_employee_list, work_day_list, weekend_day_list);

                AnalysisOfMonth first_analysis_of_month = analysis_result.analysis_of_month.First();
                int month_exist_count = context.AnalysisOfMonth.Where(x=> x.job_num==first_analysis_of_month.job_num && x.emp_name==first_analysis_of_month.emp_name && x.date==first_analysis_of_month.date).Count();
                if (month_exist_count == 0)
                {
                    context.AnalysisOfMonth.AddRange(analysis_result.analysis_of_month);
                    var month_count = context.SaveChanges();
                    Console.WriteLine("存储员工月度打卡分析记录:" + month_count + "条");
                }
                else
                {
                    Console.WriteLine("月度分析结果中的第一条在数据库中已存在,放弃录入");
                }

                AnalysisOfDay first_analysis_of_day = analysis_result.analysis_of_day.First();
                int day_exist_count = context.AnalysisOfDay.Where(x => x.job_num == first_analysis_of_day.job_num && x.emp_name == first_analysis_of_day.emp_name && x.date == first_analysis_of_day.date).Count();
                if (day_exist_count==0)
                {
                    TimeSpan zero = new TimeSpan(0, 0, 0);
                    List<AnalysisOfDay> a = analysis_result.analysis_of_day.Where(x => x.half_late_tm<zero ||x.late_tm<zero ||x.half_leave_early_tm<zero ||x.leave_early_tm<zero).ToList();
                    context.AnalysisOfDay.AddRange(a);

                    context.AnalysisOfDay.AddRange(analysis_result.analysis_of_day);
                    var day_count = context.SaveChanges();
                    Console.WriteLine("存储员工每天打卡分析记录:" + day_count + "条");
                }
                else
                {
                    Console.WriteLine("员工每天打卡记录分析结果中的第一条在数据库中已存在,放弃录入");
                }

            }
        }

        /// <summary>
        /// 对所有员工的打卡数据进行分析,返回月度和每日的分析结果
        /// </summary>
        /// <param name="attendance_info_all_list">一个月里所有员工的所有打卡数据</param>
        /// <param name="skip_employee_list">不需要分析的员工列表</param>
        /// <param name="work_day_list">某月的工作日列表</param>
        /// <param name="weekend_day_List">某月的周末日期列表(排除周末加班)</param>
        /// <returns>所有员工的月度和每日打卡数据分析结果</returns>
        public AnalysisResult GetAnalysisResult(List<AttendanceInfo> attendance_info_all_list, List<SkipEmployee> skip_employee_list, List<int> work_day_list, List<int> weekend_day_List)
        {
            //以员工为目标对数据进行分组
            //因为暂时不会写这个对象的类型,没办法通过传参来调用,所以只能放在具体的方法里,希望以后可以重写
            var employee_group = attendance_info_all_list.GroupBy(x => new { x.job_num, x.emp_name }).ToList();

            //从员工分组列表中剔除那些不需要分析的
            foreach (var skip_employee in skip_employee_list)
            {
                employee_group.RemoveAll(x => x.Key.job_num == skip_employee.job_num && x.Key.emp_name == skip_employee.emp_name);
            }

            //单个员工的月度打卡数据分析结果
            AnalysisOfMonth analysis_of_month = new AnalysisOfMonth();
            //所有员工的月度打卡数据分析结果
            List<AnalysisOfMonth> analysis_of_month_list = new List<AnalysisOfMonth>();
            //单个员工本月内每天打卡数据的分析结果
            List<AnalysisOfDay> analysis_of_day_one_list = new List<AnalysisOfDay>();
            //所有员工本月内每天打卡数据的分析结果
            List<AnalysisOfDay> analysis_of_day_all_list = new List<AnalysisOfDay>();

            foreach (var employee in employee_group)
            {
                analysis_of_day_one_list = analysisOfDay.GetAnalysisOfDayList(employee.ToList(), analysis_of_month);
                foreach (var v in analysis_of_day_one_list)
                {
                    analysis_of_day_all_list.Add(v);
                }

                analysis_of_month = analysisOfMonth.GetAnalysisOfMonth(employee.ToList(), work_day_list, weekend_day_List, analysis_of_day_one_list);
                analysis_of_month_list.Add(analysis_of_month);
            }

            AnalysisResult result = new AnalysisResult();
            result.analysis_of_day = analysis_of_day_all_list;
            result.analysis_of_month = analysis_of_month_list;

            return result;
        }
    }
}
