using System;
using Xunit;

namespace SimpleSchedules.Tests
{
    public class TestMonthlySchedule
    {
        [Fact]
        public void Once_Unreachable_Day()
        {
            // February 2020 has only 29 days

            var input = new DateTime(2020, 2, 16, 18, 30, 30);

            var expected = new DateTime(2020, 3, 7, 19, 40, 15);

            var sch = new MonthlySchedule(new int[] { 7, 31 }, new Time("19:40:15"));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Once_Inside_Active_Day_Before_Trigger()
        {
            var input = new DateTime(2020, 8, 16, 18, 30, 30);

            var expected = new DateTime(2020, 8, 16, 19, 40, 15);

            var sch = new MonthlySchedule(new int[] { 1, 3, 8, 16 }, new Time("19:40:15"));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Once_Inside_Active_Day_After_Trigger()
        {
            var input = new DateTime(2020, 8, 16, 19, 50, 30);

            var expected = new DateTime(2020, 9, 3, 19, 40, 15);

            var sch = new MonthlySchedule(new int[] { 16, 3, 8 }, new Time("19:40:15"));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Once_Outside_Active_Day()
        {
            var input = new DateTime(2020, 8, 16, 19, 50, 30);

            var expected = new DateTime(2020, 8, 21, 19, 40, 15);

            var sch = new MonthlySchedule(new int[] { 21 }, new Time("19:40:15"));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Recurring_Outside_Active_Day_Default_Active_Period()
        {
            var input = new DateTime(2020, 8, 16, 19, 50, 30);

            var expected = new DateTime(2020, 8, 21, 0, 0, 0);

            var sch = new MonthlySchedule(new int[] { 21 }, DailyIntervalUnit.Hour, 2, null, null);

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Recurring_Outside_Active_Day_Custom_Active_Period()
        {
            var input = new DateTime(2020, 8, 16, 19, 50, 30);

            var expected = new DateTime(2020, 8, 21, 2, 30, 15);

            var sch = new MonthlySchedule(new int[] { 21 }, DailyIntervalUnit.Hour, 2,
                            new Time(2, 30, 15), new Time(12, 30, 15));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Recurring_Inside_Active_Day_Default_Active_Period()
        {
            var input = new DateTime(2020, 8, 16, 9, 50, 30);

            var expected = new DateTime(2020, 8, 16, 10, 0, 0);

            var sch = new MonthlySchedule(new int[] { 16, 21 }, DailyIntervalUnit.Hour, 2, null, null);

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Recurring_Inside_Active_Day_Custom_Active_Period_Before()
        {
            var input = new DateTime(2020, 8, 16, 1, 50, 30);

            var expected = new DateTime(2020, 8, 16, 2, 30, 15);

            var sch = new MonthlySchedule(new int[] { 16, 21 }, DailyIntervalUnit.Hour, 2,
                            new Time(2, 30, 15), new Time(12, 30, 15));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Recurring_Inside_Active_Day_Custom_Active_Period_Inside()
        {
            var input = new DateTime(2020, 8, 16, 11, 50, 30);

            var expected = new DateTime(2020, 8, 16, 12, 30, 15);

            var sch = new MonthlySchedule(new int[] { 16, 21 }, DailyIntervalUnit.Hour, 2,
                            new Time(2, 30, 15), new Time(12, 30, 15));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Recurring_Inside_Active_Day_Custom_Active_Period_After()
        {
            var input = new DateTime(2020, 8, 16, 13, 50, 30);

            var expected = new DateTime(2020, 8, 21, 2, 30, 15);

            var sch = new MonthlySchedule(new int[] { 16, 21 }, DailyIntervalUnit.Hour, 2,
                            new Time(2, 30, 15), new Time(12, 30, 15));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Recurring_Unreachable_Day()
        {
            // February 2020 has only 29 days

            var input = new DateTime(2020, 2, 16, 18, 30, 30);

            var expected = new DateTime(2020, 3, 7, 2, 30, 15);

            var sch = new MonthlySchedule(new int[] { 7, 31 }, DailyIntervalUnit.Hour, 2,
                            new Time(2, 30, 15), new Time(12, 30, 15));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }
    }
}
