using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NPOI.OpenXmlFormats.Encryption;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.Agile;

public class AgileEncryptor : Encryptor
{
	private class AgileCipherOutputStream : ChunkedCipherOutputStream
	{
		private ISecretKey skey;

		public AgileCipherOutputStream(DirectoryNode dir, IEncryptionInfoBuilder builder, ISecretKey skey, AgileEncryptor encryptor)
			: base(dir, 4096, builder, encryptor)
		{
			base.builder = builder;
			this.skey = skey;
			base.encryptor = encryptor;
		}

		protected override Cipher InitCipherForBlock(Cipher existing, int block, bool lastChunk)
		{
			return AgileDecryptor.InitCipherForBlock(existing, block, lastChunk, builder, skey, Cipher.ENCRYPT_MODE);
		}

		protected override void CalculateChecksum(FileInfo fileOut, int oleStreamSize)
		{
			((AgileEncryptor)encryptor).UpdateIntegrityHMAC(fileOut, oleStreamSize);
		}

		protected override void CreateEncryptionInfoEntry(DirectoryNode dir, FileInfo tmpFile)
		{
			((AgileEncryptor)encryptor).CreateEncryptionInfoEntry(dir, tmpFile);
		}
	}

	private AgileEncryptionInfoBuilder builder;

	private byte[] integritySalt;

	private byte[] pwHash;

	private CT_KeyEncryptorUri passwordUri;

	private CT_KeyEncryptorUri certificateUri = CT_KeyEncryptorUri.httpschemasmicrosoftcomoffice2006keyEncryptorcertificate;

	protected internal AgileEncryptor(AgileEncryptionInfoBuilder builder)
	{
		this.builder = builder;
	}

	public override void ConfirmPassword(string password)
	{
		Random random = new Random();
		int blockSize = builder.GetHeader().BlockSize;
		int num = builder.GetHeader().KeySize / 8;
		int hashSize = builder.GetHeader().HashAlgorithm.hashSize;
		byte[] array = new byte[blockSize];
		byte[] array2 = new byte[blockSize];
		byte[] array3 = new byte[blockSize];
		byte[] array4 = new byte[num];
		byte[] buffer = new byte[hashSize];
		random.NextBytes(array);
		random.NextBytes(array2);
		random.NextBytes(array3);
		random.NextBytes(array4);
		random.NextBytes(buffer);
		ConfirmPassword(password, array4, array3, array, array2, buffer);
	}

	public override void ConfirmPassword(string password, byte[] keySpec, byte[] keySalt, byte[] verifier, byte[] verifierSalt, byte[] integritySalt)
	{
		AgileEncryptionVerifier verifier2 = builder.GetVerifier();
		verifier2.Salt = verifierSalt;
		AgileEncryptionHeader header = builder.GetHeader();
		header.KeySalt = keySalt;
		HashAlgorithm hashAlgorithm = verifier2.HashAlgorithm;
		int blockSize = header.BlockSize;
		pwHash = CryptoFunctions.HashPassword(password, hashAlgorithm, verifierSalt, verifier2.SpinCount);
		byte[] encryptedVerifier = AgileDecryptor.hashInput(builder, pwHash, AgileDecryptor.kVerifierInputBlock, verifier, Cipher.ENCRYPT_MODE);
		verifier2.EncryptedVerifier = encryptedVerifier;
		byte[] inputKey = CryptoFunctions.GetMessageDigest(hashAlgorithm).Digest(verifier);
		byte[] encryptedVerifierHash = AgileDecryptor.hashInput(builder, pwHash, AgileDecryptor.kHashedVerifierBlock, inputKey, Cipher.ENCRYPT_MODE);
		verifier2.EncryptedVerifierHash = encryptedVerifierHash;
		byte[] encryptedKey = AgileDecryptor.hashInput(builder, pwHash, AgileDecryptor.kCryptoKeyBlock, keySpec, Cipher.ENCRYPT_MODE);
		verifier2.EncryptedKey = encryptedKey;
		ISecretKey key = new SecretKeySpec(keySpec, verifier2.CipherAlgorithm.jceId);
		SetSecretKey(key);
		this.integritySalt = integritySalt;
		try
		{
			byte[] vec = CryptoFunctions.GenerateIv(hashAlgorithm, header.KeySalt, AgileDecryptor.kIntegrityKeyBlock, header.BlockSize);
			Cipher cipher = CryptoFunctions.GetCipher(key, verifier2.CipherAlgorithm, verifier2.ChainingMode, vec, Cipher.ENCRYPT_MODE);
			byte[] block = CryptoFunctions.GetBlock0(integritySalt, AgileDecryptor.GetNextBlockSize(integritySalt.Length, blockSize));
			byte[] encryptedHmacKey = cipher.DoFinal(block);
			header.SetEncryptedHmacKey(encryptedHmacKey);
			cipher = Cipher.GetInstance("RSA");
			foreach (AgileEncryptionVerifier.AgileCertificateEntry certificate in verifier2.GetCertificates())
			{
				cipher.Init(Cipher.ENCRYPT_MODE, certificate.x509.GetPublicKey());
				certificate.encryptedKey = cipher.DoFinal(GetSecretKey().GetEncoded());
				CryptoFunctions.Mac mac = CryptoFunctions.GetMac(hashAlgorithm);
				mac.Init(GetSecretKey());
				certificate.certVerifier = mac.DoFinal(certificate.x509.GetEncoded());
			}
		}
		catch (Exception cause)
		{
			throw new EncryptedDocumentException(cause);
		}
	}

	public override OutputStream GetDataStream(DirectoryNode dir)
	{
		new AgileCipherOutputStream(dir, builder, GetSecretKey(), this);
		throw new NotImplementedException("AgileCipherOutputStream should be derived from OutputStream");
	}

	protected void UpdateIntegrityHMAC(FileInfo tmpFile, int oleStreamSize)
	{
		HashAlgorithm hashAlgorithm = builder.GetVerifier().HashAlgorithm;
		CryptoFunctions.Mac mac = CryptoFunctions.GetMac(hashAlgorithm);
		mac.Init(new SecretKeySpec(integritySalt, hashAlgorithm.jceHmacId));
		byte[] array = new byte[1024];
		LittleEndian.PutLong(array, 0, oleStreamSize);
		mac.Update(array, 0, 8);
		FileStream fileStream = tmpFile.Create();
		try
		{
			int readBytes;
			while ((readBytes = fileStream.Read(array, 0, array.Length)) > 0)
			{
				mac.Update(array, 0, readBytes);
			}
		}
		finally
		{
			fileStream.Close();
		}
		byte[] array2 = mac.DoFinal();
		AgileEncryptionHeader header = builder.GetHeader();
		int blockSize = header.BlockSize;
		byte[] vec = CryptoFunctions.GenerateIv(header.HashAlgorithm, header.KeySalt, AgileDecryptor.kIntegrityValueBlock, blockSize);
		Cipher cipher = CryptoFunctions.GetCipher(GetSecretKey(), header.CipherAlgorithm, header.ChainingMode, vec, Cipher.ENCRYPT_MODE);
		byte[] block = CryptoFunctions.GetBlock0(array2, AgileDecryptor.GetNextBlockSize(array2.Length, blockSize));
		byte[] encryptedHmacValue = cipher.DoFinal(block);
		header.SetEncryptedHmacValue(encryptedHmacValue);
	}

	protected EncryptionDocument CreateEncryptionDocument()
	{
		AgileEncryptionVerifier verifier = builder.GetVerifier();
		AgileEncryptionHeader header = builder.GetHeader();
		EncryptionDocument encryptionDocument = EncryptionDocument.NewInstance();
		CT_Encryption cT_Encryption = encryptionDocument.AddNewEncryption();
		CT_KeyData cT_KeyData = cT_Encryption.AddNewKeyData();
		CT_KeyEncryptors cT_KeyEncryptors = cT_Encryption.AddNewKeyEncryptors();
		CT_KeyEncryptor cT_KeyEncryptor = cT_KeyEncryptors.AddNewKeyEncryptor();
		cT_KeyEncryptor.uri = passwordUri;
		CT_PasswordKeyEncryptor cT_PasswordKeyEncryptor = cT_KeyEncryptor.AddNewEncryptedPasswordKey();
		cT_PasswordKeyEncryptor.spinCount = (uint)verifier.SpinCount;
		cT_KeyData.saltSize = (uint)header.BlockSize;
		cT_PasswordKeyEncryptor.saltSize = (uint)header.BlockSize;
		cT_KeyData.blockSize = (uint)header.BlockSize;
		cT_PasswordKeyEncryptor.blockSize = (uint)header.BlockSize;
		cT_KeyData.keyBits = (uint)header.KeySize;
		cT_PasswordKeyEncryptor.keyBits = (uint)header.KeySize;
		HashAlgorithm hashAlgorithm = header.HashAlgorithm;
		cT_KeyData.hashSize = (uint)hashAlgorithm.hashSize;
		cT_PasswordKeyEncryptor.hashSize = (uint)hashAlgorithm.hashSize;
		ST_CipherAlgorithm? sT_CipherAlgorithm = (ST_CipherAlgorithm?)Enum.Parse(typeof(ST_CipherAlgorithm), header.CipherAlgorithm.xmlId);
		if (!sT_CipherAlgorithm.HasValue)
		{
			throw new EncryptedDocumentException("CipherAlgorithm " + header.CipherAlgorithm?.ToString() + " not supported.");
		}
		cT_KeyData.cipherAlgorithm = sT_CipherAlgorithm.Value;
		cT_PasswordKeyEncryptor.cipherAlgorithm = sT_CipherAlgorithm.Value;
		string jceId = header.ChainingMode.jceId;
		if (!(jceId == "cbc"))
		{
			if (!(jceId == "cfb"))
			{
				throw new EncryptedDocumentException("ChainingMode " + header.ChainingMode?.ToString() + " not supported.");
			}
			cT_KeyData.cipherChaining = ST_CipherChaining.ChainingModeCFB;
			cT_PasswordKeyEncryptor.cipherChaining = ST_CipherChaining.ChainingModeCFB;
		}
		else
		{
			cT_KeyData.cipherChaining = ST_CipherChaining.ChainingModeCBC;
			cT_PasswordKeyEncryptor.cipherChaining = ST_CipherChaining.ChainingModeCBC;
		}
		ST_HashAlgorithm? sT_HashAlgorithm = (ST_HashAlgorithm?)Enum.Parse(typeof(ST_HashAlgorithm), hashAlgorithm.ecmaString);
		if (!sT_HashAlgorithm.HasValue)
		{
			throw new EncryptedDocumentException("HashAlgorithm " + hashAlgorithm?.ToString() + " not supported.");
		}
		cT_KeyData.hashAlgorithm = sT_HashAlgorithm.Value;
		cT_PasswordKeyEncryptor.hashAlgorithm = sT_HashAlgorithm.Value;
		cT_KeyData.saltValue = header.KeySalt;
		cT_PasswordKeyEncryptor.saltValue = verifier.Salt;
		cT_PasswordKeyEncryptor.encryptedVerifierHashInput = verifier.EncryptedVerifier;
		cT_PasswordKeyEncryptor.encryptedVerifierHashValue = verifier.EncryptedVerifierHash;
		cT_PasswordKeyEncryptor.encryptedKeyValue = verifier.EncryptedKey;
		CT_DataIntegrity cT_DataIntegrity = cT_Encryption.AddNewDataIntegrity();
		cT_DataIntegrity.encryptedHmacKey = header.GetEncryptedHmacKey();
		cT_DataIntegrity.encryptedHmacValue = header.GetEncryptedHmacValue();
		foreach (AgileEncryptionVerifier.AgileCertificateEntry certificate in verifier.GetCertificates())
		{
			CT_KeyEncryptor cT_KeyEncryptor2 = cT_KeyEncryptors.AddNewKeyEncryptor();
			cT_KeyEncryptor2.uri = certificateUri;
			CT_CertificateKeyEncryptor cT_CertificateKeyEncryptor = cT_KeyEncryptor2.AddNewEncryptedCertificateKey();
			try
			{
				cT_CertificateKeyEncryptor.X509Certificate = certificate.x509.GetEncoded();
			}
			catch (Exception cause)
			{
				throw new EncryptedDocumentException(cause);
			}
			cT_CertificateKeyEncryptor.encryptedKeyValue = certificate.encryptedKey;
			cT_CertificateKeyEncryptor.certVerifier = certificate.certVerifier;
		}
		return encryptionDocument;
	}

	protected void marshallEncryptionDocument(EncryptionDocument ed, LittleEndianByteArrayOutputStream os)
	{
		new Dictionary<string, string>
		{
			{
				passwordUri.ToString(),
				"p"
			},
			{
				certificateUri.ToString(),
				"c"
			}
		};
		MemoryStream memoryStream = new MemoryStream();
		try
		{
			byte[] bytes = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n");
			memoryStream.Write(bytes, 0, bytes.Length);
			ed.Save(memoryStream);
			os.Write(memoryStream.ToArray());
		}
		catch (IOException cause)
		{
			throw new EncryptedDocumentException("error marshalling encryption info document", cause);
		}
	}

	protected void CreateEncryptionInfoEntry(DirectoryNode dir, FileInfo tmpFile)
	{
		DataSpaceMapUtils.AddDefaultDataSpace(dir);
		builder.GetInfo();
	}
}
