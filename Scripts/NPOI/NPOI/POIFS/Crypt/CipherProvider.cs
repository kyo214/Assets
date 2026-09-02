namespace NPOI.POIFS.Crypt;

public class CipherProvider
{
	public static readonly CipherProvider rc4 = new CipherProvider("RC4", 1, "Microsoft Base Cryptographic Provider v1.0");

	public static readonly CipherProvider aes = new CipherProvider("AES", 24, "Microsoft Enhanced RSA and AES Cryptographic Provider");

	public static CipherProvider[] Values = new CipherProvider[2] { rc4, aes };

	public string jceId { get; set; }

	public int ecmaId { get; set; }

	public string cipherProviderName { get; set; }

	public static CipherProvider FromEcmaId(int ecmaId)
	{
		CipherProvider[] values = Values;
		foreach (CipherProvider cipherProvider in values)
		{
			if (cipherProvider.ecmaId == ecmaId)
			{
				return cipherProvider;
			}
		}
		throw new EncryptedDocumentException("cipher provider not found");
	}

	public CipherProvider(string jceId, int ecmaId, string cipherProviderName)
	{
		this.jceId = jceId;
		this.ecmaId = ecmaId;
		this.cipherProviderName = cipherProviderName;
	}
}
