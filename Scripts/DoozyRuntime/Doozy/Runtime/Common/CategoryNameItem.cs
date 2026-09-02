using System;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;

namespace Doozy.Runtime.Common;

[Serializable]
public class CategoryNameItem
{
	public const string k_DefaultCategory = "None";

	public const string k_DefaultName = "None";

	[SerializeField]
	private string Category;

	[SerializeField]
	private string Name;

	public string category => Category;

	public string name => Name;

	public CategoryNameItem()
	{
		Category = "None";
		Name = "None";
	}

	public CategoryNameItem(string category)
	{
		Category = category;
		Name = "None";
	}

	public CategoryNameItem(string category, string name, bool removeWhitespaces = true, bool removeSpecialCharacters = true)
	{
		Category = CleanString(category, removeWhitespaces, removeSpecialCharacters);
		Name = CleanString(name, removeWhitespaces, removeSpecialCharacters);
	}

	public (bool, string) SetCategory(string newCategory, bool removeWhitespaces = true, bool removeSpecialCharacters = true)
	{
		if (newCategory.RemoveWhitespaces().RemoveAllSpecialCharacters().IsNullOrEmpty())
		{
			return (false, "Invalid 'newCategory'. It cannot be null or empty or contain special characters");
		}
		Category = CleanString(newCategory, removeWhitespaces, removeSpecialCharacters);
		return (true, "'Category' renamed to: " + Category);
	}

	public (bool, string) SetName(string newName, bool removeWhitespaces = true, bool removeSpecialCharacters = true)
	{
		if (newName.RemoveWhitespaces().RemoveAllSpecialCharacters().IsNullOrEmpty())
		{
			return (false, "Invalid 'newName'. It cannot be null or empty or contain special characters");
		}
		Name = CleanString(newName, removeWhitespaces, removeSpecialCharacters);
		return (true, "'Name' renamed to: " + Name);
	}

	public static string CleanString(string value, bool removeWhitespaces = true, bool removeSpecialCharacters = true)
	{
		if (removeWhitespaces)
		{
			value = value.RemoveWhitespaces();
		}
		if (removeSpecialCharacters)
		{
			value = value.RemoveAllSpecialCharacters();
		}
		return value.Trim();
	}
}
