using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Doozy.Runtime.Colors.Models;

[Serializable]
public struct HSL(float h, float s, float l)
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct H
	{
		public const float MIN = 0f;

		public const float MAX = 1f;

		public const int F = 360;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct S
	{
		public const float MIN = 0f;

		public const float MAX = 1f;

		public const int F = 100;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct L
	{
		public const float MIN = 0f;

		public const float MAX = 1f;

		public const int F = 100;
	}

	public float h = h;

	public float s = s;

	public float l = l;

	public HSL Copy()
	{
		return new HSL(h, s, l);
	}

	public Color ToColor(float alpha = 1f)
	{
		return ColorUtils.HSLtoRGB(this).Validate().ToColor();
	}

	public RGB ToRGB()
	{
		return ColorUtils.HSLtoRGB(this);
	}

	public HSL Validate()
	{
		h = ValidateColor(h, 0f, 1f);
		s = ValidateColor(s, 0f, 1f);
		l = ValidateColor(l, 0f, 1f);
		return this;
	}

	private float ValidateColor(float value, float min, float max)
	{
		return Mathf.Max(min, Mathf.Min(max, value));
	}

	public Vector3 Factorize()
	{
		return new Vector3
		{
			x = FactorizeColor(h, 0f, 1f, 360f),
			y = FactorizeColor(s, 0f, 1f, 100f),
			z = FactorizeColor(l, 0f, 1f, 100f)
		};
	}

	private int FactorizeColor(float value, float min, float max, float f)
	{
		return (int)Mathf.Max(min * f, Mathf.Min(max * f, Mathf.Round(value * f)));
	}

	public string ToString(bool factorize = false)
	{
		if (!factorize)
		{
			return $"hsl({h}, {s}%, {l}%)";
		}
		return $"hsl({Factorize().x}, {Factorize().y}%, {Factorize().z}%)";
	}
}
