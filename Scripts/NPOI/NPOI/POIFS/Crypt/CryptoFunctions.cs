using System;
using System.Text;
using NPOI.Util;

namespace NPOI.POIFS.Crypt;

public class CryptoFunctions
{
	public class Mac
	{
		internal static Mac GetInstance(string jceHmacId, string v)
		{
			throw new NotImplementedException();
		}

		internal static Mac GetInstance(string jceHmacId)
		{
			throw new NotImplementedException();
		}

		public byte[] DoFinal(object encoded)
		{
			throw new NotImplementedException();
		}

		public byte[] DoFinal()
		{
			throw new NotImplementedException();
		}

		public void Init(ISecretKey secretKey)
		{
			throw new NotImplementedException();
		}

		public void Update(byte[] buf, int v, int readBytes)
		{
			throw new NotImplementedException();
		}
	}

	private static int[] INITIAL_CODE_ARRAY = new int[15]
	{
		57840, 7439, 52380, 33984, 4364, 3600, 61902, 12606, 6258, 57657,
		54287, 34041, 10252, 43370, 20163
	};

	private static byte[] PAD_ARRAY = new byte[15]
	{
		187, 255, 255, 186, 255, 255, 185, 128, 0, 190,
		15, 0, 191, 15, 0
	};

	private static int[][] ENCRYPTION_MATRIX = new int[15][]
	{
		new int[7] { 44796, 19929, 39858, 10053, 20106, 40212, 10761 },
		new int[7] { 31585, 63170, 64933, 60267, 50935, 40399, 11199 },
		new int[7] { 17763, 35526, 1453, 2906, 5812, 11624, 23248 },
		new int[7] { 885, 1770, 3540, 7080, 14160, 28320, 56640 },
		new int[7] { 55369, 41139, 20807, 41614, 21821, 43642, 17621 },
		new int[7] { 28485, 56970, 44341, 19019, 38038, 14605, 29210 },
		new int[7] { 60195, 50791, 40175, 10751, 21502, 43004, 24537 },
		new int[7] { 18387, 36774, 3949, 7898, 15796, 31592, 63184 },
		new int[7] { 47201, 24803, 49606, 37805, 14203, 28406, 56812 },
		new int[7] { 17824, 35648, 1697, 3394, 6788, 13576, 27152 },
		new int[7] { 43601, 17539, 35078, 557, 1114, 2228, 4456 },
		new int[7] { 30388, 60776, 51953, 34243, 7079, 14158, 28316 },
		new int[7] { 14128, 28256, 56512, 43425, 17251, 34502, 7597 },
		new int[7] { 13105, 26210, 52420, 35241, 883, 1766, 3532 },
		new int[7] { 4129, 8258, 16516, 33032, 4657, 9314, 18628 }
	};

	public static byte[] HashPassword(string password, HashAlgorithm hashAlgorithm, byte[] salt, int spinCount)
	{
		return HashPassword(password, hashAlgorithm, salt, spinCount, iteratorFirst: true);
	}

	public static byte[] HashPassword(string password, HashAlgorithm hashAlgorithm, byte[] salt, int spinCount, bool iteratorFirst)
	{
		if (password == null)
		{
			password = Decryptor.DEFAULT_PASSWORD;
		}
		MessageDigest messageDigest = GetMessageDigest(hashAlgorithm);
		messageDigest.Update(salt);
		byte[] array = messageDigest.Digest(StringUtil.GetToUnicodeLE(password));
		byte[] array2 = new byte[4];
		byte[] passwordHash = (iteratorFirst ? array2 : array);
		byte[] passwordHash2 = (iteratorFirst ? array : array2);
		try
		{
			for (int i = 0; i < spinCount; i++)
			{
				LittleEndian.PutInt(array2, 0, i);
				messageDigest.Reset();
				messageDigest.Update(passwordHash);
				messageDigest.Update(passwordHash2);
				messageDigest.Digest(array, 0, array.Length);
			}
			return array;
		}
		catch (Exception cause)
		{
			throw new EncryptedDocumentException("error in password hashing", cause);
		}
	}

	public static byte[] GenerateIv(HashAlgorithm hashAlgorithm, byte[] salt, byte[] blockKey, int blockSize)
	{
		byte[] hash = salt;
		if (blockKey != null)
		{
			MessageDigest messageDigest = GetMessageDigest(hashAlgorithm);
			messageDigest.Update(salt);
			hash = messageDigest.Digest(blockKey);
		}
		return GetBlock36(hash, blockSize);
	}

	public static byte[] GenerateKey(byte[] passwordHash, HashAlgorithm hashAlgorithm, byte[] blockKey, int keySize)
	{
		MessageDigest messageDigest = GetMessageDigest(hashAlgorithm);
		messageDigest.Update(passwordHash);
		return GetBlock36(messageDigest.Digest(blockKey), keySize);
	}

	public static Cipher GetCipher(ISecretKey key, CipherAlgorithm cipherAlgorithm, ChainingMode chain, byte[] vec, int cipherMode)
	{
		return GetCipher(key, cipherAlgorithm, chain, vec, cipherMode, null);
	}

	public static Cipher GetCipher(IKey key, CipherAlgorithm cipherAlgorithm, ChainingMode chain, byte[] vec, int cipherMode, string padding)
	{
		int num = key.GetEncoded().Length;
		if (padding == null)
		{
			padding = "NoPadding";
		}
		try
		{
			if (Cipher.GetMaxAllowedKeyLength(cipherAlgorithm.jceId) < num * 8)
			{
				throw new EncryptedDocumentException("Export Restrictions in place - please install JCE Unlimited Strength Jurisdiction Policy files");
			}
			Cipher instance;
			if (cipherAlgorithm == CipherAlgorithm.rc4)
			{
				instance = Cipher.GetInstance(cipherAlgorithm.jceId);
			}
			else if (cipherAlgorithm.needsBouncyCastle)
			{
				registerBouncyCastle();
				instance = Cipher.GetInstance(cipherAlgorithm.jceId + "/" + chain.jceId + "/" + padding, "BC");
			}
			else
			{
				instance = Cipher.GetInstance(cipherAlgorithm.jceId + "/" + chain.jceId + "/" + padding);
			}
			if (vec == null)
			{
				instance.Init(cipherMode, key);
			}
			else
			{
				AlgorithmParameterSpec aps = ((cipherAlgorithm != CipherAlgorithm.rc2) ? ((AlgorithmParameterSpec)new IvParameterSpec(vec)) : ((AlgorithmParameterSpec)new RC2ParameterSpec(key.GetEncoded().Length * 8, vec)));
				instance.Init(cipherMode, key, aps);
			}
			return instance;
		}
		catch (Exception cause)
		{
			throw new EncryptedDocumentException(cause);
		}
	}

	private static byte[] GetBlock36(byte[] hash, int size)
	{
		return GetBlockX(hash, size, 54);
	}

	public static byte[] GetBlock0(byte[] hash, int size)
	{
		return GetBlockX(hash, size, 0);
	}

	private static byte[] GetBlockX(byte[] hash, int size, byte Fill)
	{
		if (hash.Length == size)
		{
			return hash;
		}
		byte[] array = new byte[size];
		Arrays.Fill(array, Fill);
		Array.Copy(hash, 0, array, 0, Math.Min(array.Length, hash.Length));
		return array;
	}

	public static MessageDigest GetMessageDigest(HashAlgorithm hashAlgorithm)
	{
		try
		{
			if (hashAlgorithm.needsBouncyCastle)
			{
				registerBouncyCastle();
				return MessageDigest.GetInstance(hashAlgorithm.jceId, "BC");
			}
			return MessageDigest.GetInstance(hashAlgorithm.jceId);
		}
		catch (Exception cause)
		{
			throw new EncryptedDocumentException("hash algo not supported", cause);
		}
	}

	public static Mac GetMac(HashAlgorithm hashAlgorithm)
	{
		try
		{
			if (hashAlgorithm.needsBouncyCastle)
			{
				registerBouncyCastle();
				return Mac.GetInstance(hashAlgorithm.jceHmacId, "BC");
			}
			return Mac.GetInstance(hashAlgorithm.jceHmacId);
		}
		catch (Exception cause)
		{
			throw new EncryptedDocumentException("hmac algo not supported", cause);
		}
	}

	[Obsolete("not necessary for npoi")]
	public static void registerBouncyCastle()
	{
	}

	public static int CreateXorVerifier1(string password)
	{
		byte[] array = toAnsiPassword(password);
		short num = 0;
		if (!"".Equals(password))
		{
			for (int num2 = array.Length - 1; num2 >= 0; num2--)
			{
				num = rotateLeftBase15Bit(num);
				num ^= array[num2];
			}
			num = rotateLeftBase15Bit(num);
			num ^= (short)array.Length;
			num ^= -12725;
		}
		return num & 0xFFFF;
	}

	public static int CreateXorVerifier2(string password)
	{
		byte[] data = new byte[4];
		int num = 15;
		if (!"".Equals(password))
		{
			password = password.Substring(0, Math.Min(password.Length, num));
			byte[] array = toAnsiPassword(password);
			int num2 = INITIAL_CODE_ARRAY[array.Length - 1];
			for (int i = 0; i < array.Length; i++)
			{
				int num3 = num - array.Length + i;
				for (int j = 0; j < 7; j++)
				{
					if ((array[i] & (1 << j)) != 0)
					{
						num2 ^= ENCRYPTION_MATRIX[num3][j];
					}
				}
			}
			int num4 = CreateXorVerifier1(password);
			LittleEndian.PutShort(data, 0, (short)num4);
			LittleEndian.PutShort(data, 2, (short)num2);
		}
		return LittleEndian.GetInt(data);
	}

	public static string XorHashPassword(string password)
	{
		int num = CreateXorVerifier2(password);
		return string.Format("%1$08X", num);
	}

	public static string XorHashPasswordReversed(string password)
	{
		int operand = CreateXorVerifier2(password);
		return string.Format("%1$02X%2$02X%3$02X%4$02X", Operator.UnsignedRightShift(operand, 0) & 0xFF, Operator.UnsignedRightShift(operand, 8) & 0xFF, Operator.UnsignedRightShift(operand, 16) & 0xFF, Operator.UnsignedRightShift(operand, 24) & 0xFF);
	}

	public static int CreateXorKey1(string password)
	{
		return Operator.UnsignedRightShift(CreateXorVerifier2(password), 16);
	}

	public static byte[] CreateXorArray1(string password)
	{
		if (password.Length > 15)
		{
			password = password.Substring(0, 15);
		}
		byte[] bytes = Encoding.ASCII.GetBytes(password);
		byte[] array = new byte[16];
		Array.Copy(bytes, 0, array, 0, bytes.Length);
		Array.Copy(PAD_ARRAY, 0, array, bytes.Length, PAD_ARRAY.Length - bytes.Length + 1);
		int num = CreateXorKey1(password);
		int shift = 2;
		int num2 = Operator.UnsignedRightShift(num, 8);
		byte[] array2 = new byte[2]
		{
			(byte)(num & 0xFF),
			(byte)(num2 & 0xFF)
		};
		for (int i = 0; i < array.Length; i++)
		{
			array[i] ^= array2[i & 1];
			array[i] = rotateLeft(array[i], shift);
		}
		return array;
	}

	private static byte[] toAnsiPassword(string password)
	{
		byte[] array = new byte[password.Length];
		for (int i = 0; i < password.Length; i++)
		{
			char num = password[i];
			byte b = (byte)(num & 0xFF);
			byte b2 = (byte)(Operator.UnsignedRightShift(num, 8) & 0xFF);
			array[i] = ((b != 0) ? b : b2);
		}
		return array;
	}

	private static byte rotateLeft(byte bits, int Shift)
	{
		return (byte)(((bits & 0xFF) << Shift) | Operator.UnsignedRightShift(bits & 0xFF, 8 - Shift));
	}

	private static short rotateLeftBase15Bit(short verifier)
	{
		short num = (short)(((verifier & 0x4000) != 0) ? 1 : 0);
		short num2 = (short)((verifier << 1) & 0x7FFF);
		return (short)(num | num2);
	}
}
