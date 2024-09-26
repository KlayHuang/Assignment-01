using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace assignment1_ConsoleJob.Services
{
    internal class FileService : IFileService
    {
        private readonly ILogger _logger;

        public FileService(ILogger logger)
        {
            _logger = logger;
        }

        public bool AreFilesEqual(string file1, string file2)
        {
            if (!File.Exists(file2)) return false;

            const int bufferSize = 1024 * 1024;
            using (var fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read))
            using (var fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read))
            {
                if (fs1.Length != fs2.Length) return false;

                byte[] buffer1 = new byte[bufferSize];
                byte[] buffer2 = new byte[bufferSize];

                int bytesRead1, bytesRead2;
                while ((bytesRead1 = fs1.Read(buffer1, 0, buffer1.Length)) > 0)
                {
                    bytesRead2 = fs2.Read(buffer2, 0, buffer2.Length);
                    if (bytesRead1 != bytesRead2 || !buffer1.SequenceEqual(buffer2))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void MoveFiles(string sourceDirectory, string destinationDirectory)
        {
            foreach (var file in Directory.GetFiles(sourceDirectory))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destinationDirectory, fileName);
                try
                {
                    if (AreFilesEqual(file, destFile))
                    {
                        _logger.Information($"來源檔案 {file} 與目的檔案相同");
                    }
                    else
                    {
                        File.Copy(file, destFile, true);
                        _logger.Information($"{fileName} 複製到 {destinationDirectory} ");
                    }
                    File.Delete(file);
                    _logger.Information($"來源檔案刪除: {fileName}");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"移動檔案 {fileName} 時發生錯誤");
                }
            }

            foreach (var subDirectory in Directory.GetDirectories(sourceDirectory))
            {
                string subDirectoryName = Path.GetFileName(subDirectory);
                string destSubDirectory = Path.Combine(destinationDirectory, subDirectoryName);

                if (!Directory.Exists(destSubDirectory))
                {
                    Directory.CreateDirectory(destSubDirectory);
                }

                MoveFiles(subDirectory, destSubDirectory);

                try
                {
                    Directory.Delete(subDirectory, true);
                    _logger.Information($"資料夾已刪除: {subDirectory}");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"刪除資料夾 {subDirectory} 時發生錯誤");
                }
            }
        }
    }
}
