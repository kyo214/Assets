using System.Runtime.InteropServices;

namespace MoreMountains.Feedbacks;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMF_PlayerEvent
{
	public enum EventTypes
	{
		Play = 0,
		Pause = 1,
		Resume = 2,
		Revert = 3,
		Complete = 4
	}

	public delegate void Delegate(MMF_Player source, EventTypes type);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(MMF_Player source, EventTypes type)
	{
		OnEvent?.Invoke(source, type);
	}
}
