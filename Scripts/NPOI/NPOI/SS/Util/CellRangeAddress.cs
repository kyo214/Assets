using System;
using System.Text;
using NPOI.HSSF.Record;
using NPOI.SS.Formula;
using NPOI.Util;

namespace NPOI.SS.Util;

public class CellRangeAddress : CellRangeAddressBase
{
	public const int ENCODED_SIZE = 8;

	public CellRangeAddress(int firstRow, int lastRow, int firstCol, int lastCol)
		: base(firstRow, lastRow, firstCol, lastCol)
	{
		if (lastRow < firstRow || lastCol < firstCol)
		{
			throw new ArgumentException("lastRow < firstRow || lastCol < firstCol");
		}
	}

	public CellRangeAddress(RecordInputStream in1)
		: base(ReadUShortAndCheck(in1), in1.ReadUShort(), in1.ReadUShort(), in1.ReadUShort())
	{
	}

	private static int ReadUShortAndCheck(RecordInputStream in1)
	{
		if (in1.Remaining < 8)
		{
			throw new RuntimeException("Ran out of data readin1g CellRangeAddress");
		}
		return in1.ReadUShort();
	}

	public void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(base.FirstRow);
		out1.WriteShort(base.LastRow);
		out1.WriteShort(base.FirstColumn);
		out1.WriteShort(base.LastColumn);
	}

	public string FormatAsString()
	{
		return FormatAsString(null, useAbsoluteAddress: false);
	}

	public string FormatAsString(string sheetName, bool useAbsoluteAddress)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (sheetName != null)
		{
			stringBuilder.Append(SheetNameFormatter.Format(sheetName));
			stringBuilder.Append("!");
		}
		CellReference cellReference = new CellReference(base.FirstRow, base.FirstColumn, useAbsoluteAddress, useAbsoluteAddress);
		CellReference cellReference2 = new CellReference(base.LastRow, base.LastColumn, useAbsoluteAddress, useAbsoluteAddress);
		stringBuilder.Append(cellReference.FormatAsString());
		if (!cellReference.Equals(cellReference2) || base.IsFullColumnRange || base.IsFullRowRange)
		{
			stringBuilder.Append(':');
			stringBuilder.Append(cellReference2.FormatAsString());
		}
		return stringBuilder.ToString();
	}

	public CellRangeAddress Copy()
	{
		return new CellRangeAddress(base.FirstRow, base.LastRow, base.FirstColumn, base.LastColumn);
	}

	public static int GetEncodedSize(int numberOfItems)
	{
		return numberOfItems * 8;
	}

	public static CellRangeAddress ValueOf(string reference)
	{
		int num = reference.IndexOf(":", StringComparison.Ordinal);
		CellReference cellReference;
		CellReference cellReference2;
		if (num == -1)
		{
			cellReference = new CellReference(reference);
			cellReference2 = cellReference;
		}
		else
		{
			cellReference = new CellReference(reference.Substring(0, num));
			cellReference2 = new CellReference(reference.Substring(num + 1));
		}
		return new CellRangeAddress(cellReference.Row, cellReference2.Row, cellReference.Col, cellReference2.Col);
	}
}
