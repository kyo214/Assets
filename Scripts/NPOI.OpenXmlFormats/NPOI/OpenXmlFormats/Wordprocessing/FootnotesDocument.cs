using System.IO;
using System.Xml;

namespace NPOI.OpenXmlFormats.Wordprocessing;

public class FootnotesDocument
{
	private CT_Footnotes footnotes;

	public CT_Footnotes Footnotes => footnotes;

	public FootnotesDocument()
	{
		footnotes = new CT_Footnotes();
	}

	public static FootnotesDocument Parse(XmlDocument doc, XmlNamespaceManager namespaceMgr)
	{
		return new FootnotesDocument(CT_Footnotes.Parse(doc.DocumentElement, namespaceMgr));
	}

	public FootnotesDocument(CT_Footnotes footnotes)
	{
		this.footnotes = footnotes;
	}

	public void Save(Stream stream)
	{
		using StreamWriter sw = new StreamWriter(stream);
		footnotes.Write(sw);
	}
}
