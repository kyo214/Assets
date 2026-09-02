using UnityEngine;

namespace MoreMountains.Tools;

[RequireComponent(typeof(SpriteRenderer))]
[AddComponentMenu("More Mountains/Tools/Sprites/MMAutoOrderInLayer")]
public class MMAutoOrderInLayer : MonoBehaviour
{
	private static int CurrentMaxCharacterOrderInLayer;

	[Header("Global Counter")]
	[MMInformation("Add this component to an object with a sprite renderer, and it'll give it a new order in layer based on the settings defined here. First is the global counter increment, or how much you'd like to increment the layer order between two objects on that same layer.", MMInformationAttribute.InformationType.Info, false)]
	public int GlobalCounterIncrement = 5;

	[Header("Parent")]
	[MMInformation("You can also decide to determine the new layer order based on the parent sprite's order (it'll have to be on the same layer).", MMInformationAttribute.InformationType.Info, false)]
	public bool BasedOnParentOrder;

	public int ParentIncrement = 1;

	[Header("Children")]
	[MMInformation("And here you can decide to apply your new layer order to all children.", MMInformationAttribute.InformationType.Info, false)]
	public bool ApplyNewOrderToChildren;

	public int ChildrenIncrement;

	protected SpriteRenderer _spriteRenderer;

	protected virtual void Start()
	{
		Initialization();
		AutomateLayerOrder();
	}

	protected virtual void Initialization()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	protected virtual void AutomateLayerOrder()
	{
		int num = 0;
		if (_spriteRenderer == null)
		{
			return;
		}
		Component[] array;
		if (BasedOnParentOrder)
		{
			int num2 = 0;
			Component[] componentsInParent = GetComponentsInParent(typeof(SpriteRenderer));
			if (componentsInParent != null)
			{
				array = componentsInParent;
				for (int i = 0; i < array.Length; i++)
				{
					SpriteRenderer spriteRenderer = (SpriteRenderer)array[i];
					if (spriteRenderer.sortingLayerID == _spriteRenderer.sortingLayerID && spriteRenderer.sortingOrder > num2)
					{
						num2 = spriteRenderer.sortingOrder;
					}
				}
				num = num2 + ParentIncrement;
			}
		}
		else
		{
			num = CurrentMaxCharacterOrderInLayer + GlobalCounterIncrement;
			CurrentMaxCharacterOrderInLayer += GlobalCounterIncrement;
		}
		_spriteRenderer.sortingOrder = num;
		if (!ApplyNewOrderToChildren)
		{
			return;
		}
		Component[] componentsInChildren = GetComponentsInChildren(typeof(SpriteRenderer));
		if (componentsInChildren == null)
		{
			return;
		}
		array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			SpriteRenderer spriteRenderer2 = (SpriteRenderer)array[i];
			if (spriteRenderer2.sortingLayerID == _spriteRenderer.sortingLayerID)
			{
				spriteRenderer2.sortingOrder = num + ChildrenIncrement;
			}
		}
	}
}
