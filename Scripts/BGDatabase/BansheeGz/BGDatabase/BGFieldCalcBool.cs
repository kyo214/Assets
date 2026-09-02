using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "calculatedBool", Folder = "Calculated", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerCalcBool")]
public class BGFieldCalcBool : BGFieldCalcA<bool>
{
	public const ushort CodeType = 3;

	public override ushort TypeCode => 3;

	public override BGCalcTypeCode ResultCode => BGCalcTypeCodeRegistry.Bool;

	public BGFieldCalcBool(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected internal BGFieldCalcBool(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldCalcBool(meta, id, name);
	}
}
