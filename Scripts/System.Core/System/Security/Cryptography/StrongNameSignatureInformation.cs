using System.Security.Permissions;
using Unity;

namespace System.Security.Cryptography;

[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
public sealed class StrongNameSignatureInformation
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

	public AsymmetricAlgorithm PublicKey
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return null;
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

	internal StrongNameSignatureInformation()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
