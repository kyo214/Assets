using System;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class SSTDeserializer
{
	private IntMapper<UnicodeString> strings;

	public SSTDeserializer(IntMapper<UnicodeString> strings)
	{
		this.strings = strings;
	}

	public void ManufactureStrings(int stringCount, RecordInputStream in1)
	{
		for (int i = 0; i < stringCount; i++)
		{
			UnicodeString str;
			if (in1.Available() == 0 && !in1.HasNextRecord)
			{
				Console.WriteLine("Ran out of data before creating all the strings! String at index " + i);
				str = new UnicodeString("");
			}
			else
			{
				str = new UnicodeString(in1);
			}
			AddToStringTable(strings, str);
		}
	}

	public static void AddToStringTable(IntMapper<UnicodeString> strings, UnicodeString str)
	{
		strings.Add(str);
	}
}
