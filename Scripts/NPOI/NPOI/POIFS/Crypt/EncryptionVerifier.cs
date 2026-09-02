namespace NPOI.POIFS.Crypt;

public abstract class EncryptionVerifier
{
	private byte[] encryptedVerifier;

	private byte[] encryptedVerifierHash;

	private byte[] encryptedKey;

	private int spinCount;

	private CipherAlgorithm cipherAlgorithm;

	private ChainingMode chainingMode;

	private HashAlgorithm hashAlgorithm;

	public byte[] Salt { get; set; }

	public byte[] EncryptedVerifier { get; set; }

	public byte[] EncryptedVerifierHash { get; set; }

	public int SpinCount { get; set; }

	public byte[] EncryptedKey { get; set; }

	public CipherAlgorithm CipherAlgorithm { get; set; }

	public HashAlgorithm HashAlgorithm { get; set; }

	public ChainingMode ChainingMode { get; set; }

	public byte[] GetVerifier()
	{
		return encryptedVerifier;
	}

	public byte[] GetVerifierHash()
	{
		return encryptedVerifierHash;
	}

	public int GetCipherMode()
	{
		return chainingMode.ecmaId;
	}

	public int GetAlgorithm()
	{
		return cipherAlgorithm.ecmaId;
	}

	public string GetAlgorithmName()
	{
		return cipherAlgorithm.jceId;
	}
}
