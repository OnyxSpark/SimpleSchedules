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

            // creates a schedule, which will fire every day every 30 seconds from 07:00:00 till 23:00:00

            var secSchedule = new DailySchedule(DailyIntervalUnit.Second, 30,
                                    new Time(7, 0, 0), new Time(23, 0, 0), false);

            // creates a schedule, which will fire every day every 2 minutes from 00:00:00 till 23:59:59

            var minSchedule = new DailySchedule(DailyIntervalUnit.Minute, 2, null, null, false);

            _manager.AddSchedules(secSchedule, minSchedule);

            // Load from IConfiguration

            _manager.ReadFromConfiguration(_config);
        }

        private void Schedules_EventOccurred(object sender, ScheduleEventArgs e)
        {
            /*
            Output will something like that:

            Event occurred at 11.08.2020 16:42:00. Schedule properties: DailyIntervalUnit = Second, Interval = 30
            Event occurred at 11.08.2020 16:42:00. Schedule properties: DailyIntervalUnit = Minute, Interval = 2
            Event occurred at 11.08.2020 16:42:30. Schedule properties: DailyIntervalUnit = Second, Interval = 30
            Event occurred at 11.08.2020 16:43:00. Schedule properties: DailyIntervalUnit = Second, Interval = 30
            Event occurred at 11.08.2020 16:43:30. Schedule properties: DailyIntervalUnit = Second, Interval = 30
            Event occurred at 11.08.2020 16:44:00. Schedule properties: DailyIntervalUnit = Second, Interval = 30
            Event occurred at 11.08.2020 16:44:00. Schedule properties: DailyIntervalUnit = Minute, Interval = 2
            Event occurred at 11.08.2020 16:44:30. Schedule properties: DailyIntervalUnit = Second, Interval = 30
            Event occurred at 11.08.2020 16:45:00. Schedule properties: DailyIntervalUnit = Second, Interval = 30
            */

            foreach (var schedule in e.OccurredSchedules)
            {
                string props = "";
                var sch = schedule as DailySchedule;

                if (sch != null)
                    props = $"DailyIntervalUnit = {sch.IntervalUnit}, Interval = {sch.Interval}, Desc = {sch.Description}";

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
