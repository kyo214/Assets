using UnityEngine;

namespace Unity.Services.Analytics;

public class AcquisitionSourceEvent : Event
{
	public string AcquisitionChannel
	{
		set
		{
			SetParameter("acquisitionChannel", value);
		}
	}

	public string AcquisitionCampaignId
	{
		set
		{
			SetParameter("acquisitionCampaignId", value);
		}
	}

	public string AcquisitionCreativeId
	{
		set
		{
			SetParameter("acquisitionCreativeId", value);
		}
	}

	public string AcquisitionCampaignName
	{
		set
		{
			SetParameter("acquisitionCampaignName", value);
		}
	}

	public string AcquisitionProvider
	{
		set
		{
			SetParameter("acquisitionProvider", value);
		}
	}

	public float AcquisitionCost
	{
		set
		{
			SetParameter("acquisitionCost", value);
		}
	}

	public string AcquisitionCostCurrency
	{
		set
		{
			SetParameter("acquisitionCostCurrency", value);
		}
	}

	public string AcquisitionNetwork
	{
		set
		{
			SetParameter("acquisitionNetwork", value);
		}
	}

	public string AcquisitionCampaignType
	{
		set
		{
			SetParameter("acquisitionCampaignType", value);
		}
	}

	public AcquisitionSourceEvent()
		: base("acquisitionSource", standardEvent: true, 1)
	{
	}

	public override void Validate()
	{
		base.Validate();
		if (!ParameterHasBeenSet("acquisitionChannel"))
		{
			Debug.LogWarning("A value for the AcquisitionChannel parameter is required for an AcquisitionSource event.");
		}
		if (!ParameterHasBeenSet("acquisitionCampaignId"))
		{
			Debug.LogWarning("A value for the AcquisitionCampaignId parameter is required for an AcquisitionSource event.");
		}
		if (!ParameterHasBeenSet("acquisitionCreativeId"))
		{
			Debug.LogWarning("A value for the AcquisitionCreativeId parameter is required for an AcquisitionSource event.");
		}
		if (!ParameterHasBeenSet("acquisitionCampaignName"))
		{
			Debug.LogWarning("A value for the AcquisitionCampaignName parameter is required for an AcquisitionSource event.");
		}
		if (!ParameterHasBeenSet("acquisitionProvider"))
		{
			Debug.LogWarning("A value for the AcquisitionProvider parameter is required for an AcquisitionSource event.");
		}
	}
}
