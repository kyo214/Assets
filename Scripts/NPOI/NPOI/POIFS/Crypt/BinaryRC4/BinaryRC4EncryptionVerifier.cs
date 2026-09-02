using NPOI.POIFS.Crypt.Standard;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.BinaryRC4;

public class BinaryRC4EncryptionVerifier : EncryptionVerifier, EncryptionRecord
{
	protected internal BinaryRC4EncryptionVerifier()
	{
		base.SpinCount = -1;
		base.CipherAlgorithm = CipherAlgorithm.rc4;
		base.ChainingMode = null;
		base.EncryptedKey = null;
		base.HashAlgorithm = HashAlgorithm.md5;
	}

	protected internal BinaryRC4EncryptionVerifier(ILittleEndianInput is1)
	{
		byte[] array = new byte[16];
		is1.ReadFully(array);
		SetSalt(array);
		byte[] buf = new byte[16];
		is1.ReadFully(buf);
		base.EncryptedVerifier = buf;
		byte[] buf2 = new byte[16];
		is1.ReadFully(buf2);
		base.EncryptedVerifierHash = buf2;
		base.SpinCount = -1;
		base.CipherAlgorithm = CipherAlgorithm.rc4;
		base.ChainingMode = null;
		base.EncryptedKey = null;
		base.HashAlgorithm = HashAlgorithm.md5;
	}

	protected internal void SetSalt(byte[] salt)
	{
		if (salt == null || salt.Length != 16)
		{
			throw new EncryptedDocumentException("invalid verifier salt");
		}
		base.Salt = salt;
	}

	public void Write(LittleEndianByteArrayOutputStream bos)
	{
		byte[] salt = base.Salt;
		bos.Write(salt);
		byte[] b = base.EncryptedVerifier;
		bos.Write(b);
		byte[] b2 = base.EncryptedVerifierHash;
		bos.Write(b2);
	}
}
