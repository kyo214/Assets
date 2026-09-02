using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public static class BGInterfaceFinder
{
	private static readonly Dictionary<Type, HashSet<Type>> Type2ComponentList = new Dictionary<Type, HashSet<Type>>();

	private static readonly List<Type> AllComponents = new List<Type>();

	private static readonly List<Type> Interfaces = new List<Type>(new Type[1] { typeof(BGAddonSaveLoad.BeforeSaveReciever) });

	private static bool inited;

	private static void Init()
	{
		if (inited)
		{
			return;
		}
		inited = true;
		AllComponents.Clear();
		AllComponents.AddRange(BGUtil.GetAllSubTypes(typeof(MonoBehaviour)));
		foreach (Type @interface in Interfaces)
		{
			Process(@interface);
		}
	}

	private static void Process(Type @interface)
	{
		HashSet<Type> hashSet = new HashSet<Type>();
		for (int i = 0; i < AllComponents.Count; i++)
		{
			Type type = AllComponents[i];
			if (@interface.IsAssignableFrom(type))
			{
				hashSet.Add(type);
			}
		}
		if (hashSet.Count != 0 && hashSet.Count > 0)
		{
			Type2ComponentList.Add(@interface, hashSet);
		}
	}

	public static void AddInterface(Type interfaceType)
	{
		if (!Interfaces.Contains(interfaceType))
		{
			Interfaces.Add(interfaceType);
			Process(interfaceType);
		}
	}

	public static List<T> FindObjects<T>(bool searchForInActive = false) where T : class
	{
		Init();
		Type typeFromHandle = typeof(T);
		if (!Type2ComponentList.ContainsKey(typeFromHandle))
		{
			return null;
		}
		HashSet<Type> hashSet = Type2ComponentList[typeFromHandle];
		if (hashSet.Count == 0)
		{
			return null;
		}
		List<T> list = new List<T>();
		foreach (Type item2 in hashSet)
		{
			UnityEngine.Object[] array = (searchForInActive ? Resources.FindObjectsOfTypeAll(item2) : UnityEngine.Object.FindObjectsOfType(item2));
			if (array == null || array.Length == 0)
			{
				continue;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] is T item)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}
}
