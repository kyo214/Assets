using System;

namespace BansheeGz.BGDatabase;

[MetaDescriptor(Name = "row", ManagerType = "BansheeGz.BGDatabase.Editor.BGMetaManagerRow")]
public class BGMetaRow : BGMetaEntity
{
	public const ushort CodeType = 1;

	public override ushort TypeCode => 1;

	public BGMetaRow(BGRepo repo, string name)
		: base(repo, name)
	{
	}

	internal BGMetaRow(BGRepo repo, BGId id, string name)
		: base(repo, id, name)
	{
	}

	public BGMetaRow Duplicate(string newMetaName, bool copyData)
	{
		return new BGMetaRowDuplication(this, newMetaName, copyData).Execute();
	}

	protected override Func<BGRepo, BGId, string, BGMetaEntity> CreateMetaFactory()
	{
		return (BGRepo repo, BGId id, string name) => new BGMetaRow(repo, id, name);
	}
}
