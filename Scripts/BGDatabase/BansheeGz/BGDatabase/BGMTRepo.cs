using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGMTRepo
{
	private readonly BGIdDictionary<BGMTMeta> id2Meta;

	private readonly Dictionary<string, BGMTMeta> name2Meta;

	private readonly BGMTMeta[] metas;

	public BGMTMeta this[int index] => metas[index];

	public BGMTMeta this[BGId id]
	{
		get
		{
			if (!id2Meta.TryGetValue(id, out var value))
			{
				return null;
			}
			return value;
		}
	}

	public BGMTMeta this[string name]
	{
		get
		{
			if (!name2Meta.TryGetValue(name, out var value))
			{
				return null;
			}
			return value;
		}
	}

	internal BGMTRepo(BGMTMeta[] metaList)
	{
		id2Meta = new BGIdDictionary<BGMTMeta>();
		name2Meta = new Dictionary<string, BGMTMeta>();
		metas = metaList;
		foreach (BGMTMeta bGMTMeta in metaList)
		{
			bGMTMeta.Repo = this;
			id2Meta[bGMTMeta.Id] = bGMTMeta;
			name2Meta[bGMTMeta.Name] = bGMTMeta;
		}
	}

	private BGMTRepo(BGIdDictionary<BGMTMeta> id2Meta, Dictionary<string, BGMTMeta> name2Meta, BGMTMeta[] metas)
	{
		this.id2Meta = id2Meta;
		this.name2Meta = name2Meta;
		this.metas = metas;
		if (metas != null)
		{
			for (int i = 0; i < this.metas.Length; i++)
			{
				this.metas[i].Repo = this;
			}
		}
	}

	public void ForEachMeta(Action<BGMTMeta> action)
	{
		for (int i = 0; i < metas.Length; i++)
		{
			action(metas[i]);
		}
	}

	internal BGMTRepo ToWritableRepo()
	{
		BGIdDictionary<BGMTMeta> bGIdDictionary = new BGIdDictionary<BGMTMeta>();
		Dictionary<string, BGMTMeta> dictionary = new Dictionary<string, BGMTMeta>();
		BGMTMeta[] array = metas;
		BGMTMeta[] array2 = new BGMTMeta[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			BGMTMetaUpdatable bGMTMetaUpdatable = (BGMTMetaUpdatable)(array2[i] = new BGMTMetaUpdatable(array[i]));
			bGIdDictionary[bGMTMetaUpdatable.Id] = bGMTMetaUpdatable;
			dictionary[bGMTMetaUpdatable.Name] = bGMTMetaUpdatable;
		}
		return new BGMTRepo(bGIdDictionary, dictionary, array2);
	}

	internal BGMTRepo ToReadOnlyRepo()
	{
		BGIdDictionary<BGMTMeta> bGIdDictionary = new BGIdDictionary<BGMTMeta>();
		Dictionary<string, BGMTMeta> dictionary = new Dictionary<string, BGMTMeta>();
		BGMTMeta[] array = metas;
		BGMTMeta[] array2 = new BGMTMeta[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			BGMTMeta bGMTMeta = array[i];
			BGMTMeta bGMTMeta2 = new BGMTMeta(bGMTMeta);
			bGMTMeta.Dispose();
			array2[i] = bGMTMeta2;
			bGIdDictionary[bGMTMeta2.Id] = bGMTMeta2;
			dictionary[bGMTMeta2.Name] = bGMTMeta2;
		}
		return new BGMTRepo(bGIdDictionary, dictionary, array2);
	}
}
