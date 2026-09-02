using System;

namespace Doozy.Runtime.Signals;

[Serializable]
public struct ProviderAttributes
{
	public ProviderId id { get; }

	public Type typeOfProvider { get; }

	public ProviderAttributes(ProviderId id, Type typeOfProvider)
	{
		this.id = id;
		this.typeOfProvider = typeOfProvider;
	}

	public ProviderAttributes(ProviderType providerType, string providerCategory, string providerName, Type typeOfProvider)
		: this(new ProviderId(providerType, providerCategory, providerName), typeOfProvider)
	{
	}

	public override string ToString()
	{
		return id.ToString() ?? "";
	}
}
