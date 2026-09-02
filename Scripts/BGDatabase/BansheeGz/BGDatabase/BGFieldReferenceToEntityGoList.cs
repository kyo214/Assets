using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "entityListReference", Folder = "Unity Scene", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerReferenceToEntityGoList", DeprecatedNote = "Use objectListReference field instead!")]
public class BGFieldReferenceToEntityGoList : BGFieldReferenceListA<BGEntityGo>
{
	public const ushort CodeType = 77;

	public override ushort TypeCode => 77;

	public BGFieldReferenceToEntityGoList(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldReferenceToEntityGoList(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override BGId IdProvider(BGEntityGo component)
	{
		return component.EntityId;
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldReferenceToEntityGoList(meta, id, name);
	}
}
