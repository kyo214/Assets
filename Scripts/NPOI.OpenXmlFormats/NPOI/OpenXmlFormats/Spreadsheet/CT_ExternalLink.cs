using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true, ElementName = "externalLink")]
public class CT_ExternalLink
{
	private object itemField;

	private CT_ExternalBook externalBookField;

	private CT_DdeLink ddeLinkField;

	private CT_OleLink oleLinkField;

	private CT_ExtensionList extLstField;

	public ExternalLinkItem itemType { get; set; }

	[XmlElement("ddeLink", typeof(CT_DdeLink))]
	[XmlElement("extLst", typeof(CT_ExtensionList))]
	[XmlElement("externalBook", typeof(CT_ExternalBook))]
	[XmlElement("oleLink", typeof(CT_OleLink))]
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

	public CT_ExternalBook externalBook
	{
		get
		{
			return externalBookField;
		}
		set
		{
			externalBookField = value;
		}
	}

	public CT_DdeLink ddlLink
	{
		get
		{
			return ddeLinkField;
		}
		set
		{
			ddeLinkField = value;
		}
	}

	public CT_OleLink oleLink
	{
		get
		{
			return oleLinkField;
		}
		set
		{
			oleLinkField = value;
		}
	}

	public CT_ExtensionList extLst
	{
		get
		{
			return extLstField;
		}
		set
		{
			extLstField = value;
		}
	}

	public static CT_ExternalLink Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_ExternalLink cT_ExternalLink = new CT_ExternalLink();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "externalBook")
			{
				cT_ExternalLink.externalBookField = CT_ExternalBook.Parse(childNode, namespaceManager);
				cT_ExternalLink.itemField = cT_ExternalLink.externalBookField;
				cT_ExternalLink.itemType = ExternalLinkItem.externalBook;
			}
			else if (childNode.LocalName == "ddeLink")
			{
				cT_ExternalLink.ddeLinkField = CT_DdeLink.Parse(childNode, namespaceManager);
				cT_ExternalLink.itemField = cT_ExternalLink.ddeLinkField;
				cT_ExternalLink.itemType = ExternalLinkItem.ddeLink;
			}
			else if (childNode.LocalName == "oleLink")
			{
				cT_ExternalLink.oleLinkField = CT_OleLink.Parse(childNode, namespaceManager);
				cT_ExternalLink.itemField = cT_ExternalLink.oleLinkField;
				cT_ExternalLink.itemType = ExternalLinkItem.oleLink;
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_ExternalLink.extLstField = CT_ExtensionList.Parse(childNode, namespaceManager);
				cT_ExternalLink.itemField = cT_ExternalLink.extLstField;
				cT_ExternalLink.itemType = ExternalLinkItem.extLst;
			}
		}
		return cT_ExternalLink;
	}

	internal void Write(StreamWriter sw)
	{
		sw.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
		sw.Write("<externalLink xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\" mc:Ignorable=\"x14\" xmlns:x14=\"http://schemas.microsoft.com/office/spreadsheetml/2009/9/main\">");
		if (externalBookField != null)
		{
			externalBookField.Write(sw, "externalBook");
		}
		if (ddeLinkField != null)
		{
			ddeLinkField.Write(sw, "ddeLink");
		}
		if (extLstField != null)
		{
			extLstField.Write(sw, "extLst");
		}
		if (oleLinkField != null)
		{
			oleLinkField.Write(sw, "oleLink");
		}
		sw.Write("</externalLink>");
	}

	public void AddNewExternalBook()
	{
		externalBookField = new CT_ExternalBook();
	}
}
