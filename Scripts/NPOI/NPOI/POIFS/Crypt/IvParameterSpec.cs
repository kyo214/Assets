namespace NPOI.POIFS.Crypt;

public class IvParameterSpec : AlgorithmParameterSpec
{
	private byte[] iv;

	public IvParameterSpec(byte[] iv)
	{
		this.iv = iv;
	}

	public byte[] GetIV()
	{
		return iv;
	}
}
