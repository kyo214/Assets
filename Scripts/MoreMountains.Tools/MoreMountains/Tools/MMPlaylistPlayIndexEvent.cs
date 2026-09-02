using System.Runtime.InteropServices;

namespace MoreMountains.Tools;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMPlaylistPlayIndexEvent
{
	public delegate void Delegate(int channel, int index);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(int channel, int index)
	{
		OnEvent?.Invoke(channel, index);
	}
}
