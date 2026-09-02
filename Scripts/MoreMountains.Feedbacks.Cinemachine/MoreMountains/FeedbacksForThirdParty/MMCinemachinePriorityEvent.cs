using System.Runtime.InteropServices;
using Cinemachine;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMCinemachinePriorityEvent
{
	public delegate void Delegate(int channel, bool forceMaxPriority, int newPriority, bool forceTransition, CinemachineBlendDefinition blendDefinition, bool resetValuesAfterTransition, TimescaleModes timescaleMode);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(int channel, bool forceMaxPriority, int newPriority, bool forceTransition, CinemachineBlendDefinition blendDefinition, bool resetValuesAfterTransition, TimescaleModes timescaleMode)
	{
		OnEvent?.Invoke(channel, forceMaxPriority, newPriority, forceTransition, blendDefinition, resetValuesAfterTransition, timescaleMode);
	}
}
