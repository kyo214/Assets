using System.IO;
using System.Runtime.InteropServices;

namespace BansheeGz.BGDatabase;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct ZipLocalFileHeader
{
	public const uint DataDescriptorSignature = 134695760u;

	public const uint SignatureConstant = 67324752u;

	public const int OffsetToCrcFromHeaderStart = 14;

	public const int OffsetToBitFlagFromHeaderStart = 6;

	public const int SizeOfLocalHeader = 30;

	public static bool TrySkipBlock(BinaryReader reader)
	{
		if (reader.ReadUInt32() != 67324752)
		{
			return false;
		}
		if (reader.BaseStream.Length < reader.BaseStream.Position + 22)
		{
			return false;
		}
		reader.BaseStream.Seek(22L, SeekOrigin.Current);
		ushort num = reader.ReadUInt16();
		ushort num2 = reader.ReadUInt16();
		if (reader.BaseStream.Length < reader.BaseStream.Position + num + num2)
		{
			return false;
		}
		reader.BaseStream.Seek(num + num2, SeekOrigin.Current);
		return true;
	}
}
