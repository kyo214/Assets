using System;
using System.Collections;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "calculatedList", Folder = "Calculated", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerCalcList")]
public class BGFieldCalcList : BGFieldCalcA<IList>
{
	public const ushort CodeType = 96;

	public override ushort TypeCode => 96;

	public override BGCalcTypeCode ResultCode => BGCalcTypeCodeRegistry.List;

	public BGFieldCalcList(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected internal BGFieldCalcList(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldCalcList(meta, id, name);
	}
}
