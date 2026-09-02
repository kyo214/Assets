using System;
using Doozy.Runtime.Common.Extensions;

namespace Doozy.Runtime.Signals;

[Serializable]
public struct ProviderId(ProviderType providerType, string providerCategory, string providerName) : IEquatable<ProviderId>
{
	public ProviderType Type = providerType;

	public string Category = providerCategory.RemoveWhitespaces().RemoveAllSpecialCharacters();

	public string Name = providerName.RemoveWhitespaces().RemoveAllSpecialCharacters();

	public override string ToString()
	{
		return $"{Type} {Category}.{Name}";
	}

	public static bool operator ==(ProviderId a, ProviderId b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(ProviderId a, ProviderId b)
	{
		return !(a == b);
	}

	public bool Equals(ProviderId other)
	{
		if (Type == other.Type && Category == other.Category)
		{
			return Name == other.Name;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is ProviderId other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((((int)Type * 397) ^ ((Category != null) ? Category.GetHashCode() : 0)) * 397) ^ ((Name != null) ? Name.GetHashCode() : 0);
	}
}
