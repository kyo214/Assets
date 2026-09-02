using System.Collections.Generic;
using UnityEngine;

namespace DestroyIt;

public class DynamicDestructible : MonoBehaviour
{
	public GameObject objectToSpawn;

	public GameObject destroyedPrefab;

	public List<MaterialMapping> materialsToReplace;

	public void Start()
	{
		if (!(objectToSpawn != null))
		{
			return;
		}
		Destructible destructible = Object.Instantiate(objectToSpawn, base.transform, worldPositionStays: false).AddComponent<Destructible>();
		if (destroyedPrefab != null)
		{
			destructible.destroyedPrefab = destroyedPrefab;
			if (materialsToReplace != null && materialsToReplace.Count > 0)
			{
				destructible.replaceMaterials = materialsToReplace;
			}
		}
	}
}
