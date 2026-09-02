using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using _Modules.UIGlobal;

namespace _Modules.UIMission.Scripts;

public class UIIconModifier : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[SerializeField]
	private Image _modifierMapImage;

	[SerializeField]
	private ToolTipController _toolTipController;

	public void Init(SO_MissionModifierEffect missionModifierEffect)
	{
		_modifierMapImage.sprite = missionModifierEffect.spriteSticker;
		_toolTipController.SetTooltipLocalizationText(missionModifierEffect.ModifierNameLocalization, "");
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (base.gameObject.activeSelf)
		{
			_toolTipController.ShowTooltipDescription();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_toolTipController.HideTooltipDescription();
	}
}
