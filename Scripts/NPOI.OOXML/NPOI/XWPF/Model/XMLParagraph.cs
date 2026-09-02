using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.Model;

public class XMLParagraph
{
	protected CT_P paragraph;

	public XMLParagraph(CT_P paragraph)
	{
		this.paragraph = paragraph;
	}

	public CT_P GetCTP()
	{
		return paragraph;
	}
}
