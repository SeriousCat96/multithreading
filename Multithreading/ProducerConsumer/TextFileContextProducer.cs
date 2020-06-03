using System;
using System.Linq;
using System.Threading;

namespace ProducerConsumer
{
	/// <summary>Поставщик произвольных текстовых данных.</summary>
	class TextFileContextProducer : IProducer<TextFileContext>
	{
		/// <summary>Рабочий поток поставщика.</summary>
		public Thread workerThread;
		/// <summary>Объект синхронизации производителя.</summary>
		private readonly object syncRoot;
		/// <summary>Генератор случайных чисел.</summary>
		private readonly Random random;

		private bool isWorking;


		public string Name => nameof(TextFileContextProducer);

		/// <summary>Возвращает очередь объектов потребления.</summary>
		public ISingleConsumerQueue<TextFileContext> ConsumerQueue { get; }

		/// <summary>Создание <see cref="TextFileContextProducer"/>.</summary>
		/// <param name="consumerQueue">Очередь потребления объектов.</param>
		public TextFileContextProducer(ISingleConsumerQueue<TextFileContext> consumerQueue)
		{
			ConsumerQueue = consumerQueue ?? throw new ArgumentNullException(nameof(consumerQueue));
			syncRoot      = new object();
			random        = new Random();
			workerThread  = new Thread(ProducerThreadProc)
			{
				Name         = Name,
				IsBackground = true,
			};
		}

		public void Dispose()
		{
			Monitor.Enter(syncRoot);
			try
			{
				isWorking = false;
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
		}

		public void Join() => workerThread.Join();

		public void Start()
		{
			isWorking = true;
			workerThread.Start();
		}

		public TextFileContext Produce()
		{
			Thread.Sleep(random.Next(0, 3000));

			// Рандомное количество слов от 1 до 8.
			var words = new string[random.Next(1, 8)];

			for(int i = 0; i < words.Length; ++i)
			{
				// Генерируем рандомную строку от 1 до 15 символов.
				words[i] = GetRandomString(random.Next(1, 15));
			}

			// Склеиваем строки
			return new TextFileContext(string.Join(" ", words));
		}

		private void ProducerThreadProc()
		{
			while(isWorking)
			{
				Monitor.Enter(syncRoot);

				try
				{
					// Производим объект.
					var context = Produce();

					Console.WriteLine($"({Thread.CurrentThread.Name}): Генерируем данные: \"{context}\".");
					// Добавляем объект в очередь потребления.
					ConsumerQueue.Enqueue(context);
				}
				finally
				{
					Monitor.Exit(syncRoot);
				}
			}
		}

		private string GetRandomString(int length)
		{
			const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}
}
