using System;

namespace ProducerConsumer
{
	/// <summary>Интерфейс поставщика данных.</summary>
	/// <typeparam name="T">Тип поставляемых данных.</typeparam>
	public interface IProducer<out T> : IDisposable
	{
		/// <summary>Возвращает имя производителя.</summary>
		string Name { get; }

		/// <summary>Поставляет данные.</summary>
		/// <returns>Поставляемые данные.</returns>
		T Produce();

		void Start();

		void Join();
	}
}
