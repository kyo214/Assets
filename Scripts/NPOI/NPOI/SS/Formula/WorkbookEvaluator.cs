#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using NPOI.SS.Formula.Atp;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.PTG;
using NPOI.SS.Formula.UDF;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.SS.Formula;

public class WorkbookEvaluator
{
	private IEvaluationWorkbook _workbook;

	private EvaluationCache _cache;

	private int _workbookIx;

	private IEvaluationListener _evaluationListener;

	private Dictionary<IEvaluationSheet, int> _sheetIndexesBySheet;

	private Dictionary<string, int> _sheetIndexesByName;

	private CollaboratingWorkbooksEnvironment _collaboratingWorkbookEnvironment;

	private IStabilityClassifier _stabilityClassifier;

	private UDFFinder _udfFinder;

	private bool _ignoreMissingWorkbooks;

	private bool dbgEvaluationOutputForNextEval;

	private POILogger EVAL_LOG = POILogFactory.GetLogger("POI.FormulaEval");

	private int dbgEvaluationOutputIndent = -1;

	internal IEvaluationWorkbook Workbook => _workbook;

	public bool IgnoreMissingWorkbooks
	{
		get
		{
			return _ignoreMissingWorkbooks;
		}
		set
		{
			_ignoreMissingWorkbooks = value;
		}
	}

	public bool DebugEvaluationOutputForNextEval
	{
		get
		{
			return dbgEvaluationOutputForNextEval;
		}
		set
		{
			dbgEvaluationOutputForNextEval = value;
		}
	}

	public WorkbookEvaluator(IEvaluationWorkbook workbook, IStabilityClassifier stabilityClassifier, UDFFinder udfFinder)
		: this(workbook, null, stabilityClassifier, udfFinder)
	{
	}

	public WorkbookEvaluator(IEvaluationWorkbook workbook, IEvaluationListener evaluationListener, IStabilityClassifier stabilityClassifier, UDFFinder udfFinder)
	{
		_workbook = workbook;
		_evaluationListener = evaluationListener;
		_cache = new EvaluationCache(evaluationListener);
		_sheetIndexesBySheet = new Dictionary<IEvaluationSheet, int>();
		_sheetIndexesByName = new Dictionary<string, int>();
		_collaboratingWorkbookEnvironment = CollaboratingWorkbooksEnvironment.EMPTY;
		_workbookIx = 0;
		_stabilityClassifier = stabilityClassifier;
		AggregatingUDFFinder aggregatingUDFFinder = ((workbook == null) ? null : ((AggregatingUDFFinder)workbook.GetUDFFinder()));
		if (aggregatingUDFFinder != null && udfFinder != null)
		{
			aggregatingUDFFinder.Add(udfFinder);
		}
		_udfFinder = aggregatingUDFFinder;
	}

	public string GetSheetName(int sheetIndex)
	{
		return _workbook.GetSheetName(sheetIndex);
	}

	public WorkbookEvaluator GetOtherWorkbookEvaluator(string workbookName)
	{
		return _collaboratingWorkbookEnvironment.GetWorkbookEvaluator(workbookName);
	}

	internal IEvaluationSheet GetSheet(int sheetIndex)
	{
		return _workbook.GetSheet(sheetIndex);
	}

	internal IEvaluationName GetName(string name, int sheetIndex)
	{
		return _workbook.GetName(name, sheetIndex);
	}

	private static bool IsDebugLogEnabled()
	{
		return false;
	}

	private static bool IsInfoLogEnabled()
	{
		return true;
	}

	private static void LogDebug(string s)
	{
		IsDebugLogEnabled();
	}

	private static void LogInfo(string s)
	{
		if (IsInfoLogEnabled())
		{
			Trace.WriteLine(s);
		}
	}

	public void AttachToEnvironment(CollaboratingWorkbooksEnvironment collaboratingWorkbooksEnvironment, EvaluationCache cache, int workbookIx)
	{
		_collaboratingWorkbookEnvironment = collaboratingWorkbooksEnvironment;
		_cache = cache;
		_workbookIx = workbookIx;
	}

	public CollaboratingWorkbooksEnvironment GetEnvironment()
	{
		return _collaboratingWorkbookEnvironment;
	}

	public void DetachFromEnvironment()
	{
		_collaboratingWorkbookEnvironment = CollaboratingWorkbooksEnvironment.EMPTY;
		_cache = new EvaluationCache(_evaluationListener);
		_workbookIx = 0;
	}

	public IEvaluationListener GetEvaluationListener()
	{
		return _evaluationListener;
	}

	public void ClearAllCachedResultValues()
	{
		_cache.Clear();
		_sheetIndexesBySheet.Clear();
		_workbook.ClearAllCachedResultValues();
	}

	public void NotifyUpdateCell(IEvaluationCell cell)
	{
		int sheetIndex = GetSheetIndex(cell.Sheet);
		_cache.NotifyUpdateCell(_workbookIx, sheetIndex, cell);
	}

	public void NotifyDeleteCell(IEvaluationCell cell)
	{
		int sheetIndex = GetSheetIndex(cell.Sheet);
		_cache.NotifyDeleteCell(_workbookIx, sheetIndex, cell);
	}

	public int GetSheetIndex(IEvaluationSheet sheet)
	{
		int num = int.MinValue;
		if (_sheetIndexesBySheet.ContainsKey(sheet))
		{
			num = _sheetIndexesBySheet[sheet];
		}
		if (num == int.MinValue)
		{
			int sheetIndex = _workbook.GetSheetIndex(sheet);
			if (sheetIndex < 0)
			{
				throw new Exception("Specified sheet from a different book");
			}
			num = sheetIndex;
			_sheetIndexesBySheet[sheet] = num;
		}
		return num;
	}

	internal int GetSheetIndexByExternIndex(int externSheetIndex)
	{
		return _workbook.ConvertFromExternSheetIndex(externSheetIndex);
	}

	public int GetSheetIndex(string sheetName)
	{
		int num;
		if (_sheetIndexesByName.ContainsKey(sheetName))
		{
			num = _sheetIndexesByName[sheetName];
		}
		else
		{
			int sheetIndex = _workbook.GetSheetIndex(sheetName);
			if (sheetIndex < 0)
			{
				return -1;
			}
			num = sheetIndex;
			_sheetIndexesByName[sheetName] = num;
		}
		return num;
	}

	public ValueEval Evaluate(IEvaluationCell srcCell)
	{
		int sheetIndex = GetSheetIndex(srcCell.Sheet);
		return EvaluateAny(srcCell, sheetIndex, srcCell.RowIndex, srcCell.ColumnIndex, new EvaluationTracker(_cache));
	}

	private ValueEval EvaluateAny(IEvaluationCell srcCell, int sheetIndex, int rowIndex, int columnIndex, EvaluationTracker tracker)
	{
		bool flag = _stabilityClassifier == null || !_stabilityClassifier.IsCellFinal(sheetIndex, rowIndex, columnIndex);
		if (srcCell == null || srcCell.CellType != CellType.Formula)
		{
			ValueEval valueFromNonFormulaCell = GetValueFromNonFormulaCell(srcCell);
			if (flag)
			{
				tracker.AcceptPlainValueDependency(_workbookIx, sheetIndex, rowIndex, columnIndex, valueFromNonFormulaCell);
			}
			return valueFromNonFormulaCell;
		}
		FormulaCellCacheEntry orCreateFormulaCellEntry = _cache.GetOrCreateFormulaCellEntry(srcCell);
		if (flag || orCreateFormulaCellEntry.IsInputSensitive)
		{
			tracker.AcceptFormulaDependency(orCreateFormulaCellEntry);
		}
		IEvaluationListener evaluationListener = _evaluationListener;
		if (orCreateFormulaCellEntry.GetValue() == null)
		{
			if (!tracker.StartEvaluate(orCreateFormulaCellEntry))
			{
				return ErrorEval.CIRCULAR_REF_ERROR;
			}
			OperationEvaluationContext ec = new OperationEvaluationContext(this, _workbook, sheetIndex, rowIndex, columnIndex, tracker);
			ValueEval valueFromNonFormulaCell;
			try
			{
				Ptg[] formulaTokens = _workbook.GetFormulaTokens(srcCell);
				if (evaluationListener == null)
				{
					valueFromNonFormulaCell = EvaluateFormula(ec, formulaTokens);
				}
				else
				{
					evaluationListener.OnStartEvaluate(srcCell, orCreateFormulaCellEntry);
					valueFromNonFormulaCell = EvaluateFormula(ec, formulaTokens);
					evaluationListener.OnEndEvaluate(orCreateFormulaCellEntry, valueFromNonFormulaCell);
				}
				tracker.UpdateCacheResult(valueFromNonFormulaCell);
			}
			catch (NotImplementedException inner)
			{
				throw AddExceptionInfo(inner, sheetIndex, rowIndex, columnIndex);
			}
			catch (RuntimeException ex)
			{
				if (!(ex.InnerException is WorkbookNotFoundException) || !_ignoreMissingWorkbooks)
				{
					throw ex;
				}
				LogInfo(ex.InnerException.Message + " - Continuing with cached value!");
				valueFromNonFormulaCell = srcCell.CachedFormulaResultType switch
				{
					CellType.Numeric => new NumberEval(srcCell.NumericCellValue), 
					CellType.String => new StringEval(srcCell.StringCellValue), 
					CellType.Blank => BlankEval.instance, 
					CellType.Boolean => BoolEval.ValueOf(srcCell.BooleanCellValue), 
					CellType.Error => ErrorEval.ValueOf(srcCell.ErrorCellValue), 
					_ => throw new RuntimeException("Unexpected cell type '" + srcCell.CellType.ToString() + "' found!"), 
				};
			}
			finally
			{
				tracker.EndEvaluate(orCreateFormulaCellEntry);
			}
			if (IsDebugLogEnabled())
			{
				string sheetName = GetSheetName(sheetIndex);
				CellReference cellReference = new CellReference(rowIndex, columnIndex);
				LogDebug("Evaluated " + sheetName + "!" + cellReference.FormatAsString() + " To " + orCreateFormulaCellEntry.GetValue());
			}
			return valueFromNonFormulaCell;
		}
		evaluationListener?.OnCacheHit(sheetIndex, rowIndex, columnIndex, orCreateFormulaCellEntry.GetValue());
		return orCreateFormulaCellEntry.GetValue();
	}

	private NotImplementedException AddExceptionInfo(NotImplementedException inner, int sheetIndex, int rowIndex, int columnIndex)
	{
		try
		{
			CellReference cellReference = new CellReference(_workbook.GetSheetName(sheetIndex), rowIndex, columnIndex, pAbsRow: false, pAbsCol: false);
			return new NotImplementedException("Error evaluating cell " + cellReference.FormatAsString(), inner);
		}
		catch (Exception)
		{
			return inner;
		}
	}

	internal static ValueEval GetValueFromNonFormulaCell(IEvaluationCell cell)
	{
		if (cell == null)
		{
			return BlankEval.instance;
		}
		CellType cellType = cell.CellType;
		return cellType switch
		{
			CellType.Numeric => new NumberEval(cell.NumericCellValue), 
			CellType.String => new StringEval(cell.StringCellValue), 
			CellType.Boolean => BoolEval.ValueOf(cell.BooleanCellValue), 
			CellType.Blank => BlankEval.instance, 
			CellType.Error => ErrorEval.ValueOf(cell.ErrorCellValue), 
			_ => throw new Exception("Unexpected cell type (" + cellType.ToString() + ")"), 
		};
	}

	public ValueEval EvaluateFormula(OperationEvaluationContext ec, Ptg[] ptgs)
	{
		string text = "";
		if (dbgEvaluationOutputForNextEval)
		{
			dbgEvaluationOutputIndent = 1;
			dbgEvaluationOutputForNextEval = false;
		}
		if (dbgEvaluationOutputIndent > 0)
		{
			text = "                                                                                                    ";
			text = text.Substring(0, Math.Min(text.Length, dbgEvaluationOutputIndent * 2));
			POILogger eVAL_LOG = EVAL_LOG;
			string[] obj = new string[7]
			{
				text,
				"- evaluateFormula('",
				ec.GetRefEvaluatorForCurrentSheet().SheetNameRange,
				"'/",
				new CellReference(ec.RowIndex, ec.ColumnIndex).FormatAsString(),
				"): ",
				null
			};
			obj[6] = Arrays.ToString(ptgs).Replace("\\Qorg.apache.poi.ss.formula.ptg.\\E", "");
			eVAL_LOG.Log(5, string.Concat(obj));
			dbgEvaluationOutputIndent++;
		}
		Stack<ValueEval> stack = new Stack<ValueEval>();
		int i = 0;
		for (int num = ptgs.Length; i < num; i++)
		{
			Ptg ptg = ptgs[i];
			if (dbgEvaluationOutputIndent > 0)
			{
				EVAL_LOG.Log(3, text + "  * ptg " + i + ": " + ptg.ToString());
			}
			if (ptg is AttrPtg)
			{
				AttrPtg attrPtg = (AttrPtg)ptg;
				if (attrPtg.IsSum)
				{
					ptg = FuncVarPtg.SUM;
				}
				if (attrPtg.IsOptimizedChoose)
				{
					ValueEval arg = stack.Pop();
					int[] jumpTable = attrPtg.JumpTable;
					int num2 = jumpTable.Length;
					int num4;
					try
					{
						int num3 = Choose.EvaluateFirstArg(arg, ec.RowIndex, ec.ColumnIndex);
						if (num3 < 1 || num3 > num2)
						{
							stack.Push(ErrorEval.VALUE_INVALID);
							num4 = attrPtg.ChooseFuncOffset + 4;
						}
						else
						{
							num4 = jumpTable[num3 - 1];
						}
					}
					catch (EvaluationException ex)
					{
						stack.Push(ex.GetErrorEval());
						num4 = attrPtg.ChooseFuncOffset + 4;
					}
					num4 -= num2 * 2 + 2;
					i += CountTokensToBeSkipped(ptgs, i, num4);
					continue;
				}
				if (attrPtg.IsOptimizedIf)
				{
					ValueEval arg2 = stack.Pop();
					bool flag;
					try
					{
						flag = IfFunc.EvaluateFirstArg(arg2, ec.RowIndex, ec.ColumnIndex);
					}
					catch (EvaluationException ex2)
					{
						stack.Push(ex2.GetErrorEval());
						int data = attrPtg.Data;
						i += CountTokensToBeSkipped(ptgs, i, data);
						attrPtg = (AttrPtg)ptgs[i];
						data = attrPtg.Data + 1;
						i += CountTokensToBeSkipped(ptgs, i, data);
						continue;
					}
					if (!flag)
					{
						int data2 = attrPtg.Data;
						i += CountTokensToBeSkipped(ptgs, i, data2);
						Ptg ptg2 = ptgs[i + 1];
						if (ptgs[i] is AttrPtg && ptg2 is FuncVarPtg && ((FuncVarPtg)ptg2).FunctionIndex == 1)
						{
							i++;
							stack.Push(BoolEval.FALSE);
						}
					}
					continue;
				}
				if (attrPtg.IsSkip)
				{
					int distInBytes = attrPtg.Data + 1;
					i += CountTokensToBeSkipped(ptgs, i, distInBytes);
					if (stack.Peek() == MissingArgEval.instance)
					{
						stack.Pop();
						stack.Push(BlankEval.instance);
					}
					continue;
				}
			}
			if (ptg is ControlPtg || ptg is MemFuncPtg || ptg is MemAreaPtg || ptg is MemErrPtg)
			{
				continue;
			}
			ValueEval valueEval2;
			if (ptg is OperationPtg)
			{
				OperationPtg operationPtg = (OperationPtg)ptg;
				if (operationPtg is UnionPtg)
				{
					continue;
				}
				int numberOfOperands = operationPtg.NumberOfOperands;
				ValueEval[] array = new ValueEval[numberOfOperands];
				for (int num5 = numberOfOperands - 1; num5 >= 0; num5--)
				{
					ValueEval valueEval = stack.Pop();
					array[num5] = valueEval;
				}
				valueEval2 = OperationEvaluatorFactory.Evaluate(operationPtg, array, ec);
			}
			else
			{
				valueEval2 = GetEvalForPtg(ptg, ec);
			}
			if (valueEval2 == null)
			{
				throw new Exception("Evaluation result must not be null");
			}
			stack.Push(valueEval2);
			if (dbgEvaluationOutputIndent > 0)
			{
				EVAL_LOG.Log(3, text + "    = " + valueEval2.ToString());
			}
		}
		ValueEval evaluationResult = stack.Pop();
		if (stack.Count != 0)
		{
			throw new InvalidOperationException("evaluation stack not empty");
		}
		ValueEval valueEval3 = DereferenceResult(evaluationResult, ec.RowIndex, ec.ColumnIndex);
		if (dbgEvaluationOutputIndent > 0)
		{
			EVAL_LOG.Log(3, text + "finshed eval of " + new CellReference(ec.RowIndex, ec.ColumnIndex).FormatAsString() + ": " + valueEval3.ToString());
			dbgEvaluationOutputIndent--;
			if (dbgEvaluationOutputIndent == 1)
			{
				dbgEvaluationOutputIndent = -1;
			}
		}
		return valueEval3;
	}

	private static int CountTokensToBeSkipped(Ptg[] ptgs, int startIndex, int distInBytes)
	{
		int num = distInBytes;
		int num2 = startIndex;
		while (num != 0)
		{
			num2++;
			num -= ptgs[num2].Size;
			if (num < 0)
			{
				throw new Exception("Bad skip distance (wrong token size calculation).");
			}
			if (num2 >= ptgs.Length)
			{
				throw new Exception("Skip distance too far (ran out of formula tokens).");
			}
		}
		return num2 - startIndex;
	}

	public static ValueEval DereferenceResult(ValueEval evaluationResult, int srcRowNum, int srcColNum)
	{
		ValueEval singleValue;
		try
		{
			singleValue = OperandResolver.GetSingleValue(evaluationResult, srcRowNum, srcColNum);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		if (singleValue == BlankEval.instance)
		{
			return NumberEval.ZERO;
		}
		return singleValue;
	}

	private ValueEval GetEvalForPtg(Ptg ptg, OperationEvaluationContext ec)
	{
		if (ptg is NamePtg)
		{
			NamePtg namePtg = (NamePtg)ptg;
			IEvaluationName name = _workbook.GetName(namePtg);
			return GetEvalForNameRecord(name, ec);
		}
		if (ptg is NameXPtg)
		{
			return ProcessNameEval(ec.GetNameXEval((NameXPtg)ptg), ec);
		}
		if (ptg is NameXPxg)
		{
			return ProcessNameEval(ec.GetNameXEval((NameXPxg)ptg), ec);
		}
		if (ptg is IntPtg)
		{
			return new NumberEval(((IntPtg)ptg).Value);
		}
		if (ptg is NumberPtg)
		{
			return new NumberEval(((NumberPtg)ptg).Value);
		}
		if (ptg is StringPtg)
		{
			return new StringEval(((StringPtg)ptg).Value);
		}
		if (ptg is BoolPtg)
		{
			return BoolEval.ValueOf(((BoolPtg)ptg).Value);
		}
		if (ptg is ErrPtg)
		{
			return ErrorEval.ValueOf(((ErrPtg)ptg).ErrorCode);
		}
		if (ptg is MissingArgPtg)
		{
			return MissingArgEval.instance;
		}
		if (ptg is AreaErrPtg || ptg is RefErrorPtg || ptg is DeletedArea3DPtg || ptg is DeletedRef3DPtg)
		{
			return ErrorEval.REF_INVALID;
		}
		if (ptg is Ref3DPtg)
		{
			return ec.GetRef3DEval((Ref3DPtg)ptg);
		}
		if (ptg is Ref3DPxg)
		{
			return ec.GetRef3DEval((Ref3DPxg)ptg);
		}
		if (ptg is Area3DPtg)
		{
			return ec.GetArea3DEval((Area3DPtg)ptg);
		}
		if (ptg is Area3DPxg)
		{
			return ec.GetArea3DEval((Area3DPxg)ptg);
		}
		if (ptg is RefPtg)
		{
			RefPtg refPtg = (RefPtg)ptg;
			return ec.GetRefEval(refPtg.Row, refPtg.Column);
		}
		if (ptg is AreaPtg)
		{
			AreaPtg areaPtg = (AreaPtg)ptg;
			return ec.GetAreaEval(areaPtg.FirstRow, areaPtg.FirstColumn, areaPtg.LastRow, areaPtg.LastColumn);
		}
		if (ptg is UnknownPtg)
		{
			throw new RuntimeException("UnknownPtg not allowed");
		}
		if (ptg is ExpPtg)
		{
			throw new RuntimeException("ExpPtg currently not supported");
		}
		throw new RuntimeException("Unexpected ptg class (" + ptg.GetType().Name + ")");
	}

	private ValueEval ProcessNameEval(ValueEval eval, OperationEvaluationContext ec)
	{
		if (eval is ExternalNameEval)
		{
			IEvaluationName name = ((ExternalNameEval)eval).Name;
			return GetEvalForNameRecord(name, ec);
		}
		return eval;
	}

	private ValueEval GetEvalForNameRecord(IEvaluationName nameRecord, OperationEvaluationContext ec)
	{
		if (nameRecord.IsFunctionName)
		{
			return new FunctionNameEval(nameRecord.NameText);
		}
		if (nameRecord.HasFormula)
		{
			return EvaluateNameFormula(nameRecord.NameDefinition, ec);
		}
		throw new Exception("Don't now how to Evalate name '" + nameRecord.NameText + "'");
	}

	internal ValueEval EvaluateNameFormula(Ptg[] ptgs, OperationEvaluationContext ec)
	{
		if (ptgs.Length == 1)
		{
			return GetEvalForPtg(ptgs[0], ec);
		}
		return EvaluateFormula(ec, ptgs);
	}

	public ValueEval EvaluateReference(IEvaluationSheet sheet, int sheetIndex, int rowIndex, int columnIndex, EvaluationTracker tracker)
	{
		IEvaluationCell cell = sheet.GetCell(rowIndex, columnIndex);
		return EvaluateAny(cell, sheetIndex, rowIndex, columnIndex, tracker);
	}

	public FreeRefFunction FindUserDefinedFunction(string functionName)
	{
		return _udfFinder.FindFunction(functionName);
	}

	public static IList<string> GetSupportedFunctionNames()
	{
		List<string> list = new List<string>();
		list.AddRange(FunctionEval.GetSupportedFunctionNames());
		list.AddRange(AnalysisToolPak.GetSupportedFunctionNames());
		return list.AsReadOnly();
	}

	public static IList<string> GetNotSupportedFunctionNames()
	{
		List<string> list = new List<string>();
		list.AddRange(FunctionEval.GetNotSupportedFunctionNames());
		list.AddRange(AnalysisToolPak.GetNotSupportedFunctionNames());
		return list.AsReadOnly();
	}

	public static void RegisterFunction(string name, FreeRefFunction func)
	{
		AnalysisToolPak.RegisterFunction(name, func);
	}

	public static void RegisterFunction(string name, NPOI.SS.Formula.Functions.Function func)
	{
		FunctionEval.RegisterFunction(name, func);
	}
}
