using NPOI.HSSF.Record;
using NPOI.Util;

namespace NPOI.HSSF.Model;

public class UnsupportedBOFType : RecordFormatException
{
	private BOFRecordType type;

	public BOFRecordType Type => type;

	public UnsupportedBOFType(BOFRecordType type)
		: base("BOF not of a supported type, found " + type)
	{
		this.type = type;
	}
}
