using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StoreModeling
{
	class Program
	{
		static void Main(string[] args)
		{
			var timeModel = new TimeModel(new TimeSpan(hours: 8, minutes: 0, seconds: 0), 300);
			var queue     = new SemaphoreCashDeskModel(5);
			var store     = new Shop(queue, timeModel);

			try
			{
				queue.Start();
				store.Start();
				Console.ReadKey();
			}
			finally
			{
				queue.Join();
				store.Join();

				queue.Dispose();
				store.Dispose();
			}

			Console.ReadKey();
		}
	}
}
