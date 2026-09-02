using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "calculatedInt", Folder = "Calculated", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerCalcInt")]
public class BGFieldCalcInt : BGFieldCalcA<int>
{
	public const ushort CodeType = 5;

	public override ushort TypeCode => 5;

	public override BGCalcTypeCode ResultCode => BGCalcTypeCodeRegistry.Int;

	public BGFieldCalcInt(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected internal BGFieldCalcInt(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldCalcInt(meta, id, name);
	}
}
