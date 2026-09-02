using System;
using System.Collections;
using System.Text;
using NPOI.HSSF.Record.Cont;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class SSTRecord : ContinuableRecord
{
	public const short sid = 252;

	private static readonly UnicodeString EMPTY_STRING = new UnicodeString("");

	public const int MAX_RECORD_SIZE = 8228;

	public const int STD_RECORD_OVERHEAD = 4;

	public const int SST_RECORD_OVERHEAD = 12;

	public const int MAX_DATA_SPACE = 8216;

	private int field_1_num_strings;

	private int field_2_num_unique_strings;

	private IntMapper<UnicodeString> field_3_strings;

	private SSTDeserializer deserializer;

	private int[] bucketAbsoluteOffsets;

	private int[] bucketRelativeOffsets;

	public int NumStrings
	{
		get
		{
			return field_1_num_strings;
		}
		set
		{
			field_1_num_strings = value;
		}
	}

	public int NumUniqueStrings
	{
		get
		{
			return field_2_num_unique_strings;
		}
		set
		{
			field_2_num_unique_strings = value;
		}
	}

	public override short Sid => 252;

	public int CountStrings => field_3_strings.Size;

	public SSTRecord()
	{
		field_1_num_strings = 0;
		field_2_num_unique_strings = 0;
		field_3_strings = new IntMapper<UnicodeString>();
		deserializer = new SSTDeserializer(field_3_strings);
	}

	public SSTRecord(RecordInputStream in1)
	{
		field_1_num_strings = in1.ReadInt();
		field_2_num_unique_strings = in1.ReadInt();
		field_3_strings = new IntMapper<UnicodeString>();
		deserializer = new SSTDeserializer(field_3_strings);
		deserializer.ManufactureStrings(field_2_num_unique_strings, in1);
	}

	public int AddString(UnicodeString str)
	{
		field_1_num_strings++;
		UnicodeString unicodeString = ((str == null) ? EMPTY_STRING : str);
		int index = field_3_strings.GetIndex(unicodeString);
		int result;
		if (index != -1)
		{
			result = index;
		}
		else
		{
			result = field_3_strings.Size;
			field_2_num_unique_strings++;
			SSTDeserializer.AddToStringTable(field_3_strings, unicodeString);
		}
		return result;
	}

	public UnicodeString GetString(int id)
	{
		return field_3_strings[id];
	}

	public bool IsString16bit(int id)
	{
		return (field_3_strings[id].OptionFlags & 1) == 1;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[SST]\n");
		stringBuilder.Append("    .numstrings     = ").Append(StringUtil.ToHexString(NumStrings)).Append("\n");
		stringBuilder.Append("    .uniquestrings  = ").Append(StringUtil.ToHexString(NumUniqueStrings)).Append("\n");
		for (int i = 0; i < field_3_strings.Size; i++)
		{
			UnicodeString unicodeString = field_3_strings[i];
			stringBuilder.Append("    .string_" + i + "      = ").Append(unicodeString.GetDebugInfo()).Append("\n");
		}
		stringBuilder.Append("[/SST]\n");
		return stringBuilder.ToString();
	}

	public override int GetHashCode()
	{
		return field_2_num_unique_strings;
	}

	public override bool Equals(object o)
	{
		if (o == null || o.GetType() != GetType())
		{
			return false;
		}
		SSTRecord sSTRecord = (SSTRecord)o;
		if (field_1_num_strings == sSTRecord.field_1_num_strings && field_2_num_unique_strings == sSTRecord.field_2_num_unique_strings)
		{
			return field_3_strings.Equals(sSTRecord.field_3_strings);
		}
		return false;
	}

	public IEnumerator GetStrings()
	{
		return field_3_strings.GetEnumerator();
	}

	protected override void Serialize(ContinuableRecordOutput out1)
	{
		SSTSerializer sSTSerializer = new SSTSerializer(field_3_strings, NumStrings, NumUniqueStrings);
		sSTSerializer.Serialize(out1);
		bucketAbsoluteOffsets = sSTSerializer.BucketAbsoluteOffsets;
		bucketRelativeOffsets = sSTSerializer.BucketRelativeOffsets;
	}

	private SSTDeserializer GetDeserializer()
	{
		return deserializer;
	}

	public ExtSSTRecord CreateExtSSTRecord(int sstOffset)
	{
		if (bucketAbsoluteOffsets == null || bucketRelativeOffsets == null)
		{
			throw new InvalidOperationException("SST record has not yet been Serialized.");
		}
		ExtSSTRecord extSSTRecord = new ExtSSTRecord();
		extSSTRecord.NumStringsPerBucket = 8;
		int[] array = (int[])bucketAbsoluteOffsets.Clone();
		int[] array2 = (int[])bucketRelativeOffsets.Clone();
		for (int i = 0; i < array.Length; i++)
		{
			array[i] += sstOffset;
		}
		extSSTRecord.SetBucketOffsets(array, array2);
		return extSSTRecord;
	}

	public int CalcExtSSTRecordSize()
	{
		return ExtSSTRecord.GetRecordSizeForStrings(field_3_strings.Size);
	}
}
