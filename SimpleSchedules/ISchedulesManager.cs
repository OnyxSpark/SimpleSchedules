using System.Threading.Tasks;

namespace SimpleSchedules
{
    public delegate void ScheduleEventHandler(object sender, ScheduleEventArgs e);

    public interface ISchedulesManager
    {
        /// <summary>
        /// Add schedule to the list. This schedule's events will fire on the next timer tick (1 sec)
        /// </summary>
        /// <param name="schedule">Object of one of the childs (DailySchedule, etc)</param>
        void AddSchedule(Schedule schedule);

        /// <summary>
        /// Remove schedule from the list. This schedule's events will not fire any more
        /// </summary>
        /// <param name="schedule">Object of one of the childs (DailySchedule, etc)</param>
        void RemoveSchedule(Schedule schedule);

        /// <summary>
        /// Start the timer
        /// </summary>
        void Start();

        /// <summary>
        /// Start the timer. Intended to run in StartAsync() of the HostedService
        /// </summary>
        /// <returns></returns>
        Task StartAsync();

        /// <summary>
        /// Stop the timer
        /// </summary>
        void Stop();

        /// <summary>
        /// Stop the timer. Intended to run in StopAsync() of the HostedService
        /// </summary>
        /// <returns></returns>
        Task StopAsync();

        /// <summary>
        /// Disposes internal timer
        /// </summary>
        void Dispose();

        /// <summary>
        /// Event, that will fire on current schedules
        /// </summary>
        event ScheduleEventHandler EventOccurred;
    }
}
