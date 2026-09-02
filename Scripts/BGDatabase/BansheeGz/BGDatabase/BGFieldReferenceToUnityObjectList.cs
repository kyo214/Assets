using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "objectListReference", Folder = "Unity Scene", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerReferenceToUnityObjectList")]
public class BGFieldReferenceToUnityObjectList : BGFieldReferenceA<List<BGWithId>>
{
	public const ushort CodeType = 79;

	public override ushort TypeCode => 79;

	public override bool ReadOnly => true;

	public override List<BGWithId> this[int entityIndex]
	{
		get
		{
			BGId storedValue = GetStoredValue(entityIndex);
			if (storedValue == BGId.Empty)
			{
				return null;
			}
			return BGWithId.GetAll(storedValue);
		}
		set
		{
		}
	}

	public BGFieldReferenceToUnityObjectList(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldReferenceToUnityObjectList(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldReferenceToUnityObjectList(meta, id, name);
	}
}
