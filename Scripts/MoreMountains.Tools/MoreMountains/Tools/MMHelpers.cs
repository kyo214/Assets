using System;
using System.Reflection;
using UnityEngine;

namespace MoreMountains.Tools;

public static class MMHelpers
{
	public static T CopyComponent<T>(T original, GameObject destination) where T : Component
	{
		Type type = original.GetType();
		T val = destination.GetComponent(type) as T;
		if (!val)
		{
			val = destination.AddComponent(type) as T;
		}
		FieldInfo[] fields = type.GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!fieldInfo.IsStatic)
			{
				fieldInfo.SetValue(val, fieldInfo.GetValue(original));
			}
		}
		PropertyInfo[] properties = type.GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.CanWrite && propertyInfo.CanWrite && !(propertyInfo.Name == "name"))
			{
				propertyInfo.SetValue(val, propertyInfo.GetValue(original, null), null);
			}
		}
		return val;
	}
}
