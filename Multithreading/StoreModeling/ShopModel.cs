using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StoreModeling
{
	public class ShopModel
	{
		private readonly Semaphore visitors;
		private readonly Semaphore shops;
		private readonly Semaphore mutex;

		public ShopModel(int cashDeskCount, int shopsCount = 1)
		{

		}

		private void ShopThreadProc()
		{

		}

		private void CustomerThreadProc(object state)
		{

		}
	}
}
