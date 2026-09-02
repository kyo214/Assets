using System;
using Doozy.Runtime.Common;

namespace Doozy.Runtime.UIManager;

[Serializable]
public class UIViewId : CategoryNameId
{
	public enum InGame
	{
		FailedConnect = 0,
		InGame = 1,
		Mission = 2,
		Pause = 3,
		QuitConfirmation = 4,
		StatusInventory = 5,
		TabKill = 6
	}

	public enum MainMenu
	{
		ClientRoom = 0,
		HostRoom = 1,
		MainMenu = 2,
		NewGame = 3,
		Options = 4,
		QuitConfirmation = 5,
		Test = 6,
		Username = 7
	}

	public UIViewId()
	{
	}

	public UIViewId(string category, string name, bool custom = false)
		: base(category, name, custom)
	{
	}
}
