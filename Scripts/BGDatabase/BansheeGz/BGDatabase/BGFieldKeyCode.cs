using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "keyCode", Folder = "Unity Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerKeyCode")]
public class BGFieldKeyCode : BGFieldCachedEnumA<KeyCode>
{
	public const ushort CodeType = 63;

	public override ushort TypeCode => 63;

	public BGFieldKeyCode(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldKeyCode(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldKeyCode(meta, id, name);
	}
}
