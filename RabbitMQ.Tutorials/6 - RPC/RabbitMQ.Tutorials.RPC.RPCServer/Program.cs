using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Tutorials.RPC.RPCServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var queue = "rpc_queue";
                channel.QueueDeclare(queue, false, false, false, null);
                channel.BasicQos(0, 1, false);

                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue, false, consumer);

                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += Consumer_Received;

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            string response = null;
            var channel = (sender as EventingBasicConsumer).Model;

            var body = e.Body.ToArray();
            var props = e.BasicProperties;
            var replyProps = channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            try
            {
                var message = Encoding.UTF8.GetString(body);
                var n = int.Parse(message);
                Console.WriteLine(" [.] fib({0})", message);
                response = fib(n).ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(" [.] " + ex.Message);
                response = "";
            }
            finally
            {
                var responseBytes = Encoding.UTF8.GetBytes(response);
                channel.BasicPublish("", props.ReplyTo, replyProps, responseBytes);
                channel.BasicAck(e.DeliveryTag, false);
            }
        }

        private static int fib(int n)
        {
            if (n == 0 || n == 1)
            {
                return n;
            }

            return fib(n - 1) + fib(n - 2);
        }
    }
}
