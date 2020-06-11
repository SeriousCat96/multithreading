using System;
using System.Threading;

namespace ShopModel
{
	class Program
	{
		private const string MainThreadName = "MainThread";

		static Program()
		{
			Thread.CurrentThread.Name = MainThreadName;
		}

		public static void Main(string[] args)
		{
			// 200 - значит в 200 раз быстрее.
			Console.WriteLine("Введите множитель ускорения времени:");
			var factor = double.Parse(Console.ReadLine());
			Console.WriteLine("Введите время в формате чч:мм:сс :");
			var time = TimeSpan.Parse(Console.ReadLine());

			Console.WriteLine("Введите количество касс:");
			var workerCount = int.Parse(Console.ReadLine());
			var workers = new ShopWorker[workerCount];

			Console.WriteLine("Введите время оплаты на кассе (мин.):");
			var paymentTime = double.Parse(Console.ReadLine());
			for(int i = 0; i < workerCount; ++i)
			{
				workers[i] = new ShopWorker($"Worker{i + 1}", TimeSpan.FromMinutes(paymentTime));
			}

			Console.WriteLine("Введите время нахождения покупателя (мин.):");
			var spentTime = double.Parse(Console.ReadLine());


			Console.WriteLine("Введите частоту посещения магазина покупателями (мин.):");
			var frequency = double.Parse(Console.ReadLine());

			var workersPool = new MultipleWorkersPool<Customer>(workers);
			var model       = new ShopModel(workersPool, TimeSpan.FromMinutes(spentTime), TimeSpan.FromMinutes(frequency));

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
	}
}
