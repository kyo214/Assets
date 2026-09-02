using System.Globalization;
using System.Text;
using NPOI.HSSF.Extractor;
using NPOI.OpenXml4Net.OPC;
using NPOI.SS.Extractor;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace NPOI.XSSF.Extractor;

public class XSSFExcelExtractor : POIXMLTextExtractor, IExcelExtractor
{
	public static XSSFRelation[] SUPPORTED_TYPES = new XSSFRelation[5]
	{
		XSSFRelation.WORKBOOK,
		XSSFRelation.MACRO_TEMPLATE_WORKBOOK,
		XSSFRelation.MACRO_ADDIN_WORKBOOK,
		XSSFRelation.TEMPLATE_WORKBOOK,
		XSSFRelation.MACROS_WORKBOOK
	};

	private XSSFWorkbook workbook;

	private bool includeSheetNames = true;

	private bool formulasNotResults;

	private bool includeCellComments;

	private bool includeHeadersFooters = true;

	private bool includeTextBoxes = true;

	private CultureInfo locale;

	public bool IncludeHeaderFooter
	{
		get
		{
			return includeHeadersFooters;
		}
		set
		{
			includeHeadersFooters = value;
		}
	}

	public bool IncludeSheetNames
	{
		get
		{
			return includeSheetNames;
		}
		set
		{
			includeSheetNames = value;
		}
	}

	public bool FormulasNotResults
	{
		get
		{
			return formulasNotResults;
		}
		set
		{
			formulasNotResults = value;
		}
	}

	public bool IncludeCellComments
	{
		get
		{
			return includeCellComments;
		}
		set
		{
			includeCellComments = value;
		}
	}

	public bool IncludeTextBoxes
	{
		get
		{
			return includeTextBoxes;
		}
		set
		{
			includeTextBoxes = value;
		}
	}

	public override string Text
	{
		get
		{
			DataFormatter formatter = ((locale != null) ? new DataFormatter(locale) : new DataFormatter());
			StringBuilder stringBuilder = new StringBuilder();
			foreach (XSSFSheet item in workbook)
			{
				if (includeSheetNames)
				{
					stringBuilder.Append(item.SheetName + "\n");
				}
				if (includeHeadersFooters)
				{
					stringBuilder.Append(ExtractHeaderFooter(item.FirstHeader));
					stringBuilder.Append(ExtractHeaderFooter(item.OddHeader));
					stringBuilder.Append(ExtractHeaderFooter(item.EvenHeader));
				}
				foreach (IRow item2 in item)
				{
					item2.GetEnumerator();
					bool flag = true;
					for (int i = 0; i < item2.LastCellNum; i++)
					{
						if (!flag)
						{
							stringBuilder.Append("\t");
						}
						else
						{
							flag = false;
						}
						ICell cell = item2.GetCell(i);
						if (cell == null)
						{
							continue;
						}
						if (cell.CellType == CellType.Formula)
						{
							if (formulasNotResults)
							{
								stringBuilder.Append(cell.CellFormula);
							}
							else if (cell.CachedFormulaResultType == CellType.String)
							{
								HandleStringCell(stringBuilder, cell);
							}
							else
							{
								HandleNonStringCell(stringBuilder, cell, formatter);
							}
						}
						else if (cell.CellType == CellType.String)
						{
							HandleStringCell(stringBuilder, cell);
						}
						else
						{
							HandleNonStringCell(stringBuilder, cell, formatter);
						}
						IComment cellComment = cell.CellComment;
						if (includeCellComments && cellComment != null)
						{
							string value = cellComment.String.String.Replace('\n', ' ');
							stringBuilder.Append(" Comment by ").Append(cellComment.Author).Append(": ")
								.Append(value);
						}
					}
					stringBuilder.Append("\n");
				}
				if (includeTextBoxes)
				{
					XSSFDrawing drawingPatriarch = item.GetDrawingPatriarch();
					if (drawingPatriarch != null)
					{
						foreach (XSSFShape shape in drawingPatriarch.GetShapes())
						{
							if (shape is XSSFSimpleShape)
							{
								string text = ((XSSFSimpleShape)shape).Text;
								if (text.Length > 0)
								{
									stringBuilder.Append(text);
									stringBuilder.Append('\n');
								}
							}
						}
					}
				}
				if (includeHeadersFooters)
				{
					stringBuilder.Append(ExtractHeaderFooter(item.FirstFooter));
					stringBuilder.Append(ExtractHeaderFooter(item.OddFooter));
					stringBuilder.Append(ExtractHeaderFooter(item.EvenFooter));
				}
			}
			return stringBuilder.ToString();
		}
	}

	public XSSFExcelExtractor(OPCPackage Container)
		: this(new XSSFWorkbook(Container))
	{
	}

	public XSSFExcelExtractor(XSSFWorkbook workbook)
		: base(workbook)
	{
		this.workbook = workbook;
	}

	public void SetIncludeSheetNames(bool includeSheetNames)
	{
		this.includeSheetNames = includeSheetNames;
	}

	public void SetFormulasNotResults(bool formulasNotResults)
	{
		this.formulasNotResults = formulasNotResults;
	}

	public void SetIncludeCellComments(bool includeCellComments)
	{
		this.includeCellComments = includeCellComments;
	}

	public void SetIncludeHeadersFooters(bool includeHeadersFooters)
	{
		this.includeHeadersFooters = includeHeadersFooters;
	}

	public void SetIncludeTextBoxes(bool includeTextBoxes)
	{
		this.includeTextBoxes = includeTextBoxes;
	}

	public void SetLocale(CultureInfo locale)
	{
		this.locale = locale;
	}

	private void HandleStringCell(StringBuilder text, ICell cell)
	{
		text.Append(cell.RichStringCellValue.String);
	}

	private void HandleNonStringCell(StringBuilder text, ICell cell, DataFormatter formatter)
	{
		CellType cellType = cell.CellType;
		if (cellType == CellType.Formula)
		{
			cellType = cell.CachedFormulaResultType;
		}
		if (cellType == CellType.Numeric)
		{
			ICellStyle cellStyle = cell.CellStyle;
			if (cellStyle.GetDataFormatString() != null)
			{
				text.Append(formatter.FormatRawCellContents(cell.NumericCellValue, cellStyle.DataFormat, cellStyle.GetDataFormatString()));
				return;
			}
		}
		XSSFCell xSSFCell = (XSSFCell)cell;
		text.Append(xSSFCell.GetRawValue());
	}

	private string ExtractHeaderFooter(IHeaderFooter hf)
	{
		return ExcelExtractor.ExtractHeaderFooter(hf);
	}
}
