using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_BooleanProperty
{
	private bool valField = true;

	[DefaultValue(true)]
	[XmlAttribute]
	public bool val
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

	public static CT_BooleanProperty Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_BooleanProperty cT_BooleanProperty = new CT_BooleanProperty();
		if (node.Attributes["val"] != null)
		{
			cT_BooleanProperty.val = XmlHelper.ReadBool(node.Attributes["val"]);
		}
		return cT_BooleanProperty;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		if (!val)
		{
			XmlHelper.WriteAttribute(sw, "val", val);
		}
		sw.Write("/>");
	}
}
