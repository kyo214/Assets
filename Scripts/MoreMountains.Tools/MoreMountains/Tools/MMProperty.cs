using System;
using System.Reflection;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMProperty
{
	public enum MemberTypes
	{
		Property = 0,
		Field = 1
	}

	public Component TargetComponent;

	public ScriptableObject TargetScriptableObject;

	public MemberTypes MemberType;

	public PropertyInfo MemberPropertyInfo;

	public FieldInfo MemberFieldInfo;

	public Type PropertyType;

	public string MemberName;

	public MMProperty(Component targetComponent, MemberTypes type, PropertyInfo propertyInfo, FieldInfo fieldInfo, string memberName, ScriptableObject targetScriptable)
	{
		TargetComponent = targetComponent;
		TargetScriptableObject = targetScriptable;
		MemberType = type;
		MemberPropertyInfo = propertyInfo;
		MemberFieldInfo = fieldInfo;
		MemberName = memberName;
	}

	public static MMProperty FindProperty(string propertyName, Component targetComponent, GameObject source, ScriptableObject scriptable)
	{
		FieldInfo fieldInfo = null;
		PropertyInfo propertyInfo = null;
		MMProperty mMProperty = null;
		if (scriptable == null)
		{
			propertyInfo = targetComponent.GetType().GetProperty(propertyName);
			if (propertyInfo == null)
			{
				fieldInfo = targetComponent.GetType().GetField(propertyName);
			}
		}
		else
		{
			fieldInfo = scriptable.GetType().GetField(propertyName);
		}
		if (propertyInfo != null)
		{
			mMProperty = new MMProperty(targetComponent, MemberTypes.Property, propertyInfo, null, propertyName, scriptable);
		}
		if (fieldInfo != null)
		{
			mMProperty = new MMProperty(targetComponent, MemberTypes.Field, null, fieldInfo, propertyName, scriptable);
		}
		if (propertyName == "")
		{
			if (source != null)
			{
				Debug.LogError("The MMProperty on " + source.name + " : you need to pick a property from the Property list");
			}
			return null;
		}
		if (propertyInfo == null && fieldInfo == null)
		{
			if (source != null)
			{
				Debug.LogError("The MMProperty on " + source.name + " couldn't find any property or field named " + propertyName + " on " + targetComponent.name);
			}
			return null;
		}
		if (scriptable == null)
		{
			if (mMProperty.MemberType == MemberTypes.Property)
			{
				mMProperty.MemberPropertyInfo = targetComponent.GetType().GetProperty(mMProperty.MemberName);
				mMProperty.PropertyType = mMProperty.MemberPropertyInfo.PropertyType;
			}
			else if (mMProperty.MemberType == MemberTypes.Field)
			{
				mMProperty.MemberFieldInfo = targetComponent.GetType().GetField(mMProperty.MemberName);
				mMProperty.PropertyType = mMProperty.MemberFieldInfo.FieldType;
			}
		}
		else
		{
			mMProperty.MemberFieldInfo = scriptable.GetType().GetField(mMProperty.MemberName);
			mMProperty.PropertyType = mMProperty.MemberFieldInfo.FieldType;
		}
		return mMProperty;
	}
}
