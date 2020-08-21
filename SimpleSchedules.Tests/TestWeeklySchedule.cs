using System;
using Xunit;

namespace SimpleSchedules.Tests
{
    public class TestWeeklySchedule
    {
        [Fact]
        public void Once_Inside_Active_Day_Before_Trigger()
        {
            var input = new DateTime(2020, 8, 20, 18, 30, 30);

            var expected = new DateTime(2020, 8, 20, 19, 40, 15);

            var sch = new WeeklySchedule(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Thursday },
                            new Time("19:40:15"));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Once_Inside_Active_Day_After_Trigger()
        {
            var input = new DateTime(2020, 8, 20, 19, 50, 30);

            var expected = new DateTime(2020, 8, 23, 19, 40, 15);

            var sch = new WeeklySchedule(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Sunday },
                            new Time("19:40:15"));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Once_Inside_Last_Active_Day_After_Trigger()
        {
            var input = new DateTime(2020, 8, 20, 19, 50, 30);

            var expected = new DateTime(2020, 8, 24, 19, 40, 15);

            var sch = new WeeklySchedule(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Thursday },
                            new Time("19:40:15"));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Once_Outside_Active_Day()
        {
            var input = new DateTime(2020, 8, 20, 15, 50, 30);

            var expected = new DateTime(2020, 8, 23, 19, 40, 15);

            var sch = new WeeklySchedule(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Sunday },
                            new Time("19:40:15"));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Recurring_Outside_Active_Day_Default_Active_Period()
        {
            var input = new DateTime(2020, 8, 18, 19, 50, 30);

            var expected = new DateTime(2020, 8, 23, 0, 0, 0);

            var sch = new WeeklySchedule(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Sunday },
                         DailyIntervalUnit.Hour, 2, null, null);

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Recurring_Outside_Active_Day_Custom_Active_Period()
        {
            var input = new DateTime(2020, 8, 18, 19, 50, 30);

            var expected = new DateTime(2020, 8, 23, 2, 30, 15);

            var sch = new WeeklySchedule(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Sunday },
                         DailyIntervalUnit.Hour, 2,
                         new Time(2, 30, 15), new Time(12, 30, 15));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Recurring_Inside_Active_Day_Default_Active_Period()
        {
            var input = new DateTime(2020, 8, 17, 9, 50, 30);

            var expected = new DateTime(2020, 8, 17, 10, 0, 0);

            var sch = new WeeklySchedule(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Sunday },
                         DailyIntervalUnit.Hour, 2, null, null);

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Recurring_Inside_Active_Day_Custom_Active_Period_Before()
        {
            var input = new DateTime(2020, 8, 23, 1, 50, 30);

            var expected = new DateTime(2020, 8, 23, 2, 30, 15);

            var sch = new WeeklySchedule(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Sunday },
                         DailyIntervalUnit.Hour, 2,
                         new Time(2, 30, 15), new Time(17, 30, 15));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Recurring_Inside_Active_Day_Custom_Active_Period_Inside()
        {
            var input = new DateTime(2020, 8, 23, 11, 50, 30);

            var expected = new DateTime(2020, 8, 23, 12, 30, 15);

            var sch = new WeeklySchedule(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Sunday },
                         DailyIntervalUnit.Hour, 2,
                         new Time(2, 30, 15), new Time(17, 30, 15));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Recurring_Inside_Active_Day_Custom_Active_Period_After()
        {
            var input = new DateTime(2020, 8, 23, 17, 50, 30);

            var expected = new DateTime(2020, 8, 24, 2, 30, 15);

            var sch = new WeeklySchedule(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Sunday },
                         DailyIntervalUnit.Hour, 2,
                         new Time(2, 30, 15), new Time(17, 30, 15));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }
    }
}
