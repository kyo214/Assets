using System;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Dml;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.XSSF.UserModel;

namespace NPOI.XSSF.Model;

public class ThemesTable : POIXMLDocumentPart
{
	public const int THEME_LT1 = 0;

	public const int THEME_DK1 = 1;

	public const int THEME_LT2 = 2;

	public const int THEME_DK2 = 3;

	public const int THEME_ACCENT1 = 4;

	public const int THEME_ACCENT2 = 5;

	public const int THEME_ACCENT3 = 6;

	public const int THEME_ACCENT4 = 7;

	public const int THEME_ACCENT5 = 8;

	public const int THEME_ACCENT6 = 9;

	public const int THEME_HLINK = 10;

	public const int THEME_FOLHLINK = 11;

	private ThemeDocument theme;

	public ThemesTable()
	{
		theme = new ThemeDocument();
		theme.AddNewTheme().AddNewThemeElements();
	}

	public ThemesTable(PackagePart part)
		: base(part)
	{
		XmlDocument xmldoc = POIXMLDocumentPart.ConvertStreamToXml(part.GetInputStream());
		try
		{
			theme = ThemeDocument.Parse(xmldoc, POIXMLDocumentPart.NamespaceManager);
		}
		catch (XmlException ex)
		{
			throw new IOException(ex.Message, ex);
		}
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public ThemesTable(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	internal ThemesTable(ThemeDocument theme)
	{
		this.theme = theme;
	}

	public XSSFColor GetThemeColor(int idx)
	{
		CT_ColorScheme clrScheme = theme.GetTheme().themeElements.clrScheme;
		NPOI.OpenXmlFormats.Dml.CT_Color cT_Color = null;
		switch (idx)
		{
		case 0:
			cT_Color = clrScheme.lt1;
			break;
		case 1:
			cT_Color = clrScheme.dk1;
			break;
		case 2:
			cT_Color = clrScheme.lt2;
			break;
		case 3:
			cT_Color = clrScheme.dk2;
			break;
		case 4:
			cT_Color = clrScheme.accent1;
			break;
		case 5:
			cT_Color = clrScheme.accent2;
			break;
		case 6:
			cT_Color = clrScheme.accent3;
			break;
		case 7:
			cT_Color = clrScheme.accent4;
			break;
		case 8:
			cT_Color = clrScheme.accent5;
			break;
		case 9:
			cT_Color = clrScheme.accent6;
			break;
		case 10:
			cT_Color = clrScheme.hlink;
			break;
		case 11:
			cT_Color = clrScheme.folHlink;
			break;
		default:
			return null;
		}
		byte[] array = null;
		if (cT_Color.IsSetSrgbClr())
		{
			array = cT_Color.srgbClr.val;
		}
		else
		{
			if (!cT_Color.IsSetSysClr())
			{
				return null;
			}
			array = cT_Color.sysClr.lastClr;
		}
		return new XSSFColor(array);
	}

	public void InheritFromThemeAsRequired(XSSFColor color)
	{
		if (color != null && color.GetCTColor().themeSpecified)
		{
			XSSFColor themeColor = GetThemeColor(color.Theme);
			color.GetCTColor().SetRgb(themeColor.GetCTColor().GetRgb());
		}
	}

	public void writeTo(Stream out1)
	{
		theme.Save(out1);
	}

	protected internal override void Commit()
	{
		Stream outputStream = GetPackagePart().GetOutputStream();
		writeTo(outputStream);
		outputStream.Close();
	}
}
