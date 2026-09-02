using System;
using System.Collections.Generic;

namespace UnityEngine.TextCore.Text;

internal struct MaterialReference(int index, FontAsset fontAsset, SpriteAsset spriteAsset, Material material, float padding)
{
	public int index = index;

	public FontAsset fontAsset = fontAsset;

	public SpriteAsset spriteAsset = spriteAsset;

	public Material material = material;

	public bool isDefaultMaterial = material.GetInstanceID() == fontAsset.material.GetInstanceID();

	public bool isFallbackMaterial = false;

	public Material fallbackMaterial = null;

	public float padding = padding;

	public int referenceCount = 0;

	public static bool Contains(MaterialReference[] materialReferences, FontAsset fontAsset)
	{
		int instanceID = fontAsset.GetInstanceID();
		for (int i = 0; i < materialReferences.Length && materialReferences[i].fontAsset != null; i++)
		{
			if (materialReferences[i].fontAsset.GetInstanceID() == instanceID)
			{
				return true;
			}
		}
		return false;
	}

	public static int AddMaterialReference(Material material, FontAsset fontAsset, ref MaterialReference[] materialReferences, Dictionary<int, int> materialReferenceIndexLookup)
	{
		int instanceID = material.GetInstanceID();
		if (materialReferenceIndexLookup.TryGetValue(instanceID, out var value))
		{
			return value;
		}
		value = (materialReferenceIndexLookup[instanceID] = materialReferenceIndexLookup.Count);
		if (value >= materialReferences.Length)
		{
			Array.Resize(ref materialReferences, Mathf.NextPowerOfTwo(value + 1));
		}
		materialReferences[value].index = value;
		materialReferences[value].fontAsset = fontAsset;
		materialReferences[value].spriteAsset = null;
		materialReferences[value].material = material;
		materialReferences[value].isDefaultMaterial = instanceID == fontAsset.material.GetInstanceID();
		materialReferences[value].referenceCount = 0;
		return value;
	}

	public static int AddMaterialReference(Material material, SpriteAsset spriteAsset, ref MaterialReference[] materialReferences, Dictionary<int, int> materialReferenceIndexLookup)
	{
		int instanceID = material.GetInstanceID();
		if (materialReferenceIndexLookup.TryGetValue(instanceID, out var value))
		{
			return value;
		}
		value = (materialReferenceIndexLookup[instanceID] = materialReferenceIndexLookup.Count);
		if (value >= materialReferences.Length)
		{
			Array.Resize(ref materialReferences, Mathf.NextPowerOfTwo(value + 1));
		}
		materialReferences[value].index = value;
		materialReferences[value].fontAsset = materialReferences[0].fontAsset;
		materialReferences[value].spriteAsset = spriteAsset;
		materialReferences[value].material = material;
		materialReferences[value].isDefaultMaterial = true;
		materialReferences[value].referenceCount = 0;
		return value;
	}
}
