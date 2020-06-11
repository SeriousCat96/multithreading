using System;

namespace ShopModel
{
	/// <summary>Касса, обслуживающая покупателя.</summary>
	class CashDeskWorker : IWorker<Customer>
	{
		public string Name { get; }

		/// <summary>Возвращает время оплаты на кассе.</summary>
		public TimeSpan PaymentTime { get; }

		public CashDeskWorker(string name, TimeSpan paymentTime)
		{
			Name         = name;
			PaymentTime  = paymentTime;
		}

		public void Process(Customer customer)
		{
			Log.Info($"Обслуживание покупателя {customer.Name}");
			Time.Current.Sleep(PaymentTime);
			Log.Info($"Обслуживание покупателя {customer.Name} завершено.");
		}
	}
}
