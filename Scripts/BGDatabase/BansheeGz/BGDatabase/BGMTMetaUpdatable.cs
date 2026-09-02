using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGMTMetaUpdatable : BGMTMeta
{
	private bool[] updatedField;

	private bool fieldListsAreReplaced;

	private bool entitiesListsAreReplaced;

	private HashSet<int> deletedEntities;

	public HashSet<int> DeletedEntities => deletedEntities;

	internal BGMTMetaUpdatable(BGMTMeta meta)
		: base(meta)
	{
		updatedField = new bool[fields.Length];
	}

	protected internal override void Set<T>(int fieldIndex, int entityIndex, T value)
	{
		ReplaceField((BGMTField<T>)GetField(fieldIndex))[entityIndex] = value;
	}

	protected internal override void Delete(int entityIndex)
	{
		if (deletedEntities == null)
		{
			deletedEntities = new HashSet<int>();
		}
		deletedEntities.Add(entityIndex);
	}

	protected internal override bool IsDeleted(int entityIndex)
	{
		if (deletedEntities != null)
		{
			return deletedEntities.Contains(entityIndex);
		}
		return false;
	}

	protected internal override void ApplyDelete()
	{
		if (deletedEntities == null)
		{
			return;
		}
		ReplaceFieldLists();
		ReplaceEntitiesLists();
		int[] array = new int[deletedEntities.Count];
		deletedEntities.CopyTo(array);
		Array.Sort(array);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (int num4 in array)
		{
			if (num + num2 == num4)
			{
				num2++;
				continue;
			}
			if (num2 > 0)
			{
				RemoveRange(num - num3, num2);
				num3 += num2;
			}
			num = num4;
			num2 = 1;
		}
		if (num2 > 0)
		{
			RemoveRange(num - num3, num2);
		}
	}

	private void RemoveRange(int from, int numberToRemove)
	{
		int num = from + numberToRemove;
		for (int i = from; i < num; i++)
		{
			entityId2Index.Remove(entityIds[i]);
		}
		int count = entityIds.Count;
		for (int j = num; j < count; j++)
		{
			entityId2Index[entityIds[j]] = j - numberToRemove;
		}
		entityIds.RemoveRange(from, numberToRemove);
		for (int k = 0; k < fields.Length; k++)
		{
			ReplaceField(fields[k]).RemoveRange(from, numberToRemove);
		}
	}

	protected internal override void Dispose()
	{
		base.Dispose();
		updatedField = null;
		deletedEntities = null;
	}

	public override int NewEntities(int numberOfEntities = 1)
	{
		if (numberOfEntities < 1)
		{
			throw new BGException("Number of entities can not be zero or negative");
		}
		ReplaceEntitiesLists();
		int count = entityIds.Count;
		for (int i = 0; i < numberOfEntities; i++)
		{
			BGId newId = BGId.NewId;
			entityId2Index.Add(newId, count + i);
			entityIds.Add(newId);
		}
		for (int j = 0; j < fields.Length; j++)
		{
			BGMTField bGMTField = ReplaceField(fields[j]);
			bGMTField.ResizeTo(count + numberOfEntities);
		}
		return count;
	}

	private BGMTField<T> ReplaceField<T>(BGMTField<T> existingField)
	{
		return (BGMTField<T>)ReplaceField((BGMTField)existingField);
	}

	private BGMTField ReplaceField(BGMTField existingField)
	{
		if (updatedField[existingField.Index])
		{
			return existingField;
		}
		ReplaceFieldLists();
		BGMTField bGMTField = existingField.DeepClone(this);
		updatedField[bGMTField.Index] = true;
		fields[bGMTField.Index] = bGMTField;
		id2Field[bGMTField.Id] = bGMTField;
		name2Field[bGMTField.Name] = bGMTField;
		return bGMTField;
	}

	private void ReplaceFieldLists()
	{
		if (!fieldListsAreReplaced)
		{
			fieldListsAreReplaced = true;
			fields = CloneArray(fields);
			id2Field = new BGIdDictionary<BGMTField>(id2Field);
			name2Field = new Dictionary<string, BGMTField>(name2Field);
		}
	}

	private void ReplaceEntitiesLists()
	{
		if (!entitiesListsAreReplaced)
		{
			entitiesListsAreReplaced = true;
			entityIds = new List<BGId>(entityIds);
			entityId2Index = new BGIdDictionary<int>(entityId2Index);
		}
	}

	private T[] CloneArray<T>(T[] source)
	{
		return CloneArray(source, source.Length);
	}

	private T[] CloneArray<T>(T[] source, int newSize)
	{
		T[] array = new T[newSize];
		Array.Copy(source, array, source.Length);
		return array;
	}
}
