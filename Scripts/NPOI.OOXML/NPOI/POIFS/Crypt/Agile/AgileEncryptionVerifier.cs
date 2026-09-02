using System;
using System.Collections.Generic;
using NPOI.OpenXmlFormats.Encryption;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;

namespace NPOI.POIFS.Crypt.Agile;

public class AgileEncryptionVerifier : EncryptionVerifier
{
	public class AgileCertificateEntry
	{
		internal X509Certificate x509;

		internal byte[] encryptedKey;

		internal byte[] certVerifier;
	}

	private List<AgileCertificateEntry> certList = new List<AgileCertificateEntry>();

	public AgileEncryptionVerifier(string descriptor)
		: this(AgileEncryptionInfoBuilder.ParseDescriptor(descriptor))
	{
	}

	protected internal AgileEncryptionVerifier(EncryptionDocument ed)
	{
		IEnumerator<CT_KeyEncryptor> enumerator = ed.GetEncryption().keyEncryptors.keyEncryptor.GetEnumerator();
		CT_PasswordKeyEncryptor cT_PasswordKeyEncryptor;
		try
		{
			enumerator.MoveNext();
			cT_PasswordKeyEncryptor = enumerator.Current.Item as CT_PasswordKeyEncryptor;
			if (cT_PasswordKeyEncryptor == null)
			{
				throw new NullReferenceException("encryptedKey not Set");
			}
		}
		catch (Exception cause)
		{
			throw new EncryptedDocumentException("Unable to parse keyData", cause);
		}
		int keyBits = (int)cT_PasswordKeyEncryptor.keyBits;
		CipherAlgorithm cipherAlgorithm = CipherAlgorithm.FromXmlId(cT_PasswordKeyEncryptor.cipherAlgorithm.ToString(), keyBits);
		base.CipherAlgorithm = cipherAlgorithm;
		int hashSize = (int)cT_PasswordKeyEncryptor.hashSize;
		HashAlgorithm hashAlgorithm = HashAlgorithm.FromEcmaId(cT_PasswordKeyEncryptor.hashAlgorithm.ToString());
		base.HashAlgorithm = hashAlgorithm;
		if (base.HashAlgorithm.hashSize != hashSize)
		{
			throw new EncryptedDocumentException("Unsupported hash algorithm: " + cT_PasswordKeyEncryptor.hashAlgorithm.ToString() + " @ " + hashSize + " bytes");
		}
		base.SpinCount = (int)cT_PasswordKeyEncryptor.spinCount;
		base.EncryptedVerifier = cT_PasswordKeyEncryptor.encryptedVerifierHashInput;
		base.Salt = cT_PasswordKeyEncryptor.saltValue;
		base.EncryptedKey = cT_PasswordKeyEncryptor.encryptedKeyValue;
		base.EncryptedVerifierHash = cT_PasswordKeyEncryptor.encryptedVerifierHashValue;
		if (cT_PasswordKeyEncryptor.saltSize != (uint)base.Salt.Length)
		{
			throw new EncryptedDocumentException("Invalid salt size");
		}
		switch (cT_PasswordKeyEncryptor.cipherChaining)
		{
		case ST_CipherChaining.ChainingModeCBC:
			base.ChainingMode = ChainingMode.cbc;
			break;
		case ST_CipherChaining.ChainingModeCFB:
			base.ChainingMode = ChainingMode.cfb;
			break;
		default:
			throw new EncryptedDocumentException("Unsupported chaining mode - " + cT_PasswordKeyEncryptor.cipherChaining);
		}
		try
		{
			while (enumerator.MoveNext())
			{
				CT_CertificateKeyEncryptor cT_CertificateKeyEncryptor = enumerator.Current.Item as CT_CertificateKeyEncryptor;
				AgileCertificateEntry item = new AgileCertificateEntry
				{
					certVerifier = cT_CertificateKeyEncryptor.certVerifier,
					encryptedKey = cT_CertificateKeyEncryptor.encryptedKeyValue,
					x509 = new X509Certificate(X509CertificateStructure.GetInstance(cT_CertificateKeyEncryptor.X509Certificate))
				};
				certList.Add(item);
			}
		}
		catch (Exception cause2)
		{
			throw new EncryptedDocumentException("can't parse X509 certificate", cause2);
		}
	}

	public AgileEncryptionVerifier(CipherAlgorithm cipherAlgorithm, HashAlgorithm hashAlgorithm, int keyBits, int blockSize, ChainingMode chainingMode)
	{
		base.CipherAlgorithm = cipherAlgorithm;
		base.HashAlgorithm = hashAlgorithm;
		base.ChainingMode = chainingMode;
		base.SpinCount = 100000;
	}

	protected void SetSalt(byte[] salt)
	{
		if (salt == null || salt.Length != base.CipherAlgorithm.blockSize)
		{
			throw new EncryptedDocumentException("invalid verifier salt");
		}
		base.Salt = salt;
	}

	protected void SetEncryptedVerifier(byte[] encryptedVerifier)
	{
		base.EncryptedVerifier = encryptedVerifier;
	}

	protected void SetEncryptedVerifierHash(byte[] encryptedVerifierHash)
	{
		base.EncryptedVerifierHash = encryptedVerifierHash;
	}

	protected void SetEncryptedKey(byte[] encryptedKey)
	{
		base.EncryptedKey = encryptedKey;
	}

	public void AddCertificate(X509Certificate x509)
	{
		AgileCertificateEntry agileCertificateEntry = new AgileCertificateEntry();
		agileCertificateEntry.x509 = x509;
		certList.Add(agileCertificateEntry);
	}

	public List<AgileCertificateEntry> GetCertificates()
	{
		return certList;
	}
}
