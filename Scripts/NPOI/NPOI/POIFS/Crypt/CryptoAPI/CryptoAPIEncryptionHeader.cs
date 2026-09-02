using NPOI.POIFS.Crypt.Standard;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.CryptoAPI;

public class CryptoAPIEncryptionHeader : StandardEncryptionHeader
{
	public CryptoAPIEncryptionHeader(ILittleEndianInput is1)
		: base(is1)
	{
	}

	protected internal CryptoAPIEncryptionHeader(CipherAlgorithm cipherAlgorithm, HashAlgorithm hashAlgorithm, int keyBits, int blockSize, ChainingMode chainingMode)
		: base(cipherAlgorithm, hashAlgorithm, keyBits, blockSize, chainingMode)
	{
	}

	public void SetKeySize(int keyBits)
	{
		bool flag = false;
		int[] allowedKeySize = base.CipherAlgorithm.allowedKeySize;
		for (int i = 0; i < allowedKeySize.Length; i++)
		{
			if (allowedKeySize[i] == keyBits)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			throw new EncryptedDocumentException("invalid keysize " + keyBits + " for cipher algorithm " + base.CipherAlgorithm);
		}
		base.KeySize = keyBits;
		if (keyBits > 40)
		{
			base.CspName = "Microsoft Enhanced Cryptographic Provider v1.0";
		}
		else
		{
			base.CspName = CipherProvider.rc4.cipherProviderName;
		}
	}
}
