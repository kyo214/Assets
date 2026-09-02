using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public class UnknownEscherRecord : EscherRecord, ICloneable
{
	private static byte[] NO_BYTES = new byte[0];

	private byte[] _thedata = NO_BYTES;

	private List<EscherRecord> _childRecords = new List<EscherRecord>();

	public byte[] Data => _thedata;

	public override int RecordSize => 8 + _thedata.Length;

	public override List<EscherRecord> ChildRecords
	{
		get
		{
			return _childRecords;
		}
		set
		{
			_childRecords = value;
		}
	}

	public override string RecordName => "Unknown 0x" + HexDump.ToHex(RecordId);

	public override int FillFields(byte[] data, int offset, IEscherRecordFactory recordFactory)
	{
		int num = ReadHeader(data, offset);
		int num2 = data.Length - (offset + 8);
		if (num > num2)
		{
			num = num2;
		}
		if (base.IsContainerRecord)
		{
			int num3 = 0;
			_thedata = new byte[0];
			offset += 8;
			num3 += 8;
			while (num > 0)
			{
				EscherRecord escherRecord = recordFactory.CreateRecord(data, offset);
				int num4 = escherRecord.FillFields(data, offset, recordFactory);
				num3 += num4;
				offset += num4;
				num -= num4;
				ChildRecords.Add(escherRecord);
			}
			return num3;
		}
		_thedata = new byte[num];
		Array.Copy(data, offset + 8, _thedata, 0, num);
		return num + 8;
	}

	public override int Serialize(int offset, byte[] data, EscherSerializationListener listener)
	{
		listener.BeforeRecordSerialize(offset, RecordId, this);
		LittleEndian.PutShort(data, offset, Options);
		LittleEndian.PutShort(data, offset + 2, RecordId);
		int num = _thedata.Length;
		IEnumerator enumerator = ChildRecords.GetEnumerator();
		while (enumerator.MoveNext())
		{
			EscherRecord escherRecord = (EscherRecord)enumerator.Current;
			num += escherRecord.RecordSize;
		}
		LittleEndian.PutInt(data, offset + 4, num);
		Array.Copy(_thedata, 0, data, offset + 8, _thedata.Length);
		int num2 = offset + 8 + _thedata.Length;
		IEnumerator enumerator2 = ChildRecords.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			EscherRecord escherRecord2 = (EscherRecord)enumerator2.Current;
			num2 += escherRecord2.Serialize(num2, data);
		}
		listener.AfterRecordSerialize(num2, RecordId, num2 - offset, this);
		return num2 - offset;
	}

	public override object Clone()
	{
		return new UnknownEscherRecord
		{
			_thedata = (byte[])_thedata.Clone(),
			Options = Options,
			RecordId = RecordId
		};
	}

	public override string ToString()
	{
		string newLine = Environment.NewLine;
		StringBuilder stringBuilder = new StringBuilder();
		if (ChildRecords.Count > 0)
		{
			stringBuilder.Append("  children: " + newLine);
			IEnumerator enumerator = ChildRecords.GetEnumerator();
			while (enumerator.MoveNext())
			{
				EscherRecord escherRecord = (EscherRecord)enumerator.Current;
				stringBuilder.Append(escherRecord.ToString());
				stringBuilder.Append(newLine);
			}
		}
		string text = "";
		try
		{
			if (_thedata.Length != 0)
			{
				text = "  Extra Data(" + _thedata.Length + "):" + newLine;
				text += HexDump.Dump(_thedata, 0L, 0);
			}
		}
		catch (Exception)
		{
			text = "Error!!";
		}
		return GetType().Name + ":" + newLine + "  isContainer: " + base.IsContainerRecord + newLine + "  version: 0x" + HexDump.ToHex(Version) + newLine + "  instance: 0x" + HexDump.ToHex(Instance) + newLine + "  recordId: 0x" + HexDump.ToHex(RecordId) + newLine + "  numchildren: " + ChildRecords.Count + newLine + text + stringBuilder.ToString();
	}

	public override string ToXml(string tab)
	{
		string value = HexDump.ToHex(_thedata, 32);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(tab).Append(FormatXmlRecordHeader(GetType().Name, HexDump.ToHex(RecordId), HexDump.ToHex(Version), HexDump.ToHex(Instance))).Append(tab)
			.Append("\t")
			.Append("<IsContainer>")
			.Append(base.IsContainerRecord)
			.Append("</IsContainer>\n")
			.Append(tab)
			.Append("\t")
			.Append("<Numchildren>")
			.Append(HexDump.ToHex(_childRecords.Count))
			.Append("</Numchildren>\n");
		IEnumerator<EscherRecord> enumerator = _childRecords.GetEnumerator();
		while (enumerator.MoveNext())
		{
			EscherRecord current = enumerator.Current;
			stringBuilder.Append(current.ToXml(tab + "\t"));
		}
		stringBuilder.Append(value).Append("\n");
		stringBuilder.Append(tab).Append("</").Append(GetType().Name)
			.Append(">\n");
		return stringBuilder.ToString();
	}

	public void AddChildRecord(EscherRecord childRecord)
	{
		ChildRecords.Add(childRecord);
	}
}
