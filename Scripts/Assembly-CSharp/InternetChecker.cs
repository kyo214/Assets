using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class InternetChecker : MonoBehaviour
{
	public float checkInterval = 5f;

	private bool lastStatus;

	private bool hasTriggeredFirstConnected;

	private bool hasTriggeredFirstLostConnected;

	private static readonly HttpClient httpClient = new HttpClient();

	public event Action OnInternetFirstConnected;

	public event Action OnInternetFirstLostConnected;

	public event Action OnInternetLost;

	public event Action OnInternetRestored;

	private void Start()
	{
		StartCoroutine(CheckInternetRoutine());
	}

	private IEnumerator CheckInternetRoutine()
	{
		while (true)
		{
			CheckInternetAsync();
			yield return new WaitForSeconds(checkInterval);
		}
	}

	private async Task CheckInternetAsync()
	{
		bool flag = await IsInternetAvailableAsync();
		if (flag == lastStatus)
		{
			return;
		}
		if (flag)
		{
			if (!hasTriggeredFirstConnected)
			{
				hasTriggeredFirstConnected = true;
				OnInternetFirstConnected?.Invoke();
			}
			else
			{
				OnInternetRestored?.Invoke();
			}
		}
		else
		{
			if (hasTriggeredFirstLostConnected)
			{
				hasTriggeredFirstLostConnected = true;
				OnInternetFirstLostConnected?.Invoke();
			}
			OnInternetLost?.Invoke();
		}
		lastStatus = flag;
	}

	private async Task<bool> IsInternetAvailableAsync()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			return false;
		}
		try
		{
			using CancellationTokenSource cts = new CancellationTokenSource(2000);
			return (await httpClient.GetAsync("http://clients3.google.com/generate_204", cts.Token)).StatusCode == HttpStatusCode.NoContent;
		}
		catch
		{
			return false;
		}
	}
}
