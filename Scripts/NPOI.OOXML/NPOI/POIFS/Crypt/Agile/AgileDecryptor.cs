using System;
using NPOI.POIFS.FileSystem;
using NPOI.Util;
using Org.BouncyCastle.X509;

namespace NPOI.POIFS.Crypt.Agile;

public class AgileDecryptor : Decryptor
{
	private class AgileCipherInputStream : ChunkedCipherInputStream
	{
		public AgileCipherInputStream(DocumentInputStream stream, long size, IEncryptionInfoBuilder builder, AgileDecryptor decryptor)
			: base(stream, size, 4096, builder, decryptor)
		{
			base.builder = builder;
			base.decryptor = decryptor;
		}

		protected override Cipher InitCipherForBlock(Cipher cipher, int block)
		{
			return AgileDecryptor.InitCipherForBlock(cipher, block, false, builder, decryptor.GetSecretKey(), Cipher.DECRYPT_MODE);
		}
	}

	private long _length = -1L;

	protected internal static byte[] kVerifierInputBlock;

	protected internal static byte[] kHashedVerifierBlock;

	protected internal static byte[] kCryptoKeyBlock;

	protected internal static byte[] kIntegrityKeyBlock;

	protected internal static byte[] kIntegrityValueBlock;

	static AgileDecryptor()
	{
		kVerifierInputBlock = new byte[8] { 254, 167, 210, 118, 59, 75, 158, 121 };
		kHashedVerifierBlock = new byte[8] { 215, 170, 15, 109, 48, 97, 52, 78 };
		kCryptoKeyBlock = new byte[8] { 20, 110, 11, 231, 171, 172, 208, 214 };
		kIntegrityKeyBlock = new byte[8] { 95, 178, 173, 1, 12, 185, 225, 246 };
		kIntegrityValueBlock = new byte[8] { 160, 103, 127, 2, 178, 44, 132, 51 };
	}

	protected internal AgileDecryptor(AgileEncryptionInfoBuilder builder)
		: base(builder)
	{
	}

	public override bool VerifyPassword(string password)
	{
		AgileEncryptionVerifier agileEncryptionVerifier = (AgileEncryptionVerifier)builder.GetVerifier();
		AgileEncryptionHeader agileEncryptionHeader = (AgileEncryptionHeader)builder.GetHeader();
		HashAlgorithm hashAlgorithm = agileEncryptionHeader.HashAlgorithm;
		CipherAlgorithm cipherAlgorithm = agileEncryptionHeader.CipherAlgorithm;
		int blockSize = agileEncryptionHeader.BlockSize;
		int size = agileEncryptionHeader.KeySize / 8;
		byte[] pwHash = CryptoFunctions.HashPassword(password, agileEncryptionVerifier.HashAlgorithm, agileEncryptionVerifier.Salt, agileEncryptionVerifier.SpinCount);
		byte[] input = hashInput(builder, pwHash, kVerifierInputBlock, agileEncryptionVerifier.EncryptedVerifier, Cipher.DECRYPT_MODE);
		SetVerifier(input);
		byte[] b = CryptoFunctions.GetMessageDigest(hashAlgorithm).Digest(input);
		byte[] block = CryptoFunctions.GetBlock0(hashInput(builder, pwHash, kHashedVerifierBlock, agileEncryptionVerifier.EncryptedVerifierHash, Cipher.DECRYPT_MODE), hashAlgorithm.hashSize);
		SecretKeySpec key = new SecretKeySpec(CryptoFunctions.GetBlock0(hashInput(builder, pwHash, kCryptoKeyBlock, agileEncryptionVerifier.EncryptedKey, Cipher.DECRYPT_MODE), size), agileEncryptionVerifier.CipherAlgorithm.jceId);
		byte[] hash = CryptoFunctions.GetCipher(vec: CryptoFunctions.GenerateIv(hashAlgorithm, agileEncryptionHeader.KeySalt, kIntegrityKeyBlock, blockSize), key: key, cipherAlgorithm: cipherAlgorithm, chain: agileEncryptionVerifier.ChainingMode, cipherMode: Cipher.DECRYPT_MODE).DoFinal(agileEncryptionHeader.GetEncryptedHmacKey());
		hash = CryptoFunctions.GetBlock0(hash, hashAlgorithm.hashSize);
		byte[] hash2 = CryptoFunctions.GetCipher(vec: CryptoFunctions.GenerateIv(hashAlgorithm, agileEncryptionHeader.KeySalt, kIntegrityValueBlock, blockSize), key: key, cipherAlgorithm: cipherAlgorithm, chain: agileEncryptionVerifier.ChainingMode, cipherMode: Cipher.DECRYPT_MODE).DoFinal(agileEncryptionHeader.GetEncryptedHmacValue());
		hash2 = CryptoFunctions.GetBlock0(hash2, hashAlgorithm.hashSize);
		if (Arrays.Equals(block, b))
		{
			SetSecretKey(key);
			SetIntegrityHmacKey(hash);
			SetIntegrityHmacValue(hash2);
			return true;
		}
		return false;
	}

	public bool VerifyPassword(KeyPair keyPair, X509Certificate x509)
	{
		AgileEncryptionVerifier agileEncryptionVerifier = (AgileEncryptionVerifier)builder.GetVerifier();
		AgileEncryptionHeader agileEncryptionHeader = (AgileEncryptionHeader)builder.GetHeader();
		HashAlgorithm hashAlgorithm = agileEncryptionHeader.HashAlgorithm;
		CipherAlgorithm cipherAlgorithm = agileEncryptionHeader.CipherAlgorithm;
		int blockSize = agileEncryptionHeader.BlockSize;
		AgileEncryptionVerifier.AgileCertificateEntry agileCertificateEntry = null;
		foreach (AgileEncryptionVerifier.AgileCertificateEntry certificate in agileEncryptionVerifier.GetCertificates())
		{
			if (x509.Equals(certificate.x509))
			{
				agileCertificateEntry = certificate;
				break;
			}
		}
		if (agileCertificateEntry == null)
		{
			return false;
		}
		Cipher instance = Cipher.GetInstance("RSA");
		instance.Init(Cipher.DECRYPT_MODE, keyPair.getPrivate());
		SecretKeySpec key = new SecretKeySpec(instance.DoFinal(agileCertificateEntry.encryptedKey), agileEncryptionVerifier.CipherAlgorithm.jceId);
		CryptoFunctions.Mac mac = CryptoFunctions.GetMac(hashAlgorithm);
		mac.Init(key);
		byte[] b = mac.DoFinal(agileCertificateEntry.x509.GetEncoded());
		byte[] vec = CryptoFunctions.GenerateIv(hashAlgorithm, agileEncryptionHeader.KeySalt, kIntegrityKeyBlock, blockSize);
		byte[] hash = CryptoFunctions.GetCipher(key, cipherAlgorithm, agileEncryptionVerifier.ChainingMode, vec, Cipher.DECRYPT_MODE).DoFinal(agileEncryptionHeader.GetEncryptedHmacKey());
		hash = CryptoFunctions.GetBlock0(hash, hashAlgorithm.hashSize);
		vec = CryptoFunctions.GenerateIv(hashAlgorithm, agileEncryptionHeader.KeySalt, kIntegrityValueBlock, blockSize);
		byte[] hash2 = CryptoFunctions.GetCipher(key, cipherAlgorithm, agileEncryptionVerifier.ChainingMode, vec, Cipher.DECRYPT_MODE).DoFinal(agileEncryptionHeader.GetEncryptedHmacValue());
		hash2 = CryptoFunctions.GetBlock0(hash2, hashAlgorithm.hashSize);
		if (Arrays.Equals(agileCertificateEntry.certVerifier, b))
		{
			SetSecretKey(key);
			SetIntegrityHmacKey(hash);
			SetIntegrityHmacValue(hash2);
			return true;
		}
		return false;
	}

	protected internal static int GetNextBlockSize(int inputLen, int blockSize)
	{
		int i;
		for (i = blockSize; i < inputLen; i += blockSize)
		{
		}
		return i;
	}

	protected internal static byte[] hashInput(IEncryptionInfoBuilder builder, byte[] pwHash, byte[] blockKey, byte[] inputKey, int cipherMode)
	{
		EncryptionVerifier encryptionVerifier = builder.GetVerifier();
		AgileDecryptor obj = (AgileDecryptor)builder.GetDecryptor();
		int keySizeInBytes = obj.GetKeySizeInBytes();
		int blockSizeInBytes = obj.GetBlockSizeInBytes();
		HashAlgorithm hashAlgorithm = encryptionVerifier.HashAlgorithm;
		byte[] salt = encryptionVerifier.Salt;
		Cipher cipher = CryptoFunctions.GetCipher(new SecretKeySpec(CryptoFunctions.GenerateKey(pwHash, hashAlgorithm, blockKey, keySizeInBytes), encryptionVerifier.CipherAlgorithm.jceId), vec: CryptoFunctions.GenerateIv(hashAlgorithm, salt, null, blockSizeInBytes), cipherAlgorithm: encryptionVerifier.CipherAlgorithm, chain: encryptionVerifier.ChainingMode, cipherMode: cipherMode);
		try
		{
			inputKey = CryptoFunctions.GetBlock0(inputKey, GetNextBlockSize(inputKey.Length, blockSizeInBytes));
			return cipher.DoFinal(inputKey);
		}
		catch (Exception cause)
		{
			throw new EncryptedDocumentException(cause);
		}
	}

	public override InputStream GetDataStream(DirectoryNode dir)
	{
		DocumentInputStream documentInputStream = dir.CreateDocumentInputStream(Decryptor.DEFAULT_POIFS_ENTRY);
		_length = documentInputStream.ReadLong();
		new AgileCipherInputStream(documentInputStream, _length, builder, this);
		throw new NotImplementedException("AgileCipherInputStream should be derived from InputStream");
	}

	public override long GetLength()
	{
		if (_length == -1)
		{
			throw new InvalidOperationException("EcmaDecryptor.DataStream was not called");
		}
		return _length;
	}

	protected internal static Cipher InitCipherForBlock(Cipher existing, int block, bool lastChunk, IEncryptionInfoBuilder builder, ISecretKey skey, int encryptionMode)
	{
		EncryptionHeader header = builder.GetHeader();
		if ((existing == null) | lastChunk)
		{
			string padding = (lastChunk ? "PKCS5PAdding" : "NoPAdding");
			existing = CryptoFunctions.GetCipher(skey, header.CipherAlgorithm, header.ChainingMode, header.KeySalt, encryptionMode, padding);
		}
		byte[] array = new byte[4];
		LittleEndian.PutInt(array, 0, block);
		byte[] array2 = CryptoFunctions.GenerateIv(header.HashAlgorithm, header.KeySalt, array, header.BlockSize);
		AlgorithmParameterSpec aps = ((header.CipherAlgorithm != CipherAlgorithm.rc2) ? ((AlgorithmParameterSpec)new IvParameterSpec(array2)) : ((AlgorithmParameterSpec)new RC2ParameterSpec(skey.GetEncoded().Length * 8, array2)));
		existing.Init(encryptionMode, skey, aps);
		return existing;
	}
}
