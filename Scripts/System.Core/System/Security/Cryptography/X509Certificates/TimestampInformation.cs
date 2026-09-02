using System.Security.Permissions;
using Unity;

namespace System.Security.Cryptography.X509Certificates;

[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
public sealed class TimestampInformation
{
	public string HashAlgorithm
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}

	public int HResult
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default;
		}
	}

	public bool IsValid
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default;
		}
	}

	public X509Chain SignatureChain
	{
		[SecuritySafeCritical]
		[StorePermission(SecurityAction.Demand, OpenStore = true, EnumerateCertificates = true)]
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}

	public X509Certificate2 SigningCertificate
	{
		[SecuritySafeCritical]
		[StorePermission(SecurityAction.Demand, OpenStore = true, EnumerateCertificates = true)]
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}

	public DateTime Timestamp
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default;
		}
	}

	public SignatureVerificationResult VerificationResult
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default;
		}
	}

	internal TimestampInformation()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
