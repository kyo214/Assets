using System;
using System.Collections.Generic;
using System.Linq;

namespace BansheeGz.BGDatabase;

public static class BGRepoDeltaUtils
{
	public static void ForEachMatchingMeta(BGRepo repo1, BGRepo repo2, Action<BGMetaEntity, BGMetaEntity> action)
	{
		repo1.ForEachMeta((BGMetaEntity meta1) =>
		{
			BGMetaEntity meta2 = repo2.GetMeta(meta1.Id);
			if (meta2 != null)
			{
				action(meta1, meta2);
			}
		});
	}

	public static void ForEachMatchingField(BGMetaEntity meta1, BGMetaEntity meta2, Action<BGField, BGField> action)
	{
		meta1.ForEachField((BGField field) =>
		{
			BGField field2 = meta2.GetField(field.Id, errorIfNotFound: false);
			if (field2 != null && !(field2.GetType() != field.GetType()))
			{
				action(field, field2);
			}
		});
	}

	public static void ForEachMatchingEntity(BGMetaEntity meta1, BGMetaEntity meta2, Action<BGEntity, BGEntity> action)
	{
		int countEntities = meta1.CountEntities;
		for (int i = 0; i < countEntities; i++)
		{
			BGEntity entity = meta1.GetEntity(i);
			BGEntity entity2 = meta2.GetEntity(entity.Id);
			if (entity2 != null)
			{
				action(entity, entity2);
			}
		}
	}

	public static List<BGId> Except(BGMetaEntity meta1, BGMetaEntity meta2)
	{
		List<BGId> first = ToEntityIds(meta1);
		List<BGId> second = ToEntityIds(meta2);
		return first.Except(second).ToList();
	}

	private static List<BGId> ToEntityIds(BGMetaEntity meta)
	{
		int countEntities = meta.CountEntities;
		List<BGId> list = new List<BGId>(countEntities);
		for (int i = 0; i < countEntities; i++)
		{
			list.Add(meta.GetEntity(i).Id);
		}
		return list;
	}

	public static BGMetaEntity CreateMeta(BGRepo repo, BGMetaEntity meta)
	{
		return BGMetaEntity.Create(repo, typeof(BGMetaRow).AssemblyQualifiedName, meta.Id, meta.Name, (string)null, false, (string)null, false, false, false);
	}

	public static BGField CreateField(BGMetaEntity myMeta, BGField field)
	{
		return BGField.Create(myMeta, field.GetType().AssemblyQualifiedName, field.Id, field.Name, field.ConfigToString(), system: false, null, null, required: false);
	}

	public static byte[] ToArray(ArraySegment<byte> arraySegment)
	{
		byte[] array = new byte[arraySegment.Count];
		Array.Copy(arraySegment.Array, arraySegment.Offset, array, 0, arraySegment.Count);
		return array;
	}

	public static bool IsAdded(BGMetaEntity storedMeta, BGId entityId)
	{
		return !storedMeta.HasEntity(entityId);
	}

	public static bool IsChanged(BGEntity e, BGEntity e2)
	{
		if (e == null || e2 == null)
		{
			return false;
		}
		BGMetaEntity meta = e.Meta;
		BGMetaEntity meta2 = e2.Meta;
		bool isChanged = false;
		ForEachMatchingField(meta, meta2, (BGField f, BGField f2) =>
		{
			if (!isChanged)
			{
				isChanged = !f.AreStoredValuesEqual(f2, e.Index, e2.Index);
			}
		});
		return isChanged;
	}

	public static bool IsDeleted(BGMetaEntity meta, BGId entityId)
	{
		return !meta.HasEntity(entityId);
	}
}
