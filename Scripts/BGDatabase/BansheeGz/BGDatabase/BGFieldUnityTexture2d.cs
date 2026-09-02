using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "unityTexture2d", Folder = "Unity Asset", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerUnityTexture2d")]
public class BGFieldUnityTexture2d : BGFieldUnityAssetA<Texture2D>
{
	public const ushort CodeType = 58;

	public override ushort TypeCode => 58;

	public BGFieldUnityTexture2d(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldUnityTexture2d(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldUnityTexture2d(meta, id, name);
	}
}
