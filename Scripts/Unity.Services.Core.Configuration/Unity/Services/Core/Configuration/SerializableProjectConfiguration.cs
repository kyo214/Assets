using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Unity.Services.Core.Configuration;

[Serializable]
internal struct SerializableProjectConfiguration
{
	[JsonRequired]
	[SerializeField]
	internal string[] Keys;

	[JsonRequired]
	[SerializeField]
	internal ConfigurationEntry[] Values;

	public static SerializableProjectConfiguration Empty => new SerializableProjectConfiguration
	{
		Keys = Array.Empty<string>(),
		Values = Array.Empty<ConfigurationEntry>()
	};

	public SerializableProjectConfiguration(IDictionary<string, ConfigurationEntry> configValues)
	{
		Keys = new string[configValues.Count];
		Values = new ConfigurationEntry[configValues.Count];
		int num = 0;
		foreach (KeyValuePair<string, ConfigurationEntry> configValue in configValues)
		{
			Keys[num] = configValue.Key;
			Values[num] = configValue.Value;
			num++;
		}
	}
}
