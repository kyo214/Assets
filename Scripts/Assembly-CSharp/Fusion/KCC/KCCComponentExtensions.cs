using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion.KCC;

public static class KCCComponentExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T GetComponentNoAlloc<T>(this Component component) where T : class
	{
		return component.GetComponent<T>();
	}
}
