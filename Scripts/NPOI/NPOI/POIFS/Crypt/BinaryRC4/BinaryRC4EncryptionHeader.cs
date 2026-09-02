using NPOI.POIFS.Crypt.Standard;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.BinaryRC4;

public class BinaryRC4EncryptionHeader : EncryptionHeader, EncryptionRecord
{
	protected internal BinaryRC4EncryptionHeader()
	{
		base.CipherAlgorithm = CipherAlgorithm.rc4;
		base.KeySize = 40;
		base.BlockSize = -1;
		base.CipherProvider = CipherProvider.rc4;
		base.HashAlgorithm = HashAlgorithm.md5;
		base.SizeExtra = 0;
		base.Flags = 0;
		base.CspName = "";
		base.ChainingMode = null;
	}

	public void Write(LittleEndianByteArrayOutputStream littleendianbytearrayoutputstream)
	{
	}
}
