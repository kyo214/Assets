namespace NPOI.HSSF.Record.Aggregates;

public interface RecordVisitor
{
	void VisitRecord(Record r);
}
