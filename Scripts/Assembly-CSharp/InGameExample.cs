using System.Collections.Generic;
using UGSAnalytics;
using UnityEngine;

public class InGameExample : MonoBehaviour
{
	private Canvas _canvasRaycast;

	private void Awake()
	{
		GetComponent<Canvas>().enabled = false;
	}

	private void Start()
	{
		_canvasRaycast = GetComponent<Canvas>();
	}

	public void ShowCanvas()
	{
		SendWMO.Init();
		SendWMO.SetLobbyID(Random.Range(0, 9999).ToString());
		_canvasRaycast.enabled = true;
	}

	public void TestSendLobby()
	{
		SendWMO.GameLobby(new List<string> { "Togemaster", "KangTembok", "Robert" });
	}
}
