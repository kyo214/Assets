using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class ColumnInfoRecord : StandardRecord, ICloneable
{
	public const short sid = 125;

	private int _first_col;

	private int _last_col;

	private int _col_width;

	private int _xf_index;

	private int _options;

	private static BitField hidden = BitFieldFactory.GetInstance(1);

	private static BitField outlevel = BitFieldFactory.GetInstance(1792);

	private static BitField collapsed = BitFieldFactory.GetInstance(4096);

	private int field_6_reserved;

	public int FirstColumn
	{
		get
		{
			return _first_col;
		}
		set
		{
			_first_col = value;
		}
	}

	public int LastColumn
	{
		get
		{
			return _last_col;
		}
		set
		{
			_last_col = value;
		}
	}

	public int ColumnWidth
	{
		get
		{
			return _col_width;
		}
		set
		{
			_col_width = value;
		}
	}

	public int XFIndex
	{
		get
		{
			return _xf_index;
		}
		set
		{
			_xf_index = value;
		}
	}

	public int Options
	{
		get
		{
			return _options;
		}
		set
		{
			_options = value;
		}
	}

	public bool IsHidden
	{
		get
		{
			return hidden.IsSet(_options);
		}
		set
		{
			_options = hidden.SetBoolean(_options, value);
		}
	}

	public int OutlineLevel
	{
		get
		{
			return outlevel.GetValue(_options);
		}
		set
		{
			_options = outlevel.SetValue(_options, value);
		}
	}

	public bool IsCollapsed
	{
		get
		{
			return collapsed.IsSet(_options);
		}
		set
		{
			_options = collapsed.SetBoolean(_options, value);
		}
	}

	public override short Sid => 125;

	protected override int DataSize => 12;

	public ColumnInfoRecord()
	{
		ColumnWidth = 2275;
		_options = 2;
		_xf_index = 15;
		field_6_reserved = 2;
	}

	public ColumnInfoRecord(RecordInputStream in1)
	{
		_first_col = in1.ReadUShort();
		_last_col = in1.ReadUShort();
		_col_width = in1.ReadUShort();
		_xf_index = in1.ReadUShort();
		_options = in1.ReadUShort();
		switch (in1.Remaining)
		{
		case 2:
			field_6_reserved = in1.ReadUShort();
			break;
		case 1:
			field_6_reserved = in1.ReadByte();
			break;
		case 0:
			field_6_reserved = 0;
			break;
		default:
			throw new Exception("Unusual record size remaining=(" + in1.Remaining + ")");
		}
	}

	public bool FormatMatches(ColumnInfoRecord other)
	{
		if (_xf_index != other._xf_index)
		{
			return false;
		}
		if (_options != other._options)
		{
			return false;
		}
		if (_col_width != other._col_width)
		{
			return false;
		}
		return true;
	}

	public bool ContainsColumn(int columnIndex)
	{
		if (_first_col <= columnIndex)
		{
			return columnIndex <= _last_col;
		}
		return false;
	}

	public bool IsAdjacentBefore(ColumnInfoRecord other)
	{
		return _last_col == other._first_col - 1;
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(FirstColumn);
		out1.WriteShort(LastColumn);
		out1.WriteShort(ColumnWidth);
		out1.WriteShort(XFIndex);
		out1.WriteShort(_options);
		out1.WriteShort(field_6_reserved);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[COLINFO]\n");
		stringBuilder.Append("colfirst       = ").Append(FirstColumn).Append("\n");
		stringBuilder.Append("collast        = ").Append(LastColumn).Append("\n");
		stringBuilder.Append("colwidth       = ").Append(ColumnWidth).Append("\n");
		stringBuilder.Append("xFindex        = ").Append(XFIndex).Append("\n");
		stringBuilder.Append("options        = ").Append(Options).Append("\n");
		stringBuilder.Append("  hidden       = ").Append(IsHidden).Append("\n");
		stringBuilder.Append("  olevel       = ").Append(OutlineLevel).Append("\n");
		stringBuilder.Append("  collapsed    = ").Append(IsCollapsed).Append("\n");
		stringBuilder.Append("[/COLINFO]\n");
		return stringBuilder.ToString();
	}

	public override object Clone()
	{
		return new ColumnInfoRecord
		{
			_first_col = _first_col,
			_last_col = _last_col,
			_col_width = _col_width,
			_xf_index = _xf_index,
			_options = _options,
			field_6_reserved = field_6_reserved
		};
	}
}
