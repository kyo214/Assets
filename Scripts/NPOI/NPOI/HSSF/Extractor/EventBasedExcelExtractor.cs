using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using NPOI.HPSF;
using NPOI.HSSF.EventUserModel;
using NPOI.HSSF.Model;
using NPOI.HSSF.Record;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.HSSF.Extractor;

public class EventBasedExcelExtractor : POIOLE2TextExtractor
{
	private class TextListener : IHSSFListener
	{
		public FormatTrackingHSSFListener ft;

		private SSTRecord sstRecord;

		private IList sheetNames = new ArrayList();

		public StringBuilder text = new StringBuilder();

		private int sheetNum = -1;

		private int rowNum;

		private bool outputNextStringValue;

		private int nextRow = -1;

		private bool includeSheetNames;

		private bool formulasNotResults;

		public TextListener(bool includeSheetNames, bool formulasNotResults)
		{
			this.includeSheetNames = includeSheetNames;
			this.formulasNotResults = formulasNotResults;
		}

		public void ProcessRecord(NPOI.HSSF.Record.Record record)
		{
			string text = null;
			int num = -1;
			switch (record.Sid)
			{
			case 133:
			{
				BoundSheetRecord boundSheetRecord = (BoundSheetRecord)record;
				sheetNames.Add(boundSheetRecord.Sheetname);
				break;
			}
			case 2057:
				if (((BOFRecord)record).Type != BOFRecordType.Worksheet)
				{
					break;
				}
				sheetNum++;
				rowNum = -1;
				if (includeSheetNames)
				{
					if (this.text.Length > 0)
					{
						this.text.Append("\n");
					}
					this.text.Append(sheetNames[sheetNum]);
				}
				break;
			case 252:
				sstRecord = (SSTRecord)record;
				break;
			case 6:
			{
				FormulaRecord formulaRecord = (FormulaRecord)record;
				num = formulaRecord.Row;
				if (formulasNotResults)
				{
					text = HSSFFormulaParser.ToFormulaString(null, formulaRecord.ParsedExpression);
				}
				else if (formulaRecord.HasCachedResultString)
				{
					outputNextStringValue = true;
					nextRow = formulaRecord.Row;
				}
				else
				{
					text = FormatNumberDateCell(formulaRecord, formulaRecord.Value);
				}
				break;
			}
			case 519:
				if (outputNextStringValue)
				{
					text = ((StringRecord)record).String;
					num = nextRow;
					outputNextStringValue = false;
				}
				break;
			case 516:
			{
				LabelRecord obj = (LabelRecord)record;
				num = obj.Row;
				text = obj.Value;
				break;
			}
			case 253:
			{
				LabelSSTRecord labelSSTRecord = (LabelSSTRecord)record;
				num = labelSSTRecord.Row;
				if (sstRecord == null)
				{
					throw new Exception("No SST record found");
				}
				text = sstRecord.GetString(labelSSTRecord.SSTIndex).ToString();
				break;
			}
			case 28:
				num = ((NoteRecord)record).Row;
				break;
			case 515:
			{
				NumberRecord numberRecord = (NumberRecord)record;
				num = numberRecord.Row;
				text = FormatNumberDateCell(numberRecord, numberRecord.Value);
				break;
			}
			}
			if (text == null)
			{
				return;
			}
			if (num != rowNum)
			{
				rowNum = num;
				if (this.text.Length > 0)
				{
					this.text.Append("\n");
				}
			}
			else
			{
				this.text.Append("\t");
			}
			this.text.Append(text);
		}

		private string FormatNumberDateCell(CellValueRecordInterface cell, double value)
		{
			int formatIndex = ft.GetFormatIndex(cell);
			string formatString = ft.GetFormatString(cell);
			if (formatString == null)
			{
				return value.ToString(CultureInfo.InvariantCulture);
			}
			if (DateUtil.IsADateFormat(formatIndex, formatString) && DateUtil.IsValidExcelDate(value))
			{
				formatString = formatString.Replace('m', 'M');
				formatString = formatString.Replace("\\\\-", "-");
				DateTime javaDate = DateUtil.GetJavaDate(value, use1904windowing: false);
				return new SimpleDateFormat(formatString).Format(javaDate, CultureInfo.CurrentCulture);
			}
			if (formatString == "General")
			{
				return value.ToString(CultureInfo.InvariantCulture);
			}
			return new DecimalFormat(formatString).Format(value, CultureInfo.CurrentCulture);
		}
	}

	private POIFSFileSystem fs;

	private bool includeSheetNames = true;

	private bool formulasNotResults;

	public override DocumentSummaryInformation DocSummaryInformation
	{
		get
		{
			throw new NotImplementedException("Metadata extraction not supported in streaming mode, please use ExcelExtractor");
		}
	}

	public override SummaryInformation SummaryInformation
	{
		get
		{
			throw new NotImplementedException("Metadata extraction not supported in streaming mode, please use ExcelExtractor");
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

	public override string Text
	{
		get
		{
			string text = null;
			try
			{
				text = TriggerExtraction().text.ToString();
				if (!text.EndsWith("\n", StringComparison.Ordinal))
				{
					text += "\n";
				}
			}
			catch (IOException)
			{
				throw;
			}
			return text;
		}
	}

	public EventBasedExcelExtractor(POIFSFileSystem fs)
		: base((POIDocument)null)
	{
		this.fs = fs;
	}

	private TextListener TriggerExtraction()
	{
		TextListener textListener = new TextListener(includeSheetNames, formulasNotResults);
		FormatTrackingHSSFListener lsnr = (textListener.ft = new FormatTrackingHSSFListener(textListener));
		HSSFEventFactory hSSFEventFactory = new HSSFEventFactory();
		HSSFRequest hSSFRequest = new HSSFRequest();
		hSSFRequest.AddListenerForAllRecords(lsnr);
		hSSFEventFactory.ProcessWorkbookEvents(hSSFRequest, fs);
		return textListener;
	}
}
