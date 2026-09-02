using System;
using NPOI.HSSF.Record;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

[Serializable]
public abstract class AreaPtgBase : OperandPtg, AreaI
{
	private int field_1_first_row;

	private int field_2_last_row;

	private int field_3_first_column;

	private int field_4_last_column;

	private static BitField rowRelative = BitFieldFactory.GetInstance(32768);

	private static BitField colRelative = BitFieldFactory.GetInstance(16384);

	private static BitField columnMask = BitFieldFactory.GetInstance(16383);

	public virtual int FirstRow
	{
		get
		{
			return field_1_first_row;
		}
		set
		{
			field_1_first_row = value;
		}
	}

	public virtual int LastRow
	{
		get
		{
			return field_2_last_row;
		}
		set
		{
			field_2_last_row = value;
		}
	}

	public virtual int FirstColumn
	{
		get
		{
			return columnMask.GetValue(field_3_first_column);
		}
		set
		{
			field_3_first_column = columnMask.SetValue(field_3_first_column, value);
		}
	}

	public virtual bool IsFirstRowRelative
	{
		get
		{
			return rowRelative.IsSet(field_3_first_column);
		}
		set
		{
			field_3_first_column = rowRelative.SetBoolean(field_3_first_column, value);
		}
	}

	public virtual bool IsFirstColRelative
	{
		get
		{
			return colRelative.IsSet(field_3_first_column);
		}
		set
		{
			field_3_first_column = colRelative.SetBoolean(field_3_first_column, value);
		}
	}

	public virtual int LastColumn
	{
		get
		{
			return columnMask.GetValue(field_4_last_column);
		}
		set
		{
			field_4_last_column = columnMask.SetValue(field_4_last_column, value);
		}
	}

	public virtual short LastColumnRaw => (short)field_4_last_column;

	public virtual bool IsLastRowRelative
	{
		get
		{
			return rowRelative.IsSet(field_4_last_column);
		}
		set
		{
			field_4_last_column = rowRelative.SetBoolean(field_4_last_column, value);
		}
	}

	public virtual bool IsLastColRelative
	{
		get
		{
			return colRelative.IsSet(field_4_last_column);
		}
		set
		{
			field_4_last_column = colRelative.SetBoolean(field_4_last_column, value);
		}
	}

	public override byte DefaultOperandClass => 0;

	protected Exception NotImplemented()
	{
		return new NotImplementedException("Coding Error: This method should never be called. This ptg should be Converted");
	}

	protected AreaPtgBase()
	{
	}

	protected AreaPtgBase(string arearef)
		: this(new AreaReference(arearef))
	{
	}

	protected AreaPtgBase(AreaReference ar)
	{
		CellReference firstCell = ar.FirstCell;
		CellReference lastCell = ar.LastCell;
		FirstRow = firstCell.Row;
		FirstColumn = ((firstCell.Col != -1) ? firstCell.Col : 0);
		LastRow = lastCell.Row;
		LastColumn = ((lastCell.Col == -1) ? 255 : lastCell.Col);
		IsFirstColRelative = !firstCell.IsColAbsolute;
		IsLastColRelative = !lastCell.IsColAbsolute;
		IsFirstRowRelative = !firstCell.IsRowAbsolute;
		IsLastRowRelative = !lastCell.IsRowAbsolute;
	}

	protected AreaPtgBase(int firstRow, int lastRow, int firstColumn, int lastColumn, bool firstRowRelative, bool lastRowRelative, bool firstColRelative, bool lastColRelative)
	{
		if (lastRow >= firstRow)
		{
			FirstRow = firstRow;
			LastRow = lastRow;
			IsFirstRowRelative = firstRowRelative;
			IsLastRowRelative = lastRowRelative;
		}
		else
		{
			FirstRow = lastRow;
			LastRow = firstRow;
			IsFirstRowRelative = lastRowRelative;
			IsLastRowRelative = firstRowRelative;
		}
		if (lastColumn >= firstColumn)
		{
			FirstColumn = firstColumn;
			LastColumn = lastColumn;
			IsFirstColRelative = firstColRelative;
			IsLastColRelative = lastColRelative;
		}
		else
		{
			FirstColumn = lastColumn;
			LastColumn = firstColumn;
			IsFirstColRelative = lastColRelative;
			IsLastColRelative = firstColRelative;
		}
	}

	public void SortTopLeftToBottomRight()
	{
		if (FirstRow > LastRow)
		{
			int firstRow = FirstRow;
			bool isFirstRowRelative = IsFirstRowRelative;
			FirstRow = LastRow;
			IsFirstRowRelative = IsLastRowRelative;
			LastRow = firstRow;
			IsLastRowRelative = isFirstRowRelative;
		}
		if (FirstColumn > LastColumn)
		{
			int firstColumn = FirstColumn;
			bool isFirstColRelative = IsFirstColRelative;
			FirstColumn = LastColumn;
			IsFirstColRelative = IsLastColRelative;
			LastColumn = firstColumn;
			IsLastColRelative = isFirstColRelative;
		}
	}

	protected void ReadCoordinates(ILittleEndianInput in1)
	{
		field_1_first_row = in1.ReadUShort();
		field_2_last_row = in1.ReadUShort();
		field_3_first_column = in1.ReadUShort();
		field_4_last_column = in1.ReadUShort();
	}

	protected void WriteCoordinates(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_first_row);
		out1.WriteShort(field_2_last_row);
		out1.WriteShort(field_3_first_column);
		out1.WriteShort(field_4_last_column);
	}

	protected void WriteCoordinates(byte[] array, int offset)
	{
		LittleEndian.PutUShort(array, offset, field_1_first_row);
		LittleEndian.PutUShort(array, offset + 2, field_2_last_row);
		LittleEndian.PutUShort(array, offset + 4, field_3_first_column);
		LittleEndian.PutUShort(array, offset + 6, field_4_last_column);
	}

	protected AreaPtgBase(RecordInputStream in1)
	{
		field_1_first_row = in1.ReadUShort();
		field_2_last_row = in1.ReadUShort();
		field_3_first_column = in1.ReadUShort();
		field_4_last_column = in1.ReadUShort();
	}

	public void SetLastColumnRaw(short column)
	{
		field_4_last_column = column;
	}

	public override string ToFormulaString()
	{
		return FormatReferenceAsString();
	}

	protected string FormatReferenceAsString()
	{
		CellReference cellReference = new CellReference(FirstRow, FirstColumn, !IsFirstRowRelative, !IsFirstColRelative);
		CellReference cellReference2 = new CellReference(LastRow, LastColumn, !IsLastRowRelative, !IsLastColRelative);
		if (AreaReference.IsWholeColumnReference(SpreadsheetVersion.EXCEL97, cellReference, cellReference2))
		{
			return new AreaReference(cellReference, cellReference2).FormatAsString();
		}
		return cellReference.FormatAsString() + ":" + cellReference2.FormatAsString();
	}
}
