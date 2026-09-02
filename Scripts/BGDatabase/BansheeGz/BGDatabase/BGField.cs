using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGField : BGMetaObject, IEquatable<BGField>
{
	public class FieldDescriptor : BGAttributeWithManager
	{
		public string Folder;

		public string DeprecatedNote;
	}

	protected interface FieldFactory
	{
		BGField Create(BGMetaEntity meta, BGId id, string name);
	}

	private static readonly Dictionary<string, Func<BGMetaEntity, BGId, string, BGField>> FieldTypeName2Factory = new Dictionary<string, Func<BGMetaEntity, BGId, string, BGField>>();

	private static readonly List<Type> AllFieldTypes = new List<Type>();

	private string defaultValue;

	private bool required;

	private bool userDefinedReadonly;

	private string customEditorTypeAsString;

	private string customStringFormatterTypeAsString;

	public static List<Type> FieldTypes
	{
		get
		{
			if (AllFieldTypes.Count != 0)
			{
				return AllFieldTypes;
			}
			List<Type> allSubTypes = BGUtil.GetAllSubTypes(typeof(BGField));
			foreach (Type item in allSubTypes)
			{
				AllFieldTypes.Add(item);
			}
			return AllFieldTypes;
		}
	}

	public string DisplayName => BGAttribute.GetName(GetType()) ?? GetType().Name;

	public string DefaultValue
	{
		get
		{
			return defaultValue;
		}
		set
		{
			if (!string.Equals(defaultValue, value))
			{
				defaultValue = value;
				Meta.Repo.Events.MetaWasChanged(Meta);
			}
		}
	}

	public override string Name
	{
		set
		{
			if (!string.Equals(Name, value))
			{
				Meta.CheckFieldName(value);
				string oldName = Name;
				base.Name = value;
				Meta.FieldNameWasChanged(this, oldName);
			}
		}
	}

	public string FullName => MetaName + "." + Name;

	public BGMetaEntity Meta { get; private set; }

	public BGRepo Repo => Meta.Repo;

	public BGId MetaId => Meta.Id;

	public string MetaName => Meta.Name;

	public bool Required
	{
		get
		{
			return required;
		}
		set
		{
			if (required != value)
			{
				required = value;
				Meta.Repo.Events.MetaWasChanged(Meta);
			}
		}
	}

	public virtual string Description => "Field [" + BGAttribute.GetName(GetType()) + "]";

	public virtual bool ReadOnly => false;

	public virtual int ConstantSize => 0;

	public virtual bool EmptyContent => false;

	public override int Index => Meta.GetFieldIndex(base.Id);

	public override string Comment
	{
		set
		{
			string text = base.Comment;
			if (!string.Equals(value, text))
			{
				bool flag = string.IsNullOrEmpty(value);
				bool flag2 = string.IsNullOrEmpty(text);
				if (!(flag & flag2))
				{
					SetComment(value);
					Meta.Repo.Events.MetaWasChanged(Meta);
				}
			}
		}
	}

	public override string ControllerType
	{
		set
		{
			string text = base.ControllerType;
			if (!string.Equals(value, text))
			{
				bool flag = string.IsNullOrEmpty(value);
				bool flag2 = string.IsNullOrEmpty(text);
				if (!(flag & flag2))
				{
					base.ControllerType = (string.IsNullOrEmpty(value) ? null : value);
					Meta.Repo.Events.MetaWasChanged(Meta);
				}
			}
		}
	}

	public virtual bool SupportMultiThreadedLoading => true;

	public virtual bool CanBeUsedAsKey => false;

	public virtual ushort TypeCode => 0;

	protected BGRepoEvents events => Meta.Repo.Events;

	public bool UserDefinedReadonly
	{
		get
		{
			return userDefinedReadonly;
		}
		set
		{
			if (userDefinedReadonly != value)
			{
				userDefinedReadonly = value;
				Meta.Repo.Events.MetaWasChanged(Meta);
			}
		}
	}

	public bool ReadonlyFinal
	{
		get
		{
			if (!Meta.UserDefinedReadonly && !ReadOnly)
			{
				return userDefinedReadonly;
			}
			return true;
		}
	}

	public string CustomEditorTypeAsString
	{
		get
		{
			return customEditorTypeAsString;
		}
		set
		{
			if (!BGUtil.AreEqual(customEditorTypeAsString, value))
			{
				customEditorTypeAsString = value;
				Meta.Repo.Events.MetaWasChanged(Meta);
			}
		}
	}

	public virtual bool StoredValueIsTheSameAsValueType => true;

	public string CustomStringFormatterTypeAsString
	{
		get
		{
			return customStringFormatterTypeAsString;
		}
		set
		{
			if (!BGUtil.AreEqual(customStringFormatterTypeAsString, value))
			{
				customStringFormatterTypeAsString = value;
				OnCustomStringFormatterChange();
				Meta.Repo.Events.MetaWasChanged(Meta);
			}
		}
	}

	public abstract bool CustomStringFormatSupported { get; }

	public abstract Type ValueType { get; }

	protected bool HasValueListener => ValueChanged != null;

	protected bool HasBeforeValueListener => BeforeValueChanged != null;

	public event EventHandler<BGEventArgsField> ValueChanged;

	public event EventHandler<BGEventArgsField> BeforeValueChanged;

	protected BGField(BGMetaEntity meta, string name)
		: base(meta.NewFieldId, name)
	{
		if (name == "Index")
		{
			throw new Exception("'Index' name is reserved, please, use another name");
		}
		RegisterField(meta);
		meta.ForEachEntity(OnEntityCreate);
	}

	protected BGField(BGMetaEntity meta, BGId id, string name)
		: base(id, name)
	{
		RegisterField(meta);
	}

	private void RegisterField(BGMetaEntity meta)
	{
		Meta = meta;
		Meta.Register(this);
	}

	protected void Unregister()
	{
		Meta?.Unregister(this);
	}

	public override void Delete()
	{
		if (!base.IsDeleted)
		{
			base.Delete();
			Unregister();
			Unload();
			Meta = null;
		}
	}

	[Obsolete("Use CloneTo(BGCloneContextField context) instead")]
	public virtual BGField CloneTo(BGMetaEntity meta, bool copyValues)
	{
		return Clone(meta, base.Id);
	}

	public virtual BGField CloneTo(BGCloneContextField context)
	{
		return Clone(context.meta, base.Id);
	}

	[Obsolete("use Clone(meta, meta.NewFieldId(meta));")]
	public virtual BGField Duplicate(BGMetaEntity meta)
	{
		throw new Exception("This method is obsolete");
	}

	public BGField Clone(BGMetaEntity meta, BGId fieldId)
	{
		BGField bGField = CreateFieldFactory()(meta, fieldId, Name);
		bGField.System = System;
		bGField.Addon = base.Addon;
		bGField.CustomStringFormatterTypeAsString = CustomStringFormatterTypeAsString;
		bGField.CustomEditorTypeAsString = CustomEditorTypeAsString;
		bGField.Comment = Comment;
		bGField.ControllerType = ControllerType;
		bGField.DefaultValue = DefaultValue;
		byte[] array = ConfigToBytes();
		bGField.ConfigFromBytes((array == null) ? new ArraySegment<byte>(Array.Empty<byte>()) : new ArraySegment<byte>(array));
		return bGField;
	}

	private void SetComment(string value)
	{
		base.Comment = value;
	}

	public bool Equals(BGField other)
	{
		if (other != null)
		{
			return base.Id == other.Id;
		}
		return false;
	}

	protected abstract Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory();

	public virtual void OnEntityAdd(BGEntity entity)
	{
	}

	public virtual void OnEntityDelete(BGEntity entity)
	{
	}

	public virtual void OnNameChange(int entityIndex)
	{
	}

	public virtual void OnEntityCreate(BGEntity entity)
	{
		if (string.IsNullOrEmpty(DefaultValue))
		{
			return;
		}
		try
		{
			FromString(entity.Index, DefaultValue);
		}
		catch (Exception)
		{
		}
	}

	public virtual void OnCreate()
	{
	}

	public virtual void OnDelete()
	{
	}

	public override string ConfigToString()
	{
		return null;
	}

	public override void ConfigFromString(string config)
	{
	}

	public override byte[] ConfigToBytes()
	{
		return null;
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
	}

	protected virtual void OnCustomStringFormatterChange()
	{
	}

	public abstract string ToCustomString(int entityIndex);

	public abstract void FromCustomString(int entityIndex, string formattedValue);

	public abstract void ClearValue(int entityIndex);

	public abstract void ForEachValue(Action<int> action);

	public abstract void CopyValue(BGField fromField, BGId fromEntityId, int fromEntityIndex, BGId toEntityId);

	public abstract void DuplicateValue(BGId fromEntityId, int fromEntityIndex, BGId toEntityId);

	public abstract object GetValue(BGId entityId);

	public abstract void SetValue(BGId entityId, object value);

	public abstract object GetValue(int entityIndex);

	public abstract void SetValue(int entityIndex, object value);

	public abstract void Swap(int entityIndex1, int entityIndex2);

	public abstract void MoveEntitiesValues(int fromIndex, int toIndex, int numberOfValues);

	public abstract void ClearValues();

	public abstract bool AreStoredValuesEqual(BGField field, int myEntityIndex, int otherEntityIndex);

	public abstract byte[] ToBytes(int entityIndex);

	public abstract void FromBytes(int entityIndex, ArraySegment<byte> segment);

	public abstract string ToString(int entityIndex);

	public abstract void FromString(int entityIndex, string value);

	public override string ToString()
	{
		return "Field [id:" + base.Id.ToString() + ", name:" + Name + ", type:" + GetType().FullName + "]";
	}

	public static BGField Create(BGMetaEntity meta, string type, BGId id, string name, string config, bool system, string addon, string defaultValue, bool required)
	{
		BGField bGField = Create(meta, type, id, name, system, addon, defaultValue, required);
		bGField.ConfigFromString(config);
		return bGField;
	}

	public static BGField Create(BGMetaEntity meta, string type, BGId id, string name, ArraySegment<byte> config, bool system, string addon, string defaultValue, bool required)
	{
		BGField bGField = Create(meta, type, id, name, system, addon, defaultValue, required);
		bGField.ConfigFromBytes(config);
		return bGField;
	}

	private static BGField Create(BGMetaEntity meta, string type, BGId id, string name, bool system, string addon, string defaultValue, bool required)
	{
		BGField bGField;
		if (FieldTypeName2Factory.TryGetValue(type, out var value))
		{
			bGField = value(meta, id, name);
		}
		else
		{
			bGField = BGUtil.Create<BGField>(type, includePrivateConstructors: true, new object[3] { meta, id, name });
			FieldTypeName2Factory[type] = bGField.CreateFieldFactory();
		}
		bGField.DefaultValue = defaultValue;
		bGField.System = system;
		bGField.Addon = addon;
		bGField.Required = required;
		return bGField;
	}

	internal static BGField FromBinary(BGBinaryReader binder, BGMetaEntity meta)
	{
		int num = binder.ReadInt();
		switch (num)
		{
		case 1:
		{
			BGId bGId2 = binder.ReadId();
			string text7 = binder.ReadString();
			string type2 = binder.ReadString();
			ArraySegment<byte> config2 = binder.ReadByteArray();
			bool flag3 = binder.ReadBool();
			string text8 = binder.ReadString();
			string text9 = binder.ReadString();
			bool flag4 = binder.ReadBool();
			string text10 = binder.ReadString();
			string text11 = binder.ReadString();
			string text12 = binder.ReadString();
			BGField bGField2 = Create(meta, type2, bGId2, text7, config2, flag3, text8, text9, flag4);
			bGField2.CustomStringFormatterTypeAsString = text10;
			bGField2.CustomEditorTypeAsString = text11;
			bGField2.SetComment(text12);
			return bGField2;
		}
		case 2:
		case 3:
		case 4:
		{
			ushort num2 = binder.ReadUShort();
			string type = null;
			if (num2 == 0)
			{
				type = binder.ReadString();
			}
			BGId bGId = binder.ReadId();
			string text = binder.ReadString();
			ArraySegment<byte> config = binder.ReadByteArray();
			bool flag = binder.ReadBool();
			string text2 = binder.ReadString();
			string text3 = binder.ReadString();
			bool flag2 = binder.ReadBool();
			string text4 = binder.ReadString();
			string text5 = binder.ReadString();
			string text6 = binder.ReadString();
			BGField bGField = ((num2 == 0) ? Create(meta, type, bGId, text, config, flag, text2, text3, flag2) : BGFieldTypeCodeFactory.Instance.Create(meta, num2, bGId, text, config, flag, text2, text3, flag2));
			bGField.CustomStringFormatterTypeAsString = text4;
			bGField.CustomEditorTypeAsString = text5;
			bGField.SetComment(text6);
			if (num >= 3)
			{
				bGField.UserDefinedReadonly = binder.ReadBool();
			}
			if (num >= 4)
			{
				bGField.ControllerType = binder.ReadString();
			}
			return bGField;
		}
		default:
			throw new BGException("Can not read field from binary array: unsupported version $", num);
		}
	}

	internal static void ToBinary(BGBinaryWriter builder, BGField field)
	{
		builder.AddInt(4);
		builder.AddUShort(field.TypeCode);
		if (field.TypeCode == 0)
		{
			builder.AddString(field.GetType().AssemblyQualifiedName);
		}
		builder.AddId(field.Id);
		builder.AddString(field.Name);
		builder.AddByteArray(field.ConfigToBytes());
		builder.AddBool(field.System);
		builder.AddString(field.Addon);
		builder.AddString(field.DefaultValue);
		builder.AddBool(field.Required);
		builder.AddString(field.CustomStringFormatterTypeAsString);
		builder.AddString(field.CustomEditorTypeAsString);
		builder.AddString(field.Comment);
		builder.AddBool(field.UserDefinedReadonly);
		builder.AddString(field.ControllerType);
	}

	public void FireValueChanged(BGEntity entity)
	{
		if (!events.ConsumeOnChange(MetaId))
		{
			FireValueChangedInternal(entity);
		}
	}

	protected void FireValueChanged(BGId entityId)
	{
		if (!events.ConsumeOnChange(MetaId))
		{
			BGEntity entity = Meta.GetEntity(entityId);
			if (entity != null)
			{
				FireValueChangedInternal(entity);
			}
		}
	}

	private void FireValueChangedInternal(BGEntity entity)
	{
		if (ValueChanged != null)
		{
			using BGEventArgsField e = BGEventArgsField.GetInstance(entity, base.Id);
			ValueChanged(this, e);
		}
		Meta.FireValueChanged(this, entity, nested: true);
		events.FireAnyChange();
	}

	protected void FireValueChanged(BGEventArgsField eventArgs)
	{
		ValueChanged?.Invoke(this, eventArgs);
	}

	protected void FireBeforeValueChanged(BGEventArgsField eventArgs)
	{
		BeforeValueChanged?.Invoke(this, eventArgs);
	}

	internal void TransferEventsTo(BGEventsHolder eventsHolder)
	{
		if (ValueChanged != null)
		{
			eventsHolder.AddOnFieldValueChangedListeners(base.Id, ValueChanged.GetInvocationList());
			ValueChanged = null;
		}
		if (BeforeValueChanged != null)
		{
			eventsHolder.AddOnBeforeFieldValueChangedListeners(base.Id, BeforeValueChanged.GetInvocationList());
			BeforeValueChanged = null;
		}
	}

	internal void TransferEventsFrom(BGEventsHolder eventsHolder)
	{
		ValueChanged = null;
		Delegate[] onFieldValueChangedListeners = eventsHolder.GetOnFieldValueChangedListeners(base.Id);
		if (onFieldValueChangedListeners != null && onFieldValueChangedListeners.Length != 0)
		{
			Delegate[] array = onFieldValueChangedListeners;
			foreach (Delegate obj in array)
			{
				ValueChanged += (EventHandler<BGEventArgsField>)obj;
			}
		}
		BeforeValueChanged = null;
		onFieldValueChangedListeners = eventsHolder.GetOnFieldBeforeValueChangedListeners(base.Id);
		if (onFieldValueChangedListeners != null && onFieldValueChangedListeners.Length != 0)
		{
			Delegate[] array2 = onFieldValueChangedListeners;
			foreach (Delegate obj2 in array2)
			{
				BeforeValueChanged += (EventHandler<BGEventArgsField>)obj2;
			}
		}
	}
}
public abstract class BGField<T> : BGField
{
	protected const char S = '`';

	protected const char A = '|';

	protected static readonly char[] AA = new char[1] { '|' };

	private Type valueType;

	private bool customStringFormatterActivationTried;

	private BGStringFormatter<T> customStringFormatter;

	public override Type ValueType
	{
		get
		{
			if (valueType != null)
			{
				return valueType;
			}
			valueType = typeof(T);
			return valueType;
		}
	}

	public abstract T this[BGId entityId] { get; set; }

	public abstract T this[int index] { get; set; }

	public override bool CustomStringFormatSupported
	{
		get
		{
			if (StoredValueIsTheSameAsValueType)
			{
				return CustomStringFormatter != null;
			}
			return false;
		}
	}

	private BGStringFormatter<T> CustomStringFormatter
	{
		get
		{
			if (customStringFormatter != null)
			{
				return customStringFormatter;
			}
			if (string.IsNullOrEmpty(base.CustomStringFormatterTypeAsString))
			{
				return null;
			}
			if (customStringFormatterActivationTried)
			{
				return null;
			}
			customStringFormatterActivationTried = true;
			Type type = BGUtil.GetType(base.CustomStringFormatterTypeAsString);
			if (type == null)
			{
				return null;
			}
			try
			{
				customStringFormatter = Activator.CreateInstance(type) as BGStringFormatter<T>;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return customStringFormatter;
		}
	}

	protected BGField(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGField(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override object GetValue(BGId entityId)
	{
		return this[entityId];
	}

	public override void SetValue(BGId entityId, object value)
	{
		this[entityId] = (T)value;
	}

	public override object GetValue(int entityIndex)
	{
		return this[entityIndex];
	}

	public override void SetValue(int entityIndex, object value)
	{
		this[entityIndex] = (T)value;
	}

	protected override void OnCustomStringFormatterChange()
	{
		customStringFormatter = null;
		customStringFormatterActivationTried = false;
	}

	public override string ToCustomString(int entityIndex)
	{
		try
		{
			return CustomStringFormatter.ToString(this[entityIndex]);
		}
		catch (BGStringFormatterUseDefaultException)
		{
			return ToString(entityIndex);
		}
	}

	public override void FromCustomString(int entityIndex, string formattedValue)
	{
		try
		{
			this[entityIndex] = CustomStringFormatter.FromString(formattedValue);
		}
		catch (BGStringFormatterUseDefaultException)
		{
			FromString(entityIndex, formattedValue);
		}
	}

	public void FireValueChanged(BGEntity entity, T oldValue, T newValue)
	{
		if (base.events.ConsumeOnChange(base.MetaId))
		{
			return;
		}
		if (base.HasValueListener)
		{
			using BGEventArgsFieldWithValue<T> eventArgs = BGEventArgsFieldWithValue<T>.GetInstance(entity, this, oldValue, newValue);
			FireValueChanged(eventArgs);
		}
		base.Meta.FireValueChanged(this, entity, oldValue, newValue);
		base.events.FireAnyChange();
	}

	public void FireBeforeValueChanged(BGEntity entity, T oldValue, T newValue)
	{
		if (base.events.ConsumeOnChange(base.MetaId))
		{
			return;
		}
		if (base.HasBeforeValueListener)
		{
			using BGEventArgsFieldWithValue<T> eventArgs = BGEventArgsFieldWithValue<T>.GetInstance(entity, this, oldValue, newValue);
			FireBeforeValueChanged(eventArgs);
		}
		base.Meta.FireBeforeValueChanged(this, entity, oldValue, newValue);
	}
}
