using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager;

[Serializable]
public class UIBehaviour : ModyEvent
{
	public enum Name
	{
		PointerEnter = 0,
		PointerExit = 1,
		PointerDown = 2,
		PointerUp = 3,
		PointerClick = 4,
		PointerDoubleClick = 5,
		PointerLongClick = 6,
		PointerLeftClick = 7,
		PointerMiddleClick = 8,
		PointerRightClick = 9,
		Selected = 10,
		Deselected = 11,
		Submit = 12
	}

	[SerializeField]
	private Name BehaviourName;

	[SerializeField]
	private SignalReceiver Receiver;

	[SerializeField]
	protected float Cooldown;

	[SerializeField]
	private UISelectable Selectable;

	private static List<string> s_behaviourNames;

	private static List<Name> s_behaviours;

	public Name behaviourName => BehaviourName;

	public SignalReceiver receiver => Receiver;

	public float cooldown => Cooldown;

	public UISelectable selectable => Selectable;

	public UIBehaviour()
		: this(Name.PointerClick, null)
	{
	}

	public UIBehaviour(Name behaviourName, GameObject target)
	{
		BehaviourName = behaviourName;
		EventName = behaviourName.ToString();
		Receiver = SignalReceiverExtensions.SetSignalSource(new SignalReceiver(), target).SetProviderId(GetProvideId(behaviourName));
	}

	public void Connect()
	{
		if (!receiver.isConnected)
		{
			receiver.Connect();
			if (receiver.isConnected)
			{
				receiver.providerReference.cooldown = cooldown;
				SignalReceiver signalReceiver = receiver;
				signalReceiver.onSignal = (UnityAction<Signal>)Delegate.Combine(signalReceiver.onSignal, new UnityAction<Signal>(Execute));
			}
		}
	}

	public void Disconnect()
	{
		if (receiver.isConnected)
		{
			receiver.Disconnect();
			SignalReceiver signalReceiver = receiver;
			signalReceiver.onSignal = (UnityAction<Signal>)Delegate.Remove(signalReceiver.onSignal, new UnityAction<Signal>(Execute));
		}
	}

	public UIBehaviour SetSelectable(UISelectable uiSelectable)
	{
		Selectable = uiSelectable;
		return this;
	}

	public UIBehaviour ClearSelectable()
	{
		return SetSelectable(null);
	}

	public override void Execute(Signal signal = null)
	{
		if (!(selectable != null) || (!(!selectable.IsActive() | !selectable.IsInteractable()) && !selectable.inCooldown))
		{
			Runners.RemoveNulls();
			Runners.ForEach((ModyActionRunner r) =>
			{
				r.Execute();
			});
			Event?.Invoke();
		}
	}

	public static IEnumerable<string> GetBehaviourNames()
	{
		return s_behaviourNames ?? (s_behaviourNames = (from Name name in Enum.GetValues(typeof(Name))
			select name.ToString()).ToList());
	}

	public static IEnumerable<Name> GetBehaviours()
	{
		return s_behaviours ?? (s_behaviours = Enum.GetValues(typeof(Name)).Cast<Name>().ToList());
	}

	public static ProviderId GetProvideId(Name behaviourName)
	{
		return behaviourName switch
		{
			Name.PointerEnter => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.Enter), 
			Name.PointerExit => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.Exit), 
			Name.PointerDown => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.Down), 
			Name.PointerUp => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.Up), 
			Name.PointerClick => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.Click), 
			Name.PointerDoubleClick => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.DoubleClick), 
			Name.PointerLongClick => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.LongClick), 
			Name.PointerLeftClick => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.LeftClick), 
			Name.PointerMiddleClick => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.MiddleClick), 
			Name.PointerRightClick => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.RightClick), 
			Name.Selected => SignalProvider.Local.UI.GetProviderId(SignalProvider.Local.UI.Name.Selected), 
			Name.Deselected => SignalProvider.Local.UI.GetProviderId(SignalProvider.Local.UI.Name.Deselected), 
			Name.Submit => SignalProvider.Local.UI.GetProviderId(SignalProvider.Local.UI.Name.Submit), 
			_ => throw new ArgumentOutOfRangeException("behaviourName", behaviourName, null), 
		};
	}
}
