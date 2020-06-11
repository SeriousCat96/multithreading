using System;
using System.IO;
using System.Threading;

namespace ProducerConsumer
{
	class Program
	{
		static Program()
		{
			Thread.CurrentThread.Name = Name;
		}

		static string Name => "MainThread";
		static string FileName => "data.txt";

		static void Main(string[] args)
		{
			ISingleConsumerQueue<TextFileContext> consumerQueue = new MonitorSingleConsumerQueue<TextFileContext>(new TextFileConsumer(FileName));
			IThreadWorker producerThread = new TextFileContextProducer(consumerQueue);
			IThreadWorker consumerThread = consumerQueue;

			try
			{
				consumerThread.Start();
				producerThread.Start();

				Console.WriteLine($"({Thread.CurrentThread.Name}): Модель запущена. Данные пишутся в {Path.Combine(Directory.GetCurrentDirectory(), FileName)}");
				Console.WriteLine($"({Thread.CurrentThread.Name}): Нажмите любую клавишу, чтобы остановить модель.");
				Console.ReadKey(true);
				Console.WriteLine($"({Thread.CurrentThread.Name}): Остановка модели...");
			}
			finally
			{
				producerThread.Dispose();
				consumerThread.Dispose();

				producerThread.Join();
				consumerThread.Join();

				Console.WriteLine($"({Thread.CurrentThread.Name}): Модель остановлена. Нажмите любую клавишу для выхода...");
				Console.ReadKey(true);
			}
		}
	}
}
