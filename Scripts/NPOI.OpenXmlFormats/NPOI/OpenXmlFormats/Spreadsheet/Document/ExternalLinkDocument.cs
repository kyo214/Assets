using System.IO;
using System.Xml;

namespace NPOI.OpenXmlFormats.Spreadsheet.Document;

public class ExternalLinkDocument
{
	private CT_ExternalLink link;

	public CT_ExternalLink ExternalLink
	{
		get
		{
			return link;
		}
		set
		{
			link = value;
		}
	}

	public ExternalLinkDocument()
	{
	}

	public ExternalLinkDocument(CT_ExternalLink link)
	{
		this.link = link;
	}

	public static ExternalLinkDocument Parse(XmlDocument xmldoc, XmlNamespaceManager namespaceMgr)
	{
		return new ExternalLinkDocument(CT_ExternalLink.Parse(xmldoc.DocumentElement, namespaceMgr));
	}

	public void Save(Stream stream)
	{
		using StreamWriter sw = new StreamWriter(stream);
		link.Write(sw);
	}
}
