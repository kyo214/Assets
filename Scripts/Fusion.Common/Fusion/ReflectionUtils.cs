#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fusion;

public static class ReflectionUtils
{
	public static T GetCustomAttributeOrThrow<T>(this MemberInfo member, bool inherit) where T : Attribute
	{
		object[] customAttributes = member.GetCustomAttributes(typeof(T), inherit);
		if (customAttributes.Length == 0)
		{
			throw new ArgumentOutOfRangeException("T", $"{member} has no attribute {typeof(T)}");
		}
		if (customAttributes.Length > 1)
		{
			throw new InvalidOperationException($"{member} has more than one attribute {typeof(T)}");
		}
		return (T)customAttributes[0];
	}

	internal static Type[] GetTypesIgnoreErrors(this Assembly assembly)
	{
		try
		{
			return assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException ex)
		{
			Log.DebugWarn("Failed to load some types from assembly " + assembly.FullName + ". Still going to use the ones that were loaded. Error messages:\n" + string.Join("\n", ex.LoaderExceptions.Select((Exception x) => x.Message)));
			if (ex.Types == null)
			{
				return Array.Empty<Type>();
			}
			List<Type> list = new List<Type>();
			Type[] types = ex.Types;
			foreach (Type type in types)
			{
				if (type != null)
				{
					list.Add(type);
				}
			}
			return list.ToArray();
		}
	}
}
