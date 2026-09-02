using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion;

[ScriptHelp]
public abstract class Behaviour : MonoBehaviour
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T AddBehaviour<T>() where T : Behaviour
	{
		return base.gameObject.AddComponent<T>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetBehaviour<T>(out T behaviour) where T : Behaviour
	{
		return base.gameObject.TryGetComponent<T>(out behaviour);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T GetBehaviour<T>() where T : Behaviour
	{
		return base.gameObject.GetComponentInChildren<T>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DestroyBehaviour(Behaviour behaviour)
	{
		Object.Destroy(behaviour);
	}
}
