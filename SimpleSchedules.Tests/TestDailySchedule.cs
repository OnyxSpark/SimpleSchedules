using System;
using Xunit;

namespace SimpleSchedules.Tests
{
    public class TestDailySchedule
    {
        [Fact]
        public void Ctor_Params_Start_At_Exception()
        {
            Assert.Throws<ArgumentException>(() => new DailySchedule(DailyIntervalUnit.Hour, 1,
                          new Time(20, 15, 30), new Time(10, 59, 59), false));
        }

        [Fact]
        public void Ctor_Params_Interval_Exception()
        {
            Assert.Throws<ArgumentException>(() => new DailySchedule(DailyIntervalUnit.Hour, 0,
                          new Time(10, 15, 30), new Time(20, 59, 59), false));
        }

        [Fact]
        public void GetNext_When_Disabled()
        {
            var input = new DateTime(2020, 8, 10, 7, 10, 10);

            var sch = new DailySchedule(DailyIntervalUnit.Hour, 1,
                         new Time(10, 15, 30), new Time(20, 59, 59), false);

            var actual = sch.GetNext(input);

            Assert.Null(actual);
        }

        [Fact]
        public void Once_GetNext_Before_Trigger()
        {
            var trigger = new Time(12, 13, 14);

            var input = new DateTime(2020, 8, 10, 7, 10, 10);

            var expected = new DateTime(2020, 8, 10, 12, 13, 14);

            var sch = new DailySchedule(trigger);

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Once_GetNext_On_Trigger()
        {
            var trigger = new Time(12, 13, 14);

            var input = new DateTime(2020, 8, 10, 12, 13, 14);

            var expected = new DateTime(2020, 8, 11, 12, 13, 14);

            var sch = new DailySchedule(trigger);

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Once_GetNext_After_Trigger()
        {
            var trigger = new Time(12, 13, 14);

            var input = new DateTime(2020, 8, 10, 17, 10, 10);

            var expected = new DateTime(2020, 8, 11, 12, 13, 14);

            var sch = new DailySchedule(trigger);

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Hour_GetNext_Before_Active_Interval()
        {
            var input = new DateTime(2020, 8, 10, 7, 10, 10);

            var expected = new DateTime(2020, 8, 10, 10, 15, 30);

            var sch = new DailySchedule(DailyIntervalUnit.Hour, 1,
                         new Time(10, 15, 30), new Time(20, 59, 59));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Hour_GetNext_OnStart_Of_Active_Interval()
        {
            var input = new DateTime(2020, 8, 10, 12, 10, 10);

            var expected = new DateTime(2020, 8, 10, 12, 10, 10);

            var sch = new DailySchedule(DailyIntervalUnit.Hour, 1,
                         new Time(12, 10, 10), new Time(23, 59, 59));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Hour_GetNext_Inside_Of_Active_Interval()
        {
            var input = new DateTime(2020, 8, 10, 14, 31, 40);

            var expected = new DateTime(2020, 8, 10, 15, 20, 30);

            var sch = new DailySchedule(DailyIntervalUnit.Hour, 1,
                         new Time(10, 20, 30), new Time(23, 59, 59));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Hour_GetNext_Inside_Of_Active_Interval_Default_Start_End()
        {
            var input = new DateTime(2020, 8, 10, 14, 31, 40);

            var expected = new DateTime(2020, 8, 10, 16, 0, 0);

            var sch = new DailySchedule(DailyIntervalUnit.Hour, 4, null, null);

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Hour_GetNext_On_End_Of_Active_Interval()
        {
            var input = new DateTime(2020, 8, 10, 21, 0, 0);

            var expected = new DateTime(2020, 8, 11, 12, 2, 3);

            var sch = new DailySchedule(DailyIntervalUnit.Hour, 1,
                         new Time(12, 2, 3), new Time(21, 0, 0));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Hour_GetNext_After_End_Of_Active_Interval()
        {
            var input = new DateTime(2020, 8, 10, 22, 0, 0);

            var expected = new DateTime(2020, 8, 11, 12, 2, 3);

            var sch = new DailySchedule(DailyIntervalUnit.Hour, 1,
                         new Time(12, 2, 3), new Time(21, 0, 0));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Minute_GetNext_Before_Active_Interval()
        {
            var input = new DateTime(2020, 8, 10, 7, 10, 10);

            var expected = new DateTime(2020, 8, 10, 10, 10, 15);

            var sch = new DailySchedule(DailyIntervalUnit.Minute, 2,
                         new Time(10, 10, 15), new Time(20, 59, 59));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Minute_GetNext_On_Start_Of_Active_Interval()
        {
            var input = new DateTime(2020, 8, 10, 7, 30, 15);

            var expected = new DateTime(2020, 8, 10, 7, 30, 15);

            var sch = new DailySchedule(DailyIntervalUnit.Minute, 2,
                         new Time(7, 30, 15), new Time(23, 59, 59));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Minute_GetNext_Inside_Of_Active_Interval()
        {
            var input = new DateTime(2020, 8, 10, 14, 31, 40);

            var expected = new DateTime(2020, 8, 10, 14, 33, 12);

            var sch = new DailySchedule(DailyIntervalUnit.Minute, 2,
                         new Time(6, 13, 12), new Time(23, 59, 59));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Minute_GetNext_Inside_Of_Active_Interval_Default_Start_End()
        {
            var input = new DateTime(2020, 8, 10, 14, 31, 40);

            var expected = new DateTime(2020, 8, 10, 14, 32, 0);

            var sch = new DailySchedule(DailyIntervalUnit.Minute, 2, null, null);

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Minute_GetNext_On_End_Of_Active_Interval()
        {
            var input = new DateTime(2020, 8, 10, 21, 12, 13);

            var expected = new DateTime(2020, 8, 11, 2, 7, 4);

            var sch = new DailySchedule(DailyIntervalUnit.Minute, 2,
                         new Time(2, 7, 4), new Time(21, 12, 13));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Minute_GetNext_After_End_Of_Active_Interval()
        {
            var input = new DateTime(2020, 8, 10, 22, 12, 12);

            var expected = new DateTime(2020, 8, 11, 2, 3, 4);

            var sch = new DailySchedule(DailyIntervalUnit.Minute, 2,
                         new Time(2, 3, 4), new Time(21, 21, 21));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Second_GetNext_Before_Active_Interval()
        {
            var input = new DateTime(2020, 8, 10, 7, 10, 10);

            var expected = new DateTime(2020, 8, 10, 10, 10, 15);

            var sch = new DailySchedule(DailyIntervalUnit.Second, 3,
                         new Time(10, 10, 15), new Time(20, 59, 59));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Second_GetNext_On_Start_Of_Active_Interval()
        {
            var input = new DateTime(2020, 8, 10, 7, 30, 15);

            var expected = new DateTime(2020, 8, 10, 7, 30, 15);

            var sch = new DailySchedule(DailyIntervalUnit.Second, 3,
                         new Time(7, 30, 15), new Time(23, 59, 59));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Second_GetNext_Inside_Of_Active_Interval_1()
        {
            var input = new DateTime(2020, 8, 10, 14, 31, 40);

            var expected = new DateTime(2020, 8, 10, 14, 31, 42);

            var sch = new DailySchedule(DailyIntervalUnit.Second, 3,
                         new Time(6, 13, 12), new Time(23, 59, 59));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Second_GetNext_Inside_Of_Active_Interval_2()
        {
            var input = new DateTime(2020, 8, 10, 14, 31, 51);

            var expected = new DateTime(2020, 8, 10, 14, 31, 55);

            var sch = new DailySchedule(DailyIntervalUnit.Second, 5,
                         new Time(6, 0, 0), new Time(23, 59, 59));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Second_GetNext_Inside_Of_Active_Interval_Default_Start_End()
        {
            var input = new DateTime(2020, 8, 10, 14, 31, 40);

            var expected = new DateTime(2020, 8, 10, 14, 31, 42);

            var sch = new DailySchedule(DailyIntervalUnit.Second, 3, null, null);

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Second_GetNext_Inside_Of_Active_Interval_1sec()
        {
            var input = new DateTime(2020, 8, 10, 14, 31, 59);

            var expected = new DateTime(2020, 8, 10, 14, 32, 0);

            var sch = new DailySchedule(DailyIntervalUnit.Second, 1,
                         new Time(6, 0, 0), new Time(23, 59, 59));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Second_GetNext_On_End_Of_Active_Interval()
        {
            var input = new DateTime(2020, 8, 10, 21, 12, 13);

            var expected = new DateTime(2020, 8, 11, 2, 7, 4);

            var sch = new DailySchedule(DailyIntervalUnit.Second, 3,
                         new Time(2, 7, 4), new Time(21, 12, 13));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Second_GetNext_After_End_Of_Active_Interval()
        {
            var input = new DateTime(2020, 8, 10, 22, 12, 12);

            var expected = new DateTime(2020, 8, 11, 2, 3, 4);

            var sch = new DailySchedule(DailyIntervalUnit.Second, 3,
                         new Time(2, 3, 4), new Time(21, 21, 21));

            var actual = sch.GetNext(input);

            Assert.Equal(expected, actual);
        }
    }
}
