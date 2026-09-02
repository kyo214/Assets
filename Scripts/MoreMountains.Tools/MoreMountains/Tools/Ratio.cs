using System;
using UnityEngine;

namespace MoreMountains.Tools;

[Serializable]
public class Ratio
{
	public bool DrawRatio = true;

	public Vector2 Size;

	public Color RatioColor;

	public Ratio(bool drawRatio, Vector2 size, Color ratioColor)
	{
		DrawRatio = drawRatio;
		Size = size;
		RatioColor = ratioColor;
	}
}
