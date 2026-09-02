using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventorySlotUI : MonoBehaviour
{
	[SerializeField]
	private Image _equipImage;

	[SerializeField]
	private Image _ammoIconImage;

	[SerializeField]
	private TMP_Text _ammoText;

	[SerializeField]
	private Image _armorIconImage;

	[SerializeField]
	private TMP_Text _armorText;

	public void SetActiveEquip(bool active)
	{
		if (_equipImage != null)
		{
			_equipImage.gameObject.SetActive(active);
		}
	}

	public void SetActiveAmmo(bool active)
	{
		_ammoText.gameObject.SetActive(value: false);
	}

	public void SetActiveArmor(bool active)
	{
		_armorText.gameObject.SetActive(value: false);
	}

	public void SetActiveAmmoIcon(bool active)
	{
		_ammoIconImage.gameObject.SetActive(active);
	}

	public void SetActiveArmorIcon(bool active)
	{
		if (!(_armorIconImage == null))
		{
			_armorIconImage.gameObject.SetActive(active);
		}
	}

	public void SetAmmo(string ammo = "")
	{
		if (!(_ammoText == null))
		{
			_ammoText.text = ammo;
			_ammoText.gameObject.SetActive(value: true);
		}
	}

	public void SetActiveAmmo(string ammo = "")
	{
		if (!(_ammoText == null))
		{
			_ammoText.text = ammo;
			if (string.IsNullOrEmpty(ammo) || ammo == "0" || ammo == "-1")
			{
				_ammoText.gameObject.SetActive(value: false);
			}
			else
			{
				_ammoText.gameObject.SetActive(value: true);
			}
		}
	}

	public void ResetAmount()
	{
		if ((bool)_ammoText)
		{
			_ammoText.gameObject.SetActive(value: false);
			_ammoText.text = "0";
		}
		if ((bool)_ammoIconImage)
		{
			_ammoIconImage.gameObject.SetActive(value: false);
		}
		if ((bool)_armorIconImage)
		{
			_armorIconImage.gameObject.SetActive(value: false);
		}
		if ((bool)_armorText)
		{
			_armorText.gameObject.SetActive(value: false);
			_armorText.text = "-1";
		}
		if ((bool)_equipImage)
		{
			_equipImage.gameObject.SetActive(value: false);
		}
	}

	public void SetActiveArmor(string armor = "-1")
	{
		if (!(_armorText == null))
		{
			_armorText.text = armor;
			if (string.IsNullOrEmpty(armor) || armor == "-1")
			{
				_armorText.gameObject.SetActive(value: false);
				SetActiveArmorIcon(active: false);
			}
			else
			{
				_armorText.gameObject.SetActive(value: true);
				SetActiveArmorIcon(active: true);
			}
		}
	}

	public bool GetActiveAmmoText()
	{
		return _ammoText.gameObject.activeSelf;
	}

	public bool GetActiveAmmoIconImage()
	{
		return _ammoIconImage.gameObject.activeSelf;
	}

	public bool GetActiveArmorText()
	{
		return _armorText.gameObject.activeSelf;
	}

	public bool GetActiveArmorIconImage()
	{
		return _armorIconImage.gameObject.activeSelf;
	}

	public bool GetActiveEquipImage()
	{
		return _equipImage.gameObject.activeSelf;
	}

	public TMP_Text GetAmmoText()
	{
		return _ammoText;
	}

	public TMP_Text GetArmorText()
	{
		return _armorText;
	}
}
