using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Samples.Api.Stream;
using Samples.Configuration;
using Samples.Kafka.Stream;
using Samples.Messaging.Local;
using Samples.Metrics.Prometheus;
using Samples.Redis.Stream;
using Samples.Stream.Core;

namespace Samples.Service.Stream;

internal class StreamStartup
{
    public void Configure(IApplicationBuilder app)
    {
        app.UseStreamApiDefaults()
           .UsePrometheusMetrics()
           .UseEndpoints(r => { r.MapControllers(); })
           .UseSwagger()
           .UseSwaggerUI(o =>
                         {
                             o.SwaggerEndpoint("/swagger/stream/swagger.json", "Stream API");
                             o.RoutePrefix = string.Empty;
                         });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSampleStreamApi()
                .AddSwaggerGen(c =>
                               {
                                   c.SwaggerDoc("stream", new OpenApiInfo
                                                          {
                                                              Version = "v1",
                                                              Title = "SampleStreamIngestion",
                                                              Description = "Simple service that provides ability to excercise ingestion of stream api data"
                                                          });

                                   foreach (var xmlFile in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly))
                                   {
                                       c.IncludeXmlComments(xmlFile);
                                   }
                               });

        services.AddCancelSourceShutdownRegistrar()
                .AddKafkaSnippetIngestion()
                .AddKafkaSnippetReader()
                .AddSampleLocalDeferredProducerConsumer()
                .AddStreamRedisSnippetProcessors()
                .AddSimpleSnippetProcessingServices()
                .AddPrometheusMetrics();
    }
}
