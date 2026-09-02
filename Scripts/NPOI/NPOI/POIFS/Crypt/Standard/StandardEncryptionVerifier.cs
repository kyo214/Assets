using System;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.Standard;

public class StandardEncryptionVerifier : EncryptionVerifier, EncryptionRecord
{
	private static int SPIN_COUNT = 50000;

	private int verifierHashSize;

	internal StandardEncryptionVerifier(ILittleEndianInput is1, StandardEncryptionHeader header)
	{
		if (is1.ReadInt() != 16)
		{
			throw new Exception("Salt size != 16 !?");
		}
		byte[] array = new byte[16];
		is1.ReadFully(array);
		SetSalt(array);
		byte[] buf = new byte[16];
		is1.ReadFully(buf);
		SetEncryptedVerifier(buf);
		verifierHashSize = is1.ReadInt();
		byte[] buf2 = new byte[header.CipherAlgorithm.encryptedVerifierHashLength];
		is1.ReadFully(buf2);
		SetEncryptedVerifierHash(buf2);
		base.SpinCount = SPIN_COUNT;
		base.CipherAlgorithm = header.CipherAlgorithm;
		base.ChainingMode = header.ChainingMode;
		base.EncryptedKey = null;
		base.HashAlgorithm = header.HashAlgorithm;
	}

	protected internal StandardEncryptionVerifier(CipherAlgorithm cipherAlgorithm, HashAlgorithm hashAlgorithm, int keyBits, int blockSize, ChainingMode chainingMode)
	{
		base.CipherAlgorithm = cipherAlgorithm;
		base.HashAlgorithm = hashAlgorithm;
		base.ChainingMode = chainingMode;
		base.SpinCount = SPIN_COUNT;
		verifierHashSize = hashAlgorithm.hashSize;
	}

	protected internal void SetSalt(byte[] salt)
	{
		if (salt == null || salt.Length != 16)
		{
			throw new EncryptedDocumentException("invalid verifier salt");
		}
		base.Salt = salt;
	}

	protected internal void SetEncryptedVerifier(byte[] encryptedVerifier)
	{
		base.EncryptedVerifier = encryptedVerifier;
	}

	protected internal void SetEncryptedVerifierHash(byte[] encryptedVerifierHash)
	{
		base.EncryptedVerifierHash = encryptedVerifierHash;
	}

	public void Write(LittleEndianByteArrayOutputStream bos)
	{
		byte[] salt = base.Salt;
		bos.WriteInt(salt.Length);
		bos.Write(salt);
		byte[] b = base.EncryptedVerifier;
		bos.Write(b);
		bos.WriteInt(20);
		byte[] b2 = base.EncryptedVerifierHash;
		bos.Write(b2);
	}

	protected int GetVerifierHashSize()
	{
		return verifierHashSize;
	}
}
