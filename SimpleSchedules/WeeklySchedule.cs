using System;
using System.Collections.Generic;

namespace SimpleSchedules
{
    public class WeeklySchedule : DailySchedule
    {
        public DayOfWeek[] LaunchDays { get { return days.ToArray(); } }

        private List<DayOfWeek> days;

        protected WeeklySchedule() { }

        public WeeklySchedule(DayOfWeek[] launchDays, Time occursOnceAt,
             bool enabled = true, string description = null) : base(occursOnceAt, enabled, description)
        {
            InitWeeklySchedule(launchDays);
        }

        public WeeklySchedule(DayOfWeek[] launchDays, DailyIntervalUnit intervalUnit, int interval, Time? startAt, Time? endAt,
                                bool enabled = true, string description = null)
                       : base(intervalUnit, interval, startAt, endAt, enabled, description)
        {
            InitWeeklySchedule(launchDays);
        }

        private void InitWeeklySchedule(DayOfWeek[] launchDays)
        {
            days = ProcessArrayInput<DayOfWeek>(launchDays);
        }

        public override DateTime? GetNext(DateTime currentDate)
        {
            if (days.Count == 0) return null;

            if (ScheduleType == SCHEDULE_TYPE_ONCE)
            {
                bool alreadyFired = GetOccursOnceDateTime(currentDate) <= currentDate;
                return GetNextOnCondition(currentDate, !IsActiveDay(currentDate) || alreadyFired);
            }

            TimeSpan next = GetNextInterval(currentDate);
            return GetNextOnCondition(currentDate, !IsActiveDay(currentDate) || next > SpanEnd);
        }

        private bool IsActiveDay(DateTime currentDate)
        {
            if (days.Contains(currentDate.DayOfWeek))
                return true;

            return false;
        }

        private DateTime? GetNextOnCondition(DateTime currentDate, bool needNextDay)
        {
            if (needNextDay)
                return base.GetNext(GetNextDay(currentDate));
            else
                return base.GetNext(currentDate);
        }

        private DateTime GetNextDay(DateTime currentDate)
        {
            int idx = -1;

            for (int i = 0; i < days.Count; i++)
            {
                if (days[i] > currentDate.DayOfWeek)
                {
                    idx = (int)days[i];
                    break;
                }
            }

            if (idx == -1) idx = (int)days[0];

            int daysToAdd = (idx - (int)currentDate.DayOfWeek + 7) % 7;
            var tmp = currentDate.AddDays(daysToAdd);

            return new DateTime(tmp.Year, tmp.Month, tmp.Day, SpanStart.Hours, SpanStart.Minutes, SpanStart.Seconds);
        }
    }
}
