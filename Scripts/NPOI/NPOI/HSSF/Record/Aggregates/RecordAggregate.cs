using System;

namespace NPOI.HSSF.Record.Aggregates;

[Serializable]
public abstract class RecordAggregate : RecordBase
{
	private class SerializingRecordVisitor : RecordVisitor
	{
		private byte[] _data;

		private int _startOffset;

		private int _countBytesWritten;

		public SerializingRecordVisitor(byte[] data, int startOffset)
		{
			_data = data;
			_startOffset = startOffset;
			_countBytesWritten = 0;
		}

		public int CountBytesWritten()
		{
			return _countBytesWritten;
		}

		public void VisitRecord(Record r)
		{
			int offset = _startOffset + _countBytesWritten;
			_countBytesWritten += r.Serialize(offset, _data);
		}
	}

	private class RecordSizingVisitor : RecordVisitor
	{
		private int _totalSize;

		public int TotalSize => _totalSize;

		public RecordSizingVisitor()
		{
			_totalSize = 0;
		}

		public void VisitRecord(Record r)
		{
			_totalSize += r.RecordSize;
		}
	}

	public virtual short Sid
	{
		get
		{
			throw new NotImplementedException("Should not be called");
		}
	}

	public override int RecordSize
	{
		get
		{
			RecordSizingVisitor recordSizingVisitor = new RecordSizingVisitor();
			VisitContainedRecords(recordSizingVisitor);
			return recordSizingVisitor.TotalSize;
		}
	}

	public abstract void VisitContainedRecords(RecordVisitor rv);

	public override int Serialize(int offset, byte[] data)
	{
		SerializingRecordVisitor serializingRecordVisitor = new SerializingRecordVisitor(data, offset);
		VisitContainedRecords(serializingRecordVisitor);
		return serializingRecordVisitor.CountBytesWritten();
	}

	public virtual Record CloneViaReserialise()
	{
		throw new NotImplementedException("Please implement it in subclass");
	}
}
