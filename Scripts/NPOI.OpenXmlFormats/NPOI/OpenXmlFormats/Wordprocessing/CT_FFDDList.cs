using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_FFDDList
{
	private CT_DecimalNumber resultField;

	private CT_DecimalNumber defaultField;

	private List<CT_String> listEntryField;

	[XmlElement(Order = 0)]
	public CT_DecimalNumber result
	{
		get
		{
			return resultField;
		}
		set
		{
			resultField = value;
		}
	}

	[XmlElement(Order = 1)]
	public CT_DecimalNumber @default
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

	[XmlElement("listEntry", Order = 2)]
	public List<CT_String> listEntry
	{
		get
		{
			return listEntryField;
		}
		set
		{
			listEntryField = value;
		}
	}

	public CT_FFDDList()
	{
		listEntryField = new List<CT_String>();
		defaultField = new CT_DecimalNumber();
		resultField = new CT_DecimalNumber();
	}

	public static CT_FFDDList Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_FFDDList cT_FFDDList = new CT_FFDDList();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "result")
			{
				cT_FFDDList.resultField = CT_DecimalNumber.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "default")
			{
				cT_FFDDList.defaultField = CT_DecimalNumber.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "listEntry")
			{
				cT_FFDDList.listEntryField.Add(CT_String.Parse(childNode, namespaceManager));
			}
		}
		return cT_FFDDList;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}>");
		if (defaultField != null)
		{
			defaultField.Write(sw, "default");
		}
		if (resultField != null)
		{
			resultField.Write(sw, "result");
		}
		foreach (CT_String item in listEntry)
		{
			item.Write(sw, "listEntry");
		}
		sw.Write($"</w:{nodeName}>");
	}
}
