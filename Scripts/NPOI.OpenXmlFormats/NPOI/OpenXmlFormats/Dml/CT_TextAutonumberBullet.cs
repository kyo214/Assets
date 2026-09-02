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
public class CT_TextAutonumberBullet
{
	private ST_TextAutonumberScheme typeField;

	private bool typeFieldSpecified;

	private int startAtField;

	private bool startAtFieldSpecified;

	[XmlAttribute]
	public ST_TextAutonumberScheme type
	{
		get
		{
			return typeField;
		}
		set
		{
			typeField = value;
		}
	}

	[XmlIgnore]
	public bool typeSpecified
	{
		get
		{
			return typeFieldSpecified;
		}
		set
		{
			typeFieldSpecified = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(1)]
	public int startAt
	{
		get
		{
			return startAtField;
		}
		set
		{
			startAtField = value;
		}
	}

	[XmlIgnore]
	public bool startAtSpecified
	{
		get
		{
			return startAtFieldSpecified;
		}
		set
		{
			startAtFieldSpecified = value;
		}
	}

	public static CT_TextAutonumberBullet Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_TextAutonumberBullet cT_TextAutonumberBullet = new CT_TextAutonumberBullet();
		if (node.Attributes["type"] != null)
		{
			cT_TextAutonumberBullet.type = (ST_TextAutonumberScheme)Enum.Parse(typeof(ST_TextAutonumberScheme), node.Attributes["type"].Value);
		}
		cT_TextAutonumberBullet.startAt = XmlHelper.ReadInt(node.Attributes["startAt"]);
		return cT_TextAutonumberBullet;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "type", type.ToString());
		XmlHelper.WriteAttribute(sw, "startAt", startAt);
		sw.Write(">");
		sw.Write($"</a:{nodeName}>");
	}

	public CT_TextAutonumberBullet()
	{
		startAtField = 1;
	}

	public bool IsSetStartAt()
	{
		return startAtField >= 1;
	}
}
