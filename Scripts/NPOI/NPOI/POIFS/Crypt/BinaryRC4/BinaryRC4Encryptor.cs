using System;
using System.IO;
using NPOI.POIFS.Crypt.Standard;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.BinaryRC4;

public class BinaryRC4Encryptor : Encryptor
{
	protected class BinaryRC4CipherOutputStream : ChunkedCipherOutputStream
	{
		protected override Cipher InitCipherForBlock(Cipher cipher, int block, bool lastChunk)
		{
			return BinaryRC4Decryptor.InitCipherForBlock(cipher, block, builder, encryptor.GetSecretKey(), Cipher.ENCRYPT_MODE);
		}

		protected override void CalculateChecksum(FileInfo file, int i)
		{
		}

		protected override void CreateEncryptionInfoEntry(DirectoryNode dir, FileInfo tmpFile)
		{
			((BinaryRC4Encryptor)encryptor).CreateEncryptionInfoEntry(dir);
		}

		public BinaryRC4CipherOutputStream(DirectoryNode dir, BinaryRC4EncryptionInfoBuilder builder, BinaryRC4Encryptor encryptor)
			: base(dir, 512, builder, encryptor)
		{
		}
	}

	private class EncryptionRecordInternal : EncryptionRecord
	{
		private EncryptionInfo info;

		private BinaryRC4EncryptionHeader header;

		private BinaryRC4EncryptionVerifier verifier;

		public EncryptionRecordInternal(EncryptionInfo info, BinaryRC4EncryptionHeader header, BinaryRC4EncryptionVerifier verifier)
		{
			this.info = info;
			this.header = header;
			this.verifier = verifier;
		}

		public void Write(LittleEndianByteArrayOutputStream bos)
		{
			bos.WriteShort(info.VersionMajor);
			bos.WriteShort(info.VersionMinor);
			header.Write(bos);
			verifier.Write(bos);
		}
	}

	private BinaryRC4EncryptionInfoBuilder builder;

	protected internal BinaryRC4Encryptor(BinaryRC4EncryptionInfoBuilder builder)
	{
		this.builder = builder;
	}

	public override void ConfirmPassword(string password)
	{
		Random random = new Random();
		byte[] array = new byte[16];
		byte[] array2 = new byte[16];
		random.NextBytes(array);
		random.NextBytes(array2);
		ConfirmPassword(password, null, null, array2, array, null);
	}

	public override void ConfirmPassword(string password, byte[] keySpec, byte[] keySalt, byte[] verifier, byte[] verifierSalt, byte[] integritySalt)
	{
		BinaryRC4EncryptionVerifier verifier2 = builder.GetVerifier();
		verifier2.SetSalt(verifierSalt);
		ISecretKey skey = BinaryRC4Decryptor.GenerateSecretKey(password, verifier2);
		SetSecretKey(skey);
		try
		{
			Cipher cipher = BinaryRC4Decryptor.InitCipherForBlock(null, 0, builder, skey, Cipher.ENCRYPT_MODE);
			byte[] array = new byte[16];
			cipher.Update(verifier, 0, 16, array);
			verifier2.EncryptedVerifier = array;
			byte[] block = CryptoFunctions.GetMessageDigest(verifier2.HashAlgorithm).Digest(verifier);
			byte[] encryptedVerifierHash = cipher.DoFinal(block);
			verifier2.EncryptedVerifierHash = encryptedVerifierHash;
		}
		catch (Exception cause)
		{
			throw new EncryptedDocumentException("Password Confirmation failed", cause);
		}
	}

	public override OutputStream GetDataStream(DirectoryNode dir)
	{
		_ = new BinaryRC4CipherOutputStream(dir, builder, this).out1;
		throw new NotImplementedException("BinaryRC4CipherOutputStream should be derived from OutputStream");
	}

	protected int GetKeySizeInBytes()
	{
		return builder.GetHeader().KeySize / 8;
	}

	protected internal void CreateEncryptionInfoEntry(DirectoryNode dir)
	{
		DataSpaceMapUtils.AddDefaultDataSpace(dir);
		EncryptionInfo encryptionInfo = builder.GetEncryptionInfo();
		BinaryRC4EncryptionHeader header = builder.GetHeader();
		BinaryRC4EncryptionVerifier verifier = builder.GetVerifier();
		EncryptionRecord @out = new EncryptionRecordInternal(encryptionInfo, header, verifier);
		DataSpaceMapUtils.CreateEncryptionEntry(dir, "EncryptionInfo", @out);
	}
}
