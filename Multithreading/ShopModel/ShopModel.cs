using System;
using System.Threading;

namespace ShopModel
{
	class ShopModel : IDisposable
	{

		private const int PeakHourFactor = 2;
		private static long CustomersCounter;

		private readonly Thread workerThread;
		private readonly IMultipleWorkersPool<Customer> workersPool;

		private volatile int customersInside;
		private bool isWorking;

		public string Name => "ShopModelThread";

		/// <summary>Возвращает частоту прихода покупателей.</summary>
		public TimeSpan VisitorsFrequency { get; }

		/// <summary>Возвращает частоту прихода покупателей.</summary>
		public TimeSpan SpentTime { get; }

		/// <summary>Возвращает время открытия.</summary>
		public TimeSpan OpenTime { get; } = new TimeSpan(8, 0, 0);

		/// <summary>Возвращает время закрытия.</summary>
		public TimeSpan CloseTime { get; } = new TimeSpan(23, 0, 0);

		/// <summary>Возвращает начало час-пика.</summary>
		public TimeSpan PeakHourBegin { get; } = new TimeSpan(17, 0, 0);

		/// <summary>Возвращает конец час-пика.</summary>
		public TimeSpan PeakHourEnd { get; } = new TimeSpan(20, 0, 0);

		/// <summary>Возвращает флаг режима "час-пик".</summary>
		public bool IsPeakHour
		{
			get
			{
				var time = Time.Current.Now;
				return Time.Current.Compare(time, PeakHourBegin) >= 0 && Time.Current.Compare(time, PeakHourEnd) <= 0;
			}
		}

		public ShopModel(IMultipleWorkersPool<Customer> workersPool, TimeSpan spentTime, TimeSpan frequency)
		{
			this.workersPool  = workersPool;
			SpentTime         = spentTime;
			VisitorsFrequency = frequency;
			workerThread      = new Thread(ShopModelThreadProc)
			{
				Name         = Name,
				IsBackground = true,
			};
		}

		public void Start()
		{
			isWorking = true;

			workersPool.Start();
			workerThread.Start();
		}

		public void Join()
		{
			workersPool.Join();
			workerThread.Join();
		}

		public void Dispose()
		{
			isWorking = false;
			workersPool.Dispose();
		}

		private void ShopModelThreadProc(object state)
		{
			Log.Warn("готов.");

			while(isWorking)
			{
				var freq = !IsPeakHour ? VisitorsFrequency : TimeSpan.FromMilliseconds(VisitorsFrequency.TotalMilliseconds / PeakHourFactor);
				Time.Current.Sleep(freq);

				var customer = new Customer(SpentTime, new Thread(CustomerThreadProc)
				{
					Name         = $"{nameof(Customer)}{++CustomersCounter}",
					IsBackground = true,
				});

				var time = Time.Current.Now;
				if(Time.Current.Compare(time, OpenTime) >= 0 && Time.Current.Compare(time, CloseTime) <= 0)
				{
					Interlocked.Increment(ref customersInside);
					Log.Warn($"{customer.Name} зашёл в магазин. Покупателей в магазине: {customersInside}");

					customer.Start();
				}
				else
				{
					Log.Warn($"Магазин закрыт: {customer.Name} идёт домой. Покупателей в магазине: {customersInside}");
				}
			}
		}

		private void CustomerThreadProc(object state)
		{
			var customer = state as Customer;

			Time.Current.Sleep(customer.SpentTime);

			Interlocked.Decrement(ref customersInside);
			Log.Info($"пошёл на кассу. Покупателей в магазине: {customersInside}");

			workersPool.Enqueue(customer);
		}
	}
}
