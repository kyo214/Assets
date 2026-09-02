using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMDebugMenuItemChoices : MonoBehaviour
{
	[Header("Bindings")]
	public Sprite SelectedSprite;

	public Sprite OffSprite;

	public Color OnColor = Color.white;

	public Color OffColor = Color.black;

	public Color AccentColor = MMColors.ReunoYellow;

	public List<MMDebugMenuChoiceEntry> Choices;

	public virtual void TriggerButtonEvent(int index)
	{
		MMDebugMenuButtonEvent.Trigger(Choices[index].ButtonEventName);
	}

	public virtual void Select(int index)
	{
		Deselect();
		Choices[index].ButtonBg.sprite = SelectedSprite;
		Choices[index].ButtonBg.color = AccentColor;
		Choices[index].ButtonText.color = OffColor;
	}

	public virtual void Deselect()
	{
		foreach (MMDebugMenuChoiceEntry choice in Choices)
		{
			choice.ButtonBg.sprite = OffSprite;
			choice.ButtonBg.color = OnColor;
			choice.ButtonText.color = OnColor;
		}
	}
}
