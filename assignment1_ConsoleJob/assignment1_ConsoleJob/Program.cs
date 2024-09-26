using Microsoft.Extensions.Configuration;
using Serilog;

namespace assignment1_ConsoleJob
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .AddYamlFile("SerilogConfig.yml")
                    .AddYamlFile("SysConfig.yml")
                    .Build();

                Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

                string sourceDirectory = config["SourceDirectory"]!;
                string destinationDirectory = config["DestinationDirectory"]!;
                Log.Information($"來源資料夾: {sourceDirectory}, 目的資料夾: {destinationDirectory}");

                if (!Directory.Exists(sourceDirectory))
                {
                    Log.Error($"來源資料夾不存在!!");
                    return;
                }

                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                MoveFiles(sourceDirectory, destinationDirectory);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "移動檔案時發生錯誤");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        static void MoveFiles(string sourceDirectory, string destinationDirectory)
        {
            foreach (var file in Directory.GetFiles(sourceDirectory))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destinationDirectory, fileName);

                File.Copy(file, destFile, true);
                Log.Information($"{fileName} 複製到 {destinationDirectory} ");

                File.Delete(file);
                Log.Information($"來源檔案刪除: {fileName}");
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

                Directory.Delete(subDirectory, true);
                Log.Information($"資料夾刪除: {subDirectory}");
            }
        }


    }
}
