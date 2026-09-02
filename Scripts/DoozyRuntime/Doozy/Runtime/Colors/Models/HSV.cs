using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Doozy.Runtime.Colors.Models;

[Serializable]
public struct HSV(float h, float s, float v)
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
	public struct V
	{
		public const float MIN = 0f;

		public const float MAX = 1f;

		public const int F = 100;
	}

	public float h = h;

	public float s = s;

	public float v = v;

	public HSV Copy()
	{
		return new HSV(h, s, v);
	}

	public Color ToColor(float alpha = 1f)
	{
		return ColorUtils.HSVtoRGB(this).Validate().ToColor();
	}

	public RGB ToRGB()
	{
		return ColorUtils.HSVtoRGB(this);
	}

	public HSV Validate()
	{
		h = ValidateColor(h, 0f, 1f);
		s = ValidateColor(s, 0f, 1f);
		v = ValidateColor(v, 0f, 1f);
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
			z = FactorizeColor(v, 0f, 1f, 100f)
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
			return $"hsv({h}, {s}%, {v}%)";
		}
		return $"hsv({Factorize().x}, {Factorize().y}%, {Factorize().z}%)";
	}
}
