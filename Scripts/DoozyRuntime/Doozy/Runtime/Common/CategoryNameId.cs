using System;

namespace Doozy.Runtime.Common;

public abstract class CategoryNameId : IEquatable<CategoryNameId>
{
	public string Category;

	public string Name;

	public bool Custom;

	public static string defaultCategory => "None";

	public static string defaultName => "None";

	public bool isDefaultId
	{
		get
		{
			if (isDefaultCategory)
			{
				return isDefaultName;
			}
			return false;
		}
	}

	public bool isDefaultCategory => Category == defaultCategory;

	public bool isDefaultName => Name == defaultName;

	protected CategoryNameId()
	{
		Category = defaultCategory;
		Name = defaultName;
		Custom = false;
	}

	protected CategoryNameId(string category, string name, bool custom = false)
	{
		Category = category;
		Name = name;
		Custom = custom;
	}

	public override string ToString()
	{
		return Category + " / " + Name;
	}

	public static bool operator ==(CategoryNameId a, CategoryNameId b)
	{
		return a?.Equals(b) ?? false;
	}

	public static bool operator !=(CategoryNameId a, CategoryNameId b)
	{
		return !(a == b);
	}

	public bool Equals(CategoryNameId other)
	{
		if ((object)other != null && Category == other.Category)
		{
			return Name == other.Name;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is CategoryNameId other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((((Category != null) ? Category.GetHashCode() : 0) * 397) ^ ((Name != null) ? Name.GetHashCode() : 0)) * 397) ^ Custom.GetHashCode();
	}
}
