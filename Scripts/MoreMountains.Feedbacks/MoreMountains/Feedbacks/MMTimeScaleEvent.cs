using System.Runtime.InteropServices;

namespace MoreMountains.Feedbacks;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMTimeScaleEvent
{
	public delegate void Delegate(MMTimeScaleMethods timeScaleMethod, float timeScale, float duration, bool lerp, float lerpSpeed, bool infinite);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(MMTimeScaleMethods timeScaleMethod, float timeScale, float duration, bool lerp, float lerpSpeed, bool infinite)
	{
		OnEvent?.Invoke(timeScaleMethod, timeScale, duration, lerp, lerpSpeed, infinite);
	}
}
