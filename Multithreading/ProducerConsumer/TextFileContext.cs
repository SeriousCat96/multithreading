using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer
{
	/// <summary>Текстовые данные.</summary>
	class TextFileContext
	{
		public string Text { get; }

		public TextFileContext(string text)
		{
			Text = text ?? throw new ArgumentNullException(text);
		}

		public override string ToString() => Text;
	}
}
