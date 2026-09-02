using System.Reflection;
using UnityEngine;

namespace MoreMountains.Tools;

public class MonoAttribute
{
	public enum MemberTypes
	{
		Property = 0,
		Field = 1
	}

	public MonoBehaviour TargetObject;

	public MemberTypes MemberType;

	public PropertyInfo MemberPropertyInfo;

	public FieldInfo MemberFieldInfo;

	public string MemberName;

	public MonoAttribute(MonoBehaviour targetObject, MemberTypes type, PropertyInfo propertyInfo, FieldInfo fieldInfo, string memberName)
	{
		TargetObject = targetObject;
		MemberType = type;
		MemberPropertyInfo = propertyInfo;
		MemberFieldInfo = fieldInfo;
		MemberName = memberName;
	}

	public virtual float GetValue()
	{
		if (MemberType == MemberTypes.Property)
		{
			return (float)MemberPropertyInfo.GetValue(TargetObject);
		}
		if (MemberType == MemberTypes.Field)
		{
			return (float)MemberFieldInfo.GetValue(TargetObject);
		}
		return 0f;
	}

	public virtual void SetValue(float newValue)
	{
		if (MemberType == MemberTypes.Property)
		{
			MemberPropertyInfo.SetValue(TargetObject, newValue);
		}
		else if (MemberType == MemberTypes.Field)
		{
			MemberFieldInfo.SetValue(TargetObject, newValue);
		}
	}
}
