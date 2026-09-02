using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy._Examples.E24___Popup___with_Two_Buttons;

public class ShowTwoButtonsPopup : MonoBehaviour
{
	[Header("Prefab Name")]
	public string PopupName = "TwoButtonsPopup";

	[Header("Labels")]
	public string Title = "My Title";

	public string Message = "My Message";

	[Space(5f)]
	public string LeftButtonLabel = "Ok";

	public UnityEvent OnClickLeftButton = new UnityEvent();

	[Space(5f)]
	public string RightButtonLabel = "Cancel";

	public UnityEvent OnClickRightButton = new UnityEvent();

	public void Show()
	{
		UIPopupExtensions.SetEvents(UIPopupExtensions.SetTexts(UIPopup.Get(PopupName), Title, Message, LeftButtonLabel, RightButtonLabel), OnClickLeftButton, OnClickRightButton).Show();
	}
}
