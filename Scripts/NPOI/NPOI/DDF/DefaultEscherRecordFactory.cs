using System;
using System.Collections.Generic;
using System.Reflection;
using NPOI.Util;

namespace NPOI.DDF;

public class DefaultEscherRecordFactory : IEscherRecordFactory
{
	private static Type[] escherRecordClasses = new Type[12]
	{
		typeof(EscherBSERecord),
		typeof(EscherOptRecord),
		typeof(EscherTertiaryOptRecord),
		typeof(EscherClientAnchorRecord),
		typeof(EscherDgRecord),
		typeof(EscherSpgrRecord),
		typeof(EscherSpRecord),
		typeof(EscherClientDataRecord),
		typeof(EscherDggRecord),
		typeof(EscherSplitMenuColorsRecord),
		typeof(EscherChildAnchorRecord),
		typeof(EscherTextboxRecord)
	};

	private static Dictionary<short, ConstructorInfo> recordsMap = RecordsToMap(escherRecordClasses);

	public virtual EscherRecord CreateRecord(byte[] data, int offset)
	{
		short options = LittleEndian.GetShort(data, offset);
		short num = LittleEndian.GetShort(data, offset + 2);
		if (IsContainer(options, num))
		{
			return new EscherContainerRecord
			{
				RecordId = num,
				Options = options
			};
		}
		if (num >= -4072 && num <= -3817)
		{
			EscherBlipRecord escherBlipRecord;
			switch (num)
			{
			case -4067:
			case -4066:
			case -4065:
				escherBlipRecord = new EscherBitmapBlip();
				break;
			case -4070:
			case -4069:
			case -4068:
				escherBlipRecord = new EscherMetafileBlip();
				break;
			default:
				escherBlipRecord = new EscherBlipRecord();
				break;
			}
			escherBlipRecord.RecordId = num;
			escherBlipRecord.Options = options;
			return escherBlipRecord;
		}
		ConstructorInfo constructorInfo = null;
		if (recordsMap.ContainsKey(num))
		{
			constructorInfo = recordsMap[num];
		}
		EscherRecord escherRecord = null;
		if (constructorInfo == null)
		{
			return new UnknownEscherRecord();
		}
		try
		{
			escherRecord = (EscherRecord)constructorInfo.Invoke(new object[0]);
		}
		catch (Exception)
		{
			return new UnknownEscherRecord();
		}
		escherRecord.RecordId = num;
		escherRecord.Options = options;
		return escherRecord;
	}

	private static Dictionary<short, ConstructorInfo> RecordsToMap(Type[] records)
	{
		Dictionary<short, ConstructorInfo> dictionary = new Dictionary<short, ConstructorInfo>();
		Type[] types = new Type[0];
		foreach (Type type in records)
		{
			short num = 0;
			try
			{
				num = (short)type.GetField("RECORD_ID").GetValue(null);
			}
			catch (Exception)
			{
				throw new RecordFormatException("Unable to determine record types");
			}
			ConstructorInfo constructor;
			try
			{
				constructor = type.GetConstructor(types);
			}
			catch (Exception e)
			{
				throw new RuntimeException(e);
			}
			dictionary.Add(num, constructor);
		}
		return dictionary;
	}

	public static bool IsContainer(short options, short recordId)
	{
		if (recordId >= -4096 && recordId <= -4091)
		{
			return true;
		}
		if (recordId == -4083)
		{
			return false;
		}
		return (options & 0xF) == 15;
	}
}
