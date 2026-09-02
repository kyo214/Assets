using System;
using UnityEngine;

namespace DestroyIt;

[Serializable]
public class PoolEntry
{
	public GameObject Prefab;

	public int Count;

	public bool OnlyPooled;
}
