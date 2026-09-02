using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "listQuaternion", Folder = "List/Unity Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerListQuaternion")]
public class BGFieldListQuaternion : BGFieldCachedStructListA<Quaternion>
{
	public const ushort CodeType = 21;

	public override ushort TypeCode => 21;

	protected override int ValueSize => 16;

	public BGFieldListQuaternion(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldListQuaternion(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(Quaternion value)
	{
		return BGFieldQuaternion.ValueToBytes(value);
	}

	protected override Quaternion ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldQuaternion.ValueFromBytes(segment);
	}

	protected override Quaternion ValueFromBytes(byte[] array, int offset)
	{
		return new Quaternion(BitConverter.ToSingle(array, offset), BitConverter.ToSingle(array, offset + 4), BitConverter.ToSingle(array, offset + 8), BitConverter.ToSingle(array, offset + 12));
	}

	protected override string ValueToString(Quaternion value)
	{
		return BGFieldQuaternion.ValueToString(value);
	}

	protected override Quaternion ValueFromString(string value)
	{
		return BGFieldQuaternion.ValueFromString(value);
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldListQuaternion(meta, id, name);
	}

	protected override bool AreEqual(Quaternion myValue, Quaternion myValue2)
	{
		if (Mathf.Approximately(myValue.x, myValue2.x) && Mathf.Approximately(myValue.y, myValue2.y) && Mathf.Approximately(myValue.z, myValue2.z))
		{
			return Mathf.Approximately(myValue.w, myValue2.w);
		}
		return false;
	}
}
