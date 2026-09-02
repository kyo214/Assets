using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.SS.Formula.Constant;
using NPOI.SS.Formula.Function;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.SS.Formula;

public class FormulaParser
{
	private class SimpleRangePart
	{
		public enum PartType
		{
			Cell = 0,
			Row = 1,
			Column = 2
		}

		private PartType _type;

		private string _rep;

		public bool IsCell => _type == PartType.Cell;

		public bool IsRowOrColumn => _type != PartType.Cell;

		public CellReference CellReference
		{
			get
			{
				if (_type != PartType.Cell)
				{
					throw new InvalidOperationException("Not applicable to this type");
				}
				return new CellReference(_rep);
			}
		}

		public bool IsColumn => _type == PartType.Column;

		public bool IsRow => _type == PartType.Row;

		public string Rep => _rep;

		public static PartType Get(bool hasLetters, bool hasDigits)
		{
			if (hasLetters)
			{
				if (!hasDigits)
				{
					return PartType.Column;
				}
				return PartType.Cell;
			}
			if (!hasDigits)
			{
				throw new ArgumentException("must have either letters or numbers");
			}
			return PartType.Row;
		}

		public SimpleRangePart(string rep, bool hasLetters, bool hasNumbers)
		{
			_rep = rep;
			_type = Get(hasLetters, hasNumbers);
		}

		public bool IsCompatibleForArea(SimpleRangePart part2)
		{
			return _type == part2._type;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.Append(GetType().Name).Append(" [");
			stringBuilder.Append(_rep);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}

	private string _formulaString;

	private int _formulaLength;

	private int _pointer;

	private ParseNode _rootNode;

	private const char TAB = '\t';

	private const char CR = '\r';

	private const char LF = '\n';

	private char look;

	private bool _inIntersection;

	private IFormulaParsingWorkbook _book;

	private static SpreadsheetVersion _ssVersion;

	private int _sheetIndex;

	private int _rowIndex;

	private static string specHeaders = "Headers";

	private static string specAll = "All";

	private static string specData = "Data";

	private static string specTotals = "Totals";

	private static string specThisRow = "This Row";

	private string CELL_REF_PATTERN = "(\\$?[A-Za-z]+)?(\\$?[0-9]+)?";

	public FormulaParser(string formula, IFormulaParsingWorkbook book, int sheetIndex, int rowIndex)
	{
		_formulaString = formula;
		_pointer = 0;
		_book = book;
		_ssVersion = ((book == null) ? SpreadsheetVersion.EXCEL97 : book.GetSpreadsheetVersion());
		_formulaLength = _formulaString.Length;
		_sheetIndex = sheetIndex;
		_rowIndex = rowIndex;
	}

	public static Ptg[] Parse(string formula, IFormulaParsingWorkbook workbook, FormulaType formulaType, int sheetIndex, int rowIndex)
	{
		FormulaParser formulaParser = new FormulaParser(formula, workbook, sheetIndex, rowIndex);
		formulaParser.Parse();
		return formulaParser.GetRPNPtg(formulaType);
	}

	public static Ptg[] Parse(string formula, IFormulaParsingWorkbook workbook, FormulaType formulaType, int sheetIndex)
	{
		return Parse(formula, workbook, formulaType, sheetIndex, -1);
	}

	public static Area3DPxg ParseStructuredReference(string tableText, IFormulaParsingWorkbook workbook, int rowIndex)
	{
		Ptg[] array = Parse(tableText, workbook, FormulaType.Cell, 0, rowIndex);
		if (array.Length != 1 || !(array[0] is Area3DPxg))
		{
			throw new InvalidOperationException("Illegal structured reference");
		}
		return (Area3DPxg)array[0];
	}

	private void GetChar()
	{
		if (IsWhite(look))
		{
			if (look == ' ')
			{
				_inIntersection = true;
			}
		}
		else
		{
			_inIntersection = false;
		}
		if (_pointer > _formulaLength)
		{
			throw new Exception("too far");
		}
		if (_pointer < _formulaLength)
		{
			look = _formulaString[_pointer];
		}
		else
		{
			look = '\0';
			_inIntersection = false;
		}
		_pointer++;
	}

	private Exception expected(string s)
	{
		string msg = ((look != '=' || _formulaString.Substring(0, _pointer - 1).Trim().Length >= 1) ? ("Parse error near char " + (_pointer - 1) + " '" + look + "' in specified formula '" + _formulaString + "'. Expected " + s) : ("The specified formula '" + _formulaString + "' starts with an equals sign which is not allowed."));
		return new FormulaParseException(msg);
	}

	private static bool IsAlpha(char c)
	{
		if (!char.IsLetter(c) && c != '$')
		{
			return c == '_';
		}
		return true;
	}

	private static bool IsDigit(char c)
	{
		return char.IsDigit(c);
	}

	private static bool IsAlNum(char c)
	{
		if (!IsAlpha(c))
		{
			return IsDigit(c);
		}
		return true;
	}

	private static bool IsWhite(char c)
	{
		if (c != ' ' && c != '\t' && c != '\r')
		{
			return c == '\n';
		}
		return true;
	}

	private void SkipWhite()
	{
		while (IsWhite(look))
		{
			GetChar();
		}
	}

	private void Match(char x)
	{
		if (look != x)
		{
			throw expected("'" + x + "'");
		}
		GetChar();
	}

	private string ParseUnquotedIdentifier()
	{
		if (look == '\'')
		{
			throw expected("unquoted identifier");
		}
		StringBuilder stringBuilder = new StringBuilder();
		while (char.IsLetterOrDigit(look) || look == '.')
		{
			stringBuilder.Append(look);
			GetChar();
		}
		if (stringBuilder.Length < 1)
		{
			return null;
		}
		return stringBuilder.ToString();
	}

	private string GetNum()
	{
		StringBuilder stringBuilder = new StringBuilder();
		while (IsDigit(look))
		{
			stringBuilder.Append(look);
			GetChar();
		}
		if (stringBuilder.Length != 0)
		{
			return stringBuilder.ToString();
		}
		return null;
	}

	private ParseNode ParseRangeExpression()
	{
		ParseNode parseNode = ParseRangeable();
		bool flag = false;
		while (look == ':')
		{
			int pointer = _pointer;
			GetChar();
			ParseNode parseNode2 = ParseRangeable();
			CheckValidRangeOperand("LHS", pointer, parseNode);
			CheckValidRangeOperand("RHS", pointer, parseNode2);
			ParseNode[] children = new ParseNode[2] { parseNode, parseNode2 };
			parseNode = new ParseNode(RangePtg.instance, children);
			flag = true;
		}
		if (flag)
		{
			return AugmentWithMemPtg(parseNode);
		}
		return parseNode;
	}

	private static ParseNode AugmentWithMemPtg(ParseNode root)
	{
		Ptg token = ((!NeedsMemFunc(root)) ? ((OperandPtg)new MemAreaPtg(root.EncodedSize)) : ((OperandPtg)new MemFuncPtg(root.EncodedSize)));
		return new ParseNode(token, root);
	}

	private static bool NeedsMemFunc(ParseNode root)
	{
		Ptg token = root.GetToken();
		if (token is AbstractFunctionPtg)
		{
			return true;
		}
		if (token is IExternSheetReferenceToken)
		{
			return true;
		}
		if (token is NamePtg || token is NameXPtg)
		{
			return true;
		}
		if (token is OperationPtg || token is ParenthesisPtg)
		{
			ParseNode[] children = root.GetChildren();
			for (int i = 0; i < children.Length; i++)
			{
				if (NeedsMemFunc(children[i]))
				{
					return true;
				}
			}
			return false;
		}
		if (token is OperandPtg)
		{
			return false;
		}
		if (token is OperationPtg)
		{
			return true;
		}
		return false;
	}

	private static bool IsValidDefinedNameChar(char ch)
	{
		if (char.IsLetterOrDigit(ch))
		{
			return true;
		}
		switch (ch)
		{
		case '.':
		case '?':
		case '\\':
		case '_':
			return true;
		default:
			return false;
		}
	}

	private void CheckValidRangeOperand(string sideName, int currentParsePosition, ParseNode pn)
	{
		if (!IsValidRangeOperand(pn))
		{
			throw new FormulaParseException("The " + sideName + " of the range operator ':' at position " + currentParsePosition + " is not a proper reference.");
		}
	}

	private bool IsValidRangeOperand(ParseNode a)
	{
		Ptg token = a.GetToken();
		if (token is OperandPtg)
		{
			return true;
		}
		if (token is AbstractFunctionPtg)
		{
			byte defaultOperandClass = ((AbstractFunctionPtg)token).DefaultOperandClass;
			return defaultOperandClass == 0;
		}
		if (token is ValueOperatorPtg)
		{
			return false;
		}
		if (token is OperationPtg)
		{
			return true;
		}
		if (token is ParenthesisPtg)
		{
			return IsValidRangeOperand(a.GetChildren()[0]);
		}
		if (token == ErrPtg.REF_INVALID)
		{
			return true;
		}
		return false;
	}

	private ParseNode ParseRangeable()
	{
		SkipWhite();
		int pointer = _pointer;
		SheetIdentifier sheetIdentifier = ParseSheetName();
		if (sheetIdentifier == null)
		{
			ResetPointer(pointer);
		}
		else
		{
			SkipWhite();
			pointer = _pointer;
		}
		SimpleRangePart simpleRangePart = ParseSimpleRangePart();
		if (simpleRangePart == null)
		{
			if (sheetIdentifier != null)
			{
				if (look == '#')
				{
					return new ParseNode(ErrPtg.ValueOf(ParseErrorLiteral()));
				}
				string text = ParseAsName();
				if (text.Length == 0)
				{
					throw new FormulaParseException("Cell reference or Named Range expected after sheet name at index " + _pointer + ".");
				}
				Ptg nameXPtg = _book.GetNameXPtg(text, sheetIdentifier);
				if (nameXPtg == null)
				{
					throw new FormulaParseException("Specified name '" + text + "' for sheet " + sheetIdentifier.AsFormulaString() + " not found");
				}
				return new ParseNode(nameXPtg);
			}
			return ParseNonRange(pointer);
		}
		bool flag = IsWhite(look);
		if (flag)
		{
			SkipWhite();
		}
		if (look == ':')
		{
			int pointer2 = _pointer;
			GetChar();
			SkipWhite();
			SimpleRangePart simpleRangePart2 = ParseSimpleRangePart();
			if (simpleRangePart2 != null && !simpleRangePart.IsCompatibleForArea(simpleRangePart2))
			{
				simpleRangePart2 = null;
			}
			if (simpleRangePart2 == null)
			{
				ResetPointer(pointer2);
				if (!simpleRangePart.IsCell)
				{
					string text2 = ((sheetIdentifier != null) ? ("'" + sheetIdentifier.SheetId.Name + "!") : "");
					throw new FormulaParseException(text2 + simpleRangePart.Rep + "' is not a proper reference.");
				}
				return CreateAreaRefParseNode(sheetIdentifier, simpleRangePart, simpleRangePart2);
			}
			return CreateAreaRefParseNode(sheetIdentifier, simpleRangePart, simpleRangePart2);
		}
		if (look == '.')
		{
			GetChar();
			int num = 1;
			while (look == '.')
			{
				num++;
				GetChar();
			}
			bool flag2 = IsWhite(look);
			SkipWhite();
			SimpleRangePart simpleRangePart3 = ParseSimpleRangePart();
			string text3 = _formulaString.Substring(pointer - 1, _pointer - pointer);
			if (simpleRangePart3 == null)
			{
				if (sheetIdentifier != null)
				{
					throw new FormulaParseException("Complete area reference expected after sheet name at index " + _pointer + ".");
				}
				return ParseNonRange(pointer);
			}
			if (flag | flag2)
			{
				if (simpleRangePart.IsRowOrColumn || simpleRangePart3.IsRowOrColumn)
				{
					throw new FormulaParseException("Dotted range (full row or column) expression '" + text3 + "' must not contain whitespace.");
				}
				return CreateAreaRefParseNode(sheetIdentifier, simpleRangePart, simpleRangePart3);
			}
			if (num == 1 && simpleRangePart.IsRow && simpleRangePart3.IsRow)
			{
				return ParseNonRange(pointer);
			}
			if ((simpleRangePart.IsRowOrColumn || simpleRangePart3.IsRowOrColumn) && num != 2)
			{
				throw new FormulaParseException("Dotted range (full row or column) expression '" + text3 + "' must have exactly 2 dots.");
			}
			return CreateAreaRefParseNode(sheetIdentifier, simpleRangePart, simpleRangePart3);
		}
		if (simpleRangePart.IsCell && IsValidCellReference(simpleRangePart.Rep))
		{
			return CreateAreaRefParseNode(sheetIdentifier, simpleRangePart, null);
		}
		if (sheetIdentifier != null)
		{
			throw new FormulaParseException("Second part of cell reference expected after sheet name at index " + _pointer + ".");
		}
		return ParseNonRange(pointer);
	}

	private ParseNode ParseStructuredReference(string tableName)
	{
		if (!_ssVersion.Equals(SpreadsheetVersion.EXCEL2007))
		{
			throw new FormulaParseException("Strctured references work only on XSSF (Excel 2007)!");
		}
		ITable table = _book.GetTable(tableName);
		if (table == null)
		{
			throw new FormulaParseException("Illegal table name: '" + tableName + "'");
		}
		string sheetName = table.SheetName;
		int startColIndex = table.StartColIndex;
		int endColIndex = table.EndColIndex;
		int startRowIndex = table.StartRowIndex;
		int endRowIndex = table.EndRowIndex;
		int pointer = _pointer;
		GetChar();
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		int num = 0;
		int pointer2;
		while (true)
		{
			pointer2 = _pointer;
			string text = ParseAsSpecialQuantifier();
			if (text == null)
			{
				ResetPointer(pointer2);
				break;
			}
			if (text.Equals(specAll))
			{
				flag5 = true;
			}
			else if (text.Equals(specData))
			{
				flag3 = true;
			}
			else if (text.Equals(specHeaders))
			{
				flag4 = true;
			}
			else if (text.Equals(specThisRow))
			{
				flag2 = true;
			}
			else
			{
				if (!text.Equals(specTotals))
				{
					throw new FormulaParseException("Unknown special quantifier " + text);
				}
				flag = true;
			}
			num++;
			if (look != ',')
			{
				break;
			}
			GetChar();
		}
		bool flag6 = false;
		SkipWhite();
		if (look == '@')
		{
			flag6 = true;
			GetChar();
		}
		string text2 = null;
		string text3 = null;
		int num2 = 0;
		pointer2 = _pointer;
		text2 = ParseAsColumnQuantifier();
		if (text2 == null)
		{
			ResetPointer(pointer2);
		}
		else
		{
			num2++;
			if (look == ',')
			{
				throw new FormulaParseException("The formula " + _formulaString + "is illegal: you should not use ',' with column quantifiers");
			}
			if (look == ':')
			{
				GetChar();
				text3 = ParseAsColumnQuantifier();
				num2++;
				if (text3 == null)
				{
					throw new FormulaParseException("The formula " + _formulaString + "is illegal: the string after ':' must be column quantifier");
				}
			}
		}
		if (num2 == 0 && num == 0)
		{
			ResetPointer(pointer);
			pointer = _pointer;
			text2 = ParseAsColumnQuantifier();
			if (text2 != null)
			{
				num2++;
			}
			else
			{
				ResetPointer(pointer);
				string text4 = ParseAsSpecialQuantifier();
				if (text4 == null)
				{
					throw new FormulaParseException("The formula " + _formulaString + " is illegal");
				}
				if (text4.Equals(specAll))
				{
					flag5 = true;
				}
				else if (text4.Equals(specData))
				{
					flag3 = true;
				}
				else if (text4.Equals(specHeaders))
				{
					flag4 = true;
				}
				else if (text4.Equals(specThisRow))
				{
					flag2 = true;
				}
				else
				{
					if (!text4.Equals(specTotals))
					{
						throw new FormulaParseException("Unknown special quantifier " + text4);
					}
					flag = true;
				}
				num++;
			}
		}
		else
		{
			Match(']');
		}
		if (flag && !table.IsHasTotalsRow)
		{
			return new ParseNode(ErrPtg.REF_INVALID);
		}
		if ((flag6 | flag2) && (_rowIndex < startRowIndex || endRowIndex < _rowIndex))
		{
			if (_rowIndex >= 0)
			{
				return new ParseNode(ErrPtg.VALUE_INVALID);
			}
			throw new FormulaParseException("Formula contained [#This Row] or [@] structured reference but this row < 0. Row index must be specified for row-referencing structured references.");
		}
		int num3 = startRowIndex;
		int num4 = endRowIndex;
		int num5 = startColIndex;
		int pCol = endColIndex;
		if (num > 0)
		{
			if (!((num == 1) & flag5))
			{
				if (flag3 & flag4)
				{
					if (table.IsHasTotalsRow)
					{
						num4 = endRowIndex - 1;
					}
				}
				else if (flag3 & flag)
				{
					num3 = startRowIndex + 1;
				}
				else if ((num == 1) & flag3)
				{
					num3 = startRowIndex + 1;
					if (table.IsHasTotalsRow)
					{
						num4 = endRowIndex - 1;
					}
				}
				else if ((num == 1) & flag4)
				{
					num4 = num3;
				}
				else if ((num == 1) & flag)
				{
					num3 = num4;
				}
				else
				{
					if (!(((num == 1) & flag2) | flag6))
					{
						throw new FormulaParseException("The formula " + _formulaString + " is illegal");
					}
					num3 = _rowIndex;
					num4 = _rowIndex;
				}
			}
		}
		else if (flag6)
		{
			num3 = _rowIndex;
			num4 = _rowIndex;
		}
		else
		{
			num3++;
		}
		switch (num2)
		{
		case 2:
		{
			if (text2 == null || text3 == null)
			{
				throw new InvalidOperationException("Fatal error");
			}
			int num7 = table.FindColumnIndex(text2);
			int num8 = table.FindColumnIndex(text3);
			if (num7 == -1 || num8 == -1)
			{
				throw new FormulaParseException("One of the columns " + text2 + ", " + text3 + " doesn't exist in table " + table.Name);
			}
			num5 = startColIndex + num7;
			pCol = startColIndex + num8;
			break;
		}
		case 1:
			if (!flag6)
			{
				if (text2 == null)
				{
					throw new InvalidOperationException("Fatal error");
				}
				int num6 = table.FindColumnIndex(text2);
				if (num6 == -1)
				{
					throw new FormulaParseException("The column " + text2 + " doesn't exist in table " + table.Name);
				}
				num5 = startColIndex + num6;
				pCol = num5;
			}
			break;
		}
		CellReference topLeft = new CellReference(num3, num5);
		CellReference botRight = new CellReference(num4, pCol);
		SheetIdentifier sheet = new SheetIdentifier(null, new NameIdentifier(sheetName, isQuoted: true));
		return new ParseNode(_book.Get3DReferencePtg(new AreaReference(topLeft, botRight), sheet));
	}

	private string ParseAsColumnQuantifier()
	{
		if (look != '[')
		{
			return null;
		}
		GetChar();
		if (look == '#')
		{
			return null;
		}
		if (look == '@')
		{
			GetChar();
		}
		StringBuilder stringBuilder = new StringBuilder();
		while (look != ']')
		{
			stringBuilder.Append(look);
			GetChar();
		}
		Match(']');
		return stringBuilder.ToString();
	}

	private string ParseAsSpecialQuantifier()
	{
		if (look != '[')
		{
			return null;
		}
		GetChar();
		if (look != '#')
		{
			return null;
		}
		GetChar();
		string text = ParseAsName();
		if (text.Equals("This"))
		{
			text = text + " " + ParseAsName();
		}
		Match(']');
		return text;
	}

	private ParseNode ParseNonRange(int savePointer)
	{
		ResetPointer(savePointer);
		if (char.IsDigit(look))
		{
			return new ParseNode(ParseNumber());
		}
		if (look == '"')
		{
			return new ParseNode(new StringPtg(ParseStringLiteral()));
		}
		string text = ParseAsName();
		if (look == '(')
		{
			return Function(text);
		}
		if (look == '[')
		{
			return ParseStructuredReference(text);
		}
		if (text.Equals("TRUE", StringComparison.OrdinalIgnoreCase) || text.Equals("FALSE", StringComparison.OrdinalIgnoreCase))
		{
			return new ParseNode(new BoolPtg(text.ToUpper()));
		}
		if (_book == null)
		{
			throw new InvalidOperationException("Need book to evaluate name '" + text + "'");
		}
		IEvaluationName name = _book.GetName(text, _sheetIndex);
		if (name == null)
		{
			throw new FormulaParseException("Specified named range '" + text + "' does not exist in the current workbook.");
		}
		if (name.IsRange)
		{
			return new ParseNode(name.CreatePtg());
		}
		throw new FormulaParseException("Specified name '" + text + "' is not a range as expected.");
	}

	private string ParseAsName()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (!char.IsLetter(look) && look != '_' && look != '\\')
		{
			throw expected("number, string, defined name, or data table");
		}
		while (IsValidDefinedNameChar(look))
		{
			stringBuilder.Append(look);
			GetChar();
		}
		SkipWhite();
		return stringBuilder.ToString();
	}

	private int GetSheetExtIx(SheetIdentifier sheetIden)
	{
		if (sheetIden == null)
		{
			return int.MaxValue;
		}
		string name = sheetIden.SheetId.Name;
		if (sheetIden.BookName == null)
		{
			return _book.GetExternalSheetIndex(name);
		}
		return _book.GetExternalSheetIndex(sheetIden.BookName, name);
	}

	private ParseNode CreateAreaRefParseNode(SheetIdentifier sheetIden, SimpleRangePart part1, SimpleRangePart part2)
	{
		Ptg token;
		if (part2 == null)
		{
			CellReference cellReference = part1.CellReference;
			token = ((sheetIden != null) ? _book.Get3DReferencePtg(cellReference, sheetIden) : new RefPtg(cellReference));
		}
		else
		{
			AreaReference areaReference = CreateAreaRef(part1, part2);
			token = ((sheetIden != null) ? _book.Get3DReferencePtg(areaReference, sheetIden) : new AreaPtg(areaReference));
		}
		return new ParseNode(token);
	}

	private static AreaReference CreateAreaRef(SimpleRangePart part1, SimpleRangePart part2)
	{
		if (!part1.IsCompatibleForArea(part2))
		{
			throw new FormulaParseException("has incompatible parts: '" + part1.Rep + "' and '" + part2.Rep + "'.");
		}
		if (part1.IsRow)
		{
			return AreaReference.GetWholeRow(_ssVersion, part1.Rep, part2.Rep);
		}
		if (part1.IsColumn)
		{
			return AreaReference.GetWholeColumn(_ssVersion, part1.Rep, part2.Rep);
		}
		return new AreaReference(part1.CellReference, part2.CellReference);
	}

	private SimpleRangePart ParseSimpleRangePart()
	{
		int i = _pointer - 1;
		bool flag = false;
		bool flag2 = false;
		for (; i < _formulaLength; i++)
		{
			char c = _formulaString[i];
			if (char.IsDigit(c))
			{
				flag = true;
			}
			else if (char.IsLetter(c))
			{
				flag2 = true;
			}
			else if (c != '$' && c != '_')
			{
				break;
			}
		}
		if (i <= _pointer - 1)
		{
			return null;
		}
		string text = _formulaString.Substring(_pointer - 1, i - _pointer + 1);
		if (!new Regex(CELL_REF_PATTERN).IsMatch(text))
		{
			return null;
		}
		if (flag2 & flag)
		{
			if (!IsValidCellReference(text))
			{
				return null;
			}
		}
		else if (flag2)
		{
			if (!CellReference.IsColumnWithinRange(text.Replace("$", ""), _ssVersion))
			{
				return null;
			}
		}
		else
		{
			if (!flag)
			{
				return null;
			}
			int num;
			try
			{
				num = int.Parse(text.Replace("$", ""), CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				return null;
			}
			if (num < 1 || num > _ssVersion.MaxRows)
			{
				return null;
			}
		}
		ResetPointer(i + 1);
		return new SimpleRangePart(text, flag2, flag);
	}

	private static Ptg ReduceRangeExpression(Ptg ptgA, Ptg ptgB)
	{
		if (!(ptgB is RefPtg))
		{
			return null;
		}
		RefPtg refPtg = (RefPtg)ptgB;
		if (ptgA is RefPtg)
		{
			RefPtg refPtg2 = (RefPtg)ptgA;
			return new AreaPtg(refPtg2.Row, refPtg.Row, refPtg2.Column, refPtg.Column, refPtg2.IsRowRelative, refPtg.IsRowRelative, refPtg2.IsColRelative, refPtg.IsColRelative);
		}
		if (ptgA is Ref3DPtg)
		{
			Ref3DPtg ref3DPtg = (Ref3DPtg)ptgA;
			return new Area3DPtg(ref3DPtg.Row, refPtg.Row, ref3DPtg.Column, refPtg.Column, ref3DPtg.IsRowRelative, refPtg.IsRowRelative, ref3DPtg.IsColRelative, refPtg.IsColRelative, ref3DPtg.ExternSheetIndex);
		}
		return null;
	}

	private SheetIdentifier ParseSheetName()
	{
		string text;
		if (look == '[')
		{
			StringBuilder stringBuilder = new StringBuilder();
			GetChar();
			while (look != ']')
			{
				stringBuilder.Append(look);
				GetChar();
			}
			GetChar();
			text = stringBuilder.ToString();
		}
		else
		{
			text = null;
		}
		if (look == '\'')
		{
			StringBuilder stringBuilder2 = new StringBuilder();
			Match('\'');
			bool flag = look == '\'';
			while (!flag)
			{
				stringBuilder2.Append(look);
				GetChar();
				if (look == '\'')
				{
					Match('\'');
					flag = look != '\'';
				}
			}
			NameIdentifier nameIdentifier = new NameIdentifier(stringBuilder2.ToString(), isQuoted: true);
			SkipWhite();
			if (look == '!')
			{
				GetChar();
				return new SheetIdentifier(text, nameIdentifier);
			}
			if (look == ':')
			{
				return ParseSheetRange(text, nameIdentifier);
			}
			return null;
		}
		if (look == '_' || char.IsLetter(look))
		{
			StringBuilder stringBuilder3 = new StringBuilder();
			while (IsUnquotedSheetNameChar(look))
			{
				stringBuilder3.Append(look);
				GetChar();
			}
			NameIdentifier nameIdentifier2 = new NameIdentifier(stringBuilder3.ToString(), isQuoted: false);
			SkipWhite();
			if (look == '!')
			{
				GetChar();
				return new SheetIdentifier(text, nameIdentifier2);
			}
			if (look == ':')
			{
				return ParseSheetRange(text, nameIdentifier2);
			}
			return null;
		}
		if (look == '!' && text != null)
		{
			GetChar();
			return new SheetIdentifier(text, null);
		}
		return null;
	}

	private SheetIdentifier ParseSheetRange(string bookname, NameIdentifier sheet1Name)
	{
		GetChar();
		SheetIdentifier sheetIdentifier = ParseSheetName();
		if (sheetIdentifier != null)
		{
			return new SheetRangeIdentifier(bookname, sheet1Name, sheetIdentifier._sheetIdentifier);
		}
		return null;
	}

	private bool IsUnquotedSheetNameChar(char ch)
	{
		if (char.IsLetterOrDigit(ch))
		{
			return true;
		}
		if (ch == '.' || ch == '_')
		{
			return true;
		}
		return false;
	}

	private void ResetPointer(int ptr)
	{
		_pointer = ptr;
		if (_pointer <= _formulaLength)
		{
			look = _formulaString[_pointer - 1];
		}
		else
		{
			look = '\0';
		}
	}

	private bool IsValidCellReference(string str)
	{
		bool flag = CellReference.ClassifyCellReference(str, _ssVersion) == NameType.Cell;
		if (flag && FunctionMetadataRegistry.GetFunctionByName(str.ToUpper()) != null)
		{
			int pointer = _pointer;
			ResetPointer(_pointer + str.Length);
			SkipWhite();
			flag = look != '(';
			ResetPointer(pointer);
		}
		return flag;
	}

	private ParseNode Function(string name)
	{
		Ptg ptg = null;
		if (!AbstractFunctionPtg.IsBuiltInFunctionName(name))
		{
			if (_book == null)
			{
				throw new InvalidOperationException("Need book to evaluate name '" + name + "'");
			}
			IEvaluationName name2 = _book.GetName(name, _sheetIndex);
			if (name2 != null)
			{
				if (!name2.IsFunctionName)
				{
					throw new FormulaParseException("Attempt to use name '" + name + "' as a function, but defined name in workbook does not refer to a function");
				}
				ptg = name2.CreatePtg();
			}
			else
			{
				ptg = _book.GetNameXPtg(name, null);
				if (ptg == null)
				{
					string name3 = _book.GetSpreadsheetVersion().Name;
					if (!(name3 == "EXCEL97"))
					{
						if (!(name3 == "EXCEL2007"))
						{
							throw new Exception("Unexpected spreadsheet version: " + _book.GetSpreadsheetVersion().Name);
						}
						ptg = new NameXPxg(name);
					}
					else
					{
						AddName(name);
						name2 = _book.GetName(name, _sheetIndex);
						ptg = name2.CreatePtg();
					}
				}
			}
		}
		Match('(');
		ParseNode[] args = Arguments();
		Match(')');
		return GetFunction(name, ptg, args);
	}

	private void AddName(string functionName)
	{
		IName name = _book.CreateName();
		name.SetFunction(value: true);
		name.NameName = functionName;
		name.SheetIndex = _sheetIndex;
	}

	private ParseNode GetFunction(string name, Ptg namePtg, ParseNode[] args)
	{
		FunctionMetadata functionByName = FunctionMetadataRegistry.GetFunctionByName(name.ToUpper());
		int num = args.Length;
		if (functionByName == null)
		{
			if (namePtg == null)
			{
				throw new InvalidOperationException("NamePtg must be supplied for external Functions");
			}
			ParseNode[] array = new ParseNode[num + 1];
			array[0] = new ParseNode(namePtg);
			Array.Copy(args, 0, array, 1, num);
			return new ParseNode(FuncVarPtg.Create(name, (byte)(num + 1)), array);
		}
		if (namePtg != null)
		{
			throw new InvalidOperationException("NamePtg no applicable To internal Functions");
		}
		bool flag = !functionByName.HasFixedArgsLength;
		int index = functionByName.Index;
		if (index == 4 && args.Length == 1)
		{
			return new ParseNode(AttrPtg.GetSumSingle(), args);
		}
		ValidateNumArgs(args.Length, functionByName);
		AbstractFunctionPtg token = ((!flag) ? ((AbstractFunctionPtg)FuncPtg.Create(index)) : ((AbstractFunctionPtg)FuncVarPtg.Create(name, (byte)num)));
		return new ParseNode(token, args);
	}

	private void ValidateNumArgs(int numArgs, FunctionMetadata fm)
	{
		if (numArgs < fm.MinParams)
		{
			string text = "Too few arguments to function '" + fm.Name + "'. ";
			text = ((!fm.HasFixedArgsLength) ? (text + "At least " + fm.MinParams + " were expected") : (text + "Expected " + fm.MinParams));
			text = text + " but got " + numArgs + ".";
			throw new FormulaParseException(text);
		}
		int num = ((!fm.HasUnlimitedVarags) ? fm.MaxParams : ((_book == null) ? fm.MaxParams : _book.GetSpreadsheetVersion().MaxFunctionArgs));
		if (numArgs > num)
		{
			string text2 = "Too many arguments to function '" + fm.Name + "'. ";
			text2 = ((!fm.HasFixedArgsLength) ? (text2 + "At most " + fm.MaxParams + " were expected") : (text2 + "Expected " + fm.MaxParams));
			text2 = text2 + " but got " + numArgs + ".";
			throw new FormulaParseException(text2);
		}
	}

	private static bool IsArgumentDelimiter(char ch)
	{
		if (ch != ',')
		{
			return ch == ')';
		}
		return true;
	}

	private ParseNode[] Arguments()
	{
		ArrayList arrayList = new ArrayList(2);
		SkipWhite();
		if (look == ')')
		{
			return ParseNode.EMPTY_ARRAY;
		}
		bool flag = true;
		int num = 0;
		while (true)
		{
			SkipWhite();
			if (IsArgumentDelimiter(look))
			{
				if (flag)
				{
					arrayList.Add(new ParseNode(MissingArgPtg.instance));
					num++;
				}
				if (look == ')')
				{
					break;
				}
				Match(',');
				flag = true;
			}
			else
			{
				arrayList.Add(ComparisonExpression());
				num++;
				flag = false;
				SkipWhite();
				if (!IsArgumentDelimiter(look))
				{
					throw expected("',' or ')'");
				}
			}
		}
		return (ParseNode[])arrayList.ToArray(typeof(ParseNode));
	}

	private ParseNode PowerFactor()
	{
		ParseNode parseNode = PercentFactor();
		while (true)
		{
			SkipWhite();
			if (look != '^')
			{
				break;
			}
			Match('^');
			ParseNode child = PercentFactor();
			parseNode = new ParseNode(PowerPtg.instance, parseNode, child);
		}
		return parseNode;
	}

	private ParseNode PercentFactor()
	{
		ParseNode parseNode = ParseSimpleFactor();
		while (true)
		{
			SkipWhite();
			if (look != '%')
			{
				break;
			}
			Match('%');
			parseNode = new ParseNode(PercentPtg.instance, parseNode);
		}
		return parseNode;
	}

	private ParseNode ParseSimpleFactor()
	{
		SkipWhite();
		switch (look)
		{
		case '#':
			return new ParseNode(ErrPtg.ValueOf(ParseErrorLiteral()));
		case '-':
			Match('-');
			return ParseUnary(isPlus: false);
		case '+':
			Match('+');
			return ParseUnary(isPlus: true);
		case '(':
		{
			Match('(');
			ParseNode child = UnionExpression();
			Match(')');
			return new ParseNode(ParenthesisPtg.instance, child);
		}
		case '"':
			return new ParseNode(new StringPtg(ParseStringLiteral()));
		case '{':
		{
			Match('{');
			ParseNode result = ParseArray();
			Match('}');
			return result;
		}
		default:
			if (IsAlpha(look) || char.IsDigit(look) || look == '\'' || look == '[' || look == '_' || look == '\\')
			{
				return ParseRangeExpression();
			}
			if (look == '.')
			{
				return new ParseNode(ParseNumber());
			}
			throw expected("cell ref or constant literal");
		}
	}

	private ParseNode ParseUnary(bool isPlus)
	{
		bool num = IsDigit(look) || look == '.';
		ParseNode parseNode = PowerFactor();
		if (num)
		{
			Ptg token = parseNode.GetToken();
			if (token is NumberPtg)
			{
				if (isPlus)
				{
					return parseNode;
				}
				token = new NumberPtg(0.0 - ((NumberPtg)token).Value);
				return new ParseNode(token);
			}
			if (token is IntPtg)
			{
				if (isPlus)
				{
					return parseNode;
				}
				token = new NumberPtg(-((IntPtg)token).Value);
				return new ParseNode(token);
			}
		}
		return new ParseNode(isPlus ? UnaryPlusPtg.instance : UnaryMinusPtg.instance, parseNode);
	}

	private ParseNode ParseArray()
	{
		List<object[]> list = new List<object[]>();
		while (true)
		{
			object[] item = ParseArrayRow();
			list.Add(item);
			if (look == '}')
			{
				break;
			}
			if (look != ';')
			{
				throw expected("'}' or ';'");
			}
			Match(';');
		}
		object[][] array = new object[list.Count][];
		array = list.ToArray();
		int nColumns = array[0].Length;
		CheckRowLengths(array, nColumns);
		return new ParseNode(new ArrayPtg(array));
	}

	private void CheckRowLengths(object[][] values2d, int nColumns)
	{
		for (int i = 0; i < values2d.Length; i++)
		{
			int num = values2d[i].Length;
			if (num != nColumns)
			{
				throw new FormulaParseException("Array row " + i + " Has length " + num + " but row 0 Has length " + nColumns);
			}
		}
	}

	private object[] ParseArrayRow()
	{
		ArrayList arrayList = new ArrayList();
		while (true)
		{
			arrayList.Add(ParseArrayItem());
			SkipWhite();
			switch (look)
			{
			case ',':
				break;
			default:
				throw expected("'}' or ','");
			case ';':
			case '}':
				_ = new object[arrayList.Count];
				return arrayList.ToArray();
			}
			Match(',');
		}
	}

	private object ParseArrayItem()
	{
		SkipWhite();
		switch (look)
		{
		case '"':
			return ParseStringLiteral();
		case '#':
			return ErrorConstant.ValueOf(ParseErrorLiteral());
		case 'F':
		case 'T':
		case 'f':
		case 't':
			return ParseBooleanLiteral();
		case '-':
			Match('-');
			SkipWhite();
			return ConvertArrayNumber(ParseNumber(), isPositive: false);
		default:
			return ConvertArrayNumber(ParseNumber(), isPositive: true);
		}
	}

	private bool ParseBooleanLiteral()
	{
		string value = ParseUnquotedIdentifier();
		if ("TRUE".Equals(value, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if ("FALSE".Equals(value, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		throw expected("'TRUE' or 'FALSE'");
	}

	private static double ConvertArrayNumber(Ptg ptg, bool isPositive)
	{
		double num;
		if (ptg is IntPtg)
		{
			num = ((IntPtg)ptg).Value;
		}
		else
		{
			if (!(ptg is NumberPtg))
			{
				throw new Exception("Unexpected ptg (" + ptg.GetType().Name + ")");
			}
			num = ((NumberPtg)ptg).Value;
		}
		if (!isPositive)
		{
			num = 0.0 - num;
		}
		return num;
	}

	private Ptg ParseNumber()
	{
		string text = null;
		string exponent = null;
		string num = GetNum();
		if (look == '.')
		{
			GetChar();
			text = GetNum();
		}
		if (look == 'E')
		{
			GetChar();
			string text2 = "";
			if (look == '+')
			{
				GetChar();
			}
			else if (look == '-')
			{
				GetChar();
				text2 = "-";
			}
			string num2 = GetNum();
			if (num2 == null)
			{
				throw expected("int");
			}
			exponent = text2 + num2;
		}
		if (num == null && text == null)
		{
			throw expected("int");
		}
		return GetNumberPtgFromString(num, text, exponent);
	}

	private int ParseErrorLiteral()
	{
		Match('#');
		string text = ParseUnquotedIdentifier().ToUpper();
		switch (text[0])
		{
		case 'V':
		{
			FormulaError vALUE = FormulaError.VALUE;
			if (text.Equals(vALUE.Name))
			{
				Match('!');
				return vALUE.Code;
			}
			throw expected(vALUE.String);
		}
		case 'R':
		{
			FormulaError rEF = FormulaError.REF;
			if (text.Equals(rEF.Name))
			{
				Match('!');
				return rEF.Code;
			}
			throw expected(rEF.String);
		}
		case 'D':
		{
			FormulaError dIV = FormulaError.DIV0;
			if (text.Equals("DIV"))
			{
				Match('/');
				Match('0');
				Match('!');
				return dIV.Code;
			}
			throw expected(dIV.String);
		}
		case 'N':
		{
			FormulaError nAME = FormulaError.NAME;
			if (text.Equals(nAME.Name))
			{
				Match('?');
				return nAME.Code;
			}
			nAME = FormulaError.NUM;
			if (text.Equals(nAME.Name))
			{
				Match('!');
				return nAME.Code;
			}
			nAME = FormulaError.NULL;
			if (text.Equals(nAME.Name))
			{
				Match('!');
				return nAME.Code;
			}
			nAME = FormulaError.NA;
			if (text.Equals("N"))
			{
				Match('/');
				if (look != 'A' && look != 'a')
				{
					throw expected(nAME.String);
				}
				Match(look);
				return nAME.Code;
			}
			throw expected("#NAME?, #NUM!, #NULL! or #N/A");
		}
		default:
			throw expected("#VALUE!, #REF!, #DIV/0!, #NAME?, #NUM!, #NULL! or #N/A");
		}
	}

	private static Ptg GetNumberPtgFromString(string number1, string number2, string exponent)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (number2 == null)
		{
			stringBuilder.Append(number1);
			if (exponent != null)
			{
				stringBuilder.Append('E');
				stringBuilder.Append(exponent);
			}
			string text = stringBuilder.ToString();
			int num;
			try
			{
				num = int.Parse(text, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				return new NumberPtg(text);
			}
			catch (OverflowException)
			{
				return new NumberPtg(text);
			}
			if (IntPtg.IsInRange(num))
			{
				return new IntPtg(num);
			}
			return new NumberPtg(text);
		}
		if (number1 != null)
		{
			stringBuilder.Append(number1);
		}
		stringBuilder.Append('.');
		stringBuilder.Append(number2);
		if (exponent != null)
		{
			stringBuilder.Append('E');
			stringBuilder.Append(exponent);
		}
		return new NumberPtg(stringBuilder.ToString());
	}

	private string ParseStringLiteral()
	{
		Match('"');
		StringBuilder stringBuilder = new StringBuilder();
		while (true)
		{
			if (look == '"')
			{
				GetChar();
				if (look != '"')
				{
					break;
				}
			}
			stringBuilder.Append(look);
			GetChar();
		}
		return stringBuilder.ToString();
	}

	private ParseNode Term()
	{
		ParseNode parseNode = PowerFactor();
		while (true)
		{
			SkipWhite();
			Ptg instance;
			switch (look)
			{
			case '*':
				Match('*');
				instance = MultiplyPtg.instance;
				break;
			case '/':
				Match('/');
				instance = DividePtg.instance;
				break;
			default:
				return parseNode;
			}
			ParseNode child = PowerFactor();
			parseNode = new ParseNode(instance, parseNode, child);
		}
	}

	private ParseNode ComparisonExpression()
	{
		ParseNode parseNode = ConcatExpression();
		while (true)
		{
			SkipWhite();
			switch (look)
			{
			case '<':
			case '=':
			case '>':
				break;
			default:
				return parseNode;
			}
			Ptg comparisonToken = GetComparisonToken();
			ParseNode child = ConcatExpression();
			parseNode = new ParseNode(comparisonToken, parseNode, child);
		}
	}

	private Ptg GetComparisonToken()
	{
		if (look == '=')
		{
			Match(look);
			return EqualPtg.instance;
		}
		bool num = look == '>';
		Match(look);
		if (num)
		{
			if (look == '=')
			{
				Match('=');
				return GreaterEqualPtg.instance;
			}
			return GreaterThanPtg.instance;
		}
		switch (look)
		{
		case '=':
			Match('=');
			return LessEqualPtg.instance;
		case '>':
			Match('>');
			return NotEqualPtg.instance;
		default:
			return LessThanPtg.instance;
		}
	}

	private ParseNode ConcatExpression()
	{
		ParseNode parseNode = AdditiveExpression();
		while (true)
		{
			SkipWhite();
			if (look != '&')
			{
				break;
			}
			Match('&');
			ParseNode child = AdditiveExpression();
			parseNode = new ParseNode(ConcatPtg.instance, parseNode, child);
		}
		return parseNode;
	}

	private ParseNode AdditiveExpression()
	{
		ParseNode parseNode = Term();
		while (true)
		{
			SkipWhite();
			Ptg instance;
			switch (look)
			{
			case '+':
				Match('+');
				instance = AddPtg.instance;
				break;
			case '-':
				Match('-');
				instance = SubtractPtg.instance;
				break;
			default:
				return parseNode;
			}
			ParseNode child = Term();
			parseNode = new ParseNode(instance, parseNode, child);
		}
	}

	private void Parse()
	{
		_pointer = 0;
		GetChar();
		_rootNode = UnionExpression();
		if (_pointer <= _formulaLength)
		{
			throw new FormulaParseException("Unused input [" + _formulaString.Substring(_pointer - 1) + "] after attempting to parse the formula [" + _formulaString + "]");
		}
	}

	private ParseNode UnionExpression()
	{
		ParseNode parseNode = IntersectionExpression();
		bool flag = false;
		while (true)
		{
			SkipWhite();
			if (look != ',')
			{
				break;
			}
			GetChar();
			flag = true;
			ParseNode child = IntersectionExpression();
			parseNode = new ParseNode(UnionPtg.instance, parseNode, child);
		}
		if (flag)
		{
			return AugmentWithMemPtg(parseNode);
		}
		return parseNode;
	}

	private ParseNode IntersectionExpression()
	{
		ParseNode parseNode = ComparisonExpression();
		bool flag = false;
		while (true)
		{
			SkipWhite();
			if (!_inIntersection)
			{
				break;
			}
			int pointer = _pointer;
			try
			{
				ParseNode child = ComparisonExpression();
				parseNode = new ParseNode(IntersectionPtg.instance, parseNode, child);
				flag = true;
			}
			catch (FormulaParseException)
			{
				ResetPointer(pointer);
				break;
			}
		}
		if (flag)
		{
			return AugmentWithMemPtg(parseNode);
		}
		return parseNode;
	}

	private Ptg[] GetRPNPtg(FormulaType formulaType)
	{
		new OperandClassTransformer(formulaType).TransformFormula(_rootNode);
		return ParseNode.ToTokenArray(_rootNode);
	}
}
