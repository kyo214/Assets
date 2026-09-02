using System;
using System.Collections.Generic;
using System.Reflection;

namespace BansheeGz.BGDatabase;

public class BGDnaMeta : BGDnaDescriptor
{
	public readonly List<BGDnaField> Fields = new List<BGDnaField>();

	private BGMetaEntity meta;

	private readonly BGDna dna;

	public BGMetaEntity Meta
	{
		get
		{
			return meta;
		}
		set
		{
			meta = value;
		}
	}

	public BGDna Dna => dna;

	public T MetaAs<T>() where T : BGMetaEntity
	{
		return (T)meta;
	}

	public T DnaAs<T>() where T : BGDna
	{
		return (T)dna;
	}

	protected BGDnaMeta(BGDna dna, string dnaName)
		: base(dnaName)
	{
		this.dna = dna;
		dna?.Add(this);
		Type typeFromHandle = typeof(BGDnaField);
		FieldInfo[] fields = GetType().GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.IsPublic && typeFromHandle.IsAssignableFrom(fieldInfo.FieldType))
			{
				BGDnaField bGDnaField = (BGDnaField)fieldInfo.GetValue(this);
				if (bGDnaField != null && bGDnaField.MetaDna == null)
				{
					bGDnaField.MetaDna = this;
				}
			}
		}
	}

	public BGEntity Get(BGId entityId)
	{
		return meta[entityId];
	}

	public virtual void Bind(BGRepo repo)
	{
		BGMetaEntity bGMetaEntity = repo.GetMeta(DnaName);
		meta = bGMetaEntity ?? throw new BGException("Error while dna binding: Can not find meta with name ($)", DnaName);
		foreach (BGDnaField field in Fields)
		{
			field.Bind(bGMetaEntity);
		}
	}
}
