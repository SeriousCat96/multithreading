using System;
using System.Threading;

namespace ShopModel
{
	/// <summary>Логгер.</summary>
	static class Log
	{
		private static readonly object syncRoot = new object();
		private const string MessageFormat      = @"[{0}] ({1}): {2}";
		private const string TimeFormat         = @"hh\:mm\:ss\.ffff";

		/// <summary>Отправить информационное сообщение в лог.</summary>
		/// <param name="message">Сообщение.</param>
		public static void Info(object message)
		{
			lock(syncRoot)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(MessageFormat, Time.Current.Now.ToString(TimeFormat), Thread.CurrentThread.Name, message);
				Console.ResetColor();
			}
		}

		/// <summary>Отправить предупреждение в лог.</summary>
		/// <param name="message">Сообщение.</param>
		public static void Warn(object message)
		{
			lock(syncRoot)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(MessageFormat, Time.Current.Now.ToString(TimeFormat), Thread.CurrentThread.Name, message);
				Console.ResetColor();
			}
		}

		/// <summary>Отправить отладочное сообщение в лог.</summary>
		/// <param name="message">Сообщение.</param>
		public static void Trace(object message)
		{
			lock(syncRoot)
			{
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine(MessageFormat, Time.Current.Now.ToString(TimeFormat), Thread.CurrentThread.Name, message);
				Console.ResetColor();
			}
		}
	}
}
