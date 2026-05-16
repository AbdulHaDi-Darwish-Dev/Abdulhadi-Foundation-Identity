using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Logging.Serilog.Configuration;

public static class SerilogConfiguration
{
    // نقوم بتعديل الدالة لتقبل الـ LoggerConfiguration وتعديله مباشرة
    public static LoggerConfiguration Configure(LoggerConfiguration lc, IConfiguration config)
    {
        return lc
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
            .MinimumLevel.Override("Medallion", LogEventLevel.Error)
            .MinimumLevel.Override("LuckyPennySoftware", LogEventLevel.Error)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.Seq(config["Seq:Url"] ?? "http://localhost:5341");
    }
}