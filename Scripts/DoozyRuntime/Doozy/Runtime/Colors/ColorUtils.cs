using System;
using Doozy.Runtime.Colors.Models;
using UnityEngine;

namespace Doozy.Runtime.Colors;

public static class ColorUtils
{
	public static float Hue(float r, float g, float b, bool factorize = false)
	{
		if (r == g && g == b)
		{
			return 0f;
		}
		float num = 0f;
		if (r >= g && g >= b)
		{
			num = 60f * ((g - b) / (r - b));
		}
		else if (g > r && r >= b)
		{
			num = 60f * (2f - (r - b) / (g - b));
		}
		else if (g >= b && b > r)
		{
			num = 60f * (2f + (b - r) / (g - r));
		}
		else if (b > g && g > r)
		{
			num = 60f * (4f - (g - r) / (b - r));
		}
		else if (b > r && r >= g)
		{
			num = 60f * (4f + (r - g) / (b - g));
		}
		else if (r >= b && b > g)
		{
			num = 60f * (6f - (b - g) / (r - g));
		}
		if (factorize)
		{
			Mathf.RoundToInt(num);
		}
		return (float)Math.Round(num / 360f, 2);
	}

	public static float RGBtoHUE(RGB target, bool factorize = false)
	{
		return Hue(target.r, target.g, target.b, factorize);
	}

	public static RGB HUEtoRGB(float hue)
	{
		float r = Mathf.Abs(hue * 6f - 3f) - 1f;
		float g = 2f - Mathf.Abs(hue * 6f - 2f);
		float b = 2f - Mathf.Abs(hue * 6f - 4f);
		return new RGB(r, g, b);
	}

	public static Color RGBtoCOLOR(RGB rgb)
	{
		return new Color(rgb.r, rgb.g, rgb.g);
	}

	public static Color HSLtoCOLOR(HSL hsl)
	{
		return RGBtoCOLOR(hsl.ToRGB());
	}

	public static Color HSVtoCOLOR(HSV hsv)
	{
		return RGBtoCOLOR(hsv.ToRGB());
	}

	public static HSL RGBtoHSL(float r, float g, float b)
	{
		float num = Mathf.Max(r, g, b);
		float num2 = Mathf.Min(r, g, b);
		float num3 = num - num2;
		float h = 0f;
		float s = 0f;
		float num4 = (num + num2) / 2f;
		if (num3 == 0f)
		{
			return new HSL(h, s, num4).Validate();
		}
		h = Hue(r, g, b);
		s = ((num4 < 0.5f) ? (num3 / (num + num2)) : (num3 / (2f - num - num2)));
		return new HSL(h, s, num4).Validate();
	}

	public static HSL RGBtoHSL(RGB rgb)
	{
		return RGBtoHSL(rgb.r, rgb.g, rgb.b);
	}

	public static HSL COLORtoHSL(Color color)
	{
		return RGBtoHSL(color.r, color.g, color.b);
	}

	public static HSV RGBtoHSV(float r, float g, float b)
	{
		float num = Mathf.Max(r, g, b);
		float num2 = Mathf.Min(r, g, b);
		float num3 = num - num2;
		float h = 0f;
		float s = 0f;
		float v = num;
		if (num3 == 0f)
		{
			return new HSV(h, s, v).Validate();
		}
		h = Hue(r, g, b);
		s = num3 / num;
		return new HSV(h, s, v).Validate();
	}

	public static HSV RGBtoHSV(RGB value)
	{
		return RGBtoHSV(value.r, value.g, value.g);
	}

	public static HSV COLORtoHSV(Color color)
	{
		return RGBtoHSV(color.r, color.g, color.b);
	}

	public static RGB COLORtoRGB(Color color)
	{
		return new RGB(color.r, color.g, color.b);
	}

	public static RGB HSLtoRGB(HSL value)
	{
		HSL hSL = new HSL(value.h, value.s, value.l).Validate();
		float x = hSL.Factorize().x;
		float s = hSL.s;
		float l = hSL.l;
		float num = (1f - Mathf.Abs(2f * l - 1f)) * s;
		float num2 = num * (1f - Mathf.Abs(x / 60f % 2f - 1f));
		float num3 = l - num / 2f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		if (0f <= x && x < 60f)
		{
			num4 = num;
			num5 = num2;
			num6 = 0f;
		}
		else if (60f <= x && x < 120f)
		{
			num4 = num2;
			num5 = num;
			num6 = 0f;
		}
		else if (120f <= x && x < 180f)
		{
			num4 = 0f;
			num5 = num;
			num6 = num2;
		}
		else if (180f <= x && x < 240f)
		{
			num4 = 0f;
			num5 = num2;
			num6 = num;
		}
		else if (240f <= x && x < 300f)
		{
			num4 = num2;
			num5 = 0f;
			num6 = num;
		}
		else if (300f <= x && x < 360f)
		{
			num4 = num;
			num5 = 0f;
			num6 = num2;
		}
		return new RGB(num4 + num3, num5 + num3, num6 + num3).Validate();
	}

	public static RGB HSVtoRGB(HSV value)
	{
		HSV hSV = new HSV(value.h, value.s, value.v);
		float x = hSV.Factorize().x;
		float s = hSV.s;
		float v = hSV.v;
		float num = v * s;
		float num2 = num * (1f - Mathf.Abs(x / 60f % 2f - 1f));
		float num3 = v - num;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		if (0f <= x && x < 60f)
		{
			num4 = num;
			num5 = num2;
			num6 = 0f;
		}
		else if (60f <= x && x < 120f)
		{
			num4 = num2;
			num5 = num;
			num6 = 0f;
		}
		else if (120f <= x && x < 180f)
		{
			num4 = 0f;
			num5 = num;
			num6 = num2;
		}
		else if (180f <= x && x < 240f)
		{
			num4 = 0f;
			num5 = num2;
			num6 = num;
		}
		else if (240f <= x && x < 300f)
		{
			num4 = num2;
			num5 = 0f;
			num6 = num;
		}
		else if (300f <= x && x < 360f)
		{
			num4 = num;
			num5 = 0f;
			num6 = num2;
		}
		return new RGB(num4 + num3, num5 + num3, num6 + num3);
	}
}
