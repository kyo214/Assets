using System;

namespace Unity.Services.Analytics.Internal;

internal class BufferSystemCalls : IBufferSystemCalls
{
	public string GenerateGuid()
	{
		return Guid.NewGuid().ToString();
	}

	public DateTime Now()
	{
		return DateTime.Now;
	}

	public TimeSpan GetTimeZoneUtcOffset(DateTime dateTime)
	{
		return TimeZoneInfo.Local.GetUtcOffset(dateTime);
	}
}
