namespace StoreModeling
{
	/// <summary></summary>
	/// <typeparam name="TVisitor">Тип данных посетителя.</typeparam>
	public interface ICashDeskModel<in TVisitor>
		where TVisitor : class
	{
		void ShopThreadProc(object state);

		void CustomerThreadProc(object state);
	}
}
