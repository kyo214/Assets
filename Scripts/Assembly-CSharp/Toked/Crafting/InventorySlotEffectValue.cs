using System.Collections.Generic;

namespace Toked.Crafting;

public class InventorySlotEffectValue
{
	public int Value { get; set; }

	public static int CalculateTotalValue(List<InventorySlotEffectValue> values)
	{
		int num = 0;
		foreach (InventorySlotEffectValue value in values)
		{
			num += value.Value;
		}
		return num;
	}
}
