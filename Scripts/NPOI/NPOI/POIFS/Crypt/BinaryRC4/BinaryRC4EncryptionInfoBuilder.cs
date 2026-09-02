using NPOI.Util;

namespace NPOI.POIFS.Crypt.BinaryRC4;

public class BinaryRC4EncryptionInfoBuilder : IEncryptionInfoBuilder
{
	private EncryptionInfo info;

	private BinaryRC4EncryptionHeader header;

	private BinaryRC4EncryptionVerifier verifier;

	private BinaryRC4Decryptor decryptor;

	private BinaryRC4Encryptor encryptor;

	public void Initialize(EncryptionInfo info, ILittleEndianInput dis)
	{
		this.info = info;
		_ = info.VersionMajor;
		_ = info.VersionMinor;
		header = new BinaryRC4EncryptionHeader();
		verifier = new BinaryRC4EncryptionVerifier(dis);
		decryptor = new BinaryRC4Decryptor(this);
		encryptor = new BinaryRC4Encryptor(this);
	}

	public void Initialize(EncryptionInfo info, CipherAlgorithm cipherAlgorithm, HashAlgorithm hashAlgorithm, int keyBits, int blockSize, ChainingMode chainingMode)
	{
		this.info = info;
		header = new BinaryRC4EncryptionHeader();
		verifier = new BinaryRC4EncryptionVerifier();
		decryptor = new BinaryRC4Decryptor(this);
		encryptor = new BinaryRC4Encryptor(this);
	}

	public BinaryRC4EncryptionHeader GetHeader()
	{
		return header;
	}

	public BinaryRC4EncryptionVerifier GetVerifier()
	{
		return verifier;
	}

	public BinaryRC4Decryptor GetDecryptor()
	{
		return decryptor;
	}

	public BinaryRC4Encryptor GetEncryptor()
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
