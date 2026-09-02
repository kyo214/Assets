using System;
using System.Xml;

namespace NPOI.POIFS.Crypt.Dsig.Facets;

public class KeyInfoSignatureFacet : SignatureFacet
{
	public override void postSign(XmlDocument document)
	{
		throw new NotImplementedException();
	}
}
