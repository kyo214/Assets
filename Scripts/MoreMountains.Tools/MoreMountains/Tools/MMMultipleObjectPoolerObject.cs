using System;
using UnityEngine;

namespace MoreMountains.Tools;

[Serializable]
public class MMMultipleObjectPoolerObject
{
	public GameObject GameObjectToPool;

	public int PoolSize;

	public bool PoolCanExpand = true;

	public bool Enabled = true;
}
