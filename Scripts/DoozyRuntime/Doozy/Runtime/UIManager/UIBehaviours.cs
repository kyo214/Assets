using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;

namespace Doozy.Runtime.UIManager;

[Serializable]
public class UIBehaviours
{
	[SerializeField]
	private List<UIBehaviour> Behaviours;

	[SerializeField]
	private GameObject SignalSource;

	[SerializeField]
	private UISelectable Selectable;

	public List<UIBehaviour> behaviours => Behaviours;

	public GameObject signalSource => SignalSource;

	public UISelectable selectable => Selectable;

	public UIBehaviours()
		: this(null)
	{
	}

	public UIBehaviours(GameObject signalSource)
	{
		SignalSource = signalSource;
		Behaviours = new List<UIBehaviour>();
	}

	public UIBehaviours Connect()
	{
		if (signalSource == null)
		{
			return this;
		}
		Behaviours.ForEach(ConnectBehaviour);
		return this;
	}

	public UIBehaviours Disconnect()
	{
		Behaviours.ForEach(DisconnectBehaviour);
		return this;
	}

	private void ConnectBehaviour(UIBehaviour behaviour)
	{
		if (behaviour != null)
		{
			behaviour.Disconnect();
			behaviour.SetSelectable(selectable).SetSignalSource(signalSource).Connect();
		}
	}

	private void DisconnectBehaviour(UIBehaviour behaviour)
	{
		behaviour?.Disconnect();
	}

	public UIBehaviour AddBehaviour(UIBehaviour.Name behaviourName)
	{
		if (HasBehaviour(behaviourName))
		{
			return GetBehaviour(behaviourName);
		}
		UIBehaviour uIBehaviour = new UIBehaviour(behaviourName, signalSource).SetSelectable(selectable);
		Behaviours.Add(uIBehaviour);
		if (Application.isPlaying)
		{
			ConnectBehaviour(uIBehaviour);
		}
		List<UIBehaviour> collection = (from UIBehaviour.Name name in Enum.GetValues(typeof(UIBehaviour.Name))
			select GetBehaviour(name) into b
			where b != null
			select b).ToList();
		Behaviours.Clear();
		Behaviours.AddRange(collection);
		return uIBehaviour;
	}

	public void RemoveBehaviour(UIBehaviour.Name behaviourName)
	{
		UIBehaviour behaviour = GetBehaviour(behaviourName);
		if (behaviour != null)
		{
			DisconnectBehaviour(behaviour);
			Behaviours.Remove(behaviour);
		}
	}

	public bool HasBehaviour(UIBehaviour.Name behaviourName)
	{
		return Behaviours.Any((UIBehaviour b) => b.behaviourName == behaviourName);
	}

	public UIBehaviour GetBehaviour(UIBehaviour.Name behaviourName)
	{
		return Behaviours.FirstOrDefault((UIBehaviour b) => b.behaviourName == behaviourName);
	}

	public UIBehaviours SetSignalSource(GameObject target)
	{
		SignalSource = target;
		foreach (UIBehaviour behaviour in Behaviours)
		{
			behaviour.SetSignalSource(target);
		}
		return this;
	}

	public UIBehaviours SetSelectable(UISelectable uiSelectable)
	{
		Selectable = uiSelectable;
		foreach (UIBehaviour behaviour in behaviours)
		{
			behaviour.SetSelectable(selectable);
		}
		return this;
	}

	public UIBehaviours ClearSelectable()
	{
		return SetSelectable(null);
	}
}
