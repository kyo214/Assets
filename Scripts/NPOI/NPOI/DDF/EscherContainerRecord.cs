using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherContainerRecord : EscherRecord
{
	public const short DGG_CONTAINER = -4096;

	public const short BSTORE_CONTAINER = -4095;

	public const short DG_CONTAINER = -4094;

	public const short SPGR_CONTAINER = -4093;

	public const short SP_CONTAINER = -4092;

	public const short SOLVER_CONTAINER = -4091;

	private static POILogger log = POILogFactory.GetLogger(typeof(EscherContainerRecord));

	private int _remainingLength;

	private List<EscherRecord> _childRecords = new List<EscherRecord>();

	public override int RecordSize
	{
		get
		{
			int num = 0;
			IEnumerator enumerator = ChildRecords.GetEnumerator();
			while (enumerator.MoveNext())
			{
				EscherRecord escherRecord = (EscherRecord)enumerator.Current;
				num += escherRecord.RecordSize;
			}
			return 8 + num;
		}
	}

	public override List<EscherRecord> ChildRecords
	{
		get
		{
			return new List<EscherRecord>(_childRecords);
		}
		set
		{
			if (value == _childRecords)
			{
				throw new InvalidOperationException("Child records private data member has escaped");
			}
			_childRecords.Clear();
			_childRecords.AddRange(value);
		}
	}

	public IList<EscherContainerRecord> ChildContainers
	{
		get
		{
			IList<EscherContainerRecord> list = new List<EscherContainerRecord>();
			IEnumerator enumerator = ChildRecords.GetEnumerator();
			while (enumerator.MoveNext())
			{
				EscherRecord escherRecord = (EscherRecord)enumerator.Current;
				if (escherRecord is EscherContainerRecord)
				{
					list.Add((EscherContainerRecord)escherRecord);
				}
			}
			return list;
		}
	}

	public override string RecordName => RecordId switch
	{
		-4096 => "DggContainer", 
		-4095 => "BStoreContainer", 
		-4094 => "DgContainer", 
		-4093 => "SpgrContainer", 
		-4092 => "SpContainer", 
		-4091 => "SolverContainer", 
		_ => "Container 0x" + HexDump.ToHex(RecordId), 
	};

	public override int FillFields(byte[] data, int offset, IEscherRecordFactory recordFactory)
	{
		int num = ReadHeader(data, offset);
		int num2 = 8;
		offset += 8;
		while (num > 0 && offset < data.Length)
		{
			EscherRecord escherRecord = recordFactory.CreateRecord(data, offset);
			int num3 = escherRecord.FillFields(data, offset, recordFactory);
			num2 += num3;
			offset += num3;
			num -= num3;
			AddChildRecord(escherRecord);
			if (offset >= data.Length && num > 0)
			{
				_remainingLength = num;
				log.Log(5, "Not enough Escher data: " + num + " bytes remaining but no space left");
			}
		}
		return num2;
	}

	public override int Serialize(int offset, byte[] data, EscherSerializationListener listener)
	{
		listener.BeforeRecordSerialize(offset, RecordId, this);
		LittleEndian.PutShort(data, offset, Options);
		LittleEndian.PutShort(data, offset + 2, RecordId);
		int num = 0;
		IEnumerator enumerator = ChildRecords.GetEnumerator();
		while (enumerator.MoveNext())
		{
			EscherRecord escherRecord = (EscherRecord)enumerator.Current;
			num += escherRecord.RecordSize;
		}
		num += _remainingLength;
		LittleEndian.PutInt(data, offset + 4, num);
		int num2 = offset + 8;
		IEnumerator enumerator2 = ChildRecords.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			EscherRecord escherRecord2 = (EscherRecord)enumerator2.Current;
			num2 += escherRecord2.Serialize(num2, data, listener);
		}
		listener.AfterRecordSerialize(num2, RecordId, num2 - offset, this);
		return num2 - offset;
	}

	public bool HasChildOfType(short recordId)
	{
		IEnumerator enumerator = ChildRecords.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (((EscherRecord)enumerator.Current).RecordId == recordId)
			{
				return true;
			}
		}
		return false;
	}

	public bool RemoveChildRecord(EscherRecord toBeRemoved)
	{
		return _childRecords.Remove(toBeRemoved);
	}

	public List<EscherRecord>.Enumerator GetChildIterator()
	{
		return _childRecords.GetEnumerator();
	}

	public override void Display(int indent)
	{
		base.Display(indent);
		IEnumerator enumerator = _childRecords.GetEnumerator();
		while (enumerator.MoveNext())
		{
			((EscherRecord)enumerator.Current).Display(indent + 1);
		}
	}

	public void AddChildRecord(EscherRecord record)
	{
		_childRecords.Add(record);
	}

	public void AddChildBefore(EscherRecord record, int insertBeforeRecordId)
	{
		for (int i = 0; i < _childRecords.Count; i++)
		{
			if (_childRecords[i].RecordId == insertBeforeRecordId)
			{
				_childRecords.Insert(i++, record);
			}
		}
	}

	public override string ToString()
	{
		string newLine = Environment.NewLine;
		StringBuilder stringBuilder = new StringBuilder();
		if (ChildRecords.Count > 0)
		{
			stringBuilder.Append("  children: " + newLine);
			int num = 0;
			IEnumerator enumerator = ChildRecords.GetEnumerator();
			while (enumerator.MoveNext())
			{
				EscherRecord obj = (EscherRecord)enumerator.Current;
				stringBuilder.Append("    Child " + num + ":" + newLine);
				string text = obj.ToString();
				text = text.Replace("\n", "\n    ");
				stringBuilder.Append("    ");
				stringBuilder.Append(text);
				stringBuilder.Append(newLine);
				num++;
			}
		}
		return GetType().Name + " (" + RecordName + "):" + newLine + "  isContainer: " + base.IsContainerRecord + newLine + "  version: 0x" + HexDump.ToHex(Version) + newLine + "  instance: 0x" + HexDump.ToHex(Instance) + newLine + "  recordId: 0x" + HexDump.ToHex(RecordId) + newLine + "  numchildren: " + ChildRecords.Count + newLine + stringBuilder.ToString();
	}

	public override string ToXml(string tab)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(tab).Append(FormatXmlRecordHeader(RecordName, HexDump.ToHex(RecordId), HexDump.ToHex(Version), HexDump.ToHex(Instance)));
		IEnumerator<EscherRecord> enumerator = _childRecords.GetEnumerator();
		while (enumerator.MoveNext())
		{
			EscherRecord current = enumerator.Current;
			stringBuilder.Append(current.ToXml(tab + "\t"));
		}
		stringBuilder.Append(tab).Append("</").Append(RecordName)
			.Append(">\n");
		return stringBuilder.ToString();
	}

	public EscherRecord GetChildById(short recordId)
	{
		IEnumerator enumerator = _childRecords.GetEnumerator();
		while (enumerator.MoveNext())
		{
			EscherRecord escherRecord = (EscherRecord)enumerator.Current;
			if (escherRecord.RecordId == recordId)
			{
				return escherRecord;
			}
		}
		return null;
	}

	public void GetRecordsById(short recordId, ref ArrayList out1)
	{
		IEnumerator enumerator = ChildRecords.GetEnumerator();
		while (enumerator.MoveNext())
		{
			object current = enumerator.Current;
			EscherRecord escherRecord = (EscherRecord)current;
			if (escherRecord is EscherContainerRecord)
			{
				((EscherContainerRecord)escherRecord).GetRecordsById(recordId, ref out1);
			}
			else if (escherRecord.RecordId == recordId)
			{
				out1.Add(current);
			}
		}
	}
}
