using System;

namespace StoreModeling
{
	/// <summary>Конвертер времени.</summary>
	sealed class TimeConverter
	{
		/// <summary>Множитель времени.</summary>
		private readonly double factor;

		private readonly TimeSpan initialTime;
		private readonly TimeSpan startTime;

		/// <summary>Создаёт <see cref="TimeConverter"/>.</summary>
		/// <param name="startTime">Точка отсчёта времени.</param>
		/// <param name="initialTime">Начальное значение времени.</param>
		/// <param name="factor">Множитель времени.</param>
		public TimeConverter(TimeSpan startTime, TimeSpan initialTime, double factor)
		{
			this.factor      = factor > 0 ? factor : throw new ArgumentException("Множитель времени должен быть больше 0.", nameof(factor));
			this.initialTime = initialTime;
			this.startTime   = startTime;
		}

		public TimeSpan ConvertTime(TimeSpan timeSpan)
		{
			var offset = timeSpan.Ticks - initialTime.Ticks;
			return startTime.Add(TimeSpan.FromTicks(Convert.ToInt64(offset * factor)));
		}
	}
}
