using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Cysharp.Threading.Tasks;

public static class TaskPool
{
	[CompilerGenerated]
	private sealed class _003CGetCacheSizeInfo_003Ed__4 : IEnumerable<(Type, int)>, IEnumerable, IEnumerator<(Type, int)>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private (Type, int) _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private Dictionary<Type, Func<int>> _003C_003E7__wrap1;

		private bool _003C_003E7__wrap2;

		private Dictionary<Type, Func<int>>.Enumerator _003C_003E7__wrap3;

		(Type, int) IEnumerator<(Type, int)>.Current
		{
			[DebuggerHidden]
			get
			{
				return System_002ECollections_002EGeneric_002EIEnumerator_003C_0028System_002EType_002CSystem_002EInt32_0029_003E_002Eget_Current();
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CGetCacheSizeInfo_003Ed__4(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = _003C_003E1__state;
			if ((uint)(num - -4) > 1u && num != 1)
			{
				return;
			}
			try
			{
				if (num != -4 && num != 1)
				{
					return;
				}
				try
				{
				}
				finally
				{
					_003C_003Em__Finally2();
				}
			}
			finally
			{
				_003C_003Em__Finally1();
			}
		}

		private bool MoveNext()
		{
			try
			{
				switch (_003C_003E1__state)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003C_003E7__wrap1 = sizes;
					_003C_003E7__wrap2 = false;
					_003C_003E1__state = -3;
					Monitor.Enter(_003C_003E7__wrap1, ref _003C_003E7__wrap2);
					_003C_003E7__wrap3 = sizes.GetEnumerator();
					_003C_003E1__state = -4;
					break;
				case 1:
					_003C_003E1__state = -4;
					break;
				}
				if (_003C_003E7__wrap3.MoveNext())
				{
					KeyValuePair<Type, Func<int>> current = _003C_003E7__wrap3.Current;
					_003C_003E2__current = (current.Key, current.Value());
					_003C_003E1__state = 1;
					return true;
				}
				_003C_003Em__Finally2();
				_003C_003E7__wrap3 = default;
				_003C_003Em__Finally1();
				_003C_003E7__wrap1 = null;
				return false;
			}
			catch
			{
				//try-fault
				((IDisposable)this).Dispose();
				throw;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void _003C_003Em__Finally1()
		{
			_003C_003E1__state = -1;
			if (_003C_003E7__wrap2)
			{
				Monitor.Exit(_003C_003E7__wrap1);
			}
		}

		private void _003C_003Em__Finally2()
		{
			_003C_003E1__state = -3;
			((IDisposable)_003C_003E7__wrap3/*cast due to constrained. prefix*/).Dispose();
		}

		[DebuggerHidden]
		private (Type, int) System_002ECollections_002EGeneric_002EIEnumerator_003C_0028System_002EType_002CSystem_002EInt32_0029_003E_002Eget_Current()
		{
			return _003C_003E2__current;
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<(Type, int)> IEnumerable<(Type, int)>.GetEnumerator()
		{
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				return this;
			}
			return new _003CGetCacheSizeInfo_003Ed__4(0);
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<(Type, int)>)this).GetEnumerator();
		}
	}

	internal static int MaxPoolSize;

	private static Dictionary<Type, Func<int>> sizes;

	static TaskPool()
	{
		sizes = new Dictionary<Type, Func<int>>();
		try
		{
			string environmentVariable = Environment.GetEnvironmentVariable("UNITASK_MAX_POOLSIZE");
			if (environmentVariable != null && int.TryParse(environmentVariable, out var result))
			{
				MaxPoolSize = result;
				return;
			}
		}
		catch
		{
		}
		MaxPoolSize = int.MaxValue;
	}

	public static void SetMaxPoolSize(int maxPoolSize)
	{
		MaxPoolSize = maxPoolSize;
	}

	[IteratorStateMachine(typeof(_003CGetCacheSizeInfo_003Ed__4))]
	public static IEnumerable<(Type, int)> GetCacheSizeInfo()
	{
		//yield-return decompiler failed: Could not find currentField
		return new _003CGetCacheSizeInfo_003Ed__4(-2);
	}

	public static void RegisterSizeGetter(Type type, Func<int> getSize)
	{
		lock (sizes)
		{
			sizes[type] = getSize;
		}
	}
}
[StructLayout(LayoutKind.Auto)]
public struct TaskPool<T> where T : class, ITaskPoolNode<T>
{
	private int gate;

	private int size;

	private T root;

	public int Size => size;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryPop(out T result)
	{
		if (Interlocked.CompareExchange(ref gate, 1, 0) == 0)
		{
			T val = root;
			if (val != null)
			{
				ref T nextNode = ref val.NextNode;
				root = nextNode;
				nextNode = null;
				size--;
				result = val;
				Volatile.Write(ref gate, 0);
				return true;
			}
			Volatile.Write(ref gate, 0);
		}
		result = null;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryPush(T item)
	{
		if (Interlocked.CompareExchange(ref gate, 1, 0) == 0)
		{
			if (size < TaskPool.MaxPoolSize)
			{
				item.NextNode = root;
				root = item;
				size++;
				Volatile.Write(ref gate, 0);
				return true;
			}
			Volatile.Write(ref gate, 0);
		}
		return false;
	}
}
