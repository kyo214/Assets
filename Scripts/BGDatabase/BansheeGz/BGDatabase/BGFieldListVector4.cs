using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "listVector4", Folder = "List/Unity Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerListVector4")]
public class BGFieldListVector4 : BGFieldCachedStructListA<Vector4>
{
	public const ushort CodeType = 24;

	public override ushort TypeCode => 24;

	protected override int ValueSize => 16;

	public BGFieldListVector4(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldListVector4(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(Vector4 value)
	{
		return BGFieldVector4.ValueToBytes(value);
	}

	protected override Vector4 ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldVector4.ValueFromBytes(segment);
	}

	protected override Vector4 ValueFromBytes(byte[] array, int offset)
	{
		return new Vector4(BitConverter.ToSingle(array, offset), BitConverter.ToSingle(array, offset + 4), BitConverter.ToSingle(array, offset + 8), BitConverter.ToSingle(array, offset + 12));
	}

	protected override string ValueToString(Vector4 value)
	{
		return BGFieldVector4.ValueToString(value);
	}

	protected override Vector4 ValueFromString(string value)
	{
		return BGFieldVector4.ValueFromString(value);
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldListVector4(meta, id, name);
	}

	protected override bool AreEqual(Vector4 myValue, Vector4 myValue2)
	{
		if (Mathf.Approximately(myValue.x, myValue2.x) && Mathf.Approximately(myValue.y, myValue2.y) && Mathf.Approximately(myValue.z, myValue2.z))
		{
			return Mathf.Approximately(myValue.w, myValue2.w);
		}
		return false;
	}
}
