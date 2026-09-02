using System.Collections.Generic;
using UnityEngine;

public class BakeOnPrefab : MonoBehaviour
{
	public MeshRenderer[] Meshes;

	public Texture2D[] LightmapColor;

	public Texture2D[] LightmapDir;

	public Vector4[] LightMapScaleOffset;

	[HideInInspector]
	public int[] LightmapIndex;

	[HideInInspector]
	public SetupAlternativeLighting SetupLighting;

	[HideInInspector]
	public int States = 2;

	private void Start()
	{
		SetupLighting = Object.FindFirstObjectByType<SetupAlternativeLighting>();
		if (SetupLighting != null)
		{
			SetupLighting.AddInfluencedPrefab(this);
		}
		else
		{
			Debug.LogError("SetupAlternativeLight not found! Please create one!");
		}
	}

	public int AssignMeshes()
	{
		Meshes = base.transform.GetComponentsInChildren<MeshRenderer>();
		List<MeshRenderer> list = new List<MeshRenderer>();
		for (int i = 0; i < Meshes.Length; i++)
		{
			if (Meshes[i].lightmapIndex >= 0)
			{
				list.Add(Meshes[i]);
			}
		}
		Meshes = list.ToArray();
		return Meshes.Length;
	}

	public void SwapState(int state)
	{
		for (int i = 0; i < Meshes.Length; i++)
		{
			int num = Meshes.Length * state + i;
			if (num < LightmapIndex.Length)
			{
				Meshes[i].lightmapIndex = LightmapIndex[num];
			}
			if (num < LightMapScaleOffset.Length)
			{
				Meshes[i].lightmapScaleOffset = LightMapScaleOffset[num];
			}
		}
	}

	public void TurnOff()
	{
		for (int i = 0; i < Meshes.Length; i++)
		{
			Meshes[i].lightmapIndex = -1;
		}
	}

	public void SetOcclussionMap()
	{
		for (int i = 0; i < Meshes.Length; i++)
		{
			Meshes[i].material.SetFloat("_OcclusionStrength", 0.25f);
		}
	}
}
