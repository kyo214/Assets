using System.IO;
using NPOI.OpenXml4Net.OPC.Internal.Marshallers;

namespace NPOI.OpenXml4Net.OPC.Internal;

public class MemoryPackagePart : PackagePart
{
	internal MemoryStream data;

	public override long Size
	{
		get
		{
			if (data != null)
			{
				return data.Length;
			}
			return 0L;
		}
	}

	public MemoryPackagePart(OPCPackage pack, PackagePartName partName, string contentType)
		: base(pack, partName, contentType)
	{
	}

	public MemoryPackagePart(OPCPackage pack, PackagePartName partName, string contentType, bool loadRelationships)
		: base(pack, partName, new ContentType(contentType), loadRelationships)
	{
	}

	protected override Stream GetInputStreamImpl()
	{
		if (data == null)
		{
			return new MemoryStream();
		}
		MemoryStream memoryStream = new MemoryStream((int)data.Length);
		data.Position = 0L;
		StreamHelper.CopyStream(data, memoryStream);
		memoryStream.Position = 0L;
		return memoryStream;
	}

	protected override Stream GetOutputStreamImpl()
	{
		return new MemoryPackagePartOutputStream(this);
	}

	public override void Clear()
	{
		data = null;
	}

	public override bool Save(Stream os)
	{
		return new ZipPartMarshaller().Marshall(this, os);
	}

	public override bool Load(Stream ios)
	{
		StreamHelper.CopyStream(ios, data);
		return true;
	}

	public override void Close()
	{
	}

	public override void Flush()
	{
	}
}
