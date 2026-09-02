using System.Collections.Generic;
using UnityEngine;

namespace Fusion.KCC;

public static class GameObjectExtensions<T> where T : class
{
	private static List<T> _components = new List<T>();

	public static T GetComponentNoAlloc(GameObject gameObject)
	{
		_components.Clear();
		gameObject.GetComponents(_components);
		if (_components.Count > 0)
		{
			T result = _components[0];
			_components.Clear();
			return result;
		}
		return null;
	}
}
