using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "calculatedObject", Folder = "Calculated", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerCalcObject")]
public class BGFieldCalcObject : BGFieldCalcA<object>
{
	public const ushort CodeType = 6;

	public override ushort TypeCode => 6;

	public override BGCalcTypeCode ResultCode => BGCalcTypeCodeRegistry.Object;

	public BGFieldCalcObject(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected internal BGFieldCalcObject(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldCalcObject(meta, id, name);
	}
}
