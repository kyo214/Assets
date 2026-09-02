using System.IO;
using System.Text;
using NPOI.POIFS.Crypt.Standard;
using NPOI.POIFS.EventFileSystem;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.POIFS.Crypt;

public class DataSpaceMapUtils
{
	public class POIFSWriterListenerImpl : POIFSWriterListener
	{
		private byte[] buf;

		public POIFSWriterListenerImpl(byte[] buf)
		{
			this.buf = buf;
		}

		public void ProcessPOIFSWriterEvent(POIFSWriterEvent event1)
		{
			try
			{
				event1.Stream.Write(buf, 0, event1.Limit);
			}
			catch (IOException cause)
			{
				throw new EncryptedDocumentException(cause);
			}
		}
	}

	public class DataSpaceMap : EncryptionRecord
	{
		private DataSpaceMapEntry[] entries;

		public DataSpaceMap(DataSpaceMapEntry[] entries)
		{
			this.entries = entries;
		}

		public DataSpaceMap(ILittleEndianInput is1)
		{
			is1.ReadInt();
			int num = is1.ReadInt();
			entries = new DataSpaceMapEntry[num];
			for (int i = 0; i < num; i++)
			{
				entries[i] = new DataSpaceMapEntry(is1);
			}
		}

		public void Write(LittleEndianByteArrayOutputStream os)
		{
			os.WriteInt(8);
			os.WriteInt(entries.Length);
			DataSpaceMapEntry[] array = entries;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Write(os);
			}
		}
	}

	public class DataSpaceMapEntry : EncryptionRecord
	{
		private int[] referenceComponentType;

		private string[] referenceComponent;

		private string dataSpaceName;

		public DataSpaceMapEntry(int[] referenceComponentType, string[] referenceComponent, string dataSpaceName)
		{
			this.referenceComponentType = referenceComponentType;
			this.referenceComponent = referenceComponent;
			this.dataSpaceName = dataSpaceName;
		}

		public DataSpaceMapEntry(ILittleEndianInput is1)
		{
			is1.ReadInt();
			int num = is1.ReadInt();
			referenceComponentType = new int[num];
			referenceComponent = new string[num];
			for (int i = 0; i < num; i++)
			{
				referenceComponentType[i] = is1.ReadInt();
				referenceComponent[i] = ReadUnicodeLPP4(is1);
			}
			dataSpaceName = ReadUnicodeLPP4(is1);
		}

		public void Write(LittleEndianByteArrayOutputStream os)
		{
			int writeIndex = os.WriteIndex;
			ILittleEndianOutput littleEndianOutput = os.CreateDelayedOutput(4);
			os.WriteInt(referenceComponent.Length);
			for (int i = 0; i < referenceComponent.Length; i++)
			{
				os.WriteInt(referenceComponentType[i]);
				WriteUnicodeLPP4(os, referenceComponent[i]);
			}
			WriteUnicodeLPP4(os, dataSpaceName);
			littleEndianOutput.WriteInt(os.WriteIndex - writeIndex);
		}
	}

	public class DataSpaceDefInition : EncryptionRecord
	{
		private string[] transformer;

		public DataSpaceDefInition(string[] transformer)
		{
			this.transformer = transformer;
		}

		public DataSpaceDefInition(ILittleEndianInput is1)
		{
			is1.ReadInt();
			int num = is1.ReadInt();
			transformer = new string[num];
			for (int i = 0; i < num; i++)
			{
				transformer[i] = ReadUnicodeLPP4(is1);
			}
		}

		public void Write(LittleEndianByteArrayOutputStream bos)
		{
			bos.WriteInt(8);
			bos.WriteInt(transformer.Length);
			string[] array = transformer;
			foreach (string @string in array)
			{
				WriteUnicodeLPP4(bos, @string);
			}
		}
	}

	public class IRMDSTransformInfo : EncryptionRecord
	{
		private TransformInfoHeader transformInfoHeader;

		private int extensibilityHeader;

		private string xrMLLicense;

		public IRMDSTransformInfo(TransformInfoHeader transformInfoHeader, int extensibilityHeader, string xrMLLicense)
		{
			this.transformInfoHeader = transformInfoHeader;
			this.extensibilityHeader = extensibilityHeader;
			this.xrMLLicense = xrMLLicense;
		}

		public IRMDSTransformInfo(ILittleEndianInput is1)
		{
			transformInfoHeader = new TransformInfoHeader(is1);
			extensibilityHeader = is1.ReadInt();
			xrMLLicense = ReadUtf8LPP4(is1);
		}

		public void Write(LittleEndianByteArrayOutputStream bos)
		{
			transformInfoHeader.Write(bos);
			bos.WriteInt(extensibilityHeader);
			WriteUtf8LPP4(bos, xrMLLicense);
			bos.WriteInt(4);
		}
	}

	public class TransformInfoHeader : EncryptionRecord
	{
		private int transformType;

		private string transformerId;

		private string transformerName;

		private int readerVersionMajor = 1;

		private int readerVersionMinor;

		private int updaterVersionMajor = 1;

		private int updaterVersionMinor;

		private int writerVersionMajor = 1;

		private int writerVersionMinor;

		public TransformInfoHeader(int transformType, string transformerId, string transformerName, int readerVersionMajor, int readerVersionMinor, int updaterVersionMajor, int updaterVersionMinor, int writerVersionMajor, int writerVersionMinor)
		{
			this.transformType = transformType;
			this.transformerId = transformerId;
			this.transformerName = transformerName;
			this.readerVersionMajor = readerVersionMajor;
			this.readerVersionMinor = readerVersionMinor;
			this.updaterVersionMajor = updaterVersionMajor;
			this.updaterVersionMinor = updaterVersionMinor;
			this.writerVersionMajor = writerVersionMajor;
			this.writerVersionMinor = writerVersionMinor;
		}

		public TransformInfoHeader(ILittleEndianInput is1)
		{
			is1.ReadInt();
			transformType = is1.ReadInt();
			transformerId = ReadUnicodeLPP4(is1);
			transformerName = ReadUnicodeLPP4(is1);
			readerVersionMajor = is1.ReadShort();
			readerVersionMinor = is1.ReadShort();
			updaterVersionMajor = is1.ReadShort();
			updaterVersionMinor = is1.ReadShort();
			writerVersionMajor = is1.ReadShort();
			writerVersionMinor = is1.ReadShort();
		}

		public void Write(LittleEndianByteArrayOutputStream bos)
		{
			int writeIndex = bos.WriteIndex;
			ILittleEndianOutput littleEndianOutput = bos.CreateDelayedOutput(4);
			bos.WriteInt(transformType);
			WriteUnicodeLPP4(bos, transformerId);
			littleEndianOutput.WriteInt(bos.WriteIndex - writeIndex);
			WriteUnicodeLPP4(bos, transformerName);
			bos.WriteShort(readerVersionMajor);
			bos.WriteShort(readerVersionMinor);
			bos.WriteShort(updaterVersionMajor);
			bos.WriteShort(updaterVersionMinor);
			bos.WriteShort(writerVersionMajor);
			bos.WriteShort(writerVersionMinor);
		}
	}

	public class DataSpaceVersionInfo : EncryptionRecord
	{
		private string featureIdentifier;

		private int readerVersionMajor = 1;

		private int readerVersionMinor;

		private int updaterVersionMajor = 1;

		private int updaterVersionMinor;

		private int writerVersionMajor = 1;

		private int writerVersionMinor;

		public DataSpaceVersionInfo(ILittleEndianInput is1)
		{
			featureIdentifier = ReadUnicodeLPP4(is1);
			readerVersionMajor = is1.ReadShort();
			readerVersionMinor = is1.ReadShort();
			updaterVersionMajor = is1.ReadShort();
			updaterVersionMinor = is1.ReadShort();
			writerVersionMajor = is1.ReadShort();
			writerVersionMinor = is1.ReadShort();
		}

		public DataSpaceVersionInfo(string featureIdentifier, int readerVersionMajor, int readerVersionMinor, int updaterVersionMajor, int updaterVersionMinor, int writerVersionMajor, int writerVersionMinor)
		{
			this.featureIdentifier = featureIdentifier;
			this.readerVersionMajor = readerVersionMajor;
			this.readerVersionMinor = readerVersionMinor;
			this.updaterVersionMajor = updaterVersionMajor;
			this.updaterVersionMinor = updaterVersionMinor;
			this.writerVersionMajor = writerVersionMajor;
			this.writerVersionMinor = writerVersionMinor;
		}

		public void Write(LittleEndianByteArrayOutputStream bos)
		{
			WriteUnicodeLPP4(bos, featureIdentifier);
			bos.WriteShort(readerVersionMajor);
			bos.WriteShort(readerVersionMinor);
			bos.WriteShort(updaterVersionMajor);
			bos.WriteShort(updaterVersionMinor);
			bos.WriteShort(writerVersionMajor);
			bos.WriteShort(writerVersionMinor);
		}
	}

	public static void AddDefaultDataSpace(DirectoryEntry dir)
	{
		DataSpaceMapEntry dataSpaceMapEntry = new DataSpaceMapEntry(new int[1], new string[1] { Decryptor.DEFAULT_POIFS_ENTRY }, "StrongEncryptionDataSpace");
		DataSpaceMap @out = new DataSpaceMap(new DataSpaceMapEntry[1] { dataSpaceMapEntry });
		CreateEncryptionEntry(dir, "\u0006DataSpaces/DataSpaceMap", @out);
		DataSpaceDefInition out2 = new DataSpaceDefInition(new string[1] { "StrongEncryptionTransform" });
		CreateEncryptionEntry(dir, "\u0006DataSpaces/DataSpaceInfo/StrongEncryptionDataSpace", out2);
		IRMDSTransformInfo out3 = new IRMDSTransformInfo(new TransformInfoHeader(1, "{FF9A3F03-56EF-4613-BDD5-5A41C1D07246}", "Microsoft.Container.EncryptionTransform", 1, 0, 1, 0, 1, 0), 0, null);
		CreateEncryptionEntry(dir, "\u0006DataSpaces/TransformInfo/StrongEncryptionTransform/\u0006Primary", out3);
		DataSpaceVersionInfo out4 = new DataSpaceVersionInfo("Microsoft.Container.DataSpaces", 1, 0, 1, 0, 1, 0);
		CreateEncryptionEntry(dir, "\u0006DataSpaces/Version", out4);
	}

	public static DocumentEntry CreateEncryptionEntry(DirectoryEntry dir, string path, EncryptionRecord out1)
	{
		string[] array = path.Split("/".ToCharArray());
		for (int i = 0; i < array.Length - 1; i++)
		{
			dir = (dir.HasEntry(array[i]) ? ((DirectoryEntry)dir.GetEntry(array[i])) : dir.CreateDirectory(array[i]));
		}
		byte[] buf = new byte[5000];
		LittleEndianByteArrayOutputStream littleEndianByteArrayOutputStream = new LittleEndianByteArrayOutputStream(buf, 0);
		out1.Write(littleEndianByteArrayOutputStream);
		string name = array[^1];
		if (dir.HasEntry(name))
		{
			dir.GetEntry(name).Delete();
		}
		return dir.CreateDocument(name, littleEndianByteArrayOutputStream.WriteIndex, new POIFSWriterListenerImpl(buf));
	}

	public static string ReadUnicodeLPP4(ILittleEndianInput is1)
	{
		int num = is1.ReadInt();
		if (num % 2 != 0)
		{
			throw new EncryptedDocumentException("UNICODE-LP-P4 structure is a multiple of 4 bytes. If PAdding is present, it MUST be exactly 2 bytes long");
		}
		string result = StringUtil.ReadUnicodeLE(is1, num / 2);
		if (num % 4 == 2)
		{
			is1.ReadShort();
		}
		return result;
	}

	public static void WriteUnicodeLPP4(ILittleEndianOutput os, string string1)
	{
		byte[] toUnicodeLE = StringUtil.GetToUnicodeLE(string1);
		os.WriteInt(toUnicodeLE.Length);
		os.Write(toUnicodeLE);
		if (toUnicodeLE.Length % 4 == 2)
		{
			os.WriteShort(0);
		}
	}

	public static string ReadUtf8LPP4(ILittleEndianInput is1)
	{
		int num = is1.ReadInt();
		if (num == 0 || num == 4)
		{
			is1.ReadInt();
			if (num != 0)
			{
				return "";
			}
			return null;
		}
		byte[] array = new byte[num];
		is1.ReadFully(array);
		int num2 = num % 4;
		if (num2 > 0)
		{
			for (int i = 0; i < 4 - num2; i++)
			{
				is1.ReadByte();
			}
		}
		return Encoding.UTF8.GetString(array, 0, array.Length);
	}

	public static void WriteUtf8LPP4(ILittleEndianOutput os, string str)
	{
		if (str == null || "".Equals(str))
		{
			os.WriteInt((str != null) ? 4 : 0);
			os.WriteInt(0);
			return;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(str);
		os.WriteInt(bytes.Length);
		os.Write(bytes);
		int num = bytes.Length % 4;
		if (num > 0)
		{
			for (int i = 0; i < 4 - num; i++)
			{
				os.WriteByte(0);
			}
		}
	}
}
