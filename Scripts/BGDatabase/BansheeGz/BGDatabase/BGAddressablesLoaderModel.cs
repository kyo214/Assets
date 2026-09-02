using System;

namespace BansheeGz.BGDatabase;

public class BGAddressablesLoaderModel
{
	public string Address;

	public Type Type;

	public BGAddressablesLoaderModel(string address, Type type)
	{
		Address = address;
		Type = type;
	}
}
