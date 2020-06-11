using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreModeling
{
	public interface IVisitorFactory
	{
		IVisitor CreateVisitor();
	}
}
