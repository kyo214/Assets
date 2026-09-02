using System.IO;
using System.Xml;

namespace NPOI.OpenXmlFormats.Wordprocessing;

public class NumberingDocument
{
	private CT_Numbering numbering;

	public CT_Numbering Numbering => numbering;

	public NumberingDocument()
	{
		numbering = new CT_Numbering();
	}

	public NumberingDocument(CT_Numbering numbering)
	{
		this.numbering = numbering;
	}

	public void Save(Stream stream)
	{
		using StreamWriter sw = new StreamWriter(stream);
		numbering.Write(sw);
	}

	public static NumberingDocument Parse(XmlDocument doc, XmlNamespaceManager NameSpaceManager)
	{
		return new NumberingDocument(CT_Numbering.Parse(doc.DocumentElement, NameSpaceManager));
	}
}
