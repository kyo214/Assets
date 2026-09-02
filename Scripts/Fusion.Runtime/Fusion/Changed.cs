#define DEBUG
using System.Runtime.CompilerServices;

namespace Fusion;

public struct Changed<T> where T : NetworkBehaviour
{
	private unsafe int* _old;

	private unsafe int* _new;

	private bool _rescan;

	private T _behaviour;

	public T Behaviour
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _behaviour;
		}
	}

	internal bool ShouldRescan => _rescan;

	internal unsafe Changed(T behaviour, int* old)
	{
		Assert.Check(BehaviourUtils.IsAlive(behaviour));
		Assert.Check(behaviour.Ptr);
		Assert.Check(old);
		_behaviour = behaviour;
		_new = behaviour.Ptr;
		_old = old;
		_rescan = false;
	}

	public void Rescan()
	{
		_rescan = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void LoadOld()
	{
		_behaviour.Ptr = _old;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void LoadNew()
	{
		_behaviour.Ptr = _new;
	}
}
