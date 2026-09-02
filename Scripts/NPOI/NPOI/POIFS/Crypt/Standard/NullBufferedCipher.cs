using Org.BouncyCastle.Crypto;

namespace NPOI.POIFS.Crypt.Standard;

public class NullBufferedCipher : IBufferedCipher
{
	public string AlgorithmName => "Null";

	public byte[] DoFinal()
	{
		return new byte[0];
	}

	public byte[] DoFinal(byte[] input)
	{
		return new byte[0];
	}

	public byte[] DoFinal(byte[] input, int inOff, int length)
	{
		return new byte[0];
	}

	public int DoFinal(byte[] output, int outOff)
	{
		return 0;
	}

	public int DoFinal(byte[] input, byte[] output, int outOff)
	{
		return 0;
	}

	public int DoFinal(byte[] input, int inOff, int length, byte[] output, int outOff)
	{
		return 0;
	}

	public int GetBlockSize()
	{
		return 0;
	}

	public int GetOutputSize(int inputLen)
	{
		return 0;
	}

	public int GetUpdateOutputSize(int inputLen)
	{
		return 0;
	}

	public void Init(bool forEncryption, ICipherParameters parameters)
	{
	}

	public byte[] ProcessByte(byte input)
	{
		return new byte[0];
	}

	public int ProcessByte(byte input, byte[] output, int outOff)
	{
		return 0;
	}

	public byte[] ProcessBytes(byte[] input)
	{
		return new byte[0];
	}

	public byte[] ProcessBytes(byte[] input, int inOff, int length)
	{
		return new byte[0];
	}

	public int ProcessBytes(byte[] input, byte[] output, int outOff)
	{
		return 0;
	}

	public int ProcessBytes(byte[] input, int inOff, int length, byte[] output, int outOff)
	{
		return 0;
	}

	public void Reset()
	{
	}
}
