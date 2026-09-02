using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "string", Folder = "Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerString")]
public class BGFieldString : BGFieldStringA
{
	public const ushort CodeType = 34;

	public override ushort TypeCode => 34;

	public BGFieldString(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected internal BGFieldString(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldString(meta, id, name);
	}
}
