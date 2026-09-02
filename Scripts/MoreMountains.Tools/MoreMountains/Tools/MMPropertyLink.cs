using UnityEngine;

namespace MoreMountains.Tools;

public abstract class MMPropertyLink
{
	protected bool _getterSetterInitialized;

	public virtual void Initialization(MMProperty property)
	{
		CreateGettersAndSetters(property);
	}

	public virtual void CreateGettersAndSetters(MMProperty property)
	{
	}

	public virtual float GetLevel(MMPropertyEmitter emitter, MMProperty property)
	{
		return 0f;
	}

	public virtual void SetLevel(MMPropertyReceiver receiver, MMProperty property, float level)
	{
		receiver.Level = level;
	}

	public virtual object GetValue(MMPropertyEmitter emitter, MMProperty property)
	{
		return 0f;
	}

	public virtual void SetValue(MMPropertyReceiver receiver, MMProperty property, object newValue)
	{
	}

	public virtual object GetPropertyValue(MMProperty property)
	{
		object obj = ((property.TargetScriptableObject == null) ? ((Object)property.TargetComponent) : ((Object)property.TargetScriptableObject));
		if (property.MemberType == MMProperty.MemberTypes.Property)
		{
			return property.MemberPropertyInfo.GetValue(obj);
		}
		if (property.MemberType == MMProperty.MemberTypes.Field)
		{
			return property.MemberFieldInfo.GetValue(obj);
		}
		return 0f;
	}

	protected virtual void SetPropertyValue(MMProperty property, object newValue)
	{
		object obj = ((property.TargetScriptableObject == null) ? ((Object)property.TargetComponent) : ((Object)property.TargetScriptableObject));
		if (property.MemberType == MMProperty.MemberTypes.Property)
		{
			property.MemberPropertyInfo.SetValue(obj, newValue);
		}
		else if (property.MemberType == MMProperty.MemberTypes.Field)
		{
			property.MemberFieldInfo.SetValue(obj, newValue);
		}
	}
}
