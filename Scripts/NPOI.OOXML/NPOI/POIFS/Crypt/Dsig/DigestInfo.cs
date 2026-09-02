using System;
using System.Runtime.Serialization;

namespace NPOI.POIFS.Crypt.Dsig;

public class DigestInfo : ISerializable
{
	private static long serialVersionUID = 1L;

	public byte[] digestValue;

	public string description;

	public HashAlgorithm hashAlgo;

	public DigestInfo(byte[] digestValue, HashAlgorithm hashAlgo, string description)
	{
		this.digestValue = digestValue;
		this.hashAlgo = hashAlgo;
		this.description = description;
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		throw new NotImplementedException();
	}
}
