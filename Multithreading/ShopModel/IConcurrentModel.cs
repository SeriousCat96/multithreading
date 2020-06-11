using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopModel
{
	public interface IConcurrentModel<T>
	{
		void AddVisitor(T visitor);
	}
}
