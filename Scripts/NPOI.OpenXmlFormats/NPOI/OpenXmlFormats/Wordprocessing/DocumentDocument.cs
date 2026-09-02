using System.IO;
using System.Xml;

namespace NPOI.OpenXmlFormats.Wordprocessing;

public class DocumentDocument
{
	private CT_Document document;

	public CT_Document Document => document;

	public DocumentDocument()
	{
	}

	public static DocumentDocument Parse(XmlDocument doc, XmlNamespaceManager namespaceMgr)
	{
		return new DocumentDocument(CT_Document.Parse(doc.DocumentElement, namespaceMgr));
	}

	public DocumentDocument(CT_Document document)
	{
		this.document = document;
	}

	public void Save(Stream stream)
	{
		using StreamWriter sw = new StreamWriter(stream);
		document.Write(sw);
	}
}
