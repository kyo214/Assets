using System;
using Doozy.Runtime.UIManager.Containers;

namespace Doozy.Runtime.UIManager;

[Serializable]
public class UIViewShowHideOption
{
	public UIViewId Id = new UIViewId();

	public ShowHideMode Mode;

	public void Show(int playerIndex)
	{
		UIView.Show(Id.Category, Id.Name, Mode == ShowHideMode.Instant, playerIndex);
	}

	public void Hide(int playerIndex)
	{
		UIView.Hide(Id.Category, Id.Name, Mode == ShowHideMode.Instant, playerIndex);
	}
}
