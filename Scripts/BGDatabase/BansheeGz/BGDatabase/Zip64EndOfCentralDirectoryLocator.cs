using System.IO;

namespace BansheeGz.BGDatabase;

internal struct Zip64EndOfCentralDirectoryLocator
{
	public const uint SignatureConstant = 117853008u;

	public const int SizeOfBlockWithoutSignature = 16;

	public uint NumberOfDiskWithZip64EOCD;

	public ulong OffsetOfZip64EOCD;

	public uint TotalNumberOfDisks;

	public static bool TryReadBlock(BinaryReader reader, out Zip64EndOfCentralDirectoryLocator zip64EOCDLocator)
	{
		zip64EOCDLocator = default;
		if (reader.ReadUInt32() != 117853008)
		{
			return false;
		}
		zip64EOCDLocator.NumberOfDiskWithZip64EOCD = reader.ReadUInt32();
		zip64EOCDLocator.OffsetOfZip64EOCD = reader.ReadUInt64();
		zip64EOCDLocator.TotalNumberOfDisks = reader.ReadUInt32();
		return true;
	}
}
