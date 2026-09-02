using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.POIFS.Crypt;

public abstract class Decryptor
{
	public static string DEFAULT_PASSWORD = "VelvetSweatshop";

	public static string DEFAULT_POIFS_ENTRY = "EncryptedPackage";

	protected internal IEncryptionInfoBuilder builder;

	private ISecretKey secretKey;

	private byte[] verifier;

	private byte[] integrityHmacKey;

	private byte[] integrityHmacValue;

	protected Decryptor(IEncryptionInfoBuilder builder)
	{
		this.builder = builder;
	}

	public abstract InputStream GetDataStream(DirectoryNode dir);

	public abstract bool VerifyPassword(string password);

	public abstract long GetLength();

	public static Decryptor GetInstance(EncryptionInfo info)
	{
		return info.Decryptor ?? throw new EncryptedDocumentException("Unsupported version");
	}

	public InputStream GetDataStream(NPOIFSFileSystem fs)
	{
		return GetDataStream(fs.Root);
	}

	public InputStream GetDataStream(OPOIFSFileSystem fs)
	{
		return GetDataStream(fs.Root);
	}

	public InputStream GetDataStream(POIFSFileSystem fs)
	{
		return GetDataStream(fs.Root);
	}

	public byte[] GetVerifier()
	{
		return verifier;
	}

	public ISecretKey GetSecretKey()
	{
		return secretKey;
	}

	public byte[] GetIntegrityHmacKey()
	{
		return integrityHmacKey;
	}

	public byte[] GetIntegrityHmacValue()
	{
		return integrityHmacValue;
	}

	protected void SetSecretKey(ISecretKey secretKey)
	{
		this.secretKey = secretKey;
	}

	protected void SetVerifier(byte[] verifier)
	{
		this.verifier = verifier;
	}

	protected void SetIntegrityHmacKey(byte[] integrityHmacKey)
	{
		this.integrityHmacKey = integrityHmacKey;
	}

	protected void SetIntegrityHmacValue(byte[] integrityHmacValue)
	{
		this.integrityHmacValue = integrityHmacValue;
	}

	protected int GetBlockSizeInBytes()
	{
		return builder.GetHeader().BlockSize;
	}

	protected int GetKeySizeInBytes()
	{
		return builder.GetHeader().KeySize / 8;
	}
}
