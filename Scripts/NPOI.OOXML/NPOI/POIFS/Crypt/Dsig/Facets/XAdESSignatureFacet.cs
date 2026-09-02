using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace NPOI.POIFS.Crypt.Dsig.Facets;

public class XAdESSignatureFacet : SignatureFacet
{
	private static string XADES_TYPE = "http://uri.etsi.org/01903#SignedProperties";

	private Dictionary<string, string> dataObjectFormatMimeTypes = new Dictionary<string, string>();

	public new void preSign(XmlDocument document, List<Reference> references, List<XmlNode> objects)
	{
		throw new NotImplementedException();
	}

	public void AddMimeType(string dsReferenceUri, string mimetype)
	{
		dataObjectFormatMimeTypes.Add(dsReferenceUri, mimetype);
	}

	protected static void insertXChild(XmlNode root, XmlNode child)
	{
		throw new NotImplementedException();
	}
}
