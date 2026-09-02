#define DEBUG
using System;

namespace Fusion;

public class NetworkBehaviourCallbacks<T> where T : NetworkBehaviour
{
	public unsafe static NetworkBehaviourCallbackReference RegisterCallback(NetworkBehaviourCallbacks nbc, int offset, int count, int mask, ChangedDelegate<T> callback)
	{
		return nbc.RegisterCallback(offset, count, mask, (NetworkBehaviour behaviour, int* old) =>
		{
			Changed<T> changed = new Changed<T>((T)behaviour, old);
			callback(changed);
			return changed.ShouldRescan;
		});
	}
}
public class NetworkBehaviourCallbacks
{
	public unsafe delegate bool CallbackDelegate(NetworkBehaviour behaviour, int* old);

	private struct Callback
	{
		public int Mask;

		public int Count;

		public int Offset;

		public CallbackDelegate Delegate;
	}

	private const int MAX_CALLBACKS = 64;

	private Callback[] _callbacks;

	private int _callbacksCount;

	private ulong[] _words;

	public int Count => _callbacksCount;

	public NetworkBehaviourCallbacks(int words)
	{
		_callbacks = new Callback[64];
		_words = new ulong[words];
	}

	private unsafe bool InvokeCallback(int callback, NetworkBehaviour behaviour, int mask, int* old, ulong* invoked)
	{
		if ((*invoked & (ulong)(1L << callback)) == 0)
		{
			*invoked |= (ulong)(1L << callback);
			int* ptr = behaviour.Ptr;
			try
			{
				if ((_callbacks[callback].Mask & mask) != 0 && !BehaviourUtils.IsNull(behaviour.Object))
				{
					return _callbacks[callback].Delegate(behaviour, old);
				}
			}
			catch (Exception exn)
			{
				Log.Exception(exn);
			}
			finally
			{
				behaviour.Ptr = ptr;
			}
		}
		return false;
	}

	public unsafe bool Invoke(int word, NetworkBehaviour behaviour, int mask, int* old, ulong* invoked)
	{
		bool flag = false;
		int num = 0;
		if (word >= _words.Length)
		{
			return flag;
		}
		ulong num2 = _words[word];
		if (num2 == 0)
		{
			return flag;
		}
		int num3 = 0;
		while (true)
		{
			uint num4 = ((uint*)(&num2))[num3];
			if (num4 != 0)
			{
				int num5 = 0;
				while (true)
				{
					ushort num6 = ((ushort*)(&num4))[num5];
					if (num6 != 0)
					{
						int num7 = 0;
						while (true)
						{
							byte b = ((byte*)(&num6))[num7];
							if (b != 0)
							{
								if ((b & 1) == 1)
								{
									flag |= InvokeCallback(num, behaviour, mask, old, invoked);
								}
								if ((b & 2) == 2)
								{
									flag |= InvokeCallback(num + 1, behaviour, mask, old, invoked);
								}
								if ((b & 4) == 4)
								{
									flag |= InvokeCallback(num + 2, behaviour, mask, old, invoked);
								}
								if ((b & 8) == 8)
								{
									flag |= InvokeCallback(num + 3, behaviour, mask, old, invoked);
								}
								if ((b & 0x10) == 16)
								{
									flag |= InvokeCallback(num + 4, behaviour, mask, old, invoked);
								}
								if ((b & 0x20) == 32)
								{
									flag |= InvokeCallback(num + 5, behaviour, mask, old, invoked);
								}
								if ((b & 0x40) == 64)
								{
									flag |= InvokeCallback(num + 6, behaviour, mask, old, invoked);
								}
								if ((b & 0x80) == 128)
								{
									flag |= InvokeCallback(num + 7, behaviour, mask, old, invoked);
								}
							}
							num += 8;
							if (num7 == 0)
							{
								num7++;
								continue;
							}
							break;
						}
					}
					else
					{
						num += 16;
					}
					if (num5 == 0)
					{
						num5++;
						continue;
					}
					break;
				}
			}
			else
			{
				num += 32;
			}
			if (num3 != 0)
			{
				break;
			}
			num3++;
		}
		return flag;
	}

	public void ClearAll()
	{
		Array.Clear(_words, 0, _words.Length);
		Array.Clear(_callbacks, 0, _callbacks.Length);
	}

	public bool RemoveCallback(NetworkBehaviourCallbackReference reference)
	{
		int num = reference.IndexOffsetByOne - 1;
		if (num >= 0 && num < _callbacks.Length && reference.Delegate != null && _callbacks[num].Delegate == reference.Delegate)
		{
			Callback callback = _callbacks[num];
			for (int i = 0; i < callback.Count; i++)
			{
				_words[callback.Offset + i] &= (ulong)(~(1L << num));
			}
			_callbacks[num] = default;
			_callbacksCount--;
			Assert.Check(_callbacksCount >= 0);
			return true;
		}
		return false;
	}

	public NetworkBehaviourCallbackReference RegisterCallback(int offset, int count, int mask, CallbackDelegate callback)
	{
		Assert.Check(count > 0);
		for (int i = 0; i < _callbacks.Length; i++)
		{
			if (_callbacks[i].Delegate == null)
			{
				_callbacks[i].Mask = mask;
				_callbacks[i].Count = count;
				_callbacks[i].Offset = offset;
				_callbacks[i].Delegate = callback;
				_callbacksCount++;
				Assert.Check(_callbacksCount <= _callbacks.Length);
				for (int j = 0; j < count; j++)
				{
					_words[offset + j] |= (ulong)(1L << i);
				}
				return new NetworkBehaviourCallbackReference
				{
					IndexOffsetByOne = i + 1,
					Delegate = callback
				};
			}
		}
		throw new InvalidOperationException("Can only register 64 callbacks per behaviour");
	}
}
