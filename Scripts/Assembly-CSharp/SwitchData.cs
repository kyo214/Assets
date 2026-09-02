using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SwitchData
{
	public Image Switch;

	public Sprite[] AnimationFrames;

	[HideInInspector]
	public int Ampere;

	[HideInInspector]
	public int Volt;

	public int State;

	public bool IsPowerSwitch;
}
