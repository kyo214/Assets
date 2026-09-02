using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "unityAudioClip", Folder = "Unity Asset", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerUnityAudioClip")]
public class BGFieldUnityAudioClip : BGFieldUnityAssetA<AudioClip>
{
	public const ushort CodeType = 49;

	public override ushort TypeCode => 49;

	public BGFieldUnityAudioClip(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldUnityAudioClip(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldUnityAudioClip(meta, id, name);
	}
}
