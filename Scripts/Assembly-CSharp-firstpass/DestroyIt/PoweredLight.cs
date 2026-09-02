using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DestroyIt;

public class PoweredLight : MonoBehaviour
{
	public PowerSource powerSource;

	public MeshRenderer emissiveMesh;

	public Material emissiveOffMaterial;

	private List<Light> lights;

	private Transform parent;

	private bool isPowered;

	private void Start()
	{
		isPowered = false;
		lights = base.gameObject.GetComponentsInChildren<Light>().ToList();
		if (lights.Count == 0)
		{
			Debug.Log("PoweredLight: No Light components found on [" + base.gameObject.name + "]. Removing script.");
			Object.Destroy(this);
		}
		parent = base.gameObject.transform.parent;
		if (parent == null)
		{
			Debug.Log("PoweredLight: No parent found for [" + base.gameObject.name + "]. Removing script.");
			Object.Destroy(this);
		}
	}

	private void Update()
	{
		lights.RemoveAll((Light x) => x == null);
		if (parent.gameObject.HasTag(Tag.Powered))
		{
			isPowered = true;
		}
		else
		{
			isPowered = false;
		}
		if (isPowered)
		{
			for (int num = 0; num < lights.Count; num++)
			{
				lights[num].enabled = true;
			}
			return;
		}
		for (int num2 = 0; num2 < lights.Count; num2++)
		{
			lights[num2].enabled = false;
		}
		emissiveMesh.material = emissiveOffMaterial;
	}
}
