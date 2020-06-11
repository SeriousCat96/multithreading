using System;
using System.Threading;

namespace ShopModel
{
	/// <summary>Интерфейс обработчика в отдельном потоке</summary>
	public interface IThreadWorker : IDisposable
	{
		/// <summary>Возвращает имя рабочего потока.</summary>
		string Name { get; }

		/// <summary>Запускает поток.</summary>
		void Start();

		/// <summary>Блокирует вызывающий поток, пока рабочий поток не завершит работу.</summary>
		void Join();
	}
}
