using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_TblGridCol
{
	private ulong wField;

	private bool wFieldSpecified;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ulong w
	{
		get
		{
			return wField;
		}
		set
		{
			wField = value;
		}
	}

	public bool wSpecified
	{
		get
		{
			return wFieldSpecified;
		}
		set
		{
			wFieldSpecified = value;
		}
	}

	public static CT_TblGridCol Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_TblGridCol
		{
			w = XmlHelper.ReadULong(node.Attributes["w:w"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:w", w);
		sw.Write("/>");
	}
}
