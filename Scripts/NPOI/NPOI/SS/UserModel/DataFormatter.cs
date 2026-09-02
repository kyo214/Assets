using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.SS.Format;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.SS.UserModel;

public class DataFormatter
{
	private class CellFormatResultWrapper : FormatBase
	{
		private CellFormatResult result;

		private bool emulateCSV;

		internal CellFormatResultWrapper(CellFormatResult result, bool emulateCSV)
		{
			this.emulateCSV = emulateCSV;
			this.result = result;
		}

		protected override StringBuilder Format(object obj, StringBuilder toAppendTo, int pos)
		{
			if (emulateCSV)
			{
				return toAppendTo.Append(result.Text);
			}
			return toAppendTo.Append(result.Text.Trim());
		}

		public override StringBuilder Format(object obj, StringBuilder toAppendTo, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public override object ParseObject(string source, int pos)
		{
			return null;
		}
	}

	private static string defaultFractionWholePartFormat;

	private static string defaultFractionFractionPartFormat;

	private static string numPattern;

	private static string amPmPattern;

	private static string localePatternGroup;

	private static Regex colorPattern;

	private static Regex fractionPattern;

	private static Regex fractionStripper;

	private static Regex alternateGrouping;

	private static string invalidDateTimeString;

	private NumberFormatInfo decimalSymbols;

	private DateTimeFormatInfo dateSymbols;

	private DateFormat defaultDateformat;

	private FormatBase generalNumberFormat;

	private FormatBase defaultNumFormat;

	private CultureInfo currentCulture;

	private Hashtable formats;

	private bool emulateCSV;

	private static POILogger logger;

	private bool localeIsAdapting;

	private static readonly Regex RegexDoubleBackslashAny;

	private static readonly Regex RegexContinueWs;

	private static readonly Regex RegexAnyInDoubleQuote;

	static DataFormatter()
	{
		defaultFractionWholePartFormat = "#";
		defaultFractionFractionPartFormat = "#/##";
		numPattern = "[0#]+";
		amPmPattern = "((A|P)[M/P]*)";
		localePatternGroup = "(\\[\\$[^-\\]]*-[0-9A-Z]+\\])";
		colorPattern = new Regex("(\\[BLACK\\])|(\\[BLUE\\])|(\\[CYAN\\])|(\\[GREEN\\])|(\\[MAGENTA\\])|(\\[RED\\])|(\\[WHITE\\])|(\\[YELLOW\\])|(\\[COLOR\\s*\\d\\])|(\\[COLOR\\s*[0-5]\\d\\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		fractionPattern = new Regex("(?:([#\\d]+)\\s+)?(#+)\\s*\\/\\s*([#\\d]+)", RegexOptions.Compiled);
		fractionStripper = new Regex("(\"[^\"]*\")|([^ \\?#\\d\\/]+)", RegexOptions.Compiled);
		alternateGrouping = new Regex("([#0]([^.#0])[#0]{3})", RegexOptions.Compiled);
		logger = POILogFactory.GetLogger(typeof(DataFormatter));
		RegexDoubleBackslashAny = new Regex("\\\\.", RegexOptions.Compiled);
		RegexContinueWs = new Regex("\\s", RegexOptions.Compiled);
		RegexAnyInDoubleQuote = new Regex("\"[^\"]*\"", RegexOptions.Compiled);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 255; i++)
		{
			stringBuilder.Append('#');
		}
		invalidDateTimeString = stringBuilder.ToString();
	}

	public DataFormatter()
		: this(emulateCSV: false)
	{
	}

	public DataFormatter(bool emulateCSV)
		: this(CultureInfo.CurrentCulture, localeIsAdapting: true, emulateCSV)
	{
	}

	public DataFormatter(CultureInfo locale)
		: this(locale, emulateCSV: false)
	{
	}

	public DataFormatter(CultureInfo locale, bool emulateCSV)
		: this(locale, localeIsAdapting: false, emulateCSV)
	{
	}

	public DataFormatter(CultureInfo culture, bool localeIsAdapting, bool emulateCSV)
	{
		this.localeIsAdapting = true;
		currentCulture = culture;
		this.localeIsAdapting = localeIsAdapting;
		this.emulateCSV = emulateCSV;
		dateSymbols = culture.DateTimeFormat;
		decimalSymbols = culture.NumberFormat;
		generalNumberFormat = new ExcelGeneralNumberFormat(culture);
		defaultDateformat = new SimpleDateFormat(dateSymbols.FullDateTimePattern, dateSymbols);
		defaultDateformat.TimeZone = TimeZoneInfo.Local;
		formats = new Hashtable();
		FormatBase instance = ZipPlusFourFormat.Instance;
		AddFormat("00000\\-0000", instance);
		AddFormat("00000-0000", instance);
		FormatBase instance2 = PhoneFormat.Instance;
		AddFormat("[<=9999999]###\\-####;\\(###\\)\\ ###\\-####", instance2);
		AddFormat("[<=9999999]###-####;(###) ###-####", instance2);
		AddFormat("###\\-####;\\(###\\)\\ ###\\-####", instance2);
		AddFormat("###-####;(###) ###-####", instance2);
		FormatBase instance3 = SSNFormat.Instance;
		AddFormat("000\\-00\\-0000", instance3);
		AddFormat("000-00-0000", instance3);
	}

	private FormatBase GetFormat(ICell cell)
	{
		if (cell.CellStyle == null)
		{
			return null;
		}
		int dataFormat = cell.CellStyle.DataFormat;
		string dataFormatString = cell.CellStyle.GetDataFormatString();
		if (dataFormatString == null || dataFormatString.Trim().Length == 0)
		{
			return null;
		}
		return GetFormat(cell.NumericCellValue, dataFormat, dataFormatString);
	}

	private FormatBase GetFormat(double cellValue, int formatIndex, string formatStrIn)
	{
		string text = formatStrIn;
		if (text.IndexOf(';') != -1 && text.IndexOf(';') != text.LastIndexOf(';'))
		{
			try
			{
				CellFormat instance = CellFormat.GetInstance(text);
				object obj = cellValue;
				if (DateUtil.IsADateFormat(formatIndex, text) && (double)obj != 0.0)
				{
					obj = DateUtil.GetJavaDate(cellValue);
				}
				return new CellFormatResultWrapper(instance.Apply(obj), emulateCSV);
			}
			catch (Exception exception)
			{
				logger.Log(5, "Formatting failed for format " + text + ", falling back", exception);
			}
		}
		int num = text.IndexOf(';');
		int num2 = text.LastIndexOf(';');
		if (num != -1 && num != num2)
		{
			int num3 = text.IndexOf(';', num + 1);
			text = ((num3 == num2) ? ((cellValue != 0.0) ? text.Substring(0, num2) : text.Substring(num2 + 1)) : ((cellValue != 0.0) ? text.Substring(0, num3) : text.Substring(num3 + 1, num2 - (num3 + 1))));
		}
		if (emulateCSV && cellValue == 0.0 && text.Contains("#") && !text.Contains("0"))
		{
			text = text.Replace("#", "");
		}
		FormatBase formatBase = (FormatBase)formats[text];
		if (formatBase != null)
		{
			return formatBase;
		}
		if (text.Equals("General", StringComparison.CurrentCultureIgnoreCase) || "@".Equals(text))
		{
			return generalNumberFormat;
		}
		formatBase = CreateFormat(cellValue, formatIndex, text);
		formats[text] = formatBase;
		return formatBase;
	}

	public FormatBase CreateFormat(ICell cell)
	{
		int dataFormat = cell.CellStyle.DataFormat;
		string dataFormatString = cell.CellStyle.GetDataFormatString();
		return CreateFormat(cell.NumericCellValue, dataFormat, dataFormatString);
	}

	private FormatBase CreateFormat(double cellValue, int formatIndex, string sFormat)
	{
		string text = colorPattern.Replace(sFormat, "");
		foreach (Match item in Regex.Matches(text, localePatternGroup))
		{
			string value = item.Value;
			int num = value.IndexOf('$') + 1;
			int num2 = value.IndexOf('-');
			string text2 = value.Substring(num, num2 - num);
			if (text2.IndexOf('$') > -1)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(text2.Substring(0, text2.IndexOf('$')));
				stringBuilder.Append('\\');
				stringBuilder.Append(text2.Substring(text2.IndexOf('$'), text2.Length));
				text2 = stringBuilder.ToString();
			}
			value = Regex.Replace(value, localePatternGroup, text2);
			text = text.Remove(item.Index, item.Length);
			text = text.Insert(item.Index, value);
		}
		if (text == null || text.Trim().Length == 0)
		{
			return GetDefaultFormat(cellValue);
		}
		if ("General".Equals(text, StringComparison.CurrentCultureIgnoreCase) || "@".Equals(text))
		{
			return generalNumberFormat;
		}
		if (DateUtil.IsADateFormat(formatIndex, text) && DateUtil.IsValidExcelDate(cellValue))
		{
			return CreateDateFormat(text, cellValue);
		}
		if (text.IndexOf("#/") >= 0 || text.IndexOf("?/") >= 0)
		{
			string[] array = text.Split(";".ToCharArray());
			for (int i = 0; i < array.Length; i++)
			{
				string input = array[i].Replace("?", "#");
				input = fractionStripper.Replace(input, " ");
				input = input.Replace(" +", " ");
				Match match2 = fractionPattern.Match(input);
				if (match2.Success)
				{
					return new FractionFormat((match2.Groups[1] == null || !match2.Groups[1].Success) ? "" : defaultFractionWholePartFormat, match2.Groups[3].Value);
				}
			}
			return new FractionFormat(defaultFractionWholePartFormat, defaultFractionFractionPartFormat);
		}
		if (Regex.IsMatch(text, numPattern))
		{
			return CreateNumberFormat(text, cellValue);
		}
		if (emulateCSV)
		{
			return new ConstantStringFormat(cleanFormatForNumber(text));
		}
		return null;
	}

	private int IndexOfFraction(string format)
	{
		int num = format.IndexOf("#/#");
		int num2 = format.IndexOf("?/?");
		if (num != -1)
		{
			if (num2 != -1)
			{
				return Math.Min(num, num2);
			}
			return num;
		}
		return num2;
	}

	private int LastIndexOfFraction(string format)
	{
		int num = format.LastIndexOf("#/#");
		int num2 = format.LastIndexOf("?/?");
		if (num != -1)
		{
			if (num2 != -1)
			{
				return Math.Max(num, num2);
			}
			return num;
		}
		return num2;
	}

	private FormatBase CreateDateFormat(string pformatStr, double cellValue)
	{
		string text = pformatStr;
		text = text.Replace("\\-", "-");
		text = text.Replace("\\,", ",");
		text = text.Replace("\\.", ".");
		text = text.Replace("\\ ", " ");
		text = text.Replace("\\/", "/");
		text = text.Replace(";@", "");
		text = text.Replace("\"/\"", "/");
		text = text.Replace("\"\"", "'");
		text = text.Replace("\\\\T", "'T'");
		bool flag = Regex.IsMatch(text, amPmPattern);
		if (flag)
		{
			text = Regex.Replace(text, amPmPattern, "@");
		}
		text = text.Replace("@", "tt");
		StringBuilder stringBuilder = new StringBuilder();
		char[] array = text.ToCharArray();
		bool flag2 = true;
		bool flag3 = false;
		List<int> list = new List<int>();
		for (int i = 0; i < array.Length; i++)
		{
			char c = array[i];
			switch (c)
			{
			case '\'':
				stringBuilder.Append(c);
				for (i++; i < array.Length; i++)
				{
					c = array[i];
					stringBuilder.Append(c);
					if (c == '\'')
					{
						break;
					}
				}
				continue;
			case '[':
				if (!flag3)
				{
					flag3 = true;
					flag2 = false;
					stringBuilder.Append(c);
					continue;
				}
				break;
			}
			if ((c == ']') & flag3)
			{
				flag3 = false;
				stringBuilder.Append(c);
				continue;
			}
			if (flag3)
			{
				switch (c)
				{
				case 'H':
				case 'h':
					stringBuilder.Append('H');
					break;
				case 'M':
				case 'm':
					stringBuilder.Append('m');
					break;
				case 'S':
				case 's':
					stringBuilder.Append('s');
					break;
				default:
					stringBuilder.Append(c);
					break;
				}
				continue;
			}
			switch (c)
			{
			case 'H':
			case 'h':
				flag2 = false;
				if (flag)
				{
					stringBuilder.Append('h');
				}
				else
				{
					stringBuilder.Append('H');
				}
				continue;
			case 'M':
			case 'm':
				if (flag2)
				{
					stringBuilder.Append('M');
					list.Add(stringBuilder.Length - 1);
				}
				else
				{
					stringBuilder.Append('m');
				}
				continue;
			case 'S':
			case 's':
				stringBuilder.Append('s');
				foreach (int item in list)
				{
					if (stringBuilder[item] == 'M')
					{
						stringBuilder[item] = 'm';
					}
				}
				flag2 = true;
				list.Clear();
				continue;
			}
			if (char.IsLetter(c))
			{
				flag2 = true;
				list.Clear();
				switch (c)
				{
				case 'Y':
				case 'y':
					stringBuilder.Append('y');
					break;
				case 'D':
				case 'd':
					stringBuilder.Append('d');
					break;
				default:
					stringBuilder.Append(c);
					break;
				}
			}
			else
			{
				stringBuilder.Append(c);
			}
		}
		text = stringBuilder.ToString();
		try
		{
			return new ExcelStyleDateFormatter(text);
		}
		catch (ArgumentException)
		{
			return GetDefaultFormat(cellValue);
		}
	}

	private string cleanFormatForNumber(string formatStr)
	{
		StringBuilder stringBuilder = new StringBuilder(formatStr);
		if (emulateCSV)
		{
			for (int i = 0; i < stringBuilder.Length; i++)
			{
				char c = stringBuilder[i];
				if ((c != '_' && c != '*' && c != '?') || (i > 0 && stringBuilder[i - 1] == '\\'))
				{
					continue;
				}
				if (c == '?')
				{
					stringBuilder[i] = ' ';
				}
				else if (i < stringBuilder.Length - 1)
				{
					if (c == '_')
					{
						stringBuilder[i + 1] = ' ';
					}
					else
					{
						stringBuilder.Remove(i + 1, 1);
					}
					stringBuilder.Remove(i, 1);
					i--;
				}
			}
		}
		else
		{
			for (int j = 0; j < stringBuilder.Length; j++)
			{
				char c2 = stringBuilder[j];
				if ((c2 == '_' || c2 == '*') && (j <= 0 || stringBuilder[j - 1] != '\\'))
				{
					if (j < stringBuilder.Length - 1)
					{
						stringBuilder.Remove(j + 1, 1);
					}
					stringBuilder.Remove(j, 1);
					j--;
				}
			}
		}
		for (int k = 0; k < stringBuilder.Length; k++)
		{
			switch (stringBuilder[k])
			{
			case '"':
			case '\\':
				stringBuilder.Remove(k, 1);
				k--;
				break;
			case '+':
				if (k > 0 && stringBuilder[k - 1] == 'E')
				{
					stringBuilder.Remove(k, 1);
					k--;
				}
				break;
			}
		}
		return stringBuilder.ToString();
	}

	private FormatBase CreateNumberFormat(string formatStr, double cellValue)
	{
		string text = cleanFormatForNumber(formatStr);
		NumberFormatInfo numberFormatInfo = decimalSymbols;
		Match match = alternateGrouping.Match(text);
		if (match.Success)
		{
			char c = match.Groups[2].Value[0];
			if (c != ',')
			{
				numberFormatInfo = currentCulture.NumberFormat.Clone() as NumberFormatInfo;
				numberFormatInfo.NumberGroupSeparator = c.ToString();
				string value = match.Groups[1].Value;
				string newValue = value.Replace(c, ',');
				text = text.Replace(value, newValue);
			}
		}
		try
		{
			return new DecimalFormat(text, numberFormatInfo);
		}
		catch (ArgumentException)
		{
			return GetDefaultFormat(cellValue);
		}
	}

	public FormatBase GetDefaultFormat(ICell cell)
	{
		return GetDefaultFormat(cell.NumericCellValue);
	}

	private FormatBase GetDefaultFormat(double cellValue)
	{
		if (defaultNumFormat != null)
		{
			return defaultNumFormat;
		}
		return generalNumberFormat;
	}

	private string GetFormattedDateString(ICell cell)
	{
		FormatBase format = GetFormat(cell);
		if (format is ExcelStyleDateFormatter)
		{
			((ExcelStyleDateFormatter)format).SetDateToBeFormatted(cell.NumericCellValue);
		}
		DateTime dateCellValue = cell.DateCellValue;
		return PerformDateFormatting(dateCellValue, format);
	}

	private string GetFormattedNumberString(ICell cell)
	{
		FormatBase format = GetFormat(cell);
		double numericCellValue = cell.NumericCellValue;
		if (format == null)
		{
			return numericCellValue.ToString(currentCulture);
		}
		string text = format.Format(numericCellValue);
		if (text.StartsWith("."))
		{
			text = "0" + text;
		}
		if (text.StartsWith("-."))
		{
			text = "-0" + text.Substring(1);
		}
		return Regex.Replace(text, "E(\\d)", "E+$1");
	}

	public string FormatRawCellContents(double value, int formatIndex, string formatString)
	{
		return FormatRawCellContents(value, formatIndex, formatString, use1904Windowing: false);
	}

	private string PerformDateFormatting(DateTime d, FormatBase dateFormat)
	{
		if (dateFormat != null)
		{
			return dateFormat.Format(d);
		}
		return defaultDateformat.Format(d);
	}

	public string FormatRawCellContents(double value, int formatIndex, string formatString, bool use1904Windowing)
	{
		if (DateUtil.IsADateFormat(formatIndex, formatString))
		{
			if (DateUtil.IsValidExcelDate(value))
			{
				FormatBase format = GetFormat(value, formatIndex, formatString);
				if (format is ExcelStyleDateFormatter)
				{
					((ExcelStyleDateFormatter)format).SetDateToBeFormatted(value);
				}
				DateTime javaDate = DateUtil.GetJavaDate(value, use1904Windowing);
				return PerformDateFormatting(javaDate, format);
			}
			if (emulateCSV)
			{
				return invalidDateTimeString;
			}
		}
		FormatBase format2 = GetFormat(value, formatIndex, formatString);
		if (format2 == null)
		{
			return value.ToString(currentCulture);
		}
		string text = NumberToTextConverter.ToText(value);
		string text2 = ((text.IndexOf('E') <= -1) ? format2.Format(decimal.Parse(text)) : format2.Format(value));
		if (text2.Contains("E") && !text2.Contains("E-"))
		{
			text2 = text2.Replace("E", "E+");
		}
		return text2;
	}

	public string FormatCellValue(ICell cell)
	{
		return FormatCellValue(cell, null);
	}

	public string FormatCellValue(ICell cell, IFormulaEvaluator evaluator)
	{
		if (cell == null)
		{
			return "";
		}
		CellType cellType = cell.CellType;
		if (evaluator != null && cellType == CellType.Formula)
		{
			if (evaluator == null)
			{
				return cell.CellFormula;
			}
			cellType = evaluator.EvaluateFormulaCell(cell);
		}
		switch (cellType)
		{
		case CellType.Formula:
			return cell.CellFormula;
		case CellType.Numeric:
			if (DateUtil.IsCellDateFormatted(cell))
			{
				return GetFormattedDateString(cell);
			}
			return GetFormattedNumberString(cell);
		case CellType.String:
			return cell.RichStringCellValue.String;
		case CellType.Boolean:
			if (!cell.BooleanCellValue)
			{
				return "FALSE";
			}
			return "TRUE";
		case CellType.Blank:
			return "";
		case CellType.Error:
			return FormulaError.ForInt(cell.ErrorCellValue).String;
		default:
			throw new Exception("Unexpected celltype (" + cellType.ToString() + ")");
		}
	}

	public void SetDefaultNumberFormat(FormatBase format)
	{
		IEnumerator enumerator = formats.Keys.GetEnumerator();
		while (enumerator.MoveNext())
		{
			string key = (string)enumerator.Current;
			if (formats[key] == generalNumberFormat)
			{
				formats[key] = format;
			}
		}
		defaultNumFormat = format;
	}

	public void AddFormat(string excelformatStr, FormatBase format)
	{
		formats[excelformatStr] = format;
	}

	public void Update(IObservable<object> observable, object localeObj)
	{
		if (localeObj is CultureInfo)
		{
			CultureInfo cultureInfo = (CultureInfo)localeObj;
			if (!cultureInfo.Equals(currentCulture))
			{
				currentCulture = cultureInfo;
				generalNumberFormat = new ExcelGeneralNumberFormat(currentCulture);
				formats.Clear();
				FormatBase instance = ZipPlusFourFormat.Instance;
				AddFormat("00000\\-0000", instance);
				AddFormat("00000-0000", instance);
				FormatBase instance2 = PhoneFormat.Instance;
				AddFormat("[<=9999999]###\\-####;\\(###\\)\\ ###\\-####", instance2);
				AddFormat("[<=9999999]###-####;(###) ###-####", instance2);
				AddFormat("###\\-####;\\(###\\)\\ ###\\-####", instance2);
				AddFormat("###-####;(###) ###-####", instance2);
				FormatBase instance3 = SSNFormat.Instance;
				AddFormat("000\\-00\\-0000", instance3);
				AddFormat("000-00-0000", instance3);
			}
		}
	}
}
