using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "entityReference", Folder = "Unity Scene", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerReferenceToEntityGo", DeprecatedNote = "Use objectReference field instead!")]
public class BGFieldReferenceToEntityGo : BGFieldReferenceSingleA<BGEntityGo>
{
	public const ushort CodeType = 76;

	public override ushort TypeCode => 76;

	public BGFieldReferenceToEntityGo(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected internal BGFieldReferenceToEntityGo(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override BGId IdProvider(BGEntityGo component)
	{
		return component.EntityId;
	}

	protected override BGEntityGo GetById(BGId id)
	{
		BGEntityGo[] array = UnityEngine.Object.FindObjectsOfType<BGEntityGo>();
		foreach (BGEntityGo bGEntityGo in array)
		{
			if (IdProvider(bGEntityGo) == id)
			{
				return bGEntityGo;
			}
		}
		return null;
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldReferenceToEntityGo(meta, id, name);
	}
}
