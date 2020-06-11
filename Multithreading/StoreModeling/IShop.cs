using System;

namespace StoreModeling
{
	public interface IShop<TVisitor> : IThreadWorker
		where TVisitor : class, IVisitor
	{
		/// <summary>Возвращает количество касс.</summary>
		int CashDesksCount { get; }

		/// <summary>Возвращает частоту прихода покупателей.</summary>
		TimeSpan VisitorsFrequency { get; }

		/// <summary>Возвращает время открытия.</summary>
		TimeSpan OpenTime { get; }

		/// <summary>Возвращает время закрытия.</summary>
		TimeSpan CloseTime { get; }

		/// <summary>Возвращает начало час-пика.</summary>
		TimeSpan PeakHourBegin { get; }

		/// <summary>Возвращает конец час-пика.</summary>
		TimeSpan PeakHourEnd { get; }

		/// <summary>Возвращает флаг режима "час-пик".</summary>
		bool IsPeakHour { get; }

		/// <summary>Возвращает очередь покупателей.</summary>
		ICashDeskModel<TVisitor> ShopModel { get; }
	}
}
