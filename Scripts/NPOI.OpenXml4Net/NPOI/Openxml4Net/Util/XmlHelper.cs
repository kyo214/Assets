using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXml4Net.Util;

public static class XmlHelper
{
	public static T ReadEnum<T>(XmlAttribute attr)
	{
		if (attr != null)
		{
			return (T)Enum.Parse(typeof(T), attr.Value);
		}
		return default;
	}

	public static string GetEnumValue(Enum e)
	{
		FieldInfo field = e.GetType().GetField(e.ToString("G"));
		if (!field.IsDefined(typeof(XmlEnumAttribute), inherit: false))
		{
			return e.ToString("G");
		}
		return ((XmlEnumAttribute)field.GetCustomAttributes(typeof(XmlEnumAttribute), inherit: false)[0]).Name;
	}

	public static string GetXmlAttrNameFromEnumValue<T>(T pEnumVal)
	{
		return ((XmlEnumAttribute)pEnumVal.GetType().GetField(Enum.GetName(typeof(T), pEnumVal)).GetCustomAttributes(typeof(XmlEnumAttribute), inherit: false)[0]).Name;
	}

	public static T GetEnumValueFromString<T>(string value)
	{
		foreach (object value2 in Enum.GetValues(typeof(T)))
		{
			if (GetXmlAttrNameFromEnumValue((T)value2).Equals(value, StringComparison.OrdinalIgnoreCase))
			{
				return (T)value2;
			}
		}
		throw new ArgumentException("No XmlEnumAttribute code exists for type " + typeof(T).ToString() + " corresponding to value of " + value);
	}

	public static int ReadInt(XmlAttribute attr)
	{
		if (attr == null)
		{
			return 0;
		}
		if (int.TryParse(attr.Value, out var result))
		{
			return result;
		}
		return 0;
	}

	public static long ReadLong(XmlAttribute attr)
	{
		if (attr == null)
		{
			return 0L;
		}
		if (long.TryParse(attr.Value, out var result))
		{
			return result;
		}
		return 0L;
	}

	public static int? ReadIntNull(XmlAttribute attr)
	{
		if (attr == null)
		{
			return null;
		}
		string value = attr.Value;
		if (value != "" && int.TryParse(value, out var result))
		{
			return result;
		}
		return null;
	}

	public static string ReadString(XmlAttribute attr)
	{
		return attr?.Value;
	}

	public static decimal ReadDecimal(XmlAttribute attr)
	{
		if (attr == null)
		{
			return 0m;
		}
		if (decimal.TryParse(attr.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}
		return 0m;
	}

	public static uint ReadUInt(XmlAttribute attr, uint defaultValue)
	{
		if (attr == null)
		{
			return defaultValue;
		}
		if (uint.TryParse(attr.Value, out var result))
		{
			return result;
		}
		return defaultValue;
	}

	public static uint ReadUInt(XmlAttribute attr)
	{
		return ReadUInt(attr, 0u);
	}

	public static ulong ReadULong(XmlAttribute attr)
	{
		if (attr == null)
		{
			return 0uL;
		}
		if (ulong.TryParse(attr.Value, out var result))
		{
			return result;
		}
		return 0uL;
	}

	public static bool ReadBool(XmlAttribute attr)
	{
		return ReadBool(attr, blankValue: false);
	}

	public static double ReadDouble(XmlAttribute attr)
	{
		if (attr == null)
		{
			return 0.0;
		}
		string value = attr.Value;
		if (value == "")
		{
			return 0.0;
		}
		if (double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}
		return 0.0;
	}

	public static double? ReadDoubleNull(XmlAttribute attr)
	{
		if (attr == null)
		{
			return null;
		}
		string value = attr.Value;
		if (value == "")
		{
			return null;
		}
		if (double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}
		return null;
	}

	public static bool ReadBool(XmlAttribute attr, bool blankValue)
	{
		if (attr == null)
		{
			return blankValue;
		}
		string value = attr.Value;
		if (value == "1" || value == "-1" || value.ToLower() == "true" || value.ToLower() == "on")
		{
			return true;
		}
		if (string.IsNullOrEmpty(value))
		{
			return blankValue;
		}
		return false;
	}

	public static DateTime? ReadDateTime(XmlAttribute attr)
	{
		if (attr == null)
		{
			return null;
		}
		return DateTime.Parse(attr.Value);
	}

	public static string ExcelEncodeString(string t)
	{
		StringWriter stringWriter = new StringWriter();
		for (int i = 0; i < t.Length; i++)
		{
			if (t[i] <= '\u001f' && t[i] != '\t' && t[i] != '\n' && t[i] != '\r')
			{
				stringWriter.Write('?');
			}
			else if (t[i] == '\ufffe')
			{
				stringWriter.Write('?');
			}
			else
			{
				stringWriter.Write(t[i]);
			}
		}
		return stringWriter.ToString();
	}

	public static string ExcelDecodeString(string t)
	{
		Match match = Regex.Match(t, "(_x005F|_x[0-9A-F]{4,4}_)");
		if (!match.Success)
		{
			return t;
		}
		bool flag = false;
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		while (match.Success)
		{
			if (num < match.Index)
			{
				stringBuilder.Append(t.Substring(num, match.Index - num));
			}
			if (!flag && match.Value == "_x005F")
			{
				flag = true;
			}
			else if (flag)
			{
				stringBuilder.Append(match.Value);
				flag = false;
			}
			else
			{
				stringBuilder.Append((char)int.Parse(match.Value.Substring(2, 4)));
			}
			num = match.Index + match.Length;
			match = match.NextMatch();
		}
		stringBuilder.Append(t.Substring(num, t.Length - num));
		return stringBuilder.ToString();
	}

	public static string EncodeXml(string xml)
	{
		return xml.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
			.Replace("\"", "&quot;");
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, bool value)
	{
		WriteAttribute(sw, attributeName, value, writeIfBlank: true);
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, bool value, bool writeIfBlank)
	{
		if (value || writeIfBlank)
		{
			WriteAttribute(sw, attributeName, value ? "1" : "0");
		}
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, double value)
	{
		WriteAttribute(sw, attributeName, value, writeIfBlank: false);
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, double value, bool writeIfBlank)
	{
		if (value != 0.0 || writeIfBlank)
		{
			WriteAttribute(sw, attributeName, (value == 0.0) ? "0" : value.ToString(CultureInfo.InvariantCulture));
		}
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, int value, bool writeIfBlank)
	{
		if (value != 0 || writeIfBlank)
		{
			WriteAttribute(sw, attributeName, value.ToString(CultureInfo.InvariantCulture));
		}
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, int value)
	{
		WriteAttribute(sw, attributeName, value, writeIfBlank: false);
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, string value)
	{
		WriteAttribute(sw, attributeName, value, writeIfBlank: false);
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, string value, bool writeIfBlank)
	{
		if (!string.IsNullOrEmpty(value) || writeIfBlank)
		{
			sw.Write($" {attributeName}=\"{((value == null) ? string.Empty : EncodeXml(value))}\"");
		}
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, byte[] value)
	{
		if (value != null)
		{
			WriteAttribute(sw, attributeName, BitConverter.ToString(value).Replace("-", ""), writeIfBlank: false);
		}
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, uint value, uint defaultValue, bool writeIfBlank = false)
	{
		if (value != defaultValue)
		{
			WriteAttribute(sw, attributeName, (int)value, writeIfBlank: true);
		}
		else if (writeIfBlank)
		{
			WriteAttribute(sw, attributeName, (int)value, writeIfBlank);
		}
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, uint value, bool writeIfBlank = false)
	{
		WriteAttribute(sw, attributeName, (int)value, writeIfBlank);
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, DateTime? value)
	{
		if (!value.HasValue)
		{
			return;
		}
		WriteAttribute(sw, attributeName, value.ToString(), writeIfBlank: false);
		throw new NotImplementedException();
	}

	public static void LoadXmlSafe(XmlDocument xmlDoc, Stream stream)
	{
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.XmlResolver = null;
		xmlReaderSettings.ProhibitDtd = true;
		XmlReader reader = XmlReader.Create(stream, xmlReaderSettings);
		xmlDoc.Load(reader);
	}

	public static void LoadXmlSafe(XmlDocument xmlDoc, string xml, Encoding encoding)
	{
		MemoryStream stream = new MemoryStream(encoding.GetBytes(xml));
		LoadXmlSafe(xmlDoc, stream);
	}

	public static byte[] ReadBytes(XmlAttribute attr)
	{
		if (attr == null || string.IsNullOrEmpty(attr.Value))
		{
			return null;
		}
		int length = attr.Value.Length;
		byte[] array = new byte[length / 2];
		for (int i = 0; i < length; i += 2)
		{
			array[i / 2] = Convert.ToByte(attr.Value.Substring(i, 2), 16);
		}
		return array;
	}

	public static sbyte ReadSByte(XmlAttribute attr)
	{
		if (attr == null)
		{
			return 0;
		}
		if (sbyte.TryParse(attr.Value, out var result))
		{
			return result;
		}
		return 0;
	}

	public static ushort ReadUShort(XmlAttribute attr)
	{
		if (attr == null)
		{
			return 0;
		}
		if (ushort.TryParse(attr.Value, out var result))
		{
			return result;
		}
		return 0;
	}

	public static byte ReadByte(XmlAttribute attr)
	{
		if (attr == null)
		{
			return 0;
		}
		if (byte.TryParse(attr.Value, out var result))
		{
			return result;
		}
		return 0;
	}
}
