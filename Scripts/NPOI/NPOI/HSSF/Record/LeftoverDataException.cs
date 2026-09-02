using System;
using NPOI.Util;

namespace NPOI.HSSF.Record;

[Serializable]
public class LeftoverDataException : Exception
{
	public LeftoverDataException(int sid, int remainingByteCount)
		: base("Initialisation of record 0x" + StringUtil.ToHexString(sid).ToUpper() + "(" + getRecordName(sid) + ") left " + remainingByteCount + " bytes remaining still to be read.")
	{
	}

	private static string getRecordName(int sid)
	{
		Type recordClass = RecordFactory.GetRecordClass(sid);
		if (recordClass == null)
		{
			return null;
		}
		return recordClass.Name;
	}
}
