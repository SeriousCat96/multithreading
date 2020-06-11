using System;
using System.Collections.Generic;
using System.Threading;

namespace ProducerConsumer
{
	/// <summary>Очередь потребления объектов производителя для одного потребителя.</summary>
	/// <typeparam name="T">Тип объекта потребления.</typeparam>
	/// <remarks>Реализация модели Producer-Consumer с использованием <see cref="Monitor"/>.</remarks>
	class MonitorSingleConsumerQueue<T> : ISingleConsumerQueue<T>
		where T : class
	{
		/// <summary>Рабочий поток потребителя.</summary>
		private readonly Thread workerThread;
		/// <summary>Очередь объектов потребления.</summary>
		private readonly Queue<T> consumerQueue;
		/// <summary>Объект синхронизации очереди потребления объектов производителей.</summary>
		private readonly object syncRoot;
		/// <summary>Потребитель данных.</summary>
		private readonly IConsumer<T> consumer;

		private bool isWorking;


		/// <summary>Создание <see cref="MonitorSingleConsumerQueue"/>.</summary>
		/// <param name="consumer">Потребитель данных.</param>
		public MonitorSingleConsumerQueue(IConsumer<T> consumer)
		{
			this.consumer  = consumer ?? throw new ArgumentNullException(nameof(consumer));
			syncRoot       = new object();
			consumerQueue  = new Queue<T>();
			workerThread   = new Thread(ConsumerThreadProc)
			{
				Name         = consumer.Name,
				IsBackground = true
			};
		}

		public void Dispose()
		{
			Monitor.Enter(syncRoot);
			try
			{
				// Сообщаем ждущему потоку завершиться, т.к. инициирована остановка процесса.
				Monitor.Pulse(syncRoot);
				isWorking = false;
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
		}

		public void Start()
		{
			isWorking = true;
			workerThread.Start();
		}

		public void Join() => workerThread.Join();

		/// <summary>Добавляет данные производителя в очередь на исполнение.</summary>
		/// <param name="data">Данные производителя.</param>
		public void Enqueue(T data)
		{
			if(data == null) throw new ArgumentNullException(nameof(data));

			Monitor.Enter(syncRoot);

			try
			{
				Console.WriteLine($"({Thread.CurrentThread.Name}): Pulse");
				// Добавляем объект в очередь.
				// Сообщаем очередному ждущему потоку в Monitor.Wait(syncRoot), что в очереди появился новый объект, поток может идти дальше.
				consumerQueue.Enqueue(data);
				Monitor.Pulse(syncRoot);
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
		}

		/// <summary>Обработка объектов потребителя.</summary>
		private void ConsumerThreadProc()
		{
			while(isWorking)
			{
				Monitor.Enter(syncRoot);
				try
				{
					// Если очередь объектов пуста, ожидаем объекты для потребления.
					// Пропускается только один поток. Остальные потоки ожидают, пока не вызовется Monitor.Pulse(syncRoot).
					if(consumerQueue.Count == 0)
					{
						Console.WriteLine($"({Thread.CurrentThread.Name}): Wait");
						Monitor.Wait(syncRoot);
					}

					// Достаем объект потребления из очереди.
					var consumableObject = consumerQueue.Dequeue();
					// Потребляем объект производителя.
					consumer.Consume(consumableObject);
				}
				finally
				{
					Monitor.Exit(syncRoot);
				}
			}
		}
	}
}
