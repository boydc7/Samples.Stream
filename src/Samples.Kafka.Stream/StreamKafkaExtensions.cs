using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Samples.Contracts.Stream.Interfaces;
using Samples.Contracts.Stream.Models;

namespace Samples.Kafka.Stream;

public static class StreamKafkaExtensions
{
    public static IServiceCollection AddKafkaSnippetIngestion(this IServiceCollection services)
    {
        services.AddSingleton<ISnippetIngestionService, KafkaSnippetIngestionService>()
                .TryAddSingleton<ITransform<Snippet, StreamSnippet>, SnippetToKafkaSnippetTransform>();

        return services;
    }

    public static IServiceCollection AddKafkaSnippetReader(this IServiceCollection services)
    {
        services.AddSingleton<ISnippetReader, KafkaSnippetReader>()
                .TryAddSingleton<ITransform<Snippet, StreamSnippet>, SnippetToKafkaSnippetTransform>();

        return services;
    }

    public static void LogKafkaMessage(this ILogger log, LogMessage logMessage)
        => log.Log(GetNetCoreLogLevel(logMessage),
                   "{LogMessage} :: via client [{KafkaClientInstance}]",
                   logMessage.Message,
                   logMessage.Name);

    public static void LogKafkaError(this ILogger log, Error kafkaError)
        => log.Log(LogLevel.Error,
                   "{ErrorMessage} :: errorCode [{KafkaErrorCode}], errorSource [{KafkaErrorSource}]",
                   kafkaError.ToString(),
                   kafkaError.Code,
                   kafkaError.IsLocalError
                       ? "local"
                       : "broker");

    public static LogLevel GetNetCoreLogLevel(this LogMessage kafkaLogMessage)
    {
        try
        {
            return (LogLevel)kafkaLogMessage.LevelAs(LogLevelType.MicrosoftExtensionsLogging);
        }
        catch
        {
            return LogLevel.Information;
        }
    }
}
