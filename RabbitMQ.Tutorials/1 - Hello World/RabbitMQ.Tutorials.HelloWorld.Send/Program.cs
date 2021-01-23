﻿using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.Tutorials.HelloWorld.Send
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("hello", false, false, false, null);

                string message = "Hello World";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("", "hello", null, body);

                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
