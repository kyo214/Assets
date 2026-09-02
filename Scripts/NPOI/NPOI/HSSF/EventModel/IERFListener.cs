using NPOI.HSSF.Record;

namespace NPOI.HSSF.EventModel;

public interface IERFListener
{
	bool ProcessRecord(NPOI.HSSF.Record.Record rec);
}
