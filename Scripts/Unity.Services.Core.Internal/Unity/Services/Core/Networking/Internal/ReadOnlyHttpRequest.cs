using System.Collections.Generic;

namespace Unity.Services.Core.Networking.Internal;

internal struct ReadOnlyHttpRequest(HttpRequest request)
{
	private HttpRequest m_Request = request;

	public string Method => m_Request.Method;

	public string Url => m_Request.Url;

	public IReadOnlyDictionary<string, string> Headers => m_Request.Headers;

	public byte[] Body => m_Request.Body;
}
