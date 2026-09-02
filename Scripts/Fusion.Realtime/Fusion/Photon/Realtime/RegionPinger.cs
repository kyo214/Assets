#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Fusion.Photon.Realtime;

internal class RegionPinger
{
	public static int Attempts = 5;

	public static int MaxMillisecondsPerPing = 800;

	public static int PingWhenFailed = Attempts * MaxMillisecondsPerPing;

	public int CurrentAttempt = 0;

	private Action<Region> onDoneCall;

	private PhotonPing ping;

	private List<int> rttResults;

	private Region region;

	private string regionAddress;

	public bool Done { get; private set; }

	public bool Aborted { get; internal set; }

	public RegionPinger(Region region, Action<Region> onDoneCallback)
	{
		this.region = region;
		this.region.Ping = PingWhenFailed;
		Done = false;
		onDoneCall = onDoneCallback;
	}

	private PhotonPing GetPingImplementation()
	{
		PhotonPing photonPing = null;
		if (RuntimeUnityFlagsSetup.IsUNITY_WEBGL)
		{
			if (RegionHandler.PingImplementation == null || RegionHandler.PingImplementation == typeof(PingHttp))
			{
				photonPing = new PingHttp();
			}
		}
		else if (RegionHandler.PingImplementation == null || RegionHandler.PingImplementation == typeof(PingMono))
		{
			photonPing = new PingMono();
		}
		if (photonPing == null && RegionHandler.PingImplementation != null)
		{
			photonPing = (PhotonPing)Activator.CreateInstance(RegionHandler.PingImplementation);
		}
		return photonPing;
	}

	public bool Start()
	{
		string text = region.HostAndPort;
		int num = text.LastIndexOf(':');
		if (num > 1)
		{
			text = text.Substring(0, num);
		}
		regionAddress = ResolveHost(text);
		ping = GetPingImplementation();
		Done = false;
		CurrentAttempt = 0;
		rttResults = new List<int>(Attempts);
		if (Aborted)
		{
			return false;
		}
		if (RuntimeUnityFlagsSetup.IsUNITY_WEBGL)
		{
			MonoBehaviourEmpty.BuildInstance("RegionPing_" + region.Code).StartCoroutineAndDestroy(RegionPingCoroutine());
		}
		else
		{
			bool flag = false;
			try
			{
				flag = ThreadPool.QueueUserWorkItem((object o) =>
				{
					RegionPingThreaded();
				});
			}
			catch
			{
				flag = false;
			}
			if (!flag)
			{
				SupportClass.StartBackgroundCalls(RegionPingThreaded, 0, "RegionPing_" + region.Code + "_" + region.Cluster);
			}
		}
		return true;
	}

	protected internal void Abort()
	{
		Aborted = true;
		if (ping != null)
		{
			ping.Dispose();
		}
	}

	protected internal bool RegionPingThreaded()
	{
		region.Ping = PingWhenFailed;
		int num = 0;
		int num2 = 0;
		Stopwatch stopwatch = new Stopwatch();
		CurrentAttempt = 0;
		while (CurrentAttempt < Attempts && !Aborted)
		{
			stopwatch.Reset();
			stopwatch.Start();
			try
			{
				ping.StartPing(regionAddress);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("RegionPinger.RegionPingThreaded() caught exception for ping.StartPing(). Exception: " + ex?.ToString() + " Source: " + ex.Source + " Message: " + ex.Message);
				break;
			}
			while (!ping.Done() && stopwatch.ElapsedMilliseconds < MaxMillisecondsPerPing)
			{
				Thread.Sleep(1);
			}
			stopwatch.Stop();
			int num3 = (int)(ping.Successful ? stopwatch.ElapsedMilliseconds : MaxMillisecondsPerPing);
			rttResults.Add(num3);
			num += num3;
			num2++;
			region.Ping = num / num2;
			int num4 = 4;
			while (!ping.Done() && num4 > 0)
			{
				num4--;
				Thread.Sleep(100);
			}
			Thread.Sleep(10);
			CurrentAttempt++;
		}
		Done = true;
		ping.Dispose();
		if (rttResults.Count > 1 && num2 > 0)
		{
			int num5 = rttResults.Min();
			int num6 = rttResults.Max();
			int num7 = num - num6 + num5;
			region.Ping = num7 / num2;
		}
		onDoneCall(region);
		return false;
	}

	protected internal IEnumerator RegionPingCoroutine()
	{
		region.Ping = PingWhenFailed;
		int rttSum = 0;
		int replyCount = 0;
		Stopwatch sw = new Stopwatch();
		for (CurrentAttempt = 0; CurrentAttempt < Attempts; CurrentAttempt++)
		{
			if (Aborted)
			{
				yield return null;
			}
			sw.Reset();
			sw.Start();
			try
			{
				ping.StartPing(regionAddress);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log("RegionPinger.RegionPingCoroutine() caught exception for ping.StartPing(). Exception: " + ex?.ToString() + " Source: " + ex.Source + " Message: " + ex.Message);
				break;
			}
			while (!ping.Done() && sw.ElapsedMilliseconds < MaxMillisecondsPerPing)
			{
				yield return new WaitForSecondsRealtime(0.01f);
			}
			sw.Stop();
			int rtt = (int)(ping.Successful ? sw.ElapsedMilliseconds : MaxMillisecondsPerPing);
			rttResults.Add(rtt);
			rttSum += rtt;
			replyCount++;
			region.Ping = rttSum / replyCount;
			int i = 4;
			while (!ping.Done() && i > 0)
			{
				i--;
				yield return new WaitForSeconds(0.1f);
			}
			yield return new WaitForSeconds(0.1f);
		}
		Done = true;
		ping.Dispose();
		if (rttResults.Count > 1 && replyCount > 0)
		{
			int bestRtt = rttResults.Min();
			int worstRtt = rttResults.Max();
			int weighedRttSum = rttSum - worstRtt + bestRtt;
			region.Ping = weighedRttSum / replyCount;
		}
		onDoneCall(region);
		yield return null;
	}

	public string GetResults()
	{
		return $"{region.Code}: {region.Ping} ({rttResults.ToStringFull()})";
	}

	public static string ResolveHost(string hostName)
	{
		if (hostName.StartsWith("wss://"))
		{
			hostName = hostName.Substring(6);
		}
		if (hostName.StartsWith("ws://"))
		{
			hostName = hostName.Substring(5);
		}
		string text = string.Empty;
		try
		{
			if (RuntimeUnityFlagsSetup.IsUNITY_WEBGL)
			{
				return hostName;
			}
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostName);
			if (hostAddresses.Length == 1)
			{
				return hostAddresses[0].ToString();
			}
			foreach (IPAddress iPAddress in hostAddresses)
			{
				if (iPAddress != null)
				{
					if (iPAddress.ToString().Contains(":"))
					{
						return iPAddress.ToString();
					}
					if (string.IsNullOrEmpty(text))
					{
						text = hostAddresses.ToString();
					}
				}
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine("RegionPinger.ResolveHost() caught an exception for Dns.GetHostAddresses(). Exception: " + ex?.ToString() + " Source: " + ex.Source + " Message: " + ex.Message);
		}
		return text;
	}
}
