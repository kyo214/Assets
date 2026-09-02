using System;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.Extractor;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.Extractor;

public class ExcelExtractor : POIOLE2TextExtractor, IExcelExtractor
{
	private HSSFWorkbook wb;

	private HSSFDataFormatter _formatter;

	private bool includeSheetNames = true;

	private bool formulasNotResults;

	private bool includeCellComments;

	private bool includeBlankCells;

	private bool includeHeaderFooter = true;

	public bool IncludeHeaderFooter
	{
		get
		{
			return includeHeaderFooter;
		}
		set
		{
			includeHeaderFooter = value;
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

	public bool IncludeBlankCells
	{
		get
		{
			return includeBlankCells;
		}
		set
		{
			includeBlankCells = value;
		}
	}

	public override string Text
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			wb.MissingCellPolicy = MissingCellPolicy.RETURN_BLANK_AS_NULL;
			for (int i = 0; i < wb.NumberOfSheets; i++)
			{
				HSSFSheet hSSFSheet = (HSSFSheet)wb.GetSheetAt(i);
				if (hSSFSheet == null)
				{
					continue;
				}
				if (includeSheetNames)
				{
					string sheetName = wb.GetSheetName(i);
					if (sheetName != null)
					{
						stringBuilder.Append(sheetName);
						stringBuilder.Append("\n");
					}
				}
				if (hSSFSheet.Header != null && includeHeaderFooter)
				{
					stringBuilder.Append(ExtractHeaderFooter(hSSFSheet.Header));
				}
				int firstRowNum = hSSFSheet.FirstRowNum;
				int lastRowNum = hSSFSheet.LastRowNum;
				for (int j = firstRowNum; j <= lastRowNum; j++)
				{
					IRow row = hSSFSheet.GetRow(j);
					if (row == null)
					{
						continue;
					}
					int num = row.FirstCellNum;
					int lastCellNum = row.LastCellNum;
					if (includeBlankCells)
					{
						num = 0;
					}
					for (int k = num; k < lastCellNum; k++)
					{
						ICell cell = row.GetCell(k);
						bool flag = true;
						if (cell == null)
						{
							flag = includeBlankCells;
						}
						else
						{
							switch (cell.CellType)
							{
							case CellType.String:
								stringBuilder.Append(cell.RichStringCellValue.String);
								break;
							case CellType.Numeric:
								stringBuilder.Append(_formatter.FormatCellValue(cell));
								break;
							case CellType.Boolean:
								stringBuilder.Append(cell.BooleanCellValue);
								break;
							case CellType.Error:
								stringBuilder.Append(ErrorEval.GetText(cell.ErrorCellValue));
								break;
							case CellType.Formula:
								if (formulasNotResults)
								{
									stringBuilder.Append(cell.CellFormula);
									break;
								}
								switch (cell.CachedFormulaResultType)
								{
								case CellType.String:
								{
									IRichTextString richStringCellValue = cell.RichStringCellValue;
									if (richStringCellValue != null && richStringCellValue.Length > 0)
									{
										stringBuilder.Append(richStringCellValue.ToString());
									}
									break;
								}
								case CellType.Numeric:
								{
									HSSFCellStyle hSSFCellStyle = (HSSFCellStyle)cell.CellStyle;
									if (hSSFCellStyle == null)
									{
										stringBuilder.Append(cell.NumericCellValue);
									}
									else
									{
										stringBuilder.Append(_formatter.FormatRawCellContents(cell.NumericCellValue, hSSFCellStyle.DataFormat, hSSFCellStyle.GetDataFormatString()));
									}
									break;
								}
								case CellType.Boolean:
									stringBuilder.Append(cell.BooleanCellValue);
									break;
								case CellType.Error:
									stringBuilder.Append(ErrorEval.GetText(cell.ErrorCellValue));
									break;
								}
								break;
							default:
								throw new Exception("Unexpected cell type (" + cell.CellType.ToString() + ")");
							}
							IComment cellComment = cell.CellComment;
							if (includeCellComments && cellComment != null)
							{
								string text = cellComment.String.String.Replace('\n', ' ');
								stringBuilder.Append(" Comment by " + cellComment.Author + ": " + text);
							}
						}
						if (flag && k < lastCellNum - 1)
						{
							stringBuilder.Append("\t");
						}
					}
					stringBuilder.Append("\n");
				}
				if (hSSFSheet.Footer != null && includeHeaderFooter)
				{
					stringBuilder.Append(ExtractHeaderFooter(hSSFSheet.Footer));
				}
			}
			return stringBuilder.ToString();
		}
	}

	public ExcelExtractor(HSSFWorkbook wb)
		: base(wb)
	{
		this.wb = wb;
		_formatter = new HSSFDataFormatter();
	}

	public ExcelExtractor(POIFSFileSystem fs)
		: this(new HSSFWorkbook(fs))
	{
	}

	public static string ExtractHeaderFooter(IHeaderFooter hf)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (hf.Left != null)
		{
			stringBuilder.Append(hf.Left);
		}
		if (hf.Center != null)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append("\t");
			}
			stringBuilder.Append(hf.Center);
		}
		if (hf.Right != null)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append("\t");
			}
			stringBuilder.Append(hf.Right);
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Append("\n");
		}
		return stringBuilder.ToString();
	}
}
