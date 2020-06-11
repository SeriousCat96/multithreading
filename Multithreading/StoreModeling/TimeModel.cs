using System;
using System.Threading;

namespace StoreModeling
{
	/// <summary>Временная модель.</summary>
	class TimeModel
	{
		private readonly TimeConverter timeConverter;
		private readonly TimeConverter sleepConverter;

		public TimeModel(TimeSpan initialTime, double factor)
		{
			timeConverter  = new TimeConverter(initialTime, DateTime.Now.TimeOfDay, factor);
			sleepConverter = new TimeConverter(TimeSpan.Zero, TimeSpan.Zero, factor);
		}

		public void Sleep(TimeSpan timeSpan) => Thread.Sleep(sleepConverter.ConvertTime(timeSpan));

		public TimeSpan GetCurrentTime() => timeConverter.ConvertTime(DateTime.Now.TimeOfDay);
	}
}
