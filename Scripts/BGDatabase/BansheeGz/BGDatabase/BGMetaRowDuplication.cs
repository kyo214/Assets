using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGMetaRowDuplication
{
	protected readonly BGMetaEntity meta;

	protected readonly bool copyRows;

	protected string metaName;

	public BGMetaEntity cloneMeta;

	protected readonly List<Tuple<BGField, BGField>> fields = new List<Tuple<BGField, BGField>>();

	protected readonly List<Tuple<BGEntity, BGEntity>> rows = new List<Tuple<BGEntity, BGEntity>>();

	protected readonly List<BGMetaRowDuplicationNested> nested = new List<BGMetaRowDuplicationNested>();

	public BGMetaRowDuplication(BGMetaRow meta, string metaName, bool copyRows)
	{
		if (meta.Repo.HasMeta(metaName))
		{
			throw new Exception("Meta with name " + metaName + " already exists");
		}
		if (BGLocalizationUglyHacks.HasLocaleField(meta))
		{
			throw new Exception("This meta has localization field(s), which does not support meta duplication");
		}
		this.meta = meta;
		this.metaName = metaName;
		this.copyRows = copyRows;
	}

	internal BGMetaRowDuplication(BGMetaNested meta, bool copyRows)
	{
		this.meta = meta;
		this.copyRows = copyRows;
	}

	public virtual void CreateCloneMeta()
	{
		cloneMeta = new BGMetaRow(meta.Repo, metaName);
		meta.CopyAttributesTo(cloneMeta);
	}

	public virtual void CreateCloneFields()
	{
		meta.ForEachField((BGField bGField) =>
		{
			if (bGField is BGFieldNested fieldNested)
			{
				nested.Add(new BGMetaRowDuplicationNested(this, fieldNested, copyRows));
			}
			else
			{
				BGField bGField2 = GetCloneField(bGField);
				if (bGField2 == null)
				{
					bGField2 = cloneMeta.GetField(bGField.Name, errorIfNotFound: false);
					if (bGField2 == null)
					{
						bGField2 = bGField.Clone(cloneMeta, cloneMeta.NewFieldId);
					}
				}
				fields.Add(Tuple.Create(bGField, bGField2));
			}
		});
		meta.ForEachKey((BGKey key) =>
		{
			List<BGField> keyFields = new List<BGField>();
			key.ForEachField((BGField bGField) =>
			{
				BGField field3 = cloneMeta.GetField(GetToFieldId(bGField.Id));
				keyFields.Add(field3);
			});
			BGKey bGKey = new BGKey(key.Name, keyFields.ToArray())
			{
				IsUnique = key.IsUnique,
				Comment = key.Comment,
				ControllerType = key.ControllerType
			};
		});
		meta.ForEachIndex((BGIndex index) =>
		{
			BGField field3 = cloneMeta.GetField(GetToFieldId(index.Field.Id));
			BGIndex bGIndex = new BGIndex(index.Name, field3);
		});
		foreach (BGMetaRowDuplicationNested item in nested)
		{
			item.CreateCloneMeta();
			item.CreateCloneFields();
		}
		int num = 0;
		for (int num2 = 0; num2 < meta.CountFields; num2++)
		{
			BGField field = meta.GetField(num2);
			BGField field2 = cloneMeta.GetField(field.Name, errorIfNotFound: false);
			if (field2 != null)
			{
				if (field2.Index != num)
				{
					cloneMeta.SwapFields(field2.Index, num);
				}
				num++;
			}
		}
	}

	protected virtual BGField GetCloneField(BGField field)
	{
		return null;
	}

	public virtual void CreateCloneRows()
	{
		meta.ForEachEntity((BGEntity entity) =>
		{
			rows.Add(Tuple.Create(entity, cloneMeta.NewEntity()));
		});
		foreach (BGMetaRowDuplicationNested item in nested)
		{
			item.CreateCloneRows();
		}
	}

	protected virtual void CopyValues()
	{
		foreach (Tuple<BGField, BGField> field in fields)
		{
			foreach (Tuple<BGEntity, BGEntity> row in rows)
			{
				CopyValue(field, row);
			}
		}
		foreach (BGMetaRowDuplicationNested item in nested)
		{
			item.CopyValues();
		}
	}

	protected virtual void CopyValue(Tuple<BGField, BGField> fieldTuple, Tuple<BGEntity, BGEntity> rowTuple)
	{
		fieldTuple.Item2.CopyValue(fieldTuple.Item1, rowTuple.Item1.Id, rowTuple.Item1.Index, rowTuple.Item2.Id);
	}

	public BGMetaRow Execute()
	{
		CreateCloneMeta();
		CreateCloneFields();
		if (copyRows)
		{
			CreateCloneRows();
			CopyValues();
		}
		return (BGMetaRow)cloneMeta;
	}

	public BGId GetToRowId(BGId fromRowId)
	{
		return GetToId(rows, fromRowId);
	}

	public BGId GetToFieldId(BGId fromFieldId)
	{
		return GetToId(fields, fromFieldId);
	}

	private static BGId GetToId<T>(List<Tuple<T, T>> tuples, BGId fromId) where T : BGObjectI
	{
		foreach (Tuple<T, T> tuple in tuples)
		{
			if (tuple.Item1.Id == fromId)
			{
				return tuple.Item2.Id;
			}
		}
		return BGId.Empty;
	}
}
