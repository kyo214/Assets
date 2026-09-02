using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "calculatedString", Folder = "Calculated", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerCalcString")]
public class BGFieldCalcString : BGFieldCalcA<string>
{
	public const ushort CodeType = 7;

	public override ushort TypeCode => 7;

	public override BGCalcTypeCode ResultCode => BGCalcTypeCodeRegistry.String;

	public BGFieldCalcString(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected internal BGFieldCalcString(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldCalcString(meta, id, name);
	}
}
