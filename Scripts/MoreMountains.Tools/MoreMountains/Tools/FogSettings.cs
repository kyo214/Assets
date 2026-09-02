using System;
using UnityEngine;

namespace MoreMountains.Tools;

[Serializable]
public class FogSettings
{
	public bool FogEnabled = true;

	public Color FogColor = Color.white;

	public float FogDensity = 0.01f;

	public FogMode FogMode = FogMode.ExponentialSquared;
}
