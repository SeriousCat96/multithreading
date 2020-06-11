using System;
using System.Threading;

namespace StoreModeling
{
	/// <summary>Покупатель.</summary>
	class Customer : IVisitor
	{
		/// <summary>Рабочий поток поставщика.</summary>
		private readonly Thread workerThread;
		private readonly ICashDeskModel<IVisitor> cashDeskModel;
		private readonly TimeModel timeModel;

		public string Name { get; }


		public TimeSpan PaymentTime => throw new NotImplementedException();

		public TimeSpan SpentTime => throw new NotImplementedException();

		public Customer(string name, ICashDeskModel<IVisitor> cashDeskModel, TimeModel timeModel)
		{
			Name           = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
			this.timeModel = timeModel ?? throw new ArgumentNullException(nameof(timeModel));
			workerThread   = new Thread(cashDeskModel.CustomerThreadProc)
			{
				Name         = name,
				IsBackground = true,
			};
		}

		public void Dispose()
		{

		}

		public void Join() => workerThread.Join();

		public void Start()
		{
			workerThread.Start();
		}

		public void Visit()
		{
			Console.WriteLine(GlobalConstants.MessageFormat, timeModel.GetCurrentTime(), Name, "идёт в магазин.");
			timeModel.Sleep(SpentTime);
		}

		public void Buy()
		{
			Console.WriteLine(GlobalConstants.MessageFormat, timeModel.GetCurrentTime(), Name, "идёт на кассу.");
			timeModel.Sleep(PaymentTime);
		}
	}
}
