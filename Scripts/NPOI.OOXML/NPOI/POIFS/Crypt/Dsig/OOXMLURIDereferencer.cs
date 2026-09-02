using System;
using System.IO;
using System.Security.Cryptography.Xml;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.OpenXml4Net.OPC;

namespace NPOI.POIFS.Crypt.Dsig;

public class OOXMLURIDereferencer : IURIDereferencer, ISignatureConfigurable
{
	private SignatureConfig signatureConfig;

	private IURIDereferencer baseUriDereferencer;

	public void SetSignatureConfig(SignatureConfig signatureConfig)
	{
		this.signatureConfig = signatureConfig;
	}

	public IData dereference(IURIReference uriReference, SignedXml context)
	{
		if (baseUriDereferencer == null)
		{
			throw new NotImplementedException();
		}
		if (uriReference == null)
		{
			throw new NullReferenceException("URIReference cannot be null");
		}
		if (context == null)
		{
			throw new NullReferenceException("XMLCrytoContext cannot be null");
		}
		Uri uri;
		try
		{
			uri = new Uri(uriReference.getURI());
		}
		catch (UriFormatException innerException)
		{
			throw new Exception("could not URL decode the uri: " + uriReference.getURI(), innerException);
		}
		PackagePart packagePart = FindPart(uri);
		if (packagePart == null)
		{
			throw new NotImplementedException();
		}
		try
		{
			packagePart.GetInputStream();
			if (packagePart.PartName.ToString().EndsWith(".rels"))
			{
				new MemoryStream(new MemoryStream().ToArray());
				throw new NotImplementedException();
			}
		}
		catch (IOException)
		{
			throw new NotImplementedException();
		}
		throw new NotImplementedException();
	}

	private PackagePart FindPart(Uri uri)
	{
		string absolutePath = uri.AbsolutePath;
		if (absolutePath == null || "".Equals(absolutePath))
		{
			return null;
		}
		PackagePartName partName;
		try
		{
			partName = PackagingUriHelper.CreatePartName(absolutePath);
		}
		catch (InvalidFormatException)
		{
			return null;
		}
		return signatureConfig.GetOpcPackage().GetPart(partName);
	}
}
