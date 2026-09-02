using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Modules.UIResult.Scripts;

public class MapUnlockItemUI : MonoBehaviour
{
	[SerializeField]
	private TMP_Text _itemNameText;

	[SerializeField]
	private Localize _itemNameLocalize;

	[SerializeField]
	private Image _itemImage;

	public void Init(SO_MissionMap mapMissionSo)
	{
		_itemNameText.text = "";
		_itemNameLocalize.SetTerm(mapMissionSo.MapNameLocalization);
		_itemImage.sprite = mapMissionSo.MapImage;
	}
}
