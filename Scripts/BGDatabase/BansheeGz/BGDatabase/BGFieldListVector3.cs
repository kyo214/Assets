using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "listVector3", Folder = "List/Unity Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerListVector3")]
public class BGFieldListVector3 : BGFieldCachedStructListA<Vector3>
{
	public const ushort CodeType = 23;

	public override ushort TypeCode => 23;

	protected override int ValueSize => 12;

	public BGFieldListVector3(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldListVector3(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(Vector3 value)
	{
		return BGFieldVector3.ValueToBytes(value);
	}

	protected override Vector3 ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldVector3.ValueFromBytes(segment);
	}

	protected override Vector3 ValueFromBytes(byte[] array, int offset)
	{
		return new Vector3(BitConverter.ToSingle(array, offset), BitConverter.ToSingle(array, offset + 4), BitConverter.ToSingle(array, offset + 8));
	}

	protected override string ValueToString(Vector3 value)
	{
		return BGFieldVector3.ValueToString(value);
	}

	protected override Vector3 ValueFromString(string value)
	{
		return BGFieldVector3.ValueFromString(value);
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldListVector3(meta, id, name);
	}

	protected override bool AreEqual(Vector3 myValue, Vector3 myValue2)
	{
		if (Mathf.Approximately(myValue.x, myValue2.x) && Mathf.Approximately(myValue.y, myValue2.y))
		{
			return Mathf.Approximately(myValue.z, myValue2.z);
		}
		return false;
	}
}
