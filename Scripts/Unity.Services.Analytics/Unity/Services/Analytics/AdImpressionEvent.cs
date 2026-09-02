using UnityEngine;

namespace Unity.Services.Analytics;

public class AdImpressionEvent : Event
{
	private static readonly string[] k_AdPlacementTypeValues = Event.BakeEnum2String<AdPlacementType>();

	private static readonly string[] k_AdProviderValues = Event.BakeEnum2String<AdProvider>(toUpper: true);

	private static readonly string[] k_AdCompletionStatusValues = Event.BakeEnum2String<AdCompletionStatus>(toUpper: true);

	public AdCompletionStatus AdCompletionStatus
	{
		set
		{
			SetParameter("adCompletionStatus", k_AdCompletionStatusValues[(int)value]);
		}
	}

	public AdProvider AdProvider
	{
		set
		{
			SetParameter("adProvider", k_AdProviderValues[(int)value]);
		}
	}

	public string PlacementId
	{
		set
		{
			SetParameter("placementId", value);
		}
	}

	public string PlacementName
	{
		set
		{
			SetParameter("placementName", value);
		}
	}

	public AdPlacementType PlacementType
	{
		set
		{
			SetParameter("placementType", k_AdPlacementTypeValues[(int)value]);
		}
	}

	public double AdEcpmUsd
	{
		set
		{
			SetParameter("adEcpmUsd", value);
		}
	}

	public string AdStoreDestinationId
	{
		set
		{
			SetParameter("adStoreDestinationID", value);
		}
	}

	public string AdSdkVersion
	{
		set
		{
			SetParameter("adSdkVersion", value);
		}
	}

	public string AdImpressionId
	{
		set
		{
			SetParameter("adImpressionID", value);
		}
	}

	public string AdMediaType
	{
		set
		{
			SetParameter("adMediaType", value);
		}
	}

	public long AdTimeWatchedMs
	{
		set
		{
			SetParameter("adTimeWatchedMs", value);
		}
	}

	public long AdTimeCloseButtonShownMs
	{
		set
		{
			SetParameter("adTimeCloseButtonShownMs", value);
		}
	}

	public long AdLengthMs
	{
		set
		{
			SetParameter("adLengthMs", value);
		}
	}

	public bool AdHasClicked
	{
		set
		{
			SetParameter("adHasClicked", value);
		}
	}

	public string AdSource
	{
		set
		{
			SetParameter("adSource", value);
		}
	}

	public string AdStatusCallback
	{
		set
		{
			SetParameter("adStatusCallback", value);
		}
	}

	public AdImpressionEvent()
		: base("adImpression", standardEvent: true, 1)
	{
	}

	public override void Validate()
	{
		base.Validate();
		if (!ParameterHasBeenSet("placementId"))
		{
			Debug.LogWarning("A value for the PlacementId parameter is required for an AdImpression event.");
		}
		if (!ParameterHasBeenSet("placementName"))
		{
			Debug.LogWarning("A value for the PlacementName parameter is required for an AdImpression event.");
		}
	}
}
