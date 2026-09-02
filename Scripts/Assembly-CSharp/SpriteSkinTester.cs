using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SpriteSkinTester : MonoBehaviour
{
	[Serializable]
	public class SkinPreset
	{
		public string skinName = "New Skin";

		[Header("Body Assets")]
		public SpriteLibraryAsset headAsset;

		public SpriteLibraryAsset bodyAsset;

		public SpriteLibraryAsset leftHandAsset;

		public SpriteLibraryAsset rightHandAsset;

		public SpriteLibraryAsset legAsset;
	}

	[Header("Component References")]
	public SpriteLibrary headLib;

	public SpriteLibrary bodyLib;

	public SpriteLibrary leftHandLib;

	public SpriteLibrary rightHandLib;

	public SpriteLibrary legLib;

	public SpriteLibrary meleeLib;

	public SpriteLibrary rangedLib;

	[Header("Skin Configuration")]
	public List<SkinPreset> skins = new List<SkinPreset>();

	public void ApplySkin(int index)
	{
		if (index >= 0 && index < skins.Count)
		{
			SkinPreset skinPreset = skins[index];
			if ((bool)headLib)
			{
				headLib.spriteLibraryAsset = skinPreset.headAsset;
			}
			if ((bool)bodyLib)
			{
				bodyLib.spriteLibraryAsset = skinPreset.bodyAsset;
			}
			if ((bool)leftHandLib)
			{
				leftHandLib.spriteLibraryAsset = skinPreset.leftHandAsset;
			}
			if ((bool)rightHandLib)
			{
				rightHandLib.spriteLibraryAsset = skinPreset.rightHandAsset;
			}
			if ((bool)legLib)
			{
				legLib.spriteLibraryAsset = skinPreset.legAsset;
			}
			Debug.Log("<color=cyan>Applied Skin:</color> " + skinPreset.skinName);
		}
	}

	public void AutoFindComponents()
	{
		headLib = FindLibrary("UpperBody/Head");
		bodyLib = FindLibrary("UpperBody/Body");
		leftHandLib = FindLibrary("UpperBody/LeftHand");
		rightHandLib = FindLibrary("UpperBody/RightHand");
		legLib = FindLibrary("LowerBody/Leg");
		meleeLib = FindLibrary("UpperBody/Weapon/Melee");
		rangedLib = FindLibrary("UpperBody/Weapon/Ranged");
	}

	private SpriteLibrary FindLibrary(string path)
	{
		Transform transform = base.transform.Find(path);
		if (transform != null)
		{
			return transform.GetComponent<SpriteLibrary>();
		}
		return null;
	}
}
