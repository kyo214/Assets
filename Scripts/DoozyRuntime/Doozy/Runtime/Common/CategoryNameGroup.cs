using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;

namespace Doozy.Runtime.Common;

[Serializable]
public class CategoryNameGroup<T> where T : CategoryNameItem, new()
{
	[SerializeField]
	private List<T> Items;

	public static string defaultCategory => "None";

	public static string defaultName => "None";

	public List<T> items
	{
		get
		{
			List<T> list = Items;
			if (list == null)
			{
				List<T> obj = new List<T>
				{
					new T()
				};
				List<T> list2 = obj;
				Items = obj;
				list = list2;
			}
			return list;
		}
	}

	public bool isEmpty
	{
		get
		{
			if (ContainsCategory(defaultCategory))
			{
				return items.Count < 2;
			}
			items.Add((T)new CategoryNameItem(defaultCategory, defaultName));
			CleanDatabase();
			return items.Count < 2;
		}
	}

	public bool ContainsCategory(string category)
	{
		return items.Any((T data) => data.category.Equals(CleanString(category)));
	}

	public (bool, string) CanAddCategory(string category)
	{
		category = CleanString(category);
		if (!category.IsNullOrEmpty())
		{
			if (!ContainsCategory(category))
			{
				return (true, "Can add the '" + category + "' category");
			}
			return (false, "'" + category + "' already exists");
		}
		return (false, "Invalid 'category'. It cannot be null or empty or contain special characters");
	}

	public bool AddCategory(string category)
	{
		if (!CanAddCategory(category).Item1)
		{
			return false;
		}
		items.Add((T)new CategoryNameItem(category));
		CleanDatabase();
		return true;
	}

	public (bool, string) CanRenameCategory(string oldCategory, string newCategory)
	{
		oldCategory = CleanString(oldCategory);
		newCategory = CleanString(newCategory);
		if (!oldCategory.IsNullOrEmpty())
		{
			if (ContainsCategory(oldCategory))
			{
				if (!newCategory.IsNullOrEmpty())
				{
					if (!oldCategory.Equals(newCategory))
					{
						return (true, "Can rename the '" + oldCategory + "' category to '" + newCategory + "'");
					}
					return (false, "The new category '" + newCategory + "' is the same as the old category '" + oldCategory + "'");
				}
				return (false, "Invalid 'newCategory'. It cannot be null or empty or contain special characters");
			}
			return (false, "'" + oldCategory + "' does not exist");
		}
		return (false, "Invalid 'oldCategory'. It cannot be null or empty or contain special characters");
	}

	public bool RenameCategory(string oldCategory, string newCategory)
	{
		if (!CanRenameCategory(oldCategory, newCategory).Item1)
		{
			return false;
		}
		oldCategory = CleanString(oldCategory);
		newCategory = CleanString(newCategory);
		foreach (T item in items.Where((T data) => data.category.Equals(oldCategory)).ToList())
		{
			item.SetCategory(newCategory);
		}
		CleanDatabase();
		return true;
	}

	public (bool, string) CanRemoveCategory(string category)
	{
		if (!category.Equals(defaultCategory))
		{
			if (ContainsCategory(category))
			{
				return (true, "Can remove the '" + category + "' category");
			}
			return (false, "The '" + category + "' category does not exist");
		}
		return (false, "Cannot remove the '" + category + "' category");
	}

	public bool RemoveCategory(string category)
	{
		if (!CanRemoveCategory(category).Item1)
		{
			return false;
		}
		for (int num = items.Count - 1; num >= 0; num--)
		{
			if (items[num].category.Equals(category))
			{
				items.RemoveAt(num);
			}
		}
		return true;
	}

	public IEnumerable<string> GetCategories()
	{
		CleanDatabase();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (T item in items)
		{
			hashSet.Add(item.category);
		}
		return hashSet;
	}

	public bool ContainsName(string category, string name)
	{
		return items.Any((T data) => data.category.Equals(CleanString(category)) & data.name.Equals(CleanString(name)));
	}

	public (bool, string) CanAddName(string category, string name)
	{
		category = CleanString(category);
		name = CleanString(name);
		if (!category.IsNullOrEmpty())
		{
			if (!category.Equals(defaultCategory))
			{
				if (!name.IsNullOrEmpty())
				{
					if (!ContainsName(category, name))
					{
						return (true, "Can add the '" + name + "' name to the '" + category + "' category");
					}
					return (false, "The '" + name + "' name already exists in the '" + category + "' category");
				}
				return (false, "Invalid 'name'. It cannot be null or empty or contain special characters");
			}
			return (false, "Cannot add anything to the '" + category + "' category");
		}
		return (false, "Invalid 'category'. It cannot be null or empty or contain special characters");
	}

	public bool AddName(string category, string name)
	{
		if (!CanAddName(category, name).Item1)
		{
			return false;
		}
		items.Add((T)new CategoryNameItem(category, name));
		CleanDatabase();
		return true;
	}

	public (bool, string) CanRemoveName(string category, string name)
	{
		if (!category.Equals(defaultCategory))
		{
			if (!name.Equals(defaultName))
			{
				if (ContainsName(category, name))
				{
					return (true, "Can remove the '" + name + "' from the '" + category + "' category");
				}
				return (false, "The name '" + name + "' was not found in the '" + category + "' category");
			}
			return (false, "Cannot remove '" + name + "'");
		}
		return (false, "Cannot remove anything from the '" + category + "' category");
	}

	public bool RemoveName(string category, string name)
	{
		if (!CanRemoveName(category, name).Item1)
		{
			return false;
		}
		for (int num = items.Count - 1; num >= 0; num--)
		{
			if (items[num].category.Equals(category) && items[num].name.Equals(name))
			{
				items.RemoveAt(num);
				break;
			}
		}
		return true;
	}

	public T Get(string category, string name)
	{
		return items.Where((T data) => data.category.Equals(category)).FirstOrDefault((T data) => data.name.Equals(name));
	}

	public IEnumerable<string> GetNames(string category)
	{
		CleanDatabase();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (T item in items.Where((T data) => data.category.Equals(category)))
		{
			hashSet.Add(item.name);
		}
		return hashSet;
	}

	public void ClearDatabase()
	{
		items.Clear();
		items.Add(new T());
	}

	public void CleanDatabase()
	{
		for (int num = items.Count - 1; num >= 0; num--)
		{
			if (items[num] == null)
			{
				items.RemoveAt(num);
			}
			else if (items[num].category.Trim().IsNullOrEmpty() | items[num].name.Trim().IsNullOrEmpty())
			{
				items.RemoveAt(num);
			}
		}
		Items = (from data in items
			orderby data.category, data.name
			select data).ToList();
	}

	public static string CleanString(string value, bool removeWhitespaces = true, bool removeSpecialCharacters = true)
	{
		return CategoryNameItem.CleanString(value, removeWhitespaces, removeSpecialCharacters);
	}
}
