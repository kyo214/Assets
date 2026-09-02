using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Doozy.Runtime.Colors.Models;

[Serializable]
public struct RGB(float r, float g, float b)
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct R
	{
		public const float MIN = 0f;

		public const float MAX = 1f;

		public const int F = 255;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct G
	{
		public const float MIN = 0f;

		public const float MAX = 1f;

		public const int F = 255;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct B
	{
		public const float MIN = 0f;

		public const float MAX = 1f;

		public const int F = 255;
	}

	public float r = r;

	public float g = g;

	public float b = b;

	public RGB Copy()
	{
		return new RGB(r, g, b);
	}

	public Color ToColor(float alpha = 1f)
	{
		return new Color(r, g, b, Mathf.Clamp(alpha, 0f, 1f));
	}

	public HSL ToHSL()
	{
		return ColorUtils.RGBtoHSL(this);
	}

	public HSV ToHSV()
	{
		return ColorUtils.RGBtoHSV(this);
	}

	public RGB Validate()
	{
		r = ValidateColor(r, 0f, 1f);
		g = ValidateColor(g, 0f, 1f);
		b = ValidateColor(b, 0f, 1f);
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
			x = FactorizeColor(r, 0f, 1f, 255f),
			y = FactorizeColor(g, 0f, 1f, 255f),
			z = FactorizeColor(b, 0f, 1f, 255f)
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
			return $"rgb({r}, {g}, {b})";
		}
		return $"rgb({Factorize().x}, {Factorize().y}, {Factorize().z})";
	}

	public string ToHEX(bool addHashTag = true)
	{
		return (addHashTag ? "#" : "") + ColorUtility.ToHtmlStringRGB(ToColor());
	}
}
