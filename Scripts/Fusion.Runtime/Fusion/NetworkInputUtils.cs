#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fusion;

public static class NetworkInputUtils
{
	private static bool _initialized;

	private static Dictionary<Type, int> _wordCount;

	private static Dictionary<Type, int> _typeKey;

	public static void LoadTypes()
	{
		if (_initialized)
		{
			return;
		}
		_initialized = true;
		_wordCount = new Dictionary<Type, int>();
		_typeKey = new Dictionary<Type, int>();
		List<(Type, NetworkInputWeavedAttribute)> list = new List<(Type, NetworkInputWeavedAttribute)>();
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			if (assembly.GetCustomAttribute<NetworkAssemblyIgnoreAttribute>() != null)
			{
				continue;
			}
			try
			{
				Type[] typesIgnoreErrors = assembly.GetTypesIgnoreErrors();
				foreach (Type type in typesIgnoreErrors)
				{
					if (!type.IsValueType || !typeof(INetworkInput).IsAssignableFrom(type))
					{
						continue;
					}
					try
					{
						object[] customAttributes = type.GetCustomAttributes(typeof(NetworkInputWeavedAttribute), inherit: false);
						if (customAttributes.Length == 1 && customAttributes[0] is NetworkInputWeavedAttribute item)
						{
							list.Add((type, item));
						}
					}
					catch (Exception exn)
					{
						Log.Exception(exn);
					}
				}
			}
			catch
			{
			}
		}
		list.Sort(((Type, NetworkInputWeavedAttribute) a, (Type, NetworkInputWeavedAttribute) b) =>
		{
			int num2 = StringComparer.Ordinal.Compare(a.Item1.AssemblyQualifiedName, b.Item1.AssemblyQualifiedName);
			if (num2 == 0)
			{
				Assert.AlwaysFail("order == 0");
			}
			return num2;
		});
		for (int num = 0; num < list.Count; num++)
		{
			var (key, networkInputWeavedAttribute) = list[num];
			_typeKey.Add(key, num + 1);
			_wordCount.Add(key, networkInputWeavedAttribute.WordCount);
		}
	}

	public static int GetMaxWordCount()
	{
		LoadTypes();
		if (_wordCount.Count == 0)
		{
			return 0;
		}
		int num = _wordCount.Values.Max();
		foreach (KeyValuePair<Type, int> item in _wordCount)
		{
			Assert.Check(Native.SizeOf(item.Key) <= num * 4);
		}
		return num + 1;
	}

	public static int GetWordCount(Type type)
	{
		Assert.Check(typeof(INetworkInput).IsAssignableFrom(type));
		LoadTypes();
		if (_wordCount.TryGetValue(type, out var value))
		{
			return value;
		}
		Assert.AlwaysFail($"GetWordCount for {type}");
		return -1;
	}

	public static int GetTypeKey(Type type)
	{
		Assert.Check(typeof(INetworkInput).IsAssignableFrom(type));
		LoadTypes();
		if (_typeKey.TryGetValue(type, out var value))
		{
			return value;
		}
		Assert.AlwaysFail($"GetTypeKey for {type}");
		return -1;
	}

	internal static void ResetStatics()
	{
		_initialized = false;
		_wordCount = null;
		_typeKey = null;
	}
}
