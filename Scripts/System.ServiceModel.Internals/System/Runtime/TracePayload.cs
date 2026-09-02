namespace System.Runtime;

internal struct TracePayload(string serializedException, string eventSource, string appDomainFriendlyName, string extendedData, string hostReference)
{
	private string serializedException = serializedException;

	private string eventSource = eventSource;

	private string appDomainFriendlyName = appDomainFriendlyName;

	private string extendedData = extendedData;

	private string hostReference = hostReference;

	public string SerializedException => serializedException;

	public string EventSource => eventSource;

	public string AppDomainFriendlyName => appDomainFriendlyName;

	public string ExtendedData => extendedData;

	public string HostReference => hostReference;
}
