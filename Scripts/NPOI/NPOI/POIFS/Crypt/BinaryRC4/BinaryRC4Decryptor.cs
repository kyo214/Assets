using System;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.BinaryRC4;

public class BinaryRC4Decryptor : Decryptor
{
	private class BinaryRC4CipherInputStream : ChunkedCipherInputStream
	{
		protected override Cipher InitCipherForBlock(Cipher existing, int block)
		{
			return BinaryRC4Decryptor.InitCipherForBlock(existing, block, decryptor.builder, decryptor.GetSecretKey(), Cipher.DECRYPT_MODE);
		}

		public BinaryRC4CipherInputStream(DocumentInputStream stream, long size, Decryptor decryptor)
			: base(stream, size, 512, decryptor.builder, decryptor)
		{
			base.decryptor = decryptor;
		}
	}

	private long _length = -1L;

	protected internal BinaryRC4Decryptor(BinaryRC4EncryptionInfoBuilder builder)
		: base(builder)
	{
	}

	public override bool VerifyPassword(string password)
	{
		EncryptionVerifier encryptionVerifier = builder.GetVerifier();
		ISecretKey skey = GenerateSecretKey(password, encryptionVerifier);
		try
		{
			Cipher cipher = InitCipherForBlock(null, 0, builder, skey, Cipher.DECRYPT_MODE);
			byte[] encryptedVerifier = encryptionVerifier.EncryptedVerifier;
			byte[] array = new byte[encryptedVerifier.Length];
			cipher.Update(encryptedVerifier, 0, encryptedVerifier.Length, array);
			SetVerifier(array);
			byte[] encryptedVerifierHash = encryptionVerifier.EncryptedVerifierHash;
			byte[] b = cipher.DoFinal(encryptedVerifierHash);
			if (Arrays.Equals(CryptoFunctions.GetMessageDigest(encryptionVerifier.HashAlgorithm).Digest(array), b))
			{
				SetSecretKey(skey);
				return true;
			}
		}
		catch (Exception cause)
		{
			throw new EncryptedDocumentException(cause);
		}
		return false;
	}

	protected internal static Cipher InitCipherForBlock(Cipher cipher, int block, IEncryptionInfoBuilder builder, ISecretKey skey, int encryptMode)
	{
		HashAlgorithm hashAlgorithm = builder.GetVerifier().HashAlgorithm;
		byte[] array = new byte[4];
		LittleEndian.PutUInt(array, 0, block);
		ISecretKey key = new SecretKeySpec(CryptoFunctions.GenerateKey(skey.GetEncoded(), hashAlgorithm, array, 16), skey.GetAlgorithm());
		if (cipher == null)
		{
			EncryptionHeader header = builder.GetHeader();
			cipher = CryptoFunctions.GetCipher(key, header.CipherAlgorithm, null, null, encryptMode);
		}
		else
		{
			cipher.Init(encryptMode, key);
		}
		return cipher;
	}

	protected internal static ISecretKey GenerateSecretKey(string password, EncryptionVerifier ver)
	{
		if (password.Length > 255)
		{
			password = password.Substring(0, 255);
		}
		MessageDigest messageDigest = CryptoFunctions.GetMessageDigest(ver.HashAlgorithm);
		byte[] hash = messageDigest.Digest(StringUtil.GetToUnicodeLE(password));
		byte[] salt = ver.Salt;
		messageDigest.Reset();
		for (int i = 0; i < 16; i++)
		{
			messageDigest.Update(hash, 0, 5);
			messageDigest.Update(salt);
		}
		hash = new byte[5];
		Array.Copy(messageDigest.Digest(), 0, hash, 0, 5);
		return new SecretKeySpec(hash, ver.CipherAlgorithm.jceId);
	}

	public override InputStream GetDataStream(DirectoryNode dir)
	{
		DocumentInputStream documentInputStream = dir.CreateDocumentInputStream(Decryptor.DEFAULT_POIFS_ENTRY);
		_length = documentInputStream.ReadLong();
		new BinaryRC4CipherInputStream(documentInputStream, _length, this);
		throw new NotImplementedException("BinaryRC4CipherInputStream should be derived from InputStream");
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
