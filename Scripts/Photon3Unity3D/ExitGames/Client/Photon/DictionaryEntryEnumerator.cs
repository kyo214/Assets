using System;
using System.Collections;
using System.Collections.Generic;

namespace ExitGames.Client.Photon;

public struct DictionaryEntryEnumerator(Dictionary<object, object>.Enumerator original) : IEnumerator<DictionaryEntry>, IEnumerator, IDisposable
{
	private Dictionary<object, object>.Enumerator enumerator = original;

	object IEnumerator.Current => new DictionaryEntry(enumerator.Current.Key, enumerator.Current.Value);

	public DictionaryEntry Current => new DictionaryEntry(enumerator.Current.Key, enumerator.Current.Value);

	public object Key => enumerator.Current.Key;

	public object Value => enumerator.Current.Value;

	public bool MoveNext()
	{
		return enumerator.MoveNext();
	}

	public void Reset()
	{
		((IEnumerator)enumerator).Reset();
	}

	public void Dispose()
	{
	}
}
