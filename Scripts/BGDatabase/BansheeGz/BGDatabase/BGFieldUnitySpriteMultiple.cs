using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "unitySpriteMultiple", Folder = "Unity Asset", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerUnitySpriteMultiple")]
public class BGFieldUnitySpriteMultiple : BGFieldUnityAssetArrayA<Sprite>
{
	public const ushort CodeType = 56;

	public override ushort TypeCode => 56;

	public BGFieldUnitySpriteMultiple(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldUnitySpriteMultiple(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldUnitySpriteMultiple(meta, id, name);
	}
}
