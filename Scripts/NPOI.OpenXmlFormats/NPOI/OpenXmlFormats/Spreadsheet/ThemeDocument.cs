using System.IO;
using System.Xml;
using NPOI.OpenXmlFormats.Dml;

namespace NPOI.OpenXmlFormats.Spreadsheet;

public class ThemeDocument
{
	private CT_OfficeStyleSheet stylesheet;

	public ThemeDocument()
	{
	}

	public ThemeDocument(CT_OfficeStyleSheet stylesheet)
	{
		this.stylesheet = stylesheet;
	}

	public CT_OfficeStyleSheet GetTheme()
	{
		return stylesheet;
	}

	public void Save(Stream stream)
	{
		using StreamWriter sw = new StreamWriter(stream);
		stylesheet.Write(sw);
	}

	public static ThemeDocument Parse(XmlDocument xmldoc, XmlNamespaceManager namespaceManager)
	{
		return new ThemeDocument(CT_OfficeStyleSheet.Parse(xmldoc.DocumentElement, namespaceManager));
	}

	public CT_OfficeStyleSheet AddNewTheme()
	{
		stylesheet = new CT_OfficeStyleSheet();
		return stylesheet;
	}
}
