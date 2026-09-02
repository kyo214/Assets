using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "unityTexture", Folder = "Unity Asset", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerUnityTexture")]
public class BGFieldUnityTexture : BGFieldUnityAssetA<Texture>
{
	public const ushort CodeType = 57;

	public override ushort TypeCode => 57;

	public BGFieldUnityTexture(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldUnityTexture(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldUnityTexture(meta, id, name);
	}
}
