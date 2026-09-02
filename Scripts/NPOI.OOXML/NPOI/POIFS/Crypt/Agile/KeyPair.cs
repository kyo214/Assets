namespace NPOI.POIFS.Crypt.Agile;

public class KeyPair
{
	private IPrivateKey privateKey;

	private IPublicKey publicKey;

	public KeyPair(IPublicKey publicKey, IPrivateKey privateKey)
	{
		this.publicKey = publicKey;
		this.privateKey = privateKey;
	}

	public IPublicKey getPublic()
	{
		return publicKey;
	}

	public IPrivateKey getPrivate()
	{
		return privateKey;
	}
}
