using Unity.Services.Analytics.Internal;

namespace Unity.Services.Analytics;

public class TransactionItem
{
	public string ItemName;

	public string ItemType;

	public long ItemAmount;

	internal void Serialize(IBuffer buffer)
	{
		buffer.PushString("itemName", ItemName);
		buffer.PushString("itemType", ItemType);
		buffer.PushInt64("itemAmount", ItemAmount);
	}
}
