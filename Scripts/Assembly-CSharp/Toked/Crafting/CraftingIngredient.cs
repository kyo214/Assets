using System;
using UnityEngine;

namespace Toked.Crafting;

[Serializable]
public class CraftingIngredient
{
	[SerializeField]
	private CraftMaterialScriptableObject _craftMaterialScriptableObject;

	[SerializeField]
	private int _amount;

	public CraftMaterialScriptableObject CraftMaterialScriptableObject
	{
		get
		{
			return _craftMaterialScriptableObject;
		}
		set
		{
			_craftMaterialScriptableObject = value;
		}
	}

	public int Amount
	{
		get
		{
			return _amount;
		}
		set
		{
			_amount = value;
		}
	}
}
