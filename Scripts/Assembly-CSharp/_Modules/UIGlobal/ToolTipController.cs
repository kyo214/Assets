using I2.Loc;
using Toked;
using UnityEngine;

namespace _Modules.UIGlobal;

public class ToolTipController : MonoBehaviour
{
	[SerializeField]
	protected RectTransformExtensions.PivotPresets _pivotPresets = RectTransformExtensions.PivotPresets.MiddleCenter;

	[SerializeField]
	protected Vector2 _offset;

	[SerializeField]
	[TermsPopup("")]
	protected string _description;

	[SerializeField]
	[TermsPopup("")]
	protected string _title;

	protected bool _isShowTooltip;

	public void SetTooltipLocalizationText(string title, string description)
	{
		_title = title;
		_description = description;
	}

	public virtual void ShowTooltipDescription()
	{
		string titleText = (string.IsNullOrWhiteSpace(_title) ? "" : LocalizationManager.GetTranslation(_title));
		string description = (string.IsNullOrWhiteSpace(_description) ? "" : LocalizationManager.GetTranslation(_description));
		Vector3 position = base.gameObject.transform.position;
		GenericSingleton<TooltipManager>.Instance.Show(titleText, description, position, _pivotPresets, _offset);
		_isShowTooltip = true;
	}

	public void HideTooltipDescription()
	{
		_isShowTooltip = false;
		GenericSingleton<TooltipManager>.Instance.Hide();
	}

	private void OnDestroy()
	{
		if (_isShowTooltip)
		{
			HideTooltipDescription();
		}
	}
}
