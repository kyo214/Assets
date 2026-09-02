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
public class CT_PageMar
{
	private string topField;

	private ulong rightField;

	private string bottomField;

	private ulong leftField;

	private ulong headerField;

	private ulong footerField;

	private ulong gutterField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified, DataType = "integer")]
	public string top
	{
		get
		{
			return topField;
		}
		set
		{
			topField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ulong right
	{
		get
		{
			return rightField;
		}
		set
		{
			rightField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified, DataType = "integer")]
	public string bottom
	{
		get
		{
			return bottomField;
		}
		set
		{
			bottomField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ulong left
	{
		get
		{
			return leftField;
		}
		set
		{
			leftField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ulong header
	{
		get
		{
			return headerField;
		}
		set
		{
			headerField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ulong footer
	{
		get
		{
			return footerField;
		}
		set
		{
			footerField = value;
		}
	}

	public ulong gutter
	{
		get
		{
			return gutterField;
		}
		set
		{
			gutterField = value;
		}
	}

	public static CT_PageMar Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_PageMar
		{
			top = XmlHelper.ReadString(node.Attributes["w:top"]),
			right = XmlHelper.ReadULong(node.Attributes["w:right"]),
			bottom = XmlHelper.ReadString(node.Attributes["w:bottom"]),
			left = XmlHelper.ReadULong(node.Attributes["w:left"]),
			header = XmlHelper.ReadULong(node.Attributes["w:header"]),
			footer = XmlHelper.ReadULong(node.Attributes["w:footer"]),
			gutter = XmlHelper.ReadULong(node.Attributes["w:gutter"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:top", top);
		XmlHelper.WriteAttribute(sw, "w:right", right);
		XmlHelper.WriteAttribute(sw, "w:bottom", bottom);
		XmlHelper.WriteAttribute(sw, "w:left", left);
		XmlHelper.WriteAttribute(sw, "w:header", header);
		XmlHelper.WriteAttribute(sw, "w:footer", footer);
		XmlHelper.WriteAttribute(sw, "w:gutter", gutter, writeIfBlank: true);
		sw.Write("/>");
	}
}
