using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy._Examples.E23___Popup___with_One_Button;

public class ShowOneButtonPopup : MonoBehaviour
{
	[Header("Prefab Name")]
	public string PopupName = "OneButtonPopup";

	[Header("Labels")]
	public string Title = "My Title";

	public string Message = "My Message";

	[Space(5f)]
	public string ButtonLabel = "Ok";

	public UnityEvent OnClick = new UnityEvent();

	public void Show()
	{
		UIPopupExtensions.SetEvents(UIPopupExtensions.SetTexts(UIPopup.Get(PopupName), Title, Message, ButtonLabel), OnClick).Show();
	}
}
