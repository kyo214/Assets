using System;
using System.Collections.Generic;
using System.Text;

namespace BansheeGz.BGDatabase;

public class BGKeyValidator : BGValidator
{
	private class Layer
	{
		private readonly List<BGField> fields;

		private readonly Dictionary<object, object> key2Value = new Dictionary<object, object>();

		private object nullables;

		private readonly int index;

		private bool IsLeaf => index == fields.Count - 1;

		internal Layer(List<BGField> fields, int index)
		{
			this.fields = fields;
			this.index = index;
		}

		internal bool Process(BGEntity entity)
		{
			object value = fields[index].GetValue(entity.Index);
			if (IsLeaf)
			{
				if (value == null)
				{
					if (nullables != null)
					{
						return false;
					}
					nullables = entity;
					return true;
				}
				if (key2Value.TryGetValue(value, out var _))
				{
					return false;
				}
				key2Value.Add(value, entity);
				return true;
			}
			Layer layer;
			object value3;
			if (value == null)
			{
				layer = (Layer)((nullables != null) ? ((Layer)nullables) : (nullables = new Layer(fields, index + 1)));
			}
			else if (!key2Value.TryGetValue(value, out value3))
			{
				layer = new Layer(fields, index + 1);
				key2Value.Add(value, layer);
			}
			else
			{
				layer = (Layer)value3;
			}
			return layer.Process(entity);
		}
	}

	private readonly BGKey key;

	private readonly List<int> list = new List<int>();

	private readonly Layer root;

	public BGKeyValidator(BGKey key)
	{
		this.key = key;
		root = new Layer(key.FindFields(), 0);
	}

	public void Validate(BGEntity entity, Func<BGValidationLog[]> provider)
	{
		if (key.IsUnique && !root.Process(entity))
		{
			list.Add(entity.Index);
			BGValidator.Add(provider(), "Unique Key [$] is violated", key.FullName);
		}
	}

	public void Finish(params BGValidationLog[] logs)
	{
		if (list.Count > 0)
		{
			BGValidator.Add(logs, "Key [$] is unique, but following entities violates it: $", key.FullName, GetIndexesString(list));
		}
	}

	private string GetIndexesString(List<int> list)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int i;
		for (i = 0; i < list.Count && i < 20; i++)
		{
			if (stringBuilder.Length != 0)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(list[i]);
		}
		if (i < list.Count)
		{
			stringBuilder.Append(" and (" + (list.Count - i) + ") more rows...");
		}
		return stringBuilder.ToString();
	}
}
