using System;
using NPOI.DDF;
using NPOI.SS;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFClientAnchor : HSSFAnchor, IClientAnchor
{
	public static int MAX_COL = SpreadsheetVersion.EXCEL97.LastColumnIndex;

	public static int MAX_ROW = SpreadsheetVersion.EXCEL97.LastRowIndex;

	private EscherClientAnchorRecord _escherClientAnchor;

	public int Col1
	{
		get
		{
			return _escherClientAnchor.Col1;
		}
		set
		{
			CheckRange(value, 0, MAX_COL, "col1");
			_escherClientAnchor.Col1 = (short)value;
		}
	}

	public int Col2
	{
		get
		{
			return _escherClientAnchor.Col2;
		}
		set
		{
			CheckRange(value, 0, MAX_COL, "col2");
			_escherClientAnchor.Col2 = (short)value;
		}
	}

	public int Row1
	{
		get
		{
			return unsignedValue(_escherClientAnchor.Row1);
		}
		set
		{
			CheckRange(value, 0, MAX_ROW, "row1");
			_escherClientAnchor.Row1 = (short)value;
		}
	}

	public int Row2
	{
		get
		{
			return unsignedValue(_escherClientAnchor.Row2);
		}
		set
		{
			CheckRange(value, 0, MAX_ROW, "row2");
			_escherClientAnchor.Row2 = (short)value;
		}
	}

	public override bool IsHorizontallyFlipped => _isHorizontallyFlipped;

	public override bool IsVerticallyFlipped => _isVerticallyFlipped;

	public AnchorType AnchorType
	{
		get
		{
			return (AnchorType)_escherClientAnchor.Flag;
		}
		set
		{
			_escherClientAnchor.Flag = (short)value;
		}
	}

	public override int Dx1
	{
		get
		{
			return _escherClientAnchor.Dx1;
		}
		set
		{
			_escherClientAnchor.Dx1 = (short)value;
		}
	}

	public override int Dx2
	{
		get
		{
			return _escherClientAnchor.Dx2;
		}
		set
		{
			_escherClientAnchor.Dx2 = (short)value;
		}
	}

	public override int Dy1
	{
		get
		{
			return _escherClientAnchor.Dy1;
		}
		set
		{
			_escherClientAnchor.Dy1 = (short)value;
		}
	}

	public override int Dy2
	{
		get
		{
			return _escherClientAnchor.Dy2;
		}
		set
		{
			_escherClientAnchor.Dy2 = (short)value;
		}
	}

	public HSSFClientAnchor(EscherClientAnchorRecord escherClientAnchorRecord)
	{
		_escherClientAnchor = escherClientAnchorRecord;
	}

	public HSSFClientAnchor()
	{
		_escherClientAnchor = new EscherClientAnchorRecord();
	}

	public HSSFClientAnchor(int dx1, int dy1, int dx2, int dy2, int col1, int row1, int col2, int row2)
		: base(dx1, dy1, dx2, dy2)
	{
		CheckRange(dx1, 0, 1023, "dx1");
		CheckRange(dx2, 0, 1023, "dx2");
		CheckRange(dy1, 0, 255, "dy1");
		CheckRange(dy2, 0, 255, "dy2");
		CheckRange(col1, 0, MAX_COL, "col1");
		CheckRange(col2, 0, MAX_COL, "col2");
		CheckRange(row1, 0, MAX_ROW, "row1");
		CheckRange(row2, 0, MAX_ROW, "row2");
		Col1 = (short)Math.Min(col1, col2);
		Col2 = (short)Math.Max(col1, col2);
		Row1 = Math.Min(row1, row2);
		Row2 = Math.Max(row1, row2);
		if (col1 > col2)
		{
			_isHorizontallyFlipped = true;
		}
		if (row1 > row2)
		{
			_isVerticallyFlipped = true;
		}
	}

	public float GetAnchorHeightInPoints(ISheet sheet)
	{
		int dy = Dy1;
		int dy2 = Dy2;
		int num = Math.Min(Row1, Row2);
		int num2 = Math.Max(Row1, Row2);
		float num3 = 0f;
		if (num == num2)
		{
			return (float)(dy2 - dy) / 256f * GetRowHeightInPoints(sheet, num2);
		}
		num3 += (256f - (float)dy) / 256f * GetRowHeightInPoints(sheet, num);
		for (int i = num + 1; i < num2; i++)
		{
			num3 += GetRowHeightInPoints(sheet, i);
		}
		return num3 + (float)dy2 / 256f * GetRowHeightInPoints(sheet, num2);
	}

	private float GetRowHeightInPoints(ISheet sheet, int rowNum)
	{
		return sheet.GetRow(rowNum)?.HeightInPoints ?? sheet.DefaultRowHeightInPoints;
	}

	public void SetAnchor(short col1, int row1, int x1, int y1, short col2, int row2, int x2, int y2)
	{
		CheckRange(x1, 0, 1023, "dx1");
		CheckRange(x2, 0, 1023, "dx2");
		CheckRange(y1, 0, 255, "dy1");
		CheckRange(y2, 0, 255, "dy2");
		CheckRange(col1, 0, MAX_COL, "col1");
		CheckRange(col2, 0, MAX_COL, "col2");
		CheckRange(row1, 0, MAX_ROW, "row1");
		CheckRange(row2, 0, MAX_ROW, "row2");
		Col1 = col1;
		Row1 = row1;
		Dx1 = x1;
		Dy1 = y1;
		Col2 = col2;
		Row2 = row2;
		Dx2 = x2;
		Dy2 = y2;
	}

	private void CheckRange(int value, int minRange, int maxRange, string varName)
	{
		if (value < minRange || value > maxRange)
		{
			throw new ArgumentOutOfRangeException(varName + " must be between " + minRange + " and " + maxRange + ", but was: " + value);
		}
	}

	internal override EscherRecord GetEscherAnchor()
	{
		return _escherClientAnchor;
	}

	protected override void CreateEscherAnchor()
	{
		_escherClientAnchor = new EscherClientAnchorRecord();
	}

	private static int unsignedValue(short s)
	{
		if (s >= 0)
		{
			return s;
		}
		return 65536 + s;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		HSSFClientAnchor hSSFClientAnchor = (HSSFClientAnchor)obj;
		if (hSSFClientAnchor.Col1 == Col1 && hSSFClientAnchor.Col2 == Col2 && hSSFClientAnchor.Dx1 == Dx1 && hSSFClientAnchor.Dx2 == Dx2 && hSSFClientAnchor.Dy1 == Dy1 && hSSFClientAnchor.Dy2 == Dy2 && hSSFClientAnchor.Row1 == Row1 && hSSFClientAnchor.Row2 == Row2)
		{
			return hSSFClientAnchor.AnchorType == AnchorType;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Col1.GetHashCode() ^ Col2.GetHashCode() ^ Dx1.GetHashCode() ^ Dx2.GetHashCode() ^ Dy1.GetHashCode() ^ Dy2.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode() ^ AnchorType.GetHashCode();
	}
}
