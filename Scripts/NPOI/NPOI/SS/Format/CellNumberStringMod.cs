using System;

namespace NPOI.SS.Format;

public class CellNumberStringMod : IComparable<CellNumberStringMod>
{
	public const int BEFORE = 1;

	public const int AFTER = 2;

	public const int REPLACE = 3;

	private CellNumberFormatter.Special special;

	private int op;

	private string toAdd;

	private CellNumberFormatter.Special end;

	private bool startInclusive;

	private bool endInclusive;

	public int Op => op;

	public string ToAdd => toAdd;

	public CellNumberFormatter.Special End => end;

	public bool IsStartInclusive => startInclusive;

	public bool IsEndInclusive => endInclusive;

	public CellNumberStringMod(CellNumberFormatter.Special special, string toAdd, int op)
	{
		this.special = special;
		this.toAdd = toAdd;
		this.op = op;
	}

	public CellNumberStringMod(CellNumberFormatter.Special start, bool startInclusive, CellNumberFormatter.Special end, bool endInclusive, char toAdd)
		: this(start, startInclusive, end, endInclusive)
	{
		this.toAdd = toAdd.ToString() ?? "";
	}

	public CellNumberStringMod(CellNumberFormatter.Special start, bool startInclusive, CellNumberFormatter.Special end, bool endInclusive)
	{
		special = start;
		this.startInclusive = startInclusive;
		this.end = end;
		this.endInclusive = endInclusive;
		op = 3;
		toAdd = "";
	}

	public int CompareTo(CellNumberStringMod that)
	{
		int num = special.pos - that.special.pos;
		if (num == 0)
		{
			return op - that.op;
		}
		return num;
	}

	public override bool Equals(object that)
	{
		try
		{
			return CompareTo((CellNumberStringMod)that) == 0;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public override int GetHashCode()
	{
		return special.GetHashCode() + op;
	}

	public CellNumberFormatter.Special GetSpecial()
	{
		return special;
	}
}
