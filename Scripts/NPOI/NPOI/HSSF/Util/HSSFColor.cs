using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.Util;

public class HSSFColor : IColor
{
	public class Black : HSSFColor
	{
		public const short Index = 8;

		public static readonly byte[] Triplet = new byte[3];

		public const string HexString = "0:0:0";

		public override short Indexed => 8;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "0:0:0";
		}
	}

	public class Brown : HSSFColor
	{
		public const short Index = 60;

		public static readonly byte[] Triplet = new byte[3] { 153, 51, 0 };

		public const string HexString = "9999:3333:0";

		public override short Indexed => 60;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "9999:3333:0";
		}
	}

	public class OliveGreen : HSSFColor
	{
		public const short Index = 59;

		public static readonly byte[] Triplet = new byte[3] { 51, 51, 0 };

		public const string HexString = "3333:3333:0";

		public override short Indexed => 59;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "3333:3333:0";
		}
	}

	public class DarkGreen : HSSFColor
	{
		public const short Index = 58;

		public static readonly byte[] Triplet = new byte[3] { 0, 51, 0 };

		public const string HexString = "0:3333:0";

		public override short Indexed => 58;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "0:3333:0";
		}
	}

	public class DarkTeal : HSSFColor
	{
		public const short Index = 56;

		public static readonly byte[] Triplet = new byte[3] { 0, 51, 102 };

		public const string HexString = "0:3333:6666";

		public override short Indexed => 56;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "0:3333:6666";
		}
	}

	public class DarkBlue : HSSFColor
	{
		public const short Index = 18;

		public const short Index2 = 32;

		public static readonly byte[] Triplet = new byte[3] { 0, 0, 128 };

		public const string HexString = "0:0:8080";

		public override short Indexed => 18;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "0:0:8080";
		}
	}

	public class Indigo : HSSFColor
	{
		public const short Index = 62;

		public static readonly byte[] Triplet = new byte[3] { 51, 51, 153 };

		public const string HexString = "3333:3333:9999";

		public override short Indexed => 62;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "3333:3333:9999";
		}
	}

	public class Grey80Percent : HSSFColor
	{
		public const short Index = 63;

		public static readonly byte[] Triplet = new byte[3] { 51, 51, 51 };

		public const string HexString = "3333:3333:3333";

		public override short Indexed => 63;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "3333:3333:3333";
		}
	}

	public class DarkRed : HSSFColor
	{
		public const short Index = 16;

		public const short Index2 = 37;

		public static readonly byte[] Triplet = new byte[3] { 128, 0, 0 };

		public const string HexString = "8080:0:0";

		public override short Indexed => 16;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "8080:0:0";
		}
	}

	public class Orange : HSSFColor
	{
		public const short Index = 53;

		public static readonly byte[] Triplet = new byte[3] { 255, 102, 0 };

		public const string HexString = "FFFF:6666:0";

		public override short Indexed => 53;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "FFFF:6666:0";
		}
	}

	public class DarkYellow : HSSFColor
	{
		public const short Index = 19;

		public static readonly byte[] Triplet = new byte[3] { 128, 128, 0 };

		public const string HexString = "8080:8080:0";

		public override short Indexed => 19;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "8080:8080:0";
		}
	}

	public class Green : HSSFColor
	{
		public const short Index = 17;

		public static readonly byte[] Triplet = new byte[3] { 0, 128, 0 };

		public const string HexString = "0:8080:0";

		public override short Indexed => 17;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "0:8080:0";
		}
	}

	public class Teal : HSSFColor
	{
		public const short Index = 21;

		public const short Index2 = 38;

		public static readonly byte[] Triplet = new byte[3] { 0, 128, 128 };

		public const string HexString = "0:8080:8080";

		public override short Indexed => 21;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "0:8080:8080";
		}
	}

	public class Blue : HSSFColor
	{
		public const short Index = 12;

		public const short Index2 = 39;

		public static readonly byte[] Triplet = new byte[3] { 0, 0, 255 };

		public const string HexString = "0:0:FFFF";

		public override short Indexed => 12;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "0:0:FFFF";
		}
	}

	public class BlueGrey : HSSFColor
	{
		public const short Index = 54;

		public static readonly byte[] Triplet = new byte[3] { 102, 102, 153 };

		public const string HexString = "6666:6666:9999";

		public override short Indexed => 54;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "6666:6666:9999";
		}
	}

	public class Grey50Percent : HSSFColor
	{
		public const short Index = 23;

		public static readonly byte[] Triplet = new byte[3] { 128, 128, 128 };

		public const string HexString = "8080:8080:8080";

		public override short Indexed => 23;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "8080:8080:8080";
		}
	}

	public class Red : HSSFColor
	{
		public const short Index = 10;

		public static readonly byte[] Triplet = new byte[3] { 255, 0, 0 };

		public const string HexString = "FFFF:0:0";

		public override short Indexed => 10;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "FFFF:0:0";
		}
	}

	public class LightOrange : HSSFColor
	{
		public const short Index = 52;

		public static readonly byte[] Triplet = new byte[3] { 255, 153, 0 };

		public const string HexString = "FFFF:9999:0";

		public override short Indexed => 52;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "FFFF:9999:0";
		}
	}

	public class Lime : HSSFColor
	{
		public const short Index = 50;

		public static readonly byte[] Triplet = new byte[3] { 153, 204, 0 };

		public const string HexString = "9999:CCCC:0";

		public override short Indexed => 50;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "9999:CCCC:0";
		}
	}

	public class SeaGreen : HSSFColor
	{
		public const short Index = 57;

		public static readonly byte[] Triplet = new byte[3] { 51, 153, 102 };

		public const string HexString = "3333:9999:6666";

		public override short Indexed => 57;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "3333:9999:6666";
		}
	}

	public class Aqua : HSSFColor
	{
		public const short Index = 49;

		public static readonly byte[] Triplet = new byte[3] { 51, 204, 204 };

		public const string HexString = "3333:CCCC:CCCC";

		public override short Indexed => 49;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "3333:CCCC:CCCC";
		}
	}

	public class LightBlue : HSSFColor
	{
		public const short Index = 48;

		public static readonly byte[] Triplet = new byte[3] { 51, 102, 255 };

		public const string HexString = "3333:6666:FFFF";

		public override short Indexed => 48;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "3333:6666:FFFF";
		}
	}

	public class Violet : HSSFColor
	{
		public const short Index = 20;

		public const short Index2 = 36;

		public static readonly byte[] Triplet = new byte[3] { 128, 0, 128 };

		public const string HexString = "8080:0:8080";

		public override short Indexed => 20;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "8080:0:8080";
		}
	}

	public class Grey40Percent : HSSFColor
	{
		public const short Index = 55;

		public static readonly byte[] Triplet = new byte[3] { 150, 150, 150 };

		public const string HexString = "9696:9696:9696";

		public override short Indexed => 55;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "9696:9696:9696";
		}
	}

	public class Pink : HSSFColor
	{
		public const short Index = 14;

		public const short Index2 = 33;

		public static readonly byte[] Triplet = new byte[3] { 255, 0, 255 };

		public const string HexString = "FFFF:0:FFFF";

		public override short Indexed => 14;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "FFFF:0:FFFF";
		}
	}

	public class Gold : HSSFColor
	{
		public const short Index = 51;

		public static readonly byte[] Triplet = new byte[3] { 255, 204, 0 };

		public const string HexString = "FFFF:CCCC:0";

		public override short Indexed => 51;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "FFFF:CCCC:0";
		}
	}

	public class Yellow : HSSFColor
	{
		public const short Index = 13;

		public const short Index2 = 34;

		public static readonly byte[] Triplet = new byte[3] { 255, 255, 0 };

		public const string HexString = "FFFF:FFFF:0";

		public override short Indexed => 13;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "FFFF:FFFF:0";
		}
	}

	public class BrightGreen : HSSFColor
	{
		public const short Index = 11;

		public const short Index2 = 35;

		public static readonly byte[] Triplet = new byte[3] { 0, 255, 0 };

		public const string HexString = "0:FFFF:0";

		public override short Indexed => 11;

		public override string GetHexString()
		{
			return "0:FFFF:0";
		}

		public override byte[] GetTriplet()
		{
			return Triplet;
		}
	}

	public class Turquoise : HSSFColor
	{
		public const short Index = 15;

		public const short Index2 = 35;

		public static readonly byte[] Triplet = new byte[3] { 0, 255, 255 };

		public const string HexString = "0:FFFF:FFFF";

		public override short Indexed => 15;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "0:FFFF:FFFF";
		}
	}

	public class SkyBlue : HSSFColor
	{
		public const short Index = 40;

		public static readonly byte[] Triplet = new byte[3] { 0, 204, 255 };

		public const string HexString = "0:CCCC:FFFF";

		public override short Indexed => 40;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "0:CCCC:FFFF";
		}
	}

	public class Plum : HSSFColor
	{
		public const short Index = 61;

		public const short Index2 = 25;

		public static readonly byte[] Triplet = new byte[3] { 153, 51, 102 };

		public const string HexString = "9999:3333:6666";

		public override short Indexed => 61;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "9999:3333:6666";
		}
	}

	public class Grey25Percent : HSSFColor
	{
		public const short Index = 22;

		public static readonly byte[] Triplet = new byte[3] { 192, 192, 192 };

		public const string HexString = "C0C0:C0C0:C0C0";

		public override short Indexed => 22;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "C0C0:C0C0:C0C0";
		}
	}

	public class Rose : HSSFColor
	{
		public const short Index = 45;

		public static readonly byte[] Triplet = new byte[3] { 255, 153, 204 };

		public const string HexString = "FFFF:9999:CCCC";

		public override short Indexed => 45;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "FFFF:9999:CCCC";
		}
	}

	public class Tan : HSSFColor
	{
		public const short Index = 47;

		public static readonly byte[] Triplet = new byte[3] { 255, 204, 153 };

		public const string HexString = "FFFF:CCCC:9999";

		public override short Indexed => 47;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "FFFF:CCCC:9999";
		}
	}

	public class LightYellow : HSSFColor
	{
		public const short Index = 43;

		public static readonly byte[] Triplet = new byte[3] { 255, 255, 153 };

		public const string HexString = "FFFF:FFFF:9999";

		public override short Indexed => 43;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "FFFF:FFFF:9999";
		}
	}

	public class LightGreen : HSSFColor
	{
		public const short Index = 42;

		public static readonly byte[] Triplet = new byte[3] { 204, 255, 204 };

		public const string HexString = "CCCC:FFFF:CCCC";

		public override short Indexed => 42;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "CCCC:FFFF:CCCC";
		}
	}

	public class LightTurquoise : HSSFColor
	{
		public const short Index = 41;

		public const short Index2 = 27;

		public static readonly byte[] Triplet = new byte[3] { 204, 255, 255 };

		public const string HexString = "CCCC:FFFF:FFFF";

		public override short Indexed => 41;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "CCCC:FFFF:FFFF";
		}
	}

	public class PaleBlue : HSSFColor
	{
		public const short Index = 44;

		public static readonly byte[] Triplet = new byte[3] { 153, 204, 255 };

		public const string HexString = "9999:CCCC:FFFF";

		public override short Indexed => 44;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "9999:CCCC:FFFF";
		}
	}

	public class Lavender : HSSFColor
	{
		public const short Index = 46;

		public static readonly byte[] Triplet = new byte[3] { 204, 153, 255 };

		public const string HexString = "CCCC:9999:FFFF";

		public override short Indexed => 46;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "CCCC:9999:FFFF";
		}
	}

	public class White : HSSFColor
	{
		public const short Index = 9;

		public static readonly byte[] Triplet = new byte[3] { 255, 255, 255 };

		public const string HexString = "FFFF:FFFF:FFFF";

		public override short Indexed => 9;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "FFFF:FFFF:FFFF";
		}
	}

	public class CornflowerBlue : HSSFColor
	{
		public const short Index = 24;

		public static readonly byte[] Triplet = new byte[3] { 153, 153, 255 };

		public const string HexString = "9999:9999:FFFF";

		public override short Indexed => 24;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "9999:9999:FFFF";
		}
	}

	public class LemonChiffon : HSSFColor
	{
		public const short Index = 26;

		public static readonly byte[] Triplet = new byte[3] { 255, 255, 204 };

		public const string HexString = "FFFF:FFFF:CCCC";

		public override short Indexed => 26;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "FFFF:FFFF:CCCC";
		}
	}

	public class Maroon : HSSFColor
	{
		public const short Index = 25;

		public static readonly byte[] Triplet = new byte[3] { 127, 0, 0 };

		public const string HexString = "8000:0:0";

		public override short Indexed => 25;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "8000:0:0";
		}
	}

	public class Orchid : HSSFColor
	{
		public const short Index = 28;

		public static readonly byte[] Triplet = new byte[3] { 102, 0, 102 };

		public const string HexString = "6666:0:6666";

		public override short Indexed => 28;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "6666:0:6666";
		}
	}

	public class Coral : HSSFColor
	{
		public const short Index = 29;

		public static readonly byte[] Triplet = new byte[3] { 255, 128, 128 };

		public const string HexString = "FFFF:8080:8080";

		public override short Indexed => 29;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "FFFF:8080:8080";
		}
	}

	public class RoyalBlue : HSSFColor
	{
		public const short Index = 30;

		public static readonly byte[] Triplet = new byte[3] { 0, 102, 204 };

		public const string HexString = "0:6666:CCCC";

		public override short Indexed => 30;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "0:6666:CCCC";
		}
	}

	public class LightCornflowerBlue : HSSFColor
	{
		public const short Index = 31;

		public static readonly byte[] Triplet = new byte[3] { 204, 204, 255 };

		public const string HexString = "CCCC:CCCC:FFFF";

		public override short Indexed => 31;

		public override byte[] GetTriplet()
		{
			return Triplet;
		}

		public override string GetHexString()
		{
			return "CCCC:CCCC:FFFF";
		}
	}

	public class Automatic : HSSFColor
	{
		private static HSSFColor instance = new Automatic();

		public const short Index = 64;

		public override byte[] GetTriplet()
		{
			return Black.Triplet;
		}

		public override string GetHexString()
		{
			return "0:0:0";
		}

		public static HSSFColor GetInstance()
		{
			return instance;
		}
	}

	private static Dictionary<int, HSSFColor> indexHash;

	public const short COLOR_NORMAL = short.MaxValue;

	public virtual short Indexed => 8;

	public byte[] RGB => GetTriplet();

	public static Dictionary<int, HSSFColor> GetIndexHash()
	{
		if (indexHash == null)
		{
			indexHash = CreateColorsByIndexMap();
		}
		return indexHash;
	}

	public static Dictionary<int, HSSFColor> GetMutableIndexHash()
	{
		return CreateColorsByIndexMap();
	}

	private static Dictionary<int, HSSFColor> CreateColorsByIndexMap()
	{
		HSSFColor[] allColors = GetAllColors();
		Dictionary<int, HSSFColor> dictionary = new Dictionary<int, HSSFColor>(allColors.Length * 3 / 2);
		foreach (HSSFColor hSSFColor in allColors)
		{
			int indexed = hSSFColor.Indexed;
			if (dictionary.ContainsKey(indexed))
			{
				HSSFColor hSSFColor2 = dictionary[indexed];
				throw new InvalidDataException("Dup color index (" + indexed + ") for colors (" + hSSFColor2.GetType().Name + "),(" + hSSFColor.GetType().Name + ")");
			}
			dictionary.Add(indexed, hSSFColor);
		}
		foreach (HSSFColor hSSFColor3 in allColors)
		{
			int index = GetIndex2(hSSFColor3);
			if (index != -1)
			{
				dictionary[index] = hSSFColor3;
			}
		}
		return dictionary;
	}

	private static int GetIndex2(HSSFColor color)
	{
		FieldInfo field = color.GetType().GetField("Index2", BindingFlags.Static | BindingFlags.Public);
		if (field == null)
		{
			return -1;
		}
		return Convert.ToInt32((short)field.GetValue(color));
	}

	internal static HSSFColor[] GetAllColors()
	{
		return new HSSFColor[47]
		{
			new Black(),
			new Brown(),
			new OliveGreen(),
			new DarkGreen(),
			new DarkTeal(),
			new DarkBlue(),
			new Indigo(),
			new Grey80Percent(),
			new Orange(),
			new DarkYellow(),
			new Green(),
			new Teal(),
			new Blue(),
			new BlueGrey(),
			new Grey50Percent(),
			new Red(),
			new LightOrange(),
			new Lime(),
			new SeaGreen(),
			new Aqua(),
			new LightBlue(),
			new Violet(),
			new Grey40Percent(),
			new Pink(),
			new Gold(),
			new Yellow(),
			new BrightGreen(),
			new Turquoise(),
			new DarkRed(),
			new SkyBlue(),
			new Plum(),
			new Grey25Percent(),
			new Rose(),
			new LightYellow(),
			new LightGreen(),
			new LightTurquoise(),
			new PaleBlue(),
			new Lavender(),
			new White(),
			new CornflowerBlue(),
			new LemonChiffon(),
			new Maroon(),
			new Orchid(),
			new Coral(),
			new RoyalBlue(),
			new LightCornflowerBlue(),
			new Tan()
		};
	}

	public static Dictionary<string, HSSFColor> GetTripletHash()
	{
		return CreateColorsByHexStringMap();
	}

	private static Dictionary<string, HSSFColor> CreateColorsByHexStringMap()
	{
		HSSFColor[] allColors = GetAllColors();
		Dictionary<string, HSSFColor> dictionary = new Dictionary<string, HSSFColor>(allColors.Length * 3 / 2);
		foreach (HSSFColor hSSFColor in allColors)
		{
			string hexString = hSSFColor.GetHexString();
			if (dictionary.ContainsKey(hexString))
			{
				throw new InvalidDataException("Dup color hexString (" + hexString + ") for color (" + hSSFColor.GetType().Name + ")");
			}
			dictionary[hexString] = hSSFColor;
		}
		return dictionary;
	}

	public virtual byte[] GetTriplet()
	{
		return Black.Triplet;
	}

	public virtual string GetHexString()
	{
		return "0:0:0";
	}

	public static HSSFColor ToHSSFColor(IColor color)
	{
		if (color != null && !(color is HSSFColor))
		{
			throw new ArgumentException("Only HSSFColor objects are supported");
		}
		return (HSSFColor)color;
	}
}
