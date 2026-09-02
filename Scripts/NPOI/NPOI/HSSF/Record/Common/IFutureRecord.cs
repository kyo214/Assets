using NPOI.SS.Util;

namespace NPOI.HSSF.Record.Common;

public interface IFutureRecord
{
	short GetFutureRecordType();

	FtrHeader GetFutureHeader();

	CellRangeAddress GetAssociatedRange();
}
