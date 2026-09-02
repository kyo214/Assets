using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "unityMaterial", Folder = "Unity Asset", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerUnityMaterial")]
public class BGFieldUnityMaterial : BGFieldUnityAssetA<Material>
{
	public const ushort CodeType = 51;

	public override ushort TypeCode => 51;

	public BGFieldUnityMaterial(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldUnityMaterial(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldUnityMaterial(meta, id, name);
	}
}
