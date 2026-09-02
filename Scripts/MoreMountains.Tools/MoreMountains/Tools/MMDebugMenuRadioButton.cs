using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMDebugMenuRadioButton : MMDebugMenuSpriteReplace
{
	public string RadioButtonGroupName;

	protected List<MMDebugMenuRadioButton> _group;

	public override void Initialization()
	{
		base.Initialization();
		FindAllRadioButtonsFromTheSameGroup();
	}

	protected virtual void FindAllRadioButtonsFromTheSameGroup()
	{
		_group = new List<MMDebugMenuRadioButton>();
		MMDebugMenuRadioButton[] array = Object.FindObjectsOfType(typeof(MMDebugMenuRadioButton)) as MMDebugMenuRadioButton[];
		foreach (MMDebugMenuRadioButton mMDebugMenuRadioButton in array)
		{
			if (mMDebugMenuRadioButton.RadioButtonGroupName == RadioButtonGroupName && mMDebugMenuRadioButton != this)
			{
				_group.Add(mMDebugMenuRadioButton);
			}
		}
	}

	protected override void SpriteOn()
	{
		base.SpriteOn();
		if (_group.Count < 1)
		{
			return;
		}
		foreach (MMDebugMenuRadioButton item in _group)
		{
			item.SwitchToOffSprite();
		}
	}
}
