using System.Collections.Generic;
using Unity.Services.Analytics.Internal;
using UnityEngine;

namespace Unity.Services.Analytics;

public class TransactionEvent : Event
{
	private static readonly string[] k_TransactionTypeValues = Event.BakeEnum2String<TransactionType>();

	private static readonly string[] k_TransactionServerValues = Event.BakeEnum2String<TransactionServer>();

	public string TransactionName
	{
		set
		{
			SetParameter("transactionName", value);
		}
	}

	public string TransactionId
	{
		set
		{
			SetParameter("transactionID", value);
		}
	}

	public TransactionType TransactionType
	{
		set
		{
			SetParameter("transactionType", k_TransactionTypeValues[(int)value]);
		}
	}

	public string PaymentCountry
	{
		set
		{
			SetParameter("paymentCountry", value);
		}
	}

	public string ProductId
	{
		set
		{
			SetParameter("productID", value);
		}
	}

	public string StoreItemSkuId
	{
		set
		{
			SetParameter("storeItemSkuID", value);
		}
	}

	public string StoreItemId
	{
		set
		{
			SetParameter("storeItemID", value);
		}
	}

	public string StoreId
	{
		set
		{
			SetParameter("storeID", value);
		}
	}

	public string StoreSourceId
	{
		set
		{
			SetParameter("storeSourceID", value);
		}
	}

	public string TransactionReceipt
	{
		set
		{
			SetParameter("transactionReceipt", value);
		}
	}

	public string TransactionReceiptSignature
	{
		set
		{
			SetParameter("transactionReceiptSignature", value);
		}
	}

	public TransactionServer TransactionServer
	{
		set
		{
			SetParameter("transactionServer", k_TransactionServerValues[(int)value]);
		}
	}

	public string TransactorID
	{
		set
		{
			SetParameter("transactorID", value);
		}
	}

	public TransactionRealCurrency SpentRealCurrency { get; set; }

	public List<TransactionVirtualCurrency> SpentVirtualCurrencies { get; private set; }

	public List<TransactionItem> SpentItems { get; private set; }

	public TransactionRealCurrency ReceivedRealCurrency { get; set; }

	public List<TransactionVirtualCurrency> ReceivedVirtualCurrencies { get; private set; }

	public List<TransactionItem> ReceivedItems { get; private set; }

	public TransactionEvent()
		: this("transaction")
	{
	}

	protected internal TransactionEvent(string name)
		: base(name, standardEvent: true, 1)
	{
		SpentVirtualCurrencies = new List<TransactionVirtualCurrency>();
		SpentItems = new List<TransactionItem>();
		ReceivedVirtualCurrencies = new List<TransactionVirtualCurrency>();
		ReceivedItems = new List<TransactionItem>();
	}

	internal override void Serialize(IBuffer buffer)
	{
		buffer.PushString("sdkVersion", SdkVersion.SDK_VERSION);
		base.Serialize(buffer);
		buffer.PushProduct("productsReceived", ReceivedRealCurrency, ReceivedVirtualCurrencies, ReceivedItems);
		buffer.PushProduct("productsSpent", SpentRealCurrency, SpentVirtualCurrencies, SpentItems);
	}

	public override void Validate()
	{
		base.Validate();
		if (!ParameterHasBeenSet("transactionName"))
		{
			Debug.LogWarning("A value for the TransactionName parameter is required for a Transaction event.");
		}
		if (!ParameterHasBeenSet("transactionType"))
		{
			Debug.LogWarning("A value for the TransactionType parameter is required for a Transaction event.");
		}
	}

	public override void Reset()
	{
		base.Reset();
		SpentRealCurrency = null;
		SpentItems.Clear();
		SpentVirtualCurrencies.Clear();
		ReceivedRealCurrency = null;
		ReceivedItems.Clear();
		ReceivedVirtualCurrencies.Clear();
	}
}
