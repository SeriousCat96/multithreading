namespace ShopModel
{
	/// <summary>Интерфейс пула обработчиков.</summary>
	/// <typeparam name="T">Тип обрабатываемого объекта.</typeparam>
	public interface IMultipleWorkersPool<in T> : IThreadWorker
	{
		/// <summary>Добавляет объект в пул.</summary>
		/// <param name="workObject">Обрабатываемый объект.</param>
		void Enqueue(T workObject);
	}
}
