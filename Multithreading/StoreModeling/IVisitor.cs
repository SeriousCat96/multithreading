using System;

namespace StoreModeling
{
	/// <summary>Интерфейс посетителя.</summary>
	public interface IVisitor : IThreadWorker
	{
		/// <summary>Возвращает время оплаты на кассе.</summary>
		TimeSpan PaymentTime { get; }

		/// <summary>Возвращает время нахождения в магазине.</summary>
		TimeSpan SpentTime { get; }

		/// <summary></summary>
		void Buy();
	}
}
