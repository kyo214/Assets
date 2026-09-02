using System.Runtime.InteropServices;

namespace MoreMountains.Tools;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMDebugMenuSliderEvent
{
	public enum EventModes
	{
		FromSlider = 0,
		SetSlider = 1
	}

	public delegate void Delegate(string sliderEventName, float value, EventModes eventMode = EventModes.FromSlider);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(string sliderEventName, float value, EventModes eventMode = EventModes.FromSlider)
	{
		OnEvent?.Invoke(sliderEventName, value, eventMode);
	}
}
