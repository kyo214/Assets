using NPOI.Util;

namespace NPOI.POIFS.Crypt;

public interface IEncryptionInfoBuilder
{
	void Initialize(EncryptionInfo ei, ILittleEndianInput dis);

	void Initialize(EncryptionInfo ei, CipherAlgorithm cipherAlgorithm, HashAlgorithm hashAlgorithm, int keyBits, int blockSize, ChainingMode chainingMode);

	EncryptionHeader GetHeader();

	EncryptionVerifier GetVerifier();

	Decryptor GetDecryptor();

	Encryptor GetEncryptor();
}
