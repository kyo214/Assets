using System;

namespace DestroyIt;

[Serializable]
public class ObjectToDestroy
{
	public string key;

	public Destructible[] destructibles;
}
