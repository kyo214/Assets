using System;
using System.Collections.Generic;
using System.Linq;

namespace BansheeGz.BGDatabase;

public class BGMetaViewMappings
{
	private readonly HashSet<BGId> includedMetas = new HashSet<BGId>();

	private readonly BGMetaView view;

	public BGMetaView View => view;

	public BGId[] IncludedMetas => includedMetas.ToArray();

	public int MappingsCount => includedMetas.Count;

	public BGMetaViewMappings(BGMetaView view)
	{
		this.view = view;
	}

	public bool IsIncluded(BGId metaId)
	{
		return includedMetas.Contains(metaId);
	}

	public void Add(BGId metaId)
	{
		if (includedMetas.Add(metaId))
		{
			View.FireViewChanged();
		}
	}

	public void Remove(BGId metaId)
	{
		if (!includedMetas.Remove(metaId))
		{
			return;
		}
		BGMetaEntity meta = View.Repo.GetMeta(metaId);
		if (meta != null)
		{
			List<BGAbstractRelationI> relationsInbound = View.RelationsInbound;
			foreach (BGAbstractRelationI item in relationsInbound)
			{
				if (!(item is BGFieldViewRelationSingle bGFieldViewRelationSingle))
				{
					if (item is BGFieldViewRelationMultiple bGFieldViewRelationMultiple)
					{
						bGFieldViewRelationMultiple.RemoveRelatedMeta(meta);
					}
				}
				else
				{
					bGFieldViewRelationSingle.RemoveRelatedMeta(meta);
				}
			}
		}
		View.FireViewChanged();
	}

	public void Trim()
	{
		List<BGId> list = null;
		foreach (BGId includedMeta in includedMetas)
		{
			if (!view.Repo.HasMeta(includedMeta))
			{
				list = list ?? new List<BGId>();
				list.Add(includedMeta);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (BGId item in list)
		{
			Remove(item);
		}
	}

	public void CheckStatus(BGMetaEntity meta)
	{
		if (!IsIncluded(meta.Id))
		{
			return;
		}
		view.DelegateMeta.ForEachField((BGField viewField) =>
		{
			BGField field = meta.GetField(viewField.Name, errorIfNotFound: false);
			if (field == null)
			{
				throw new Exception("View [" + view.Name + "] mapping error for table [" + meta.Name + "]: field [" + viewField.Name + "] is not found ");
			}
			if (field.ValueType != viewField.ValueType)
			{
				throw new Exception("View [" + view.Name + "] mapping error for table [" + meta.Name + "]: field [" + viewField.Name + "] has wrong type, expected [" + viewField.ValueType.FullName + "] actual [" + field.ValueType.FullName + "]");
			}
			if (!viewField.ReadonlyFinal && field.ReadonlyFinal)
			{
				throw new Exception("View [" + view.Name + "] mapping error for table [" + meta.Name + "]: field [" + field.FullName + "] should not be readonly");
			}
			if (!(viewField is BGFieldUnityAssetI))
			{
				string a = viewField.ConfigToString();
				string b = field.ConfigToString();
				if (!string.Equals(a, b))
				{
					throw new Exception("View [" + view.Name + "] mapping error for table [" + meta.Name + "]: field [" + field.FullName + "] has incompatible configuration (field's settings)");
				}
			}
		});
	}

	public void CloneTo(BGMetaViewMappings cloneMappings)
	{
		cloneMappings.Clear();
		foreach (BGId includedMeta in includedMetas)
		{
			cloneMappings.Add(includedMeta);
		}
	}

	private void Clear()
	{
		includedMetas.Clear();
	}

	public bool DeepEqual(BGMetaViewMappings t2)
	{
		return includedMetas.SetEquals(t2.includedMetas);
	}

	public void TransferFrom(BGMetaViewMappings t2)
	{
		List<BGId> list = null;
		foreach (BGId includedMeta in includedMetas)
		{
			if (!t2.IsIncluded(includedMeta))
			{
				list = list ?? new List<BGId>();
				list.Add(includedMeta);
			}
		}
		foreach (BGId includedMeta2 in t2.includedMetas)
		{
			if (!IsIncluded(includedMeta2))
			{
				Add(includedMeta2);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (BGId item in list)
		{
			Remove(item);
		}
	}

	public string MappingsToString()
	{
		string text = "";
		foreach (BGId includedMeta in includedMetas)
		{
			if (text.Length != 0)
			{
				text += ", ";
			}
			string text2 = text;
			BGId bGId = includedMeta;
			text = text2 + bGId.ToString();
		}
		return text;
	}
}
