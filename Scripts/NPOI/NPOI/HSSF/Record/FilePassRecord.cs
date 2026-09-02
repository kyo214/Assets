using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class FilePassRecord : StandardRecord, ICloneable
{
	private interface KeyData : ICloneable
	{
		int DataSize { get; }

		void Read(RecordInputStream in1);

		void Serialize(ILittleEndianOutput out1);

		void AppendToString(StringBuilder buffer);
	}

	public class Rc4KeyData : KeyData, ICloneable
	{
		private const int ENCRYPTION_OTHER_RC4 = 1;

		private const int ENCRYPTION_OTHER_CAPI_2 = 2;

		private const int ENCRYPTION_OTHER_CAPI_3 = 3;

		private const int ENCRYPTION_OTHER_CAPI_4 = 4;

		private byte[] _salt;

		private byte[] _encryptedVerifier;

		private byte[] _encryptedVerifierHash;

		private int _encryptionInfo;

		private int _minorVersionNo;

		public int DataSize => 54;

		public byte[] Salt
		{
			get
			{
				return (byte[])_salt.Clone();
			}
			set
			{
				_salt = (byte[])value.Clone();
			}
		}

		public byte[] EncryptedVerifier
		{
			get
			{
				return (byte[])_encryptedVerifier.Clone();
			}
			set
			{
				_encryptedVerifier = (byte[])value.Clone();
			}
		}

		public byte[] EncryptedVerifierHash
		{
			get
			{
				return (byte[])_encryptedVerifierHash.Clone();
			}
			set
			{
				_encryptedVerifierHash = (byte[])value.Clone();
			}
		}

		public void Read(RecordInputStream in1)
		{
			_encryptionInfo = in1.ReadUShort();
			switch (_encryptionInfo)
			{
			case 2:
			case 3:
			case 4:
				throw new EncryptedDocumentException("HSSF does not currently support CryptoAPI encryption");
			default:
				throw new RecordFormatException("Unknown encryption info " + _encryptionInfo);
			case 1:
				_minorVersionNo = in1.ReadUShort();
				if (_minorVersionNo != 1)
				{
					throw new RecordFormatException("Unexpected VersionInfo number for RC4Header " + _minorVersionNo);
				}
				_salt = FilePassRecord.Read(in1, 16);
				_encryptedVerifier = FilePassRecord.Read(in1, 16);
				_encryptedVerifierHash = FilePassRecord.Read(in1, 16);
				break;
			}
		}

		public void Serialize(ILittleEndianOutput out1)
		{
			out1.WriteShort(_encryptionInfo);
			out1.WriteShort(_minorVersionNo);
			out1.Write(_salt);
			out1.Write(_encryptedVerifier);
			out1.Write(_encryptedVerifierHash);
		}

		public void AppendToString(StringBuilder buffer)
		{
			buffer.Append("    .rc4.info = ").Append(HexDump.ShortToHex(_encryptionInfo)).Append("\n");
			buffer.Append("    .rc4.ver  = ").Append(HexDump.ShortToHex(_minorVersionNo)).Append("\n");
			buffer.Append("    .rc4.salt = ").Append(HexDump.ToHex(_salt)).Append("\n");
			buffer.Append("    .rc4.verifier = ").Append(HexDump.ToHex(_encryptedVerifier)).Append("\n");
			buffer.Append("    .rc4.verifierHash = ").Append(HexDump.ToHex(_encryptedVerifierHash)).Append("\n");
		}

		public object Clone()
		{
			return new Rc4KeyData
			{
				_salt = (byte[])_salt.Clone(),
				_encryptedVerifier = (byte[])_encryptedVerifier.Clone(),
				_encryptedVerifierHash = (byte[])_encryptedVerifierHash.Clone(),
				_encryptionInfo = _encryptionInfo,
				_minorVersionNo = _minorVersionNo
			};
		}
	}

	public class XorKeyData : KeyData, ICloneable
	{
		private int _key;

		private int _verifier;

		public int DataSize => 6;

		public int Key
		{
			get
			{
				return _key;
			}
			set
			{
				_key = value;
			}
		}

		public int Verifier
		{
			get
			{
				return _verifier;
			}
			set
			{
				_verifier = value;
			}
		}

		public void Read(RecordInputStream in1)
		{
			_key = in1.ReadUShort();
			_verifier = in1.ReadUShort();
		}

		public void Serialize(ILittleEndianOutput out1)
		{
			out1.WriteShort(_key);
			out1.WriteShort(_verifier);
		}

		public void AppendToString(StringBuilder buffer)
		{
			buffer.Append("    .xor.key = ").Append(HexDump.IntToHex(_key)).Append("\n");
			buffer.Append("    .xor.verifier  = ").Append(HexDump.IntToHex(_verifier)).Append("\n");
		}

		public object Clone()
		{
			return new XorKeyData
			{
				_key = _key,
				_verifier = _verifier
			};
		}
	}

	public const short sid = 47;

	private const int ENCRYPTION_XOR = 0;

	private const int ENCRYPTION_OTHER = 1;

	private int _encryptionType;

	private KeyData _keyData;

	protected override int DataSize => _keyData.DataSize;

	public override short Sid => 47;

	private FilePassRecord(FilePassRecord other)
	{
		_encryptionType = other._encryptionType;
		_keyData = (KeyData)other._keyData.Clone();
	}

	public FilePassRecord(RecordInputStream in1)
	{
		_encryptionType = in1.ReadUShort();
		switch (_encryptionType)
		{
		case 0:
			_keyData = new XorKeyData();
			break;
		case 1:
			_keyData = new Rc4KeyData();
			break;
		default:
			throw new RecordFormatException("Unknown encryption type " + _encryptionType);
		}
		_keyData.Read(in1);
	}

	private static byte[] Read(RecordInputStream in1, int size)
	{
		byte[] array = new byte[size];
		in1.ReadFully(array);
		return array;
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(_encryptionType);
		_keyData.Serialize(out1);
	}

	public Rc4KeyData GetRc4KeyData()
	{
		if (!(_keyData is Rc4KeyData))
		{
			return null;
		}
		return (Rc4KeyData)_keyData;
	}

	public XorKeyData GetXorKeyData()
	{
		if (!(_keyData is XorKeyData))
		{
			return null;
		}
		return (XorKeyData)_keyData;
	}

	private Rc4KeyData CheckRc4()
	{
		return GetRc4KeyData() ?? throw new RecordFormatException("file pass record doesn't contain a rc4 key.");
	}

	public override object Clone()
	{
		return new FilePassRecord(this);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[FILEPASS]\n");
		stringBuilder.Append("    .type = ").Append(HexDump.ShortToHex(_encryptionType)).Append("\n");
		_keyData.AppendToString(stringBuilder);
		stringBuilder.Append("[/FILEPASS]\n");
		return stringBuilder.ToString();
	}
}
