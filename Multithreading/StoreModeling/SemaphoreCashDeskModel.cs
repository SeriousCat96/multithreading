using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace StoreModeling
{
	/// <summary>.</summary>
	class SemaphoreCashDeskModel : ICashDeskModel<IVisitor>
	{
		private readonly Semaphore customers;
		private readonly Semaphore stores;
		/// <summary>Мьютекс для ограничения доступа к разделяемым ресурсам.</summary>
		private readonly Semaphore mutex;
		private readonly Queue<IVisitor> visitorsQueue;

		private bool isWorking;

		private int waiting;

		private int customersCount;

		public string Name => nameof(SemaphoreCashDeskModel);

		public SemaphoreCashDeskModel(int customersCount)
		{
			visitorsQueue = new Queue<IVisitor>();

			mutex              = new Semaphore(1, 1);
			stores             = new Semaphore(1, 1);
			customers          = new Semaphore(customersCount, customersCount);
			this.customersCount = customersCount;
		}

		// shop
		public void ShopThreadProc(object state)
		{
			var shop = state as IShop<IVisitor>;
			while(true)
			{
				try
				{
					customers.WaitOne();
					mutex.WaitOne();
					waiting--;

					stores.Release();
				}
				finally
				{
					mutex.Release();
				}
			}
		}

		// visitors
		public void CustomerThreadProc(object state)
		{
			var customer = state as IVisitor;

			mutex.WaitOne();
			try
			{
				if(waiting < customersCount)
				{
					waiting++;
					customers.Release();

					stores.WaitOne();
				}
			}
			finally
			{
				mutex.Release();
			}

			customer.Buy();
		}
	}
}
