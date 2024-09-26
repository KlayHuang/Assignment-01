using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1_ConsoleJob.Services
{
    public interface IConfigurationService
    {
        string GetSourceDirectory();
        string GetDestinationDirectory();
    }
}
