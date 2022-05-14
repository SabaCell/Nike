using System;
using System.Threading;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Kafka.Model;

namespace Nike.EventBus.Kafka.Extenstion;

public static class KafkaConsumerHelper
{
    public static bool TryConsumeMessage(this IConsumer<Ignore, string> consumer, ConsumeMessageResult result,
        ILogger logger, CancellationToken cancellationToken)
    {
        try
        {
            if (!TryConsume(consumer, cancellationToken, out var consumeResult))
                return false;

            result.SetMessage(consumeResult, cancellationToken);

            return true;
        }
        catch (Exception e)
        {
            logger.LogCritical(e, $"Nike exception: {consumer.Name} has exception in TryConsumeMessage {e.Message}");
            return false;
        }
    }

    private static bool TryConsume(this IConsumer<Ignore, string> consumer, CancellationToken cancellationToken,
        out ConsumeResult<Ignore, string> consumeResult)
    {
        consumeResult = null;
        consumeResult = consumer.Consume(cancellationToken);

        return consumeResult != null ? consumeResult.Message != null : consumeResult?.Message != null;
    }
}