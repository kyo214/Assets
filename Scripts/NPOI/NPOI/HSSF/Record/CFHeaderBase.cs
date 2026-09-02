using System;
using System.Text;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public abstract class CFHeaderBase : StandardRecord, ICloneable
{
	private int field_1_numcf;

	private int field_2_need_recalculation_and_id;

	private CellRangeAddress field_3_enclosing_cell_range;

	private CellRangeAddressList field_4_cell_ranges;

	public int NumberOfConditionalFormats
	{
		get
		{
			return field_1_numcf;
		}
		set
		{
			field_1_numcf = value;
		}
	}

	public bool NeedRecalculation
	{
		get
		{
			return (field_2_need_recalculation_and_id & 1) == 1;
		}
		set
		{
			if (value != NeedRecalculation)
			{
				if (value)
				{
					field_2_need_recalculation_and_id++;
				}
				else
				{
					field_2_need_recalculation_and_id--;
				}
			}
		}
	}

	public int ID
	{
		get
		{
			return field_2_need_recalculation_and_id >> 1;
		}
		set
		{
			bool needRecalculation = NeedRecalculation;
			field_2_need_recalculation_and_id = value << 1;
			if (needRecalculation)
			{
				field_2_need_recalculation_and_id++;
			}
		}
	}

	public CellRangeAddress EnclosingCellRange
	{
		get
		{
			return field_3_enclosing_cell_range;
		}
		set
		{
			field_3_enclosing_cell_range = value;
		}
	}

	public CellRangeAddress[] CellRanges
	{
		get
		{
			return field_4_cell_ranges.CellRangeAddresses;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentException("cellRanges must not be null");
			}
			CellRangeAddressList cellRangeAddressList = new CellRangeAddressList();
			CellRangeAddress crB = null;
			foreach (CellRangeAddress cellRangeAddress in value)
			{
				crB = CellRangeUtil.CreateEnclosingCellRange(cellRangeAddress, crB);
				cellRangeAddressList.AddCellRangeAddress(cellRangeAddress);
			}
			field_3_enclosing_cell_range = crB;
			field_4_cell_ranges = cellRangeAddressList;
		}
	}

	protected abstract string RecordName { get; }

	protected override int DataSize => 12 + field_4_cell_ranges.Size;

	protected CFHeaderBase()
	{
	}

	protected CFHeaderBase(CellRangeAddress[] regions, int nRules)
	{
		CellRangeAddress[] cellRanges = CellRangeUtil.MergeCellRanges(regions);
		CellRanges = cellRanges;
		field_1_numcf = nRules;
	}

	protected void CreateEmpty()
	{
		field_3_enclosing_cell_range = new CellRangeAddress(0, 0, 0, 0);
		field_4_cell_ranges = new CellRangeAddressList();
	}

	protected void Read(RecordInputStream in1)
	{
		field_1_numcf = in1.ReadShort();
		field_2_need_recalculation_and_id = in1.ReadShort();
		field_3_enclosing_cell_range = new CellRangeAddress(in1);
		field_4_cell_ranges = new CellRangeAddressList(in1);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[").Append(RecordName).Append("]\n");
		stringBuilder.Append("\t.numCF             = ").Append(NumberOfConditionalFormats).Append("\n");
		stringBuilder.Append("\t.needRecalc        = ").Append(NeedRecalculation).Append("\n");
		stringBuilder.Append("\t.id                = ").Append(ID).Append("\n");
		stringBuilder.Append("\t.enclosingCellRange= ").Append(EnclosingCellRange).Append("\n");
		stringBuilder.Append("\t.CFranges=[");
		for (int i = 0; i < field_4_cell_ranges.CountRanges(); i++)
		{
			stringBuilder.Append((i == 0) ? "" : ",").Append(field_4_cell_ranges.GetCellRangeAddress(i).ToString());
		}
		stringBuilder.Append("]\n");
		stringBuilder.Append("[/").Append(RecordName).Append("]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_numcf);
		out1.WriteShort(field_2_need_recalculation_and_id);
		field_3_enclosing_cell_range.Serialize(out1);
		field_4_cell_ranges.Serialize(out1);
	}

	protected void CopyTo(CFHeaderBase result)
	{
		result.field_1_numcf = field_1_numcf;
		result.field_2_need_recalculation_and_id = field_2_need_recalculation_and_id;
		result.field_3_enclosing_cell_range = field_3_enclosing_cell_range.Copy();
		result.field_4_cell_ranges = field_4_cell_ranges.Copy();
	}
}
