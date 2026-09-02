using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.POIFS.Crypt;

public abstract class Encryptor
{
	internal static string DEFAULT_POIFS_ENTRY = Decryptor.DEFAULT_POIFS_ENTRY;

	private ISecretKey secretKey;

	public abstract OutputStream GetDataStream(DirectoryNode dir);

	public abstract void ConfirmPassword(string password, byte[] keySpec, byte[] keySalt, byte[] verifier, byte[] verifierSalt, byte[] integritySalt);

	public abstract void ConfirmPassword(string password);

	public static Encryptor GetInstance(EncryptionInfo info)
	{
		return info.Encryptor;
	}

	public OutputStream GetDataStream(NPOIFSFileSystem fs)
	{
		return GetDataStream(fs.Root);
	}

	public OutputStream GetDataStream(OPOIFSFileSystem fs)
	{
		return GetDataStream(fs.Root);
	}

	public OutputStream GetDataStream(POIFSFileSystem fs)
	{
		return GetDataStream(fs.Root);
	}

	public ISecretKey GetSecretKey()
	{
		return secretKey;
	}

	protected void SetSecretKey(ISecretKey secretKey)
	{
		this.secretKey = secretKey;
	}
}
