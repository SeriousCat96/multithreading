namespace ShopModel
{
	/// <summary>Интерфейс работника.</summary>
	/// <typeparam name="T">Тип обрабатываемого объекта.</typeparam>
	public interface IWorker<in T>
	{
		string Name { get; }

		/// <summary>Выполнить работу.</summary>
		/// <param name="workObject">Обрабатываемый объект.</param>
		void Process(T workObject);
	}
}
