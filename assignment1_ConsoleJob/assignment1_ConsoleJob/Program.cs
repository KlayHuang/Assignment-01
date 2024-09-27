using assignment1_ConsoleJob.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Collections;
using System.Security.Cryptography;

namespace assignment1_ConsoleJob
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger>();
            var fileService = serviceProvider.GetRequiredService<IFileService>();
            var configService = serviceProvider.GetRequiredService<IConfigurationService>();

            try
            {
                string sourceDirectory = configService.GetSourceDirectory();
                string destinationDirectory = configService.GetDestinationDirectory();
                
                if (string.IsNullOrEmpty(sourceDirectory))
                {
                    do
                    {
                        logger.Error($"來源資料夾不存在!! 請輸入 [來源資料夾] 路徑:");
                        sourceDirectory = Console.ReadLine()!;
                    } 
                    while (!Directory.Exists(sourceDirectory));
                }

                if (string.IsNullOrEmpty(destinationDirectory))
                {
                    do
                    {
                        logger.Error($"目的資料夾不存在!! 請輸入 [目的資料夾] 路徑:");
                        destinationDirectory = Console.ReadLine()!;
                    } while (!Directory.Exists(destinationDirectory));
                }

                if (IsSubDirectory(sourceDirectory, destinationDirectory))
                {
                    logger.Error("路徑異常: 目的資料夾是來源資料夾的子資料夾，終止程序!!");
                    return;
                }
                logger.Information($"來源資料夾: {sourceDirectory}, 目的資料夾: {destinationDirectory}");

                fileService.MoveFiles(sourceDirectory, destinationDirectory);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "程式發生錯誤!!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .AddYamlFile("Configurations/SerilogConfig.yml")
                .AddYamlFile("Configurations/SysConfig.yml")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            services.AddSingleton<ILogger>(Log.Logger);
            services.AddSingleton<IConfiguration>(config);
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddSingleton<IFileService, FileService>();
        }

        private static bool IsSubDirectory(string parentDirectory, string childDirectory)
        {
            string parentPath = Path.GetFullPath(parentDirectory);
            string childPath = Path.GetFullPath(childDirectory);

            return childPath.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase);
        }


    }
}
