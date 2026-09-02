namespace NPOI.POIFS.Crypt;

public class EncryptionMode
{
	public static readonly EncryptionMode BinaryRC4 = new EncryptionMode("NPOI.POIFS.Crypt.BinaryRC4.BinaryRC4EncryptionInfoBuilder", 1, 1, 0);

	public static readonly EncryptionMode CryptoAPI = new EncryptionMode("NPOI.POIFS.Crypt.CryptoAPI.CryptoAPIEncryptionInfoBuilder", 4, 2, 4);

	public static readonly EncryptionMode Standard = new EncryptionMode("NPOI.POIFS.Crypt.Standard.StandardEncryptionInfoBuilder", 4, 2, 36);

	public static readonly EncryptionMode Agile = new EncryptionMode("NPOI.POIFS.Crypt.Agile.AgileEncryptionInfoBuilder", 4, 4, 64);

	public string Builder { get; private set; }

	public int VersionMajor { get; private set; }

	public int VersionMinor { get; private set; }

	public int EncryptionFlags { get; private set; }

	public EncryptionMode(string builder, int versionMajor, int versionMinor, int encryptionFlags)
	{
		Builder = builder;
		VersionMajor = versionMajor;
		VersionMinor = versionMinor;
		EncryptionFlags = encryptionFlags;
	}
}
