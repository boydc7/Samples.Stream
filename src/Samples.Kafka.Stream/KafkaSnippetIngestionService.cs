using Confluent.Kafka;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Samples.Configuration.Abstractions;
using Samples.Configuration.Exceptions;
using Samples.Contracts.Stream.Interfaces;
using Samples.Contracts.Stream.Models;
using Samples.Extensions;
using Samples.Metrics.Abstractions;

namespace Samples.Kafka.Stream;

public class KafkaSnippetIngestionService : ISnippetIngestionService
{
    private const string _topicName = "stream-snippets";

    private readonly ITransform<Snippet, StreamSnippet> _transform;
    private readonly IStatsManager _statsManager;
    private readonly ILogger<KafkaSnippetIngestionService> _log;
    private readonly IProducer<string, byte[]> _producer;

    private bool _inShutdown;

    public KafkaSnippetIngestionService(IConfiguration configuration,
                                        ITransform<Snippet, StreamSnippet> transform,
                                        IShutdownRegistrar shutdownRegistrar,
                                        IStatsManager statsManager,
                                        ILogger<KafkaSnippetIngestionService> log)
    {
        _transform = transform;
        _statsManager = statsManager;
        _log = log;

        var bootstrapServers = configuration.GetConnectionString("Kafka") ?? throw new ArgumentNullException(nameof(configuration));

        _producer = KafkaProducerFactory.GetOrCreate(bootstrapServers, log, "stream");

        shutdownRegistrar.Register(OnShutdown);
    }

    public ValueTask IngestAsync(Snippet snippet)
    {
        if (_inShutdown)
        {
            return ValueTask.CompletedTask;
        }

        if (IsResetRequired)
        {
            throw new InvalidRequestException("Producing currently disabled until emptied and reset");
        }

        var kafkaSnippet = _transform.Transform(snippet);
        var bytes = kafkaSnippet.ToByteArray();

        _producer.Produce(_topicName,
                          new Message<string, byte[]>
                          {
                              Key = kafkaSnippet.UserId,
                              Value = bytes
                          },
                          OnMessageDelivery);

        return ValueTask.CompletedTask;
    }

    public bool IsResetRequired { get; private set; }

    public void OnShutdown()
    {
        _inShutdown = true;

        FlushProducerOnce(TimeSpan.FromMilliseconds(3250));
    }

    private void FlushProducerOnce(TimeSpan timeout)
    {
        try
        {
            _producer.Flush(timeout);
        }
        catch(ObjectDisposedException)
        {
            // Ignore
        }
        catch(Exception x)
        {
            if (!_inShutdown && !IsResetRequired)
            {
                _log.Exception(x);
            }
        }
    }

    private void OnMessageDelivery(DeliveryReport<string, byte[]> deliveryReport)
    {
        var failed = deliveryReport.Error?.IsError ?? false;
        
        _statsManager.Increment(SamplesMetrics.IngestionSnippetsProduced);
        
        if (failed)
        {
            if (!IsResetRequired)
            { // Avoid flooding the logs here...
                var sha = deliveryReport.Value.ToSha256Base64();

                _log.LogError("Delivery failure with message hash of [{ShaHash}] for userId [{UserId}]. KafkaErrorCode [{KafkaErrorCode}], Reason [{KafkaErrorReason}]",
                              sha, deliveryReport.Key, deliveryReport.Error.Code.ToStringInvariant(), deliveryReport.Error.ToString());
            }

            IsResetRequired = true;
        }
    }
}
