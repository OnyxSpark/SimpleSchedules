using System;

namespace SimpleSchedules
{
    public class DailySchedule : Schedule
    {
        /// <summary>
        /// Specifies that the event will only occur once a day at the specified time
        /// </summary>
        public Time? OccursOnceAt { get; }

        /// <summary>
        /// Specifies the time interval for the event to occur
        /// </summary>
        public int Interval { get; }

        /// <summary>
        /// Type of interval: hours, minutes, seconds
        /// </summary>
        public DailyIntervalUnit IntervalUnit { get; }

        /// <summary>
        /// Starting point of active period, within which event will fire. If not set, it will suppose begin of the day (00:00:00)
        /// </summary>
        public Time? StartAt { get; }

        /// <summary>
        /// Ending point of active period, within which event will fire. If not set, it will suppose end of the day (23:59:59)
        /// </summary>
        public Time? EndAt { get; }

        protected const int SCHEDULE_TYPE_ONCE = 0;
        protected const int SCHEDULE_TYPE_RECURRING = 1;

        protected readonly int ScheduleType;
        protected readonly TimeSpan SpanStart;
        protected readonly TimeSpan SpanEnd;

        protected DailySchedule() { }

        /// <summary>
        /// Sets a schedule, that will only fire event once a day at the specified time
        /// </summary>
        /// <param name="occursOnceAt">A time, at which event will fire</param>
        /// <param name="enabled">Flag, indicating that schedule is enabled. Disabled schedules wil not fire events</param>
        /// <param name="description">Optional description of a schedule</param>
        public DailySchedule(Time occursOnceAt, bool enabled = true, string description = null)
        {
            Init(enabled, description);
            OccursOnceAt = occursOnceAt;
            ScheduleType = SCHEDULE_TYPE_ONCE;
        }

        /// <summary>
        /// Sets a schedule, that will fire event through the specified interval within active period
        /// </summary>
        /// <param name="intervalUnit">Unit of the interval</param>
        /// <param name="interval">Specifies the time interval for the event to occur</param>
        /// <param name="startAt">Starting point of active period, within which event will fire. If null, it will assume begin of the day (00:00:00)</param>
        /// <param name="endAt">Ending point of active period, within which event will fire. If null, it will assume end of the day (23:59:59)</param>
        /// <param name="enabled">Flag, indicating that schedule is enabled. Disabled schedules wil not fire events</param>
        /// <param name="description">Optional description of a schedule</param>
        public DailySchedule(DailyIntervalUnit intervalUnit, int interval, Time? startAt, Time? endAt,
                                bool enabled = true, string description = null)
        {
            if (startAt.HasValue && endAt.HasValue)
            {
                if (startAt.Value >= endAt.Value)
                    throw new ArgumentException("Argument endAt must be greater than argument startAt.");
            }

            if (interval <= 0)
            {
                throw new ArgumentException("Argument interval must be greater than 0.");
            }

            Init(enabled, description);

            Interval = interval;
            IntervalUnit = intervalUnit;
            StartAt = startAt;
            EndAt = endAt;
            ScheduleType = SCHEDULE_TYPE_RECURRING;

            SpanStart = GetTimeSpan(startAt, 0, 0, 0);
            SpanEnd = GetTimeSpan(endAt, 23, 59, 59);
        }

        /// <summary>
        /// Returns next DateTime when this schedule need to fire event, relative to currentDate
        /// </summary>
        /// <param name="currentDate">Relative this value next date will return</param>
        /// <returns>DateTime when next event need to fire or null if it should not</returns>
        public override DateTime? GetNext(DateTime currentDate)
        {
            if (!Enabled)
                return null;

            if (ScheduleType == SCHEDULE_TYPE_ONCE)
            {
                var occurs = GetOccursOnceDateTime(currentDate);

                if (currentDate < occurs)
                    return occurs;
                else
                    return occurs.AddDays(1);
            }

            // for SCHEDULE_TYPE_RECURRING

            TimeSpan next = GetNextInterval(currentDate);

            if (currentDate.TimeOfDay <= SpanStart)
                return new DateTime(currentDate.Year, currentDate.Month, currentDate.Day,
                    SpanStart.Hours, SpanStart.Minutes, SpanStart.Seconds);

            if (currentDate.TimeOfDay >= SpanEnd || next >= SpanEnd)
            {
                var tmp = currentDate.AddDays(1);
                return new DateTime(tmp.Year, tmp.Month, tmp.Day,
                    SpanStart.Hours, SpanStart.Minutes, SpanStart.Seconds);
            }

            return new DateTime(currentDate.Year, currentDate.Month, currentDate.Day,
                next.Hours, next.Minutes, next.Seconds);
        }

        protected DateTime GetOccursOnceDateTime(DateTime currentDate)
        {
            return currentDate.Date + OccursOnceAt.Value.GetCurrentValue();
        }

        private TimeSpan GetTimeSpan(Time? time, int defaultHour, int defaultMinute, int defaultSecond)
        {
            if (!time.HasValue)
                return new TimeSpan(defaultHour, defaultMinute, defaultSecond);
            else
                return time.Value.GetCurrentValue();
        }

        protected int CalcFinishedIntervals(double timeUnits, int interval)
        {
            // show how many full intervals passed since SpanStart
            // for example, if from SpanStart passed 10,23333 minutes, this method will return 10

            int res = (int)(timeUnits / interval);
            return res * interval;
        }

        protected TimeSpan GetNextInterval(DateTime currentDate)
        {
            currentDate = currentDate.AddTicks(-(currentDate.Ticks % TimeSpan.TicksPerSecond));
            var fromStart = currentDate.TimeOfDay - SpanStart;

            TimeSpan nextTime = default;

            switch (IntervalUnit)
            {
                case DailyIntervalUnit.Hour:
                    nextTime = new TimeSpan(CalcFinishedIntervals(fromStart.TotalHours, Interval) + Interval, 0, 0);
                    break;

                case DailyIntervalUnit.Minute:
                    nextTime = new TimeSpan(0, CalcFinishedIntervals(fromStart.TotalMinutes, Interval) + Interval, 0);
                    break;

                case DailyIntervalUnit.Second:
                    nextTime = new TimeSpan(0, 0, CalcFinishedIntervals(fromStart.TotalSeconds, Interval) + Interval);
                    break;
            }

            return SpanStart.Add(nextTime);
        }
    }
}
