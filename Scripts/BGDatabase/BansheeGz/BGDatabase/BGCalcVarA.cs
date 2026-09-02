using System;

namespace BansheeGz.BGDatabase;

public abstract class BGCalcVarA
{
	protected BGCalcTypeCode typeCode;

	protected object value;

	public Type Type => typeCode.Type;

	public BGCalcTypeCode TypeCode => typeCode;

	public object Value
	{
		get
		{
			object obj = value;
			if (obj != null)
			{
				if (obj is BGObjectI bGObjectI)
				{
					BGObjectI dbObject = bGObjectI;
					if (RefreshDbValue(ref dbObject))
					{
						value = dbObject;
					}
				}
				return value;
			}
			return value;
		}
		set
		{
			if (!object.Equals(this.value, value))
			{
				object obj = this.value;
				this.value = value;
				FireOnChange();
			}
		}
	}

	public event Action OnValueChange;

	protected BGCalcVarA(BGCalcTypeCode typeCode)
	{
		this.typeCode = typeCode ?? throw new Exception("code can not be null");
		if (typeCode.SupportDefaultValue)
		{
			value = typeCode.DefaultValue;
		}
	}

	public virtual void FireOnChange()
	{
		OnValueChange?.Invoke();
	}

	public virtual void ClearListeners()
	{
		OnValueChange = null;
	}

	protected bool Equals(BGCalcVarA other)
	{
		if (object.Equals(typeCode, other.typeCode))
		{
			return object.Equals(value, other.value);
		}
		return false;
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
		return Equals((BGCalcVarA)obj);
	}

	public override int GetHashCode()
	{
		return (((typeCode != null) ? typeCode.GetHashCode() : 0) * 397) ^ ((value != null) ? value.GetHashCode() : 0);
	}

	public static bool RefreshDbValue(ref BGObjectI dbObject)
	{
		bool result = false;
		BGObjectI bGObjectI = dbObject;
		if (!(bGObjectI is BGMetaEntity bGMetaEntity))
		{
			if (!(bGObjectI is BGField bGField))
			{
				if (!(bGObjectI is BGEntity bGEntity))
				{
					if (bGObjectI is BGCalcCell { Field: var field, Entity: var entity } bGCalcCell)
					{
						if (field != null)
						{
							if (field.IsDeleted || field.Meta.IsDeleted)
							{
								result = true;
								bGCalcCell.Field = null;
								BGMetaEntity meta = BGRepo.I.GetMeta(field.MetaId);
								if (meta != null)
								{
									bGCalcCell.Field = meta.GetField(field.Id, errorIfNotFound: false);
									if (entity != null && entity.Meta.IsDeleted)
									{
										bGCalcCell.Entity = meta.GetEntity(entity.Id);
									}
								}
							}
						}
						else if (entity != null && entity.IsDeleted)
						{
							result = true;
							bGCalcCell.Entity = null;
							BGMetaEntity meta2 = BGRepo.I.GetMeta(entity.MetaId);
							if (meta2 != null)
							{
								bGCalcCell.Entity = meta2.GetEntity(entity.Id);
							}
						}
					}
				}
				else if (bGEntity.Meta.IsDeleted)
				{
					result = true;
					dbObject = null;
					BGMetaEntity meta3 = BGRepo.I.GetMeta(bGEntity.MetaId);
					if (meta3 != null)
					{
						dbObject = meta3.GetEntity(bGEntity.Id);
					}
				}
			}
			else if (bGField.IsDeleted || bGField.Meta.IsDeleted)
			{
				result = true;
				dbObject = null;
				BGMetaEntity meta4 = BGRepo.I.GetMeta(bGField.MetaId);
				if (meta4 != null)
				{
					dbObject = meta4.GetField(bGField.Id, errorIfNotFound: false);
				}
			}
		}
		else if (bGMetaEntity.IsDeleted)
		{
			dbObject = BGRepo.I.GetMeta(bGMetaEntity.Id);
			result = true;
		}
		return result;
	}
}
public abstract class BGCalcVarA<T> : BGCalcVarA where T : BGCalcVarsOwnerBaseI
{
	protected readonly T owner;

	protected BGCalcVarA(T owner, BGCalcTypeCode typeCode)
		: base(typeCode)
	{
		if (owner == null)
		{
			throw new Exception("var owner can not be null");
		}
		this.owner = owner;
	}
}
