using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MoreMountains.Feedbacks;

public static class MMFeedbackStaticMethods
{
	private static List<Component> m_ComponentCache = new List<Component>();

	public static Component GetComponentNoAlloc(this GameObject @this, Type componentType)
	{
		@this.GetComponents(componentType, m_ComponentCache);
		Component result = ((m_ComponentCache.Count > 0) ? m_ComponentCache[0] : null);
		m_ComponentCache.Clear();
		return result;
	}

	public static Type MMFGetTypeByName(string name)
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			Type[] types = assemblies[i].GetTypes();
			foreach (Type type in types)
			{
				if (type.Name == name)
				{
					return type;
				}
			}
		}
		return null;
	}

	public static T MMFGetComponentNoAlloc<T>(this GameObject @this) where T : Component
	{
		@this.GetComponents(typeof(T), m_ComponentCache);
		Component obj = ((m_ComponentCache.Count > 0) ? m_ComponentCache[0] : null);
		m_ComponentCache.Clear();
		return obj as T;
	}
}
