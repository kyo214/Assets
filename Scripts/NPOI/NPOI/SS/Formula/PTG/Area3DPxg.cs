using System;
using System.Text;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

public class Area3DPxg : AreaPtgBase, Pxg3D, Pxg
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

	public Area3DPxg(int externalWorkbookNumber, SheetIdentifier sheetName, string arearef)
		: this(externalWorkbookNumber, sheetName, new AreaReference(arearef))
	{
	}

	public Area3DPxg(int externalWorkbookNumber, SheetIdentifier sheetName, AreaReference arearef)
		: base(arearef)
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

	public Area3DPxg(SheetIdentifier sheetName, string arearef)
		: this(sheetName, new AreaReference(arearef))
	{
	}

	public Area3DPxg(SheetIdentifier sheetName, AreaReference arearef)
		: this(-1, sheetName, arearef)
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
		stringBuilder.Append("sheet=").Append(SheetName);
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
		SheetNameFormatter.AppendFormat(stringBuilder, firstSheetName);
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
