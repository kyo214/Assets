using Doozy.Runtime.UIManager.Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Modules.UITitle.CreateRoom;

public class PageButtonUI : MonoBehaviour
{
	[SerializeField]
	private UIToggle _uiToggle;

	[SerializeField]
	private Image _disableImage;

	[SerializeField]
	private Image _lockImage;

	public void Init(UnityAction onClickAction, bool disableImage, bool lockImage)
	{
		_uiToggle.onClickEvent.RemoveAllListeners();
		_uiToggle.onClickEvent.AddListener(onClickAction);
		SetActiveDisableImage(disableImage);
		SetActiveLockImage(lockImage);
	}

	public void SetActive(bool active)
	{
		base.gameObject.SetActive(active);
	}

	public void SetActiveToggle(bool active)
	{
		_uiToggle.isOn = active;
	}

	public void SetToggleGroup(UIToggleGroup targetToggleGroup)
	{
		_uiToggle.AddToToggleGroup(targetToggleGroup);
	}

	public void SetActiveDisableImage(bool active)
	{
		_disableImage.gameObject.SetActive(active);
	}

	public void SetActiveLockImage(bool active)
	{
		_lockImage.gameObject.SetActive(active);
	}
}
