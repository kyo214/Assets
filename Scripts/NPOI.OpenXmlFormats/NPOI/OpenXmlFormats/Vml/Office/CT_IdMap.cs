using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Vml.Office;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:office")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = true)]
public class CT_IdMap
{
	private ST_Ext extField;

	private string dataField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "urn:schemas-microsoft-com:vml")]
	public ST_Ext ext
	{
		get
		{
			return extField;
		}
		set
		{
			extField = value;
		}
	}

	[XmlIgnore]
	public bool extSpecified => extField != ST_Ext.NONE;

	[XmlAttribute]
	public string data
	{
		get
		{
			return dataField;
		}
		set
		{
			dataField = value;
		}
	}

	[XmlIgnore]
	public bool dataSpecified => dataField != null;

	public static CT_IdMap Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_IdMap cT_IdMap = new CT_IdMap();
		if (node.Attributes["v:ext"] != null)
		{
			cT_IdMap.ext = (ST_Ext)Enum.Parse(typeof(ST_Ext), node.Attributes["v:ext"].Value);
		}
		cT_IdMap.data = XmlHelper.ReadString(node.Attributes["data"]);
		return cT_IdMap;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<o:{nodeName}");
		XmlHelper.WriteAttribute(sw, "v:ext", ext.ToString());
		XmlHelper.WriteAttribute(sw, "data", data);
		sw.Write("/>");
	}
}
