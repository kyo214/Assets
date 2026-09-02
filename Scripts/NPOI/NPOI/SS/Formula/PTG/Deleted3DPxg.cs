using System;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

public class Deleted3DPxg : OperandPtg, Pxg
{
	private int externalWorkbookNumber = -1;

	private string sheetName;

	public int ExternalWorkbookNumber => externalWorkbookNumber;

	public string SheetName
	{
		get
		{
			return sheetName;
		}
		set
		{
			sheetName = value;
		}
	}

	public override byte DefaultOperandClass => 32;

	public override int Size => 1;

	public Deleted3DPxg(int externalWorkbookNumber, string sheetName)
	{
		this.externalWorkbookNumber = externalWorkbookNumber;
		this.sheetName = sheetName;
	}

	public Deleted3DPxg(string sheetName)
		: this(-1, sheetName)
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GetType().Name);
		stringBuilder.Append(" [");
		if (externalWorkbookNumber >= 0)
		{
			stringBuilder.Append(" [");
			stringBuilder.Append("workbook=").Append(ExternalWorkbookNumber);
			stringBuilder.Append("] ");
		}
		if (sheetName != null)
		{
			SheetNameFormatter.AppendFormat(stringBuilder, sheetName);
		}
		stringBuilder.Append(" ! ");
		stringBuilder.Append(FormulaError.REF.String);
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public override string ToFormulaString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (externalWorkbookNumber >= 0)
		{
			stringBuilder.Append('[');
			stringBuilder.Append(externalWorkbookNumber);
			stringBuilder.Append(']');
		}
		if (sheetName != null)
		{
			stringBuilder.Append(sheetName);
		}
		stringBuilder.Append('!');
		stringBuilder.Append(FormulaError.REF.String);
		return stringBuilder.ToString();
	}

	public override void Write(ILittleEndianOutput out1)
	{
		throw new InvalidOperationException("XSSF-only Ptg, should not be serialised");
	}
}
