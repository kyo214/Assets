using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XSSF.UserModel.Extensions;

namespace NPOI.XSSF.Model;

public class StylesTable : POIXMLDocumentPart
{
	private SortedDictionary<short, string> numberFormats = new SortedDictionary<short, string>();

	private bool[] usedNumberFormats = new bool[SpreadsheetVersion.EXCEL2007.MaxCellStyles];

	private List<XSSFFont> fonts = new List<XSSFFont>();

	private List<XSSFCellFill> fills = new List<XSSFCellFill>();

	private List<XSSFCellBorder> borders = new List<XSSFCellBorder>();

	private List<CT_Xf> styleXfs = new List<CT_Xf>();

	private List<CT_Xf> xfs = new List<CT_Xf>();

	private List<CT_Dxf> dxfs = new List<CT_Dxf>();

	public static int FIRST_CUSTOM_STYLE_ID = 165;

	private static int MAXIMUM_STYLE_ID = SpreadsheetVersion.EXCEL2007.MaxCellStyles;

	private static short FIRST_USER_DEFINED_NUMBER_FORMAT_ID = 164;

	private int MAXIMUM_NUMBER_OF_DATA_FORMATS = 250;

	private StyleSheetDocument doc;

	private XSSFWorkbook workbook;

	private ThemesTable theme;

	public int MaxNumberOfDataFormats
	{
		get
		{
			return MAXIMUM_NUMBER_OF_DATA_FORMATS;
		}
		set
		{
			if (value < NumDataFormats)
			{
				if (value < 0)
				{
					throw new ArgumentException("Maximum Number of Data Formats must be greater than or equal to 0");
				}
				throw new InvalidOperationException("Cannot set the maximum number of data formats less than the current quantity.Data formats must be explicitly removed (via StylesTable.removeNumberFormat) before the limit can be decreased.");
			}
			MAXIMUM_NUMBER_OF_DATA_FORMATS = value;
		}
	}

	public int NumCellStyles => xfs.Count;

	public int NumDataFormats => numberFormats.Count;

	[Obsolete("deprecated POI 3.14 beta 2. Use {@link #getNumDataFormats()} instead.")]
	internal int NumberFormatSize => numberFormats.Count;

	internal int XfsSize => xfs.Count;

	internal int StyleXfsSize => styleXfs.Count;

	internal int DXfsSize => dxfs.Count;

	public StylesTable()
	{
		doc = new StyleSheetDocument();
		doc.AddNewStyleSheet();
		Initialize();
	}

	internal StylesTable(PackagePart part)
		: base(part)
	{
		XmlDocument xmldoc = POIXMLDocumentPart.ConvertStreamToXml(part.GetInputStream());
		ReadFrom(xmldoc);
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public StylesTable(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	public void SetWorkbook(XSSFWorkbook wb)
	{
		workbook = wb;
	}

	public ThemesTable GetTheme()
	{
		return theme;
	}

	public void SetTheme(ThemesTable theme)
	{
		this.theme = theme;
		foreach (XSSFFont font in fonts)
		{
			font.SetThemesTable(theme);
		}
		foreach (XSSFCellBorder border in borders)
		{
			border.SetThemesTable(theme);
		}
	}

	public void EnsureThemesTable()
	{
		if (theme == null)
		{
			theme = (ThemesTable)workbook.CreateRelationship(XSSFRelation.THEME, XSSFFactory.GetInstance());
		}
	}

	protected void ReadFrom(XmlDocument xmldoc)
	{
		try
		{
			doc = StyleSheetDocument.Parse(xmldoc, POIXMLDocumentPart.NamespaceManager);
			CT_Stylesheet styleSheet = doc.GetStyleSheet();
			CT_NumFmts numFmts = styleSheet.numFmts;
			if (numFmts != null)
			{
				foreach (CT_NumFmt item2 in numFmts.numFmt)
				{
					short key = (short)item2.numFmtId;
					numberFormats.Add(key, item2.formatCode);
				}
			}
			CT_Fonts cT_Fonts = styleSheet.fonts;
			if (cT_Fonts != null)
			{
				int num = 0;
				foreach (CT_Font item3 in cT_Fonts.font)
				{
					XSSFFont item = new XSSFFont(item3, num);
					fonts.Add(item);
					num++;
				}
			}
			CT_Fills cT_Fills = styleSheet.fills;
			if (cT_Fills != null)
			{
				foreach (CT_Fill item4 in cT_Fills.fill)
				{
					fills.Add(new XSSFCellFill(item4));
				}
			}
			CT_Borders cT_Borders = styleSheet.borders;
			if (cT_Borders != null)
			{
				foreach (CT_Border item5 in cT_Borders.border)
				{
					borders.Add(new XSSFCellBorder(item5));
				}
			}
			CT_CellXfs cellXfs = styleSheet.cellXfs;
			if (cellXfs != null)
			{
				xfs.AddRange(cellXfs.xf);
			}
			CT_CellStyleXfs cellStyleXfs = styleSheet.cellStyleXfs;
			if (cellStyleXfs != null)
			{
				styleXfs.AddRange(cellStyleXfs.xf);
			}
			CT_Dxfs cT_Dxfs = styleSheet.dxfs;
			if (cT_Dxfs != null)
			{
				dxfs.AddRange(cT_Dxfs.dxf);
			}
		}
		catch (XmlException ex)
		{
			throw new IOException(ex.Message);
		}
	}

	[Obsolete("deprecated POI 3.14-beta2. Use {@link #getNumberFormatAt(short)} instead.")]
	public string GetNumberFormatAt(int idx)
	{
		return GetNumberFormatAt((short)idx);
	}

	public string GetNumberFormatAt(short fmtId)
	{
		if (numberFormats.ContainsKey(fmtId))
		{
			return numberFormats[fmtId];
		}
		return null;
	}

	private short GetNumberFormatId(string fmt)
	{
		foreach (KeyValuePair<short, string> numberFormat in numberFormats)
		{
			if (numberFormat.Value.Equals(fmt))
			{
				return numberFormat.Key;
			}
		}
		throw new InvalidOperationException("Number format not in style table: " + fmt);
	}

	public int PutNumberFormat(string fmt)
	{
		if (numberFormats.ContainsValue(fmt))
		{
			try
			{
				return GetNumberFormatId(fmt);
			}
			catch (InvalidOperationException)
			{
				throw new InvalidOperationException("Found the format, but couldn't figure out where - should never happen!");
			}
		}
		if (numberFormats.Count >= MAXIMUM_NUMBER_OF_DATA_FORMATS)
		{
			throw new InvalidOperationException("The maximum number of Data Formats was exceeded. You can define up to " + MAXIMUM_NUMBER_OF_DATA_FORMATS + " formats in a .xlsx Workbook.");
		}
		short num;
		if (numberFormats.Count == 0)
		{
			num = FIRST_USER_DEFINED_NUMBER_FORMAT_ID;
		}
		else
		{
			short num2 = (short)(numberFormats.Last().Key + 1);
			if (num2 < 0)
			{
				throw new InvalidOperationException("Cowardly avoiding creating a number format with a negative id.This is probably due to arithmetic overflow.");
			}
			num = Math.Max(num2, FIRST_USER_DEFINED_NUMBER_FORMAT_ID);
		}
		if (numberFormats.ContainsKey(num))
		{
			numberFormats[num] = fmt;
		}
		else
		{
			numberFormats.Add(num, fmt);
		}
		return num;
	}

	public void PutNumberFormat(short index, string fmt)
	{
		if (numberFormats.ContainsKey(index))
		{
			numberFormats[index] = fmt;
		}
		else
		{
			numberFormats.Add(index, fmt);
		}
	}

	public bool RemoveNumberFormat(short index)
	{
		_ = numberFormats[index];
		bool flag = numberFormats.Remove(index);
		if (flag)
		{
			foreach (CT_Xf xf in xfs)
			{
				if (xf.numFmtIdSpecified && xf.numFmtId == index)
				{
					xf.applyNumberFormat = false;
					xf.numFmtId = 0u;
					xf.numFmtIdSpecified = false;
				}
			}
		}
		return flag;
	}

	public bool RemoveNumberFormat(string fmt)
	{
		short numberFormatId = GetNumberFormatId(fmt);
		return RemoveNumberFormat(numberFormatId);
	}

	public XSSFFont GetFontAt(int idx)
	{
		return fonts[idx];
	}

	public int PutFont(XSSFFont font, bool forceRegistration)
	{
		int num = -1;
		if (!forceRegistration)
		{
			num = fonts.IndexOf(font);
		}
		if (num != -1)
		{
			return num;
		}
		num = fonts.Count;
		fonts.Add(font);
		return num;
	}

	public int PutFont(XSSFFont font)
	{
		return PutFont(font, forceRegistration: false);
	}

	public XSSFCellStyle GetStyleAt(int idx)
	{
		int cellStyleXfId = 0;
		if (xfs.Count == 0)
		{
			return null;
		}
		if (xfs[idx].xfId != 0)
		{
			cellStyleXfId = (int)xfs[idx].xfId;
		}
		return new XSSFCellStyle(idx, cellStyleXfId, this, theme);
	}

	public int PutStyle(XSSFCellStyle style)
	{
		CT_Xf coreXf = style.GetCoreXf();
		if (!xfs.Contains(coreXf))
		{
			xfs.Add(coreXf);
		}
		return xfs.IndexOf(coreXf);
	}

	public XSSFCellBorder GetBorderAt(int idx)
	{
		return borders[idx];
	}

	public int PutBorder(XSSFCellBorder border)
	{
		int num = borders.IndexOf(border);
		if (num != -1)
		{
			return num;
		}
		borders.Add(border);
		border.SetThemesTable(theme);
		return borders.Count - 1;
	}

	public XSSFCellFill GetFillAt(int idx)
	{
		return fills[idx];
	}

	public ReadOnlyCollection<XSSFCellBorder> GetBorders()
	{
		return borders.AsReadOnly();
	}

	public ReadOnlyCollection<XSSFCellFill> GetFills()
	{
		return fills.AsReadOnly();
	}

	public ReadOnlyCollection<XSSFFont> GetFonts()
	{
		return fonts.AsReadOnly();
	}

	public IDictionary<short, string> GetNumberFormats()
	{
		return numberFormats;
	}

	public int PutFill(XSSFCellFill fill)
	{
		int num = fills.IndexOf(fill);
		if (num != -1)
		{
			return num;
		}
		fills.Add(fill);
		return fills.Count - 1;
	}

	internal CT_Xf GetCellXfAt(int idx)
	{
		return xfs[idx];
	}

	internal int PutCellXf(CT_Xf cellXf)
	{
		xfs.Add(cellXf);
		return xfs.Count;
	}

	internal void ReplaceCellXfAt(int idx, CT_Xf cellXf)
	{
		xfs[idx] = cellXf;
	}

	internal CT_Xf GetCellStyleXfAt(int idx)
	{
		if (idx < 0 || idx > styleXfs.Count)
		{
			return null;
		}
		return styleXfs[idx];
	}

	internal int PutCellStyleXf(CT_Xf cellStyleXf)
	{
		styleXfs.Add(cellStyleXf);
		return styleXfs.Count;
	}

	internal void ReplaceCellStyleXfAt(int idx, CT_Xf cellStyleXf)
	{
		styleXfs[idx] = cellStyleXf;
	}

	internal CT_Stylesheet GetCTStylesheet()
	{
		return doc.GetStyleSheet();
	}

	public void WriteTo(Stream out1)
	{
		CT_Stylesheet styleSheet = doc.GetStyleSheet();
		CT_NumFmts cT_NumFmts = new CT_NumFmts();
		cT_NumFmts.count = (uint)numberFormats.Count;
		foreach (KeyValuePair<short, string> numberFormat in numberFormats)
		{
			CT_NumFmt cT_NumFmt = cT_NumFmts.AddNewNumFmt();
			cT_NumFmt.numFmtId = (uint)numberFormat.Key;
			cT_NumFmt.formatCode = numberFormat.Value;
		}
		styleSheet.numFmts = cT_NumFmts;
		CT_Fonts cT_Fonts = styleSheet.fonts;
		if (cT_Fonts == null)
		{
			cT_Fonts = new CT_Fonts();
		}
		cT_Fonts.count = (uint)fonts.Count;
		if (cT_Fonts.count != 0)
		{
			cT_Fonts.countSpecified = true;
		}
		List<CT_Font> list = new List<CT_Font>(fonts.Count);
		foreach (XSSFFont font in fonts)
		{
			list.Add(font.GetCTFont());
		}
		cT_Fonts.SetFontArray(list);
		styleSheet.fonts = cT_Fonts;
		CT_Fills cT_Fills = styleSheet.fills;
		if (cT_Fills == null)
		{
			cT_Fills = new CT_Fills();
		}
		cT_Fills.count = (uint)fills.Count;
		List<CT_Fill> list2 = new List<CT_Fill>(fills.Count);
		foreach (XSSFCellFill fill in fills)
		{
			list2.Add(fill.GetCTFill());
		}
		cT_Fills.SetFillArray(list2);
		if (cT_Fills.count != 0)
		{
			cT_Fills.countSpecified = true;
		}
		styleSheet.fills = cT_Fills;
		CT_Borders cT_Borders = styleSheet.borders;
		if (cT_Borders == null)
		{
			cT_Borders = new CT_Borders();
		}
		cT_Borders.count = (uint)borders.Count;
		List<CT_Border> list3 = new List<CT_Border>(borders.Count);
		foreach (XSSFCellBorder border in borders)
		{
			list3.Add(border.GetCTBorder());
		}
		cT_Borders.SetBorderArray(list3);
		styleSheet.borders = cT_Borders;
		if (xfs.Count > 0)
		{
			CT_CellXfs cT_CellXfs = styleSheet.cellXfs;
			if (cT_CellXfs == null)
			{
				cT_CellXfs = new CT_CellXfs();
			}
			cT_CellXfs.count = (uint)xfs.Count;
			if (cT_CellXfs.count != 0)
			{
				cT_CellXfs.countSpecified = true;
			}
			cT_CellXfs.xf = xfs;
			styleSheet.cellXfs = cT_CellXfs;
		}
		if (styleXfs.Count > 0)
		{
			CT_CellStyleXfs cT_CellStyleXfs = styleSheet.cellStyleXfs;
			if (cT_CellStyleXfs == null)
			{
				cT_CellStyleXfs = new CT_CellStyleXfs();
			}
			cT_CellStyleXfs.count = (uint)styleXfs.Count;
			if (cT_CellStyleXfs.count != 0)
			{
				cT_CellStyleXfs.countSpecified = true;
			}
			cT_CellStyleXfs.xf = styleXfs;
			styleSheet.cellStyleXfs = cT_CellStyleXfs;
		}
		if (dxfs.Count > 0)
		{
			CT_Dxfs cT_Dxfs = styleSheet.dxfs;
			if (cT_Dxfs == null)
			{
				cT_Dxfs = new CT_Dxfs();
			}
			cT_Dxfs.count = (uint)dxfs.Count;
			if (cT_Dxfs.count != 0)
			{
				cT_Dxfs.countSpecified = true;
			}
			cT_Dxfs.dxf = dxfs;
			styleSheet.dxfs = cT_Dxfs;
		}
		doc.Save(out1);
	}

	protected internal override void Commit()
	{
		Stream outputStream = GetPackagePart().GetOutputStream();
		WriteTo(outputStream);
		outputStream.Close();
	}

	private void Initialize()
	{
		XSSFFont item = CreateDefaultFont();
		fonts.Add(item);
		CT_Fill[] array = CreateDefaultFills();
		fills.Add(new XSSFCellFill(array[0]));
		fills.Add(new XSSFCellFill(array[1]));
		CT_Border border = CreateDefaultBorder();
		borders.Add(new XSSFCellBorder(border));
		CT_Xf item2 = CreateDefaultXf();
		styleXfs.Add(item2);
		CT_Xf cT_Xf = CreateDefaultXf();
		cT_Xf.xfId = 0u;
		xfs.Add(cT_Xf);
	}

	private static CT_Xf CreateDefaultXf()
	{
		return new CT_Xf
		{
			numFmtId = 0u,
			fontId = 0u,
			fillId = 0u,
			borderId = 0u
		};
	}

	private static CT_Border CreateDefaultBorder()
	{
		CT_Border cT_Border = new CT_Border();
		cT_Border.AddNewLeft();
		cT_Border.AddNewRight();
		cT_Border.AddNewTop();
		cT_Border.AddNewBottom();
		cT_Border.AddNewDiagonal();
		return cT_Border;
	}

	private static CT_Fill[] CreateDefaultFills()
	{
		CT_Fill[] obj = new CT_Fill[2]
		{
			new CT_Fill(),
			new CT_Fill()
		};
		obj[0].AddNewPatternFill().patternType = ST_PatternType.none;
		obj[1].AddNewPatternFill().patternType = ST_PatternType.darkGray;
		return obj;
	}

	private static XSSFFont CreateDefaultFont()
	{
		XSSFFont xSSFFont = new XSSFFont(new CT_Font(), 0);
		xSSFFont.FontHeightInPoints = 11.0;
		xSSFFont.Color = XSSFFont.DEFAULT_FONT_COLOR;
		xSSFFont.FontName = "Calibri";
		xSSFFont.SetFamily(FontFamily.SWISS);
		xSSFFont.SetScheme(FontScheme.MINOR);
		return xSSFFont;
	}

	public CT_Dxf GetDxfAt(int idx)
	{
		return dxfs[idx];
	}

	public int PutDxf(CT_Dxf dxf)
	{
		dxfs.Add(dxf);
		return dxfs.Count;
	}

	public XSSFCellStyle CreateCellStyle()
	{
		if (NumCellStyles > MAXIMUM_STYLE_ID)
		{
			throw new InvalidOperationException("The maximum number of Cell Styles was exceeded. You can define up to " + MAXIMUM_STYLE_ID + " style in a .xlsx Workbook");
		}
		int count = styleXfs.Count;
		CT_Xf cT_Xf = new CT_Xf();
		cT_Xf.numFmtId = 0u;
		cT_Xf.fontId = 0u;
		cT_Xf.fillId = 0u;
		cT_Xf.borderId = 0u;
		cT_Xf.xfId = 0u;
		return new XSSFCellStyle(PutCellXf(cT_Xf) - 1, count - 1, this, theme);
	}

	[Obsolete("deprecated POI 3.15 beta 2. Use {@link #findFont(boolean, short, short, String, boolean, boolean, short, byte)} instead.")]
	public XSSFFont FindFont(short boldWeight, short color, short fontHeight, string name, bool italic, bool strikeout, FontSuperScript typeOffset, FontUnderlineType underline)
	{
		foreach (XSSFFont font in fonts)
		{
			if (font.Boldweight == boldWeight && font.Color == color && font.FontHeight == (double)fontHeight && font.FontName.Equals(name) && font.IsItalic == italic && font.IsStrikeout == strikeout && font.TypeOffset == typeOffset && font.Underline == underline)
			{
				return font;
			}
		}
		return null;
	}

	public XSSFFont FindFont(bool bold, short color, short fontHeight, string name, bool italic, bool strikeout, FontSuperScript typeOffset, FontUnderlineType underline)
	{
		foreach (XSSFFont font in fonts)
		{
			if (font.IsBold == bold && font.Color == color && font.FontHeight == (double)fontHeight && font.FontName.Equals(name) && font.IsItalic == italic && font.IsStrikeout == strikeout && font.TypeOffset == typeOffset && font.Underline == underline)
			{
				return font;
			}
		}
		return null;
	}
}
