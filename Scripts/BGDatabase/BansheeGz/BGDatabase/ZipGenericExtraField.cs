using System.IO;

namespace BansheeGz.BGDatabase;

internal struct ZipGenericExtraField
{
	private const int SizeOfHeader = 4;

	private ushort _tag;

	private ushort _size;

	private byte[] _data;

	public ushort Tag => _tag;

	public ushort Size => _size;

	public byte[] Data => _data;

	public static bool TryReadBlock(BinaryReader reader, long endExtraField, out ZipGenericExtraField field)
	{
		field = default;
		if (endExtraField - reader.BaseStream.Position < 4)
		{
			return false;
		}
		field._tag = reader.ReadUInt16();
		field._size = reader.ReadUInt16();
		if (endExtraField - reader.BaseStream.Position < field._size)
		{
			return false;
		}
		field._data = reader.ReadBytes(field._size);
		return true;
	}
}
