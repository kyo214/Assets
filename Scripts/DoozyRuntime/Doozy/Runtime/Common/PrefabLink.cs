using System;
using UnityEngine;

namespace Doozy.Runtime.Common;

[Serializable]
public abstract class PrefabLink : ScriptableObject
{
	[SerializeField]
	private string PrefabName;

	[SerializeField]
	private GameObject Prefab;

	public string prefabName
	{
		get
		{
			return PrefabName;
		}
		protected set
		{
			PrefabName = value;
		}
	}

	public GameObject prefab
	{
		get
		{
			return Prefab;
		}
		protected set
		{
			Prefab = value;
		}
	}

	public bool hasPrefab => prefab != null;

	public bool hasPrefabName => !string.IsNullOrEmpty(prefabName);

	protected PrefabLink(GameObject prefab, string prefabName = null)
	{
		Prefab = prefab;
		PrefabName = prefabName;
	}

	public abstract void Validate();
}
