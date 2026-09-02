using UnityEngine;

namespace DestroyIt;

public static class ParticleHelper
{
	private static Material[] GetNewMaterialsForDestroyedParticle(Renderer destMesh, Destructible destructibleObj)
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
			if (!(currentMat == null))
			{
				MaterialMapping materialMapping = destructibleObj.replaceParticleMats.Find((MaterialMapping x) => x.SourceMaterial == currentMat);
				array[i] = ((materialMapping == null) ? currentMat : materialMapping.ReplacementMaterial);
				if (destructibleObj.UseProgressiveDamage && (destructibleObj.damageLevels == null || destructibleObj.damageLevels.Count == 0))
				{
					destructibleObj.damageLevels = DestructibleHelper.DefaultDamageLevels();
				}
			}
		}
		return array;
	}
}
