using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_MeasureDimensionMap
{
	private uint measureGroupField;

	private bool measureGroupFieldSpecified;

	private uint dimensionField;

	private bool dimensionFieldSpecified;

	[XmlAttribute]
	public uint measureGroup
	{
		get
		{
			return measureGroupField;
		}
		set
		{
			measureGroupField = value;
		}
	}

	[XmlIgnore]
	public bool measureGroupSpecified
	{
		get
		{
			return measureGroupFieldSpecified;
		}
		set
		{
			measureGroupFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint dimension
	{
		get
		{
			return dimensionField;
		}
		set
		{
			dimensionField = value;
		}
	}

	[XmlIgnore]
	public bool dimensionSpecified
	{
		get
		{
			return dimensionFieldSpecified;
		}
		set
		{
			dimensionFieldSpecified = value;
		}
	}

	public static CT_MeasureDimensionMap Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_MeasureDimensionMap cT_MeasureDimensionMap = new CT_MeasureDimensionMap();
		if (node.Attributes["measureGroup"] != null)
		{
			cT_MeasureDimensionMap.measureGroup = XmlHelper.ReadUInt(node.Attributes["measureGroup"]);
		}
		if (node.Attributes["dimension"] != null)
		{
			cT_MeasureDimensionMap.dimension = XmlHelper.ReadUInt(node.Attributes["dimension"]);
		}
		return cT_MeasureDimensionMap;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "measureGroup", measureGroup);
		XmlHelper.WriteAttribute(sw, "dimension", dimension);
		sw.Write("/>");
	}
}
