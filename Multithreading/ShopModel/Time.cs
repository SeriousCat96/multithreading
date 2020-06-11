using System;
using System.Collections.Generic;
using System.Threading;

namespace ShopModel
{
	/// <summary>Временная модель.</summary>
	public class Time : IComparer<TimeSpan>, IEqualityComparer<TimeSpan>
	{
		/// <summary>Возвращает текущую модель времени.</summary>
		public static Time Current { get; private set; } = new Time(TimeSpan.Zero, 60);

		private readonly TimeConverter timeConverter;
		private readonly TimeConverter sleepConverter;

		/// <summary>Возвращает текущее время модели.</summary>
		public TimeSpan Now => timeConverter.ConvertTime(DateTime.Now.TimeOfDay);

		private Time(TimeSpan initialTime, double factor)
		{
			timeConverter  = new TimeConverter(initialTime, DateTime.Now.TimeOfDay, factor);
			sleepConverter = new TimeConverter(TimeSpan.Zero, TimeSpan.Zero, 1 / factor);
		}

		/// <summary>Замораживает текущий поток на указанный промежуток времени.</summary>
		/// <param name="timeSpan">Промежуток времени в единицах измерения модели.</param>
		public void Sleep(TimeSpan timeSpan) => Thread.Sleep(sleepConverter.ConvertTime(timeSpan));

		/// <summary>Настраивает временную модель.</summary>
		/// <param name="initialTime">Точка отсчёта времени.</param>
		/// <param name="factor">Множитель ускорения времени.</param>
		public static void Setup(TimeSpan initialTime, double factor)
		{
			Current = new Time(initialTime, factor);
		}

		public int GetHashCode(TimeSpan obj) => (3600 * obj.Hours + 60 * obj.Hours + obj.Seconds).GetHashCode();

		public int Compare(TimeSpan x, TimeSpan y)
		{
			if(x.Hours >= y.Hours)
			{
				if(x.Minutes >= y.Minutes)
				{
					if(x.Seconds >= y.Seconds)
					{
						if(x.Seconds == y.Seconds) return 0;
						else return 1;
					}
					else return -1;
				}
				else return -1;
			}
			else return -1;
		}

		public bool Equals(TimeSpan x, TimeSpan y) => Compare(x, y) == 0;
	}
}
