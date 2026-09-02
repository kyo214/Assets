using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "programmableBool", Folder = "Programmable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerCodedBool")]
public class BGFieldCodedBool : BGFieldCodedA<bool>
{
	public const ushort CodeType = 100;

	public override ushort TypeCode => 100;

	public BGFieldCodedBool(BGMetaEntity meta, string name, Type delegateType)
		: base(meta, name, delegateType)
	{
	}

	protected internal BGFieldCodedBool(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldCodedBool(meta, id, name);
	}
}
