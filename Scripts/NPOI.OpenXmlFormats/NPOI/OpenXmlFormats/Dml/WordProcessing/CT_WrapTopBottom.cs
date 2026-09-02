using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml.WordProcessing;

public class CT_WrapTopBottom
{
	private CT_EffectExtent effectExtentField;

	private uint distTField;

	private bool distTFieldSpecified;

	private uint distBField;

	private bool distBFieldSpecified;

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

	public CT_WrapTopBottom()
	{
		effectExtentField = new CT_EffectExtent();
	}

	internal static CT_WrapTopBottom Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_WrapTopBottom
		{
			distB = XmlHelper.ReadUInt(node.Attributes["distB"]),
			distT = XmlHelper.ReadUInt(node.Attributes["distT"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<wp:{nodeName}");
		XmlHelper.WriteAttribute(sw, "distT", distT);
		XmlHelper.WriteAttribute(sw, "distB", distB);
		sw.Write("/>");
	}
}
