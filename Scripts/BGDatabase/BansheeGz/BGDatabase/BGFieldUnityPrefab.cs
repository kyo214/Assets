using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "unityPrefab", Folder = "Unity Asset", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerUnityPrefab")]
public class BGFieldUnityPrefab : BGFieldUnityAssetA<GameObject>
{
	public const ushort CodeType = 53;

	public override ushort TypeCode => 53;

	public BGFieldUnityPrefab(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldUnityPrefab(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldUnityPrefab(meta, id, name);
	}
}
