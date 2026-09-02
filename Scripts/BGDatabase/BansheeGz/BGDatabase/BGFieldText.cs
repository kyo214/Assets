using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "text", Folder = "Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerText")]
public class BGFieldText : BGFieldStringA
{
	public const ushort CodeType = 35;

	public override ushort TypeCode => 35;

	public BGFieldText(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldText(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldText(meta, id, name);
	}
}
