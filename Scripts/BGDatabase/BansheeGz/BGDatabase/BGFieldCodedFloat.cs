using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "programmableFloat", Folder = "Programmable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerCodedFloat")]
public class BGFieldCodedFloat : BGFieldCodedA<float>
{
	public const ushort CodeType = 101;

	public override ushort TypeCode => 101;

	public BGFieldCodedFloat(BGMetaEntity meta, string name, Type delegateType)
		: base(meta, name, delegateType)
	{
	}

	protected internal BGFieldCodedFloat(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldCodedFloat(meta, id, name);
	}
}
