using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "listVector2", Folder = "List/Unity Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerListVector2")]
public class BGFieldListVector2 : BGFieldCachedStructListA<Vector2>
{
	public const ushort CodeType = 22;

	public override ushort TypeCode => 22;

	protected override int ValueSize => 8;

	public BGFieldListVector2(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldListVector2(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(Vector2 value)
	{
		return BGFieldVector2.ValueToBytes(value);
	}

	protected override Vector2 ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldVector2.ValueFromBytes(segment);
	}

	protected override Vector2 ValueFromBytes(byte[] array, int offset)
	{
		return new Vector2(BitConverter.ToSingle(array, offset), BitConverter.ToSingle(array, offset + 4));
	}

	protected override string ValueToString(Vector2 value)
	{
		return BGFieldVector2.ValueToString(value);
	}

	protected override Vector2 ValueFromString(string value)
	{
		return BGFieldVector2.ValueFromString(value);
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldListVector2(meta, id, name);
	}

	protected override bool AreEqual(Vector2 myValue, Vector2 myValue2)
	{
		if (Mathf.Approximately(myValue.x, myValue2.x))
		{
			return Mathf.Approximately(myValue.y, myValue2.y);
		}
		return false;
	}
}
