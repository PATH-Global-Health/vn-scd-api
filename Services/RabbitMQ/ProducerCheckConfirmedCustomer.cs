﻿using System;
using System.Collections.Concurrent;
using System.Text;
using Data.Constants;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Services.RabbitMQ
{

    public interface IProducerCheckConfirmedCustomer : IDisposable
    {
        string CheckConfirmedCustomer(string message);
        void Close();
    }
    public class ProducerCheckConfirmedCustomer: IProducerCheckConfirmedCustomer
    {
        private IConnection connection;
        private IModel channel;
        private string replyQueueName;
        private EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private IBasicProperties props;
        private readonly IConfiguration _configuration;
        private ConnectionFactory _factory;

        public ProducerCheckConfirmedCustomer(IConfiguration configuration)
        {
            _configuration = configuration;
            _factory = new ConnectionFactory();
            _configuration.Bind("RabbitMqConnection", _factory);
            _factory.ClientProvidedName = "CheckConfirmedCustomer";
        }

        public string CheckConfirmedCustomer(string message)
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
                    routingKey: RabbitQueue.CHECK_CONFIRMED_CUSTOMER,
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



    }


}
