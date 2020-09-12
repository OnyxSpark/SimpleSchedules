# SimpleSchedules

.NET Core library, which supports event triggering, based on several schedules.

### Description

The library is made to work in .NET Core Worker Services, where it is necessary to start some work according to one or several schedules. An example worker service with the same code, which shown below, is in the `ExampleWorkerService` project.

### How to use

Schedules can be set both in code and via `IConfiguration`.

Examples of creating a schedules in code:

```csharp
// creates a schedule, which will fire event every day every 30 seconds from 07:00:00 till 23:00:00

var daily1 = new DailySchedule(DailyIntervalUnit.Second, 30,
                       new Time(7, 0, 0), new Time(23, 0, 0),
                       enabled: true, description: "every day every 30 seconds");

// creates a schedule, which will fire event every day every 2 minutes from 00:00:00 till 23:59:59,
// schedule will be created disabled

var daily2 = new DailySchedule(DailyIntervalUnit.Minute, 2, null, null,
                      enabled: false, description: "every day every 2 minutes");

// creates a schedule, which will fire event every day at 12:50:00

var daily3 = new DailySchedule(new Time(12, 50, 0));

// creates a schedule, which will fire event on every Monday and Wednesday,
// every hour from 00:00:00 till 23:59:59

var weekly1 = new WeeklySchedule(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Wednesday },
                      DailyIntervalUnit.Hour, 1, null, null);

// creates a schedule, which will fire event on 1, 2, 15 day of every month at 14:20:15

var monthly1 = new MonthlySchedule(new int[] { 1, 2, 15 }, new Time("14:20:15"));
```

Example of creation via `IConfiguration`:

```json
{
  "SimpleSchedules": [
    {
      "Type": "SimpleSchedules.WeeklySchedule",
      "DaysOfWeek": [ "Monday", "Thursday", "saturday" ],
      "IntervalUnit": "Second",
      "Interval": 3,
      "StartAt": "12:00:12",
      "EndAt": "21:05:22"
    },
    {
      "Type": "SimpleSchedules.MonthlySchedule",
      "LaunchDays": [ 1, 5, 15 ],
      "IntervalUnit": "Second",
      "Interval": 3,
      "StartAt": "12:00:12",
      "EndAt": "21:05:22"
    },
    {
      "Type": "SimpleSchedules.DailySchedule",
      "IntervalUnit": "Minute",
      "Interval": 1,
      "Enabled": false,
      "Description": "minute schedule description"
    },
    {
      "Type": "SimpleSchedules.DailySchedule",
      "OccursOnceAt": "12:55",
      "Description": "OneTime schedule description"
    }
  ]
}
```

After the schedules have been created, you need to place them in the `SchedulesManager`:

```csharp
ISchedulesManager manager = new SchedulesManager();

// add schedules, created in code
manager.AddSchedules(daily1, daily2, daily3, weekly1, monthly1);

// add schedules, created via IConfiguration:
manager.ReadFromConfiguration(config);

// start event triggering
manager.Start();
```

`SchedulesManager` must also have an `EventOccurred` event handler configured:

```csharp
manager.EventOccurred += Schedules_EventOccurred;

private void Schedules_EventOccurred(object sender, ScheduleEventArgs e)
{
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
```

After launch, the schedule manager generates events at the time points, specified in these schedules.

Example of triggering an events:

```
Event occurred at 02.09.2020 12:49:00. Schedule properties: DailyIntervalUnit = Second, Interval = 30, Desc = every day every 30 seconds
Event occurred at 02.09.2020 12:49:00. Schedule properties: DailyIntervalUnit = Minute, Interval = 1, Desc = minute schedule description
Event occurred at 02.09.2020 12:49:30. Schedule properties: DailyIntervalUnit = Second, Interval = 30, Desc = every day every 30 seconds
Event occurred at 02.09.2020 12:50:00. Schedule properties: DailyIntervalUnit = Second, Interval = 30, Desc = every day every 30 seconds
Event occurred at 02.09.2020 12:50:00. Schedule properties: OccursOnceAt = 12:50:00
Event occurred at 02.09.2020 12:50:00. Schedule properties: DailyIntervalUnit = Minute, Interval = 1, Desc = minute schedule description
Event occurred at 02.09.2020 12:50:30. Schedule properties: DailyIntervalUnit = Second, Interval = 30, Desc = every day every 30 seconds
```

An example worker service with the same code, which shown here, is in the `ExampleWorkerService` project.
