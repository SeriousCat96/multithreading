using System;
using System.Threading;

namespace ShopModel
{
	/// <summary>Посетитель магазина.</summary>
	class Customer
	{
		private readonly Thread workerThread;

		public TimeSpan SpentTime { get; }

		public string Name => workerThread.Name;

		public Customer(TimeSpan spentTime, Thread workerThread)
		{
			SpentTime         = spentTime;
			this.workerThread = workerThread;
		}


		public void Dispose()
		{
		}

		public void Join() => workerThread.Join();

		public void Start() => workerThread.Start(this);
	}
}
