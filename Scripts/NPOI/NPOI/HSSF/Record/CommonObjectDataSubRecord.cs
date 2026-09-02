using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class CommonObjectDataSubRecord : SubRecord, ICloneable
{
	public const short sid = 21;

	private short field_1_objectType;

	private int field_2_objectId;

	private short field_3_option;

	private BitField locked = BitFieldFactory.GetInstance(1);

	private BitField printable = BitFieldFactory.GetInstance(16);

	private BitField autoFill = BitFieldFactory.GetInstance(8192);

	private BitField autoline = BitFieldFactory.GetInstance(16384);

	private int field_4_reserved1;

	private int field_5_reserved2;

	private int field_6_reserved3;

	public override int DataSize => 18;

	public override short Sid => 21;

	public CommonObjectType ObjectType
	{
		get
		{
			return (CommonObjectType)field_1_objectType;
		}
		set
		{
			field_1_objectType = (short)value;
		}
	}

	public int ObjectId
	{
		get
		{
			return field_2_objectId;
		}
		set
		{
			field_2_objectId = value;
		}
	}

	public short Option
	{
		get
		{
			return field_3_option;
		}
		set
		{
			field_3_option = value;
		}
	}

	public int Reserved1
	{
		get
		{
			return field_4_reserved1;
		}
		set
		{
			field_4_reserved1 = value;
		}
	}

	public int Reserved2
	{
		get
		{
			return field_5_reserved2;
		}
		set
		{
			field_5_reserved2 = value;
		}
	}

	public int Reserved3
	{
		get
		{
			return field_6_reserved3;
		}
		set
		{
			field_6_reserved3 = value;
		}
	}

	public bool IsLocked
	{
		get
		{
			return locked.IsSet(field_3_option);
		}
		set
		{
			field_3_option = locked.SetShortBoolean(field_3_option, value);
		}
	}

	public bool IsPrintable
	{
		get
		{
			return printable.IsSet(field_3_option);
		}
		set
		{
			field_3_option = printable.SetShortBoolean(field_3_option, value);
		}
	}

	public bool IsAutoFill
	{
		get
		{
			return autoFill.IsSet(field_3_option);
		}
		set
		{
			field_3_option = autoFill.SetShortBoolean(field_3_option, value);
		}
	}

	public bool IsAutoline
	{
		get
		{
			return autoline.IsSet(field_3_option);
		}
		set
		{
			field_3_option = autoline.SetShortBoolean(field_3_option, value);
		}
	}

	public CommonObjectDataSubRecord()
	{
	}

	public CommonObjectDataSubRecord(ILittleEndianInput in1, int size)
	{
		if (size != 18)
		{
			throw new RecordFormatException("Expected size 18 but got (" + size + ")");
		}
		field_1_objectType = in1.ReadShort();
		field_2_objectId = in1.ReadUShort();
		field_3_option = in1.ReadShort();
		field_4_reserved1 = in1.ReadInt();
		field_5_reserved2 = in1.ReadInt();
		field_6_reserved3 = in1.ReadInt();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[ftCmo]\n");
		stringBuilder.Append("    .objectType           = ").Append("0x").Append(HexDump.ToHex((short)ObjectType))
			.Append(" (")
			.Append(ObjectType)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .objectId             = ").Append("0x").Append(HexDump.ToHex(ObjectId))
			.Append(" (")
			.Append(ObjectId)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .option               = ").Append("0x").Append(HexDump.ToHex(Option))
			.Append(" (")
			.Append(Option)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("         .locked                   = ").Append(IsLocked).Append('\n');
		stringBuilder.Append("         .printable                = ").Append(IsPrintable).Append('\n');
		stringBuilder.Append("         .autoFill                 = ").Append(IsAutoFill).Append('\n');
		stringBuilder.Append("         .autoline                 = ").Append(IsAutoline).Append('\n');
		stringBuilder.Append("    .reserved1            = ").Append("0x").Append(HexDump.ToHex(Reserved1))
			.Append(" (")
			.Append(Reserved1)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .reserved2            = ").Append("0x").Append(HexDump.ToHex(Reserved2))
			.Append(" (")
			.Append(Reserved2)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .reserved3            = ").Append("0x").Append(HexDump.ToHex(Reserved3))
			.Append(" (")
			.Append(Reserved3)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/ftCmo]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(21);
		out1.WriteShort(DataSize);
		out1.WriteShort(field_1_objectType);
		out1.WriteShort(field_2_objectId);
		out1.WriteShort(field_3_option);
		out1.WriteInt(field_4_reserved1);
		out1.WriteInt(field_5_reserved2);
		out1.WriteInt(field_6_reserved3);
	}

	public override object Clone()
	{
		return new CommonObjectDataSubRecord
		{
			field_1_objectType = field_1_objectType,
			field_2_objectId = field_2_objectId,
			field_3_option = field_3_option,
			field_4_reserved1 = field_4_reserved1,
			field_5_reserved2 = field_5_reserved2,
			field_6_reserved3 = field_6_reserved3
		};
	}
}
