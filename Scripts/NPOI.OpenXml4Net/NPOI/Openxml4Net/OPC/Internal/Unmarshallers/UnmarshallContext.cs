using ICSharpCode.SharpZipLib.Zip;

namespace NPOI.OpenXml4Net.OPC.Internal.Unmarshallers;

public class UnmarshallContext
{
	private OPCPackage _package;

	private PackagePartName partName;

	private ZipEntry zipEntry;

	internal OPCPackage Package
	{
		get
		{
			return _package;
		}
		set
		{
			_package = value;
		}
	}

	internal PackagePartName PartName
	{
		get
		{
			return partName;
		}
		set
		{
			partName = value;
		}
	}

	internal ZipEntry ZipEntry
	{
		get
		{
			return zipEntry;
		}
		set
		{
			zipEntry = value;
		}
	}

	public UnmarshallContext(OPCPackage targetPackage, PackagePartName partName)
	{
		_package = targetPackage;
		this.partName = partName;
	}
}
