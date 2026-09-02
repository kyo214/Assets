using System.Security.Cryptography.Xml;

namespace NPOI.POIFS.Crypt.Dsig;

public interface IURIDereferencer
{
	IData dereference(IURIReference uriReference, SignedXml context);
}
