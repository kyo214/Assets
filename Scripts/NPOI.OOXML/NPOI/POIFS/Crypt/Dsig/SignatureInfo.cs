using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.Dsig;

public class SignatureInfo : ISignatureConfigurable
{
	public class SignaturePart
	{
		private PackagePart signaturePart;

		private X509Certificate signer;

		private List<X509Certificate> certChain;

		private SignaturePart(PackagePart signaturePart)
		{
			this.signaturePart = signaturePart;
		}

		public PackagePart GetPackagePart()
		{
			return signaturePart;
		}

		public X509Certificate GetSigner()
		{
			return signer;
		}

		public List<X509Certificate> GetCertChain()
		{
			return certChain;
		}

		public bool Validate()
		{
			throw new NotImplementedException();
		}
	}

	private static bool IsInitialized;

	protected internal SignatureConfig signatureConfig;

	public SignatureInfo()
	{
		InitXmlProvider();
	}

	public SignatureConfig GetSignatureConfig()
	{
		throw new NotImplementedException();
	}

	public void SetSignatureConfig(SignatureConfig signatureConfig)
	{
		throw new NotImplementedException();
	}

	public bool VerifySignature()
	{
		throw new NotImplementedException();
	}

	public void ConfirmSignature()
	{
		DocumentHelper.CreateDocument();
		throw new NotImplementedException();
	}

	public byte[] signDigest(byte[] digest)
	{
		CryptoFunctions.GetCipher(signatureConfig.GetKey(), CipherAlgorithm.rsa, ChainingMode.ecb, null, Cipher.ENCRYPT_MODE, "PKCS1PAdding");
		throw new NotImplementedException();
	}

	public IEnumerable<SignaturePart> GetSignatureParts()
	{
		signatureConfig.Init(onlyValidation: true);
		throw new NotImplementedException();
	}

	protected static void InitXmlProvider()
	{
		throw new NotImplementedException();
	}

	public DigestInfo preSign(XmlDocument document, List<DigestInfo> digestInfos)
	{
		signatureConfig.Init(onlyValidation: false);
		throw new NotImplementedException();
	}

	public void postSign(XmlDocument document, byte[] signatureValue)
	{
		throw new NotImplementedException();
	}

	protected void WriteDocument(XmlDocument document)
	{
	}

	private static List<T> safe<T>(List<T> other)
	{
		throw new NotImplementedException();
	}
}
