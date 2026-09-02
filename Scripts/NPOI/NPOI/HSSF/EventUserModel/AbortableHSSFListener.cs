using NPOI.HSSF.Record;

namespace NPOI.HSSF.EventUserModel;

public abstract class AbortableHSSFListener : IHSSFListener
{
	public virtual void ProcessRecord(NPOI.HSSF.Record.Record record)
	{
	}

	public abstract short AbortableProcessRecord(NPOI.HSSF.Record.Record record);
}
