namespace ProducerConsumer
{
	/// <summary>Интерфейс потребителя данных.</summary>
	/// <typeparam name="T">Тип объекта потребления.</typeparam>
	public interface IConsumer<in T>
		where T : class
	{
		/// <summary>Возвращает имя потребителя.</summary>
		string Name { get; }

		/// <summary>Потребить объект производителя.</summary>
		/// <param name="consumableObject">Объект потребления.</param>
		void Consume(T consumableObject);
	}
}
