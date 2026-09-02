using NPOI.Util;

namespace NPOI.POIFS.Crypt.CryptoAPI;

public class CryptoAPIEncryptionInfoBuilder : IEncryptionInfoBuilder
{
	private EncryptionInfo info;

	private CryptoAPIEncryptionHeader header;

	private CryptoAPIEncryptionVerifier verifier;

	private CryptoAPIDecryptor decryptor;

	private CryptoAPIEncryptor encryptor;

	public void Initialize(EncryptionInfo info, ILittleEndianInput dis)
	{
		this.info = info;
		dis.ReadInt();
		header = new CryptoAPIEncryptionHeader(dis);
		verifier = new CryptoAPIEncryptionVerifier(dis, header);
		decryptor = new CryptoAPIDecryptor(this);
		encryptor = new CryptoAPIEncryptor(this);
	}

	public void Initialize(EncryptionInfo info, CipherAlgorithm cipherAlgorithm, HashAlgorithm hashAlgorithm, int keyBits, int blockSize, ChainingMode chainingMode)
	{
		this.info = info;
		if (cipherAlgorithm == null)
		{
			cipherAlgorithm = CipherAlgorithm.rc4;
		}
		if (hashAlgorithm == null)
		{
			hashAlgorithm = HashAlgorithm.sha1;
		}
		if (keyBits == -1)
		{
			keyBits = 40;
		}
		header = new CryptoAPIEncryptionHeader(cipherAlgorithm, hashAlgorithm, keyBits, blockSize, chainingMode);
		verifier = new CryptoAPIEncryptionVerifier(cipherAlgorithm, hashAlgorithm, keyBits, blockSize, chainingMode);
		decryptor = new CryptoAPIDecryptor(this);
		encryptor = new CryptoAPIEncryptor(this);
	}

	public CryptoAPIEncryptionHeader GetHeader()
	{
		return header;
	}

	public CryptoAPIEncryptionVerifier GetVerifier()
	{
		return verifier;
	}

	public CryptoAPIDecryptor GetDecryptor()
	{
		return decryptor;
	}

	public CryptoAPIEncryptor GetEncryptor()
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
