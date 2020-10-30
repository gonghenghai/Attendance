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
        public static AnalysisOfDay GetAnalysisOfDay(List<AttendanceInfo> infos, AnalysisOfMonth analysisOfMonth)
        {
            AnalysisOfDay analysis_of_day = new AnalysisOfDay();

            AttendanceInfo AttendanceInfo_First = infos.First();
            AttendanceInfo AttendanceInfo_Last = infos.Last();

            analysis_of_day.date = AttendanceInfo_First.inout_time.Date;
            analysis_of_day.week = Convert.ToInt32(AttendanceInfo_First.inout_time.DayOfWeek);
            analysis_of_day.job_num = AttendanceInfo_First.job_num;
            analysis_of_day.emp_name = AttendanceInfo_First.emp_name;


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
            analysis_of_day.first_tm = AttendanceInfo_First.inout_time;
            analysis_of_day.last_tm = AttendanceInfo_Last.inout_time;
            analysis_of_day.cmp_tm_total = AttendanceInfo_Last.inout_time - AttendanceInfo_First.inout_time;

            //获取此员工今天打卡次数,一分钟内的打卡进行去重
            int count = 0;
            TimeSpan cmp_tm_valid = new TimeSpan();
            for (int i = 0; i < infos.Count() - 1; i++)
            {
                TimeSpan interval = infos[i + 1].inout_time - infos[i].inout_time;
                if (interval > new TimeSpan(0, 0, 30))
                {
                    count++;
                    cmp_tm_valid += interval;
                }
            }
            analysis_of_day.cmp_tm_valid = cmp_tm_valid;

            analysis_of_day.times_valid = count + 1;

            //此员工今天总共打卡次数
            analysis_of_day.times_total = infos.Count();



            return analysis_of_day;
        }
    }
}
