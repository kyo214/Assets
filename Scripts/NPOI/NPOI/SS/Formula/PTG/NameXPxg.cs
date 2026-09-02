using System;
using System.Text;
using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

[Serializable]
public class NameXPxg : OperandPtg, Pxg
{
	private int externalWorkbookNumber = -1;

	private string sheetName;

	private string nameName;

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

	public string NameName => nameName;

	public override byte DefaultOperandClass => 32;

	public override int Size => 1;

	public NameXPxg(int externalWorkbookNumber, string sheetName, string nameName)
	{
		this.externalWorkbookNumber = externalWorkbookNumber;
		this.sheetName = sheetName;
		this.nameName = nameName;
	}

	public NameXPxg(string sheetName, string nameName)
		: this(-1, sheetName, nameName)
	{
	}

	public NameXPxg(string nameName)
		: this(-1, null, nameName)
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
		if (SheetName != null)
		{
			stringBuilder.Append("sheet=").Append(SheetName);
		}
		stringBuilder.Append(" ! ");
		stringBuilder.Append("name=");
		stringBuilder.Append(nameName);
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public override string ToFormulaString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		if (externalWorkbookNumber >= 0)
		{
			stringBuilder.Append('[');
			stringBuilder.Append(externalWorkbookNumber);
			stringBuilder.Append(']');
			flag = true;
		}
		if (sheetName != null)
		{
			SheetNameFormatter.AppendFormat(stringBuilder, sheetName);
			flag = true;
		}
		if (flag)
		{
			stringBuilder.Append('!');
		}
		stringBuilder.Append(nameName);
		return stringBuilder.ToString();
	}

	public override void Write(ILittleEndianOutput out1)
	{
		throw new InvalidOperationException("XSSF-only Ptg, should not be serialised");
	}
}
