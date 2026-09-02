using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "programmableInt", Folder = "Programmable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerCodedInt")]
public class BGFieldCodedInt : BGFieldCodedA<int>
{
	public const ushort CodeType = 102;

	public override ushort TypeCode => 102;

	public BGFieldCodedInt(BGMetaEntity meta, string name, Type delegateType)
		: base(meta, name, delegateType)
	{
	}

	protected internal BGFieldCodedInt(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldCodedInt(meta, id, name);
	}
}
