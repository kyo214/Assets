using System;

namespace Unity.Services.Analytics.Internal;

internal interface IBufferSystemCalls
{
	string GenerateGuid();

	DateTime Now();

	TimeSpan GetTimeZoneUtcOffset(DateTime dateTime);
}
