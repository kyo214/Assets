using System;
using Doozy.Runtime.Colors.Models;
using UnityEngine;

namespace Doozy.Runtime.Colors;

public static class ColorExtensions
{
	public static Color From256(this Color target, int r, int g, int b, float a = 1f)
	{
		target.r = (float)r / 256f;
		target.g = (float)g / 256f;
		target.b = (float)b / 256f;
		target.a = a;
		return target;
	}

	public static float MaxRGB(this Color target)
	{
		return Mathf.Max(target.r, target.g, target.b);
	}

	public static float MinRGB(this Color target)
	{
		return Mathf.Min(target.r, target.g, target.b);
	}

	public static float Luminosity(this Color target)
	{
		return (float)Math.Round(target.GetHSLLightness(), 2);
	}

	public static float Saturation(this Color target)
	{
		return (float)Math.Round(target.GetHSLSaturation(), 2);
	}

	public static float Hue(this Color target, bool factorize = false)
	{
		return ColorUtils.Hue(target.r, target.g, target.b, factorize);
	}

	public static float Alpha(this Color target)
	{
		return target.a;
	}

	public static Color WithAlpha(this Color target, float alpha)
	{
		target.a = Mathf.Clamp01(alpha);
		return target;
	}

	public static float GetHSLHue(this Color target, bool factorize = false)
	{
		if (!factorize)
		{
			return target.ToHSL().h;
		}
		return Mathf.Round(target.ToHSL().h * 360f);
	}

	public static Color SetHSLHue(this Color target, float hue)
	{
		HSL hSL = target.ToHSL();
		hSL.h = Mathf.Clamp01(hue);
		return hSL.ToColor();
	}

	public static Color SetHSLHue(this Color target, int factorizedHue)
	{
		HSL hSL = target.ToHSL();
		hSL.h = Mathf.Clamp(factorizedHue / 360, 0, 360);
		return hSL.ToColor();
	}

	public static float GetHSLSaturation(this Color target)
	{
		return target.ToHSL().s;
	}

	public static Color SetHSLSaturation(this Color target, float saturation)
	{
		HSL hSL = target.ToHSL();
		hSL.s = Mathf.Clamp01(saturation);
		return hSL.ToColor();
	}

	public static float GetHSLLightness(this Color target)
	{
		return target.ToHSL().l;
	}

	public static Color SetHSLLightness(this Color target, float lightness)
	{
		HSL hSL = target.ToHSL();
		hSL.l = Mathf.Clamp01(lightness);
		return hSL.ToColor();
	}

	public static float GetHSVHue(this Color target, bool factorize = false)
	{
		if (!factorize)
		{
			return target.ToHSV().h;
		}
		return Mathf.Round(target.ToHSV().h * 360f);
	}

	public static Color SetHSVHue(this Color target, float hue)
	{
		HSV hSV = target.ToHSV();
		hSV.h = Mathf.Clamp01(hue);
		return hSV.ToColor();
	}

	public static Color SetHSVHue(this Color target, int factorizedHue)
	{
		HSV hSV = target.ToHSV();
		hSV.h = Mathf.Clamp(factorizedHue / 360, 0, 360);
		return hSV.ToColor();
	}

	public static float GetHSVSaturation(this Color target)
	{
		return target.ToHSV().s;
	}

	public static Color SetHSVSaturation(this Color target, float saturation)
	{
		HSV hSV = target.ToHSV();
		hSV.s = Mathf.Clamp01(saturation);
		return hSV.ToColor();
	}

	public static float GetHSVValue(this Color target)
	{
		return target.ToHSV().v;
	}

	public static Color SetHSVValue(this Color target, float value)
	{
		HSV hSV = target.ToHSV();
		hSV.v = Mathf.Clamp01(value);
		return hSV.ToColor();
	}

	public static Color WithRGBShade(this Color target, float factor)
	{
		target.r *= 1f - factor;
		target.g *= 1f - factor;
		target.b *= 1f - factor;
		return target;
	}

	public static Color WithRGBTint(this Color target, float factor)
	{
		target.r += (1f - target.r) * factor;
		target.g += (1f - target.g) * factor;
		target.b += (1f - target.b) * factor;
		return target;
	}

	public static Color MixedWithColor(this Color target, Color other)
	{
		return new Color(target.r + (other.r - target.r) * other.a, target.g + (other.g - target.g) * other.a, target.b + (other.b - target.b) * other.a);
	}

	public static HSL ToHSL(this Color target)
	{
		return ColorUtils.COLORtoHSL(target);
	}

	public static HSV ToHSV(this Color target)
	{
		return ColorUtils.COLORtoHSV(target);
	}

	public static RGB ToRGB(this Color target)
	{
		return ColorUtils.COLORtoRGB(target);
	}
}
