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
                logger.Information($"來源資料夾: {sourceDirectory}, 目的資料夾: {destinationDirectory}");

                if (!Directory.Exists(sourceDirectory))
                {
                    logger.Error($"來源資料夾不存在!!");
                    return;
                }

                if (IsSubDirectory(sourceDirectory, destinationDirectory))
                {
                    logger.Error("路徑異常: 目的資料夾是來源資料夾的子資料夾!");
                    return;
                }

                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                fileService.MoveFiles(sourceDirectory, destinationDirectory);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "移動檔案時發生錯誤");
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
