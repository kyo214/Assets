using System.Configuration;
using Unity;

namespace System.Net.Configuration;

public sealed class HttpListenerTimeoutsElement : ConfigurationElement
{
	public TimeSpan DrainEntityBody
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default;
		}
	}

	public TimeSpan EntityBody
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default;
		}
	}

	public TimeSpan HeaderWait
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default;
		}
	}

	public TimeSpan IdleConnection
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default;
		}
	}

	public long MinSendBytesPerSecond
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default;
		}
	}

	public TimeSpan RequestQueue
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default;
		}
	}

	public HttpListenerTimeoutsElement()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
