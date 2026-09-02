using System;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[Serializable]
public class MMFlashDebugSettings
{
	public int Channel;

	public Color FlashColor = Color.white;

	public float FlashDuration = 0.2f;

	public float FlashAlpha = 1f;

	public int FlashID;
}
