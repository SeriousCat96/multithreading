using System;

namespace ProducerConsumer
{
	/// <summary>Интерфейс очереди объектов производителей для одного потребителя.</summary>
	/// <typeparam name="T">Тип объекта потребления.</typeparam>
	/// <remarks>Реализация модели Producer-Consumer.</remarks>
	public interface ISingleConsumerQueue<in T> : IDisposable
	{
		/// <summary>Добавляет объект потребления в очередь потребителя.</summary>
		/// <param name="consumableObject">Объект потребления.</param>
		void Enqueue(T consumableObject);

		void Start();

		void Join();
	}
}
