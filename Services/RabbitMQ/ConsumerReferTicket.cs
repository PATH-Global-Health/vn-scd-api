using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Constants;
using Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Service.RabbitMQ;

namespace Services.RabbitMQ
{
    public class ConsumerReferTicket : BackgroundService
    {
        private IConnection connection;
        private IModel channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private EventingBasicConsumer consumer;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConsumerReferTicket> _logger;

        public ConsumerReferTicket(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<ConsumerReferTicket> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            try
            {
                var factory = new ConnectionFactory();
                _configuration.Bind("RabbitMqConnection", factory);

                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.QueueDeclare(queue: RabbitQueue.ADD_REFER_TICKET, durable: false,
                    exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: RabbitQueue.ADD_REFER_TICKET,
                    autoAck: false, consumer: consumer);
                _logger.LogInformation("-RabbitMQ queue created: AddReferTicket1");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "RabbitMQ queue create fail.");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            consumer.Received += (model, ea) =>
            {
                string response = null;

                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    var result = AddReferTicket(message);
                    response = JsonConvert.SerializeObject(result);
                }
                catch (Exception e)
                {
                    var result = new ResultModel();
                    result.Succeed = false;
                    result.ErrorMessage = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                    response = JsonConvert.SerializeObject(result);
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                        basicProperties: replyProps, body: responseBytes);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag,
                        multiple: false);
                }

            };

            return Task.CompletedTask;
        }

        private ResultModel AddReferTicket(string message)
        {
            var messageDeser = JsonConvert.DeserializeObject<TicketEmployeeModel>(message);
            using (var scope = _scopeFactory.CreateScope())
            {
                IReferTicketService referTicketService = scope.ServiceProvider.GetRequiredService<IReferTicketService>();
                return referTicketService.CreateTicket(messageDeser);
            }
        }
    }
}
