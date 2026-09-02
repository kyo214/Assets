using NPOI.Util;

namespace NPOI.POIFS.Crypt.Standard;

public interface EncryptionRecord
{
	void Write(LittleEndianByteArrayOutputStream os);
}
