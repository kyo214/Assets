namespace NPOI.HSSF.Record;

public interface CellValueRecordInterface
{
	int Row { get; set; }

	int Column { get; set; }

	short XFIndex { get; set; }
}
