using Attendance.Methods;
using Attendance.Models.DataBaseModels;
using Attendance.Models.UtilModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Attendance.Functions
{
    public class Methods
    {
        public void AnalysisResultsToMySQL()
        {
            //工具对象定义
            List<AttendanceInfo> attendanceInfoList_All = new List<AttendanceInfo>();
            List<AttendanceInfo> attendanceInfoList_Snippet = new List<AttendanceInfo>();
            List<AttendanceInfo> attendanceInfoList_Snippet_2 = new List<AttendanceInfo>();
            AttendanceInfo AttendanceInfo_First;
            AttendanceInfo AttendanceInfo_Last;
            DateTime StartTime;
            DateTime EndTime;
            int Month;
            int Year;
            Dictionary<string, string> Employee_All = new Dictionary<string, string>();
            Dictionary<string, string> Employee_Today = new Dictionary<string, string>();
            AttendanceAnalysis AttendanceAnalysis=null;
            List<AttendanceAnalysis> AttendanceAnalysisList=new List<AttendanceAnalysis>();
            List<SkipEmployee> SkipEmployeesList = new List<SkipEmployee>();

            using (var context=new Context())
            {
                //获取数据存到内存
                attendanceInfoList_All = context.AttendanceInfo
                    .OrderBy(x => x.inout_time)
                    .AsNoTracking()
                    .ToList();

                AttendanceInfo_First = attendanceInfoList_All.First();
                AttendanceInfo_Last = attendanceInfoList_All.Last();

                //确认数据表里的数据是同一年同一个月的
                if (AttendanceInfo_First.inout_time.Year != AttendanceInfo_Last.inout_time.Year && AttendanceInfo_First.inout_time.Day != AttendanceInfo_Last.inout_time.Day)
                {
                    Console.WriteLine("此数据表横跨多个月份，请提供一个月份内的数据表");
                    return;
                }

                //获取本月内所有打过卡的人(工号和姓名以防重名问题)
                //attendanceInfoList_Snippet = attendanceInfoList_All.GroupBy(x => new { x.job_num,x.emp_name}).Select(x=>x.First()).ToList();
                var employeeGroup = attendanceInfoList_All.GroupBy(x => new { x.job_num, x.emp_name }).ToList();

                //获取当月工作日列表
                List<int> workDayList = Utils.GetWorkDayList(AttendanceInfo_First.inout_time.Year, AttendanceInfo_First.inout_time.Month);

                List<ShiftModel> shiftList = new List<ShiftModel>();
                ShiftModel shift = new ShiftModel();

                foreach (var employee in employeeGroup)
                {
                    Employee_All.Add(employee.Key.job_num,employee.Key.emp_name);
                    attendanceInfoList_Snippet = employee.ToList();
                    shift = GetAttendanceAnalysisOfMonth(attendanceInfoList_Snippet);
                }


                //获取本EXCEL里数据所在的时间段
                Year = AttendanceInfo_First.inout_time.Year;
                Month = AttendanceInfo_First.inout_time.Month;

                //获取所有过滤人员
                SkipEmployeesList = context.SkipEmployee.AsNoTracking().ToList();

                StartTime = new DateTime(Year, Month, 1, 4, 0, 0);
                EndTime = new DateTime(Year, Month, 2, 4, 0, 0);

                for (int i = 1; i <= 31; i++)
                {
                    //把一个月的数据按照天进行切片，获取一天内的所有数据
                    attendanceInfoList_Snippet= attendanceInfoList_All.Where(x => x.inout_time > StartTime && x.inout_time < EndTime).ToList();

                    //将当天所有打过卡的人员进行去重
                    attendanceInfoList_Snippet_2 = attendanceInfoList_Snippet.GroupBy(x => new {x.job_num, x.emp_name }).Select(x => x.First()).ToList();

                    //将当天所有打过卡的人存放到Employee_Today工具对象里
                    foreach (var employee in attendanceInfoList_Snippet_2)
                    {
                        Employee_Today.Add(employee.job_num, employee.emp_name);
                    }

                    //清空工具对象
                    attendanceInfoList_Snippet_2.Clear();

                    //对今天所有打过卡的人的数据进行分析
                    foreach(var employee in Employee_Today)
                    {
                        //从当日的数据切片中提取所有此员工的打卡数据
                        attendanceInfoList_Snippet_2 = attendanceInfoList_Snippet.Where(x => x.job_num == employee.Key && x.emp_name==employee.Value).ToList();
                        //跳过部分人员，例如保安，保洁阿姨和其他一些人
                        if (SkipSomeEmployee(attendanceInfoList_Snippet_2,SkipEmployeesList))
                        {
                            //利用方法对此员工今天的数据进行分析
                            GetAttendanceAnalysisOfMonth(attendanceInfoList_Snippet_2);
                            GetAttendanceAnalysisOfDay(attendanceInfoList_Snippet_2);
                        }
                    }

                    //因为基本上没有人会提早到早上四点钟来，或是加班到第二天早上四点，所以选择了这个时间点作为区隔
                    StartTime = StartTime.AddDays(1);
                    EndTime = EndTime.AddDays(1);

                    attendanceInfoList_Snippet.Clear();
                    Employee_Today.Clear();
                }

            }
        }

        public ShiftModel GetAttendanceAnalysisOfMonth(List<AttendanceInfo> attendanceInfos)
        {
            ShiftModel shift = new ShiftModel();

            AttendanceInfo attendanceInfoFirst = attendanceInfos.First();
            DateTime attendanceInfoFirstDateTime = attendanceInfoFirst.inout_time;

            shift.job_num = attendanceInfoFirst.job_num;
            shift.emp_name = attendanceInfoFirst.emp_name;

            //这个月一共的打卡次数
            int allCount = attendanceInfos.Count();

            //所有打卡了的日期
            HashSet<int> daySet = new HashSet<int>();
            foreach (var atten in attendanceInfos)
            {
                daySet.Add(atten.inout_time.Day);
            }
            //一共打卡了几天
            int dayCount = daySet.Count();

            //理顺打卡日期
            daySet.ToList();

            

            //分析当月所有打卡数据

            List<AttendanceInfo> attendanceInfosDay = new List<AttendanceInfo>();
            DateTime StartTime = new DateTime(attendanceInfos.First().inout_time.Year, attendanceInfos.First().inout_time.Month, 1, 4, 0, 0);
            DateTime EndTime = new DateTime(attendanceInfos.First().inout_time.Year, attendanceInfos.First().inout_time.Month, 2, 4, 0, 0);
            Dictionary<int, int> StartHour = new Dictionary<int, int>();
            Dictionary<int, int> EndHour = new Dictionary<int, int>();
            TimeSpan timespan = new TimeSpan();
            int minutes;
            int hour;
            for (int i=0;i<31;i++)
            {
                //当天内的所有打卡记录
                attendanceInfosDay = attendanceInfos.Where(x => x.inout_time > StartTime && x.inout_time < EndTime).ToList();

                //本月所有第一次打卡时间的小时值，0-30视为本小时，31-59.59视为下一个小时
                timespan = attendanceInfosDay.First().inout_time.TimeOfDay;
                minutes = timespan.Minutes % 60;
                hour = minutes > 30 ? timespan.Hours+1 : timespan.Hours;
                if (StartHour.ContainsKey(hour)) 
                    StartHour[hour]= StartHour[hour] + 1;
                else
                    StartHour.Add(hour, 1);

                Dictionary<int, int> mostHour = new Dictionary<int, int>() { { 0,0} };
                foreach(var Hour in StartHour)
                {
                    //if(mostHour.)
                }
                

                //本月所有最后一次打卡时间的小时值，0-30视为本小时，31-59.59视为下一个小时
                timespan = attendanceInfosDay.Last().inout_time.TimeOfDay;
                minutes = timespan.Minutes % 60;
                hour = minutes > 30 ? timespan.Hours + 1 : timespan.Hours;
                if (StartHour.ContainsKey(hour))
                    EndHour[hour]= EndHour[hour]+1;
                else
                    EndHour.Add(hour, 1);

                



                attendanceInfosDay.Clear();
                StartTime = StartTime.AddDays(1);
                EndTime = EndTime.AddDays(1);
            }

            List<int> list = new List<int>();

            List<AttendanceInfo> attendanceInfos_Day = new List<AttendanceInfo>();

            //attendanceInfos_Day = attendanceInfos.Where();

            foreach (var atten in attendanceInfos)
            {
                var m = atten.inout_time.Hour;
                //var m = atten.inout_time.Hour;
            }

            return shift;
        }

        public AttendanceAnalysis GetAttendanceAnalysisOfDay(List<AttendanceInfo> AttendanceInfoList)
        {
            AttendanceAnalysis AttendanceAnalysis=new AttendanceAnalysis();

            AttendanceInfo AttendanceInfo_First = AttendanceInfoList.First();
            AttendanceInfo AttendanceInfo_Last = AttendanceInfoList.Last();

            AttendanceAnalysis.date = AttendanceInfo_First.inout_time.Date;
            AttendanceAnalysis.week = Convert.ToInt32(AttendanceInfo_First.inout_time.DayOfWeek);
            AttendanceAnalysis.job_num = AttendanceInfo_First.job_num;
            AttendanceAnalysis.emp_name = AttendanceInfo_First.emp_name;


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
            AttendanceAnalysis.first_tm = AttendanceInfo_First.inout_time;
            AttendanceAnalysis.last_tm = AttendanceInfo_Last.inout_time;
            AttendanceAnalysis.cmp_tm_total = AttendanceInfo_Last.inout_time-AttendanceInfo_First.inout_time;

            //获取此员工今天打卡次数,一分钟内的打卡进行去重
            int count = 0;
            TimeSpan cmp_tm_valid = new TimeSpan();
            for(int i = 0; i < AttendanceInfoList.Count()-1; i++)
            {
                TimeSpan interval = AttendanceInfoList[i+1].inout_time-AttendanceInfoList[i].inout_time;
                if (interval > new TimeSpan(0, 0, 30))
                {
                    count++;
                    cmp_tm_valid += interval;
                }
            }
            AttendanceAnalysis.cmp_tm_valid = cmp_tm_valid;

            AttendanceAnalysis.times_valid = count+1;

            //此员工今天总共打卡次数
            AttendanceAnalysis.times_total = AttendanceInfoList.Count();
            
            

            return AttendanceAnalysis;
        }

        public bool SkipSomeEmployee(List<AttendanceInfo> AttendanceInfoList, List<SkipEmployee> SkipEmployeesList)
        {
            AttendanceInfo AttendanceInfo = AttendanceInfoList.First();

            int Count=SkipEmployeesList.Where(x => x.emp_name == AttendanceInfo.emp_name && x.job_num == AttendanceInfo.job_num && x.card_id == AttendanceInfo.card_id).Count();

            return Count==0;
        }

        

    }
}
