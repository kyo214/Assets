using NPOI.HSSF.Record;

namespace NPOI.HSSF.EventUserModel;

public interface IHSSFListener
{
	void ProcessRecord(NPOI.HSSF.Record.Record record);
}
