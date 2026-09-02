using Toked.Crafting;

namespace Toked.Skill;

public class WeaponInventoryEffectValue
{
	public enum WeaponType
	{
		None = 0,
		Melee = 1,
		Range = 2
	}

	public WeaponType Type { get; set; }

	public ItemScriptableObject Value { get; set; }
}
