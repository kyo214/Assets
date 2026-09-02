using System;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMObservableDemoObserver : MonoBehaviour
{
	public MMObservableDemoSubject TargetSubject;

	protected virtual void OnPositionChange()
	{
		base.transform.position = base.transform.position.MMSetY(TargetSubject.PositionX.Value);
	}

	protected virtual void OnEnable()
	{
		ref Action onValueChanged = ref TargetSubject.PositionX.OnValueChanged;
		onValueChanged = (Action)Delegate.Combine(onValueChanged, new Action(OnPositionChange));
	}

	protected virtual void OnDisable()
	{
		ref Action onValueChanged = ref TargetSubject.PositionX.OnValueChanged;
		onValueChanged = (Action)Delegate.Remove(onValueChanged, new Action(OnPositionChange));
	}
}
