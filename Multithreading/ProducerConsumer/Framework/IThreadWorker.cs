using System;

namespace ProducerConsumer
{
	/// <summary>Интерфейс обработки в отдельном потоке</summary>
	public interface IThreadWorker : IDisposable
	{
		/// <summary>Запускает поток.</summary>
		void Start();

		/// <summary>Блокирует вызывающий поток, пока рабочий поток не завершит работу.</summary>
		void Join();
	}
}
