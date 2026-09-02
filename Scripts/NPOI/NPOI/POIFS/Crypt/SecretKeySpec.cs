using System;

namespace NPOI.POIFS.Crypt;

public class SecretKeySpec : IKeySpec, ISecretKey, IKey
{
	private byte[] key;

	private string algorithm;

	public SecretKeySpec(byte[] key, string algorithm)
	{
		if (key == null || algorithm == null)
		{
			throw new ArgumentException("Missing argument");
		}
		if (key.Length == 0)
		{
			throw new ArgumentException("Empty key");
		}
		this.key = new byte[key.Length];
		Array.Copy(key, this.key, key.Length);
		this.algorithm = algorithm;
	}

	public string GetAlgorithm()
	{
		return algorithm;
	}

	public byte[] GetEncoded()
	{
		byte[] array = new byte[key.Length];
		Array.Copy(key, array, key.Length);
		return array;
	}

	public string GetFormat()
	{
		return "RAW";
	}
}
