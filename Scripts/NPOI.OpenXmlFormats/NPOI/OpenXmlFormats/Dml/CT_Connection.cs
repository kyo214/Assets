using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_Connection
{
	private uint idField;

	private uint idxField;

	[XmlAttribute]
	public uint id
	{
		get
		{
			return idField;
		}
		set
		{
			idField = value;
		}
	}

	[XmlAttribute]
	public uint idx
	{
		get
		{
			return idxField;
		}
		set
		{
			idxField = value;
		}
	}

	public static CT_Connection Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_Connection
		{
			id = XmlHelper.ReadUInt(node.Attributes["id"]),
			idx = XmlHelper.ReadUInt(node.Attributes["idx"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "id", id);
		XmlHelper.WriteAttribute(sw, "idx", idx);
		sw.Write("/>");
	}
}
