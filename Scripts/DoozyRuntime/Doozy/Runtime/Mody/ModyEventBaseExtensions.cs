namespace Doozy.Runtime.Mody;

public static class ModyEventBaseExtensions
{
	public static T SetEventName<T>(this T target, string eventName) where T : ModyEventBase
	{
		target.EventName = eventName;
		return target;
	}
}
