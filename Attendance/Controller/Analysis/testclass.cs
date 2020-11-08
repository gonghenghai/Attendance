using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace Attendance.Controller.Analysis
{
    public class testclass
    {
        public void test()
        {
            //List<int> a = new List<int>{ 1, 2, 3, 5, 9 };
            //List<int> b = new List<int> { 4,3,9 };
            //var c = a.Intersect(b).ToList();
            //var d = a.Except(b).ToList();
            //List<int> e = a.Union(b).ToList();
            //e.Sort();
            //var f = a.Concat(b);
            //a.AddRange(b);

            //List<int> a = new List<int> { 1, 1, 3, 3, 3, 5, 7, 9 };
            //var f = a.ToString();
            //var m = a.Distinct().ToList();

            //for(int f = 0; f < a.Count; f++)
            //{
            //    Console.WriteLine(f);
            //}

            DateTime m = new DateTime(2020,5,5,6,0,0);
            DateTime m1 = m.Date;

            DateTime first = new DateTime(2020, 5, 5, 6, 9, 1);
            DateTime last = new DateTime(2020, 5, 5, 7, 10, 6);

            var f1 = first - last;
            var f2 = -f1;
            var t=f1.GetType();

            List<string> a = new List<string>() { "小明", "小王", "小红", "小三", "小四", "小玩", };
            var d = a.ToArray();
            var f = string.Join(",", a);
            foreach(var x in a)
            {
                Console.WriteLine(x);
            }

            if (true)
                Console.WriteLine("aaa");
                Console.WriteLine("bbb");



            List<List<int>> outer_list_weekend = new List<List<int>>();
            List<int> inner_list_weekend = new List<int>();

            List<int> weekend_and_less_than_workday_list = new List<int> { 1, 5, 6, 7, 8, 9, 10, 11, 15, 16, 19, 23 };

            List<int> weekendDayList = new List<int> { 5, 6, 8, 9, 15, 16 };

            inner_list_weekend.Add(weekend_and_less_than_workday_list[0]);

            int i = 1;
            while (true)
            {
                if (weekend_and_less_than_workday_list[i] - weekend_and_less_than_workday_list[i - 1] == 1)
                {
                    inner_list_weekend.Add(weekend_and_less_than_workday_list[i]);
                }
                else
                {
                    outer_list_weekend.Add(inner_list_weekend);
                    inner_list_weekend = new List<int>();
                    inner_list_weekend.Add(weekend_and_less_than_workday_list[i]);
                }
                if (i < weekend_and_less_than_workday_list.Count() - 1)
                {
                    i++;
                }
                else
                {
                    break;
                }
            }

            outer_list_weekend.Add(inner_list_weekend);

            //获取不包含weekend的切片
            List<List<int>> outer_list = new List<List<int>>();
            for (int j = 0; j < outer_list_weekend.Count(); j++)
            {
                List<int> list = outer_list_weekend[j];
                List<int> list2 = list.Except(weekendDayList).ToList<int>();
                if (list2.Count() != 0)
                {
                    outer_list.Add(list2);
                }
            }
        }
    }
}
