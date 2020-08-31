using Microsoft.Extensions.Configuration;

namespace SimpleSchedules
{
    internal interface IConfigurationLoader
    {
        Schedule[] ReadFromConfiguration(IConfiguration configuration);
        Schedule[] ReadFromConfiguration(IConfiguration configuration, string section);
    }
}