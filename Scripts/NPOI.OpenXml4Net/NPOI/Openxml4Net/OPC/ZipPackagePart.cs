using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using NPOI.OpenXml4Net.OPC.Internal.Marshallers;

namespace NPOI.OpenXml4Net.OPC;

public class ZipPackagePart : PackagePart
{
	private ZipEntry zipEntry;

	public ZipEntry ZipArchive => zipEntry;

	public override long Size => zipEntry.Size;

	public ZipPackagePart(OPCPackage container, PackagePartName partName, string contentType)
		: base(container, partName, contentType)
	{
	}

	public ZipPackagePart(OPCPackage container, ZipEntry zipEntry, PackagePartName partName, string contentType)
		: base(container, partName, contentType)
	{
		this.zipEntry = zipEntry;
	}

	protected override Stream GetInputStreamImpl()
	{
		return ((ZipPackage)_container).ZipArchive.GetInputStream(zipEntry);
	}

	protected override Stream GetOutputStreamImpl()
	{
		return null;
	}

	public override bool Save(Stream os)
	{
		return new ZipPartMarshaller().Marshall(this, os);
	}

	public override bool Load(Stream ios)
	{
		throw new InvalidOperationException("Method not implemented !");
	}

	public override void Close()
	{
		throw new InvalidOperationException("Method not implemented !");
	}

	public override void Flush()
	{
		throw new InvalidOperationException("Method not implemented !");
	}
}
