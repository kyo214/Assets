using System;

namespace BansheeGz.BGDatabase;

public class BGMetaRowDuplicationNested : BGMetaRowDuplication
{
	private readonly BGMetaRowDuplication parent;

	private readonly BGFieldNested fieldNested;

	private BGFieldNested cloneField;

	public BGMetaRowDuplicationNested(BGMetaRowDuplication parent, BGFieldNested fieldNested, bool copyRows)
		: base((BGMetaNested)fieldNested.RelatedMeta, copyRows)
	{
		this.parent = parent;
		this.fieldNested = fieldNested;
	}

	private static string GetNestedMetaName(BGMetaRowDuplication parent, BGFieldNested fieldNested)
	{
		return BGUtil.DuplicateMetaName(fieldNested.RelatedMeta, (string s) => !fieldNested.Meta.HasField(s));
	}

	public override void CreateCloneMeta()
	{
		metaName = GetNestedMetaName(parent, fieldNested);
		cloneField = new BGFieldNested(parent.cloneMeta, metaName);
		cloneMeta = cloneField.NestedMeta;
		cloneField.Name = fieldNested.Name;
		cloneField.NestedMeta.OwnerRelation.Name = fieldNested.NestedMeta.OwnerRelation.Name;
		cloneMeta.System = fieldNested.Meta.System;
		cloneMeta.UniqueName = fieldNested.Meta.UniqueName;
		cloneMeta.Singleton = fieldNested.Meta.Singleton;
		cloneMeta.EmptyName = fieldNested.Meta.EmptyName;
		cloneMeta.Comment = fieldNested.Meta.Comment;
		cloneMeta.ControllerType = fieldNested.Meta.ControllerType;
		cloneMeta.UserDefinedReadonly = fieldNested.Meta.UserDefinedReadonly;
	}

	protected override BGField GetCloneField(BGField field)
	{
		if (fieldNested.NestedMeta.OwnerRelationId == field.Id)
		{
			return cloneField.NestedMeta.OwnerRelation;
		}
		return null;
	}

	protected override void CopyValue(Tuple<BGField, BGField> fieldTuple, Tuple<BGEntity, BGEntity> rowTuple)
	{
		if (fieldTuple.Item1.Id == fieldNested.NestedMeta.OwnerRelationId)
		{
			BGFieldRelationSingle bGFieldRelationSingle = (BGFieldRelationSingle)fieldTuple.Item1;
			BGFieldRelationSingle bGFieldRelationSingle2 = (BGFieldRelationSingle)fieldTuple.Item2;
			BGId storedValue = bGFieldRelationSingle.GetStoredValue(rowTuple.Item1.Index);
			if (!storedValue.IsEmpty)
			{
				bGFieldRelationSingle2.SetStoredValue(rowTuple.Item2.Index, parent.GetToRowId(storedValue));
			}
		}
		else
		{
			base.CopyValue(fieldTuple, rowTuple);
		}
	}
}
