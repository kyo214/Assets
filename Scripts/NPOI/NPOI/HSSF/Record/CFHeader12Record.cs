using System;
using NPOI.HSSF.Record.Common;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class CFHeader12Record : CFHeaderBase, IFutureRecord, ICloneable
{
	public static short sid = 2169;

	private FtrHeader futureHeader;

	protected override string RecordName => "CFHEADER12";

	protected override int DataSize => FtrHeader.GetDataSize() + base.DataSize;

	public override short Sid => sid;

	public CFHeader12Record()
	{
		CreateEmpty();
		futureHeader = new FtrHeader();
		futureHeader.RecordType = sid;
	}

	public CFHeader12Record(CellRangeAddress[] regions, int nRules)
		: base(regions, nRules)
	{
		futureHeader = new FtrHeader();
		futureHeader.RecordType = sid;
	}

	public CFHeader12Record(RecordInputStream in1)
	{
		futureHeader = new FtrHeader(in1);
		Read(in1);
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		futureHeader.AssociatedRange = base.EnclosingCellRange;
		futureHeader.Serialize(out1);
		base.Serialize(out1);
	}

	public short GetFutureRecordType()
	{
		return futureHeader.RecordType;
	}

	public FtrHeader GetFutureHeader()
	{
		return futureHeader;
	}

	public CellRangeAddress GetAssociatedRange()
	{
		return futureHeader.AssociatedRange;
	}

	public override object Clone()
	{
		CFHeader12Record cFHeader12Record = new CFHeader12Record();
		cFHeader12Record.futureHeader = (FtrHeader)futureHeader.Clone();
		CopyTo(cFHeader12Record);
		return cFHeader12Record;
	}
}
