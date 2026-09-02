using System;
using System.Collections.Generic;
using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public abstract class EscherRecord : ICloneable
{
	internal class DeleteEscherRecordHeader
	{
		private short options;

		private short recordId;

		private int remainingBytes;

		public short Options => options;

		public virtual short RecordId => recordId;

		public int RemainingBytes => remainingBytes;

		private DeleteEscherRecordHeader()
		{
		}

		public static DeleteEscherRecordHeader ReadHeader(byte[] data, int offset)
		{
			return new DeleteEscherRecordHeader
			{
				options = LittleEndian.GetShort(data, offset),
				recordId = LittleEndian.GetShort(data, offset + 2),
				remainingBytes = LittleEndian.GetInt(data, offset + 4)
			};
		}

		public override string ToString()
		{
			return "EscherRecordHeader{options=" + options + ", recordId=" + recordId + ", remainingBytes=" + remainingBytes + "}";
		}
	}

	private static BitField fInstance = BitFieldFactory.GetInstance(65520);

	private static BitField fVersion = BitFieldFactory.GetInstance(15);

	private short _options;

	private short _recordId;

	public bool IsContainerRecord => Version == 15;

	internal virtual short Options
	{
		get
		{
			return _options;
		}
		set
		{
			Version = fVersion.GetShortValue(value);
			Instance = fInstance.GetShortValue(value);
			_options = value;
		}
	}

	public abstract int RecordSize { get; }

	public virtual short RecordId
	{
		get
		{
			return _recordId;
		}
		set
		{
			_recordId = value;
		}
	}

	public virtual List<EscherRecord> ChildRecords
	{
		get
		{
			return new List<EscherRecord>();
		}
		set
		{
			throw new ArgumentException("This record does not support child records.");
		}
	}

	public abstract string RecordName { get; }

	public virtual short Instance
	{
		get
		{
			return fInstance.GetShortValue(_options);
		}
		set
		{
			_options = fInstance.SetShortValue(_options, value);
		}
	}

	public virtual short Version
	{
		get
		{
			return fVersion.GetShortValue(_options);
		}
		set
		{
			_options = fVersion.SetShortValue(_options, value);
		}
	}

	public EscherRecord()
	{
	}

	public int FillFields(byte[] data, IEscherRecordFactory f)
	{
		return FillFields(data, 0, f);
	}

	public abstract int FillFields(byte[] data, int offset, IEscherRecordFactory recordFactory);

	protected int ReadHeader(byte[] data, int offset)
	{
		_options = LittleEndian.GetShort(data, offset);
		_recordId = LittleEndian.GetShort(data, offset + 2);
		return LittleEndian.GetInt(data, offset + 4);
	}

	protected static short ReadInstance(byte[] data, int offset)
	{
		short holder = LittleEndian.GetShort(data, offset);
		return fInstance.GetShortValue(holder);
	}

	public byte[] Serialize()
	{
		byte[] array = new byte[RecordSize];
		Serialize(0, array);
		return array;
	}

	public int Serialize(int offset, byte[] data)
	{
		return Serialize(offset, data, new NullEscherSerializationListener());
	}

	public abstract int Serialize(int offset, byte[] data, EscherSerializationListener listener);

	public virtual object Clone()
	{
		throw new NotSupportedException("The class " + GetType().Name + " needs to define a clone method");
	}

	public EscherRecord GetChild(int index)
	{
		return ChildRecords[index];
	}

	public virtual void Display(int indent)
	{
		for (int i = 0; i < indent * 4; i++)
		{
			Console.Write(' ');
		}
		Console.WriteLine(RecordName);
	}

	public virtual string ToXml(string tab)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(tab).Append("<").Append(GetType().Name)
			.Append(">\n")
			.Append(tab)
			.Append("\t")
			.Append("<RecordId>0x")
			.Append(HexDump.ToHex(_recordId))
			.Append("</RecordId>\n")
			.Append(tab)
			.Append("\t")
			.Append("<Options>")
			.Append(_options)
			.Append("</Options>\n")
			.Append(tab)
			.Append("</")
			.Append(GetType().Name)
			.Append(">\n");
		return stringBuilder.ToString();
	}

	protected string FormatXmlRecordHeader(string className, string recordId, string version, string instance)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<").Append(className).Append(" recordId=\"0x")
			.Append(recordId)
			.Append("\" version=\"0x")
			.Append(version)
			.Append("\" instance=\"0x")
			.Append(instance)
			.Append("\" size=\"")
			.Append(RecordSize)
			.Append("\">\n");
		return stringBuilder.ToString();
	}

	public string ToXml()
	{
		return ToXml("");
	}
}
