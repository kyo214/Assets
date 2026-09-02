using System;
using System.Text;

namespace NPOI.HSSF.Record;

[Obsolete("Not found in poi,is it useful?")]
public interface CustomField : ICloneable
{
	int Size { get; }

	int FillField(RecordInputStream in1);

	void ToString(StringBuilder str);

	int SerializeField(int offset, byte[] data);
}
