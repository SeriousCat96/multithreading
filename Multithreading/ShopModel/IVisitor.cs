namespace ShopModel
{
	/// <summary>Интерфейс посетителя.</summary>
	/// <typeparam name="T">Тип посещаемого объекта.</typeparam>
	public interface IVisitor<in T> : IThreadWorker
		where T : class
	{
		/// <summary>Выполнить визит.</summary>
		/// <param name="visitedObject">Посещаемый объект.</param>
		void Visit(T visitedObject);
	}
}
