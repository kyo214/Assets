using System;
using System.IO;
using System.Reflection;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.POIFS.Crypt;

public class EncryptionInfo
{
	public static BitField flagCryptoAPI = BitFieldFactory.GetInstance(4);

	public static BitField flagDocProps = BitFieldFactory.GetInstance(8);

	public static BitField flagExternal = BitFieldFactory.GetInstance(16);

	public static BitField flagAES = BitFieldFactory.GetInstance(32);

	private int _versionMajor;

	private int _versionMinor;

	private int _encryptionFlags;

	private EncryptionHeader _header;

	private EncryptionVerifier _verifier;

	private Decryptor _decryptor;

	private Encryptor _encryptor;

	public int VersionMajor => _versionMajor;

	public int VersionMinor => _versionMinor;

	public int EncryptionFlags => _encryptionFlags;

	public EncryptionHeader Header => _header;

	public EncryptionVerifier Verifier => _verifier;

	public Decryptor Decryptor => _decryptor;

	public Encryptor Encryptor => _encryptor;

	public EncryptionInfo(POIFSFileSystem fs)
		: this(fs.Root)
	{
	}

	public EncryptionInfo(OPOIFSFileSystem fs)
		: this(fs.Root)
	{
	}

	public EncryptionInfo(NPOIFSFileSystem fs)
		: this(fs.Root)
	{
	}

	public EncryptionInfo(DirectoryNode dir)
		: this(dir.CreateDocumentInputStream("EncryptionInfo"), isCryptoAPI: false)
	{
	}

	public EncryptionInfo(ILittleEndianInput dis, bool isCryptoAPI)
	{
		_versionMajor = dis.ReadShort();
		_versionMinor = dis.ReadShort();
		EncryptionMode encryptionMode;
		if (!isCryptoAPI && VersionMajor == EncryptionMode.BinaryRC4.VersionMajor && VersionMinor == EncryptionMode.BinaryRC4.VersionMinor)
		{
			encryptionMode = EncryptionMode.BinaryRC4;
			_encryptionFlags = -1;
		}
		else if (!isCryptoAPI && VersionMajor == EncryptionMode.Agile.VersionMajor && VersionMinor == EncryptionMode.Agile.VersionMinor)
		{
			encryptionMode = EncryptionMode.Agile;
			_encryptionFlags = dis.ReadInt();
		}
		else if (!isCryptoAPI && 2 <= VersionMajor && VersionMajor <= 4 && VersionMinor == EncryptionMode.Standard.VersionMinor)
		{
			encryptionMode = EncryptionMode.Standard;
			_encryptionFlags = dis.ReadInt();
		}
		else
		{
			if (!isCryptoAPI || 2 > VersionMajor || VersionMajor > 4 || VersionMinor != EncryptionMode.CryptoAPI.VersionMinor)
			{
				_encryptionFlags = dis.ReadInt();
				throw new EncryptedDocumentException("Unknown encryption: version major: " + VersionMajor + " / version minor: " + VersionMinor + " / fCrypto: " + flagCryptoAPI.IsSet(EncryptionFlags) + " / fExternal: " + flagExternal.IsSet(EncryptionFlags) + " / fDocProps: " + flagDocProps.IsSet(EncryptionFlags) + " / fAES: " + flagAES.IsSet(EncryptionFlags));
			}
			encryptionMode = EncryptionMode.CryptoAPI;
			_encryptionFlags = dis.ReadInt();
		}
		IEncryptionInfoBuilder builder;
		try
		{
			builder = GetBuilder(encryptionMode);
		}
		catch (Exception ex)
		{
			throw new IOException(ex.Message, ex);
		}
		builder.Initialize(this, dis);
		_header = builder.GetHeader();
		_verifier = builder.GetVerifier();
		_decryptor = builder.GetDecryptor();
		_encryptor = builder.GetEncryptor();
	}

	public EncryptionInfo(POIFSFileSystem fs, EncryptionMode encryptionMode)
		: this(encryptionMode)
	{
	}

	public EncryptionInfo(NPOIFSFileSystem fs, EncryptionMode encryptionMode)
		: this(encryptionMode)
	{
	}

	public EncryptionInfo(DirectoryNode dir, EncryptionMode encryptionMode)
		: this(encryptionMode)
	{
	}

	public EncryptionInfo(POIFSFileSystem fs, EncryptionMode encryptionMode, CipherAlgorithm cipherAlgorithm, HashAlgorithm hashAlgorithm, int keyBits, int blockSize, ChainingMode chainingMode)
		: this(encryptionMode, cipherAlgorithm, hashAlgorithm, keyBits, blockSize, chainingMode)
	{
	}

	public EncryptionInfo(NPOIFSFileSystem fs, EncryptionMode encryptionMode, CipherAlgorithm cipherAlgorithm, HashAlgorithm hashAlgorithm, int keyBits, int blockSize, ChainingMode chainingMode)
		: this(encryptionMode, cipherAlgorithm, hashAlgorithm, keyBits, blockSize, chainingMode)
	{
	}

	public EncryptionInfo(DirectoryNode dir, EncryptionMode encryptionMode, CipherAlgorithm cipherAlgorithm, HashAlgorithm hashAlgorithm, int keyBits, int blockSize, ChainingMode chainingMode)
		: this(encryptionMode, cipherAlgorithm, hashAlgorithm, keyBits, blockSize, chainingMode)
	{
	}

	public EncryptionInfo(EncryptionMode encryptionMode)
		: this(encryptionMode, null, null, -1, -1, null)
	{
	}

	public EncryptionInfo(EncryptionMode encryptionMode, CipherAlgorithm cipherAlgorithm, HashAlgorithm hashAlgorithm, int keyBits, int blockSize, ChainingMode chainingMode)
	{
		_versionMajor = encryptionMode.VersionMajor;
		_versionMinor = encryptionMode.VersionMinor;
		_encryptionFlags = encryptionMode.EncryptionFlags;
		IEncryptionInfoBuilder builder;
		try
		{
			builder = GetBuilder(encryptionMode);
		}
		catch (Exception cause)
		{
			throw new EncryptedDocumentException(cause);
		}
		builder.Initialize(this, cipherAlgorithm, hashAlgorithm, keyBits, blockSize, chainingMode);
		_header = builder.GetHeader();
		_verifier = builder.GetVerifier();
		_decryptor = builder.GetDecryptor();
		_encryptor = builder.GetEncryptor();
	}

	protected static IEncryptionInfoBuilder GetBuilder(EncryptionMode encryptionMode)
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		Type type = null;
		Assembly[] array = assemblies;
		for (int i = 0; i < array.Length; i++)
		{
			type = array[i].GetType(encryptionMode.Builder);
			if (type != null)
			{
				break;
			}
		}
		if (type == null)
		{
			throw new EncryptedDocumentException("Not found type " + encryptionMode.Builder);
		}
		return (IEncryptionInfoBuilder)type.Assembly.CreateInstance(encryptionMode.Builder);
	}
}
