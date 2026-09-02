using System;
using System.Xml;

namespace NPOI.POIFS.Crypt.Dsig.Facets;

public class Office2010SignatureFacet : SignatureFacet
{
	public override void postSign(XmlDocument document)
	{
		throw new NotImplementedException();
	}
}
