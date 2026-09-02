using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion.KCC;

public static class KCCGameObjectExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T GetComponentNoAlloc<T>(this GameObject gameObject) where T : class
	{
		return gameObject.GetComponent<T>();
	}
}
