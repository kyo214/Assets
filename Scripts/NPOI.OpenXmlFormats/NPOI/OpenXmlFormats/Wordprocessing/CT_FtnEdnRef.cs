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
public class CT_FtnEdnRef
{
	private ST_OnOff customMarkFollowsField;

	private bool customMarkFollowsFieldSpecified;

	private string idField;

	public XmlNode DomNode { get; set; }

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_OnOff customMarkFollows
	{
		get
		{
			return customMarkFollowsField;
		}
		set
		{
			customMarkFollowsField = value;
		}
	}

	[XmlIgnore]
	public bool customMarkFollowsSpecified
	{
		get
		{
			return customMarkFollowsFieldSpecified;
		}
		set
		{
			customMarkFollowsFieldSpecified = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships")]
	public string id
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

	public static CT_FtnEdnRef Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_FtnEdnRef cT_FtnEdnRef = new CT_FtnEdnRef();
		if (node.Attributes["w:customMarkFollows"] != null)
		{
			cT_FtnEdnRef.customMarkFollows = (ST_OnOff)Enum.Parse(typeof(ST_OnOff), node.Attributes["w:customMarkFollows"].Value, ignoreCase: true);
		}
		cT_FtnEdnRef.id = XmlHelper.ReadString(node.Attributes["w:id"]);
		cT_FtnEdnRef.DomNode = node;
		return cT_FtnEdnRef;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:customMarkFollows", customMarkFollows.ToString());
		XmlHelper.WriteAttribute(sw, "w:id", id);
		sw.Write(">");
		sw.Write($"</w:{nodeName}>");
	}
}
