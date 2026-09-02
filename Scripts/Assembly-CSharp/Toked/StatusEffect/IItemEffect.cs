using I2.Loc;
using UnityEngine;

namespace Toked.StatusEffect;

public interface IItemEffect
{
	private static Color nameColor;

	static readonly string HexColor;

	int ItemId { get; set; }

	int UniqueItemId { get; set; }

	static string GetItemEffectLocalize()
	{
		return "<color=#" + HexColor + ">" + LocalizationManager.GetTranslation("StatusEffect/Cursed") + "</color>";
	}

	void Init(int itemId, int uniqueItemId);

	GameObject GetItemEffectParticle();

	static IItemEffect()
	{
		nameColor = new Color(0.79f, 0.5f, 0.96f, 1f);
		HexColor = ColorUtility.ToHtmlStringRGB(nameColor);
	}
}
