using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleSchedules;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleWorkerService
{
    public class Worker : BackgroundService
    {
        // From this example:
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-3.1&tabs=visual-studio#timed-background-tasks-1

        private readonly ILogger<Worker> _logger;
        private readonly ISchedulesManager _manager;
        private readonly IConfiguration _config;

        public Worker(ILogger<Worker> logger, ISchedulesManager manager, IConfiguration config)
        {
            _logger = logger;
            _manager = manager;
            _config = config;

            // Set the event handler

            _manager.EventOccurred += Schedules_EventOccurred;

            // creates a schedule, which will fire event every day every 30 seconds from 07:00:00 till 23:00:00

            var daily1 = new DailySchedule(DailyIntervalUnit.Second, 30,
                                    new Time(7, 0, 0), new Time(23, 0, 0),
                                    enabled: true, description: "every day every 30 seconds");

            // creates a schedule, which will fire event every day every 2 minutes from 00:00:00 till 23:59:59,
            // schedule will be created disabled

            var daily2 = new DailySchedule(DailyIntervalUnit.Minute, 2, null, null,
                                    enabled: false, description: "every day every 2 minutes");

            // creates a schedule, which will fire event every day at 12:15:00

            var daily3 = new DailySchedule(new Time(12, 50, 0));

            // creates a schedule, which will fire event on every Monday and Wednesday,
            // every hour from 00:00:00 till 23:59:59

            var weekly1 = new WeeklySchedule(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Wednesday },
                                    DailyIntervalUnit.Hour, 1, null, null);

            // creates a schedule, which will fire event on 1, 2, 15 day of every month at 14:20:15

            var monthly1 = new MonthlySchedule(new int[] { 1, 2, 15 }, new Time("14:20:15"));

            _manager.AddSchedules(daily1, daily2, daily3, weekly1, monthly1);

            // Load from IConfiguration, section "SimpleSchedules" (see appsettings.json)

            _manager.LoadFrom(_config);
        }

        private void Schedules_EventOccurred(object sender, ScheduleEventArgs e)
        {
            /*
            Output will be something like that:

            Event occurred at 02.09.2020 12:49:00. Schedule properties: DailyIntervalUnit = Second, Interval = 30, Desc = every day every 30 seconds
            Event occurred at 02.09.2020 12:49:00. Schedule properties: DailyIntervalUnit = Minute, Interval = 1, Desc = minute schedule description
            Event occurred at 02.09.2020 12:49:30. Schedule properties: DailyIntervalUnit = Second, Interval = 30, Desc = every day every 30 seconds
            Event occurred at 02.09.2020 12:50:00. Schedule properties: DailyIntervalUnit = Second, Interval = 30, Desc = every day every 30 seconds
            Event occurred at 02.09.2020 12:50:00. Schedule properties: OccursOnceAt = 12:50:00
            Event occurred at 02.09.2020 12:50:00. Schedule properties: DailyIntervalUnit = Minute, Interval = 1, Desc = minute schedule description
            Event occurred at 02.09.2020 12:50:30. Schedule properties: DailyIntervalUnit = Second, Interval = 30, Desc = every day every 30 seconds
            */

            foreach (var schedule in e.OccurredSchedules)
            {
                string props = "";

                if (schedule is DailySchedule sch)
                {
                    if (sch.Type == ScheduleType.Recurring)
                        props = $"DailyIntervalUnit = {sch.IntervalUnit}, Interval = {sch.Interval}, Desc = {sch.Description}";
                    else
                        props = $"OccursOnceAt = {sch.OccursOnceAt.GetValueOrDefault().GetCurrentValue()}";
                }

                _logger.LogInformation($"Event occurred at {DateTime.Now}. Schedule properties: {props}");
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting timer");
            return _manager.StartAsync();
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping timer");
            return _manager.StopAsync();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _manager.Dispose();
        }
    }
}
