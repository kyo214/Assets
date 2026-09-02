using System;

namespace NPOI.POIFS.Crypt;

public class HashAlgorithm
{
	public static readonly HashAlgorithm none = new HashAlgorithm("", 0, "", 0, "", needsBouncyCastle: false);

	public static readonly HashAlgorithm sha1 = new HashAlgorithm("SHA-1", 32772, "SHA1", 20, "HmacSHA1", needsBouncyCastle: false);

	public static readonly HashAlgorithm sha256 = new HashAlgorithm("SHA-256", 32780, "SHA256", 32, "HmacSHA256", needsBouncyCastle: false);

	public static readonly HashAlgorithm sha384 = new HashAlgorithm("SHA-384", 32781, "SHA384", 48, "HmacSHA384", needsBouncyCastle: false);

	public static readonly HashAlgorithm sha512 = new HashAlgorithm("SHA-512", 32782, "SHA512", 64, "HmacSHA512", needsBouncyCastle: false);

	public static readonly HashAlgorithm md5 = new HashAlgorithm("MD5", -1, "MD5", 16, "HmacMD5", needsBouncyCastle: false);

	public static readonly HashAlgorithm md2 = new HashAlgorithm("MD2", -1, "MD2", 16, "Hmac-MD2", needsBouncyCastle: true);

	public static readonly HashAlgorithm md4 = new HashAlgorithm("MD4", -1, "MD4", 16, "Hmac-MD4", needsBouncyCastle: true);

	public static readonly HashAlgorithm ripemd128 = new HashAlgorithm("RipeMD128", -1, "RIPEMD-128", 16, "HMac-RipeMD128", needsBouncyCastle: true);

	public static readonly HashAlgorithm ripemd160 = new HashAlgorithm("RipeMD160", -1, "RIPEMD-160", 20, "HMac-RipeMD160", needsBouncyCastle: true);

	public static readonly HashAlgorithm whirlpool = new HashAlgorithm("Whirlpool", -1, "WHIRLPOOL", 64, "HMac-Whirlpool", needsBouncyCastle: true);

	public static readonly HashAlgorithm sha224 = new HashAlgorithm("SHA-224", -1, "SHA224", 28, "HmacSHA224", needsBouncyCastle: true);

	public static HashAlgorithm[] values = new HashAlgorithm[12]
	{
		none, sha1, sha256, sha384, sha512, md5, md4, md2, ripemd128, ripemd160,
		whirlpool, sha224
	};

	public string jceId;

	public int ecmaId;

	public string ecmaString;

	public int hashSize;

	public string jceHmacId;

	public bool needsBouncyCastle;

	public HashAlgorithm(string jceId, int ecmaId, string ecmaString, int hashSize, string jceHmacId, bool needsBouncyCastle)
	{
		this.jceId = jceId;
		this.ecmaId = ecmaId;
		this.ecmaString = ecmaString;
		this.hashSize = hashSize;
		this.jceHmacId = jceHmacId;
		this.needsBouncyCastle = needsBouncyCastle;
	}

	public static HashAlgorithm FromEcmaId(int ecmaId)
	{
		HashAlgorithm[] array = values;
		foreach (HashAlgorithm hashAlgorithm in array)
		{
			if (hashAlgorithm.ecmaId == ecmaId)
			{
				return hashAlgorithm;
			}
		}
		throw new EncryptedDocumentException("hash algorithm not found");
	}

	public static HashAlgorithm FromEcmaId(string ecmaString)
	{
		HashAlgorithm[] array = values;
		foreach (HashAlgorithm hashAlgorithm in array)
		{
			if (hashAlgorithm.ecmaString.Equals(ecmaString))
			{
				return hashAlgorithm;
			}
		}
		throw new EncryptedDocumentException("hash algorithm not found");
	}

	public static HashAlgorithm FromString(string string1)
	{
		HashAlgorithm[] array = values;
		foreach (HashAlgorithm hashAlgorithm in array)
		{
			if (hashAlgorithm.ecmaString.Equals(string1, StringComparison.CurrentCultureIgnoreCase) || hashAlgorithm.jceId.Equals(string1, StringComparison.CurrentCultureIgnoreCase))
			{
				return hashAlgorithm;
			}
		}
		throw new EncryptedDocumentException("hash algorithm not found");
	}
}
