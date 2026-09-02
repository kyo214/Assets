using System;
using System.Reflection;

namespace BansheeGz.BGDatabase;

public static class BGPrivate
{
	public static T GetField<T>(object obj, string name)
	{
		return (T)GetField(obj, name).GetValue(obj);
	}

	public static void SetField<T>(object obj, string name, T value)
	{
		GetField(obj, name).SetValue(obj, value);
	}

	public static FieldInfo GetField(object obj, string name)
	{
		bool flag = obj is Type;
		Type type = (flag ? ((Type)obj) : obj.GetType());
		return GetField(type, name, flag);
	}

	public static FieldInfo GetField(Type type, string name, bool isStatic, bool includeBaseTypes = true)
	{
		BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		if (isStatic)
		{
			bindingFlags |= BindingFlags.Static;
		}
		FieldInfo field = type.GetField(name, bindingFlags);
		if (field != null)
		{
			return field;
		}
		if (!includeBaseTypes)
		{
			return null;
		}
		Type baseType = type.BaseType;
		Type typeFromHandle = typeof(object);
		while (field == null && baseType != null && baseType != typeFromHandle)
		{
			field = baseType.GetField(name, bindingFlags);
			baseType = baseType.BaseType;
		}
		return field;
	}

	public static T GetProperty<T>(object obj, string name)
	{
		return (T)GetProperty(obj, name).GetValue(obj, null);
	}

	public static void SetProperty<T>(object obj, string name, T value)
	{
		GetProperty(obj, name).SetValue(obj, value, null);
	}

	public static PropertyInfo GetProperty(object obj, string name)
	{
		bool flag = obj is Type;
		Type type = (flag ? ((Type)obj) : obj.GetType());
		return GetProperty(type, name, flag);
	}

	public static PropertyInfo GetProperty(Type type, string name, bool isStatic, bool includeBaseTypes = true)
	{
		BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		if (isStatic)
		{
			bindingFlags |= BindingFlags.Static;
		}
		PropertyInfo property = type.GetProperty(name, bindingFlags);
		if (property != null)
		{
			return property;
		}
		if (!includeBaseTypes)
		{
			return null;
		}
		Type baseType = type.BaseType;
		while (property == null && baseType != null && baseType != typeof(object))
		{
			property = baseType.GetProperty(name, bindingFlags);
			baseType = baseType.BaseType;
		}
		return property;
	}

	public static object Invoke(object obj, string methodName, params object[] parameters)
	{
		return GetMethod(obj, methodName).Invoke(obj, parameters);
	}

	public static object Invoke(object obj, string methodName, Type[] types, params object[] parameters)
	{
		return GetMethod(obj, methodName, types).Invoke(obj, parameters);
	}

	public static MethodInfo GetMethod(object obj, string name, Type[] types = null)
	{
		bool flag = obj is Type;
		Type type = (flag ? ((Type)obj) : obj.GetType());
		BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		if (flag)
		{
			bindingFlags |= BindingFlags.Static;
		}
		MethodInfo methodInfo = ((types == null) ? type.GetMethod(name, bindingFlags) : type.GetMethod(name, bindingFlags, null, types, null));
		if (methodInfo != null)
		{
			return methodInfo;
		}
		Type baseType = type.BaseType;
		while (methodInfo == null && baseType != null && baseType != typeof(object))
		{
			methodInfo = ((types == null) ? baseType.GetMethod(name, bindingFlags) : baseType.GetMethod(name, bindingFlags, null, types, null));
			baseType = baseType.BaseType;
		}
		return methodInfo;
	}
}
