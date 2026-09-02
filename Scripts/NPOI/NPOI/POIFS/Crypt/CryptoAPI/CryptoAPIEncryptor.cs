using System;
using System.Collections.Generic;
using System.IO;
using NPOI.HPSF;
using NPOI.POIFS.Crypt.Standard;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.CryptoAPI;

public class CryptoAPIEncryptor : Encryptor
{
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
			header.Write(bos);
			verifier.Write(bos);
		}
	}

	private class CipherByteArrayOutputStream : ByteArrayOutputStream
	{
		private CryptoAPIEncryptor encryptor;

		private Cipher cipher;

		private byte[] oneByte = new byte[1];

		public CipherByteArrayOutputStream(CryptoAPIEncryptor encryptor)
		{
			this.encryptor = encryptor;
		}

		public CipherByteArrayOutputStream()
		{
			SetBlock(0);
		}

		public byte[] GetBuf()
		{
			return base.ToArray();
		}

		public void SetSize(long count)
		{
			SetLength(count);
		}

		public void SetBlock(int block)
		{
			cipher = encryptor.InitCipherForBlock(cipher, block);
		}

		public new void Write(int b)
		{
			try
			{
				oneByte[0] = (byte)b;
				cipher.Update(oneByte, 0, 1, oneByte, 0);
				base.Write(oneByte);
			}
			catch (Exception cause)
			{
				throw new EncryptedDocumentException(cause);
			}
		}

		public new void Write(byte[] b, int off, int len)
		{
			try
			{
				cipher.Update(b, off, len, b, off);
				base.Write(b, off, len);
			}
			catch (Exception cause)
			{
				throw new EncryptedDocumentException(cause);
			}
		}
	}

	private CryptoAPIEncryptionInfoBuilder builder;

	protected internal CryptoAPIEncryptor(CryptoAPIEncryptionInfoBuilder builder)
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
		CryptoAPIEncryptionVerifier verifier2 = builder.GetVerifier();
		verifier2.SetSalt(verifierSalt);
		ISecretKey secretKey = CryptoAPIDecryptor.GenerateSecretKey(password, verifier2);
		SetSecretKey(secretKey);
		try
		{
			Cipher cipher = InitCipherForBlock(null, 0);
			byte[] array = new byte[verifier.Length];
			cipher.Update(verifier, 0, verifier.Length, array);
			verifier2.SetEncryptedVerifier(array);
			byte[] block = CryptoFunctions.GetMessageDigest(verifier2.HashAlgorithm).Digest(verifier);
			byte[] encryptedVerifierHash = cipher.DoFinal(block);
			verifier2.SetEncryptedVerifierHash(encryptedVerifierHash);
		}
		catch (Exception cause)
		{
			throw new EncryptedDocumentException("Password Confirmation failed", cause);
		}
	}

	public Cipher InitCipherForBlock(Cipher cipher, int block)
	{
		return CryptoAPIDecryptor.InitCipherForBlock(cipher, block, builder, GetSecretKey(), Cipher.ENCRYPT_MODE);
	}

	public override OutputStream GetDataStream(DirectoryNode dir)
	{
		CipherByteArrayOutputStream cipherByteArrayOutputStream = new CipherByteArrayOutputStream(this);
		byte[] array = new byte[8];
		cipherByteArrayOutputStream.Write(array, 0, 8);
		string[] obj = new string[2] { "\u0005SummaryInformation", "\u0005DocumentSummaryInformation" };
		List<CryptoAPIDecryptor.StreamDescriptorEntry> list = new List<CryptoAPIDecryptor.StreamDescriptorEntry>();
		int num = 0;
		string[] array2 = obj;
		foreach (string text in array2)
		{
			if (dir.HasEntry(text))
			{
				CryptoAPIDecryptor.StreamDescriptorEntry streamDescriptorEntry = new CryptoAPIDecryptor.StreamDescriptorEntry();
				streamDescriptorEntry.block = num;
				streamDescriptorEntry.streamOffset = (int)cipherByteArrayOutputStream.Length;
				streamDescriptorEntry.streamName = text;
				streamDescriptorEntry.flags = CryptoAPIDecryptor.StreamDescriptorEntry.flagStream.SetValue(0, 1);
				streamDescriptorEntry.reserved2 = 0;
				cipherByteArrayOutputStream.SetBlock(num);
				DocumentInputStream documentInputStream = dir.CreateDocumentInputStream(text);
				IOUtils.Copy(documentInputStream, cipherByteArrayOutputStream);
				documentInputStream.Close();
				streamDescriptorEntry.streamSize = (int)(cipherByteArrayOutputStream.Length - streamDescriptorEntry.streamOffset);
				list.Add(streamDescriptorEntry);
				dir.GetEntry(text).Delete();
				num++;
			}
		}
		int num2 = (int)cipherByteArrayOutputStream.Length;
		cipherByteArrayOutputStream.SetBlock(0);
		LittleEndian.PutUInt(array, 0, list.Count);
		cipherByteArrayOutputStream.Write(array, 0, 4);
		foreach (CryptoAPIDecryptor.StreamDescriptorEntry item in list)
		{
			LittleEndian.PutUInt(array, 0, item.streamOffset);
			cipherByteArrayOutputStream.Write(array, 0, 4);
			LittleEndian.PutUInt(array, 0, item.streamSize);
			cipherByteArrayOutputStream.Write(array, 0, 4);
			LittleEndian.PutUShort(array, 0, item.block);
			cipherByteArrayOutputStream.Write(array, 0, 2);
			LittleEndian.PutUByte(array, 0, (short)item.streamName.Length);
			cipherByteArrayOutputStream.Write(array, 0, 1);
			LittleEndian.PutUByte(array, 0, (short)item.flags);
			cipherByteArrayOutputStream.Write(array, 0, 1);
			LittleEndian.PutUInt(array, 0, item.reserved2);
			cipherByteArrayOutputStream.Write(array, 0, 4);
			byte[] toUnicodeLE = StringUtil.GetToUnicodeLE(item.streamName);
			cipherByteArrayOutputStream.Write(toUnicodeLE, 0, toUnicodeLE.Length);
			LittleEndian.PutShort(array, 0, 0);
			cipherByteArrayOutputStream.Write(array, 0, 2);
		}
		int num3 = (int)cipherByteArrayOutputStream.Length;
		int num4 = num3 - num2;
		LittleEndian.PutUInt(array, 0, num2);
		LittleEndian.PutUInt(array, 4, num4);
		cipherByteArrayOutputStream.Reset();
		cipherByteArrayOutputStream.SetBlock(0);
		cipherByteArrayOutputStream.Write(array, 0, 8);
		cipherByteArrayOutputStream.SetSize(num3);
		dir.CreateDocument("EncryptedSummary", new MemoryStream(cipherByteArrayOutputStream.GetBuf(), 0, num3));
		DocumentSummaryInformation documentSummaryInformation = PropertySetFactory.NewDocumentSummaryInformation();
		try
		{
			documentSummaryInformation.Write(dir, "\u0005DocumentSummaryInformation");
		}
		catch (WritingNotSupportedException ex)
		{
			throw new IOException(ex.Message);
		}
		throw new NotImplementedException("CipherByteArrayOutputStream should be derived from OutputStream");
	}

	protected int GetKeySizeInBytes()
	{
		return builder.GetHeader().KeySize / 8;
	}

	protected void CreateEncryptionInfoEntry(DirectoryNode dir)
	{
		DataSpaceMapUtils.AddDefaultDataSpace(dir);
		EncryptionInfo encryptionInfo = builder.GetEncryptionInfo();
		CryptoAPIEncryptionHeader header = builder.GetHeader();
		CryptoAPIEncryptionVerifier verifier = builder.GetVerifier();
		EncryptionRecord @out = new EncryptionRecordInternal(encryptionInfo, header, verifier);
		DataSpaceMapUtils.CreateEncryptionEntry(dir, "EncryptionInfo", @out);
	}
}
