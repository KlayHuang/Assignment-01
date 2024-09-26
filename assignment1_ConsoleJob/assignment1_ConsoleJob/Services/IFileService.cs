using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1_ConsoleJob.Services
{
    public interface IFileService
    {
        bool AreFilesEqual(string file1, string file2);
        void MoveFiles(string sourceDirectory, string destinationDirectory);
    }
}
