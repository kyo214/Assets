namespace Doozy.Runtime.Reactor.Easings;

public static class Extensions
{
	public static float Evaluate(this IEasing easing, float startValue, float targetValue, float time)
	{
		return startValue + (targetValue - startValue) * easing.Evaluate(time);
	}
}
