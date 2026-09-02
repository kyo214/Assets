using UnityEngine;

namespace Unity.Services.Analytics;

public class TransactionFailedEvent : TransactionEvent
{
	public string FailureReason
	{
		set
		{
			SetParameter("failureReason", value);
		}
	}

	public TransactionFailedEvent()
		: base("transactionFailed")
	{
	}

	public override void Validate()
	{
		base.Validate();
		if (!ParameterHasBeenSet("failureReason"))
		{
			Debug.LogWarning("A value for the FailureReason parameter is required for a TransactionFailed event.");
		}
	}
}
