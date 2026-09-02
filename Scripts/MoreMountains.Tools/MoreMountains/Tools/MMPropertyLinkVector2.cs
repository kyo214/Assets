using System;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMPropertyLinkVector2 : MMPropertyLink
{
	public Func<Vector2> GetVector2Delegate;

	public Action<Vector2> SetVector2Delegate;

	protected Vector2 _initialValue;

	protected Vector2 _newValue;

	protected Vector2 _vector2;

	public override void Initialization(MMProperty property)
	{
		base.Initialization(property);
		_initialValue = (Vector2)GetPropertyValue(property);
	}

	public override void CreateGettersAndSetters(MMProperty property)
	{
		base.CreateGettersAndSetters(property);
		if (property.MemberType == MMProperty.MemberTypes.Property)
		{
			object firstArgument = ((property.TargetScriptableObject == null) ? ((UnityEngine.Object)property.TargetComponent) : ((UnityEngine.Object)property.TargetScriptableObject));
			if (property.MemberPropertyInfo.GetGetMethod() != null)
			{
				GetVector2Delegate = (Func<Vector2>)Delegate.CreateDelegate(typeof(Func<Vector2>), firstArgument, property.MemberPropertyInfo.GetGetMethod());
			}
			if (property.MemberPropertyInfo.GetSetMethod() != null)
			{
				SetVector2Delegate = (Action<Vector2>)Delegate.CreateDelegate(typeof(Action<Vector2>), firstArgument, property.MemberPropertyInfo.GetSetMethod());
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
		SetValueOptimized(property, (Vector2)newValue);
	}

	public override float GetLevel(MMPropertyEmitter emitter, MMProperty property)
	{
		_vector2 = (_getterSetterInitialized ? GetVector2Delegate() : ((Vector2)GetPropertyValue(property)));
		float num = 0f;
		switch (emitter.Vector2Option)
		{
		case MMPropertyEmitter.Vector2Options.X:
			num = _vector2.x;
			break;
		case MMPropertyEmitter.Vector2Options.Y:
			num = _vector2.y;
			break;
		}
		float value = num;
		value = MMMaths.Clamp(value, emitter.FloatRemapMinToZero, emitter.FloatRemapMaxToOne, emitter.ClampMin, emitter.ClampMax);
		return emitter.Level = MMMaths.Remap(value, emitter.FloatRemapMinToZero, emitter.FloatRemapMaxToOne, 0f, 1f);
	}

	public override void SetLevel(MMPropertyReceiver receiver, MMProperty property, float level)
	{
		base.SetLevel(receiver, property, level);
		_newValue.x = (receiver.ModifyX ? MMMaths.Remap(level, 0f, 1f, receiver.Vector2RemapZero.x, receiver.Vector2RemapOne.x) : 0f);
		_newValue.y = (receiver.ModifyY ? MMMaths.Remap(level, 0f, 1f, receiver.Vector2RemapZero.y, receiver.Vector2RemapOne.y) : 0f);
		if (receiver.RelativeValue)
		{
			_newValue = _initialValue + _newValue;
		}
		if (_getterSetterInitialized)
		{
			SetVector2Delegate(_newValue);
		}
		else
		{
			SetPropertyValue(property, _newValue);
		}
	}

	protected virtual Vector2 GetValueOptimized(MMProperty property)
	{
		if (!_getterSetterInitialized)
		{
			return (Vector2)GetPropertyValue(property);
		}
		return GetVector2Delegate();
	}

	protected virtual void SetValueOptimized(MMProperty property, Vector2 newValue)
	{
		if (_getterSetterInitialized)
		{
			SetVector2Delegate(_newValue);
		}
		else
		{
			SetPropertyValue(property, _newValue);
		}
	}
}
