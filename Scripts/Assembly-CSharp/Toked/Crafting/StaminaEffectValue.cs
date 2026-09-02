using System.Collections.Generic;

namespace Toked.Crafting;

public class StaminaEffectValue
{
	public int Value { get; set; }

	public static int CalculateTotalValue(List<StaminaEffectValue> values)
	{
		int num = 0;
		foreach (StaminaEffectValue value in values)
		{
			num += value.Value;
		}
		return num;
	}
}
