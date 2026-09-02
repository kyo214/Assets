namespace NPOI.POIFS.Crypt;

public class RC2ParameterSpec : AlgorithmParameterSpec
{
	private byte[] iv;

	private int effectiveKeyBits;

	public RC2ParameterSpec(int v, byte[] vec)
	{
		effectiveKeyBits = v;
		iv = vec;
	}

	public int GetEffectiveKeyBits()
	{
		return effectiveKeyBits;
	}

	public byte[] GetIV()
	{
		return iv;
	}
}
