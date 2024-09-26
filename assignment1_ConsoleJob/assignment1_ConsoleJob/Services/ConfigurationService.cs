using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace assignment1_ConsoleJob.Services
{
    internal class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetSourceDirectory()
        {
            return _configuration.GetValue<string>("SourceDirectory")!;
        }

        public string GetDestinationDirectory()
        {
            return _configuration.GetValue<string>("DestinationDirectory")!;
        }
    }
}
