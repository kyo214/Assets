using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace NPOI.POIFS.Crypt.Dsig.Facets;

public class EnvelopedSignatureFacet : SignatureFacet
{
	public override void preSign(XmlDocument document, List<Reference> references, List<XmlNode> objects)
	{
		throw new NotImplementedException();
	}
}
