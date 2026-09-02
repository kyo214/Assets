using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace NPOI.POIFS.Crypt.Dsig.Facets;

public abstract class SignatureFacet : ISignatureConfigurable
{
	public static string XML_NS = "http://www.w3.org/2000/xmlns/";

	public static string XML_DIGSIG_NS = "http://www.w3.org/2000/09/xmldsig#";

	public static string OO_DIGSIG_NS = "http://schemas.openxmlformats.org/package/2006/digital-signature";

	public static string MS_DIGSIG_NS = "http://schemas.microsoft.com/office/2006/digsig";

	public static string XADES_132_NS = "http://uri.etsi.org/01903/v1.3.2#";

	public static string XADES_141_NS = "http://uri.etsi.org/01903/v1.4.1#";

	protected SignatureConfig signatureConfig;

	public void SetSignatureConfig(SignatureConfig signatureConfig)
	{
		this.signatureConfig = signatureConfig;
	}

	public virtual void preSign(XmlDocument document, List<Reference> references, List<XmlNode> objects)
	{
	}

	public virtual void postSign(XmlDocument document)
	{
	}

	protected Transform newTransform(string canonicalizationMethod)
	{
		throw new NotImplementedException();
	}

	protected Reference newReference(string uri, List<Transform> transforms, string type, string id, byte[] digestValue)
	{
		return newReference(uri, transforms, type, id, digestValue, signatureConfig);
	}

	public static Reference newReference(string uri, List<Transform> transforms, string type, string id, byte[] digestValue, SignatureConfig signatureConfig)
	{
		throw new NotImplementedException();
	}

	public static void brokenJvmWorkaround(Reference reference)
	{
		throw new NotImplementedException();
	}
}
