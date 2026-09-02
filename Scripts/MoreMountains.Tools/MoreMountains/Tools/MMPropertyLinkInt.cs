using System;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMPropertyLinkInt : MMPropertyLink
{
	public Func<int> GetIntDelegate;

	public Action<int> SetIntDelegate;

	protected int _initialValue;

	protected int _newValue;

	public override void Initialization(MMProperty property)
	{
		base.Initialization(property);
		_initialValue = (int)GetPropertyValue(property);
	}

	public override void CreateGettersAndSetters(MMProperty property)
	{
		base.CreateGettersAndSetters(property);
		if (property.MemberType == MMProperty.MemberTypes.Property)
		{
			object firstArgument = ((property.TargetScriptableObject == null) ? ((UnityEngine.Object)property.TargetComponent) : ((UnityEngine.Object)property.TargetScriptableObject));
			if (property.MemberPropertyInfo.GetGetMethod() != null)
			{
				GetIntDelegate = (Func<int>)Delegate.CreateDelegate(typeof(Func<int>), firstArgument, property.MemberPropertyInfo.GetGetMethod());
			}
			if (property.MemberPropertyInfo.GetSetMethod() != null)
			{
				SetIntDelegate = (Action<int>)Delegate.CreateDelegate(typeof(Action<int>), firstArgument, property.MemberPropertyInfo.GetSetMethod());
			}
			_getterSetterInitialized = true;
		}
	}

	public override object GetValue(MMPropertyEmitter emitter, MMProperty property)
	{
		return GetValueOptimized(property);
	}

	public override void SetValue(MMPropertyReceiver receiver, MMProperty property, object newValue)
	{
		SetValueOptimized(property, (int)newValue);
	}

	public override float GetLevel(MMPropertyEmitter emitter, MMProperty property)
	{
		float value = GetValueOptimized(property);
		value = MMMaths.Clamp(value, emitter.IntRemapMinToZero, emitter.IntRemapMaxToOne, emitter.ClampMin, emitter.ClampMax);
		return emitter.Level = MMMaths.Remap(value, emitter.IntRemapMinToZero, emitter.IntRemapMaxToOne, 0f, 1f);
	}

	public override void SetLevel(MMPropertyReceiver receiver, MMProperty property, float level)
	{
		base.SetLevel(receiver, property, level);
		_newValue = (int)MMMaths.Remap(level, 0f, 1f, receiver.IntRemapZero, receiver.IntRemapOne);
		if (receiver.RelativeValue)
		{
			_newValue = _initialValue + _newValue;
		}
		SetValueOptimized(property, _newValue);
	}

	protected virtual int GetValueOptimized(MMProperty property)
	{
		if (!_getterSetterInitialized)
		{
			return (int)GetPropertyValue(property);
		}
		return GetIntDelegate();
	}

	protected virtual void SetValueOptimized(MMProperty property, int newValue)
	{
		if (_getterSetterInitialized)
		{
			SetIntDelegate(_newValue);
		}
		else
		{
			SetPropertyValue(property, _newValue);
		}
	}
}
