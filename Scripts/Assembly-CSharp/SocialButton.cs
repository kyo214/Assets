using Toked;
using UnityEngine;
using UnityEngine.EventSystems;

public class SocialButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[SerializeField]
	private string _hyperLink;

	[SerializeField]
	private GameObject _normal;

	[SerializeField]
	private GameObject _highlighted;

	private void Start()
	{
		_normal.SetActive(value: true);
		_highlighted.SetActive(value: false);
	}

	public void OpenLink()
	{
		if (_hyperLink != null && _hyperLink != "")
		{
			Application.OpenURL(_hyperLink);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Highlight();
	}

	public void Highlight()
	{
		_normal.SetActive(value: false);
		_highlighted.SetActive(value: true);
		AudioManager.PlaySFX("ui_select");
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Normal();
	}

	public void Normal()
	{
		_normal.SetActive(value: true);
		_highlighted.SetActive(value: false);
	}
}
