using System;
using System.Globalization;
using System.Text;
using NPOI.HSSF.Model;
using NPOI.HSSF.Record;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.HSSF.UserModel;

public class DVConstraint : IDataValidationConstraint
{
	public class FormulaPair
	{
		private Ptg[] _formula1;

		private Ptg[] _formula2;

		public Ptg[] Formula1 => _formula1;

		public Ptg[] Formula2 => _formula2;

		public FormulaPair(Ptg[] formula1, Ptg[] formula2)
		{
			_formula1 = ((formula1 == null) ? null : ((Ptg[])formula1.Clone()));
			_formula2 = ((formula2 == null) ? null : ((Ptg[])formula2.Clone()));
		}
	}

	private class FormulaValuePair
	{
		internal string _formula;

		internal string _value;

		public double Value
		{
			get
			{
				if (_value == null)
				{
					return double.NaN;
				}
				return double.Parse(_value);
			}
		}

		public string formula()
		{
			return _formula;
		}

		public string AsString()
		{
			if (_formula != null)
			{
				return _formula;
			}
			if (_value != null)
			{
				return _value;
			}
			return null;
		}
	}

	private int _validationType;

	private int _operator;

	private string[] _explicitListValues;

	private string _formula1;

	private string _formula2;

	private double _value1;

	private double _value2;

	public bool IsListValidationType => _validationType == 3;

	public bool IsExplicitList
	{
		get
		{
			if (_validationType == 3)
			{
				return _explicitListValues != null;
			}
			return false;
		}
	}

	public int Operator
	{
		get
		{
			return _operator;
		}
		set
		{
			_operator = value;
		}
	}

	public string[] ExplicitListValues
	{
		get
		{
			return _explicitListValues;
		}
		set
		{
			if (_validationType != 3)
			{
				throw new InvalidOperationException("Cannot setExplicitListValues on non-list constraint");
			}
			_formula1 = null;
			_explicitListValues = value;
		}
	}

	public string Formula1
	{
		get
		{
			return _formula1;
		}
		set
		{
			_value1 = double.NaN;
			_explicitListValues = null;
			_formula1 = value;
		}
	}

	public string Formula2
	{
		get
		{
			return _formula2;
		}
		set
		{
			_value2 = double.NaN;
			_formula2 = value;
		}
	}

	public double Value1
	{
		get
		{
			return _value1;
		}
		set
		{
			_formula1 = null;
			_value1 = value;
		}
	}

	public double Value2
	{
		get
		{
			return _value2;
		}
		set
		{
			_formula2 = null;
			_value2 = value;
		}
	}

	private DVConstraint(int validationType, int comparisonOperator, string formulaA, string formulaB, double value1, double value2, string[] excplicitListValues)
	{
		_validationType = validationType;
		_operator = comparisonOperator;
		_formula1 = formulaA;
		_formula2 = formulaB;
		_value1 = value1;
		_value2 = value2;
		_explicitListValues = ((excplicitListValues == null) ? null : ((string[])excplicitListValues.Clone()));
	}

	private DVConstraint(string listFormula, string[] excplicitListValues)
		: this(3, 0, listFormula, null, double.NaN, double.NaN, excplicitListValues)
	{
	}

	public static DVConstraint CreateNumericConstraint(int validationType, int comparisonOperator, string expr1, string expr2)
	{
		switch (validationType)
		{
		case 0:
			if (expr1 != null || expr2 != null)
			{
				throw new ArgumentException("expr1 and expr2 must be null for validation type 'any'");
			}
			break;
		case 1:
		case 2:
		case 6:
			if (expr1 == null)
			{
				throw new ArgumentException("expr1 must be supplied");
			}
			OperatorType.ValidateSecondArg(comparisonOperator, expr2);
			break;
		default:
			throw new ArgumentException("Validation Type (" + validationType + ") not supported with this method");
		}
		string formulaFromTextExpression = GetFormulaFromTextExpression(expr1);
		double value = ((formulaFromTextExpression == null) ? ConvertNumber(expr1) : double.NaN);
		string formulaFromTextExpression2 = GetFormulaFromTextExpression(expr2);
		double value2 = ((formulaFromTextExpression2 == null) ? ConvertNumber(expr2) : double.NaN);
		return new DVConstraint(validationType, comparisonOperator, formulaFromTextExpression, formulaFromTextExpression2, value, value2, null);
	}

	public static DVConstraint CreateFormulaListConstraint(string listFormula)
	{
		return new DVConstraint(listFormula, null);
	}

	public static DVConstraint CreateExplicitListConstraint(string[] explicitListValues)
	{
		return new DVConstraint(null, explicitListValues);
	}

	public static DVConstraint CreateTimeConstraint(int comparisonOperator, string expr1, string expr2)
	{
		if (expr1 == null)
		{
			throw new ArgumentException("expr1 must be supplied");
		}
		OperatorType.ValidateSecondArg(comparisonOperator, expr1);
		string formulaFromTextExpression = GetFormulaFromTextExpression(expr1);
		double value = ((formulaFromTextExpression == null) ? ConvertTime(expr1) : double.NaN);
		string formulaFromTextExpression2 = GetFormulaFromTextExpression(expr2);
		double value2 = ((formulaFromTextExpression2 == null) ? ConvertTime(expr2) : double.NaN);
		return new DVConstraint(5, comparisonOperator, formulaFromTextExpression, formulaFromTextExpression2, value, value2, null);
	}

	public static DVConstraint CreateDateConstraint(int comparisonOperator, string expr1, string expr2, string dateFormat)
	{
		if (expr1 == null)
		{
			throw new ArgumentException("expr1 must be supplied");
		}
		OperatorType.ValidateSecondArg(comparisonOperator, expr2);
		SimpleDateFormat dateFormat2 = ((dateFormat == null) ? null : new SimpleDateFormat(dateFormat));
		string formulaFromTextExpression = GetFormulaFromTextExpression(expr1);
		double value = ((formulaFromTextExpression == null) ? ConvertDate(expr1, dateFormat2) : double.NaN);
		string formulaFromTextExpression2 = GetFormulaFromTextExpression(expr2);
		double value2 = ((formulaFromTextExpression2 == null) ? ConvertDate(expr2, dateFormat2) : double.NaN);
		return new DVConstraint(4, comparisonOperator, formulaFromTextExpression, formulaFromTextExpression2, value, value2, null);
	}

	private static string GetFormulaFromTextExpression(string textExpr)
	{
		if (textExpr == null)
		{
			return null;
		}
		if (textExpr.Length < 1)
		{
			throw new ArgumentException("Empty string is not a valid formula/value expression");
		}
		if (textExpr[0] == '=')
		{
			return textExpr.Substring(1);
		}
		return null;
	}

	private static double ConvertNumber(string numberStr)
	{
		if (numberStr == null)
		{
			return double.NaN;
		}
		try
		{
			return double.Parse(numberStr, CultureInfo.CurrentCulture);
		}
		catch (FormatException)
		{
			throw new InvalidOperationException("The supplied text '" + numberStr + "' could not be parsed as a number");
		}
	}

	private static double ConvertTime(string timeStr)
	{
		if (timeStr == null)
		{
			return double.NaN;
		}
		return DateUtil.ConvertTime(timeStr);
	}

	private static double ConvertDate(string dateStr, SimpleDateFormat dateFormat)
	{
		if (dateStr == null)
		{
			return double.NaN;
		}
		DateTime date;
		if (dateFormat == null)
		{
			date = DateUtil.ParseYYYYMMDDDate(dateStr);
		}
		else
		{
			try
			{
				date = DateTime.Parse(dateStr, CultureInfo.CurrentCulture);
			}
			catch (FormatException innerException)
			{
				throw new InvalidOperationException("Failed to parse date '" + dateStr + "' using specified format '" + dateFormat?.ToString() + "'", innerException);
			}
		}
		return DateUtil.GetExcelDate(date);
	}

	public static DVConstraint CreateCustomFormulaConstraint(string formula)
	{
		if (formula == null)
		{
			throw new ArgumentException("formula must be supplied");
		}
		return new DVConstraint(7, 0, formula, null, double.NaN, double.NaN, null);
	}

	public int GetValidationType()
	{
		return _validationType;
	}

	public FormulaPair CreateFormulas(HSSFSheet sheet)
	{
		Ptg[] formula;
		Ptg[] formula2;
		if (IsListValidationType)
		{
			formula = CreateListFormula(sheet);
			formula2 = Ptg.EMPTY_PTG_ARRAY;
		}
		else
		{
			formula = ConvertDoubleFormula(_formula1, _value1, sheet);
			formula2 = ConvertDoubleFormula(_formula2, _value2, sheet);
		}
		return new FormulaPair(formula, formula2);
	}

	private Ptg[] CreateListFormula(HSSFSheet sheet)
	{
		if (_explicitListValues == null)
		{
			IWorkbook workbook = sheet.Workbook;
			return HSSFFormulaParser.Parse(_formula1, (HSSFWorkbook)workbook, FormulaType.DataValidationList, workbook.GetSheetIndex(sheet));
		}
		StringBuilder stringBuilder = new StringBuilder(_explicitListValues.Length * 16);
		for (int i = 0; i < _explicitListValues.Length; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append('\0');
			}
			stringBuilder.Append(_explicitListValues[i]);
		}
		return new Ptg[1]
		{
			new StringPtg(stringBuilder.ToString())
		};
	}

	private static Ptg[] ConvertDoubleFormula(string formula, double value, HSSFSheet sheet)
	{
		if (formula == null)
		{
			if (double.IsNaN(value))
			{
				return Ptg.EMPTY_PTG_ARRAY;
			}
			return new Ptg[1]
			{
				new NumberPtg(value)
			};
		}
		if (!double.IsNaN(value))
		{
			throw new InvalidOperationException("Both formula and value cannot be present");
		}
		IWorkbook workbook = sheet.Workbook;
		return HSSFFormulaParser.Parse(formula, (HSSFWorkbook)workbook, FormulaType.Cell, workbook.GetSheetIndex(sheet));
	}

	internal static DVConstraint CreateDVConstraint(DVRecord dvRecord, IFormulaRenderingWorkbook book)
	{
		switch (dvRecord.DataType)
		{
		case 0:
			return new DVConstraint(0, dvRecord.ConditionOperator, null, null, double.NaN, double.NaN, null);
		case 1:
		case 2:
		case 4:
		case 5:
		case 6:
		{
			FormulaValuePair formulaValuePair = toFormulaString(dvRecord.Formula1, book);
			FormulaValuePair formulaValuePair2 = toFormulaString(dvRecord.Formula2, book);
			return new DVConstraint(dvRecord.DataType, dvRecord.ConditionOperator, formulaValuePair.formula(), formulaValuePair2.formula(), formulaValuePair.Value, formulaValuePair2.Value, null);
		}
		case 3:
			if (dvRecord.ListExplicitFormula)
			{
				string text = toFormulaString(dvRecord.Formula1, book).AsString();
				if (text.StartsWith("\""))
				{
					text = text.Substring(1);
				}
				if (text.EndsWith("\""))
				{
					text = text.Substring(0, text.Length - 1);
				}
				return CreateExplicitListConstraint(text.Split("\0".ToCharArray()));
			}
			return CreateFormulaListConstraint(toFormulaString(dvRecord.Formula1, book).AsString());
		case 7:
			return CreateCustomFormulaConstraint(toFormulaString(dvRecord.Formula1, book).AsString());
		default:
			throw new InvalidOperationException($"validationType={dvRecord.DataType}");
		}
	}

	private static FormulaValuePair toFormulaString(Ptg[] ptgs, IFormulaRenderingWorkbook book)
	{
		FormulaValuePair formulaValuePair = new FormulaValuePair();
		if (ptgs != null && ptgs.Length != 0)
		{
			string text = FormulaRenderer.ToFormulaString(book, ptgs);
			if (ptgs.Length == 1 && ptgs[0].GetType() == typeof(NumberPtg))
			{
				formulaValuePair._value = text;
			}
			else
			{
				formulaValuePair._formula = text;
			}
		}
		return formulaValuePair;
	}
}
