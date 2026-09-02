using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "unityFont", Folder = "Unity Asset", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerUnityFont")]
public class BGFieldUnityFont : BGFieldUnityAssetA<Font>
{
	public const ushort CodeType = 50;

	public override ushort TypeCode => 50;

	public BGFieldUnityFont(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldUnityFont(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldUnityFont(meta, id, name);
	}
}
