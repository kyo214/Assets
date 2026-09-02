using System;
using System.Xml;

namespace NPOI.POIFS.Crypt.Dsig;

public class SignatureMarshalListener : IEventListener, ISignatureConfigurable
{
	private SignatureConfig signatureConfig;

	public void handleEvent(IEvent e)
	{
		throw new NotImplementedException();
	}

	public void handleElement(XmlElement el)
	{
		throw new NotImplementedException();
	}

	protected void SetPrefix(XmlNode el)
	{
		string text = signatureConfig.GetNamespacePrefixes()[el.NamespaceURI];
		if (text != null && el.Prefix == null)
		{
			el.Prefix = text;
		}
		XmlNodeList childNodes = el.ChildNodes;
		for (int i = 0; i < childNodes.Count; i++)
		{
			SetPrefix(childNodes.Item(i));
		}
	}

	public void SetSignatureConfig(SignatureConfig signatureConfig)
	{
		this.signatureConfig = signatureConfig;
	}
}
