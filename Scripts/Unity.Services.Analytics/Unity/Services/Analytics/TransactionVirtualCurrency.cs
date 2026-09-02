using Unity.Services.Analytics.Internal;

namespace Unity.Services.Analytics;

public class TransactionVirtualCurrency
{
	private static readonly string[] k_VirtualCurrencyTypeValues = Event.BakeEnum2String<VirtualCurrencyType>();

	public string VirtualCurrencyName;

	public VirtualCurrencyType VirtualCurrencyType;

	public long VirtualCurrencyAmount;

	internal void Serialize(IBuffer buffer)
	{
		buffer.PushString("virtualCurrencyName", VirtualCurrencyName);
		buffer.PushString("virtualCurrencyType", k_VirtualCurrencyTypeValues[(int)VirtualCurrencyType]);
		buffer.PushInt64("virtualCurrencyAmount", VirtualCurrencyAmount);
	}
}
