using System.Configuration;
using Unity;

namespace System.Net.Configuration;

public sealed class WindowsAuthenticationElement : ConfigurationElement
{
	public int DefaultCredentialsHandleCacheSize
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default;
		}
		set
		{
			Unity.ThrowStub.ThrowNotSupportedException();
		}
	}

	public WindowsAuthenticationElement()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
