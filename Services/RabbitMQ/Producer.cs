using Data.Models;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Data.Constants;

namespace Services.RabbitMQ
{
    public interface IProducer : IDisposable
    {
        string CreateAccount(string message);
        ResultModel CreateAccount(UserCreateModel model);
        ResultModel CreateAccount(CBOCreateModel model);
        void Close();
    }
    public class Producer : IProducer
    {
        private IConnection connection;
        private IModel channel;
        private string replyQueueName;
        private EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private IBasicProperties props;
        private readonly IConfiguration _configuration;
        private ConnectionFactory _factory;


        public Producer(IConfiguration configuration)
        {
            _configuration = configuration;
            _factory = new ConnectionFactory();
            _configuration.Bind("RabbitMqConnection", _factory);
        }

        public string CreateAccount(string message)
        {
            try
            {
                connection = _factory.CreateConnection();
                channel = connection.CreateModel();

                replyQueueName = "amq.rabbitmq.reply-to";

                consumer = new EventingBasicConsumer(channel);

                props = channel.CreateBasicProperties();
                var correlationId = Guid.NewGuid().ToString();
                props.CorrelationId = correlationId;
                props.ReplyTo = replyQueueName;

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var response = Encoding.UTF8.GetString(body);
                    if (ea.BasicProperties.CorrelationId == correlationId)
                    {
                        respQueue.Add(response);
                    }
                };

                var messageBytes = Encoding.UTF8.GetBytes(message);
                channel.BasicConsume(
                   consumer: consumer,
                   queue: replyQueueName,
                   autoAck: true);

                channel.BasicPublish(
                    exchange: "",
                    routingKey: RabbitQueue.CREATE_ACCOUNT,
                    basicProperties: props,
                    body: messageBytes);

                //set timeout = 60s
                respQueue.TryTake(out string result, 60000);
                if (string.IsNullOrEmpty(result))
                {
                    result = "UserManagement Service is not working or too busy at this moment, please try again later!";
                }
                return result;
            }
            finally
            {
                Close();
            }
        }

        public void Close()
        {
            if (channel != null)
            {
                channel.Close();
            }
            if (connection != null)
            {
                connection.Close();
            }
        }

        public void Dispose()
        {
            Close();
        }

        public ResultModel CreateAccount(UserCreateModel model)
        {
            try
            {
                var result = new ResultModel();
                this.InitConnection();

                var json = GetJson(model);

                var messageBytes = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(
                    exchange: "",
                    routingKey: RabbitQueue.CREATE_ACCOUNT,
                    basicProperties: props,
                    body: messageBytes);

                //set timeout = 60s
                respQueue.TryTake(out string resultString, 60000);
                if (string.IsNullOrEmpty(resultString))
                {
                    result.ErrorMessage = "UserManagement Service is not working or too busy at this moment, please try again later!";
                    result.Succeed = false;
                }
                else
                {
                    result = GetResult(resultString);
                }
                return result;
            }
            finally
            {
                Close();
            }
        }

        private void InitConnection()
        {
            connection = _factory.CreateConnection();
            channel = connection.CreateModel();

            replyQueueName = "amq.rabbitmq.reply-to";

            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };

            channel.BasicConsume(
                   consumer: consumer,
                   queue: replyQueueName,
                   autoAck: true);
        }

        private string GetJson(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }
        private ResultModel GetResult(string json)
        {
            return JsonSerializer.Deserialize<ResultModel>(json);
        }

        public ResultModel CreateAccount(CBOCreateModel model)
        {
            try
            {
                var result = new ResultModel();
                this.InitConnection();

                var json = GetJson(model);

                var messageBytes = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(
                    exchange: "",
                    routingKey: "CreateAccountCBODev1",
                    basicProperties: props,
                    body: messageBytes);

                //set timeout = 60s
                respQueue.TryTake(out string resultString, 60000);
                if (string.IsNullOrEmpty(resultString))
                {
                    result.ErrorMessage = "UserManagement Service is not working or too busy at this moment, please try again later!";
                    result.Succeed = false;
                }
                else
                {
                    result = GetResult(resultString);
                }
                return result;
            }
            finally
            {
                Close();
            }
        }
    }
}
