using Doozy.Runtime.UIManager.Containers;
using UnityEngine;

namespace Doozy._Examples.E22___Popup___without_Buttons;

public class ShowSimplePopup : MonoBehaviour
{
	[Header("Prefab Name")]
	public string PopupName = "SimplePopup";

	[Header("Labels")]
	public string Title = "My Title";

	public string Message = "My Message";

	public void Show()
	{
		UIPopupExtensions.SetTexts(UIPopup.Get(PopupName), Title, Message).Show();
	}
}
