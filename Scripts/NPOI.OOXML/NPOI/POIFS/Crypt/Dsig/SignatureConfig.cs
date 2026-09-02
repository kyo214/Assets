using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using NPOI.OpenXml4Net.OPC;
using NPOI.POIFS.Crypt.Dsig.Facets;
using NPOI.POIFS.Crypt.Dsig.Services;

namespace NPOI.POIFS.Crypt.Dsig;

public class SignatureConfig
{
	private ThreadLocal<OPCPackage> opcPackage = new ThreadLocal<OPCPackage>();

	private List<SignatureFacet> signatureFacets = new List<SignatureFacet>();

	private HashAlgorithm digestAlgo = HashAlgorithm.sha1;

	private DateTime executionTime = DateTime.Now;

	private IPrivateKey key;

	private List<X509Certificate> signingCertificateChain;

	private ISignaturePolicyService signaturePolicyService;

	private IURIDereferencer uriDereferencer;

	private string canonicalizationMethod = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

	private bool includeEntireCertificateChain = true;

	private bool includeIssuerSerial;

	private bool includeKeyValue;

	private ITimeStampService tspService = new TSPTimeStampService();

	private string tspUrl;

	private bool tspOldProtocol;

	private HashAlgorithm tspDigestAlgo;

	private string tspUser;

	private string tspPass;

	private ITimeStampServiceValidator tspValidator;

	private string tspRequestPolicy = "1.3.6.1.4.1.13762.3";

	private string userAgent = "POI XmlSign Service TSP Client";

	private string proxyUrl;

	private IRevocationDataService revocationDataService;

	private HashAlgorithm xadesDigestAlgo;

	private string xadesRole;

	private string xadesSignatureId = "idSignedProperties";

	private bool xadesSignaturePolicyImplied = true;

	private string xadesCanonicalizationMethod = "http://www.w3.org/2001/10/xml-exc-c14n#";

	private bool xadesIssuerNameNoReverseOrder = true;

	private string packageSignatureId = "idPackageSignature";

	private string signatureDescription = "Office OpenXML Document";

	private IEventListener signatureMarshalListener;

	private Dictionary<string, string> namespacePrefixes = new Dictionary<string, string>();

	protected internal void Init(bool onlyValidation)
	{
		if (opcPackage == null)
		{
			throw new EncryptedDocumentException("opcPackage is null");
		}
		if (uriDereferencer == null)
		{
			uriDereferencer = new OOXMLURIDereferencer();
		}
		if (uriDereferencer is ISignatureConfigurable)
		{
			((ISignatureConfigurable)uriDereferencer).SetSignatureConfig(this);
		}
		if (namespacePrefixes.Count == 0)
		{
			namespacePrefixes.Add(SignatureFacet.OO_DIGSIG_NS, "mdssi");
			namespacePrefixes.Add(SignatureFacet.XADES_132_NS, "xd");
		}
		if (onlyValidation)
		{
			return;
		}
		if (signatureMarshalListener == null)
		{
			signatureMarshalListener = new SignatureMarshalListener();
		}
		if (signatureMarshalListener is ISignatureConfigurable)
		{
			((ISignatureConfigurable)signatureMarshalListener).SetSignatureConfig(this);
		}
		if (tspService != null)
		{
			tspService.SetSignatureConfig(this);
		}
		if (signatureFacets.Count == 0)
		{
			AddSignatureFacet(new OOXMLSignatureFacet());
			AddSignatureFacet(new KeyInfoSignatureFacet());
			AddSignatureFacet(new XAdESSignatureFacet());
			AddSignatureFacet(new Office2010SignatureFacet());
		}
		foreach (SignatureFacet signatureFacet in signatureFacets)
		{
			signatureFacet.SetSignatureConfig(this);
		}
	}

	public void AddSignatureFacet(SignatureFacet signatureFacet)
	{
		signatureFacets.Add(signatureFacet);
	}

	public List<SignatureFacet> GetSignatureFacets()
	{
		return signatureFacets;
	}

	public void SetSignatureFacets(List<SignatureFacet> signatureFacets)
	{
		this.signatureFacets = signatureFacets;
	}

	public HashAlgorithm GetDigestAlgo()
	{
		return digestAlgo;
	}

	public void SetDigestAlgo(HashAlgorithm digestAlgo)
	{
		this.digestAlgo = digestAlgo;
	}

	public OPCPackage GetOpcPackage()
	{
		return opcPackage.Value;
	}

	public void SetOpcPackage(OPCPackage opcPackage)
	{
		this.opcPackage.Value = opcPackage;
	}

	public IPrivateKey GetKey()
	{
		return key;
	}

	public void SetKey(IPrivateKey key)
	{
		this.key = key;
	}

	public List<X509Certificate> GetSigningCertificateChain()
	{
		return signingCertificateChain;
	}

	public void SetSigningCertificateChain(List<X509Certificate> signingCertificateChain)
	{
		this.signingCertificateChain = signingCertificateChain;
	}

	public DateTime GetExecutionTime()
	{
		return executionTime;
	}

	public void SetExecutionTime(DateTime executionTime)
	{
		this.executionTime = executionTime;
	}

	public ISignaturePolicyService GetSignaturePolicyService()
	{
		return signaturePolicyService;
	}

	public void SetSignaturePolicyService(ISignaturePolicyService signaturePolicyService)
	{
		this.signaturePolicyService = signaturePolicyService;
	}

	public string GetSignatureDescription()
	{
		return signatureDescription;
	}

	public void SetSignatureDescription(string signatureDescription)
	{
		this.signatureDescription = signatureDescription;
	}

	public string GetCanonicalizationMethod()
	{
		return canonicalizationMethod;
	}

	public void SetCanonicalizationMethod(string canonicalizationMethod)
	{
		this.canonicalizationMethod = canonicalizationMethod;
	}

	public string GetPackageSignatureId()
	{
		return packageSignatureId;
	}

	public void SetPackageSignatureId(string packageSignatureId)
	{
		this.packageSignatureId = nvl(packageSignatureId, "xmldsig-" + Guid.NewGuid().ToString());
	}

	public string GetTspUrl()
	{
		return tspUrl;
	}

	public void SetTspUrl(string tspUrl)
	{
		this.tspUrl = tspUrl;
	}

	public bool IsTspOldProtocol()
	{
		return tspOldProtocol;
	}

	public void SetTspOldProtocol(bool tspOldProtocol)
	{
		this.tspOldProtocol = tspOldProtocol;
	}

	public HashAlgorithm GetTspDigestAlgo()
	{
		return nvl(tspDigestAlgo, digestAlgo);
	}

	public void SetTspDigestAlgo(HashAlgorithm tspDigestAlgo)
	{
		this.tspDigestAlgo = tspDigestAlgo;
	}

	public string GetProxyUrl()
	{
		return proxyUrl;
	}

	public void SetProxyUrl(string proxyUrl)
	{
		this.proxyUrl = proxyUrl;
	}

	public ITimeStampService GetTspService()
	{
		return tspService;
	}

	public void SetTspService(ITimeStampService tspService)
	{
		this.tspService = tspService;
	}

	public string GetTspUser()
	{
		return tspUser;
	}

	public void SetTspUser(string tspUser)
	{
		this.tspUser = tspUser;
	}

	public string GetTspPass()
	{
		return tspPass;
	}

	public void SetTspPass(string tspPass)
	{
		this.tspPass = tspPass;
	}

	public ITimeStampServiceValidator GetTspValidator()
	{
		return tspValidator;
	}

	public void SetTspValidator(ITimeStampServiceValidator tspValidator)
	{
		this.tspValidator = tspValidator;
	}

	public IRevocationDataService GetRevocationDataService()
	{
		return revocationDataService;
	}

	public void SetRevocationDataService(IRevocationDataService revocationDataService)
	{
		this.revocationDataService = revocationDataService;
	}

	public HashAlgorithm GetXadesDigestAlgo()
	{
		return nvl(xadesDigestAlgo, digestAlgo);
	}

	public void SetXadesDigestAlgo(HashAlgorithm xadesDigestAlgo)
	{
		this.xadesDigestAlgo = xadesDigestAlgo;
	}

	public string GetUserAgent()
	{
		return userAgent;
	}

	public void SetUserAgent(string userAgent)
	{
		this.userAgent = userAgent;
	}

	public string GetTspRequestPolicy()
	{
		return tspRequestPolicy;
	}

	public void SetTspRequestPolicy(string tspRequestPolicy)
	{
		this.tspRequestPolicy = tspRequestPolicy;
	}

	public bool IsIncludeEntireCertificateChain()
	{
		return includeEntireCertificateChain;
	}

	public void SetIncludeEntireCertificateChain(bool includeEntireCertificateChain)
	{
		this.includeEntireCertificateChain = includeEntireCertificateChain;
	}

	public bool IsIncludeIssuerSerial()
	{
		return includeIssuerSerial;
	}

	public void SetIncludeIssuerSerial(bool includeIssuerSerial)
	{
		this.includeIssuerSerial = includeIssuerSerial;
	}

	public bool IsIncludeKeyValue()
	{
		return includeKeyValue;
	}

	public void SetIncludeKeyValue(bool includeKeyValue)
	{
		this.includeKeyValue = includeKeyValue;
	}

	public string GetXadesRole()
	{
		return xadesRole;
	}

	public void SetXadesRole(string xadesRole)
	{
		this.xadesRole = xadesRole;
	}

	public string GetXadesSignatureId()
	{
		return nvl(xadesSignatureId, "idSignedProperties");
	}

	public void SetXadesSignatureId(string xadesSignatureId)
	{
		this.xadesSignatureId = xadesSignatureId;
	}

	public bool IsXadesSignaturePolicyImplied()
	{
		return xadesSignaturePolicyImplied;
	}

	public void SetXadesSignaturePolicyImplied(bool xadesSignaturePolicyImplied)
	{
		this.xadesSignaturePolicyImplied = xadesSignaturePolicyImplied;
	}

	public bool IsXadesIssuerNameNoReverseOrder()
	{
		return xadesIssuerNameNoReverseOrder;
	}

	public void SetXadesIssuerNameNoReverseOrder(bool xadesIssuerNameNoReverseOrder)
	{
		this.xadesIssuerNameNoReverseOrder = xadesIssuerNameNoReverseOrder;
	}

	public Dictionary<string, string> GetNamespacePrefixes()
	{
		return namespacePrefixes;
	}

	public void SetNamespacePrefixes(Dictionary<string, string> namespacePrefixes)
	{
		this.namespacePrefixes = namespacePrefixes;
	}

	protected static T nvl<T>(T value, T defaultValue)
	{
		if (value != null)
		{
			return value;
		}
		return defaultValue;
	}

	public byte[] GetHashMagic()
	{
		return GetDigestAlgo().jceId switch
		{
			"sha1" => new byte[13]
			{
				48, 31, 48, 7, 6, 5, 43, 14, 3, 2,
				26, 4, 20
			}, 
			"sha224" => new byte[17]
			{
				48, 43, 48, 11, 6, 9, 96, 134, 72, 1,
				101, 3, 4, 2, 4, 4, 28
			}, 
			"sha256" => new byte[17]
			{
				48, 47, 48, 11, 6, 9, 96, 134, 72, 1,
				101, 3, 4, 2, 1, 4, 32
			}, 
			"sha384" => new byte[17]
			{
				48, 63, 48, 11, 6, 9, 96, 134, 72, 1,
				101, 3, 4, 2, 2, 4, 48
			}, 
			"sha512" => new byte[17]
			{
				48, 79, 48, 11, 6, 9, 96, 134, 72, 1,
				101, 3, 4, 2, 3, 4, 64
			}, 
			"ripemd128" => new byte[13]
			{
				48, 27, 48, 7, 6, 5, 43, 36, 3, 2,
				2, 4, 16
			}, 
			"ripemd160" => new byte[13]
			{
				48, 31, 48, 7, 6, 5, 43, 36, 3, 2,
				1, 4, 20
			}, 
			_ => throw new EncryptedDocumentException("Hash algorithm " + GetDigestAlgo()?.ToString() + " not supported for signing."), 
		};
	}

	public string GetSignatureMethodUri()
	{
		return GetDigestAlgo().jceId switch
		{
			"sha1" => XMLSignature.ALGO_ID_SIGNATURE_RSA_SHA1, 
			"sha224" => XMLSignature.ALGO_ID_SIGNATURE_RSA_SHA224, 
			"sha256" => XMLSignature.ALGO_ID_SIGNATURE_RSA_SHA256, 
			"sha384" => XMLSignature.ALGO_ID_SIGNATURE_RSA_SHA384, 
			"sha512" => XMLSignature.ALGO_ID_SIGNATURE_RSA_SHA512, 
			"ripemd160" => XMLSignature.ALGO_ID_SIGNATURE_RSA_RIPEMD160, 
			_ => throw new EncryptedDocumentException("Hash algorithm " + GetDigestAlgo()?.ToString() + " not supported for signing."), 
		};
	}

	public string GetDigestMethodUri()
	{
		return GetDigestMethodUri(GetDigestAlgo());
	}

	public static string GetDigestMethodUri(HashAlgorithm digestAlgo)
	{
		return digestAlgo.jceId switch
		{
			"sha1" => "http://www.w3.org/2000/09/xmldsig#sha1", 
			"sha224" => "http://www.w3.org/2001/04/xmldsig-more#sha224", 
			"sha256" => "http://www.w3.org/2001/04/xmlenc#sha256", 
			"sha384" => "http://www.w3.org/2001/04/xmldsig-more#sha384", 
			"sha512" => "http://www.w3.org/2001/04/xmlenc#sha512", 
			"ripemd160" => "http://www.w3.org/2001/04/xmlenc#ripemd160", 
			_ => throw new EncryptedDocumentException("Hash algorithm " + digestAlgo?.ToString() + " not supported for signing."), 
		};
	}

	public string GetXadesCanonicalizationMethod()
	{
		return xadesCanonicalizationMethod;
	}

	public void SetXadesCanonicalizationMethod(string xadesCanonicalizationMethod)
	{
		this.xadesCanonicalizationMethod = xadesCanonicalizationMethod;
	}
}
