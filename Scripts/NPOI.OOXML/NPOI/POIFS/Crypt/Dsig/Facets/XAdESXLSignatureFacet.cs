using System;
using System.Collections.Generic;
using System.Xml;

namespace NPOI.POIFS.Crypt.Dsig.Facets;

public class XAdESXLSignatureFacet : SignatureFacet
{
	public XAdESXLSignatureFacet()
	{
		throw new NotImplementedException();
	}

	public override void postSign(XmlDocument document)
	{
		throw new NotImplementedException();
	}

	public static byte[] GetC14nValue(List<XmlNode> nodeList, string c14nAlgoId)
	{
		throw new NotImplementedException();
	}
}
