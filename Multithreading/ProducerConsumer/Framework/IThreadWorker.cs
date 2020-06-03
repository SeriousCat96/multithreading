using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
