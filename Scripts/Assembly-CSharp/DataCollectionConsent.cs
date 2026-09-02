using UGSAnalytics;
using UnityEngine;

public class DataCollectionConsent : MonoBehaviour
{
	[SerializeField]
	private InGameExample _example;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void ShowConsent()
	{
	}

	public void Accept()
	{
		DataCollection.SendAccept();
		GetComponent<Canvas>().enabled = false;
		_example.ShowCanvas();
	}

	public void Decline()
	{
		DataCollection.SendDecline();
		_example.ShowCanvas();
	}
}
