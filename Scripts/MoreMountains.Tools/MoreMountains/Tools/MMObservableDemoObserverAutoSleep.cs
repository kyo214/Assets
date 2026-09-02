using System;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMObservableDemoObserverAutoSleep : MonoBehaviour
{
	public MMObservableDemoSubject TargetSubject;

	protected virtual void OnSpeedChange()
	{
		base.transform.position = base.transform.position.MMSetY(TargetSubject.PositionX.Value);
	}

	protected virtual void Awake()
	{
		ref Action onValueChanged = ref TargetSubject.PositionX.OnValueChanged;
		onValueChanged = (Action)Delegate.Combine(onValueChanged, new Action(OnSpeedChange));
		base.enabled = false;
	}

	protected virtual void OnDestroy()
	{
		ref Action onValueChanged = ref TargetSubject.PositionX.OnValueChanged;
		onValueChanged = (Action)Delegate.Remove(onValueChanged, new Action(OnSpeedChange));
	}

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
	}
}
