using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Common;

public class FeatSmartTag : SharedFeature
{
	private byte[] data;

	public int DataSize => data.Length;

	public FeatSmartTag()
	{
		data = new byte[0];
	}

	public FeatSmartTag(RecordInputStream in1)
	{
		data = in1.ReadRemainder();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(" [FEATURE SMART TAGS]\n");
		stringBuilder.Append(" [/FEATURE SMART TAGS]\n");
		return stringBuilder.ToString();
	}

	public void Serialize(ILittleEndianOutput out1)
	{
		out1.Write(data);
	}
}
