using System;

namespace ProducerConsumer
{
	/// <summary>Текстовые данные.</summary>
	class TextFileContext
	{
		/// <summary>Возвращает текстовые данные.</summary>
		public string Text { get; }

		/// <summary>Создание <see cref="TextFileContext"/>.</summary>
		/// <param name="text">Текстовые данные.</param>
		public TextFileContext(string text)
		{
			Text = text ?? throw new ArgumentNullException(text);
		}

		public override string ToString() => Text;
	}
}
