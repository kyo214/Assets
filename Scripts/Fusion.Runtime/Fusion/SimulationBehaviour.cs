using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Fusion;

[ScriptHelp(BackColor = EditorHeaderBackColor.Green, Icon = EditorHeaderIcon.FusionGreen)]
[HelpURL("https://doc.photonengine.com/fusion/current/manual/network-object#simulationbehaviour")]
public abstract class SimulationBehaviour : Behaviour, ILogBuilder
{
	[NonSerialized]
	internal SimulationBehaviour Prev;

	[NonSerialized]
	internal SimulationBehaviour Next;

	[NonSerialized]
	internal SimulationBehaviourFlags Flags;

	[NonSerialized]
	public NetworkRunner Runner;

	[NonSerialized]
	public NetworkObject Object;

	public bool CanReceiveCallback
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (Flags & SimulationBehaviourFlags.PendingRemoval) == 0 && BehaviourUtils.IsAlive(this) && base.isActiveAndEnabled;
		}
	}

	public virtual bool HasInputAuthority
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return BehaviourUtils.IsAlive(Object) && Object.HasInputAuthority;
		}
	}

	public virtual bool HasStateAuthority
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return BehaviourUtils.IsAlive(Object) && Object.HasStateAuthority;
		}
	}

	public virtual bool IsProxy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return BehaviourUtils.IsAlive(Object) && Object.IsProxy;
		}
	}

	public virtual void FixedUpdateNetwork()
	{
	}

	public virtual void Render()
	{
	}

	[Conditional("DEBUG")]
	internal void DebugNotifySpawned()
	{
	}

	[Conditional("DEBUG")]
	internal void DebugNotifyDespawned()
	{
	}

	void ILogBuilder.BuildLogMessage(StringBuilder builder, string message, in LogOptions options)
	{
		builder.Append(BehaviourUtils.IsAlive(this) ? base.name : "(destroyed)");
		if (this is NetworkBehaviour { Id: { IsValid: not false } } networkBehaviour)
		{
			builder.Append(" ");
			builder.Append(networkBehaviour.Id);
		}
		int length = builder.Length;
		if (NetworkRunner.TryGetPrettyRunnerName(builder, Runner, in options))
		{
			builder.Insert(length, '@');
		}
		builder.Append(": ");
		builder.Append(message);
	}
}
