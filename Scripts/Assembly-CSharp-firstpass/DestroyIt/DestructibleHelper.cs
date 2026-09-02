using System.Collections.Generic;
using UnityEngine;

namespace DestroyIt;

public static class DestructibleHelper
{
	public static void TransferMaterials(Destructible oldObj, GameObject newObj)
	{
		if (oldObj == null)
		{
			return;
		}
		Renderer[] componentsInChildren = oldObj.GetComponentsInChildren<Renderer>();
		Renderer[] componentsInChildren2 = newObj.GetComponentsInChildren<Renderer>();
		if (componentsInChildren.Length == 0 || componentsInChildren2.Length == 0)
		{
			return;
		}
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			if (componentsInChildren2[i] is MeshRenderer || componentsInChildren2[i] is SkinnedMeshRenderer)
			{
				componentsInChildren2[i].materials = GetNewMaterialsForDestroyedMesh(componentsInChildren2[i], oldObj);
			}
		}
	}

	public static void LockHueVariation(this GameObject go)
	{
		if (go == null)
		{
			return;
		}
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		if (componentsInChildren.Length == 0)
		{
			return;
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			for (int j = 0; j < componentsInChildren[i].materials.Length; j++)
			{
				Material material = componentsInChildren[i].materials[j];
				if (material.HasProperty("_HueVariationPos"))
				{
					material.SetVector("_HueVariationPos", go.transform.position);
				}
			}
		}
	}

	private static Material[] GetNewMaterialsForDestroyedMesh(Renderer destMesh, Destructible destructibleObj)
	{
		if (destructibleObj == null)
		{
			return null;
		}
		Material[] sharedMaterials = destMesh.sharedMaterials;
		Material[] array = new Material[sharedMaterials.Length];
		for (int i = 0; i < sharedMaterials.Length; i++)
		{
			Material currentMat = sharedMaterials[i];
			if (currentMat == null)
			{
				continue;
			}
			MaterialMapping materialMapping = destructibleObj.replaceMaterials.Find((MaterialMapping x) => x.SourceMaterial == currentMat);
			array[i] = ((materialMapping == null) ? currentMat : materialMapping.ReplacementMaterial);
			if (destructibleObj.UseProgressiveDamage)
			{
				if (destructibleObj.damageLevels == null || destructibleObj.damageLevels.Count == 0)
				{
					destructibleObj.damageLevels = DefaultDamageLevels();
				}
				DestructionManager.Instance.SetProgressiveDamageTexture(destMesh, array[i], destructibleObj.damageLevels[destructibleObj.damageLevels.Count - 1]);
			}
		}
		return array;
	}

	public static void ReapplyImpactForce(ImpactDamage info, float velocityReduction)
	{
		if (!(info.ImpactObject == null) && !info.ImpactObject.isKinematic)
		{
			info.ImpactObject.velocity = Vector3.zero;
			info.ImpactObject.AddForce(info.ImpactObjectVelocityTo * velocityReduction, ForceMode.Impulse);
		}
	}

	public static List<DamageLevel> DefaultDamageLevels()
	{
		return new List<DamageLevel>
		{
			new DamageLevel
			{
				healthPercent = 100,
				visibleDamageLevel = 0
			},
			new DamageLevel
			{
				healthPercent = 80,
				visibleDamageLevel = 2
			},
			new DamageLevel
			{
				healthPercent = 60,
				visibleDamageLevel = 4
			},
			new DamageLevel
			{
				healthPercent = 40,
				visibleDamageLevel = 6
			},
			new DamageLevel
			{
				healthPercent = 20,
				visibleDamageLevel = 8
			}
		};
	}

	public static void SinkAndDestroy(Destructible destObj)
	{
		Rigidbody[] componentsInChildren = destObj.GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody obj in componentsInChildren)
		{
			obj.isKinematic = false;
			obj.WakeUp();
		}
		Collider[] componentsInChildren2 = destObj.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			componentsInChildren2[i].enabled = false;
		}
		destObj.gameObject.AddComponent<DestroyAfter>().seconds = 5f;
	}
}
