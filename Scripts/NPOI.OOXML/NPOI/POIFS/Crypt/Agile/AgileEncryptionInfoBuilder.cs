using System;
using System.Text;
using System.Xml;
using NPOI.OpenXml4Net.Util;
using NPOI.OpenXmlFormats.Encryption;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.Agile;

public class AgileEncryptionInfoBuilder : IEncryptionInfoBuilder
{
	private EncryptionInfo info;

	private AgileEncryptionHeader header;

	private AgileEncryptionVerifier verifier;

	private AgileDecryptor decryptor;

	private AgileEncryptor encryptor;

	public void Initialize(EncryptionInfo info, ILittleEndianInput dis)
	{
		this.info = info;
		EncryptionDocument ed = ParseDescriptor((DocumentInputStream)dis);
		header = new AgileEncryptionHeader(ed);
		verifier = new AgileEncryptionVerifier(ed);
		if (info.VersionMajor == EncryptionMode.Agile.VersionMajor && info.VersionMinor == EncryptionMode.Agile.VersionMinor)
		{
			decryptor = new AgileDecryptor(this);
			encryptor = new AgileEncryptor(this);
		}
	}

	public void Initialize(EncryptionInfo info, CipherAlgorithm cipherAlgorithm, HashAlgorithm hashAlgorithm, int keyBits, int blockSize, ChainingMode chainingMode)
	{
		this.info = info;
		if (cipherAlgorithm == null)
		{
			cipherAlgorithm = CipherAlgorithm.aes128;
		}
		if (cipherAlgorithm == CipherAlgorithm.rc4)
		{
			throw new EncryptedDocumentException("RC4 must not be used with agile encryption.");
		}
		if (hashAlgorithm == null)
		{
			hashAlgorithm = HashAlgorithm.sha1;
		}
		if (chainingMode == null)
		{
			chainingMode = ChainingMode.cbc;
		}
		if (chainingMode != ChainingMode.cbc && chainingMode != ChainingMode.cfb)
		{
			throw new EncryptedDocumentException("Agile encryption only supports CBC/CFB chaining.");
		}
		if (keyBits == -1)
		{
			keyBits = cipherAlgorithm.defaultKeySize;
		}
		if (blockSize == -1)
		{
			blockSize = cipherAlgorithm.blockSize;
		}
		bool flag = false;
		int[] allowedKeySize = cipherAlgorithm.allowedKeySize;
		foreach (int num in allowedKeySize)
		{
			flag |= num == keyBits;
		}
		if (!flag)
		{
			throw new EncryptedDocumentException("KeySize " + keyBits + " not allowed for Cipher " + cipherAlgorithm.ToString());
		}
		header = new AgileEncryptionHeader(cipherAlgorithm, hashAlgorithm, keyBits, blockSize, chainingMode);
		verifier = new AgileEncryptionVerifier(cipherAlgorithm, hashAlgorithm, keyBits, blockSize, chainingMode);
		decryptor = new AgileDecryptor(this);
		encryptor = new AgileEncryptor(this);
	}

	public AgileEncryptionHeader GetHeader()
	{
		return header;
	}

	public AgileEncryptionVerifier GetVerifier()
	{
		return verifier;
	}

	public AgileDecryptor GetDecryptor()
	{
		return decryptor;
	}

	public AgileEncryptor GetEncryptor()
	{
		return encryptor;
	}

	public EncryptionInfo GetInfo()
	{
		return info;
	}

	public static EncryptionDocument ParseDescriptor(string descriptor)
	{
		try
		{
			return EncryptionDocument.Parse(descriptor);
		}
		catch (XmlException cause)
		{
			throw new EncryptedDocumentException("Unable to parse encryption descriptor", cause);
		}
	}

	protected static EncryptionDocument ParseDescriptor(DocumentInputStream descriptor)
	{
		try
		{
			XmlDocument xmlDoc = new XmlDocument();
			byte[] array = new byte[descriptor.Length - descriptor.Position];
			descriptor.ReadFully(array);
			string xml = Encoding.UTF8.GetString(array);
			XmlHelper.LoadXmlSafe(xmlDoc, xml, Encoding.UTF8);
			return EncryptionDocument.Parse(xmlDoc);
		}
		catch (Exception cause)
		{
			throw new EncryptedDocumentException("Unable to parse encryption descriptor", cause);
		}
	}

	EncryptionHeader IEncryptionInfoBuilder.GetHeader()
	{
		return header;
	}

	EncryptionVerifier IEncryptionInfoBuilder.GetVerifier()
	{
		return verifier;
	}

	Decryptor IEncryptionInfoBuilder.GetDecryptor()
	{
		return decryptor;
	}

	Encryptor IEncryptionInfoBuilder.GetEncryptor()
	{
		return encryptor;
	}
}
