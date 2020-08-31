using Microsoft.Extensions.Configuration;
using Xunit;

namespace SimpleSchedules.Tests
{
    public class TestConfigurationLoader
    {
        private readonly IConfiguration config;
        private readonly SchedulesManager manager;

        public TestConfigurationLoader()
        {
            config = new ConfigurationBuilder().AddJsonFile("test.appsettings.json").Build();
            manager = new SchedulesManager();
        }

        [Fact]
        public void Load_Configuration_From_Default_Section()
        {
            int expected = 4;

            manager.ReadFromConfiguration(config);

            Assert.Equal(expected, manager.Schedules.Count);
        }

        [Fact]
        public void Load_Configuration_From_Custom_Section()
        {
            int expected = 1;

            manager.ReadFromConfiguration(config, "CustomSectionSch");

            Assert.Equal(expected, manager.Schedules.Count);
        }
    }
}
