using System;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.Record.Aggregates;

[Serializable]
public class FormulaRecordAggregate : RecordAggregate, CellValueRecordInterface, IComparable, ICloneable
{
	public const short sid = -2000;

	private FormulaRecord _formulaRecord;

	private SharedValueManager _sharedValueManager;

	private StringRecord _stringRecord;

	[NonSerialized]
	private SharedFormulaRecord _sharedFormulaRecord;

	public bool IsPartOfArrayFormula
	{
		get
		{
			if (_sharedFormulaRecord != null)
			{
				return false;
			}
			CellReference expReference = _formulaRecord.Formula.ExpReference;
			return ((expReference == null) ? null : _sharedValueManager.GetArrayRecord(expReference.Row, expReference.Col)) != null;
		}
	}

	public override int RecordSize => _formulaRecord.RecordSize + ((_stringRecord != null) ? _stringRecord.RecordSize : 0);

	public override short Sid => -2000;

	public FormulaRecord FormulaRecord
	{
		get
		{
			return _formulaRecord;
		}
		set
		{
			_formulaRecord = value;
		}
	}

	public StringRecord StringRecord
	{
		get
		{
			return _stringRecord;
		}
		set
		{
			_stringRecord = value;
		}
	}

	public short XFIndex
	{
		get
		{
			return _formulaRecord.XFIndex;
		}
		set
		{
			_formulaRecord.XFIndex = value;
		}
	}

	public int Column
	{
		get
		{
			return _formulaRecord.Column;
		}
		set
		{
			_formulaRecord.Column = value;
		}
	}

	public int Row
	{
		get
		{
			return _formulaRecord.Row;
		}
		set
		{
			_formulaRecord.Row = value;
		}
	}

	public string StringValue
	{
		get
		{
			if (_stringRecord == null)
			{
				return null;
			}
			return _stringRecord.String;
		}
	}

	public Ptg[] FormulaTokens
	{
		get
		{
			if (_sharedFormulaRecord != null)
			{
				return _sharedFormulaRecord.GetFormulaTokens(_formulaRecord);
			}
			CellReference expReference = _formulaRecord.Formula.ExpReference;
			if (expReference != null)
			{
				return _sharedValueManager.GetArrayRecord(expReference.Row, expReference.Col).FormulaTokens;
			}
			return _formulaRecord.ParsedExpression;
		}
	}

	public FormulaRecordAggregate(FormulaRecord formulaRec, StringRecord stringRec, SharedValueManager svm)
	{
		if (svm == null)
		{
			throw new ArgumentException("sfm must not be null");
		}
		if (formulaRec.HasCachedResultString)
		{
			if (stringRec == null)
			{
				throw new RecordFormatException("Formula record flag is set but String record was not found");
			}
			_stringRecord = stringRec;
		}
		else
		{
			_stringRecord = null;
		}
		_formulaRecord = formulaRec;
		_sharedValueManager = svm;
		if (formulaRec.IsSharedFormula)
		{
			CellReference expReference = formulaRec.Formula.ExpReference;
			if (expReference == null)
			{
				HandleMissingSharedFormulaRecord(formulaRec);
			}
			else
			{
				_sharedFormulaRecord = svm.LinkSharedFormulaRecord(expReference, this);
			}
		}
	}

	public void NotifyFormulaChanging()
	{
		if (_sharedFormulaRecord != null)
		{
			_sharedValueManager.Unlink(_sharedFormulaRecord);
		}
	}

	public override int Serialize(int offset, byte[] data)
	{
		int num = offset;
		num += _formulaRecord.Serialize(num, data);
		if (_stringRecord != null)
		{
			num += _stringRecord.Serialize(num, data);
		}
		return num - offset;
	}

	public override void VisitContainedRecords(RecordVisitor rv)
	{
		rv.VisitRecord(_formulaRecord);
		Record recordForFirstCell = _sharedValueManager.GetRecordForFirstCell(this);
		if (recordForFirstCell != null)
		{
			rv.VisitRecord(recordForFirstCell);
		}
		if (_formulaRecord.HasCachedResultString && _stringRecord != null)
		{
			rv.VisitRecord(_stringRecord);
		}
	}

	private static void HandleMissingSharedFormulaRecord(FormulaRecord formula)
	{
		if (formula.ParsedExpression[0] is ExpPtg)
		{
			throw new RecordFormatException("SharedFormulaRecord not found for FormulaRecord with (isSharedFormula=true)");
		}
		formula.IsSharedFormula = false;
	}

	public int CompareTo(object o)
	{
		return _formulaRecord.CompareTo(o);
	}

	public override bool Equals(object obj)
	{
		return _formulaRecord.Equals(obj);
	}

	public override int GetHashCode()
	{
		return _formulaRecord.GetHashCode();
	}

	public override string ToString()
	{
		return _formulaRecord.ToString();
	}

	public void SetCachedDoubleResult(double value)
	{
		_stringRecord = null;
		_formulaRecord.Value = value;
	}

	public void SetCachedStringResult(string value)
	{
		if (_stringRecord == null)
		{
			_stringRecord = new StringRecord();
		}
		_stringRecord.String = value;
		if (value.Length < 1)
		{
			_formulaRecord.SetCachedResultTypeEmptyString();
		}
		else
		{
			_formulaRecord.SetCachedResultTypeString();
		}
	}

	public void SetCachedBooleanResult(bool value)
	{
		_stringRecord = null;
		_formulaRecord.SetCachedResultBoolean(value);
	}

	public void SetCachedErrorResult(int errorCode)
	{
		_stringRecord = null;
		_formulaRecord.SetCachedResultErrorCode(errorCode);
	}

	public void SetCachedErrorResult(FormulaError error)
	{
		SetCachedErrorResult(error.Code);
	}

	public object Clone()
	{
		return this;
	}

	public void SetParsedExpression(Ptg[] ptgs)
	{
		NotifyFormulaChanging();
		_formulaRecord.ParsedExpression = ptgs;
	}

	public void UnlinkSharedFormula()
	{
		Ptg[] formulaTokens = (_sharedFormulaRecord ?? throw new InvalidOperationException("Formula not linked to shared formula")).GetFormulaTokens(_formulaRecord);
		_formulaRecord.SetParsedExpression(formulaTokens);
		_formulaRecord.SetSharedFormula(flag: false);
		_sharedFormulaRecord = null;
	}

	public CellRangeAddress GetArrayFormulaRange()
	{
		if (_sharedFormulaRecord != null)
		{
			throw new InvalidOperationException("not an array formula cell.");
		}
		CellReference expReference = _formulaRecord.Formula.ExpReference;
		if (expReference == null)
		{
			throw new InvalidOperationException("not an array formula cell.");
		}
		CellRangeAddress8Bit range = (_sharedValueManager.GetArrayRecord(expReference.Row, expReference.Col) ?? throw new InvalidOperationException("ArrayRecord was not found for the locator " + expReference.FormatAsString())).Range;
		return new CellRangeAddress(range.FirstRow, range.LastRow, range.FirstColumn, range.LastColumn);
	}

	public void SetArrayFormula(CellRangeAddress r, Ptg[] ptgs)
	{
		ArrayRecord ar = new ArrayRecord(Formula.Create(ptgs), new CellRangeAddress8Bit(r.FirstRow, r.LastRow, r.FirstColumn, r.LastColumn));
		_sharedValueManager.AddArrayRecord(ar);
	}

	public CellRangeAddress RemoveArrayFormula(int rowIndex, int columnIndex)
	{
		CellRangeAddress8Bit cellRangeAddress8Bit = _sharedValueManager.RemoveArrayFormula(rowIndex, columnIndex);
		_formulaRecord.ParsedExpression = null;
		return new CellRangeAddress(cellRangeAddress8Bit.FirstRow, cellRangeAddress8Bit.LastRow, cellRangeAddress8Bit.FirstColumn, cellRangeAddress8Bit.LastColumn);
	}
}
