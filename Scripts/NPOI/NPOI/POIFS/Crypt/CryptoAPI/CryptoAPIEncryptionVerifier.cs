using NPOI.POIFS.Crypt.Standard;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.CryptoAPI;

public class CryptoAPIEncryptionVerifier : StandardEncryptionVerifier
{
	protected internal CryptoAPIEncryptionVerifier(ILittleEndianInput is1, CryptoAPIEncryptionHeader header)
		: base(is1, header)
	{
	}

	protected internal CryptoAPIEncryptionVerifier(CipherAlgorithm cipherAlgorithm, HashAlgorithm hashAlgorithm, int keyBits, int blockSize, ChainingMode chainingMode)
		: base(cipherAlgorithm, hashAlgorithm, keyBits, blockSize, chainingMode)
	{
	}

	protected new void SetSalt(byte[] salt)
	{
		base.SetSalt(salt);
	}

	protected new void SetEncryptedVerifier(byte[] encryptedVerifier)
	{
		base.SetEncryptedVerifier(encryptedVerifier);
	}

	protected new void SetEncryptedVerifierHash(byte[] encryptedVerifierHash)
	{
		base.SetEncryptedVerifierHash(encryptedVerifierHash);
	}
}
