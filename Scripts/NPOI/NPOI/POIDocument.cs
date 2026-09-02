using System;
using System.Collections;
using System.IO;
using NPOI.HPSF;
using NPOI.POIFS.Crypt;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI;

[Serializable]
public abstract class POIDocument : ICloseable
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(POIDocument));

	protected SummaryInformation sInf;

	protected DocumentSummaryInformation dsInf;

	protected DirectoryNode directory;

	protected bool initialized;

	public DocumentSummaryInformation DocumentSummaryInformation
	{
		get
		{
			if (!initialized)
			{
				ReadProperties();
			}
			return dsInf;
		}
		set
		{
			dsInf = value;
		}
	}

	public SummaryInformation SummaryInformation
	{
		get
		{
			if (!initialized)
			{
				ReadProperties();
			}
			return sInf;
		}
		set
		{
			sInf = value;
		}
	}

	public DirectoryNode Directory => directory;

	protected internal void SetDirectoryNode(DirectoryNode directory)
	{
		this.directory = directory;
	}

	protected POIDocument(DirectoryNode dir)
	{
		directory = dir;
	}

	protected POIDocument(OPOIFSFileSystem fs)
		: this(fs.Root)
	{
	}

	public POIDocument(NPOIFSFileSystem fs)
		: this(fs.Root)
	{
	}

	protected POIDocument(POIFSFileSystem fs)
		: this(fs.Root)
	{
	}

	public void CreateInformationProperties()
	{
		if (!initialized)
		{
			ReadProperties();
		}
		if (sInf == null)
		{
			sInf = PropertySetFactory.CreateSummaryInformation();
		}
		if (dsInf == null)
		{
			dsInf = PropertySetFactory.CreateDocumentSummaryInformation();
		}
	}

	protected internal void ReadProperties()
	{
		PropertySet propertySet = GetPropertySet("\u0005DocumentSummaryInformation");
		if (propertySet != null && propertySet is DocumentSummaryInformation)
		{
			dsInf = (DocumentSummaryInformation)propertySet;
		}
		else if (propertySet != null)
		{
			logger.Log(5, "DocumentSummaryInformation property Set came back with wrong class - ", propertySet.GetType());
		}
		else
		{
			logger.Log(5, "DocumentSummaryInformation property set came back as null");
		}
		propertySet = GetPropertySet("\u0005SummaryInformation");
		if (propertySet is SummaryInformation)
		{
			sInf = (SummaryInformation)propertySet;
		}
		else if (propertySet != null)
		{
			logger.Log(5, "SummaryInformation property Set came back with wrong class - ", propertySet.GetType());
		}
		else
		{
			logger.Log(5, "SummaryInformation property set came back as null");
		}
		initialized = true;
	}

	protected PropertySet GetPropertySet(string setName)
	{
		return GetPropertySet(setName, null);
	}

	protected PropertySet GetPropertySet(string setName, EncryptionInfo encryptionInfo)
	{
		DirectoryNode root = directory;
		NPOIFSFileSystem nPOIFSFileSystem = null;
		string text = "getting";
		try
		{
			if (encryptionInfo != null)
			{
				text = "getting encrypted";
				InputStream dataStream = encryptionInfo.Decryptor.GetDataStream(directory);
				try
				{
					nPOIFSFileSystem = new NPOIFSFileSystem(dataStream);
					root = nPOIFSFileSystem.Root;
				}
				finally
				{
					dataStream.Close();
				}
			}
			if (root == null || !root.HasEntry(setName))
			{
				return null;
			}
			text = "getting";
			DocumentInputStream documentInputStream = root.CreateDocumentInputStream(root.GetEntry(setName));
			try
			{
				text = "creating";
				return PropertySetFactory.Create(documentInputStream);
			}
			finally
			{
				documentInputStream.Close();
			}
		}
		catch (Exception exception)
		{
			logger.Log(5, "Error " + text + " property set with name " + setName, exception);
			return null;
		}
		finally
		{
			if (nPOIFSFileSystem != null)
			{
				try
				{
					nPOIFSFileSystem.Close();
				}
				catch (IOException exception2)
				{
					logger.Log(5, "Error closing encrypted property poifs", exception2);
				}
			}
		}
	}

	protected internal void WriteProperties()
	{
		ValidateInPlaceWritePossible();
		WriteProperties(directory.FileSystem, null);
	}

	protected internal void WriteProperties(NPOIFSFileSystem outFS)
	{
		WriteProperties(outFS, null);
	}

	protected internal void WriteProperties(NPOIFSFileSystem outFS, IList writtenEntries)
	{
		if (sInf != null)
		{
			WritePropertySet("\u0005SummaryInformation", sInf, outFS);
			writtenEntries?.Add("\u0005SummaryInformation");
		}
		if (dsInf != null)
		{
			WritePropertySet("\u0005DocumentSummaryInformation", dsInf, outFS);
			writtenEntries?.Add("\u0005DocumentSummaryInformation");
		}
	}

	protected void WritePropertySet(string name, PropertySet Set, NPOIFSFileSystem outFS)
	{
		try
		{
			MutablePropertySet mutablePropertySet = new MutablePropertySet(Set);
			using MemoryStream memoryStream = new MemoryStream();
			mutablePropertySet.Write(memoryStream);
			using MemoryStream stream = new MemoryStream(memoryStream.ToArray());
			outFS.CreateOrUpdateDocument(stream, name);
		}
		catch (WritingNotSupportedException)
		{
		}
	}

	protected void ValidateInPlaceWritePossible()
	{
		if (directory == null)
		{
			throw new InvalidOperationException("Newly created Document, cannot save in-place");
		}
		if (directory.Parent != null)
		{
			throw new InvalidOperationException("This is not the root Document, cannot save embedded resource in-place");
		}
		if (directory.FileSystem == null || !directory.FileSystem.IsInPlaceWriteable())
		{
			throw new InvalidOperationException("Opened read-only or via an InputStream, a Writeable File is required");
		}
	}

	public abstract void Write();

	public abstract void Write(FileInfo newFile);

	public abstract void Write(Stream out1);

	public virtual void Close()
	{
		if (directory != null && directory.NFileSystem != null)
		{
			directory.NFileSystem.Close();
			directory = null;
		}
	}
}
