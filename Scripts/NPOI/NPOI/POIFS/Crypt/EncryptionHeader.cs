using System;

namespace NPOI.POIFS.Crypt;

public abstract class EncryptionHeader
{
	public static int ALGORITHM_RC4 = CipherAlgorithm.rc4.ecmaId;

	public static int ALGORITHM_AES_128 = CipherAlgorithm.aes128.ecmaId;

	public static int ALGORITHM_AES_192 = CipherAlgorithm.aes192.ecmaId;

	public static int ALGORITHM_AES_256 = CipherAlgorithm.aes256.ecmaId;

	public static int HASH_NONE = HashAlgorithm.none.ecmaId;

	public static int HASH_SHA1 = HashAlgorithm.sha1.ecmaId;

	public static int HASH_SHA256 = HashAlgorithm.sha256.ecmaId;

	public static int HASH_SHA384 = HashAlgorithm.sha384.ecmaId;

	public static int HASH_SHA512 = HashAlgorithm.sha512.ecmaId;

	public static int PROVIDER_RC4 = CipherProvider.rc4.ecmaId;

	public static int PROVIDER_AES = CipherProvider.aes.ecmaId;

	public static int MODE_ECB = ChainingMode.ecb.ecmaId;

	public static int MODE_CBC = ChainingMode.cbc.ecmaId;

	public static int MODE_CFB = ChainingMode.cfb.ecmaId;

	public ChainingMode ChainingMode { get; set; }

	public int Flags { get; set; }

	public int SizeExtra { get; set; }

	public CipherAlgorithm CipherAlgorithm { get; set; }

	public HashAlgorithm HashAlgorithm { get; set; }

	public int KeySize { get; set; }

	public int BlockSize { get; set; }

	public byte[] KeySalt { get; set; }

	public CipherProvider CipherProvider { get; set; }

	public string CspName { get; set; }

	[Obsolete("use ChainingMode.ecmaId", true)]
	public int GetCipherMode()
	{
		return ChainingMode.ecmaId;
	}

	[Obsolete("use CipherAlgorithm")]
	public int GetAlgorithm()
	{
		return CipherAlgorithm.ecmaId;
	}

	[Obsolete("use HashAlgorithmEx")]
	public int GetHashAlgorithm()
	{
		return HashAlgorithm.ecmaId;
	}

	[Obsolete("use CipherProvider")]
	public int GetProviderType()
	{
		return CipherProvider.ecmaId;
	}
}
