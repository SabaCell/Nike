// using System.Threading;
// using System.Threading.Tasks;
// using Consumers.Models;
// using Microsoft.Extensions.Logging;
//
// namespace Consumers.Services
// {
//     public class ResponseMessageHandler
//     {
//         private ILogger<ResponseMessageHandler> _logger;
//         private readonly KafkaConsumer<MyResponseMessage> _consumer;
//
//         public ResponseMessageHandler(ILogger<ResponseMessageHandler> logger, KafkaConsumer<MyResponseMessage> consumer)
//         {
//             _logger = logger;
//             _consumer = consumer;
//         }
//         internal async Task ConsumeAsync(CancellationToken cancellationToken)
//         {
//             while (true)
//             {
//                 cancellationToken.ThrowIfCancellationRequested();
//                 var resMessage = _consumer.Consume(cancellationToken);
//
//                 if (resMessage == null)
//                 {
//                     _logger.LogWarning("message response is null");
//                     continue;
//                 }
//                 await HandleAsync(resMessage);
//
//                 _logger.LogDebug($"consume new response-message {resMessage.Name}");
//             }
//         }
//
//
//         public Task HandleAsync(MyResponseMessage message)
//         {
//             _logger.LogWarning($"            processed-response-message in {message.ToString()}");
//             return Task.CompletedTask;
//         }
//     }
// }