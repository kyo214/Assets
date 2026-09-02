using System;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.X509;

namespace NPOI.POIFS.Crypt.Dsig.Services;

public class TSPTimeStampService : ITimeStampService, ISignatureConfigurable
{
	private SignatureConfig signatureConfig;

	public DerObjectIdentifier mapDigestAlgoToOID(HashAlgorithm digestAlgo)
	{
		return digestAlgo.jceId switch
		{
			"sha1" => X509ObjectIdentifiers.IdSha1, 
			"sha256" => NistObjectIdentifiers.IdSha256, 
			"sha384" => NistObjectIdentifiers.IdSha384, 
			"sha512" => NistObjectIdentifiers.IdSha512, 
			_ => throw new ArgumentException("unsupported digest algo: " + digestAlgo), 
		};
	}

	public byte[] TimeStamp(byte[] data, RevocationData revocationData)
	{
		CryptoFunctions.GetMessageDigest(signatureConfig.GetTspDigestAlgo()).Digest(data);
		throw new NotImplementedException();
	}

	public void SetSignatureConfig(SignatureConfig signatureConfig)
	{
		this.signatureConfig = signatureConfig;
	}
}
