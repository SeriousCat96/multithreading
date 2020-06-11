using System;
using System.Globalization;
using System.Threading;

namespace ShopModel
{
	class ShopModel : IDisposable
	{
		#region Static

		private const string MainThreadName = "MainThread";
		private const int PeakHourFactor    = 2;

		private static long CustomersCounter;


		static ShopModel()
		{
			Thread.CurrentThread.Name = MainThreadName;
		}

		#endregion

		private readonly Thread workerThread;
		private readonly MultipleWorkersPool<Customer> workersPool;

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

		public ShopModel(CashDeskWorker[] cashDeskworkers, TimeSpan spentTime, TimeSpan frequency)
		{
			workersPool       = new MultipleWorkersPool<Customer>(cashDeskworkers);
			SpentTime         = spentTime;
			VisitorsFrequency = frequency;
			workerThread      = new Thread(ShopModelThreadProc)
			{
				Name         = Name,
				IsBackground = true,
			};
		}

		static void Main(string[] args)
		{
			// 200 - значит в 200 раз быстрее.
			Console.WriteLine("Введите множитель ускорения времени:");
			var factor = double.Parse(Console.ReadLine());
			Console.WriteLine("Введите время в формате чч:мм:сс :");
			var time = TimeSpan.Parse(Console.ReadLine());

			Console.WriteLine("Введите количество касс:");
			var workerCount = int.Parse(Console.ReadLine());
			var workers     = new CashDeskWorker[workerCount];

			Console.WriteLine("Введите время оплаты на кассе (мин.):");
			var paymentTime = double.Parse(Console.ReadLine());
			for(int i = 0; i < workerCount; ++i)
			{
				workers[i] = new CashDeskWorker($"Worker{i + 1}", TimeSpan.FromMinutes(paymentTime));
			}

			Console.WriteLine("Введите время нахождения покупателя (мин.):");
			var spentTime = double.Parse(Console.ReadLine());


			Console.WriteLine("Введите частоту посещения магазина покупателями (мин.):");
			var frequency = double.Parse(Console.ReadLine());

			var model = new ShopModel(workers, TimeSpan.FromMinutes(spentTime), TimeSpan.FromMinutes(frequency));

			Console.WriteLine();

			try
			{
				Time.Setup(time, factor);
				Log.Warn("Нажмите любую клавишу, чтобы остановить модель.");
				model.Start();

				Console.ReadKey(true);
			}
			finally
			{
				Log.Warn("Остановка модели.");

				model.Dispose();
				model.Join();

				Log.Warn("Модель остановлена.");
				Console.ReadKey(true);
			}
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
