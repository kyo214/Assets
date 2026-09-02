using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Doozy.Runtime.Common.Utils;

public static class ReflectionUtils
{
	private static Assembly[] s_domainAssemblies;

	private static Assembly s_doozyEditorAssembly;

	private static Assembly s_doozyRuntimeAssembly;

	private static IEnumerable<Type> s_doozyRuntimeTypes;

	public static IEnumerable<Assembly> domainAssemblies => s_domainAssemblies ?? (s_domainAssemblies = AppDomain.CurrentDomain.GetAssemblies());

	public static Assembly doozyEditorAssembly
	{
		get
		{
			if (s_doozyEditorAssembly != null)
			{
				return s_doozyEditorAssembly;
			}
			foreach (Assembly domainAssembly in domainAssemblies)
			{
				if (domainAssembly.DefinedTypes.Any((TypeInfo typeInfo) => typeInfo.Namespace != null && typeInfo.Namespace.Contains("Doozy.Editor.")))
				{
					s_doozyEditorAssembly = domainAssembly;
					return s_doozyEditorAssembly;
				}
			}
			return s_doozyEditorAssembly;
		}
	}

	public static Assembly doozyRuntimeAssembly => s_doozyRuntimeAssembly ?? (s_doozyRuntimeAssembly = Assembly.GetAssembly(typeof(ReflectionUtils)));

	public static IEnumerable<Type> doozyRuntimeTypes => s_doozyRuntimeTypes ?? (s_doozyRuntimeTypes = doozyRuntimeAssembly.GetTypes());

	public static IEnumerable<Type> GetDerivedTypes(IEnumerable<Type> types, Type baseType)
	{
		List<Type> list = new List<Type>();
		foreach (Type type in types)
		{
			if (!(type.BaseType != baseType) && !type.IsAbstract)
			{
				list.Add(type);
			}
		}
		return list;
	}

	public static IEnumerable<Type> GetTypesThatImplementInterface<T>(Assembly fromAssembly)
	{
		return from p in fromAssembly.GetTypes()
			where typeof(T).IsAssignableFrom(p) && !p.IsInterface
			select p;
	}

	public static IEnumerable<Type> GetTypesThatImplementInterface<T>()
	{
		return from p in domainAssemblies.SelectMany((Assembly s) => s.GetTypes())
			where typeof(T).IsAssignableFrom(p) && !p.IsInterface
			select p;
	}

	public static IEnumerable<Type> GetDerivedTypes(Assembly assembly, Type baseType)
	{
		return GetDerivedTypes(assembly.GetTypes(), baseType);
	}

	public static IEnumerable<Type> GetDerivedTypes(Type baseType)
	{
		return GetDerivedTypes(Assembly.GetAssembly(baseType), baseType);
	}

	public static IEnumerable<T> GetAttributeReferences<T>(IEnumerable<Type> types) where T : Attribute
	{
		List<T> list = new List<T>();
		foreach (Type type in types)
		{
			MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < members.Length; i++)
			{
				T customAttribute = members[i].GetCustomAttribute<T>();
				if (customAttribute != null)
				{
					list.Add(customAttribute);
				}
			}
		}
		return list;
	}

	private static bool GetAttribute<T>(IEnumerable<object> attributes, out T attributeOut) where T : Attribute
	{
		foreach (object attribute in attributes)
		{
			if (!(attribute.GetType() != typeof(T)))
			{
				attributeOut = attribute as T;
				return true;
			}
		}
		attributeOut = null;
		return false;
	}

	public static bool GetAttribute<T>(Type classType, out T attributeOut) where T : Attribute
	{
		return GetAttribute<T>(classType.GetCustomAttributes(typeof(T), inherit: false), out attributeOut);
	}

	public static bool GetAttribute<T>(Type classType, string fieldName, out T attributeOut) where T : Attribute
	{
		return GetAttribute<T>(classType.GetField(fieldName).GetCustomAttributes(typeof(T), inherit: false), out attributeOut);
	}

	public static bool HasAttribute<T>(IEnumerable<object> attributes) where T : Attribute
	{
		return attributes.Any((object t) => t.GetType() == typeof(T));
	}

	public static bool IsCastableTo(this Type from, Type to)
	{
		if (to.IsAssignableFrom(from))
		{
			return true;
		}
		return (from m in @from.GetMethods(BindingFlags.Static | BindingFlags.Public)
			where m.ReturnType == to && (m.Name == "op_Implicit" || m.Name == "op_Explicit")
			select m).Any();
	}

	public static string PrettyName(this Type type)
	{
		if (type == null)
		{
			return "null";
		}
		if (type == typeof(object))
		{
			return "object";
		}
		if (type == typeof(float))
		{
			return "float";
		}
		if (type == typeof(int))
		{
			return "int";
		}
		if (type == typeof(long))
		{
			return "long";
		}
		if (type == typeof(double))
		{
			return "double";
		}
		if (type == typeof(string))
		{
			return "string";
		}
		if (type == typeof(bool))
		{
			return "bool";
		}
		if (type.IsGenericType)
		{
			string text = "";
			text = ((type.GetGenericTypeDefinition() == typeof(List<>)) ? "List" : type.GetGenericTypeDefinition().ToString());
			Type[] genericArguments = type.GetGenericArguments();
			string[] array = new string[genericArguments.Length];
			for (int i = 0; i < genericArguments.Length; i++)
			{
				array[i] = genericArguments[i].PrettyName();
			}
			return text + "<" + string.Join(", ", array) + ">";
		}
		if (!type.IsArray)
		{
			return type.ToString();
		}
		string text2 = "";
		for (int j = 1; j < type.GetArrayRank(); j++)
		{
			text2 += ",";
		}
		Type elementType = type.GetElementType();
		if ((object)elementType != null && !elementType.IsArray)
		{
			return elementType.PrettyName() + "[" + text2 + "]";
		}
		string text3 = elementType.PrettyName();
		int num = text3.IndexOf('[');
		return text3.Substring(0, num) + "[" + text2 + "]" + text3.Substring(num);
	}
}
