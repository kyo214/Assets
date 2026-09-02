using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.POIFS.Macros;

public class VBAMacroReader : ICloseable
{
	protected class Module
	{
		public int? offset;

		public byte[] buf;

		public void Read(Stream in1)
		{
			MemoryStream memoryStream = new MemoryStream();
			IOUtils.Copy(in1, memoryStream);
			memoryStream.Close();
			buf = memoryStream.ToArray();
		}
	}

	protected class ModuleMap : Dictionary<string, Module>
	{
		public static Encoding charset;

		static ModuleMap()
		{
			charset = Encoding.GetEncoding(1252);
		}

		public Module Get(string key)
		{
			if (!ContainsKey(key))
			{
				return null;
			}
			return base[key];
		}

		public Module Put(string key, Module value)
		{
			Module result = null;
			if (ContainsKey(key))
			{
				result = base[key];
				base[key] = value;
			}
			else
			{
				Add(key, value);
			}
			return result;
		}
	}

	protected static string VBA_PROJECT_OOXML = "vbaProject.bin";

	protected static string VBA_PROJECT_POIFS = "VBA";

	private NPOIFSFileSystem fs;

	private const int EOF = -1;

	private const int VERSION_INDEPENDENT_TERMINATOR = 16;

	private const int VERSION_DEPENDENT_TERMINATOR = 43;

	private const int PROJECTVERSION = 9;

	private const int PROJECTCODEPAGE = 3;

	private const int STREAMNAME = 26;

	private const int MODULEOFFSET = 49;

	private const int MODULETYPE_PROCEDURAL = 33;

	private const int MODULETYPE_DOCUMENT_CLASS_OR_DESIGNER = 34;

	private const int PROJECTLCID = 2;

	public VBAMacroReader(InputStream rstream)
	{
		PushbackInputStream pushbackInputStream = new PushbackInputStream(rstream, 8);
		if (NPOIFSFileSystem.HasPOIFSHeader(IOUtils.PeekFirst8Bytes(pushbackInputStream)))
		{
			fs = new NPOIFSFileSystem(pushbackInputStream);
		}
		else
		{
			OpenOOXML(pushbackInputStream);
		}
	}

	public VBAMacroReader(FileInfo file)
	{
		try
		{
			fs = new NPOIFSFileSystem(file);
		}
		catch (OfficeXmlFileException)
		{
			OpenOOXML(file.OpenRead());
		}
	}

	public VBAMacroReader(NPOIFSFileSystem fs)
	{
		this.fs = fs;
	}

	private void OpenOOXML(Stream zipFile)
	{
		ZipInputStream zipInputStream = new ZipInputStream(zipFile);
		ZipEntry nextEntry;
		while ((nextEntry = zipInputStream.GetNextEntry()) != null)
		{
			if (nextEntry.Name.EndsWith(VBA_PROJECT_OOXML, StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					fs = new NPOIFSFileSystem(zipInputStream);
					return;
				}
				catch (IOException ex)
				{
					zipInputStream.Close();
					throw ex;
				}
			}
		}
		zipInputStream.Close();
		throw new ArgumentException("No VBA project found");
	}

	public void Close()
	{
		fs.Close();
		fs = null;
	}

	public Dictionary<string, string> ReadMacros()
	{
		ModuleMap moduleMap = new ModuleMap();
		FindMacros(fs.Root, moduleMap);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (KeyValuePair<string, Module> item in moduleMap)
		{
			Module value = item.Value;
			if (value.buf != null && value.buf.Length != 0)
			{
				dictionary.Add(item.Key, ModuleMap.charset.GetString(value.buf));
			}
		}
		return dictionary;
	}

	protected void FindMacros(DirectoryNode dir, ModuleMap modules)
	{
		if (VBA_PROJECT_POIFS.Equals(dir.Name, StringComparison.OrdinalIgnoreCase))
		{
			ReadMacros(dir, modules);
			return;
		}
		foreach (Entry item in dir)
		{
			if (item is DirectoryNode)
			{
				FindMacros((DirectoryNode)item, modules);
			}
		}
	}

	private static string ReadString(InputStream stream, int length, Encoding charset)
	{
		byte[] array = new byte[length];
		int count = stream.Read(array);
		return charset.GetString(array, 0, count);
	}

	private static void ReadModule(RLEDecompressingInputStream in1, string streamName, ModuleMap modules)
	{
		int num = in1.ReadInt();
		Module module = modules.Get(streamName);
		if (module == null)
		{
			module = new Module();
			module.offset = num;
			modules.Put(streamName, module);
		}
		else
		{
			InputStream inputStream = new RLEDecompressingInputStream(new MemoryStream(module.buf, num, module.buf.Length - num));
			module.Read(inputStream);
			inputStream.Close();
		}
	}

	private static void ReadModule(DocumentInputStream dis, string name, ModuleMap modules)
	{
		Module module = modules.Get(name);
		if (module == null)
		{
			module = new Module();
			modules.Put(name, module);
			module.Read(dis);
			return;
		}
		if (!module.offset.HasValue)
		{
			throw new IOException("Module offset for '" + name + "' was never Read.");
		}
		long num = dis.Skip(module.offset.Value);
		if (num != module.offset)
		{
			string[] obj = new string[5] { "tried to skip ", null, null, null, null };
			int? offset = module.offset;
			obj[1] = offset.ToString();
			obj[2] = " bytes, but actually skipped ";
			obj[3] = num.ToString();
			obj[4] = " bytes";
			throw new IOException(string.Concat(obj));
		}
		InputStream inputStream = new RLEDecompressingInputStream(dis);
		module.Read(inputStream);
		inputStream.Close();
	}

	private static void TrySkip(InputStream in1, long n)
	{
		long num = in1.Skip(n);
		if (num != n)
		{
			if (num < 0)
			{
				throw new IOException("Tried skipping " + n + " bytes, but no bytes were skipped. The end of the stream has been reached or the stream is closed.");
			}
			throw new IOException("Tried skipping " + n + " bytes, but only " + num + " bytes were skipped. This should never happen.");
		}
	}

	protected void ReadMacros(DirectoryNode macroDir, ModuleMap modules)
	{
		foreach (Entry item in macroDir)
		{
			if (!(item is DocumentNode))
			{
				continue;
			}
			string name = item.Name;
			DocumentInputStream documentInputStream = new DocumentInputStream((DocumentNode)item);
			try
			{
				if ("dir".Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					RLEDecompressingInputStream rLEDecompressingInputStream = new RLEDecompressingInputStream(documentInputStream);
					string streamName = null;
					int num = 0;
					try
					{
						while (true)
						{
							num = rLEDecompressingInputStream.ReadShort();
							if (-1 != num && 16 != num)
							{
								int num2 = rLEDecompressingInputStream.ReadInt();
								switch (num)
								{
								case 9:
									TrySkip(rLEDecompressingInputStream, 6L);
									break;
								case 3:
									ModuleMap.charset = Encoding.GetEncoding(rLEDecompressingInputStream.ReadShort());
									break;
								case 26:
									streamName = ReadString(rLEDecompressingInputStream, num2, ModuleMap.charset);
									break;
								case 49:
									ReadModule(rLEDecompressingInputStream, streamName, modules);
									break;
								default:
									TrySkip(rLEDecompressingInputStream, num2);
									break;
								}
								continue;
							}
							break;
						}
					}
					catch (IOException innerException)
					{
						throw new IOException("Error occurred while Reading macros at section id " + num + " (" + HexDump.ShortToHex(num)?.ToString() + ")", innerException);
					}
					finally
					{
						rLEDecompressingInputStream.Close();
					}
				}
				else if (!name.StartsWith("__SRP", StringComparison.OrdinalIgnoreCase) && !name.StartsWith("_VBA_PROJECT", StringComparison.OrdinalIgnoreCase))
				{
					ReadModule(documentInputStream, name, modules);
				}
			}
			finally
			{
				documentInputStream.Close();
			}
		}
	}
}
