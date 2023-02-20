using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Samples.Extensions;

namespace Samples.Service.Stream;

internal static class Program
{
    internal static async Task Main()
    {
        var urls = BuildConfiguration.GetValue("SamplesStream:httphosturl", $"http://*:{8082}");

        var port = urls.LastRightPart(':').ToInt(8082);

        var builder = new HostBuilder().UseContentRoot(Directory.GetCurrentDirectory())
                                       .ConfigureHostConfiguration(b => b.AddConfiguration(BuildConfiguration))
                                       .ConfigureAppConfiguration((_, conf) => conf.AddConfiguration(BuildConfiguration))
                                       .ConfigureLogging((x, b) => b.AddConfiguration(x.Configuration.GetSection("Logging"))
                                                                    .AddSimpleConsole(o =>
                                                                                      {
                                                                                          o.SingleLine = true;
                                                                                          o.TimestampFormat = "HH:mm:ss.fff ";
                                                                                      }))
                                       .ConfigureWebHost(whb => whb.UseShutdownTimeout(TimeSpan.FromSeconds(15))
                                                                   .UseKestrel(ko =>
                                                                               {
                                                                                   ko.Listen(IPAddress.Any, port, l => l.Protocols = HttpProtocols.Http1AndHttp2);
                                                                                   ko.Limits.MaxRequestBodySize = 15 * 1024 * 1024;
                                                                                   ko.AllowSynchronousIO = false;
                                                                               })
                                                                   .UseStartup<StreamStartup>())
                                       .UseConsoleLifetime();

        var host = builder.Build();

        await host.RunAsync();
    }

    private static IConfiguration BuildConfiguration { get; } = new ConfigurationBuilder().AddJsonFile("appsettings.Global.json", true)
                                                                                          .AddJsonFile("appsettings.json", true)
                                                                                          .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                                                                                          .AddEnvironmentVariables("STREAM_")
                                                                                          .Build();
}
