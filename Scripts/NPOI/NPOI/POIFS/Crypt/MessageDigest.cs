using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace NPOI.POIFS.Crypt;

public class MessageDigest
{
	private IDigest digestImpl;

	internal static MessageDigest GetInstance(string jceId, string v)
	{
		return GetInstance(jceId);
	}

	internal static MessageDigest GetInstance(string jceId)
	{
		return new MessageDigest
		{
			digestImpl = DigestUtilities.GetDigest(jceId)
		};
	}

	internal void Update(byte[] passwordHash)
	{
		Update(passwordHash, 0, passwordHash.Length);
	}

	internal void Reset()
	{
		digestImpl.Reset();
	}

	internal int Digest(byte[] buf, int offset, int len)
	{
		if (buf == null)
		{
			throw new ArgumentNullException("No output buffer given");
		}
		if (buf.Length - offset < len)
		{
			throw new ArgumentOutOfRangeException("Output buffer too small for specified offset and length");
		}
		byte[] array = Digest();
		if (len < array.Length)
		{
			throw new Exception("partial digests not returned");
		}
		if (buf.Length - offset < array.Length)
		{
			throw new Exception("insufficient space in the output buffer to store the digest");
		}
		Array.Copy(array, 0, buf, offset, array.Length);
		return array.Length;
	}

	internal byte[] Digest()
	{
		byte[] array = new byte[digestImpl.GetDigestSize()];
		digestImpl.DoFinal(array, 0);
		return array;
	}

	internal void Update(byte[] hash, int v1, int v2)
	{
		digestImpl.BlockUpdate(hash, v1, v2);
	}

	public byte[] Digest(byte[] input)
	{
		byte[] array = new byte[digestImpl.GetDigestSize()];
		digestImpl.BlockUpdate(input, 0, input.Length);
		digestImpl.DoFinal(array, 0);
		return array;
	}
}
