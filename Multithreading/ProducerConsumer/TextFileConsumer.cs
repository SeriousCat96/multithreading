using System;
using System.IO;
using System.Threading;

namespace ProducerConsumer
{
	/// <summary>Потребитель текстовых данных.</summary>
	class TextFileConsumer : IConsumer<TextFileContext>
	{
		/// <summary>Объект синхронизации записи в файл.</summary>
		private readonly object syncRoot;
		/// <summary>Генератор случайных чисел.</summary>
		private readonly Random random;
		/// <summary>Имя файла для записи данных.</summary>
		private readonly string fileName;

		public string Name => nameof(TextFileConsumer);

		/// <summary>Создание <see cref="TextFileConsumer"/>.</summary>
		/// <param name="fileName">Имя файла для записи данных.</param>
		public TextFileConsumer(string fileName)
		{
			this.fileName = !string.IsNullOrWhiteSpace(fileName) ? fileName : throw new ArgumentNullException(nameof(fileName));
			syncRoot      = new object();
			random        = new Random();

			// Создание файла
			if(!Directory.Exists(fileName))
			{
				File.Create(fileName).Dispose();
			}
		}

		/// <summary>Потребить объект производителя.</summary>
		/// <param name="context">Объект потребления.</param>
		public void Consume(TextFileContext context)
		{
			if(context == null) throw new ArgumentNullException(nameof(context));

			Monitor.Enter(syncRoot);

			WriteToFile(context);

			Monitor.Exit(syncRoot);
		}

		/// <summary>Записывает текстовые данные в файл.</summary>
		/// <param name="data">Текстовые данные.</param>
		private void WriteToFile(TextFileContext context)
		{
			Thread.Sleep(random.Next(0, 3000));
			Console.WriteLine($"({Thread.CurrentThread.Name}): Пишем данные в файл: \"{context}\".");

			using(var stream = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.Read))
			using(var writer = new StreamWriter(stream))
			{
				writer.WriteLine(context);
			}
		}
	}
}
