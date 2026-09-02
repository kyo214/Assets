using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "listFloat", Folder = "List/Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerListFloat")]
public class BGFieldListFloat : BGFieldCachedStructListA<float>
{
	public const ushort CodeType = 15;

	public override ushort TypeCode => 15;

	protected override int ValueSize => 4;

	public BGFieldListFloat(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldListFloat(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(float value)
	{
		return BGFieldFloat.ValueToBytes(value);
	}

	protected override float ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldFloat.ValueFromBytes(segment);
	}

	protected override float ValueFromBytes(byte[] array, int offset)
	{
		return BitConverter.ToSingle(array, offset);
	}

	protected override string ValueToString(float value)
	{
		return BGFieldFloat.ValueToString(value);
	}

	protected override float ValueFromString(string value)
	{
		return BGFieldFloat.ValueFromString(value);
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldListFloat(meta, id, name);
	}

	protected override bool AreEqual(float myValue, float myValue2)
	{
		return Mathf.Approximately(myValue, myValue2);
	}
}
