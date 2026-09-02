using System.Collections;
using System.Collections.Generic;

namespace System.Xml.Xsl;

internal struct IListEnumerator<T>(IList<T> sequence) : IEnumerator<T>, IDisposable, IEnumerator
{
	private IList<T> sequence = sequence;

	private int index = 0;

	private T current = default;

	public T Current => current;

	object IEnumerator.Current
	{
		get
		{
			if (index == 0)
			{
				throw new InvalidOperationException(Res.GetString("Enumeration has not started. Call MoveNext.", string.Empty));
			}
			if (index > sequence.Count)
			{
				throw new InvalidOperationException(Res.GetString("Enumeration has already finished.", string.Empty));
			}
			return current;
		}
	}

	public void Dispose()
	{
	}

	public bool MoveNext()
	{
		if (index < sequence.Count)
		{
			current = sequence[index];
			index++;
			return true;
		}
		current = default;
		return false;
	}

	void IEnumerator.Reset()
	{
		index = 0;
		current = default;
	}
}
