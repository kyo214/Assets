using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace NPOI.OpenXml4Net.OPC.Internal.Marshallers;

public class ZipPackagePropertiesMarshaller : PackagePropertiesMarshaller
{
	public override bool Marshall(PackagePart part, Stream out1)
	{
		if (!(out1 is ZipOutputStream))
		{
			throw new ArgumentException("ZipOutputStream expected!");
		}
		ZipOutputStream zipOutputStream = (ZipOutputStream)out1;
		ZipEntry entry = new ZipEntry(ZipHelper.GetZipItemNameFromOPCName(part.PartName.URI.ToString()));
		try
		{
			zipOutputStream.PutNextEntry(entry);
			base.Marshall(part, out1);
			StreamHelper.SaveXmlInStream(xmlDoc, out1);
			zipOutputStream.CloseEntry();
		}
		catch (IOException ex)
		{
			throw new OpenXml4NetException(ex.Message, ex);
		}
		catch
		{
			return false;
		}
		return true;
	}
}
