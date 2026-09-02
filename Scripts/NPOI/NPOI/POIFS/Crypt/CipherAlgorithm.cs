using System;

namespace NPOI.POIFS.Crypt;

public class CipherAlgorithm
{
	public static CipherAlgorithm rc4 = new CipherAlgorithm(CipherProvider.rc4, "RC4", 26625, 64, new int[12]
	{
		40, 48, 56, 64, 72, 80, 88, 96, 104, 112,
		120, 128
	}, -1, 20, "RC4", needsBouncyCastle: false)
	{
		name = "rc4"
	};

	public static CipherAlgorithm aes128 = new CipherAlgorithm(CipherProvider.aes, "AES", 26126, 128, new int[1] { 128 }, 16, 32, "AES", needsBouncyCastle: false)
	{
		name = "aes128"
	};

	public static CipherAlgorithm aes192 = new CipherAlgorithm(CipherProvider.aes, "AES", 26127, 192, new int[1] { 192 }, 16, 32, "AES", needsBouncyCastle: false)
	{
		name = "aes192"
	};

	public static CipherAlgorithm aes256 = new CipherAlgorithm(CipherProvider.aes, "AES", 26128, 256, new int[1] { 256 }, 16, 32, "AES", needsBouncyCastle: false)
	{
		name = "aes256"
	};

	public static CipherAlgorithm rc2 = new CipherAlgorithm(null, "RC2", -1, 128, new int[12]
	{
		40, 48, 56, 64, 72, 80, 88, 96, 104, 112,
		120, 128
	}, 8, 20, "RC2", needsBouncyCastle: false)
	{
		name = "rc2"
	};

	public static CipherAlgorithm des = new CipherAlgorithm(null, "DES", -1, 64, new int[1] { 64 }, 8, 32, "DES", needsBouncyCastle: false)
	{
		name = "des"
	};

	public static CipherAlgorithm des3 = new CipherAlgorithm(null, "DESede", -1, 192, new int[1] { 192 }, 8, 32, "3DES", needsBouncyCastle: false)
	{
		name = "des3"
	};

	public static CipherAlgorithm des3_112 = new CipherAlgorithm(null, "DESede", -1, 128, new int[1] { 128 }, 8, 32, "3DES_112", needsBouncyCastle: true)
	{
		name = "des3_112"
	};

	public static CipherAlgorithm rsa = new CipherAlgorithm(null, "RSA", -1, 1024, new int[4] { 1024, 2048, 3072, 4096 }, -1, -1, "", needsBouncyCastle: false)
	{
		name = "rsa"
	};

	public static CipherAlgorithm[] Values = new CipherAlgorithm[9] { rc4, aes128, aes192, aes256, rc2, des, des3, des3_112, rsa };

	public CipherProvider provider;

	public string jceId;

	public int ecmaId;

	public int defaultKeySize;

	public int[] allowedKeySize;

	public int blockSize;

	public int encryptedVerifierHashLength;

	public string xmlId;

	public bool needsBouncyCastle;

	private string name;

	public static CipherAlgorithm ValueOf(string alg)
	{
		return alg.ToLower() switch
		{
			"rc4" => rc4, 
			"rc2" => rc2, 
			"aes128" => aes128, 
			"aes192" => aes192, 
			"aes256" => aes256, 
			"des" => des, 
			"des3" => des3, 
			"des3_112" => des3_112, 
			"rsa" => rsa, 
			_ => throw new ArgumentException($"not found definition of cipher algorithm {alg}"), 
		};
	}

	public override string ToString()
	{
		return name;
	}

	public CipherAlgorithm(CipherProvider provider, string jceId, int ecmaId, int defaultKeySize, int[] allowedKeySize, int blockSize, int encryptedVerifierHashLength, string xmlId, bool needsBouncyCastle)
	{
		this.provider = provider;
		this.jceId = jceId;
		this.ecmaId = ecmaId;
		this.defaultKeySize = defaultKeySize;
		this.allowedKeySize = (int[])allowedKeySize.Clone();
		this.blockSize = blockSize;
		this.encryptedVerifierHashLength = encryptedVerifierHashLength;
		this.xmlId = xmlId;
		this.needsBouncyCastle = needsBouncyCastle;
	}

	public static CipherAlgorithm FromEcmaId(int ecmaId)
	{
		CipherAlgorithm[] values = Values;
		foreach (CipherAlgorithm cipherAlgorithm in values)
		{
			if (cipherAlgorithm.ecmaId == ecmaId)
			{
				return cipherAlgorithm;
			}
		}
		throw new EncryptedDocumentException("cipher algorithm " + ecmaId + " not found");
	}

	public static CipherAlgorithm FromXmlId(string xmlId, int keySize)
	{
		CipherAlgorithm[] values = Values;
		foreach (CipherAlgorithm cipherAlgorithm in values)
		{
			if (!cipherAlgorithm.xmlId.Equals(xmlId))
			{
				continue;
			}
			int[] array = cipherAlgorithm.allowedKeySize;
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] == keySize)
				{
					return cipherAlgorithm;
				}
			}
		}
		throw new EncryptedDocumentException("cipher algorithm " + xmlId + "/" + keySize + " not found");
	}
}
