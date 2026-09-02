using System.Collections.Generic;

namespace Toked.Crafting;

public class HealthEffectValue
{
	public int Value { get; set; }

	public static int CalculateTotalValue(List<HealthEffectValue> values)
	{
		int num = 0;
		foreach (HealthEffectValue value in values)
		{
			num += value.Value;
		}
		return num;
	}
}
