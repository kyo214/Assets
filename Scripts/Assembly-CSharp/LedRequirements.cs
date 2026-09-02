using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LedRequirements
{
	public Image LedImage;

	public Transform[] Interactives;

	[HideInInspector]
	public int LedMap;
}
