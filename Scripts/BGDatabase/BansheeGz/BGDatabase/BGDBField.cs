using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGDBField : BGDBA
{
	public class FieldPath
	{
		private readonly BGField targetField;

		private readonly List<BGFieldRelationSingle> relations;

		private readonly string fieldNameOverride;

		public BGField TargetField => targetField;

		public List<BGFieldRelationSingle> Relations => relations;

		public string PathAsNames
		{
			get
			{
				string text = null;
				if (relations != null && relations.Count != 0)
				{
					foreach (BGFieldRelationSingle relation in relations)
					{
						text = ((text == null) ? relation.Name : (text + "." + relation.Name));
					}
				}
				if (!string.IsNullOrEmpty(fieldNameOverride))
				{
					return (text == null) ? fieldNameOverride : (text + "." + fieldNameOverride);
				}
				return (targetField == null) ? null : ((text == null) ? targetField.Name : (text + "." + targetField.Name));
			}
		}

		public string PathAsIds
		{
			get
			{
				string text = null;
				bool flag = !string.IsNullOrEmpty(fieldNameOverride);
				bool flag2 = relations != null && relations.Count != 0;
				if (flag)
				{
					text = fieldNameOverride + (flag2 ? "$" : "");
				}
				if (flag2)
				{
					foreach (BGFieldRelationSingle relation in relations)
					{
						text = ((text == null) ? relation.Id.ToString() : (text + relation.Id.ToString()));
					}
				}
				if (!flag)
				{
					text = ((targetField == null) ? null : ((text == null) ? targetField.Id.ToString() : (text + targetField.Id.ToString())));
				}
				return text;
			}
		}

		public FieldValueProvider ValueProvider
		{
			get
			{
				if (!UseSpecial)
				{
					return null;
				}
				return BGLocalizationUglyHacks.DataBindingInitValueProvider(fieldNameOverride);
			}
		}

		public bool UseSpecial
		{
			get
			{
				if (!string.IsNullOrEmpty(fieldNameOverride))
				{
					return fieldNameOverride[0] == '$';
				}
				return false;
			}
		}

		public FieldPath(BGField targetField, List<BGFieldRelationSingle> relations)
		{
			this.targetField = targetField;
			this.relations = relations;
		}

		public FieldPath(string fieldNameOverride, List<BGFieldRelationSingle> relations)
		{
			this.fieldNameOverride = fieldNameOverride;
			this.relations = relations;
		}

		public BGEntity GetTargetEntity(BGEntity sourceEntity)
		{
			if (sourceEntity == null)
			{
				return null;
			}
			BGEntity bGEntity = sourceEntity;
			if (relations != null)
			{
				foreach (BGFieldRelationSingle relation in relations)
				{
					if (bGEntity.MetaId != relation.MetaId)
					{
						return null;
					}
					bGEntity = relation[bGEntity.Index];
					if (bGEntity == null)
					{
						return null;
					}
				}
			}
			return bGEntity;
		}

		public BGMetaEntity GetTargetMeta(BGMetaEntity sourceMeta)
		{
			if (sourceMeta == null)
			{
				return null;
			}
			BGMetaEntity bGMetaEntity = sourceMeta;
			if (relations != null)
			{
				foreach (BGFieldRelationSingle relation in relations)
				{
					if (bGMetaEntity.Id != relation.MetaId)
					{
						return null;
					}
					bGMetaEntity = relation.To;
					if (bGMetaEntity == null)
					{
						return null;
					}
				}
			}
			return bGMetaEntity;
		}

		public override string ToString()
		{
			return PathAsNames ?? "";
		}

		protected bool Equals(FieldPath other)
		{
			if (fieldNameOverride != null && object.Equals(fieldNameOverride, other.fieldNameOverride))
			{
				return true;
			}
			bool flag = object.Equals(targetField, other.targetField);
			bool flag2 = AreEquals(relations, other.relations);
			return flag & flag2;
		}

		private bool AreEquals(List<BGFieldRelationSingle> r1, List<BGFieldRelationSingle> r2)
		{
			bool flag = r1 == null || r1.Count == 0;
			bool flag2 = r2 == null || r2.Count == 0;
			if (flag & flag2)
			{
				return true;
			}
			if (flag | flag2)
			{
				return false;
			}
			if (r1.Count != r2.Count)
			{
				return false;
			}
			for (int i = 0; i < r1.Count; i++)
			{
				BGFieldRelationSingle objA = r1[i];
				BGFieldRelationSingle objB = r2[i];
				if (!object.Equals(objA, objB))
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((FieldPath)obj);
		}

		public override int GetHashCode()
		{
			int num = ((targetField != null) ? targetField.GetHashCode() : 0);
			num = (num * 397) ^ ((relations != null) ? relations.GetHashCode() : 0);
			return (num * 397) ^ ((fieldNameOverride != null) ? fieldNameOverride.GetHashCode() : 0);
		}

		public static bool operator ==(FieldPath left, FieldPath right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(FieldPath left, FieldPath right)
		{
			return !object.Equals(left, right);
		}
	}

	public class FieldBindSourceProvider
	{
		private string error;

		private readonly BGDBField source;

		private string sourceFieldIdString;

		private string sourceMetaIdString;

		private string sourceEntityIdString;

		private BGMetaEntity sourceMeta;

		private BGEntity sourceEntity;

		private FieldPath fieldPath;

		public BGMetaEntity SourceMeta => sourceMeta;

		public BGEntity SourceEntity => sourceEntity;

		public BGField TargetField
		{
			get
			{
				if (!(fieldPath == null))
				{
					return fieldPath.TargetField;
				}
				return null;
			}
		}

		public BGMetaEntity TargetMeta
		{
			get
			{
				if (!(fieldPath == null))
				{
					return fieldPath.GetTargetMeta(sourceMeta);
				}
				return sourceMeta;
			}
		}

		public BGEntity TargetEntity
		{
			get
			{
				if (!(fieldPath == null))
				{
					return fieldPath.GetTargetEntity(SourceEntity);
				}
				return null;
			}
		}

		public FieldPath FieldPath => fieldPath;

		public bool IsObsolete
		{
			get
			{
				if (!string.Equals(sourceFieldIdString, source.fieldIdString) || !string.Equals(sourceMetaIdString, source.metaIdString) || !string.Equals(sourceEntityIdString, source.entityIdString))
				{
					return true;
				}
				BGMetaEntity bGMetaEntity = SourceMeta;
				if (bGMetaEntity == null || bGMetaEntity.IsDeleted)
				{
					return true;
				}
				BGEntity bGEntity = SourceEntity;
				if (bGEntity == null || bGEntity.IsDeleted || bGEntity.Meta.IsDeleted)
				{
					return true;
				}
				BGField targetField = TargetField;
				if (targetField == null || targetField.IsDeleted || targetField.Meta.IsDeleted)
				{
					return true;
				}
				BGEntity targetEntity = TargetEntity;
				if (targetEntity == null || targetEntity.IsDeleted || targetEntity.Meta.IsDeleted)
				{
					return true;
				}
				return false;
			}
		}

		public FieldBindSourceProvider(BGDBField source)
		{
			this.source = source;
		}

		public void Build()
		{
			error = null;
			sourceMeta = null;
			sourceEntity = null;
			fieldPath = null;
			sourceFieldIdString = source.fieldIdString;
			sourceMetaIdString = source.metaIdString;
			sourceEntityIdString = source.entityIdString;
			if (!SetError(string.IsNullOrEmpty(sourceMetaIdString), "meta not set"))
			{
				sourceMeta = BGRepo.I[BGId.Parse(sourceMetaIdString)];
				if (!SetError(sourceMeta == null, "can not find meta with id " + sourceMetaIdString))
				{
					BuildField();
					BuildEntity();
				}
			}
		}

		private void BuildEntity()
		{
			if (SetError(string.IsNullOrEmpty(sourceEntityIdString), "entity is not set"))
			{
				return;
			}
			BGId entityId = BGId.Parse(sourceEntityIdString);
			if (!SetError(entityId.IsEmpty, "entity is not set"))
			{
				sourceEntity = sourceMeta.GetEntity(entityId);
				if (!SetError(sourceEntity == null, "can not find entity with id " + sourceEntityIdString) && !SetError(fieldPath == null, "source field not set"))
				{
					BGEntity targetEntity = fieldPath.GetTargetEntity(sourceEntity);
					SetError(targetEntity == null, "source entity can not be found");
				}
			}
		}

		private void BuildField()
		{
			if (SetError(string.IsNullOrEmpty(sourceFieldIdString), "field not set"))
			{
				return;
			}
			string text = sourceFieldIdString;
			bool isUsingSpecialField = source.IsUsingSpecialField;
			string fieldNameOverride = null;
			if (isUsingSpecialField)
			{
				int num = text.IndexOf('$', 1);
				if (num == -1 || text.Length <= num + 1)
				{
					fieldPath = new FieldPath(text, null);
					return;
				}
				fieldNameOverride = text.Substring(0, num);
				text = text.Substring(num + 1);
			}
			if (SetError(text.Length % 22 != 0, "field has invalid id"))
			{
				return;
			}
			BGMetaEntity to = sourceMeta;
			BGField bGField = null;
			List<BGFieldRelationSingle> list = null;
			for (int i = 0; i < text.Length; i += 22)
			{
				string text2 = text.Substring(i, 22);
				if (!BGId.TryParse(text2, out var id))
				{
					SetError("invalid field id " + sourceFieldIdString);
					return;
				}
				BGField field = to.GetField(id, errorIfNotFound: false);
				if (SetError(field == null, "field can not be found, id= " + text2))
				{
					return;
				}
				if ((i + 22 != text.Length) | isUsingSpecialField)
				{
					if (!(field is BGFieldRelationSingle bGFieldRelationSingle))
					{
						SetError("invalid field id: path field is not relation " + text2);
						return;
					}
					list = list ?? new List<BGFieldRelationSingle>();
					list.Add(bGFieldRelationSingle);
					to = bGFieldRelationSingle.To;
				}
				else
				{
					bGField = field;
				}
			}
			if (isUsingSpecialField)
			{
				fieldPath = new FieldPath(fieldNameOverride, list);
			}
			else if (!SetError(bGField == null, "source field can not be found " + sourceFieldIdString))
			{
				fieldPath = new FieldPath(bGField, list);
			}
		}

		private bool SetError(bool condition, string message)
		{
			if (!condition)
			{
				return false;
			}
			SetError(message);
			return true;
		}

		private void SetError(string message)
		{
			string text = (error = ((message == null) ? null : ("[" + message + "]")));
			source.error = text;
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(error))
			{
				return error;
			}
			string text = "";
			text = text + sourceMeta.Name + "@";
			text += fieldPath.ToString();
			BGEntity targetEntity = TargetEntity;
			return text + "@" + ((targetEntity == null) ? "[entity not found]" : targetEntity.Name);
		}
	}

	public interface FieldValueProvider
	{
		object GetValue(BGEntity entity);

		FieldValueProvider Create();

		Type GetValueType(BGMetaEntity meta);
	}

	[SerializeField]
	private string metaIdString;

	[SerializeField]
	private string entityIdString;

	[SerializeField]
	private string fieldIdString;

	[SerializeField]
	private short functionCode;

	[SerializeField]
	private string functionClass;

	[NonSerialized]
	private FieldValueProvider valueProvider;

	[NonSerialized]
	private readonly FieldBindSourceProvider sourceProvider;

	[NonSerialized]
	private BGFBFuntion function;

	public FieldBindSourceProvider BindSourceProvider
	{
		get
		{
			if (sourceProvider.IsObsolete)
			{
				sourceProvider.Build();
			}
			return sourceProvider;
		}
	}

	public string MetaIdString
	{
		get
		{
			return metaIdString;
		}
		set
		{
			metaIdString = value;
		}
	}

	public string EntityIdString
	{
		get
		{
			return entityIdString;
		}
		set
		{
			entityIdString = value;
		}
	}

	public string FieldIdString
	{
		get
		{
			return fieldIdString;
		}
		set
		{
			fieldIdString = value;
		}
	}

	public short FunctionCode
	{
		get
		{
			return functionCode;
		}
		set
		{
			functionCode = value;
		}
	}

	public string FunctionClass
	{
		get
		{
			return functionClass;
		}
		set
		{
			functionClass = value;
		}
	}

	public BGFBFuntion Function
	{
		get
		{
			if (functionCode == 0)
			{
				return null;
			}
			if (function != null)
			{
				if (functionCode == 2 && function is BGFBFuntionToString)
				{
					return function;
				}
				if (functionCode == 1 && function.GetType().FullName == functionClass)
				{
					return function;
				}
			}
			function = null;
			switch (functionCode)
			{
			case 2:
				return function = new BGFBFuntionToString();
			case 1:
				try
				{
					return function = (BGFBFuntion)Activator.CreateInstance(BGUtil.GetType(functionClass));
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
				break;
			}
			return null;
		}
	}

	public FieldValueProvider ValueProvider
	{
		get
		{
			if (valueProvider == null && IsUsingSpecialField)
			{
				valueProvider = BGLocalizationUglyHacks.DataBindingInitValueProvider(fieldIdString);
			}
			return valueProvider;
		}
	}

	public BGMetaEntity Meta => BindSourceProvider.SourceMeta;

	public BGField Field => BindSourceProvider.TargetField;

	public Type FieldType
	{
		get
		{
			if (IsUsingSpecialField)
			{
				FieldValueProvider fieldValueProvider = ValueProvider;
				if (fieldValueProvider != null)
				{
					return ValueProvider.GetValueType(BindSourceProvider.TargetMeta);
				}
				return null;
			}
			return Field?.ValueType;
		}
	}

	public BGEntity Entity => BindSourceProvider.SourceEntity;

	public bool IsUsingSpecialField
	{
		get
		{
			if (!string.IsNullOrEmpty(fieldIdString))
			{
				return fieldIdString[0] == '$';
			}
			return false;
		}
	}

	public override object ValueToBind
	{
		get
		{
			error = null;
			EnsureTarget();
			if (error != null)
			{
				return null;
			}
			object obj = GetValue();
			if (functionCode != 0)
			{
				BGFBFuntion bGFBFuntion = Function;
				if (bGFBFuntion != null)
				{
					obj = bGFBFuntion.Convert(Field, Entity, obj);
				}
			}
			return obj;
		}
	}

	public override object GetValue()
	{
		FieldBindSourceProvider bindSourceProvider = BindSourceProvider;
		if (error != null)
		{
			return null;
		}
		if (IsUsingSpecialField)
		{
			return ValueProvider.GetValue(bindSourceProvider.TargetEntity);
		}
		return bindSourceProvider.TargetField.GetValue(bindSourceProvider.TargetEntity.Index);
	}

	public BGDBField()
	{
		sourceProvider = new FieldBindSourceProvider(this);
	}

	public override string ReverseBind()
	{
		if (IsUsingSpecialField)
		{
			return null;
		}
		BGField bGField = BindSourceProvider.TargetField;
		if (bGField != null && !bGField.StoredValueIsTheSameAsValueType)
		{
			return null;
		}
		try
		{
			if (bGField == null)
			{
				error = "Can not find field with id " + fieldIdString;
				return error;
			}
			BGEntity targetEntity = BindSourceProvider.TargetEntity;
			if (targetEntity == null)
			{
				error = "Can not find entity with id " + entityIdString;
				return error;
			}
			if (targetEntity.IsDeleted)
			{
				error = "Entity is deleted. id " + entityIdString;
				return error;
			}
			object obj = (base.IsTargetProperty ? targetProperty.GetValue(target, null) : targetField.GetValue(target));
			if (obj != null && !bGField.ValueType.IsInstanceOfType(obj))
			{
				error = "Object of type " + obj.GetType().FullName + " is not compatible with field " + bGField.FullName + " value type " + bGField.ValueType.FullName;
				return error;
			}
			bGField.SetValue(targetEntity.Index, obj);
			error = null;
		}
		catch (Exception ex)
		{
			error = ex.Message;
		}
		return error;
	}

	public override int AddFieldsListeners(Action action)
	{
		RemoveFieldsListeners();
		FieldPath fieldPath = BindSourceProvider.FieldPath;
		BGEntity bGEntity = Entity;
		if (bGEntity == null)
		{
			return 0;
		}
		if (fieldPath != null && fieldPath.Relations != null)
		{
			foreach (BGFieldRelationSingle relation in fieldPath.Relations)
			{
				if (bGEntity.MetaId != relation.MetaId)
				{
					return eventHandlers.Count;
				}
				eventHandlers.Add(new FieldEventHandler(bGEntity.MetaId, relation.Id, bGEntity.Id, () =>
				{
					RelationAction(action);
				}));
				bGEntity = relation[bGEntity.Index];
				if (bGEntity == null)
				{
					return eventHandlers.Count;
				}
			}
		}
		BGField field = Field;
		if (field == null)
		{
			return eventHandlers.Count;
		}
		eventHandlers.Add(new FieldEventHandler(field.MetaId, field.Id, bGEntity.Id, action));
		return eventHandlers.Count;
	}

	private void RelationAction(Action action)
	{
		action();
		AddFieldsListeners(action);
	}

	public void AssignFunction(BGFBFuntion function)
	{
		if (function == null)
		{
			FunctionCode = 0;
			FunctionClass = null;
		}
		else if (function is BGFBFuntionToString)
		{
			FunctionCode = 2;
			FunctionClass = null;
		}
		else
		{
			FunctionCode = 1;
			FunctionClass = function.GetType().FullName;
		}
	}
}
