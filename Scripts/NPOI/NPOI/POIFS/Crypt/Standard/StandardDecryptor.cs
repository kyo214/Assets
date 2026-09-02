using System;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.Standard;

public class StandardDecryptor : Decryptor
{
	private long _length = -1L;

	internal StandardDecryptor(IEncryptionInfoBuilder builder)
		: base(builder)
	{
	}

	public override bool VerifyPassword(string password)
	{
		EncryptionVerifier encryptionVerifier = builder.GetVerifier();
		ISecretKey key = GenerateSecretKey(password, encryptionVerifier, GetKeySizeInBytes());
		Cipher cipher = GetCipher(key);
		try
		{
			byte[] encryptedVerifier = encryptionVerifier.EncryptedVerifier;
			byte[] input = cipher.DoFinal(encryptedVerifier);
			SetVerifier(input);
			byte[] array = CryptoFunctions.GetMessageDigest(encryptionVerifier.HashAlgorithm).Digest(input);
			byte[] encryptedVerifierHash = encryptionVerifier.EncryptedVerifierHash;
			byte[] b = Arrays.CopyOf(cipher.DoFinal(encryptedVerifierHash), array.Length);
			if (Arrays.Equals(array, b))
			{
				SetSecretKey(key);
				return true;
			}
			return false;
		}
		catch (Exception cause)
		{
			throw new EncryptedDocumentException(cause);
		}
	}

	protected internal static ISecretKey GenerateSecretKey(string password, EncryptionVerifier ver, int keySize)
	{
		HashAlgorithm hashAlgorithm = ver.HashAlgorithm;
		byte[] passwordHash = CryptoFunctions.HashPassword(password, hashAlgorithm, ver.Salt, ver.SpinCount);
		byte[] array = new byte[4];
		LittleEndian.PutInt(array, 0, 0);
		byte[] hash = CryptoFunctions.GenerateKey(passwordHash, hashAlgorithm, array, hashAlgorithm.hashSize);
		byte[] array2 = FillAndXor(hash, 54);
		byte[] array3 = FillAndXor(hash, 92);
		byte[] array4 = new byte[array2.Length + array3.Length];
		Array.Copy(array2, 0, array4, 0, array2.Length);
		Array.Copy(array3, 0, array4, array2.Length, array3.Length);
		return new SecretKeySpec(Arrays.CopyOf(array4, keySize), ver.CipherAlgorithm.jceId);
	}

	protected static byte[] FillAndXor(byte[] hash, byte FillByte)
	{
		byte[] array = new byte[64];
		Arrays.Fill(array, FillByte);
		for (int i = 0; i < hash.Length; i++)
		{
			array[i] ^= hash[i];
		}
		return CryptoFunctions.GetMessageDigest(HashAlgorithm.sha1).Digest(array);
	}

	private Cipher GetCipher(ISecretKey key)
	{
		EncryptionHeader header = builder.GetHeader();
		ChainingMode chainingMode = header.ChainingMode;
		return CryptoFunctions.GetCipher(key, header.CipherAlgorithm, chainingMode, null, Cipher.DECRYPT_MODE);
	}

	public override InputStream GetDataStream(DirectoryNode dir)
	{
		DocumentInputStream documentInputStream = dir.CreateDocumentInputStream(Encryptor.DEFAULT_POIFS_ENTRY);
		_length = documentInputStream.ReadLong();
		if (GetSecretKey() == null)
		{
			VerifyPassword(null);
		}
		int blockSize = builder.GetHeader().CipherAlgorithm.blockSize;
		long size = (_length / blockSize + 1) * blockSize;
		Cipher cipher = GetCipher(GetSecretKey());
		return new BoundedInputStream(new CipherInputStream(new BoundedInputStream(documentInputStream, size), cipher), _length);
	}

	public override long GetLength()
	{
		if (_length == -1)
		{
			throw new InvalidOperationException("Decryptor.DataStream was not called");
		}
		return _length;
	}
}
