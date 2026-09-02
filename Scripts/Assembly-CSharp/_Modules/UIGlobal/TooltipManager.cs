using TMPro;
using Toked;
using UnityEngine;

namespace _Modules.UIGlobal;

public class TooltipManager : GenericSingleton<TooltipManager>
{
	[SerializeField]
	private RectTransform _rootPanel;

	[SerializeField]
	private TMP_Text _titleText;

	[SerializeField]
	private TMP_Text _descriptionText;

	[SerializeField]
	private RectTransform _content;

	public RectTransform RootPanel => _rootPanel ?? (_rootPanel = base.transform.GetChild(0).GetComponent<RectTransform>());

	public TMP_Text TitleText => _titleText ?? (_titleText = base.transform.GetChild(0).GetChild(0).GetChild(0)
		.GetComponent<TMP_Text>());

	public TMP_Text DescriptionText => _descriptionText ?? (_descriptionText = base.transform.GetChild(0).GetChild(0).GetChild(1)
		.GetComponent<TMP_Text>());

	public RectTransform Content => _content ?? (_content = base.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>());

	public void Show(string titleText, string description, Vector2 position, RectTransformExtensions.PivotPresets preset = RectTransformExtensions.PivotPresets.MiddleCenter, Vector2 customOffset = default(Vector2))
	{
		Vector2 gUIElementOffset = Content.GetGUIElementOffset();
		Content.position = position + gUIElementOffset;
		TitleText.text = titleText;
		DescriptionText.text = description;
		Content.SetPivot(preset);
		if (customOffset != default(Vector2))
		{
			Content.anchoredPosition = new Vector2(Content.anchoredPosition.x + customOffset.x, Content.anchoredPosition.y + customOffset.y);
		}
		RootPanel.gameObject.SetActive(value: true);
	}

	public void Hide()
	{
		TitleText.text = "";
		DescriptionText.text = "";
		RootPanel.gameObject.SetActive(value: false);
	}
}
