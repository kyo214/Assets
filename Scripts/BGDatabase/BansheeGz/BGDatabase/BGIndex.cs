using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGIndex : BGMetaObject
{
	public static StringComparison DefaultStringComparison = StringComparison.Ordinal;

	private readonly BGField field;

	private readonly BGIndexTypeEnum indexType;

	private BGIndexStorage store;

	public BGMetaEntity Meta => this.field.Meta;

	public BGField Field => this.field;

	internal BGIndexTypeEnum IndexType => indexType;

	public override string Name
	{
		set
		{
			if (!string.Equals(Name, value))
			{
				Meta.CheckFieldName(value);
				string oldName = Name;
				base.Name = value;
				Meta.IndexNameWasChanged(this, oldName);
			}
		}
	}

	public override int Index => Meta.GetIndexIndex(base.Id);

	public string FullName => Meta.Name + "." + Name;

	public BGId MetaId => Meta.Id;

	public int Count
	{
		get
		{
			Build();
			return store.Count;
		}
	}

	public BGRepo Repo => Meta.Repo;

	public BGIndex(string name, BGField field)
		: this(BGId.NewId, name, field)
	{
	}

	private BGIndex(BGId id, string name, BGField field)
		: base(id, name)
	{
		this.field = field ?? throw new BGException("Field can not be empty");
		indexType = GetKeyType(field);
		field.Meta.Register(this);
	}

	public void OnCreate()
	{
	}

	public void OnDelete()
	{
	}

	public void Build()
	{
		if (store == null)
		{
			switch (indexType)
			{
			case BGIndexTypeEnum.Bool:
				store = new BGIndexStorage<bool>(this, (BGField<bool>)field);
				break;
			case BGIndexTypeEnum.Byte:
				store = new BGIndexStorage<byte>(this, (BGField<byte>)field);
				break;
			case BGIndexTypeEnum.Decimal:
				store = new BGIndexStorage<decimal>(this, (BGField<decimal>)field);
				break;
			case BGIndexTypeEnum.Int:
				store = new BGIndexStorage<int>(this, (BGField<int>)field);
				break;
			case BGIndexTypeEnum.Long:
				store = new BGIndexStorage<long>(this, (BGField<long>)field);
				break;
			case BGIndexTypeEnum.Short:
				store = new BGIndexStorage<short>(this, (BGField<short>)field);
				break;
			case BGIndexTypeEnum.String:
				store = new BGIndexStorage<string>(this, (BGField<string>)field);
				break;
			case BGIndexTypeEnum.Float:
				store = new BGIndexStorage<float>(this, (BGField<float>)field);
				break;
			case BGIndexTypeEnum.Double:
				store = new BGIndexStorage<double>(this, (BGField<double>)field);
				break;
			default:
				throw new ArgumentOutOfRangeException("indexType", $"keyType is illegal {indexType}");
			}
		}
	}

	public void MarkDirty()
	{
		store?.MarkDirty();
	}

	public override void Delete()
	{
		if (!base.IsDeleted)
		{
			base.Delete();
			Meta.Unregister(this);
			Unload();
		}
	}

	public List<BGEntity> FindEntitiesByIndex(BGIndexOperator @operator)
	{
		return FindEntitiesByIndex<BGEntity>(null, @operator);
	}

	public List<T> FindEntitiesByIndex<T>(List<T> result, BGIndexOperator @operator) where T : BGEntity
	{
		Build();
		if (result == null)
		{
			result = new List<T>();
		}
		else
		{
			result.Clear();
		}
		@operator.GetResult(result, store);
		return result;
	}

	public T GetMin<T>() where T : IComparable<T>
	{
		Build();
		if (store.Count == 0)
		{
			throw new Exception("Index is empty, it's not possible to calculate minimum value");
		}
		if (!(store is BGIndexStorage<T> bGIndexStorage))
		{
			throw new Exception($"Wrong generic parameter type, expected type is {indexType}");
		}
		return bGIndexStorage.Min;
	}

	public T GetMax<T>() where T : IComparable<T>
	{
		Build();
		if (store.Count == 0)
		{
			throw new Exception("Index is empty, it's not possible to calculate maximum value");
		}
		if (!(store is BGIndexStorage<T> bGIndexStorage))
		{
			throw new Exception($"Wrong generic parameter type, expected type is {indexType}");
		}
		return bGIndexStorage.Max;
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

	internal static BGIndex FromBinary(BGBinaryReader binder, BGMetaEntity meta)
	{
		int num = binder.ReadInt();
		if ((uint)(num - 1) <= 1u)
		{
			BGId bGId = binder.ReadId();
			string text = binder.ReadString();
			BGId fieldId = binder.ReadId();
			BGField bGField = meta.GetField(fieldId, errorIfNotFound: false);
			if (bGField == null)
			{
				return null;
			}
			BGIndex bGIndex = Create(bGId, text, bGField);
			if (num >= 2)
			{
				bGIndex.Comment = binder.ReadString();
				bGIndex.ControllerType = binder.ReadString();
			}
			return bGIndex;
		}
		throw new BGException("Can not read key from binary array: unsupported version $", num);
	}

	internal static void ToBinary(BGBinaryWriter builder, BGIndex index)
	{
		builder.AddInt(2);
		builder.AddId(index.Id);
		builder.AddString(index.Name);
		builder.AddId(index.Field?.Id ?? BGId.Empty);
		builder.AddString(index.Comment);
		builder.AddString(index.ControllerType);
	}

	public static BGIndex Create(BGId id, string name, BGField field)
	{
		return new BGIndex(id, name, field);
	}

	private static BGIndexTypeEnum GetKeyType(BGField field)
	{
		if (!(field is BGFieldBool))
		{
			if (!(field is BGFieldByte))
			{
				if (!(field is BGFieldDecimal))
				{
					if (!(field is BGFieldInt))
					{
						if (!(field is BGFieldLong))
						{
							if (!(field is BGFieldShort))
							{
								if (!(field is BGFieldString) && !(field is BGFieldText))
								{
									if (!(field is BGFieldFloat))
									{
										if (field is BGFieldDouble)
										{
											return BGIndexTypeEnum.Double;
										}
										throw new BGException("Field $ can not be used as a index field!", field.Name);
									}
									return BGIndexTypeEnum.Float;
								}
								return BGIndexTypeEnum.String;
							}
							return BGIndexTypeEnum.Short;
						}
						return BGIndexTypeEnum.Long;
					}
					return BGIndexTypeEnum.Int;
				}
				return BGIndexTypeEnum.Decimal;
			}
			return BGIndexTypeEnum.Byte;
		}
		return BGIndexTypeEnum.Bool;
	}

	public static bool IsFieldSupportedAsIndex(BGField field)
	{
		if (field is BGFieldBool || field is BGFieldByte || field is BGFieldDecimal || field is BGFieldInt || field is BGFieldLong || field is BGFieldShort || field is BGFieldString || field is BGFieldText || field is BGFieldFloat || field is BGFieldDouble)
		{
			return true;
		}
		return false;
	}

	public override string ToString()
	{
		return "Index [id:" + base.Id.ToString() + ", name:" + Name + ", field:" + field?.Name + "]";
	}

	public BGIndex CloneTo(BGMetaEntity meta)
	{
		BGField bGField = meta.GetField(field?.Id ?? BGId.Empty, errorIfNotFound: false);
		if (bGField == null)
		{
			return null;
		}
		return new BGIndex(base.Id, Name, bGField)
		{
			Comment = Comment,
			ControllerType = ControllerType
		};
	}

	public bool DeepEqual(BGIndex t2)
	{
		if (!string.Equals(ConfigToString(), t2.ConfigToString()))
		{
			return false;
		}
		if (!string.Equals(Name, t2.Name))
		{
			return false;
		}
		if (!string.Equals(Comment, t2.Comment))
		{
			return false;
		}
		if (!string.Equals(ControllerType, t2.ControllerType))
		{
			return false;
		}
		if (!object.Equals(field, t2.field))
		{
			return false;
		}
		return true;
	}
}
