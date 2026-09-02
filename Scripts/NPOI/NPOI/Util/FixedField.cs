using System.IO;

namespace NPOI.Util;

public interface FixedField
{
	void ReadFromBytes(byte[] data);

	void ReadFromStream(Stream stream);

	new string ToString();

	void WriteToBytes(byte[] data);
}
