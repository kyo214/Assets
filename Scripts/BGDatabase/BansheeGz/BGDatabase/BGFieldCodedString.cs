using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "programmableString", Folder = "Programmable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerCodedString")]
public class BGFieldCodedString : BGFieldCodedA<string>
{
	public const ushort CodeType = 103;

	public override ushort TypeCode => 103;

	public BGFieldCodedString(BGMetaEntity meta, string name, Type delegateType)
		: base(meta, name, delegateType)
	{
	}

	protected internal BGFieldCodedString(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldCodedString(meta, id, name);
	}
}
