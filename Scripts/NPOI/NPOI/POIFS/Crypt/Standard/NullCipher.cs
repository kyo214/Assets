namespace NPOI.POIFS.Crypt.Standard;

public class NullCipher : Cipher
{
	public NullCipher()
	{
		cipherImpl = new NullBufferedCipher();
	}
}
