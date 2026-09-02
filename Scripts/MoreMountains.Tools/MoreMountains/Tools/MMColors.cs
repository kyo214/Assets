using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

public static class MMColors
{
	public enum ColoringMode
	{
		Tint = 0,
		Multiply = 1,
		Replace = 2,
		ReplaceKeepAlpha = 3,
		Add = 4
	}

	public static readonly Color ReunoYellow = new Color32(byte.MaxValue, 196, 0, byte.MaxValue);

	public static readonly Color BestRed = new Color32(byte.MaxValue, 24, 0, byte.MaxValue);

	public static readonly Color AliceBlue = new Color32(240, 248, byte.MaxValue, byte.MaxValue);

	public static readonly Color AntiqueWhite = new Color32(250, 235, 215, byte.MaxValue);

	public static readonly Color Aqua = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color Aquamarine = new Color32(127, byte.MaxValue, 212, byte.MaxValue);

	public static readonly Color Azure = new Color32(240, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color Beige = new Color32(245, 245, 220, byte.MaxValue);

	public static readonly Color Bisque = new Color32(byte.MaxValue, 228, 196, byte.MaxValue);

	public static readonly Color Black = new Color32(0, 0, 0, byte.MaxValue);

	public static readonly Color BlanchedAlmond = new Color32(byte.MaxValue, 235, 205, byte.MaxValue);

	public static readonly Color Blue = new Color32(0, 0, byte.MaxValue, byte.MaxValue);

	public static readonly Color BlueViolet = new Color32(138, 43, 226, byte.MaxValue);

	public static readonly Color Brown = new Color32(165, 42, 42, byte.MaxValue);

	public static readonly Color Burlywood = new Color32(222, 184, 135, byte.MaxValue);

	public static readonly Color CadetBlue = new Color32(95, 158, 160, byte.MaxValue);

	public static readonly Color Chartreuse = new Color32(127, byte.MaxValue, 0, byte.MaxValue);

	public static readonly Color Chocolate = new Color32(210, 105, 30, byte.MaxValue);

	public static readonly Color Coral = new Color32(byte.MaxValue, 127, 80, byte.MaxValue);

	public static readonly Color CornflowerBlue = new Color32(100, 149, 237, byte.MaxValue);

	public static readonly Color Cornsilk = new Color32(byte.MaxValue, 248, 220, byte.MaxValue);

	public static readonly Color Crimson = new Color32(220, 20, 60, byte.MaxValue);

	public static readonly Color Cyan = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color DarkBlue = new Color32(0, 0, 139, byte.MaxValue);

	public static readonly Color DarkCyan = new Color32(0, 139, 139, byte.MaxValue);

	public static readonly Color DarkGoldenrod = new Color32(184, 134, 11, byte.MaxValue);

	public static readonly Color DarkGray = new Color32(169, 169, 169, byte.MaxValue);

	public static readonly Color DarkGreen = new Color32(0, 100, 0, byte.MaxValue);

	public static readonly Color DarkKhaki = new Color32(189, 183, 107, byte.MaxValue);

	public static readonly Color DarkMagenta = new Color32(139, 0, 139, byte.MaxValue);

	public static readonly Color DarkOliveGreen = new Color32(85, 107, 47, byte.MaxValue);

	public static readonly Color DarkOrange = new Color32(byte.MaxValue, 140, 0, byte.MaxValue);

	public static readonly Color DarkOrchid = new Color32(153, 50, 204, byte.MaxValue);

	public static readonly Color DarkRed = new Color32(139, 0, 0, byte.MaxValue);

	public static readonly Color DarkSalmon = new Color32(233, 150, 122, byte.MaxValue);

	public static readonly Color DarkSeaGreen = new Color32(143, 188, 143, byte.MaxValue);

	public static readonly Color DarkSlateBlue = new Color32(72, 61, 139, byte.MaxValue);

	public static readonly Color DarkSlateGray = new Color32(47, 79, 79, byte.MaxValue);

	public static readonly Color DarkTurquoise = new Color32(0, 206, 209, byte.MaxValue);

	public static readonly Color DarkViolet = new Color32(148, 0, 211, byte.MaxValue);

	public static readonly Color DeepPink = new Color32(byte.MaxValue, 20, 147, byte.MaxValue);

	public static readonly Color DeepSkyBlue = new Color32(0, 191, byte.MaxValue, byte.MaxValue);

	public static readonly Color DimGray = new Color32(105, 105, 105, byte.MaxValue);

	public static readonly Color DodgerBlue = new Color32(30, 144, byte.MaxValue, byte.MaxValue);

	public static readonly Color FireBrick = new Color32(178, 34, 34, byte.MaxValue);

	public static readonly Color FloralWhite = new Color32(byte.MaxValue, 250, 240, byte.MaxValue);

	public static readonly Color ForestGreen = new Color32(34, 139, 34, byte.MaxValue);

	public static readonly Color Fuchsia = new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);

	public static readonly Color Gainsboro = new Color32(220, 220, 220, byte.MaxValue);

	public static readonly Color GhostWhite = new Color32(248, 248, byte.MaxValue, byte.MaxValue);

	public static readonly Color Gold = new Color32(byte.MaxValue, 215, 0, byte.MaxValue);

	public static readonly Color Goldenrod = new Color32(218, 165, 32, byte.MaxValue);

	public static readonly Color Gray = new Color32(128, 128, 128, byte.MaxValue);

	public static readonly Color Green = new Color32(0, 128, 0, byte.MaxValue);

	public static readonly Color GreenYellow = new Color32(173, byte.MaxValue, 47, byte.MaxValue);

	public static readonly Color Honeydew = new Color32(240, byte.MaxValue, 240, byte.MaxValue);

	public static readonly Color HotPink = new Color32(byte.MaxValue, 105, 180, byte.MaxValue);

	public static readonly Color IndianRed = new Color32(205, 92, 92, byte.MaxValue);

	public static readonly Color Indigo = new Color32(75, 0, 130, byte.MaxValue);

	public static readonly Color Ivory = new Color32(byte.MaxValue, byte.MaxValue, 240, byte.MaxValue);

	public static readonly Color Khaki = new Color32(240, 230, 140, byte.MaxValue);

	public static readonly Color Lavender = new Color32(230, 230, 250, byte.MaxValue);

	public static readonly Color Lavenderblush = new Color32(byte.MaxValue, 240, 245, byte.MaxValue);

	public static readonly Color LawnGreen = new Color32(124, 252, 0, byte.MaxValue);

	public static readonly Color LemonChiffon = new Color32(byte.MaxValue, 250, 205, byte.MaxValue);

	public static readonly Color LightBlue = new Color32(173, 216, 230, byte.MaxValue);

	public static readonly Color LightCoral = new Color32(240, 128, 128, byte.MaxValue);

	public static readonly Color LightCyan = new Color32(224, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color LightGoldenodYellow = new Color32(250, 250, 210, byte.MaxValue);

	public static readonly Color LightGray = new Color32(211, 211, 211, byte.MaxValue);

	public static readonly Color LightGreen = new Color32(144, 238, 144, byte.MaxValue);

	public static readonly Color LightPink = new Color32(byte.MaxValue, 182, 193, byte.MaxValue);

	public static readonly Color LightSalmon = new Color32(byte.MaxValue, 160, 122, byte.MaxValue);

	public static readonly Color LightSeaGreen = new Color32(32, 178, 170, byte.MaxValue);

	public static readonly Color LightSkyBlue = new Color32(135, 206, 250, byte.MaxValue);

	public static readonly Color LightSlateGray = new Color32(119, 136, 153, byte.MaxValue);

	public static readonly Color LightSteelBlue = new Color32(176, 196, 222, byte.MaxValue);

	public static readonly Color LightYellow = new Color32(byte.MaxValue, byte.MaxValue, 224, byte.MaxValue);

	public static readonly Color Lime = new Color32(0, byte.MaxValue, 0, byte.MaxValue);

	public static readonly Color LimeGreen = new Color32(50, 205, 50, byte.MaxValue);

	public static readonly Color Linen = new Color32(250, 240, 230, byte.MaxValue);

	public static readonly Color Magenta = new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);

	public static readonly Color Maroon = new Color32(128, 0, 0, byte.MaxValue);

	public static readonly Color MediumAquamarine = new Color32(102, 205, 170, byte.MaxValue);

	public static readonly Color MediumBlue = new Color32(0, 0, 205, byte.MaxValue);

	public static readonly Color MediumOrchid = new Color32(186, 85, 211, byte.MaxValue);

	public static readonly Color MediumPurple = new Color32(147, 112, 219, byte.MaxValue);

	public static readonly Color MediumSeaGreen = new Color32(60, 179, 113, byte.MaxValue);

	public static readonly Color MediumSlateBlue = new Color32(123, 104, 238, byte.MaxValue);

	public static readonly Color MediumSpringGreen = new Color32(0, 250, 154, byte.MaxValue);

	public static readonly Color MediumTurquoise = new Color32(72, 209, 204, byte.MaxValue);

	public static readonly Color MediumVioletRed = new Color32(199, 21, 133, byte.MaxValue);

	public static readonly Color MidnightBlue = new Color32(25, 25, 112, byte.MaxValue);

	public static readonly Color Mintcream = new Color32(245, byte.MaxValue, 250, byte.MaxValue);

	public static readonly Color MistyRose = new Color32(byte.MaxValue, 228, 225, byte.MaxValue);

	public static readonly Color Moccasin = new Color32(byte.MaxValue, 228, 181, byte.MaxValue);

	public static readonly Color NavajoWhite = new Color32(byte.MaxValue, 222, 173, byte.MaxValue);

	public static readonly Color Navy = new Color32(0, 0, 128, byte.MaxValue);

	public static readonly Color OldLace = new Color32(253, 245, 230, byte.MaxValue);

	public static readonly Color Olive = new Color32(128, 128, 0, byte.MaxValue);

	public static readonly Color Olivedrab = new Color32(107, 142, 35, byte.MaxValue);

	public static readonly Color Orange = new Color32(byte.MaxValue, 165, 0, byte.MaxValue);

	public static readonly Color Orangered = new Color32(byte.MaxValue, 69, 0, byte.MaxValue);

	public static readonly Color Orchid = new Color32(218, 112, 214, byte.MaxValue);

	public static readonly Color PaleGoldenrod = new Color32(238, 232, 170, byte.MaxValue);

	public static readonly Color PaleGreen = new Color32(152, 251, 152, byte.MaxValue);

	public static readonly Color PaleTurquoise = new Color32(175, 238, 238, byte.MaxValue);

	public static readonly Color PaleVioletred = new Color32(219, 112, 147, byte.MaxValue);

	public static readonly Color PapayaWhip = new Color32(byte.MaxValue, 239, 213, byte.MaxValue);

	public static readonly Color PeachPuff = new Color32(byte.MaxValue, 218, 185, byte.MaxValue);

	public static readonly Color Peru = new Color32(205, 133, 63, byte.MaxValue);

	public static readonly Color Pink = new Color32(byte.MaxValue, 192, 203, byte.MaxValue);

	public static readonly Color Plum = new Color32(221, 160, 221, byte.MaxValue);

	public static readonly Color PowderBlue = new Color32(176, 224, 230, byte.MaxValue);

	public static readonly Color Purple = new Color32(128, 0, 128, byte.MaxValue);

	public static readonly Color Red = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);

	public static readonly Color RosyBrown = new Color32(188, 143, 143, byte.MaxValue);

	public static readonly Color RoyalBlue = new Color32(65, 105, 225, byte.MaxValue);

	public static readonly Color SaddleBrown = new Color32(139, 69, 19, byte.MaxValue);

	public static readonly Color Salmon = new Color32(250, 128, 114, byte.MaxValue);

	public static readonly Color SandyBrown = new Color32(244, 164, 96, byte.MaxValue);

	public static readonly Color SeaGreen = new Color32(46, 139, 87, byte.MaxValue);

	public static readonly Color Seashell = new Color32(byte.MaxValue, 245, 238, byte.MaxValue);

	public static readonly Color Sienna = new Color32(160, 82, 45, byte.MaxValue);

	public static readonly Color Silver = new Color32(192, 192, 192, byte.MaxValue);

	public static readonly Color SkyBlue = new Color32(135, 206, 235, byte.MaxValue);

	public static readonly Color SlateBlue = new Color32(106, 90, 205, byte.MaxValue);

	public static readonly Color SlateGray = new Color32(112, 128, 144, byte.MaxValue);

	public static readonly Color Snow = new Color32(byte.MaxValue, 250, 250, byte.MaxValue);

	public static readonly Color SpringGreen = new Color32(0, byte.MaxValue, 127, byte.MaxValue);

	public static readonly Color SteelBlue = new Color32(70, 130, 180, byte.MaxValue);

	public static readonly Color Tan = new Color32(210, 180, 140, byte.MaxValue);

	public static readonly Color Teal = new Color32(0, 128, 128, byte.MaxValue);

	public static readonly Color Thistle = new Color32(216, 191, 216, byte.MaxValue);

	public static readonly Color Tomato = new Color32(byte.MaxValue, 99, 71, byte.MaxValue);

	public static readonly Color Turquoise = new Color32(64, 224, 208, byte.MaxValue);

	public static readonly Color Violet = new Color32(238, 130, 238, byte.MaxValue);

	public static readonly Color Wheat = new Color32(245, 222, 179, byte.MaxValue);

	public static readonly Color White = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color WhiteSmoke = new Color32(245, 245, 245, byte.MaxValue);

	public static readonly Color Yellow = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);

	public static readonly Color YellowGreen = new Color32(154, 205, 50, byte.MaxValue);

	public static Dictionary<int, Color> ColorDictionary;

	public static Color RandomColor()
	{
		return GetColorAt(Random.Range(0, 140));
	}

	public static Color GetColorAt(int index)
	{
		if (ColorDictionary == null)
		{
			InitializeDictionary();
		}
		if (index < ColorDictionary.Count)
		{
			return ColorDictionary[index];
		}
		return Color.white;
	}

	public static void InitializeDictionary()
	{
		ColorDictionary = new Dictionary<int, Color>
		{
			{ 0, AliceBlue },
			{ 1, AntiqueWhite },
			{ 2, Aqua },
			{ 3, Aquamarine },
			{ 4, Azure },
			{ 5, Beige },
			{ 6, Bisque },
			{ 7, Black },
			{ 8, BlanchedAlmond },
			{ 9, Blue },
			{ 10, BlueViolet },
			{ 11, Brown },
			{ 12, Burlywood },
			{ 13, CadetBlue },
			{ 14, Chartreuse },
			{ 15, Chocolate },
			{ 16, Coral },
			{ 17, CornflowerBlue },
			{ 18, Cornsilk },
			{ 19, Crimson },
			{ 20, Cyan },
			{ 21, DarkBlue },
			{ 22, DarkCyan },
			{ 23, DarkGoldenrod },
			{ 24, DarkGray },
			{ 25, DarkGreen },
			{ 26, DarkKhaki },
			{ 27, DarkMagenta },
			{ 28, DarkOliveGreen },
			{ 29, DarkOrange },
			{ 30, DarkOrchid },
			{ 31, DarkRed },
			{ 32, DarkSalmon },
			{ 33, DarkSeaGreen },
			{ 34, DarkSlateBlue },
			{ 35, DarkSlateGray },
			{ 36, DarkTurquoise },
			{ 37, DarkViolet },
			{ 38, DeepPink },
			{ 39, DeepSkyBlue },
			{ 40, DimGray },
			{ 41, DodgerBlue },
			{ 42, FireBrick },
			{ 43, FloralWhite },
			{ 44, ForestGreen },
			{ 45, Fuchsia },
			{ 46, Gainsboro },
			{ 47, GhostWhite },
			{ 48, Gold },
			{ 49, Goldenrod },
			{ 50, Gray },
			{ 51, Green },
			{ 52, GreenYellow },
			{ 53, Honeydew },
			{ 54, HotPink },
			{ 55, IndianRed },
			{ 56, Indigo },
			{ 57, Ivory },
			{ 58, Khaki },
			{ 59, Lavender },
			{ 60, Lavenderblush },
			{ 61, LawnGreen },
			{ 62, LemonChiffon },
			{ 63, LightBlue },
			{ 64, LightCoral },
			{ 65, LightCyan },
			{ 66, LightGoldenodYellow },
			{ 67, LightGray },
			{ 68, LightGreen },
			{ 69, LightPink },
			{ 70, LightSalmon },
			{ 71, LightSeaGreen },
			{ 72, LightSkyBlue },
			{ 73, LightSlateGray },
			{ 74, LightSteelBlue },
			{ 75, LightYellow },
			{ 76, Lime },
			{ 77, LimeGreen },
			{ 78, Linen },
			{ 79, Magenta },
			{ 80, Maroon },
			{ 81, MediumAquamarine },
			{ 82, MediumBlue },
			{ 83, MediumOrchid },
			{ 84, MediumPurple },
			{ 85, MediumSeaGreen },
			{ 86, MediumSlateBlue },
			{ 87, MediumSpringGreen },
			{ 88, MediumTurquoise },
			{ 89, MediumVioletRed },
			{ 90, MidnightBlue },
			{ 91, Mintcream },
			{ 92, MistyRose },
			{ 93, Moccasin },
			{ 94, NavajoWhite },
			{ 95, Navy },
			{ 96, OldLace },
			{ 97, Olive },
			{ 98, Olivedrab },
			{ 99, Orange },
			{ 100, Orangered },
			{ 101, Orchid },
			{ 102, PaleGoldenrod },
			{ 103, PaleGreen },
			{ 104, PaleTurquoise },
			{ 105, PaleVioletred },
			{ 106, PapayaWhip },
			{ 107, PeachPuff },
			{ 108, Peru },
			{ 109, Pink },
			{ 110, Plum },
			{ 111, PowderBlue },
			{ 112, Purple },
			{ 113, Red },
			{ 114, RosyBrown },
			{ 115, RoyalBlue },
			{ 116, SaddleBrown },
			{ 117, Salmon },
			{ 118, SandyBrown },
			{ 119, SeaGreen },
			{ 120, Seashell },
			{ 121, Sienna },
			{ 122, Silver },
			{ 123, SkyBlue },
			{ 124, SlateBlue },
			{ 125, SlateGray },
			{ 126, Snow },
			{ 127, SpringGreen },
			{ 128, SteelBlue },
			{ 129, Tan },
			{ 130, Teal },
			{ 131, Thistle },
			{ 132, Tomato },
			{ 133, Turquoise },
			{ 134, Violet },
			{ 135, Wheat },
			{ 136, White },
			{ 137, WhiteSmoke },
			{ 138, Yellow },
			{ 139, YellowGreen },
			{ 140, ReunoYellow },
			{ 141, BestRed }
		};
	}

	public static Color MMRandomColor(this Color color, Color min, Color max)
	{
		return new Color
		{
			r = Random.Range(min.r, max.r),
			g = Random.Range(min.g, max.g),
			b = Random.Range(min.b, max.b),
			a = Random.Range(min.a, max.a)
		};
	}

	public static Color MMColorize(this Color originalColor, Color targetColor, ColoringMode coloringMode, float lerpAmount = 1f)
	{
		Color b = Color.white;
		switch (coloringMode)
		{
		case ColoringMode.Tint:
		{
			Color.RGBToHSV(originalColor, out var _, out var _, out var V);
			Color.RGBToHSV(targetColor, out var H2, out var S2, out var V2);
			b = Color.HSVToRGB(H2, S2, V * V2);
			b.a = originalColor.a * targetColor.a;
			break;
		}
		case ColoringMode.Multiply:
			b = originalColor * targetColor;
			break;
		case ColoringMode.Replace:
			b = targetColor;
			break;
		case ColoringMode.ReplaceKeepAlpha:
			b = targetColor;
			b.a = originalColor.a;
			break;
		case ColoringMode.Add:
			b = originalColor + targetColor;
			break;
		}
		return Color.Lerp(originalColor, b, lerpAmount);
	}
}
