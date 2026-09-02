using System.Threading;

namespace Fusion;

public struct AtomicInt(int value)
{
	private volatile int _value = value;

	public int Value => Thread.VolatileRead(ref _value);

	public int IncrementPost()
	{
		return Interlocked.Increment(ref _value) - 1;
	}

	public int IncrementPre()
	{
		return Interlocked.Increment(ref _value);
	}

	public int Decrement()
	{
		return Interlocked.Decrement(ref _value);
	}

	public int Exchange(int value)
	{
		return Interlocked.Exchange(ref _value, value);
	}

	public int CompareExchange(int value, int assumed)
	{
		return Interlocked.CompareExchange(ref _value, value, assumed);
	}
}
