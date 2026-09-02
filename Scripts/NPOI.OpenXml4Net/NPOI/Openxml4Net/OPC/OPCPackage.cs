using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.OpenXml4Net.OPC.Internal;
using NPOI.OpenXml4Net.OPC.Internal.Marshallers;
using NPOI.OpenXml4Net.OPC.Internal.Unmarshallers;
using NPOI.OpenXml4Net.Util;
using NPOI.Util;

namespace NPOI.OpenXml4Net.OPC;

public abstract class OPCPackage : RelationshipSource
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(OPCPackage));

	protected static PackageAccess defaultPackageAccess = PackageAccess.READ_WRITE;

	private PackageAccess packageAccess;

	protected PackagePartCollection partList;

	protected PackageRelationshipCollection relationships;

	protected SortedList<ContentType, PartMarshaller> partMarshallers;

	protected PartMarshaller defaultPartMarshaller;

	protected SortedList<ContentType, PartUnmarshaller> partUnmarshallers;

	protected PackagePropertiesPart packageProperties;

	protected ContentTypeManager contentTypeManager;

	protected bool isDirty;

	protected string originalPackagePath;

	protected Stream output;

	public PackageRelationshipCollection Relationships => GetRelationshipsHelper(null);

	public bool HasRelationships => relationships.Size > 0;

	public OPCPackage(PackageAccess access)
	{
		if (GetType() != typeof(ZipPackage))
		{
			throw new ArgumentException("PackageBase may not be subclassed");
		}
		Init();
		packageAccess = access;
	}

	private void Init()
	{
		partMarshallers = new SortedList<ContentType, PartMarshaller>(5);
		partUnmarshallers = new SortedList<ContentType, PartUnmarshaller>(2);
		try
		{
			partUnmarshallers.Add(new ContentType(ContentTypes.CORE_PROPERTIES_PART), new PackagePropertiesUnmarshaller());
			defaultPartMarshaller = new DefaultMarshaller();
			partMarshallers.Add(new ContentType(ContentTypes.CORE_PROPERTIES_PART), new ZipPackagePropertiesMarshaller());
		}
		catch (InvalidFormatException)
		{
			throw new OpenXml4NetException("Package.init() : this exception should never happen, if you read this message please send a mail to the developers team.");
		}
	}

	public static OPCPackage Open(string path)
	{
		return Open(path, defaultPackageAccess);
	}

	public static OPCPackage Open(FileInfo file)
	{
		return Open(file, defaultPackageAccess);
	}

	public static OPCPackage Open(ZipEntrySource zipEntry)
	{
		OPCPackage oPCPackage = new ZipPackage(zipEntry, PackageAccess.READ);
		try
		{
			if (oPCPackage.partList == null)
			{
				oPCPackage.GetParts();
			}
			return oPCPackage;
		}
		catch (InvalidFormatException ex)
		{
			try
			{
				oPCPackage.Close();
			}
			catch (IOException ex2)
			{
				throw new InvalidOperationException(ex2.Message, ex);
			}
			throw ex;
		}
		catch (RuntimeException ex3)
		{
			try
			{
				oPCPackage.Close();
			}
			catch (IOException ex4)
			{
				throw new InvalidOperationException(ex4.Message, ex3);
			}
			throw ex3;
		}
	}

	public static OPCPackage Open(string path, PackageAccess access)
	{
		if (path == null || "".Equals(path.Trim()))
		{
			throw new ArgumentException("'path' must be given");
		}
		if (new DirectoryInfo(path).Exists)
		{
			throw new ArgumentException("path must not be a directory");
		}
		OPCPackage oPCPackage = new ZipPackage(path, access);
		bool flag = false;
		if (oPCPackage.partList == null && access != PackageAccess.WRITE)
		{
			try
			{
				oPCPackage.GetParts();
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					try
					{
						oPCPackage.Close();
					}
					catch (IOException innerException)
					{
						throw new InvalidOperationException("Could not close OPCPackage while cleaning up", innerException);
					}
				}
			}
		}
		oPCPackage.originalPackagePath = new DirectoryInfo(path).FullName;
		return oPCPackage;
	}

	public static OPCPackage Open(FileInfo file, PackageAccess access)
	{
		if (file == null)
		{
			throw new ArgumentNullException("'file' must be given");
		}
		if (new DirectoryInfo(file.FullName).Exists)
		{
			throw new ArgumentException("file must not be a directory");
		}
		OPCPackage oPCPackage = new ZipPackage(file, access);
		try
		{
			if (oPCPackage.partList == null && access != PackageAccess.WRITE)
			{
				oPCPackage.GetParts();
			}
			oPCPackage.originalPackagePath = file.FullName;
			return oPCPackage;
		}
		catch (InvalidFormatException ex)
		{
			try
			{
				oPCPackage.Close();
			}
			catch (IOException ex2)
			{
				throw new InvalidOperationException(ex2.Message, ex);
			}
			throw ex;
		}
		catch (RuntimeException ex3)
		{
			try
			{
				oPCPackage.Close();
			}
			catch (IOException ex4)
			{
				throw new InvalidOperationException(ex4.Message, ex3);
			}
			throw ex3;
		}
	}

	public static OPCPackage Open(Stream in1)
	{
		OPCPackage oPCPackage = new ZipPackage(in1, PackageAccess.READ_WRITE);
		if (oPCPackage.partList == null)
		{
			oPCPackage.GetParts();
		}
		return oPCPackage;
	}

	public static OPCPackage Open(Stream in1, bool bReadonly)
	{
		OPCPackage oPCPackage = new ZipPackage(in1, (!bReadonly) ? PackageAccess.READ_WRITE : PackageAccess.READ);
		if (oPCPackage.partList == null)
		{
			oPCPackage.GetParts();
		}
		return oPCPackage;
	}

	public static OPCPackage OpenOrCreate(string path)
	{
		OPCPackage oPCPackage = null;
		if (File.Exists(path))
		{
			return Open(path);
		}
		return Create(path);
	}

	public static OPCPackage Create(string path)
	{
		if (new DirectoryInfo(path).Exists)
		{
			throw new ArgumentException("file");
		}
		if (File.Exists(path))
		{
			throw new InvalidOperationException("This package (or file) already exists : use the open() method or delete the file.");
		}
		ZipPackage obj = new ZipPackage
		{
			originalPackagePath = new FileInfo(path).FullName
		};
		ConfigurePackage(obj);
		return obj;
	}

	public static OPCPackage Create(Stream output)
	{
		ZipPackage obj = new ZipPackage
		{
			originalPackagePath = null,
			output = output
		};
		ConfigurePackage(obj);
		return obj;
	}

	private static void ConfigurePackage(OPCPackage pkg)
	{
		try
		{
			pkg.contentTypeManager = new ZipContentTypeManager(null, pkg);
			pkg.contentTypeManager.AddContentType(PackagingUriHelper.CreatePartName(PackagingUriHelper.PACKAGE_RELATIONSHIPS_ROOT_URI), ContentTypes.RELATIONSHIPS_PART);
			pkg.contentTypeManager.AddContentType(PackagingUriHelper.CreatePartName("/default.xml"), ContentTypes.PLAIN_OLD_XML);
			pkg.packageProperties = new PackagePropertiesPart(pkg, PackagingUriHelper.CORE_PROPERTIES_PART_NAME);
			pkg.packageProperties.SetCreatorProperty("Generated by OpenXml4Net");
			pkg.packageProperties.SetCreatedProperty(DateTime.Now);
		}
		catch (InvalidFormatException)
		{
			throw;
		}
	}

	public void Flush()
	{
		ThrowExceptionIfReadOnly();
		if (packageProperties != null)
		{
			packageProperties.Flush();
		}
		FlushImpl();
	}

	public void Close()
	{
		if (packageAccess == PackageAccess.READ)
		{
			logger.Log(5, "The close() method is intended to SAVE a package. This package is open in READ ONLY mode, use the revert() method instead !");
			Revert();
			return;
		}
		if (contentTypeManager == null)
		{
			logger.Log(5, "Unable to call close() on a package that hasn't been fully opened yet");
			Revert();
			return;
		}
		if (originalPackagePath != null && !"".Equals(originalPackagePath.Trim()))
		{
			FileInfo fileInfo = new FileInfo(originalPackagePath);
			if (!File.Exists(originalPackagePath) || !originalPackagePath.Equals(fileInfo.FullName, StringComparison.InvariantCultureIgnoreCase))
			{
				Save(originalPackagePath);
			}
			else
			{
				CloseImpl();
			}
		}
		else if (output != null)
		{
			Save(output);
		}
		contentTypeManager.ClearAll();
	}

	public void Revert()
	{
		RevertImpl();
	}

	public void AddThumbnail(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentException("path");
		}
		string filename = path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
		FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		try
		{
			AddThumbnail(filename, fileStream);
		}
		finally
		{
			fileStream.Close();
		}
	}

	public void AddThumbnail(string filename, Stream data)
	{
		if (string.IsNullOrEmpty(filename))
		{
			throw new ArgumentException("filename");
		}
		string contentTypeFromFileExtension = ContentTypes.GetContentTypeFromFileExtension(filename);
		PackagePartName packagePartName = null;
		try
		{
			packagePartName = PackagingUriHelper.CreatePartName("/docProps/" + filename);
		}
		catch (InvalidFormatException)
		{
			string partName = "/docProps/thumbnail" + filename.Substring(filename.LastIndexOf(".") + 1);
			try
			{
				packagePartName = PackagingUriHelper.CreatePartName(partName);
			}
			catch (InvalidFormatException)
			{
				throw new InvalidOperationException("Can't add a thumbnail file named '" + filename + "'");
			}
		}
		if (GetPart(packagePartName) != null)
		{
			throw new InvalidOperationException("You already add a thumbnail named '" + filename + "'");
		}
		PackagePart packagePart = CreatePart(packagePartName, contentTypeFromFileExtension, loadRelationships: false);
		AddRelationship(packagePartName, TargetMode.Internal, "http://schemas.openxmlformats.org/package/2006/relationships/metadata/thumbnail");
		StreamHelper.CopyStream(data, packagePart.GetOutputStream());
	}

	internal void ThrowExceptionIfReadOnly()
	{
		if (packageAccess == PackageAccess.READ)
		{
			throw new InvalidOperationException("Operation not allowed, document open in read only mode!");
		}
	}

	internal void ThrowExceptionIfWriteOnly()
	{
		if (packageAccess == PackageAccess.WRITE)
		{
			throw new InvalidOperationException("Operation not allowed, document open in write only mode!");
		}
	}

	public PackageProperties GetPackageProperties()
	{
		ThrowExceptionIfWriteOnly();
		if (packageProperties == null)
		{
			packageProperties = new PackagePropertiesPart(this, PackagingUriHelper.CORE_PROPERTIES_PART_NAME);
		}
		return packageProperties;
	}

	public bool PartExists(Uri uri)
	{
		if (uri.IsAbsoluteUri)
		{
			return false;
		}
		return GetPartImpl(new PackagePartName(uri.OriginalString, checkConformance: true)) != null;
	}

	public PackagePart GetPart(Uri uri)
	{
		ThrowExceptionIfWriteOnly();
		PackagePartName packagePartName = new PackagePartName(uri.ToString(), checkConformance: true);
		if (packagePartName == null)
		{
			throw new ArgumentException("PartName");
		}
		if (partList == null)
		{
			try
			{
				GetParts();
			}
			catch (InvalidFormatException)
			{
				return null;
			}
		}
		return GetPartImpl(packagePartName);
	}

	public PackagePart GetPart(PackagePartName partName)
	{
		ThrowExceptionIfWriteOnly();
		if (partName == null)
		{
			throw new ArgumentException("PartName");
		}
		if (partList == null)
		{
			try
			{
				GetParts();
			}
			catch (InvalidFormatException)
			{
				return null;
			}
		}
		return GetPartImpl(partName);
	}

	public List<PackagePart> GetPartsByContentType(string contentType)
	{
		List<PackagePart> list = new List<PackagePart>();
		foreach (PackagePart value in partList.Values)
		{
			if (value.ContentType.Equals(contentType))
			{
				list.Add(value);
			}
		}
		list.Sort();
		return list;
	}

	public List<PackagePart> GetPartsByRelationshipType(string relationshipType)
	{
		if (relationshipType == null)
		{
			throw new ArgumentException("relationshipType");
		}
		List<PackagePart> list = new List<PackagePart>();
		foreach (PackageRelationship item in GetRelationshipsByType(relationshipType))
		{
			PackagePart part = GetPart(item);
			if (part != null)
			{
				list.Add(part);
			}
		}
		list.Sort();
		return list;
	}

	public List<PackagePart> GetPartsByName(Regex namePattern)
	{
		if (namePattern == null)
		{
			throw new ArgumentException("name pattern must not be null");
		}
		List<PackagePart> list = new List<PackagePart>();
		foreach (PackagePart value in partList.Values)
		{
			string name = value.PartName.Name;
			if (namePattern.IsMatch(name))
			{
				list.Add(value);
			}
		}
		list.Sort();
		return list;
	}

	public PackagePart GetPart(PackageRelationship partRel)
	{
		PackagePart result = null;
		EnsureRelationships();
		foreach (PackageRelationship relationship in relationships)
		{
			if (relationship.RelationshipType.Equals(partRel.RelationshipType))
			{
				try
				{
					result = GetPart(PackagingUriHelper.CreatePartName(relationship.TargetUri));
				}
				catch (InvalidFormatException)
				{
					continue;
				}
				break;
			}
		}
		return result;
	}

	public List<PackagePart> GetParts()
	{
		ThrowExceptionIfWriteOnly();
		if (partList == null)
		{
			bool flag = false;
			bool flag2 = true;
			PackagePart[] partsImpl = GetPartsImpl();
			partList = new PackagePartCollection();
			PackagePart[] array = partsImpl;
			foreach (PackagePart packagePart in array)
			{
				bool flag3 = false;
				foreach (PackagePartName key in partList.Keys)
				{
					if (packagePart.PartName.Name.StartsWith(key.Name))
					{
						flag3 = true;
						break;
					}
				}
				if (flag3)
				{
					throw new InvalidFormatException("A part with the name '" + packagePart.PartName?.ToString() + "' already exist : Packages shall not contain equivalent part names and package implementers shall neither create nor recognize packages with equivalent part names. [M1.12]");
				}
				if (packagePart.ContentType.Equals(ContentTypes.CORE_PROPERTIES_PART))
				{
					if (!flag)
					{
						flag = true;
					}
					else
					{
						Console.WriteLine("OPC Compliance error [M4.1]: there is more than one core properties relationship in the package ! POI will use only the first, but other software may reject this file.");
					}
				}
				if (partUnmarshallers.ContainsKey(packagePart._contentType))
				{
					PartUnmarshaller partUnmarshaller = partUnmarshallers[packagePart._contentType];
					UnmarshallContext context = new UnmarshallContext(this, packagePart.PartName);
					try
					{
						PackagePart packagePart2 = partUnmarshaller.Unmarshall(context, packagePart.GetInputStream());
						partList[packagePart2.PartName] = packagePart2;
						if ((packagePart2 is PackagePropertiesPart) & flag & flag2)
						{
							packageProperties = (PackagePropertiesPart)packagePart2;
							flag2 = false;
						}
					}
					catch (IOException)
					{
						logger.Log(5, "Unmarshall operation : IOException for " + packagePart.PartName);
					}
					catch (InvalidOperationException ex2)
					{
						throw new InvalidFormatException(ex2.Message);
					}
				}
				else
				{
					try
					{
						partList[packagePart.PartName] = packagePart;
					}
					catch (InvalidOperationException ex3)
					{
						throw new InvalidFormatException(ex3.Message);
					}
				}
			}
		}
		return new List<PackagePart>(partList.Values);
	}

	public PackagePart CreatePart(Uri partName, string contentType)
	{
		return CreatePart(new PackagePartName(partName.OriginalString, checkConformance: true), contentType, loadRelationships: true);
	}

	public PackagePart CreatePart(PackagePartName partName, string contentType)
	{
		return CreatePart(partName, contentType, loadRelationships: true);
	}

	public PackagePart CreatePart(PackagePartName partName, string contentType, bool loadRelationships)
	{
		ThrowExceptionIfReadOnly();
		if (partName == null)
		{
			throw new ArgumentException("PartName");
		}
		if (contentType == null || contentType == "")
		{
			throw new ArgumentException("contentType");
		}
		bool flag = false;
		bool flag2 = false;
		foreach (PackagePartName key in partList.Keys)
		{
			if (partName.Name.StartsWith(key.Name))
			{
				flag = true;
				if (partList[key].IsDeleted)
				{
					flag2 = true;
				}
				break;
			}
		}
		if (flag && !flag2)
		{
			throw new PartAlreadyExistsException("A part with the name '" + partName.Name + "' already exists : Packages shall not contain equivalent part names and package implementers shall neither create nor recognize packages with equivalent part names. [M1.12]");
		}
		if (contentType == ContentTypes.CORE_PROPERTIES_PART && packageProperties != null)
		{
			throw new InvalidOperationException("OPC Compliance error [M4.1]: you try to add more than one core properties relationship in the package !");
		}
		PackagePart packagePart = CreatePartImpl(partName, contentType, loadRelationships);
		contentTypeManager.AddContentType(partName, contentType);
		partList[partName] = packagePart;
		isDirty = true;
		return packagePart;
	}

	public PackagePart CreatePart(PackagePartName partName, string contentType, MemoryStream content)
	{
		PackagePart packagePart = CreatePart(partName, contentType);
		if (packagePart == null)
		{
			return null;
		}
		if (content != null)
		{
			try
			{
				Stream outputStream = packagePart.GetOutputStream();
				if (outputStream == null)
				{
					return null;
				}
				outputStream.Write(content.ToArray(), 0, (int)content.Length);
				outputStream.Close();
				return packagePart;
			}
			catch (IOException)
			{
				return null;
			}
		}
		return null;
	}

	protected PackagePart AddPackagePart(PackagePart part)
	{
		ThrowExceptionIfReadOnly();
		if (part == null)
		{
			throw new ArgumentException("part");
		}
		if (partList.ContainsKey(part.PartName))
		{
			if (!partList[part.PartName].IsDeleted)
			{
				throw new InvalidOperationException("A part with the name '" + part.PartName.Name + "' already exists : Packages shall not contain equivalent part names and package implementers shall neither Create nor recognize packages with equivalent part names. [M1.12]");
			}
			part.IsDeleted = false;
			partList.Remove(part.PartName);
		}
		partList[part.PartName] = part;
		isDirty = true;
		return part;
	}

	public void RemovePart(PackagePart part)
	{
		if (part != null)
		{
			RemovePart(part.PartName);
		}
	}

	public void RemovePart(PackagePartName PartName)
	{
		ThrowExceptionIfReadOnly();
		if (PartName == null || !ContainPart(PartName))
		{
			throw new ArgumentException("PartName");
		}
		if (partList.ContainsKey(PartName))
		{
			partList[PartName].IsDeleted = true;
			RemovePartImpl(PartName);
			partList.Remove(PartName);
		}
		else
		{
			RemovePartImpl(PartName);
		}
		contentTypeManager.RemoveContentType(PartName);
		if (PartName.IsRelationshipPartURI())
		{
			Uri sourcePartUriFromRelationshipPartUri = PackagingUriHelper.GetSourcePartUriFromRelationshipPartUri(PartName.URI);
			PackagePartName packagePartName;
			try
			{
				packagePartName = PackagingUriHelper.CreatePartName(sourcePartUriFromRelationshipPartUri);
			}
			catch (InvalidFormatException)
			{
				logger.Log(7, "Part name URI '" + sourcePartUriFromRelationshipPartUri?.ToString() + "' is not valid ! This message is not intended to be displayed !");
				return;
			}
			if (packagePartName.URI.Equals(PackagingUriHelper.PACKAGE_ROOT_URI))
			{
				ClearRelationships();
			}
			else if (ContainPart(packagePartName))
			{
				GetPart(packagePartName)?.ClearRelationships();
			}
		}
		isDirty = true;
	}

	public void RemovePartRecursive(PackagePartName PartName)
	{
		PackagePart packagePart = partList[PackagingUriHelper.GetRelationshipPartName(PartName)];
		PackagePart packagePart2 = partList[PartName];
		if (packagePart != null)
		{
			foreach (PackageRelationship item in new PackageRelationshipCollection(packagePart2))
			{
				PackagePartName partName = PackagingUriHelper.CreatePartName(PackagingUriHelper.ResolvePartUri(item.SourceUri, item.TargetUri));
				RemovePart(partName);
			}
			RemovePart(packagePart.PartName);
		}
		RemovePart(packagePart2.PartName);
	}

	public void DeletePart(Uri uri)
	{
		PackagePartName packagePartName = new PackagePartName(uri.ToString(), checkConformance: true);
		if (packagePartName == null)
		{
			throw new ArgumentException("PartName");
		}
		RemovePart(packagePartName);
		RemovePart(PackagingUriHelper.GetRelationshipPartName(packagePartName));
	}

	public void DeletePart(PackagePartName partName)
	{
		if (partName == null)
		{
			throw new ArgumentException("PartName");
		}
		RemovePart(partName);
		RemovePart(PackagingUriHelper.GetRelationshipPartName(partName));
	}

	public void DeletePartRecursive(PackagePartName partName)
	{
		if (partName == null || !ContainPart(partName))
		{
			throw new ArgumentException("PartName");
		}
		PackagePart part = GetPart(partName);
		RemovePart(partName);
		try
		{
			foreach (PackageRelationship relationship in part.Relationships)
			{
				PackagePartName partName2 = PackagingUriHelper.CreatePartName(PackagingUriHelper.ResolvePartUri(partName.URI, relationship.TargetUri));
				DeletePartRecursive(partName2);
			}
		}
		catch (InvalidFormatException ex)
		{
			logger.Log(5, "An exception occurs while deleting part '" + partName.Name + "'. Some parts may remain in the package. - " + ex.Message);
			return;
		}
		PackagePartName relationshipPartName = PackagingUriHelper.GetRelationshipPartName(partName);
		if (relationshipPartName != null && ContainPart(relationshipPartName))
		{
			RemovePart(relationshipPartName);
		}
	}

	public bool ContainPart(PackagePartName partName)
	{
		return GetPart(partName) != null;
	}

	public PackageRelationship AddRelationship(PackagePartName targetPartName, TargetMode targetMode, string relationshipType, string relID)
	{
		if (relationshipType.Equals("http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties") && packageProperties != null)
		{
			throw new InvalidOperationException("OPC Compliance error [M4.1]: can't add another core properties part ! Use the built-in package method instead.");
		}
		if (targetPartName.IsRelationshipPartURI())
		{
			throw new InvalidOperationException("Rule M1.25: The Relationships part shall not have relationships to any other part.");
		}
		EnsureRelationships();
		PackageRelationship result = relationships.AddRelationship(targetPartName.URI, targetMode, relationshipType, relID);
		isDirty = true;
		return result;
	}

	public PackageRelationship AddRelationship(PackagePartName targetPartName, TargetMode targetMode, string relationshipType)
	{
		return AddRelationship(targetPartName, targetMode, relationshipType, null);
	}

	public PackageRelationship AddExternalRelationship(string target, string relationshipType)
	{
		return AddExternalRelationship(target, relationshipType, null);
	}

	public PackageRelationship AddExternalRelationship(string target, string relationshipType, string id)
	{
		if (target == null)
		{
			throw new ArgumentException("target");
		}
		if (relationshipType == null)
		{
			throw new ArgumentException("relationshipType");
		}
		Uri targetUri;
		try
		{
			targetUri = PackagingUriHelper.ParseUri(target, UriKind.Absolute);
		}
		catch (UriFormatException ex)
		{
			throw new ArgumentException("Invalid target - " + ex);
		}
		EnsureRelationships();
		PackageRelationship result = relationships.AddRelationship(targetUri, TargetMode.External, relationshipType, id);
		isDirty = true;
		return result;
	}

	public void RemoveRelationship(string id)
	{
		if (relationships != null)
		{
			relationships.RemoveRelationship(id);
			isDirty = true;
		}
	}

	public PackageRelationshipCollection GetRelationshipsByType(string relationshipType)
	{
		ThrowExceptionIfWriteOnly();
		if (relationshipType == null)
		{
			throw new ArgumentException("relationshipType");
		}
		return GetRelationshipsHelper(relationshipType);
	}

	private PackageRelationshipCollection GetRelationshipsHelper(string id)
	{
		ThrowExceptionIfWriteOnly();
		EnsureRelationships();
		return relationships.GetRelationships(id);
	}

	public void ClearRelationships()
	{
		if (relationships != null)
		{
			relationships.Clear();
			isDirty = true;
		}
	}

	public void EnsureRelationships()
	{
		if (relationships == null)
		{
			try
			{
				relationships = new PackageRelationshipCollection(this);
			}
			catch (InvalidFormatException)
			{
				relationships = new PackageRelationshipCollection();
			}
		}
	}

	public PackageRelationship GetRelationship(string id)
	{
		return relationships.GetRelationshipByID(id);
	}

	public bool IsRelationshipExists(PackageRelationship rel)
	{
		foreach (PackageRelationship relationship in Relationships)
		{
			if (relationship == rel)
			{
				return true;
			}
		}
		return false;
	}

	public void AddMarshaller(string contentType, PartMarshaller marshaller)
	{
		try
		{
			partMarshallers[new ContentType(contentType)] = marshaller;
		}
		catch (InvalidFormatException ex)
		{
			logger.Log(5, "The specified content type is not valid: '" + ex.Message + "'. The marshaller will not be Added !");
		}
	}

	public void AddUnmarshaller(string contentType, PartUnmarshaller unmarshaller)
	{
		try
		{
			partUnmarshallers[new ContentType(contentType)] = unmarshaller;
		}
		catch (InvalidFormatException ex)
		{
			logger.Log(5, "The specified content type is not valid: '" + ex.Message + "'. The unmarshaller will not be Added !");
		}
	}

	public void RemoveMarshaller(string contentType)
	{
		partMarshallers.Remove(new ContentType(contentType));
	}

	public void RemoveUnmarshaller(string contentType)
	{
		partUnmarshallers.Remove(new ContentType(contentType));
	}

	public PackageAccess GetPackageAccess()
	{
		return packageAccess;
	}

	public bool ValidatePackage(OPCPackage pkg)
	{
		throw new InvalidOperationException("Not implemented yet !!!");
	}

	public void Save(string path)
	{
		if (path == null)
		{
			throw new ArgumentException("targetFile");
		}
		ThrowExceptionIfReadOnly();
		FileStream fileStream = null;
		try
		{
			fileStream = new FileStream(path, FileMode.OpenOrCreate);
		}
		catch (IOException ex)
		{
			throw new IOException(ex.Message, ex);
		}
		try
		{
			Save(fileStream);
		}
		finally
		{
			fileStream.Close();
		}
	}

	public void Save(Stream outputStream)
	{
		ThrowExceptionIfReadOnly();
		SaveImpl(outputStream);
	}

	protected abstract PackagePart CreatePartImpl(PackagePartName PartName, string contentType, bool loadRelationships);

	protected abstract void RemovePartImpl(PackagePartName PartName);

	protected abstract void FlushImpl();

	protected abstract void CloseImpl();

	protected abstract void RevertImpl();

	protected abstract void SaveImpl(Stream outputStream);

	protected abstract PackagePart GetPartImpl(PackagePartName PartName);

	protected abstract PackagePart[] GetPartsImpl();

	public bool ReplaceContentType(string oldContentType, string newContentType)
	{
		bool result = false;
		foreach (PackagePart item in GetPartsByContentType(oldContentType))
		{
			if (item.ContentType.Equals(oldContentType))
			{
				PackagePartName partName = item.PartName;
				contentTypeManager.AddContentType(partName, newContentType);
				result = true;
			}
		}
		return result;
	}

	public void RegisterPartAndContentType(PackagePart part)
	{
		AddPackagePart(part);
		contentTypeManager.AddContentType(part.PartName, part.ContentType);
		isDirty = true;
	}

	public void UnregisterPartAndContentType(PackagePartName partName)
	{
		RemovePart(partName);
		contentTypeManager.RemoveContentType(partName);
		isDirty = true;
	}
}
