using System;
using System.Text;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

public class Ref3DPxg : RefPtgBase, Pxg3D, Pxg
{
	private int externalWorkbookNumber = -1;

	private string firstSheetName;

	private string lastSheetName;

	public int ExternalWorkbookNumber => externalWorkbookNumber;

	public string SheetName
	{
		get
		{
			return firstSheetName;
		}
		set
		{
			firstSheetName = value;
		}
	}

	public string LastSheetName
	{
		get
		{
			return lastSheetName;
		}
		set
		{
			lastSheetName = value;
		}
	}

	public override int Size => 1;

	public Ref3DPxg(int externalWorkbookNumber, SheetIdentifier sheetName, string cellref)
		: this(externalWorkbookNumber, sheetName, new CellReference(cellref))
	{
	}

	public Ref3DPxg(int externalWorkbookNumber, SheetIdentifier sheetName, CellReference c)
		: base(c)
	{
		this.externalWorkbookNumber = externalWorkbookNumber;
		firstSheetName = sheetName.SheetId.Name;
		if (sheetName is SheetRangeIdentifier)
		{
			lastSheetName = ((SheetRangeIdentifier)sheetName).LastSheetIdentifier.Name;
		}
		else
		{
			lastSheetName = null;
		}
	}

	public Ref3DPxg(SheetIdentifier sheetName, string cellref)
		: this(sheetName, new CellReference(cellref))
	{
	}

	public Ref3DPxg(SheetIdentifier sheetName, CellReference c)
		: this(-1, sheetName, c)
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
		stringBuilder.Append("sheet=").Append(firstSheetName);
		if (lastSheetName != null)
		{
			stringBuilder.Append(" : ");
			stringBuilder.Append("sheet=").Append(lastSheetName);
		}
		stringBuilder.Append(" ! ");
		stringBuilder.Append(FormatReferenceAsString());
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public string Format2DRefAsString()
	{
		return FormatReferenceAsString();
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
		if (firstSheetName != null)
		{
			SheetNameFormatter.AppendFormat(stringBuilder, firstSheetName);
		}
		if (lastSheetName != null)
		{
			stringBuilder.Append(':');
			SheetNameFormatter.AppendFormat(stringBuilder, lastSheetName);
		}
		stringBuilder.Append('!');
		stringBuilder.Append(FormatReferenceAsString());
		return stringBuilder.ToString();
	}

	public override void Write(ILittleEndianOutput out1)
	{
		throw new InvalidOperationException("XSSF-only Ptg, should not be serialised");
	}
}
