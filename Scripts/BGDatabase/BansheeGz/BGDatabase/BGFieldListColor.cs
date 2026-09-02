using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "listColor", Folder = "List/Unity Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerListColor")]
public class BGFieldListColor : BGFieldCachedStructListA<Color>
{
	public const ushort CodeType = 20;

	public override ushort TypeCode => 20;

	protected override int ValueSize => 16;

	public BGFieldListColor(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldListColor(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(Color value)
	{
		return BGFieldColor.ValueToBytes(value);
	}

	protected override Color ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldColor.ValueFromBytes(segment);
	}

	protected override Color ValueFromBytes(byte[] array, int offset)
	{
		return new Color(BitConverter.ToSingle(array, offset), BitConverter.ToSingle(array, offset + 4), BitConverter.ToSingle(array, offset + 8), BitConverter.ToSingle(array, offset + 12));
	}

	protected override string ValueToString(Color value)
	{
		return BGFieldColor.ValueToString(value);
	}

	protected override Color ValueFromString(string value)
	{
		return BGFieldColor.ValueFromString(value);
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldListColor(meta, id, name);
	}

	protected override bool AreEqual(Color myValue, Color myValue2)
	{
		if (Mathf.Approximately(myValue.r, myValue2.r) && Mathf.Approximately(myValue.g, myValue2.g) && Mathf.Approximately(myValue.b, myValue2.b))
		{
			return Mathf.Approximately(myValue.a, myValue2.a);
		}
		return false;
	}
}
