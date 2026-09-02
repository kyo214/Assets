using System.IO;

namespace BansheeGz.BGDatabase;

internal struct ZipEndOfCentralDirectoryBlock
{
	public const uint SignatureConstant = 101010256u;

	public const int SizeOfBlockWithoutSignature = 18;

	public uint Signature;

	public ushort NumberOfThisDisk;

	public ushort NumberOfTheDiskWithTheStartOfTheCentralDirectory;

	public ushort NumberOfEntriesInTheCentralDirectoryOnThisDisk;

	public ushort NumberOfEntriesInTheCentralDirectory;

	public uint SizeOfCentralDirectory;

	public uint OffsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber;

	public byte[] ArchiveComment;

	public static bool TryReadBlock(BinaryReader reader, out ZipEndOfCentralDirectoryBlock eocdBlock)
	{
		eocdBlock = default;
		if (reader.ReadUInt32() != 101010256)
		{
			return false;
		}
		eocdBlock.Signature = 101010256u;
		eocdBlock.NumberOfThisDisk = reader.ReadUInt16();
		eocdBlock.NumberOfTheDiskWithTheStartOfTheCentralDirectory = reader.ReadUInt16();
		eocdBlock.NumberOfEntriesInTheCentralDirectoryOnThisDisk = reader.ReadUInt16();
		eocdBlock.NumberOfEntriesInTheCentralDirectory = reader.ReadUInt16();
		eocdBlock.SizeOfCentralDirectory = reader.ReadUInt32();
		eocdBlock.OffsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber = reader.ReadUInt32();
		ushort count = reader.ReadUInt16();
		eocdBlock.ArchiveComment = reader.ReadBytes(count);
		return true;
	}
}
