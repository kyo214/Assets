using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml.WordProcessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing", IsNullable = true)]
public class CT_WrapSquare
{
	private CT_EffectExtent effectExtentField;

	private ST_WrapText wrapTextField;

	private uint distTField;

	private bool distTFieldSpecified;

	private uint distBField;

	private bool distBFieldSpecified;

	private uint distLField;

	private bool distLFieldSpecified;

	private uint distRField;

	private bool distRFieldSpecified;

	[XmlElement(Order = 0)]
	public CT_EffectExtent effectExtent
	{
		get
		{
			return effectExtentField;
		}
		set
		{
			effectExtentField = value;
		}
	}

	[XmlAttribute]
	public ST_WrapText wrapText
	{
		get
		{
			return wrapTextField;
		}
		set
		{
			wrapTextField = value;
		}
	}

	[XmlAttribute]
	public uint distT
	{
		get
		{
			return distTField;
		}
		set
		{
			distTField = value;
		}
	}

	[XmlIgnore]
	public bool distTSpecified
	{
		get
		{
			return distTFieldSpecified;
		}
		set
		{
			distTFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint distB
	{
		get
		{
			return distBField;
		}
		set
		{
			distBField = value;
		}
	}

	[XmlIgnore]
	public bool distBSpecified
	{
		get
		{
			return distBFieldSpecified;
		}
		set
		{
			distBFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint distL
	{
		get
		{
			return distLField;
		}
		set
		{
			distLField = value;
		}
	}

	[XmlIgnore]
	public bool distLSpecified
	{
		get
		{
			return distLFieldSpecified;
		}
		set
		{
			distLFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint distR
	{
		get
		{
			return distRField;
		}
		set
		{
			distRField = value;
		}
	}

	[XmlIgnore]
	public bool distRSpecified
	{
		get
		{
			return distRFieldSpecified;
		}
		set
		{
			distRFieldSpecified = value;
		}
	}

	public CT_WrapSquare()
	{
		effectExtentField = new CT_EffectExtent();
	}

	public static CT_WrapSquare Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_WrapSquare
		{
			wrapText = (ST_WrapText)Enum.Parse(typeof(ST_WrapText), node.Attributes["wrapText"].Value)
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<wp:{nodeName}");
		XmlHelper.WriteAttribute(sw, "wrapText", wrapText.ToString(), writeIfBlank: true);
		sw.Write("/>");
	}
}
