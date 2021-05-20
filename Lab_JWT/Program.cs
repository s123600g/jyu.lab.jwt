using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace Lab_JWT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NLog.Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Debug("The app init main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // 確保當App結束時，NLog正常結束釋放掉
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                // 重新設置app Log配置
                .ConfigureLogging(logging =>
                {
                    // 清除app預設Log Provider
                    logging.ClearProviders();
                    // 設定Log輸出最小層級
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();  // NLog: Setup NLog for Dependency injection);
    }
}
