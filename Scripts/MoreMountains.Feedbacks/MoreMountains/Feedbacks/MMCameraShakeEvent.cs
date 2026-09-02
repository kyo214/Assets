using System.Runtime.InteropServices;

namespace MoreMountains.Feedbacks;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMCameraShakeEvent
{
	public delegate void Delegate(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool infinite = false, int channel = 0, bool useUnscaledTime = false);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool infinite = false, int channel = 0, bool useUnscaledTime = false)
	{
		OnEvent?.Invoke(duration, amplitude, frequency, amplitudeX, amplitudeY, amplitudeZ, infinite, channel, useUnscaledTime);
	}
}
