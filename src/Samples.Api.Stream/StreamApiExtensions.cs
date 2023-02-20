using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Samples.Api.Stream.Controllers;
using Samples.Api.Stream.Models;
using Samples.Api.Stream.Transforms;
using Samples.Configuration;
using Samples.Contracts.Stream.Interfaces;
using Samples.Contracts.Stream.Models;
using Samples.Extensions.Hosting;
using Samples.Extensions.Hosting.Middleware;

namespace Samples.Api.Stream;

public static class StreamApiExtensions
{
    public static IApplicationBuilder UseStreamApiDefaults(this IApplicationBuilder app)
    {
        app.RegisterOnApplicationStopping(() =>
                                          {
                                              var logger = app.ApplicationServices.GetRequiredService<ILogger<StreamControllerBase>>();

                                              ApplicationShutdownCancellationSource.Instance.TryCancel();

                                              logger.LogInformation("*** StreamApi Shutdown initiated, stopping services...");
                                          });

        app.RegisterOnApplicationStarted(() =>
                                         {
                                             var logger = app.ApplicationServices.GetRequiredService<ILogger<StreamControllerBase>>();

                                             logger.LogInformation("StreamApi service is ready for requests");
                                         });

        app.RegisterOnApplicationStopped(() =>
                                         {
                                             var logger = app.ApplicationServices.GetRequiredService<ILogger<StreamControllerBase>>();

                                             logger.LogInformation("*** Shutdown completed, exiting...");
                                         });

        app.UseRouting()
           .UseSampleRequestResponseLogging()
           .UseHealthChecks("/ping");

        return app;
    }

    public static IServiceCollection AddSampleStreamApi(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHealthChecks()
                         .AddServicePingHealthCheck("SampleStreamApi");

        serviceCollection.AddControllers()
                         .AddJsonOptions(x =>
                                         {
                                             x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                                             x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                                             x.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                                             x.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                                             x.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                                             x.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                                             x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                                         })
                         .AddApplicationPart(typeof(SnippetsController).Assembly)
                         .AddMvcOptions(o =>
                                        {
                                            o.Filters.Add(new ModelAttributeValidationFilter());
                                            o.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "The field is required.");
                                            o.MaxModelValidationErrors = 10;
                                        });

        serviceCollection.AddSingleton<ITransform<ApiSnippet, Snippet>, ApiSnippetToSnippetTransform>()
                         .AddSingleton<ITransform<ApiSnippetStatValue, SnippetStat>, ApiSnippetStatValueToSnippetStatTransform>();

        return serviceCollection;
    }

    public static ActionResult<SampleApiResults<T>> AsOkSampleApiResults<T>(this IEnumerable<T> response)
        where T : class
        => new OkObjectResult(AsSampleApiResults(response));

    private static SampleApiResults<T> AsSampleApiResults<T>(this IEnumerable<T> response)
        where T : class
        => new()
           {
               Data = response.ToList()
           };
}
