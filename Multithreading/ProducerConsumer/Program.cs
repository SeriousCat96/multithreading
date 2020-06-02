using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProducerConsumer
{
	class Program
	{
		static Program()
		{
			Thread.CurrentThread.Name = Name;
		}

		static string Name => "MainThread";

		static void Main(string[] args)
		{
			var consumerQueue = new SingleConsumerQueue<TextFileContext>(new TextFileConsumer("data.txt"));
			var producer      = new TextFileContextProducer(consumerQueue);

			Console.WriteLine($"({Thread.CurrentThread.Name}): Нажмите любую клавишу чтобы остановить модель.");

			try
			{
				consumerQueue.Start();
				producer.Start();

				Console.ReadKey();
				Console.WriteLine($"({Thread.CurrentThread.Name}): Остановка модели...");
			}
			finally
			{
				producer.Dispose();
				consumerQueue.Dispose();

				producer.Join();
				consumerQueue.Join();

				Console.WriteLine($"({Thread.CurrentThread.Name}): Модель остановлена. Нажмите любую клавишу для выхода...");
				Console.ReadKey();
			}
		}
	}
}
