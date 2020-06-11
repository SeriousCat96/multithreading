using System.Threading;

namespace ShopModel
{
	/// <summary>Пул обработчиков для решения "проблемы спящего парикмахера".</summary>
	/// <typeparam name="T">Тип объекта потребления.</typeparam>
	/// <remarks>
	/// Реализация "проблемы спящего парикмахера" с использованием <see cref="Semaphore"/>. Подробнее <seealso cref="https://ru.wikipedia.org/wiki/Проблема_спящего_парикмахера"/>.
	/// </remarks>
	class MultipleWorkersPool<T> : IMultipleWorkersPool<T>
		where T : class
	{
		private readonly Semaphore workers;
		private readonly Semaphore visitors;
		private readonly Semaphore mutex;

		private readonly Thread[] workerThreads;
		private readonly IWorker<T>[] workerCollection;
		private readonly T[] queue;
		private readonly int visitorsLimit;

		private volatile int waiting;
		private volatile int serveNext;
		private volatile int positionNext;
		private volatile bool isWorking;

		public MultipleWorkersPool(IWorker<T>[] workerCollection, int visitorsLimit = 10)
		{
			queue                 = new T[visitorsLimit];
			workerThreads         = new Thread[workerCollection.Length];
			this.workerCollection = workerCollection;
			this.visitorsLimit    = visitorsLimit;

			workers  = new Semaphore(workerCollection.Length, workerCollection.Length);
			visitors = new Semaphore(0, visitorsLimit);
			mutex    = new Semaphore(1, 1);

			for(int i = 0; i < workerCollection.Length; ++i)
			{
				workerThreads[i] = new Thread(WorkerThreadProc)
				{
					Name         = workerCollection[i].Name,
					IsBackground = true,
				};
				this.workerCollection[i] = workerCollection[i];
			}
		}

		/// <summary>Работа потоков-посетителей.</summary>
		/// <param name="visitor">Посетитель.</param>
		public void Enqueue(T visitor)
		{
			// Вход в критическую секцию. Исключаем ситуацию гонки (race condition), когда одновременно два посетителя пытаются занять одно место в очереди.
			mutex.WaitOne();

			// Если в очереди находится меньше посетителей, чем установленный лимит, то посетитель ищет место в очереди.
			// Иначе он уходит ни с чем.
			if(waiting < visitorsLimit)
			{
				Log.Trace($"в очереди покупателей: {waiting}");
				// Занимает место в очереди посетителей.
				EnqueueVisitor(visitor);

				// Сигнализируем обработчикам, что в очереди появился покупатель.
				visitors.Release();
				// Выход из критической секции.
				mutex.Release();

				// Ожидаем свободного обработчика.
				workers.WaitOne();
			}
			else
			{
				Log.Trace($"ушел не обслуженным (переполнение очереди). Посетителей ожидает: {waiting}");
				mutex.Release();
			}
		}

		/// <summary>Работа потоков-касс (обработчики).</summary>
		/// <param name="state"></param>
		private void WorkerThreadProc(object state)
		{
			var worker = state as IWorker<T>;
			T visitor  = null;

			Log.Info("готов к работе.");

			while(isWorking)
			{
				// Ждёт клиентов в очереди (отдыхает).
				Log.Trace($"ожидает обслуживания посетителей: {waiting}");
				visitors.WaitOne();

				// Вход в критическую секцию. Исключаем ситуацию гонки (race condition), чтобы знать актуальное количество посетителей в очереди.
				mutex.WaitOne();
				try
				{
					visitor = DequeueVisitor();

					// Сообщаем посетителям о готовности к работе.
					workers.Release();
				}
				finally
				{
					// Выход из критической секции.
					mutex.Release();
				}

				// Обслуживание посетителя.
				worker.Process(visitor);
			}
		}

		private void EnqueueVisitor(T visitor)
		{
			Interlocked.Increment(ref waiting);
			positionNext        = positionNext++ % visitorsLimit;
			queue[positionNext] = visitor;
		}

		private T DequeueVisitor()
		{
			serveNext = serveNext++ % visitorsLimit;
			Interlocked.Decrement(ref waiting);
			return queue[serveNext];
		}

		public void Start()
		{
			isWorking = true;

			for(int i = 0; i < workerThreads.Length; ++i)
			{
				workerThreads[i].Start(workerCollection[i]);
			}
		}

		public void Join()
		{
			foreach(var thread in workerThreads)
			{
				thread.Join();
			}
		}

		public void Dispose()
		{
			isWorking = false;
		}
	}
}
