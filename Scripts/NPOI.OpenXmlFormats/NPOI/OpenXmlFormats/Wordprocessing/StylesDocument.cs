using System.IO;
using System.Xml;

namespace NPOI.OpenXmlFormats.Wordprocessing;

public class StylesDocument
{
	private CT_Styles styles;

	public CT_Styles Styles => styles;

	public StylesDocument()
	{
		styles = new CT_Styles();
	}

	public static StylesDocument Parse(XmlDocument doc, XmlNamespaceManager namespaceMgr)
	{
		return new StylesDocument(CT_Styles.Parse(doc.DocumentElement, namespaceMgr));
	}

	public StylesDocument(CT_Styles styles)
	{
		this.styles = styles;
	}

	public void Save(Stream stream)
	{
		using StreamWriter sw = new StreamWriter(stream);
		styles.Write(sw);
	}
}
