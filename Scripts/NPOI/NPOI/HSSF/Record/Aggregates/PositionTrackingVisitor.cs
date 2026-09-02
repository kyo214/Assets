namespace NPOI.HSSF.Record.Aggregates;

public class PositionTrackingVisitor : RecordVisitor
{
	private RecordVisitor _rv;

	private int _position;

	public int Position
	{
		get
		{
			return _position;
		}
		set
		{
			_position = value;
		}
	}

	public PositionTrackingVisitor(RecordVisitor rv, int initialPosition)
	{
		_rv = rv;
		_position = initialPosition;
	}

	public void VisitRecord(Record r)
	{
		_position += r.RecordSize;
		_rv.VisitRecord(r);
	}
}
