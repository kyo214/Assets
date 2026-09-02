using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.OpenXml4Net.OPC.Internal;
using NPOI.OpenXml4Net.OPC.Internal.Marshallers;
using NPOI.OpenXml4Net.Util;
using NPOI.Openxml4Net.Exceptions;
using NPOI.Util;

namespace NPOI.OpenXml4Net.OPC;

public class ZipPackage : OPCPackage
{
	private static string MIMETYPE = "mimetype";

	private static string SETTINGS_XML = "settings.xml";

	private static POILogger logger = POILogFactory.GetLogger(typeof(ZipPackage));

	private ZipEntrySource zipArchive;

	private bool isStream;

	public bool IsExternalStream
	{
		get
		{
			return isStream;
		}
		set
		{
			isStream = value;
		}
	}

	public ZipEntrySource ZipArchive => zipArchive;

	public ZipPackage()
		: base(OPCPackage.defaultPackageAccess)
	{
		zipArchive = null;
		try
		{
			contentTypeManager = new ZipContentTypeManager(null, this);
		}
		catch (InvalidFormatException exception)
		{
			logger.Log(5, "Could not parse ZipPackage", exception);
		}
	}

	public ZipPackage(Stream in1, PackageAccess access)
		: base(access)
	{
		isStream = true;
		ZipInputStream inp = ZipHelper.OpenZipStream(in1);
		zipArchive = new ZipInputStreamZipEntrySource(inp);
	}

	public ZipPackage(string path, PackageAccess access)
		: this(new FileInfo(path), access)
	{
	}

	public ZipPackage(FileInfo file, PackageAccess access)
		: base(access)
	{
		ZipEntrySource zipEntrySource;
		try
		{
			zipEntrySource = new ZipFileZipEntrySource(ZipHelper.OpenZipFile(file));
		}
		catch (IOException innerException)
		{
			if (access == PackageAccess.WRITE)
			{
				throw new InvalidOperationException("Can't open the specified file: '" + file?.ToString() + "'", innerException);
			}
			logger.Log(7, "Error in zip file " + file?.ToString() + " - falling back to stream processing (i.e. ignoring zip central directory)");
			FileStream fileStream = null;
			ZipInputStream zipInputStream = null;
			try
			{
				fileStream = file.Create();
				zipInputStream = ZipHelper.OpenZipStream(fileStream);
				zipEntrySource = new ZipInputStreamZipEntrySource(zipInputStream);
			}
			catch (IOException innerException2)
			{
				if (zipInputStream != null)
				{
					try
					{
						zipInputStream.Close();
					}
					catch (IOException)
					{
						throw new InvalidOperationException("Can't open the specified file: '" + file?.ToString() + "' and couldn't close the file input stream", innerException);
					}
				}
				else if (fileStream != null)
				{
					try
					{
						fileStream.Close();
					}
					catch (IOException)
					{
						throw new InvalidOperationException("Can't open the specified file: '" + file?.ToString() + "' and couldn't close the file input stream", innerException);
					}
				}
				throw new InvalidOperationException("Can't open the specified file: '" + file?.ToString() + "'", innerException2);
			}
		}
		zipArchive = zipEntrySource;
	}

	public ZipPackage(ZipEntrySource zipEntry, PackageAccess access)
		: base(access)
	{
		zipArchive = zipEntry;
	}

	protected override PackagePart[] GetPartsImpl()
	{
		if (partList == null)
		{
			partList = new PackagePartCollection();
		}
		if (zipArchive == null)
		{
			PackagePart[] array = new PackagePart[partList.Values.Count];
			partList.Values.CopyTo(array, 0);
			return array;
		}
		IEnumerator entries = zipArchive.Entries;
		while (entries.MoveNext())
		{
			ZipEntry zipEntry = (ZipEntry)entries.Current;
			if (zipEntry.Name.ToLower().Equals("[Content_Types].xml".ToLower()))
			{
				try
				{
					contentTypeManager = new ZipContentTypeManager(ZipArchive.GetInputStream(zipEntry), this);
				}
				catch (IOException ex)
				{
					throw new InvalidFormatException(ex.Message, ex);
				}
				break;
			}
		}
		if (contentTypeManager == null)
		{
			int num = 0;
			bool flag = false;
			bool flag2 = false;
			entries = zipArchive.Entries;
			while (entries.MoveNext())
			{
				ZipEntry obj = entries.Current as ZipEntry;
				if (obj.Name.Equals(MIMETYPE))
				{
					flag = true;
				}
				if (obj.Name.Equals(SETTINGS_XML))
				{
					flag2 = true;
				}
				num++;
			}
			if (flag & flag2)
			{
				throw new ODFNotOfficeXmlFileException("The supplied data appears to be in ODF (Open Document) Format. Formats like these (eg ODS, ODP) are not supported, try Apache ODFToolkit");
			}
			if (num == 0)
			{
				throw new NotOfficeXmlFileException("No valid entries or contents found, this is not a valid OOXML (Office Open XML) file");
			}
			throw new InvalidFormatException("Package should contain a content type part [M1.13]");
		}
		entries = zipArchive.Entries;
		while (entries.MoveNext())
		{
			ZipEntry zipEntry2 = (ZipEntry)entries.Current;
			PackagePartName packagePartName = BuildPartName(zipEntry2);
			if (packagePartName == null)
			{
				continue;
			}
			string contentType = contentTypeManager.GetContentType(packagePartName);
			if (contentType != null && contentType.Equals(ContentTypes.RELATIONSHIPS_PART))
			{
				try
				{
					PackagePart value = new ZipPackagePart(this, zipEntry2, packagePartName, contentType);
					partList[packagePartName] = value;
				}
				catch (InvalidOperationException ex2)
				{
					throw new InvalidFormatException(ex2.Message, ex2);
				}
			}
		}
		entries = zipArchive.Entries;
		while (entries.MoveNext())
		{
			ZipEntry zipEntry3 = entries.Current as ZipEntry;
			PackagePartName packagePartName2 = BuildPartName(zipEntry3);
			if (packagePartName2 == null)
			{
				continue;
			}
			string contentType2 = contentTypeManager.GetContentType(packagePartName2);
			if (contentType2 == null || !contentType2.Equals(ContentTypes.RELATIONSHIPS_PART))
			{
				if (contentType2 == null)
				{
					throw new InvalidFormatException("The part " + packagePartName2.URI.OriginalString + " does not have any content type ! Rule: Package require content types when retrieving a part from a package. [M.1.14]");
				}
				try
				{
					PackagePart value2 = new ZipPackagePart(this, zipEntry3, packagePartName2, contentType2);
					partList[packagePartName2] = value2;
				}
				catch (InvalidOperationException ex3)
				{
					throw new InvalidFormatException(ex3.Message, ex3);
				}
			}
		}
		ZipPackagePart[] array2 = new ZipPackagePart[partList.Count];
		IList<PackagePart> values = partList.Values;
		PackagePart[] array3 = array2;
		values.CopyTo(array3, 0);
		return array2;
	}

	private PackagePartName BuildPartName(ZipEntry entry)
	{
		try
		{
			if (entry.Name.ToLower().Equals("[Content_Types].xml".ToLower()))
			{
				return null;
			}
			return PackagingUriHelper.CreatePartName(ZipHelper.GetOPCNameFromZipItemName(entry.Name));
		}
		catch (Exception)
		{
			return null;
		}
	}

	protected override PackagePart CreatePartImpl(PackagePartName partName, string contentType, bool loadRelationships)
	{
		if (contentType == null)
		{
			throw new ArgumentException("contentType");
		}
		if (partName == null)
		{
			throw new ArgumentException("partName");
		}
		try
		{
			return new MemoryPackagePart(this, partName, contentType, loadRelationships);
		}
		catch (InvalidFormatException)
		{
			return null;
		}
	}

	protected override void RemovePartImpl(PackagePartName partName)
	{
		if (partName == null)
		{
			throw new ArgumentException("partUri");
		}
	}

	protected override void FlushImpl()
	{
	}

	protected override void CloseImpl()
	{
		Flush();
		if (originalPackagePath == null || "".Equals(originalPackagePath))
		{
			return;
		}
		if (File.Exists(originalPackagePath))
		{
			string text = GenerateTempFileName(FileHelper.GetDirectory(originalPackagePath));
			FileInfo fileInfo = TempFile.CreateTempFile(text, ".tmp");
			try
			{
				Save(fileInfo.FullName);
				return;
			}
			finally
			{
				try
				{
					if (zipArchive != null)
					{
						zipArchive.Close();
					}
					FileHelper.CopyFile(fileInfo.FullName, originalPackagePath);
				}
				finally
				{
					File.Delete(fileInfo.FullName);
					logger.Log(5, "The temporary file: '" + text + "' cannot be deleted ! Make sure that no other application use it.");
				}
			}
		}
		throw new InvalidOperationException("Can't close a package not previously open with the open() method !");
	}

	private string GenerateTempFileName(string directory)
	{
		FileInfo fileInfo = null;
		string text = null;
		do
		{
			text = directory + "\\OpenXml4Net" + DateTime.Now.Ticks;
			fileInfo = new FileInfo(text);
		}
		while (File.Exists(text));
		return fileInfo.Name;
	}

	protected override void RevertImpl()
	{
		try
		{
			if (zipArchive != null)
			{
				zipArchive.Close();
			}
		}
		catch (IOException)
		{
		}
	}

	protected override PackagePart GetPartImpl(PackagePartName partName)
	{
		if (partList.ContainsKey(partName))
		{
			return partList[partName];
		}
		return null;
	}

	protected override void SaveImpl(Stream outputStream)
	{
		ThrowExceptionIfReadOnly();
		ZipOutputStream zipOutputStream = null;
		try
		{
			zipOutputStream = ((outputStream is ZipOutputStream) ? ((ZipOutputStream)outputStream) : new ZipOutputStream(outputStream));
			zipOutputStream.UseZip64 = UseZip64.Off;
			if (GetPartsByRelationshipType("http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties").Count == 0 && GetPartsByRelationshipType("http://schemas.openxmlformats.org/officedocument/2006/relationships/metadata/core-properties").Count == 0)
			{
				logger.Log(1, "Save core properties part");
				GetPackageProperties();
				AddPackagePart(packageProperties);
				relationships.AddRelationship(packageProperties.PartName.URI, TargetMode.Internal, "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties", null);
				if (!contentTypeManager.IsContentTypeRegister(ContentTypes.CORE_PROPERTIES_PART))
				{
					contentTypeManager.AddContentType(packageProperties.PartName, ContentTypes.CORE_PROPERTIES_PART);
				}
			}
			logger.Log(1, "Save package relationships");
			ZipPartMarshaller.MarshallRelationshipPart(base.Relationships, PackagingUriHelper.PACKAGE_RELATIONSHIPS_ROOT_PART_NAME, zipOutputStream);
			logger.Log(1, "Save content types part");
			contentTypeManager.Save(zipOutputStream);
			foreach (PackagePart part in GetParts())
			{
				if (part.IsRelationshipPart)
				{
					continue;
				}
				logger.Log(1, "Save part '" + ZipHelper.GetZipItemNameFromOPCName(part.PartName.Name) + "'");
				if (partMarshallers.ContainsKey(part._contentType))
				{
					PartMarshaller partMarshaller = partMarshallers[part._contentType];
					if (!partMarshaller.Marshall(part, zipOutputStream))
					{
						throw new OpenXml4NetException("The part " + part.PartName.URI?.ToString() + " fail to be saved in the stream with marshaller " + partMarshaller);
					}
				}
				else if (!defaultPartMarshaller.Marshall(part, zipOutputStream))
				{
					throw new OpenXml4NetException("The part " + part.PartName.URI?.ToString() + " fail to be saved in the stream with marshaller " + defaultPartMarshaller);
				}
			}
			if (isStream)
			{
				zipOutputStream.Finish();
			}
			else
			{
				zipOutputStream.Close();
			}
		}
		catch (OpenXML4NetRuntimeException ex)
		{
			throw ex;
		}
		catch (IOException ex2)
		{
			throw new OpenXML4NetRuntimeException("Fail to save: an error occurs while saving the package : " + ex2.Message, ex2);
		}
		catch (Exception ex3)
		{
			throw new OpenXML4NetRuntimeException("Fail to save: an error occurs while saving the package : " + ex3.Message, ex3);
		}
	}
}
