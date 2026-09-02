using NPOI.Util;

namespace NPOI.HSSF.Record.Common;

public interface SharedFeature
{
	int DataSize { get; }

	new string ToString();

	void Serialize(ILittleEndianOutput out1);
}
