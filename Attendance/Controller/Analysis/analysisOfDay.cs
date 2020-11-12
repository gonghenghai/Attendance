using Attendance.Model.DataBase;
using Attendance.Model.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Attendance.Controller.Analysis
{
    class analysisOfDay
    {
        /// <summary>
        /// 分析某个员工对象一天的打卡数据
        /// </summary>
        /// <param name="infos">当天员工所有的打卡记录</param>
        /// <returns></returns>
        public static List<AnalysisOfDay> GetAnalysisOfDayList(List<AttendanceInfo> one_emp_month_infos, AnalysisOfMonth analysis_of_month)
        {
            List<AnalysisOfDay> one_emp_day_analysis_list = new List<AnalysisOfDay>();

            List<List<AttendanceInfo>> one_emp_day_infos_group = analysisUtil.GetSliceOfDayFromMonth(one_emp_month_infos.First().inout_time.Date,one_emp_month_infos);

            foreach(var one_emp_day_infos in one_emp_day_infos_group)
            {
                AnalysisOfDay analysis_result_of_one_day = GetAnalysisResultOfOneDay(one_emp_day_infos);
                one_emp_day_analysis_list.Add(analysis_result_of_one_day);
            }

            return one_emp_day_analysis_list;
        }


        public static AnalysisOfDay GetAnalysisResultOfOneDay(List<AttendanceInfo> day_infos)
        {
            AnalysisOfDay analysis_of_day = new AnalysisOfDay();

            AttendanceInfo first_info = day_infos.First();
            AttendanceInfo last_info = day_infos.Last();

            analysis_of_day.date = first_info.inout_time.Date;
            analysis_of_day.week = Convert.ToInt32(first_info.inout_time.DayOfWeek);
            analysis_of_day.card_id = first_info.card_id;
            analysis_of_day.job_num = first_info.job_num;
            analysis_of_day.emp_name = first_info.emp_name;
            analysis_of_day.first_tm = first_info.inout_time;
            analysis_of_day.last_tm = last_info.inout_time;

            TimeSpan timespan = first_info.inout_time.TimeOfDay;
            analysis_of_day.first_hr = timespan.Minutes % 60 > 30 ? timespan.Hours + 1 : timespan.Hours;
            timespan = last_info.inout_time.TimeOfDay;
            analysis_of_day.last_hr = timespan.Minutes % 60 > 30 ? timespan.Hours + 1 : timespan.Hours;
            if (analysis_of_day.last_tm.Day>analysis_of_day.first_tm.Day)
            {
                analysis_of_day.last_hr += 24;
            }
            analysis_of_day.shift = analysisUtil.GetShift(analysis_of_day.first_hr);

            FirstAndLastPunchCardAnalysisResult result = analysisUtil.GetFirstAndLastPunchCardAnalysis(analysis_of_day.first_tm, analysis_of_day.last_tm, analysis_of_day.first_hr, analysis_of_day.last_hr, analysis_of_day.shift);

            analysis_of_day.punch_card_hour = result.punch_card_hour;
            analysis_of_day.punch_card_time = result.punch_card_time;
            analysis_of_day.punch_card_hour_compare = result.punch_card_hour_compare;
            analysis_of_day.punch_card_minute_compare = result.punch_card_minute_compare;
            analysis_of_day.perfect = result.perfect;
            analysis_of_day.half_late = result.half_late;
            analysis_of_day.half_late_tm = result.half_late_tm;
            analysis_of_day.late = result.late;
            analysis_of_day.late_tm = result.late_tm;
            analysis_of_day.half_leave_early = result.half_leave_early;
            analysis_of_day.half_leave_early_tm = result.half_leave_early_tm;
            analysis_of_day.leave_early = result.leave_early;
            analysis_of_day.leave_early_tm = result.leave_early_tm;

            analysis_of_day.punch_card_count_total = day_infos.Count;
            analysis_of_day.punch_card_count_valid = analysisUtil.GetValidPunchCardCount(day_infos);
            //analysis_of_day.in_company_time = analysisUtil.GetInCompanyTime(day_infos);


            return analysis_of_day;
        }

    }
}
