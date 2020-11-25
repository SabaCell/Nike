using System.Threading;
using Confluent.Kafka;
using Nike.EventBus.Kafka.Model;

namespace Nike.EventBus.Kafka.Extenstion
{
    public static class KafkaConsumerHelper
    {
        public static bool TryConsumeMessage(this IConsumer<Ignore, string> consumer, int timeOut,
            ConsumeMessageResult result, CancellationToken cancellationToken)
        {
            if (!TryConsume(consumer, timeOut, out var consumeResult, cancellationToken))
                return false;

            result.SetMessageAsync(consumeResult);

            return true;
        }

        public static bool TryConsume(this IConsumer<Ignore, string> consumer, int timeout,
            out ConsumeResult<Ignore, string> consumeResult, CancellationToken cancellationToken)
        {
            consumeResult = null;

            consumeResult = consumer.Consume(timeout);

            if (consumeResult != null)
                return consumeResult.Message != null;

            // consumeResult = consumer.Consume(cancellationToken);

            return consumeResult?.Message != null;
        }
    }
}