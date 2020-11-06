using System;

namespace Attendance.Model.Util
{
    public class FirstAndLastPunchCardAnalysisResult
    {
        public int punch_card_hour { get; set; }
        public TimeSpan punch_card_time { get; set; }
        public int punch_card_hour_compare { get; set; }
        public int punch_card_minute_compare { get; set; }
        public bool perfect { get; set; }
        public bool half_late { get; set; }
        public TimeSpan half_late_tm { get; set; }
        public bool late { get; set; }
        public TimeSpan late_tm { get; set; }
        public bool half_leave_early { get; set; }
        public TimeSpan half_leave_early_tm { get; set; }
        public bool leave_early { get; set; }
        public TimeSpan leave_early_tm { get; set; }
        public TimeSpan in_company_tm { get; set; }
        public int punch_card_count_total { get; set; }
        public int punch_card_count_valid { get; set; }

    }
}
