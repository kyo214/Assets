using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "objectReference", Folder = "Unity Scene", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerReferenceToUnityObject")]
public class BGFieldReferenceToUnityObject : BGFieldReferenceSingleA<BGWithId>
{
	public const ushort CodeType = 78;

	public override ushort TypeCode => 78;

	public BGFieldReferenceToUnityObject(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected internal BGFieldReferenceToUnityObject(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override BGId IdProvider(BGWithId component)
	{
		return component.Id;
	}

	protected override BGWithId GetById(BGId id)
	{
		return BGWithId.Get(id);
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldReferenceToUnityObject(meta, id, name);
	}
}
