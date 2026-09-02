using System;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMPropertyLinkQuaternion : MMPropertyLink
{
	public Func<Quaternion> GetQuaternionDelegate;

	public Action<Quaternion> SetQuaternionDelegate;

	protected Quaternion _initialValue = Quaternion.identity;

	protected Quaternion _newValue;

	public override void Initialization(MMProperty property)
	{
		base.Initialization(property);
		_initialValue = (Quaternion)GetPropertyValue(property);
	}

	public override void CreateGettersAndSetters(MMProperty property)
	{
		base.CreateGettersAndSetters(property);
		if (property.MemberType == MMProperty.MemberTypes.Property)
		{
			object firstArgument = ((property.TargetScriptableObject == null) ? ((UnityEngine.Object)property.TargetComponent) : ((UnityEngine.Object)property.TargetScriptableObject));
			if (property.MemberPropertyInfo.GetGetMethod() != null)
			{
				GetQuaternionDelegate = (Func<Quaternion>)Delegate.CreateDelegate(typeof(Func<Quaternion>), firstArgument, property.MemberPropertyInfo.GetGetMethod());
			}
			if (property.MemberPropertyInfo.GetSetMethod() != null)
			{
				SetQuaternionDelegate = (Action<Quaternion>)Delegate.CreateDelegate(typeof(Action<Quaternion>), firstArgument, property.MemberPropertyInfo.GetSetMethod());
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
		SetValueOptimized(property, (Quaternion)newValue);
	}

	public override float GetLevel(MMPropertyEmitter emitter, MMProperty property)
	{
		float value = 0f;
		Quaternion valueOptimized = GetValueOptimized(property);
		switch (emitter.Vector3Option)
		{
		case MMPropertyEmitter.Vector3Options.X:
			value = valueOptimized.eulerAngles.x;
			break;
		case MMPropertyEmitter.Vector3Options.Y:
			value = valueOptimized.eulerAngles.y;
			break;
		case MMPropertyEmitter.Vector3Options.Z:
			value = valueOptimized.eulerAngles.z;
			break;
		}
		value = MMMaths.Clamp(value, emitter.QuaternionRemapMinToZero, emitter.QuaternionRemapMaxToOne, emitter.ClampMin, emitter.ClampMax);
		return emitter.Level = MMMaths.Remap(value, emitter.QuaternionRemapMinToZero, emitter.QuaternionRemapMaxToOne, 0f, 1f);
	}

	public override void SetLevel(MMPropertyReceiver receiver, MMProperty property, float level)
	{
		base.SetLevel(receiver, property, level);
		_newValue = (receiver.RelativeValue ? _initialValue : Quaternion.identity);
		if (receiver.ModifyX)
		{
			float angle = MMMaths.Remap(level, 0f, 1f, receiver.QuaternionRemapZero.x, receiver.QuaternionRemapOne.x);
			_newValue *= Quaternion.AngleAxis(angle, Vector3.right);
		}
		if (receiver.ModifyY)
		{
			float angle2 = MMMaths.Remap(level, 0f, 1f, receiver.QuaternionRemapZero.y, receiver.QuaternionRemapOne.y);
			_newValue *= Quaternion.AngleAxis(angle2, Vector3.up);
		}
		if (receiver.ModifyZ)
		{
			float angle3 = MMMaths.Remap(level, 0f, 1f, receiver.QuaternionRemapZero.z, receiver.QuaternionRemapOne.z);
			_newValue *= Quaternion.AngleAxis(angle3, Vector3.forward);
		}
		SetValueOptimized(property, _newValue);
	}

	protected virtual Quaternion GetValueOptimized(MMProperty property)
	{
		if (!_getterSetterInitialized)
		{
			return (Quaternion)GetPropertyValue(property);
		}
		return GetQuaternionDelegate();
	}

	protected virtual void SetValueOptimized(MMProperty property, Quaternion newValue)
	{
		if (_getterSetterInitialized)
		{
			SetQuaternionDelegate(_newValue);
		}
		else
		{
			SetPropertyValue(property, _newValue);
		}
	}
}
