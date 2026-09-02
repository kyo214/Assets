using NPOI.Util;

namespace NPOI.POIFS.Crypt.Standard;

public class StandardEncryptionInfoBuilder : IEncryptionInfoBuilder
{
	private EncryptionInfo info;

	private StandardEncryptionHeader header;

	private StandardEncryptionVerifier verifier;

	private StandardDecryptor decryptor;

	private StandardEncryptor encryptor;

	public void Initialize(EncryptionInfo info, ILittleEndianInput dis)
	{
		this.info = info;
		dis.ReadInt();
		header = new StandardEncryptionHeader(dis);
		verifier = new StandardEncryptionVerifier(dis, header);
		if (info.VersionMinor == 2 && (info.VersionMajor == 3 || info.VersionMajor == 4))
		{
			decryptor = new StandardDecryptor(this);
		}
	}

	public void Initialize(EncryptionInfo info, CipherAlgorithm cipherAlgorithm, HashAlgorithm hashAlgorithm, int keyBits, int blockSize, ChainingMode chainingMode)
	{
		this.info = info;
		if (cipherAlgorithm == null)
		{
			cipherAlgorithm = CipherAlgorithm.aes128;
		}
		if (cipherAlgorithm != CipherAlgorithm.aes128 && cipherAlgorithm != CipherAlgorithm.aes192 && cipherAlgorithm != CipherAlgorithm.aes256)
		{
			throw new EncryptedDocumentException("Standard encryption only supports AES128/192/256.");
		}
		if (hashAlgorithm == null)
		{
			hashAlgorithm = HashAlgorithm.sha1;
		}
		if (hashAlgorithm != HashAlgorithm.sha1)
		{
			throw new EncryptedDocumentException("Standard encryption only supports SHA-1.");
		}
		if (chainingMode == null)
		{
			chainingMode = ChainingMode.ecb;
		}
		if (chainingMode != ChainingMode.ecb)
		{
			throw new EncryptedDocumentException("Standard encryption only supports ECB chaining.");
		}
		if (keyBits == -1)
		{
			keyBits = cipherAlgorithm.defaultKeySize;
		}
		if (blockSize == -1)
		{
			blockSize = cipherAlgorithm.blockSize;
		}
		bool flag = false;
		int[] allowedKeySize = cipherAlgorithm.allowedKeySize;
		foreach (int num in allowedKeySize)
		{
			flag |= num == keyBits;
		}
		if (!flag)
		{
			throw new EncryptedDocumentException("KeySize " + keyBits + " not allowed for Cipher " + cipherAlgorithm.ToString());
		}
		header = new StandardEncryptionHeader(cipherAlgorithm, hashAlgorithm, keyBits, blockSize, chainingMode);
		verifier = new StandardEncryptionVerifier(cipherAlgorithm, hashAlgorithm, keyBits, blockSize, chainingMode);
		decryptor = new StandardDecryptor(this);
		encryptor = new StandardEncryptor(this);
	}

	public StandardEncryptionHeader GetHeader()
	{
		return header;
	}

	public StandardEncryptionVerifier GetVerifier()
	{
		return verifier;
	}

	public StandardDecryptor GetDecryptor()
	{
		return decryptor;
	}

	public StandardEncryptor GetEncryptor()
	{
		return encryptor;
	}

	public EncryptionInfo GetEncryptionInfo()
	{
		return info;
	}

	EncryptionHeader IEncryptionInfoBuilder.GetHeader()
	{
		return GetHeader();
	}

	EncryptionVerifier IEncryptionInfoBuilder.GetVerifier()
	{
		return GetVerifier();
	}

	Decryptor IEncryptionInfoBuilder.GetDecryptor()
	{
		return GetDecryptor();
	}

	Encryptor IEncryptionInfoBuilder.GetEncryptor()
	{
		return GetEncryptor();
	}
}
