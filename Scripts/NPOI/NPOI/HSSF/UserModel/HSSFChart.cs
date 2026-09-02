using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.HSSF.Record;
using NPOI.HSSF.Record.Chart;
using NPOI.SS.Formula.PTG;
using NPOI.SS.Util;

namespace NPOI.HSSF.UserModel;

public class HSSFChart
{
	public class HSSFSeries
	{
		internal SeriesRecord series;

		internal SeriesTextRecord seriesTitleText;

		private LinkedDataRecord dataName;

		private LinkedDataRecord dataValues;

		private LinkedDataRecord dataCategoryLabels;

		private LinkedDataRecord dataSecondaryCategoryLabels;

		public short NumValues => series.NumValues;

		public short ValueType => series.ValuesDataType;

		public string SeriesTitle
		{
			get
			{
				if (seriesTitleText != null)
				{
					return seriesTitleText.Text;
				}
				return null;
			}
			set
			{
				if (seriesTitleText != null)
				{
					seriesTitleText.Text = value;
					return;
				}
				throw new InvalidOperationException("No series title found to Change");
			}
		}

		public HSSFSeries(SeriesRecord series)
		{
			this.series = series;
		}

		internal void InsertData(LinkedDataRecord data)
		{
			switch (data.LinkType)
			{
			case 0:
				dataName = data;
				break;
			case 1:
				dataValues = data;
				break;
			case 2:
				dataCategoryLabels = data;
				break;
			case 3:
				dataSecondaryCategoryLabels = data;
				break;
			}
		}

		internal void SetSeriesTitleText(SeriesTextRecord seriesTitleText)
		{
			this.seriesTitleText = seriesTitleText;
		}

		public LinkedDataRecord GetDataName()
		{
			return dataName;
		}

		public LinkedDataRecord GetDataValues()
		{
			return dataValues;
		}

		public LinkedDataRecord GetDataCategoryLabels()
		{
			return dataCategoryLabels;
		}

		public LinkedDataRecord GetDataSecondaryCategoryLabels()
		{
			return dataSecondaryCategoryLabels;
		}

		public SeriesRecord GetSeries()
		{
			return series;
		}

		private CellRangeAddressBase GetCellRange(LinkedDataRecord linkedDataRecord)
		{
			if (linkedDataRecord == null)
			{
				return null;
			}
			int firstRow = 0;
			int lastRow = 0;
			int firstCol = 0;
			int lastCol = 0;
			Ptg[] formulaOfLink = linkedDataRecord.FormulaOfLink;
			foreach (Ptg ptg in formulaOfLink)
			{
				if (ptg is AreaPtgBase)
				{
					AreaPtgBase obj = (AreaPtgBase)ptg;
					firstRow = obj.FirstRow;
					lastRow = obj.LastRow;
					firstCol = obj.FirstColumn;
					lastCol = obj.LastColumn;
				}
			}
			return new CellRangeAddress(firstRow, lastRow, firstCol, lastCol);
		}

		public CellRangeAddressBase GetValuesCellRange()
		{
			return GetCellRange(dataValues);
		}

		public CellRangeAddressBase GetCategoryLabelsCellRange()
		{
			return GetCellRange(dataCategoryLabels);
		}

		private int SetVerticalCellRange(LinkedDataRecord linkedDataRecord, CellRangeAddressBase range)
		{
			if (linkedDataRecord == null)
			{
				throw new ArgumentNullException("linkedDataRecord should not be null");
			}
			List<Ptg> list = new List<Ptg>();
			int num = range.LastRow - range.FirstRow + 1;
			int num2 = range.LastColumn - range.FirstColumn + 1;
			Ptg[] formulaOfLink = linkedDataRecord.FormulaOfLink;
			foreach (Ptg ptg in formulaOfLink)
			{
				if (ptg is AreaPtgBase)
				{
					AreaPtgBase areaPtgBase = (AreaPtgBase)ptg;
					areaPtgBase.FirstRow = range.FirstRow;
					areaPtgBase.LastRow = range.LastRow;
					areaPtgBase.FirstColumn = range.FirstColumn;
					areaPtgBase.LastColumn = range.LastColumn;
					list.Add(areaPtgBase);
				}
			}
			linkedDataRecord.FormulaOfLink = list.ToArray();
			return num * num2;
		}

		public void SetValuesCellRange(CellRangeAddressBase range)
		{
			int num = SetVerticalCellRange(dataValues, range);
			series.NumValues = (short)num;
		}

		public void SetCategoryLabelsCellRange(CellRangeAddressBase range)
		{
			int num = SetVerticalCellRange(dataCategoryLabels, range);
			series.NumCategories = (short)num;
		}
	}

	private HSSFSheet sheet;

	private ChartRecord chartRecord;

	private LegendRecord legendRecord;

	private AlRunsRecord chartTitleFormat;

	private SeriesTextRecord chartTitleText;

	private List<ValueRangeRecord> valueRanges = new List<ValueRangeRecord>();

	private HSSFChartType type;

	private List<HSSFSeries> series = new List<HSSFSeries>();

	public int ChartX
	{
		get
		{
			return chartRecord.X;
		}
		set
		{
			chartRecord.X = value;
		}
	}

	public int ChartY
	{
		get
		{
			return chartRecord.Y;
		}
		set
		{
			chartRecord.Y = value;
		}
	}

	public int ChartWidth
	{
		get
		{
			return chartRecord.Width;
		}
		set
		{
			chartRecord.Width = value;
		}
	}

	public int ChartHeight
	{
		get
		{
			return chartRecord.Height;
		}
		set
		{
			chartRecord.Height = value;
		}
	}

	public HSSFSeries[] Series => series.ToArray();

	public string ChartTitle
	{
		get
		{
			if (chartTitleText != null)
			{
				return chartTitleText.Text;
			}
			return null;
		}
		set
		{
			if (chartTitleText != null)
			{
				chartTitleText.Text = value;
				return;
			}
			throw new InvalidOperationException("No chart title found to change");
		}
	}

	public HSSFChartType Type => type;

	private HSSFChart(HSSFSheet sheet, ChartRecord chartRecord)
	{
		this.chartRecord = chartRecord;
		this.sheet = sheet;
	}

	public void CreateBarChart(HSSFWorkbook workbook, HSSFSheet sheet)
	{
		List<RecordBase> list = new List<RecordBase>();
		list.Add(CreateMSDrawingObjectRecord());
		list.Add(CreateOBJRecord());
		list.Add(CreateBOFRecord());
		list.Add(new HeaderRecord(string.Empty));
		list.Add(new FooterRecord(string.Empty));
		list.Add(CreateHCenterRecord());
		list.Add(CreateVCenterRecord());
		list.Add(CreatePrintSetupRecord());
		list.Add(CreateFontBasisRecord1());
		list.Add(CreateFontBasisRecord2());
		list.Add(new ProtectRecord(isProtected: false));
		list.Add(CreateUnitsRecord());
		list.Add(CreateChartRecord(0, 0, 30434904, 19031616));
		list.Add(CreateBeginRecord());
		list.Add(CreateSCLRecord(1, 1));
		list.Add(CreatePlotGrowthRecord(65536, 65536));
		list.Add(CreateFrameRecord1());
		list.Add(CreateBeginRecord());
		list.Add(CreateLineFormatRecord(drawTicks: true));
		list.Add(CreateAreaFormatRecord1());
		list.Add(CreateEndRecord());
		list.Add(CreateSeriesRecord());
		list.Add(CreateBeginRecord());
		list.Add(CreateTitleLinkedDataRecord());
		list.Add(CreateValuesLinkedDataRecord());
		list.Add(CreateCategoriesLinkedDataRecord());
		list.Add(CreateDataFormatRecord());
		list.Add(new SerToCrtRecord());
		list.Add(CreateEndRecord());
		list.Add(CreateSheetPropsRecord());
		list.Add(CreateDefaultTextRecord(2));
		list.Add(CreateAllTextRecord());
		list.Add(CreateBeginRecord());
		list.Add(CreateFontIndexRecord(5));
		list.Add(CreateDirectLinkRecord());
		list.Add(CreateEndRecord());
		list.Add(CreateDefaultTextRecord(3));
		list.Add(CreateUnknownTextRecord());
		list.Add(CreateBeginRecord());
		list.Add(CreateFontIndexRecord(6));
		list.Add(CreateDirectLinkRecord());
		list.Add(CreateEndRecord());
		list.Add(CreateAxisUsedRecord(1));
		CreateAxisRecords(list);
		list.Add(CreateEndRecord());
		list.Add(CreateDimensionsRecord());
		list.Add(CreateSeriesIndexRecord(2));
		list.Add(CreateSeriesIndexRecord(1));
		list.Add(CreateSeriesIndexRecord(3));
		list.Add(EOFRecord.instance);
		sheet.InsertChartRecords(list);
		workbook.InsertChartRecord();
	}

	public static HSSFChart[] GetSheetCharts(HSSFSheet sheet)
	{
		List<HSSFChart> list = new List<HSSFChart>();
		HSSFChart hSSFChart = null;
		HSSFSeries hSSFSeries = null;
		foreach (RecordBase item in (IEnumerable)sheet.Sheet.Records)
		{
			if (item is ChartRecord)
			{
				hSSFSeries = null;
				hSSFChart = new HSSFChart(sheet, (ChartRecord)item);
				list.Add(hSSFChart);
			}
			else if (item is LegendRecord)
			{
				hSSFChart.legendRecord = (LegendRecord)item;
			}
			else if (item is SeriesRecord)
			{
				HSSFSeries hSSFSeries2 = new HSSFSeries((SeriesRecord)item);
				hSSFChart.series.Add(hSSFSeries2);
				hSSFSeries = hSSFSeries2;
			}
			else if (item is AlRunsRecord)
			{
				hSSFChart.chartTitleFormat = (AlRunsRecord)item;
			}
			else if (item is SeriesTextRecord)
			{
				SeriesTextRecord seriesTitleText = (SeriesTextRecord)item;
				if (hSSFChart.legendRecord == null && hSSFChart.series.Count > 0)
				{
					hSSFChart.series[hSSFChart.series.Count - 1].seriesTitleText = seriesTitleText;
				}
				else
				{
					hSSFChart.chartTitleText = seriesTitleText;
				}
			}
			else if (item is LinkedDataRecord)
			{
				LinkedDataRecord data = (LinkedDataRecord)item;
				hSSFSeries?.InsertData(data);
			}
			else if (item is ValueRangeRecord)
			{
				hSSFChart.valueRanges.Add((ValueRangeRecord)item);
			}
			else
			{
				if (!(item is NPOI.HSSF.Record.Record) || hSSFChart == null)
				{
					continue;
				}
				NPOI.HSSF.Record.Record record = (NPOI.HSSF.Record.Record)item;
				foreach (int value in Enum.GetValues(typeof(HSSFChartType)))
				{
					if (value != 0 && record.Sid == value)
					{
						hSSFChart.type = (HSSFChartType)value;
						break;
					}
				}
			}
		}
		return list.ToArray();
	}

	public void SetValueRange(int axisIndex, double? minimum, double? maximum, double? majorUnit, double? minorUnit)
	{
		ValueRangeRecord valueRangeRecord = valueRanges[axisIndex];
		if (valueRangeRecord != null)
		{
			if (minimum.HasValue)
			{
				valueRangeRecord.IsAutomaticMinimum = double.IsNaN(minimum.Value);
				valueRangeRecord.MinimumAxisValue = minimum.Value;
			}
			if (maximum.HasValue)
			{
				valueRangeRecord.IsAutomaticMaximum = double.IsNaN(maximum.Value);
				valueRangeRecord.MaximumAxisValue = maximum.Value;
			}
			if (majorUnit.HasValue)
			{
				valueRangeRecord.IsAutomaticMajor = double.IsNaN(majorUnit.Value);
				valueRangeRecord.MajorIncrement = majorUnit.Value;
			}
			if (minorUnit.HasValue)
			{
				valueRangeRecord.IsAutomaticMinor = double.IsNaN(minorUnit.Value);
				valueRangeRecord.MinorIncrement = minorUnit.Value;
			}
		}
	}

	private SeriesIndexRecord CreateSeriesIndexRecord(int index)
	{
		return new SeriesIndexRecord
		{
			Index = (short)index
		};
	}

	private DimensionsRecord CreateDimensionsRecord()
	{
		return new DimensionsRecord
		{
			FirstRow = 0,
			LastRow = 31,
			FirstCol = 0,
			LastCol = 1
		};
	}

	private HCenterRecord CreateHCenterRecord()
	{
		return new HCenterRecord
		{
			HCenter = false
		};
	}

	private VCenterRecord CreateVCenterRecord()
	{
		return new VCenterRecord
		{
			VCenter = false
		};
	}

	private PrintSetupRecord CreatePrintSetupRecord()
	{
		return new PrintSetupRecord
		{
			PaperSize = 0,
			Scale = 18,
			PageStart = 1,
			FitWidth = 1,
			FitHeight = 1,
			LeftToRight = false,
			Landscape = false,
			ValidSettings = true,
			NoColor = false,
			Draft = false,
			Notes = false,
			NoOrientation = false,
			UsePage = false,
			HResolution = 0,
			VResolution = 0,
			HeaderMargin = 0.5,
			FooterMargin = 0.5,
			Copies = 15
		};
	}

	private FbiRecord CreateFontBasisRecord1()
	{
		return new FbiRecord
		{
			XBasis = 9120,
			YBasis = 5640,
			HeightBasis = 200,
			Scale = 0,
			IndexToFontTable = 5
		};
	}

	private FbiRecord CreateFontBasisRecord2()
	{
		FbiRecord fbiRecord = CreateFontBasisRecord1();
		fbiRecord.IndexToFontTable = 6;
		return fbiRecord;
	}

	private BOFRecord CreateBOFRecord()
	{
		return new BOFRecord
		{
			Version = 600,
			Type = BOFRecordType.Chart,
			Build = 7422,
			BuildYear = 1997,
			HistoryBitMask = 16585,
			RequiredVersion = 106
		};
	}

	private UnknownRecord CreateOBJRecord()
	{
		byte[] data = new byte[26]
		{
			21, 0, 18, 0, 5, 0, 2, 0, 17, 96,
			0, 0, 0, 0, 184, 3, 135, 3, 0, 0,
			0, 0, 0, 0, 0, 0
		};
		return new UnknownRecord(93, data);
	}

	private UnknownRecord CreateMSDrawingObjectRecord()
	{
		byte[] data = new byte[200]
		{
			15, 0, 2, 240, 192, 0, 0, 0, 16, 0,
			8, 240, 8, 0, 0, 0, 2, 0, 0, 0,
			2, 4, 0, 0, 15, 0, 3, 240, 168, 0,
			0, 0, 15, 0, 4, 240, 40, 0, 0, 0,
			1, 0, 9, 240, 16, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 2, 0, 10, 240, 8, 0,
			0, 0, 0, 4, 0, 0, 5, 0, 0, 0,
			15, 0, 4, 240, 112, 0, 0, 0, 146, 12,
			10, 240, 8, 0, 0, 0, 2, 4, 0, 0,
			0, 10, 0, 0, 147, 0, 11, 240, 54, 0,
			0, 0, 127, 0, 4, 1, 4, 1, 191, 0,
			8, 0, 8, 0, 129, 1, 78, 0, 0, 8,
			131, 1, 77, 0, 0, 8, 191, 1, 16, 0,
			17, 0, 192, 1, 77, 0, 0, 8, 255, 1,
			8, 0, 8, 0, 63, 2, 0, 0, 2, 0,
			191, 3, 0, 0, 8, 0, 0, 0, 16, 240,
			18, 0, 0, 0, 0, 0, 4, 0, 192, 2,
			10, 0, 244, 0, 14, 0, 102, 1, 32, 0,
			233, 0, 0, 0, 17, 240, 0, 0, 0, 0
		};
		return new UnknownRecord(236, data);
	}

	private void CreateAxisRecords(IList records)
	{
		records.Add(CreateAxisParentRecord());
		records.Add(CreateBeginRecord());
		records.Add(CreateAxisRecord(0));
		records.Add(CreateBeginRecord());
		records.Add(CreateCategorySeriesAxisRecord());
		records.Add(CreateAxisOptionsRecord());
		records.Add(CreateTickRecord1());
		records.Add(CreateEndRecord());
		records.Add(CreateAxisRecord(1));
		records.Add(CreateBeginRecord());
		records.Add(CreateValueRangeRecord());
		records.Add(CreateTickRecord2());
		records.Add(CreateAxisLineFormatRecord(AxisLineFormatRecord.AXIS_TYPE_MAJOR_GRID_LINE));
		records.Add(CreateLineFormatRecord(drawTicks: false));
		records.Add(CreateEndRecord());
		records.Add(CreatePlotAreaRecord());
		records.Add(CreateFrameRecord2());
		records.Add(CreateBeginRecord());
		records.Add(CreateLineFormatRecord2());
		records.Add(CreateAreaFormatRecord2());
		records.Add(CreateEndRecord());
		records.Add(CreateChartFormatRecord());
		records.Add(CreateBeginRecord());
		records.Add(CreateBarRecord());
		records.Add(CreateLegendRecord());
		records.Add(CreateBeginRecord());
		records.Add(CreateTextRecord());
		records.Add(CreateBeginRecord());
		records.Add(CreateLinkedDataRecord());
		records.Add(CreateEndRecord());
		records.Add(CreateEndRecord());
		records.Add(CreateEndRecord());
		records.Add(CreateEndRecord());
	}

	private LinkedDataRecord CreateLinkedDataRecord()
	{
		return new LinkedDataRecord
		{
			LinkType = LinkedDataRecord.LINK_TYPE_TITLE_OR_TEXT,
			ReferenceType = LinkedDataRecord.REFERENCE_TYPE_DIRECT,
			IsCustomNumberFormat = false,
			IndexNumberFmtRecord = 0,
			FormulaOfLink = null
		};
	}

	private TextRecord CreateTextRecord()
	{
		return new TextRecord
		{
			HorizontalAlignment = 2,
			VerticalAlignment = 2,
			DisplayMode = 1,
			RgbColor = 0,
			X = -37,
			Y = -60,
			Width = 0,
			Height = 0,
			IsAutoColor = true,
			ShowKey = false,
			ShowValue = false,
			IsAutoGeneratedText = true,
			IsGenerated = true,
			IsAutoLabelDeleted = false,
			IsAutoBackground = true,
			ShowCategoryLabelAsPercentage = false,
			ShowValueAsPercentage = false,
			ShowBubbleSizes = false,
			ShowLabel = false,
			IndexOfColorValue = 77,
			DataLabelPlacement = 0,
			TextRotation = 0
		};
	}

	private LegendRecord CreateLegendRecord()
	{
		return new LegendRecord
		{
			XAxisUpperLeft = 3542,
			YAxisUpperLeft = 1566,
			XSize = 437,
			YSize = 213,
			Type = 3,
			Spacing = 1,
			IsAutoPosition = true,
			IsAutoSeries = true,
			IsAutoXPositioning = true,
			IsAutoYPositioning = true,
			IsVertical = true,
			IsDataTable = false
		};
	}

	private BarRecord CreateBarRecord()
	{
		return new BarRecord
		{
			BarSpace = 0,
			CategorySpace = 150,
			IsHorizontal = false,
			IsStacked = false,
			IsDisplayAsPercentage = false,
			IsShadow = false
		};
	}

	private ChartFormatRecord CreateChartFormatRecord()
	{
		return new ChartFormatRecord
		{
			XPosition = 0,
			YPosition = 0,
			Width = 0,
			Height = 0,
			VaryDisplayPattern = false
		};
	}

	private PlotAreaRecord CreatePlotAreaRecord()
	{
		return new PlotAreaRecord();
	}

	private AxisLineFormatRecord CreateAxisLineFormatRecord(short format)
	{
		return new AxisLineFormatRecord
		{
			AxisType = format
		};
	}

	private ValueRangeRecord CreateValueRangeRecord()
	{
		return new ValueRangeRecord
		{
			MinimumAxisValue = 0.0,
			MaximumAxisValue = 0.0,
			MajorIncrement = 0.0,
			MinorIncrement = 0.0,
			CategoryAxisCross = 0.0,
			IsAutomaticMinimum = true,
			IsAutomaticMaximum = true,
			IsAutomaticMajor = true,
			IsAutomaticMinor = true,
			IsAutomaticCategoryCrossing = true,
			IsLogarithmicScale = false,
			IsValuesInReverse = false,
			IsCrossCategoryAxisAtMaximum = false,
			IsReserved = true
		};
	}

	private TickRecord CreateTickRecord1()
	{
		TickRecord tickRecord = new TickRecord();
		tickRecord.MajorTickType = 2;
		tickRecord.MinorTickType = 0;
		tickRecord.LabelPosition = 3;
		tickRecord.Background = 1;
		tickRecord.LabelColorRgb = 0;
		tickRecord.Zero1 = 0;
		tickRecord.Zero2 = 0;
		tickRecord.Zero3 = 45;
		tickRecord.IsAutorotate = true;
		tickRecord.IsAutoTextBackground = true;
		tickRecord.Rotation = 0;
		tickRecord.IsAutorotate = true;
		tickRecord.TickColor = 77;
		return tickRecord;
	}

	private TickRecord CreateTickRecord2()
	{
		TickRecord tickRecord = CreateTickRecord1();
		tickRecord.Zero3 = 0;
		return tickRecord;
	}

	private AxcExtRecord CreateAxisOptionsRecord()
	{
		return new AxcExtRecord
		{
			MinimumDate = -28644,
			MaximumDate = -28715,
			MajorInterval = 2,
			MajorUnit = DateUnit.Days,
			MinorInterval = 1,
			MinorUnit = DateUnit.Days,
			BaseUnit = DateUnit.Days,
			CrossDate = -28644,
			IsAutoMin = true,
			IsAutoMax = true,
			IsAutoMajor = true,
			IsAutoMinor = true,
			IsDateAxis = true,
			IsAutoBase = true,
			IsAutoCross = true,
			IsAutoDate = true
		};
	}

	private CatSerRangeRecord CreateCategorySeriesAxisRecord()
	{
		return new CatSerRangeRecord
		{
			CrossPoint = 1,
			LabelInterval = 1,
			MarkInterval = 1,
			IsBetween = true,
			IsMaxCross = false,
			IsReverse = false
		};
	}

	private AxisRecord CreateAxisRecord(short axisType)
	{
		return new AxisRecord
		{
			AxisType = axisType
		};
	}

	private AxisParentRecord CreateAxisParentRecord()
	{
		return new AxisParentRecord
		{
			AxisType = 0,
			X = 479,
			Y = 221,
			Width = 2995,
			Height = 2902
		};
	}

	private AxesUsedRecord CreateAxisUsedRecord(short numAxis)
	{
		return new AxesUsedRecord
		{
			NumAxis = numAxis
		};
	}

	private LinkedDataRecord CreateDirectLinkRecord()
	{
		return new LinkedDataRecord
		{
			LinkType = LinkedDataRecord.LINK_TYPE_TITLE_OR_TEXT,
			ReferenceType = LinkedDataRecord.REFERENCE_TYPE_DIRECT,
			IsCustomNumberFormat = false,
			IndexNumberFmtRecord = 0,
			FormulaOfLink = null
		};
	}

	private FontIndexRecord CreateFontIndexRecord(int index)
	{
		return new FontIndexRecord
		{
			FontIndex = (short)index
		};
	}

	private TextRecord CreateAllTextRecord()
	{
		return new TextRecord
		{
			HorizontalAlignment = 2,
			VerticalAlignment = 2,
			DisplayMode = 1,
			RgbColor = 0,
			X = -37,
			Y = -60,
			Width = 0,
			Height = 0,
			IsAutoColor = true,
			ShowKey = false,
			ShowValue = true,
			IsAutoGeneratedText = true,
			IsGenerated = true,
			IsAutoLabelDeleted = false,
			IsAutoBackground = true,
			ShowCategoryLabelAsPercentage = false,
			ShowValueAsPercentage = false,
			ShowBubbleSizes = false,
			ShowLabel = false,
			IndexOfColorValue = 77,
			DataLabelPlacement = 0,
			TextRotation = 0
		};
	}

	private TextRecord CreateUnknownTextRecord()
	{
		return new TextRecord
		{
			HorizontalAlignment = 2,
			VerticalAlignment = 2,
			DisplayMode = 1,
			RgbColor = 0,
			X = -37,
			Y = -60,
			Width = 0,
			Height = 0,
			IsAutoColor = true,
			ShowKey = false,
			ShowValue = false,
			IsAutoGeneratedText = true,
			IsGenerated = true,
			IsAutoLabelDeleted = false,
			IsAutoBackground = true,
			ShowCategoryLabelAsPercentage = false,
			ShowValueAsPercentage = false,
			ShowBubbleSizes = false,
			ShowLabel = false,
			IndexOfColorValue = 77,
			DataLabelPlacement = 11088,
			TextRotation = 0
		};
	}

	private DefaultTextRecord CreateDefaultTextRecord(short categoryDataType)
	{
		return new DefaultTextRecord
		{
			FormatType = (TextFormatInfo)categoryDataType
		};
	}

	private ShtPropsRecord CreateSheetPropsRecord()
	{
		return new ShtPropsRecord
		{
			IsManSerAlloc = false,
			IsPlotVisibleOnly = true,
			IsNotSizeWithWindow = false,
			IsManPlotArea = true,
			IsAlwaysAutoPlotArea = false
		};
	}

	private DataFormatRecord CreateDataFormatRecord()
	{
		return new DataFormatRecord
		{
			PointNumber = -1,
			SeriesIndex = 0,
			SeriesNumber = 0,
			UseExcel4Colors = false
		};
	}

	private LinkedDataRecord CreateCategoriesLinkedDataRecord()
	{
		LinkedDataRecord linkedDataRecord = new LinkedDataRecord();
		linkedDataRecord.LinkType = LinkedDataRecord.LINK_TYPE_CATEGORIES;
		linkedDataRecord.ReferenceType = LinkedDataRecord.REFERENCE_TYPE_WORKSHEET;
		linkedDataRecord.IsCustomNumberFormat = false;
		linkedDataRecord.IndexNumberFmtRecord = 0;
		Area3DPtg area3DPtg = new Area3DPtg(0, 31, 1, 1, firstRowRelative: false, lastRowRelative: false, firstColRelative: false, lastColRelative: false, 0);
		linkedDataRecord.FormulaOfLink = new Ptg[1] { area3DPtg };
		return linkedDataRecord;
	}

	private LinkedDataRecord CreateValuesLinkedDataRecord()
	{
		LinkedDataRecord linkedDataRecord = new LinkedDataRecord();
		linkedDataRecord.LinkType = LinkedDataRecord.LINK_TYPE_VALUES;
		linkedDataRecord.ReferenceType = LinkedDataRecord.REFERENCE_TYPE_WORKSHEET;
		linkedDataRecord.IsCustomNumberFormat = false;
		linkedDataRecord.IndexNumberFmtRecord = 0;
		Area3DPtg area3DPtg = new Area3DPtg(0, 31, 0, 0, firstRowRelative: false, lastRowRelative: false, firstColRelative: false, lastColRelative: false, 0);
		linkedDataRecord.FormulaOfLink = new Ptg[1] { area3DPtg };
		return linkedDataRecord;
	}

	private LinkedDataRecord CreateTitleLinkedDataRecord()
	{
		return new LinkedDataRecord
		{
			LinkType = LinkedDataRecord.LINK_TYPE_TITLE_OR_TEXT,
			ReferenceType = LinkedDataRecord.REFERENCE_TYPE_DIRECT,
			IsCustomNumberFormat = false,
			IndexNumberFmtRecord = 0,
			FormulaOfLink = null
		};
	}

	private SeriesRecord CreateSeriesRecord()
	{
		return new SeriesRecord
		{
			CategoryDataType = 1,
			ValuesDataType = 1,
			NumCategories = 32,
			NumValues = 31,
			BubbleSeriesType = 1,
			NumBubbleValues = 0
		};
	}

	private EndRecord CreateEndRecord()
	{
		return new EndRecord();
	}

	private AreaFormatRecord CreateAreaFormatRecord1()
	{
		return new AreaFormatRecord
		{
			ForegroundColor = 16777215,
			BackgroundColor = 0,
			Pattern = 1,
			IsAutomatic = true,
			IsInvert = false,
			ForecolorIndex = 78,
			BackcolorIndex = 77
		};
	}

	private AreaFormatRecord CreateAreaFormatRecord2()
	{
		return new AreaFormatRecord
		{
			ForegroundColor = 12632256,
			BackgroundColor = 0,
			Pattern = 1,
			IsAutomatic = false,
			IsInvert = false,
			ForecolorIndex = 22,
			BackcolorIndex = 79
		};
	}

	private LineFormatRecord CreateLineFormatRecord(bool drawTicks)
	{
		return new LineFormatRecord
		{
			LineColor = 0,
			LinePattern = 0,
			Weight = -1,
			IsAuto = true,
			IsDrawTicks = drawTicks,
			ColourPaletteIndex = 77
		};
	}

	private LineFormatRecord CreateLineFormatRecord2()
	{
		return new LineFormatRecord
		{
			LineColor = 8421504,
			LinePattern = 0,
			Weight = 0,
			IsAuto = false,
			IsDrawTicks = false,
			IsUnknown = false,
			ColourPaletteIndex = 23
		};
	}

	private FrameRecord CreateFrameRecord1()
	{
		return new FrameRecord
		{
			BorderType = 0,
			IsAutoSize = false,
			IsAutoPosition = true
		};
	}

	private FrameRecord CreateFrameRecord2()
	{
		return new FrameRecord
		{
			BorderType = 0,
			IsAutoSize = true,
			IsAutoPosition = true
		};
	}

	private PlotGrowthRecord CreatePlotGrowthRecord(int horizScale, int vertScale)
	{
		return new PlotGrowthRecord
		{
			HorizontalScale = horizScale,
			VerticalScale = vertScale
		};
	}

	private SCLRecord CreateSCLRecord(short numerator, short denominator)
	{
		return new SCLRecord
		{
			Denominator = denominator,
			Numerator = numerator
		};
	}

	private BeginRecord CreateBeginRecord()
	{
		return new BeginRecord();
	}

	private ChartRecord CreateChartRecord(int x, int y, int width, int height)
	{
		return new ChartRecord
		{
			X = x,
			Y = y,
			Width = width,
			Height = height
		};
	}

	private UnitsRecord CreateUnitsRecord()
	{
		return new UnitsRecord
		{
			Units = 0
		};
	}

	public HSSFSeries CreateSeries()
	{
		List<RecordBase> list = new List<RecordBase>();
		bool flag = false;
		int num = 0;
		int num2 = 0;
		int num3 = -1;
		int num4 = -1;
		int num5 = -1;
		int num6 = -1;
		int num7 = 0;
		IList records = sheet.Sheet.Records;
		foreach (RecordBase item in records)
		{
			num++;
			if (item is BeginRecord)
			{
				num2++;
			}
			else if (item is EndRecord)
			{
				num2--;
				if (num5 == num2)
				{
					num5 = -1;
					num6 = num;
					if (!flag)
					{
						list.Add(item);
						flag = true;
					}
				}
				if (num4 == num2)
				{
					break;
				}
			}
			if (item is ChartRecord)
			{
				if (item == chartRecord)
				{
					num3 = num;
					num4 = num2;
				}
			}
			else if (item is SeriesRecord && num3 != -1)
			{
				num7++;
				num5 = num2;
			}
			if (num5 != -1 && !flag)
			{
				list.Add(item);
			}
		}
		if (num6 == -1)
		{
			return null;
		}
		num = num6 + 1;
		HSSFSeries hSSFSeries = null;
		List<RecordBase> list2 = new List<RecordBase>();
		foreach (RecordBase item2 in list)
		{
			NPOI.HSSF.Record.Record record = null;
			if (item2 is BeginRecord)
			{
				record = new BeginRecord();
			}
			else if (item2 is EndRecord)
			{
				record = new EndRecord();
			}
			else if (item2 is SeriesRecord)
			{
				SeriesRecord obj = (SeriesRecord)((SeriesRecord)item2).Clone();
				hSSFSeries = new HSSFSeries(obj);
				record = obj;
			}
			else if (item2 is LinkedDataRecord)
			{
				LinkedDataRecord linkedDataRecord = (LinkedDataRecord)((LinkedDataRecord)item2).Clone();
				hSSFSeries?.InsertData(linkedDataRecord);
				record = linkedDataRecord;
			}
			else if (item2 is DataFormatRecord)
			{
				DataFormatRecord obj2 = (DataFormatRecord)((DataFormatRecord)item2).Clone();
				obj2.SeriesIndex = (short)num7;
				obj2.SeriesNumber = (short)num7;
				record = obj2;
			}
			else if (item2 is SeriesTextRecord)
			{
				SeriesTextRecord seriesTextRecord = (SeriesTextRecord)((SeriesTextRecord)item2).Clone();
				hSSFSeries?.SetSeriesTitleText(seriesTextRecord);
				record = seriesTextRecord;
			}
			else if (item2 is NPOI.HSSF.Record.Record)
			{
				record = (NPOI.HSSF.Record.Record)((NPOI.HSSF.Record.Record)item2).Clone();
			}
			if (record != null)
			{
				list2.Add(record);
			}
		}
		if (hSSFSeries == null)
		{
			return null;
		}
		foreach (RecordBase item3 in list2)
		{
			records.Insert(num++, item3);
		}
		return hSSFSeries;
	}

	public bool RemoveSeries(HSSFSeries series)
	{
		int num = 0;
		int num2 = 0;
		int num3 = -1;
		int num4 = -1;
		int num5 = -1;
		bool flag = false;
		bool flag2 = false;
		bool result = false;
		IList records = sheet.Sheet.Records;
		IEnumerator enumerator = records.GetEnumerator();
		while (enumerator.MoveNext())
		{
			RecordBase recordBase = (RecordBase)enumerator.Current;
			num++;
			if (recordBase is BeginRecord)
			{
				num2++;
			}
			else if (recordBase is EndRecord)
			{
				num2--;
				if (num4 == num2)
				{
					num4 = -1;
					if (flag)
					{
						flag = false;
						result = true;
						records.Remove(recordBase);
					}
				}
				if (num3 == num2)
				{
					break;
				}
			}
			if (recordBase is ChartRecord)
			{
				if (recordBase == chartRecord)
				{
					num3 = num2;
					flag2 = true;
				}
			}
			else if (recordBase is SeriesRecord)
			{
				if (flag2)
				{
					if (series.series == recordBase)
					{
						num4 = num2;
						flag = true;
					}
					else
					{
						num5++;
					}
				}
			}
			else if (recordBase is DataFormatRecord && flag2 && !flag)
			{
				DataFormatRecord obj = (DataFormatRecord)recordBase;
				obj.SeriesIndex = (short)num5;
				obj.SeriesNumber = (short)num5;
			}
			if (flag)
			{
				records.Remove(recordBase);
			}
		}
		return result;
	}
}
