using Attendance.Model.DataBase;
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
        public static List<AnalysisOfDay> GetAnalysisOfDay(List<AttendanceInfo> one_emp_month_infos, AnalysisOfMonth analysis_of_month)
        {
            List<AnalysisOfDay> one_emp_day_analysis_list = new List<AnalysisOfDay>();

            AttendanceInfo attendanceInfo_first = one_emp_month_infos.First();
            AttendanceInfo attendanceInfo_last = one_emp_month_infos.Last();

            List<List<AttendanceInfo>> one_emp_day_infos_group = analysisUtil.GetSliceOfDayFromMonth(attendanceInfo_first.inout_time.Date,one_emp_month_infos);

            foreach(var one_emp_day_infos in one_emp_day_infos_group)
            {
                AnalysisOfDay analysis_result_of_one_day = analysisUtil.GetAnalysisResultOfOneDay(one_emp_day_infos);
                one_emp_day_analysis_list.Add(analysis_result_of_one_day);
            }

            AnalysisOfDay analysis_of_day = new AnalysisOfDay();


            analysis_of_day.date = attendanceInfo_first.inout_time.Date;
            analysis_of_day.week = Convert.ToInt32(attendanceInfo_first.inout_time.DayOfWeek);
            analysis_of_day.job_num = attendanceInfo_first.job_num;
            analysis_of_day.emp_name = attendanceInfo_first.emp_name;


            //AttendanceAnalysis.shift = null;
            //AttendanceAnalysis.perfect = null;
            //AttendanceAnalysis.only_half_late = null;
            //AttendanceAnalysis.late_or_early_leave = null;
            //AttendanceAnalysis.no_attnd = null;
            //AttendanceAnalysis.half_late = null;
            //AttendanceAnalysis.half_late_tm = null;
            //AttendanceAnalysis.late = null;
            //AttendanceAnalysis.late_tm = null;
            //AttendanceAnalysis.early_leave = null;
            //AttendanceAnalysis.early_leave_tm = null;
            analysis_of_day.first_tm = attendanceInfo_first.inout_time;
            analysis_of_day.last_tm = attendanceInfo_last.inout_time;
            //analysis_of_day.cmp_tm_total = attendanceInfo_last.inout_time - attendanceInfo_first.inout_time;

            //获取此员工今天打卡次数,一分钟内的打卡进行去重
            int count = 0;
            TimeSpan cmp_tm_valid = new TimeSpan();
            for (int i = 0; i < one_emp_month_infos.Count() - 1; i++)
            {
                TimeSpan interval = one_emp_month_infos[i + 1].inout_time - one_emp_month_infos[i].inout_time;
                if (interval > new TimeSpan(0, 0, 30))
                {
                    count++;
                    cmp_tm_valid += interval;
                }
            }
            //analysis_of_day.cmp_tm_valid = cmp_tm_valid;

            //analysis_of_day.times_valid = count + 1;

            //此员工今天总共打卡次数
            //analysis_of_day.times_total = one_emp_month_infos.Count();



            return one_emp_day_analysis_list;
        }
    }
}
