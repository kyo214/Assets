using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart", IsNullable = true)]
public class CT_ErrValType
{
	private ST_ErrValType valField;

	[XmlAttribute]
	[DefaultValue(ST_ErrValType.fixedVal)]
	public ST_ErrValType val
	{
		get
		{
			return valField;
		}
		set
		{
			valField = value;
		}
	}

	public CT_ErrValType()
	{
		valField = ST_ErrValType.fixedVal;
	}

	public static CT_ErrValType Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_ErrValType cT_ErrValType = new CT_ErrValType();
		if (node.Attributes["val"] != null)
		{
			cT_ErrValType.val = (ST_ErrValType)Enum.Parse(typeof(ST_ErrValType), node.Attributes["val"].Value);
		}
		return cT_ErrValType;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "val", val.ToString());
		sw.Write(">");
		sw.Write($"</c:{nodeName}>");
	}
}
