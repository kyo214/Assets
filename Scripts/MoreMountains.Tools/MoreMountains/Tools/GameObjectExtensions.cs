using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

public static class GameObjectExtensions
{
	private static List<Component> m_ComponentCache = new List<Component>();

	public static Component MMGetComponentNoAlloc(this GameObject @this, Type componentType)
	{
		@this.GetComponents(componentType, m_ComponentCache);
		Component result = ((m_ComponentCache.Count > 0) ? m_ComponentCache[0] : null);
		m_ComponentCache.Clear();
		return result;
	}

	public static T MMGetComponentNoAlloc<T>(this GameObject @this) where T : Component
	{
		@this.GetComponents(typeof(T), m_ComponentCache);
		Component obj = ((m_ComponentCache.Count > 0) ? m_ComponentCache[0] : null);
		m_ComponentCache.Clear();
		return obj as T;
	}

	public static T MMGetComponentAroundOrAdd<T>(this GameObject @this) where T : Component
	{
		T val = @this.GetComponentInChildren<T>(includeInactive: true);
		if (val == null)
		{
			val = @this.GetComponentInParent<T>();
		}
		if (val == null)
		{
			val = @this.AddComponent<T>();
		}
		return val;
	}
}
