using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_FFCheckBox
{
	private object itemField;

	private CT_OnOff defaultField;

	private CT_OnOff checkedField;

	[XmlElement("size", typeof(CT_HpsMeasure), Order = 0)]
	[XmlElement("sizeAuto", typeof(CT_OnOff), Order = 0)]
	public object Item
	{
		get
		{
			return itemField;
		}
		set
		{
			itemField = value;
		}
	}

	[XmlElement(Order = 1)]
	public CT_OnOff @default
	{
		get
		{
			return defaultField;
		}
		set
		{
			defaultField = value;
		}
	}

	[XmlElement(Order = 2)]
	public CT_OnOff @checked
	{
		get
		{
			return checkedField;
		}
		set
		{
			checkedField = value;
		}
	}

	public CT_FFCheckBox()
	{
		checkedField = new CT_OnOff();
		defaultField = new CT_OnOff();
	}

	public static CT_FFCheckBox Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_FFCheckBox cT_FFCheckBox = new CT_FFCheckBox();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "checked")
			{
				cT_FFCheckBox.checkedField = CT_OnOff.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "default")
			{
				cT_FFCheckBox.defaultField = CT_OnOff.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "size")
			{
				cT_FFCheckBox.itemField = CT_HpsMeasure.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "sizeAuto")
			{
				cT_FFCheckBox.itemField = CT_OnOff.Parse(childNode, namespaceManager);
			}
		}
		return cT_FFCheckBox;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}>");
		if (defaultField != null)
		{
			defaultField.Write(sw, "default");
		}
		if (checkedField != null)
		{
			checkedField.Write(sw, "checked");
		}
		if (itemField != null)
		{
			if (itemField is CT_OnOff)
			{
				(itemField as CT_OnOff).Write(sw, "sizeAuto");
			}
			else
			{
				(itemField as CT_HpsMeasure).Write(sw, "size");
			}
		}
		sw.Write($"</w:{nodeName}>");
	}
}
