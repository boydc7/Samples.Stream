using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Samples.Contracts.Stream.Interfaces;
using Samples.Contracts.Stream.Models;

namespace Samples.Kafka.Stream;

public class KafkaSnippetReader : ISnippetReader, IAsyncDisposable
{
    private const string _topicName = "stream-snippets";

    private bool _inShutdown;
    private readonly ILogger<KafkaSnippetReader> _log;
    private readonly ITransform<Snippet, StreamSnippet> _transform;
    private readonly IConsumer<string, byte[]> _consumer;
    private readonly TimeSpan _streamReadTimeout;

    public KafkaSnippetReader(IConfiguration configuration, ILogger<KafkaSnippetReader> log,
                              ITransform<Snippet, StreamSnippet> transform)
    {
        _log = log;
        _transform = transform;
        _streamReadTimeout = TimeSpan.FromSeconds(7);

        var bootstrapServers = configuration.GetConnectionString("Kafka") ?? throw new ArgumentNullException(nameof(configuration));

        _consumer = KafkaConsumerFactory.Instance.GetOrCreate(bootstrapServers, log, clientId: "stream-snippets");
    }

    public IEnumerable<ConsumedEvent<Snippet>> Read()
    {
        try
        {
            _consumer.Subscribe(_topicName);

            do
            {
                var consumedEvent = KafkaConsume(_streamReadTimeout);

                if (consumedEvent == null)
                {
                    yield break;
                }

                yield return consumedEvent;
            } while (!_inShutdown);
        }
        finally
        {
            _consumer.Unsubscribe();
        }
    }

    private ConsumedEvent<Snippet> KafkaConsume(TimeSpan timeout)
    {
        var consumed = _consumer.Consume(timeout);

        if (consumed?.Message?.Value == null || consumed.Message.Value.Length <= 0)
        {
            return null;
        }

        var proto = StreamSnippet.Parser.ParseFrom(consumed.Message.Value);

        _log.LogDebug("ConsumedEvent StreamSnippet message with key [{MessageId}] and length [{ByteLen}] for UserId [{UserId}]",
                      consumed.Message.Key, consumed.Message.Value.Length, proto.UserId);

        var model = _transform.Transform(proto);

        return new ConsumedEvent<Snippet>
               {
                   Event = model,
                   Partition = consumed.Partition.Value,
                   Offset = consumed.Offset.Value
               };
    }

    public void AckConsume(int partition, long offset)
        => KafkaAckConsume(new TopicPartitionOffset(_topicName, partition, offset + 1));

    private void KafkaAckConsume(TopicPartitionOffset ack)
    {
        if (ack == null)
        {
            return;
        }

        _consumer.StoreOffset(ack);
    }

    public void Stop()
    {
        _inShutdown = true;

        try
        {
            _consumer.Unsubscribe();
        }
        catch
        {
            // ignored
        }

        try
        {
            _consumer.Close();
        }
        catch
        {
            // ignored
        }

        try
        {
            _consumer.Dispose();
        }
        catch
        {
            // ignored
        }
    }

    public void Dispose()
        => DisposeAsync().AsTask().GetAwaiter().GetResult();

    public ValueTask DisposeAsync()
    {
        Stop();

        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }
}
