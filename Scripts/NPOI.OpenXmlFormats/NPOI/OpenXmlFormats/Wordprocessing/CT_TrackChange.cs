using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlInclude(typeof(CT_RunTrackChange))]
[XmlInclude(typeof(CT_RPrChange))]
[XmlInclude(typeof(CT_ParaRPrChange))]
[XmlInclude(typeof(CT_PPrChange))]
[XmlInclude(typeof(CT_SectPrChange))]
[XmlInclude(typeof(CT_TblPrChange))]
[XmlInclude(typeof(CT_TrPrChange))]
[XmlInclude(typeof(CT_TcPrChange))]
[XmlInclude(typeof(CT_TblPrExChange))]
[XmlInclude(typeof(CT_TrackChangeNumbering))]
[XmlInclude(typeof(CT_Comment))]
[XmlInclude(typeof(CT_TrackChangeRange))]
[XmlInclude(typeof(CT_CellMergeTrackChange))]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_TrackChange : CT_Markup
{
	private string authorField;

	private string dateField;

	private bool dateFieldSpecified;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string author
	{
		get
		{
			return authorField;
		}
		set
		{
			authorField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string date
	{
		get
		{
			return dateField;
		}
		set
		{
			dateField = value;
		}
	}

	[XmlIgnore]
	public bool dateSpecified
	{
		get
		{
			return dateFieldSpecified;
		}
		set
		{
			dateFieldSpecified = value;
		}
	}

	public new static CT_TrackChange Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_TrackChange
		{
			author = XmlHelper.ReadString(node.Attributes["w:author"]),
			date = XmlHelper.ReadString(node.Attributes["w:date"]),
			id = XmlHelper.ReadString(node.Attributes["w:id"])
		};
	}

	internal new void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:author", author, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "w:date", date);
		XmlHelper.WriteAttribute(sw, "w:id", base.id, writeIfBlank: true);
		sw.Write("/>");
	}
}
