using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Signals;

public static class SignalsService
{
	public static readonly List<ISignalProvider> Providers = new List<ISignalProvider>();

	public static UnityAction<ISignalProvider> OnProviderAdded;

	public static UnityAction<ISignalProvider> OnProviderRemoved;

	public const string k_TypeCategory = "Type";

	public static readonly Dictionary<Guid, SignalStream> Streams = new Dictionary<Guid, SignalStream>();

	public static UnityAction<SignalStream> OnStreamAdded;

	public static UnityAction<SignalStream> OnStreamRemoved;

	public static UnityAction<Signal> OnSignal;

	[ExecuteOnReload]
	private static void OnReload()
	{
		Providers.Clear();
		foreach (SignalStream value in Streams.Values)
		{
			value.Close();
		}
		Streams.Clear();
	}

	internal static ISignalProvider AddProvider(ISignalProvider provider)
	{
		RemoveNullProviders();
		if (provider == null)
		{
			return null;
		}
		if (Providers.Contains(provider))
		{
			return provider;
		}
		Providers.Add(provider);
		OnProviderAdded?.Invoke(provider);
		return provider;
	}

	internal static void RemoveProvider(ISignalProvider provider)
	{
		RemoveNullProviders();
		if (provider != null && Providers.Contains(provider))
		{
			Providers.Remove(provider);
			OnProviderRemoved?.Invoke(provider);
		}
	}

	internal static void RemoveNullProviders()
	{
		for (int num = Providers.Count - 1; num >= 0; num--)
		{
			if (Providers[num] == null)
			{
				Providers.RemoveAt(num);
			}
		}
	}

	public static ISignalProvider GetProvider(ProviderId providerId, GameObject signalSource)
	{
		Type providerType = SignalProvider.GetProviderType(providerId);
		signalSource = ((providerId.Type == ProviderType.Global) ? SingletonBehaviour<Signals>.instance.gameObject : signalSource);
		if (signalSource == null)
		{
			throw new NullReferenceException(string.Format("{0} cannot be null when the {1} is {2}", "signalSource", "ProviderType", providerId.Type));
		}
		return (ISignalProvider)(signalSource.GetComponent(providerType) ?? signalSource.AddComponent(providerType));
	}

	public static ISignalProvider GetProvider(ProviderType providerType, string providerCategory, string providerName, GameObject signalSource)
	{
		return GetProvider(new ProviderId(providerType, providerCategory, providerName), signalSource);
	}

	public static ISignalProvider GetProvider(SignalStream stream)
	{
		return Providers.FirstOrDefault((ISignalProvider provider) => provider.stream == stream);
	}

	internal static SignalStream AddStream(SignalStream stream)
	{
		if (stream == null)
		{
			return null;
		}
		if (Streams.ContainsValue(stream))
		{
			return stream;
		}
		Streams.Add(stream.key, stream);
		OnStreamAdded?.Invoke(stream);
		return stream;
	}

	internal static void RemoveStream(SignalStream stream)
	{
		if (stream != null && Streams.ContainsKey(stream.key))
		{
			Streams.Remove(stream.key);
			OnStreamRemoved?.Invoke(stream);
		}
	}

	internal static Guid GetNewStreamKey()
	{
		Guid guid = Guid.NewGuid();
		bool flag = Streams.ContainsKey(guid);
		while (flag)
		{
			guid = Guid.NewGuid();
			flag = Streams.ContainsKey(guid);
		}
		return guid;
	}

	public static SignalStream GetStream()
	{
		return AddStream(new SignalStream(GetNewStreamKey()));
	}

	public static SignalStream GetTypeStream(string typeName)
	{
		return GetStream("Type", typeName);
	}

	public static SignalStream GetStream(string streamCategory, string streamName)
	{
		streamCategory = streamCategory.Trim();
		if (streamCategory.IsNullOrEmpty())
		{
			streamCategory = "None";
		}
		streamName = streamName.Trim();
		if (streamName.IsNullOrEmpty())
		{
			return GetStream();
		}
		using (IEnumerator<SignalStream> enumerator = (from s in Streams.Values
			where s.category.Equals(streamCategory)
			where s.name.Equals(streamName)
			select s).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return AddStream(new SignalStream(GetNewStreamKey()).SetCategory(streamCategory).SetName(streamName));
	}

	public static SignalStream FindStream(Guid streamKey)
	{
		if (!Streams.ContainsKey(streamKey))
		{
			return null;
		}
		return Streams[streamKey];
	}

	public static SignalStream FindStream(string streamCategory, string streamName)
	{
		streamCategory = streamCategory.Trim();
		if (streamCategory.IsNullOrEmpty())
		{
			streamCategory = "None";
		}
		streamName = streamName.Trim();
		if (streamName.IsNullOrEmpty())
		{
			return null;
		}
		return Streams.Values.Where((SignalStream s) => s.category.Equals(streamCategory)).FirstOrDefault((SignalStream s) => s.name.Equals(streamName));
	}

	public static void CloseStream(SignalStream stream)
	{
		stream?.Close();
		RemoveStream(stream);
		RemoveProvider(GetProvider(stream));
	}

	public static void CloseStream(ISignalProvider provider)
	{
		if (provider != null)
		{
			if (provider.isConnected)
			{
				CloseStream(provider.stream);
			}
			else
			{
				RemoveProvider(provider);
			}
		}
	}

	public static bool SendSignal(string streamCategory, string streamName, string message = "")
	{
		return SendSignal(GetStream(streamCategory, streamName), message);
	}

	public static bool SendSignal(string streamCategory, string streamName, GameObject signalSource, string message = "")
	{
		return SendSignal(GetStream(streamCategory, streamName), signalSource, message);
	}

	public static bool SendSignal(string streamCategory, string streamName, SignalProvider signalProvider, string message = "")
	{
		return SendSignal(GetStream(streamCategory, streamName), signalProvider, message);
	}

	public static bool SendSignal(string streamCategory, string streamName, UnityEngine.Object signalSender, string message = "")
	{
		return SendSignal(GetStream(streamCategory, streamName), signalSender, message);
	}

	public static bool SendSignal<T>(string streamCategory, string streamName, T signalValue, string message = "")
	{
		return SendSignal(GetStream(streamCategory, streamName), signalValue, message);
	}

	public static bool SendSignal<T>(string streamCategory, string streamName, T signalValue, GameObject signalSource, string message = "")
	{
		return SendSignal(GetStream(streamCategory, streamName), signalValue, signalSource, message);
	}

	public static bool SendSignal<T>(string streamCategory, string streamName, T signalValue, SignalProvider signalProvider, string message = "")
	{
		return SendSignal(GetStream(streamCategory, streamName), signalValue, signalProvider, message);
	}

	public static bool SendSignal<T>(string streamCategory, string streamName, T signalValue, UnityEngine.Object signalSender, string message = "")
	{
		return SendSignal(GetStream(streamCategory, streamName), signalValue, signalSender, message);
	}

	public static bool SendSignal(Guid streamKey, string message = "")
	{
		return SendSignal(FindStream(streamKey), message);
	}

	public static bool SendSignal(Guid streamKey, GameObject signalSource, string message = "")
	{
		return SendSignal(FindStream(streamKey), signalSource, message);
	}

	public static bool SendSignal(Guid streamKey, SignalProvider signalProvider, string message = "")
	{
		return SendSignal(FindStream(streamKey), signalProvider, message);
	}

	public static bool SendSignal(Guid streamKey, UnityEngine.Object signalSender, string message = "")
	{
		return SendSignal(FindStream(streamKey), signalSender, message);
	}

	public static bool SendSignal<T>(Guid streamKey, T signalValue, string message = "")
	{
		return SendSignal(FindStream(streamKey), signalValue, message);
	}

	public static bool SendSignal<T>(Guid streamKey, T signalValue, GameObject signalSource, string message = "")
	{
		return SendSignal(FindStream(streamKey), signalValue, signalSource, message);
	}

	public static bool SendSignal<T>(Guid streamKey, T signalValue, SignalProvider signalProvider, string message = "")
	{
		return SendSignal(FindStream(streamKey), signalValue, signalProvider, message);
	}

	public static bool SendSignal<T>(Guid streamKey, T signalValue, UnityEngine.Object signalSender, string message = "")
	{
		return SendSignal(FindStream(streamKey), signalValue, signalSender, message);
	}

	public static bool SendSignal(SignalStream stream, string message = "")
	{
		return stream?.SendSignal(message) ?? false;
	}

	public static bool SendSignal(SignalStream stream, GameObject signalSource, string message = "")
	{
		return stream?.SendSignal(signalSource, message) ?? false;
	}

	public static bool SendSignal(SignalStream stream, SignalProvider signalProvider, string message = "")
	{
		return stream?.SendSignal(signalProvider, message) ?? false;
	}

	public static bool SendSignal(SignalStream stream, UnityEngine.Object signalSender, string message = "")
	{
		return stream?.SendSignal(signalSender, message) ?? false;
	}

	public static bool SendSignal<T>(SignalStream stream, T signalValue, string message = "")
	{
		return stream?.SendSignal(signalValue, message) ?? false;
	}

	public static bool SendSignal<T>(SignalStream stream, T signalValue, GameObject signalSource, string message = "")
	{
		return stream?.SendSignal(signalValue, signalSource, message) ?? false;
	}

	public static bool SendSignal<T>(SignalStream stream, T signalValue, SignalProvider signalProvider, string message = "")
	{
		return stream?.SendSignal(signalValue, signalProvider, message) ?? false;
	}

	public static bool SendSignal<T>(SignalStream stream, T signalValue, UnityEngine.Object signalSender, string message = "")
	{
		return stream?.SendSignal(signalValue, signalSender, message) ?? false;
	}
}
