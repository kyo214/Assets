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
public class CT_TrackChangeNumbering : CT_TrackChange
{
	private string originalField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string original
	{
		get
		{
			return originalField;
		}
		set
		{
			originalField = value;
		}
	}

	public new static CT_TrackChangeNumbering Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_TrackChangeNumbering
		{
			original = XmlHelper.ReadString(node.Attributes["original"]),
			author = XmlHelper.ReadString(node.Attributes["author"]),
			date = XmlHelper.ReadString(node.Attributes["date"]),
			id = XmlHelper.ReadString(node.Attributes["id"])
		};
	}

	internal new void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "original", original);
		XmlHelper.WriteAttribute(sw, "author", base.author);
		XmlHelper.WriteAttribute(sw, "date", base.date);
		XmlHelper.WriteAttribute(sw, "id", base.id);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
