using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.Util;
using NPOI.OpenXmlFormats.Vml;
using NPOI.OpenXmlFormats.Vml.Office;
using NPOI.OpenXmlFormats.Vml.Spreadsheet;
using NPOI.OpenXmlFormats.Vml.Wordprocessing;

namespace NPOI.OpenXmlFormats.Util;

public static class XmlHelper
{
	public static ST_TrueFalseBlank ReadTrueFalseBlank(string attrValue)
	{
		if (string.IsNullOrEmpty(attrValue))
		{
			return ST_TrueFalseBlank.NONE;
		}
		if (string.IsNullOrEmpty(attrValue))
		{
			return ST_TrueFalseBlank.NONE;
		}
		string text = attrValue.ToLower();
		if (text == "t" || text == "true")
		{
			return ST_TrueFalseBlank.@true;
		}
		return ST_TrueFalseBlank.@false;
	}

	public static ST_TrueFalseBlank ReadTrueFalseBlank(XmlAttribute attr)
	{
		if (attr == null)
		{
			return ST_TrueFalseBlank.NONE;
		}
		if (string.IsNullOrEmpty(attr.Value))
		{
			return ST_TrueFalseBlank.NONE;
		}
		string text = attr.Value.ToLower();
		if (text == "t" || text == "true")
		{
			return ST_TrueFalseBlank.@true;
		}
		return ST_TrueFalseBlank.@false;
	}

	public static NPOI.OpenXmlFormats.Vml.Office.ST_TrueFalse ReadTrueFalse(XmlAttribute attr)
	{
		if (attr == null)
		{
			return NPOI.OpenXmlFormats.Vml.Office.ST_TrueFalse.@false;
		}
		if (string.IsNullOrEmpty(attr.Value))
		{
			return NPOI.OpenXmlFormats.Vml.Office.ST_TrueFalse.@false;
		}
		string text = attr.Value.ToLower();
		if (text == "t" || text == "true")
		{
			return NPOI.OpenXmlFormats.Vml.Office.ST_TrueFalse.@true;
		}
		return NPOI.OpenXmlFormats.Vml.Office.ST_TrueFalse.@false;
	}

	public static ST_BorderShadow ReadBorderShadow(XmlAttribute attr)
	{
		if (attr == null)
		{
			return ST_BorderShadow.@false;
		}
		if (string.IsNullOrEmpty(attr.Value))
		{
			return ST_BorderShadow.@false;
		}
		string text = attr.Value.ToLower();
		if (text == "t" || text == "true")
		{
			return ST_BorderShadow.@true;
		}
		return ST_BorderShadow.@false;
	}

	public static NPOI.OpenXmlFormats.Vml.ST_TrueFalse ReadTrueFalse2(XmlAttribute attr)
	{
		if (attr == null)
		{
			return NPOI.OpenXmlFormats.Vml.ST_TrueFalse.@false;
		}
		if (string.IsNullOrEmpty(attr.Value))
		{
			return NPOI.OpenXmlFormats.Vml.ST_TrueFalse.@false;
		}
		string text = attr.Value.ToLower();
		if (text == "t" || text == "true")
		{
			return NPOI.OpenXmlFormats.Vml.ST_TrueFalse.@true;
		}
		return NPOI.OpenXmlFormats.Vml.ST_TrueFalse.@false;
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, NPOI.OpenXmlFormats.Vml.Office.ST_TrueFalse value)
	{
		WriteAttribute(sw, attributeName, value, defaultValue: false);
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, NPOI.OpenXmlFormats.Vml.Office.ST_TrueFalse value, bool defaultValue)
	{
		if ((!defaultValue || (value != NPOI.OpenXmlFormats.Vml.Office.ST_TrueFalse.t && value != NPOI.OpenXmlFormats.Vml.Office.ST_TrueFalse.@true)) && (defaultValue || (value != NPOI.OpenXmlFormats.Vml.Office.ST_TrueFalse.f && value != NPOI.OpenXmlFormats.Vml.Office.ST_TrueFalse.@false)))
		{
			if (value == NPOI.OpenXmlFormats.Vml.Office.ST_TrueFalse.t || value == NPOI.OpenXmlFormats.Vml.Office.ST_TrueFalse.@true)
			{
				NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, attributeName, "t");
			}
			else
			{
				NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, attributeName, "f");
			}
		}
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, ST_BorderShadow value)
	{
		WriteAttribute(sw, attributeName, value, defaultValue: false);
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, ST_BorderShadow value, bool defaultValue)
	{
		if ((!defaultValue || (value != ST_BorderShadow.t && value != ST_BorderShadow.@true)) && (defaultValue || (value != ST_BorderShadow.f && value != ST_BorderShadow.@false)))
		{
			if (value == ST_BorderShadow.t || value == ST_BorderShadow.@true)
			{
				NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, attributeName, "t");
			}
			else
			{
				NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, attributeName, "f");
			}
		}
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, NPOI.OpenXmlFormats.Vml.ST_TrueFalse value)
	{
		WriteAttribute(sw, attributeName, value, defaultValue: false);
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, NPOI.OpenXmlFormats.Vml.ST_TrueFalse value, bool defaultValue)
	{
		if ((!defaultValue || (value != NPOI.OpenXmlFormats.Vml.ST_TrueFalse.t && value != NPOI.OpenXmlFormats.Vml.ST_TrueFalse.@true)) && (defaultValue || (value != NPOI.OpenXmlFormats.Vml.ST_TrueFalse.f && value != NPOI.OpenXmlFormats.Vml.ST_TrueFalse.@false)))
		{
			if (value == NPOI.OpenXmlFormats.Vml.ST_TrueFalse.t || value == NPOI.OpenXmlFormats.Vml.ST_TrueFalse.@true)
			{
				NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, attributeName, "t");
			}
			else
			{
				NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, attributeName, "f");
			}
		}
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, ST_TrueFalseBlank value)
	{
		WriteAttribute(sw, attributeName, value, null);
	}

	public static void WriteAttribute(StreamWriter sw, string attributeName, ST_TrueFalseBlank value, bool? defaultValue)
	{
		if ((defaultValue.HasValue || value != ST_TrueFalseBlank.NONE) && (defaultValue != true || (value != ST_TrueFalseBlank.t && value != ST_TrueFalseBlank.@true)) && (defaultValue != false || (value != ST_TrueFalseBlank.f && value != ST_TrueFalseBlank.@false)))
		{
			if (value == ST_TrueFalseBlank.t || value == ST_TrueFalseBlank.@true)
			{
				NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, attributeName, "t");
			}
			else
			{
				NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, attributeName, "f");
			}
		}
	}
}
