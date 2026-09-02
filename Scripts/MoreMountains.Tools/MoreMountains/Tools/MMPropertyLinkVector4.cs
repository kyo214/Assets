using System;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMPropertyLinkVector4 : MMPropertyLink
{
	public Func<Vector4> GetVector4Delegate;

	public Action<Vector4> SetVector4Delegate;

	protected Vector4 _initialValue;

	protected Vector4 _newValue;

	protected Vector4 _vector4;

	public override void Initialization(MMProperty property)
	{
		base.Initialization(property);
		_initialValue = (Vector4)GetPropertyValue(property);
	}

	public override void CreateGettersAndSetters(MMProperty property)
	{
		base.CreateGettersAndSetters(property);
		if (property.MemberType == MMProperty.MemberTypes.Property)
		{
			object firstArgument = ((property.TargetScriptableObject == null) ? ((UnityEngine.Object)property.TargetComponent) : ((UnityEngine.Object)property.TargetScriptableObject));
			if (property.MemberPropertyInfo.GetGetMethod() != null)
			{
				GetVector4Delegate = (Func<Vector4>)Delegate.CreateDelegate(typeof(Func<Vector4>), firstArgument, property.MemberPropertyInfo.GetGetMethod());
			}
			if (property.MemberPropertyInfo.GetSetMethod() != null)
			{
				SetVector4Delegate = (Action<Vector4>)Delegate.CreateDelegate(typeof(Action<Vector4>), firstArgument, property.MemberPropertyInfo.GetSetMethod());
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
		SetValueOptimized(property, (Vector4)newValue);
	}

	public override float GetLevel(MMPropertyEmitter emitter, MMProperty property)
	{
		_vector4 = GetValueOptimized(property);
		float num = 0f;
		switch (emitter.Vector4Option)
		{
		case MMPropertyEmitter.Vector4Options.X:
			num = _vector4.x;
			break;
		case MMPropertyEmitter.Vector4Options.Y:
			num = _vector4.y;
			break;
		case MMPropertyEmitter.Vector4Options.Z:
			num = _vector4.z;
			break;
		case MMPropertyEmitter.Vector4Options.W:
			num = _vector4.w;
			break;
		}
		float value = num;
		value = MMMaths.Clamp(value, emitter.FloatRemapMinToZero, emitter.FloatRemapMaxToOne, emitter.ClampMin, emitter.ClampMax);
		return emitter.Level = MMMaths.Remap(value, emitter.FloatRemapMinToZero, emitter.FloatRemapMaxToOne, 0f, 1f);
	}

	public override void SetLevel(MMPropertyReceiver receiver, MMProperty property, float level)
	{
		base.SetLevel(receiver, property, level);
		_newValue.x = (receiver.ModifyX ? MMMaths.Remap(level, 0f, 1f, receiver.Vector4RemapZero.x, receiver.Vector4RemapOne.x) : 0f);
		_newValue.y = (receiver.ModifyY ? MMMaths.Remap(level, 0f, 1f, receiver.Vector4RemapZero.y, receiver.Vector4RemapOne.y) : 0f);
		_newValue.z = (receiver.ModifyZ ? MMMaths.Remap(level, 0f, 1f, receiver.Vector4RemapZero.z, receiver.Vector4RemapOne.z) : 0f);
		_newValue.w = (receiver.ModifyW ? MMMaths.Remap(level, 0f, 1f, receiver.Vector4RemapZero.w, receiver.Vector4RemapOne.w) : 0f);
		if (receiver.RelativeValue)
		{
			_newValue = _initialValue + _newValue;
		}
		SetValueOptimized(property, _newValue);
	}

	protected virtual Vector4 GetValueOptimized(MMProperty property)
	{
		if (!_getterSetterInitialized)
		{
			return (Vector4)GetPropertyValue(property);
		}
		return GetVector4Delegate();
	}

	protected virtual void SetValueOptimized(MMProperty property, Vector4 newValue)
	{
		if (_getterSetterInitialized)
		{
			SetVector4Delegate(_newValue);
		}
		else
		{
			SetPropertyValue(property, _newValue);
		}
	}
}
