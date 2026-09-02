using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerPartitionMetaReference")]
public class BGFieldPartitionMetaReference : BGFieldMetaReference
{
	public override ushort TypeCode => 0;

	public BGFieldPartitionMetaReference(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldPartitionMetaReference(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldPartitionMetaReference(meta, id, name);
	}
}
