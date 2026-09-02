using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "calculatedFloat", Folder = "Calculated", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerCalcFloat")]
public class BGFieldCalcFloat : BGFieldCalcA<float>
{
	public const ushort CodeType = 4;

	public override ushort TypeCode => 4;

	public override BGCalcTypeCode ResultCode => BGCalcTypeCodeRegistry.Float;

	public BGFieldCalcFloat(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected internal BGFieldCalcFloat(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldCalcFloat(meta, id, name);
	}
}
