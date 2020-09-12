using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Xunit;

namespace SimpleSchedules.Tests
{
    class TestJson
    {
        public List<ScheduleConfig> SimpleSchedules { get; set; }
        public List<ScheduleConfig> CustomSectionSch { get; set; }
    }

    public class TestConfigurationLoader
    {
        private readonly IConfiguration config;
        private readonly SchedulesManager manager;
        private readonly string json;

        public TestConfigurationLoader()
        {
            config = new ConfigurationBuilder().AddJsonFile("test.appsettings.json").Build();
            json = File.ReadAllText("test.appsettings.json");
            manager = new SchedulesManager();
        }

        [Fact]
        public void Load_Configuration_From_Default_Section()
        {
            int expected = 4;

            manager.LoadFrom(config);

            Assert.Equal(expected, manager.Schedules.Count);
        }

        [Fact]
        public void Load_Configuration_From_Custom_Section()
        {
            int expected = 1;

            manager.LoadFrom(config, "CustomSectionSch");

            Assert.Equal(expected, manager.Schedules.Count);
        }

        [Fact]
        public void Load_Configuration_From_Json()
        {
            int expected = 4;

            var testJson = JsonSerializer.Deserialize<TestJson>(json);

            manager.LoadFrom(testJson.SimpleSchedules);

            Assert.Equal(expected, manager.Schedules.Count);
        }
    }
}
