using System;
using NPOI.OpenXmlFormats.Encryption;

namespace NPOI.POIFS.Crypt.Agile;

public class AgileEncryptionHeader : EncryptionHeader
{
	private byte[] encryptedHmacKey;

	private byte[] encryptedHmacValue;

	public AgileEncryptionHeader(string descriptor)
		: this(AgileEncryptionInfoBuilder.ParseDescriptor(descriptor))
	{
	}

	protected internal AgileEncryptionHeader(EncryptionDocument ed)
	{
		CT_KeyData keyData;
		try
		{
			keyData = ed.GetEncryption().keyData;
			if (keyData == null)
			{
				throw new NullReferenceException("keyData not Set");
			}
		}
		catch (Exception)
		{
			throw new EncryptedDocumentException("Unable to parse keyData");
		}
		base.KeySize = (int)keyData.keyBits;
		base.Flags = 0;
		base.SizeExtra = 0;
		base.CspName = null;
		base.BlockSize = (int)keyData.blockSize;
		int keyBits = (int)keyData.keyBits;
		base.CipherProvider = (base.CipherAlgorithm = CipherAlgorithm.FromXmlId(keyData.cipherAlgorithm.ToString(), keyBits)).provider;
		switch (keyData.cipherChaining)
		{
		case ST_CipherChaining.ChainingModeCBC:
			base.ChainingMode = ChainingMode.cbc;
			break;
		case ST_CipherChaining.ChainingModeCFB:
			base.ChainingMode = ChainingMode.cfb;
			break;
		default:
			throw new EncryptedDocumentException("Unsupported chaining mode - " + keyData.cipherChaining);
		}
		int hashSize = (int)keyData.hashSize;
		HashAlgorithm hashAlgorithm = HashAlgorithm.FromEcmaId(keyData.hashAlgorithm.ToString());
		base.HashAlgorithm = hashAlgorithm;
		if (base.HashAlgorithm.hashSize != hashSize)
		{
			throw new EncryptedDocumentException("Unsupported hash algorithm: " + keyData.hashAlgorithm.ToString() + " @ " + hashSize + " bytes");
		}
		int saltSize = (int)keyData.saltSize;
		SetKeySalt(keyData.saltValue);
		if (base.KeySalt.Length != saltSize)
		{
			throw new EncryptedDocumentException("Invalid salt length");
		}
		CT_DataIntegrity dataIntegrity = ed.GetEncryption().dataIntegrity;
		SetEncryptedHmacKey(dataIntegrity.encryptedHmacKey);
		SetEncryptedHmacValue(dataIntegrity.encryptedHmacValue);
	}

	public AgileEncryptionHeader(CipherAlgorithm algorithm, HashAlgorithm hashAlgorithm, int keyBits, int blockSize, ChainingMode chainingMode)
	{
		base.CipherAlgorithm = algorithm;
		base.HashAlgorithm = hashAlgorithm;
		base.KeySize = keyBits;
		base.BlockSize = blockSize;
		base.ChainingMode = chainingMode;
	}

	protected void SetKeySalt(byte[] salt)
	{
		if (salt == null || salt.Length != base.BlockSize)
		{
			throw new EncryptedDocumentException("invalid verifier salt");
		}
		base.KeySalt = salt;
	}

	public byte[] GetEncryptedHmacKey()
	{
		return encryptedHmacKey;
	}

	public void SetEncryptedHmacKey(byte[] encryptedHmacKey)
	{
		this.encryptedHmacKey = encryptedHmacKey;
	}

	public byte[] GetEncryptedHmacValue()
	{
		return encryptedHmacValue;
	}

	public void SetEncryptedHmacValue(byte[] encryptedHmacValue)
	{
		this.encryptedHmacValue = encryptedHmacValue;
	}
}
