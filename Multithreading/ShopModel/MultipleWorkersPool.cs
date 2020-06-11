using System.Threading;

namespace ShopModel
{
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

		public void Enqueue(T visitor)
		{
			mutex.WaitOne();

			if(waiting < visitorsLimit)
			{
				Log.Trace($"в очереди покупателей: {waiting}");
				EnqueueVisitor(visitor);

				visitors.Release();
				mutex.Release();

				workers.WaitOne();
			}
			else
			{
				Log.Trace($"ушел не обслуженным (переполнение очереди). Посетителей ожидает: {waiting}");
				mutex.Release();
			}
		}

		private void WorkerThreadProc(object state)
		{
			var worker = state as IWorker<T>;
			T visitor  = null;

			Log.Info("готов к работе.");

			while(isWorking)
			{
				Log.Trace($"ожидает обслуживания посетителей: {waiting}");
				visitors.WaitOne();

				mutex.WaitOne();
				try
				{
					visitor = DequeueVisitor();

					workers.Release();
				}
				finally
				{
					mutex.Release();
				}

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
