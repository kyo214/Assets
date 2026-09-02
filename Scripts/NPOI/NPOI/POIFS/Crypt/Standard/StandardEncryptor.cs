using System;
using System.IO;
using NPOI.POIFS.EventFileSystem;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.Standard;

public class StandardEncryptor : Encryptor
{
	protected class StandardCipherOutputStream : ByteArrayOutputStream, POIFSWriterListener
	{
		private StandardEncryptor encryptor;

		protected long countBytes;

		protected FileInfo fileOut;

		protected DirectoryNode dir;

		private ByteArrayOutputStream out1;

		private FileStream rawStream;

		protected internal StandardCipherOutputStream(DirectoryNode dir, StandardEncryptor encryptor)
		{
			this.encryptor = encryptor;
			this.dir = dir;
			fileOut = TempFile.CreateTempFile("encrypted_package", "crypt");
			rawStream = new FileStream(fileOut.FullName, FileMode.Open, FileAccess.ReadWrite);
			CipherOutputStream cipherOutputStream = new CipherOutputStream(rawStream, encryptor.GetCipher(encryptor.GetSecretKey(), "PKCS5Padding"));
			out1 = cipherOutputStream;
		}

		public override void Write(byte[] b, int off, int len)
		{
			out1.Write(b, off, len);
			countBytes += len;
		}

		public override void Write(int b)
		{
			out1.Write(b);
			countBytes++;
		}

		public override void Close()
		{
			base.Close();
			WriteToPOIFS();
		}

		private void WriteToPOIFS()
		{
			int size = (int)(fileOut.Length + 8);
			dir.CreateDocument(Encryptor.DEFAULT_POIFS_ENTRY, size, this);
		}

		public void ProcessPOIFSWriterEvent(POIFSWriterEvent event1)
		{
			try
			{
				LittleEndianOutputStream littleEndianOutputStream = new LittleEndianOutputStream(event1.Stream);
				littleEndianOutputStream.WriteLong(countBytes);
				long position = rawStream.Position;
				IOUtils.Copy(rawStream, littleEndianOutputStream.out1);
				rawStream.Position = position;
				fileOut.Delete();
				littleEndianOutputStream.Close();
			}
			catch (IOException cause)
			{
				throw new EncryptedDocumentException(cause);
			}
		}
	}

	private class EncryptionRecordInternal : EncryptionRecord
	{
		private EncryptionInfo info;

		private StandardEncryptionHeader header;

		private StandardEncryptionVerifier verifier;

		public EncryptionRecordInternal(EncryptionInfo info, StandardEncryptionHeader header, StandardEncryptionVerifier verifier)
		{
			this.info = info;
			this.header = header;
			this.verifier = verifier;
		}

		public void Write(LittleEndianByteArrayOutputStream bos)
		{
			bos.WriteShort(info.VersionMajor);
			bos.WriteShort(info.VersionMinor);
			bos.WriteInt(info.EncryptionFlags);
			header.Write(bos);
			verifier.Write(bos);
		}
	}

	private StandardEncryptionInfoBuilder builder;

	protected internal StandardEncryptor(StandardEncryptionInfoBuilder builder)
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
		ConfirmPassword(password, null, null, array, array2, null);
	}

	public override void ConfirmPassword(string password, byte[] keySpec, byte[] keySalt, byte[] verifier, byte[] verifierSalt, byte[] integritySalt)
	{
		StandardEncryptionVerifier verifier2 = builder.GetVerifier();
		verifier2.SetSalt(verifierSalt);
		ISecretKey key = StandardDecryptor.GenerateSecretKey(password, verifier2, GetKeySizeInBytes());
		SetSecretKey(key);
		Cipher cipher = GetCipher(key, null);
		try
		{
			byte[] encryptedVerifier = cipher.DoFinal(verifier);
			byte[] source = CryptoFunctions.GetMessageDigest(verifier2.HashAlgorithm).Digest(verifier);
			int encryptedVerifierHashLength = verifier2.CipherAlgorithm.encryptedVerifierHashLength;
			byte[] encryptedVerifierHash = cipher.DoFinal(Arrays.CopyOf(source, encryptedVerifierHashLength));
			verifier2.SetEncryptedVerifier(encryptedVerifier);
			verifier2.SetEncryptedVerifierHash(encryptedVerifierHash);
		}
		catch (Exception cause)
		{
			throw new EncryptedDocumentException("Password Confirmation failed", cause);
		}
	}

	private Cipher GetCipher(ISecretKey key, string padding)
	{
		EncryptionVerifier verifier = builder.GetVerifier();
		return CryptoFunctions.GetCipher(key, verifier.CipherAlgorithm, verifier.ChainingMode, null, Cipher.ENCRYPT_MODE, padding);
	}

	public override OutputStream GetDataStream(DirectoryNode dir)
	{
		CreateEncryptionInfoEntry(dir);
		DataSpaceMapUtils.AddDefaultDataSpace(dir);
		new StandardCipherOutputStream(dir, this);
		throw new NotImplementedException("StandardCipherOutputStream should be derived from OutputStream");
	}

	protected int GetKeySizeInBytes()
	{
		return builder.GetHeader().KeySize / 8;
	}

	protected internal void CreateEncryptionInfoEntry(DirectoryNode dir)
	{
		EncryptionInfo encryptionInfo = builder.GetEncryptionInfo();
		StandardEncryptionHeader header = builder.GetHeader();
		StandardEncryptionVerifier verifier = builder.GetVerifier();
		EncryptionRecord @out = new EncryptionRecordInternal(encryptionInfo, header, verifier);
		DataSpaceMapUtils.CreateEncryptionEntry(dir, "EncryptionInfo", @out);
	}
}
