using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleSchedules
{
    public class SchedulesManager : ISchedulesManager, IDisposable
    {
        private List<Schedule> schedules = new List<Schedule>();
        private Timer timer;
        private Dictionary<Schedule, DateTime> nextDates;
        private bool disposed = false;

        private ConfigurationLoader configLoader = new ConfigurationLoader();

        /// <summary>
        /// Event, that will fire on current schedules
        /// </summary>
        public event ScheduleEventHandler EventOccurred;

        /// <summary>
        /// Creates Manager, which holds list of schedules and fires events according them
        /// </summary>
        public SchedulesManager()
        {
            timer = new Timer(new TimerCallback(TimerTick), null, Timeout.Infinite, Timeout.Infinite);
            nextDates = new Dictionary<Schedule, DateTime>();
        }

        /// <summary>
        /// Reads array of schedules from IConfiguration, using default section name "SimpleSchedules"
        /// </summary>
        /// <param name="configuration">Standard .Net Core IConfiguration object</param>
        public void ReadFromConfiguration(IConfiguration configuration)
        {
            var schedules = configLoader.ReadFromConfiguration(configuration);
            AddSchedules(schedules);
        }

        /// <summary>
        /// Reads array of schedules from IConfiguration, using custom section name
        /// </summary>
        /// <param name="configuration">Standard .Net Core IConfiguration object</param>
        /// <param name="section">Custom section name</param>
        public void ReadFromConfiguration(IConfiguration configuration, string section)
        {
            var schedules = configLoader.ReadFromConfiguration(configuration, section);
            AddSchedules(schedules);
        }

        private void TimerTick(object state)
        {
            if (nextDates == null || nextDates.Count == 0)
                return;

            var now = DateTime.Now;

            // minimal resolution interval of this object is 1 second, so we do not need milliseconds
            // best way to truncate them:
            // https://stackoverflow.com/questions/1004698/how-to-truncate-milliseconds-off-of-a-net-datetime

            now = now.AddTicks(-(now.Ticks % TimeSpan.TicksPerSecond));

            if (nextDates.ContainsValue(now))
            {
                var eventArgs = new List<Schedule>();

                PopulateEventArgs(eventArgs, now);

                RefreshDates(eventArgs);

                EventOccurred?.Invoke(this, new ScheduleEventArgs(eventArgs.ToArray()));
            }
        }

        private void PopulateEventArgs(List<Schedule> eventArgs, DateTime now)
        {
            // determine which schedules are firing now

            foreach (var kvp in nextDates)
            {
                if (kvp.Value == now)
                {
                    eventArgs.Add(kvp.Key);     // kvp.Key <- fired schedule
                }
            }
        }

        private void RefreshDates(List<Schedule> eventArgs)
        {
            if (eventArgs.Count > 0)
            {
                foreach (var sch in eventArgs)
                {
                    RemoveDateFromDict(sch);
                    AddDateToDict(sch);
                }
            }
        }

        private void AddDateToDict(Schedule schedule)
        {
            var next = schedule.GetNext(DateTime.Now);
            if (next.HasValue)
                nextDates.Add(schedule, next.Value);
        }

        private void RemoveDateFromDict(Schedule schedule)
        {
            if (nextDates.ContainsKey(schedule))
                nextDates.Remove(schedule);
        }

        private void CheckSchedule(Schedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException("schedule", "schedule is null");
        }

        /// <summary>
        /// Add schedule to the list. This schedule's events will fire on the next timer tick (1 sec)
        /// </summary>
        /// <param name="schedule">Object of types: DailySchedule, WeeklySchedule, MonthlySchedule</param>
        public void AddSchedule(Schedule schedule)
        {
            CheckSchedule(schedule);
            schedules.Add(schedule);
            AddDateToDict(schedule);
        }

        /// <summary>
        /// Add several schedules to the list. These schedules events will fire on the next timer tick (1 sec)
        /// </summary>
        /// <param name="schedules">Several objects of types: DailySchedule, WeeklySchedule, MonthlySchedule</param>
        public void AddSchedules(params Schedule[] schedules)
        {
            foreach (var schedule in schedules)
                AddSchedule(schedule);
        }

        /// <summary>
        /// Remove schedule from the list. This schedule's events will not fire any more
        /// </summary>
        /// <param name="schedule">Object of one of the childs (DailySchedule, etc)</param>
        public void RemoveSchedule(Schedule schedule)
        {
            CheckSchedule(schedule);
            RemoveDateFromDict(schedule);
            schedules.Remove(schedule);
        }

        /// <summary>
        /// Start the timer
        /// </summary>
        public void Start()
        {
            timer.Change(0, 1000);
        }

        /// <summary>
        /// Start the timer. Intended to run in StartAsync() of the HostedService
        /// </summary>
        /// <returns></returns>
        public Task StartAsync()
        {
            Start();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stop the timer
        /// </summary>
        public void Stop()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Stop the timer. Intended to run in StopAsync() of the HostedService
        /// </summary>
        /// <returns></returns>
        public Task StopAsync()
        {
            Stop();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Disposes internal timer
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                Stop();
                timer?.Dispose();
                disposed = true;
            }
        }
    }
}
