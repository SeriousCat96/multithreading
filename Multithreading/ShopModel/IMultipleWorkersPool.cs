using System;

namespace ShopModel
{
	/// <summary>Интерфейс пула обработчиков.</summary>
	/// <typeparam name="T">Тип обрабатываемого объекта.</typeparam>
	public interface IMultipleWorkersPool<T> : IDisposable
	{
		/// <summary>Добавляет объект в пул.</summary>
		/// <param name="workObject">Обрабатываемый объект.</param>
		void Enqueue(T workObject);
	}
}
