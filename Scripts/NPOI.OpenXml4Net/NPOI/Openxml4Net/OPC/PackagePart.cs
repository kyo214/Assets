using System;
using System.IO;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.OpenXml4Net.OPC.Internal;

namespace NPOI.OpenXml4Net.OPC;

public abstract class PackagePart : RelationshipSource, IComparable<PackagePart>
{
	internal OPCPackage _container;

	protected PackagePartName _partName;

	internal ContentType _contentType;

	private bool _isRelationshipPart;

	private bool _isDeleted;

	private PackageRelationshipCollection _relationships;

	public PackageRelationshipCollection Relationships => GetRelationshipsCore(null);

	public bool HasRelationships
	{
		get
		{
			if (!IsRelationshipPart)
			{
				if (_relationships != null)
				{
					return _relationships.Size > 0;
				}
				return false;
			}
			return false;
		}
	}

	public PackagePartName PartName => _partName;

	public string ContentType
	{
		get
		{
			return _contentType.ToString();
		}
		set
		{
			if (_container == null)
			{
				_contentType = new ContentType(value);
				return;
			}
			_container.UnregisterPartAndContentType(_partName);
			_contentType = new ContentType(value);
			_container.RegisterPartAndContentType(this);
		}
	}

	public ContentType ContentTypeDetails => _contentType;

	public OPCPackage Package => _container;

	public bool IsRelationshipPart => _isRelationshipPart;

	public bool IsDeleted
	{
		get
		{
			return _isDeleted;
		}
		set
		{
			_isDeleted = value;
		}
	}

	public virtual long Size => -1L;

	protected PackagePart(OPCPackage pack, PackagePartName partName, ContentType contentType)
		: this(pack, partName, contentType, loadRelationships: true)
	{
	}

	protected PackagePart(OPCPackage pack, PackagePartName partName, ContentType contentType, bool loadRelationships)
	{
		_partName = partName;
		_contentType = contentType;
		_container = (ZipPackage)pack;
		_isRelationshipPart = _partName.IsRelationshipPartURI();
		if (loadRelationships)
		{
			LoadRelationships();
		}
	}

	public PackagePart(OPCPackage pack, PackagePartName partName, string contentType)
		: this(pack, partName, new ContentType(contentType))
	{
	}

	public PackageRelationship FindExistingRelation(PackagePart packagePart)
	{
		return _relationships.FindExistingInternalRelation(packagePart);
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
		if (_relationships == null)
		{
			_relationships = new PackageRelationshipCollection();
		}
		Uri targetUri;
		try
		{
			targetUri = PackagingUriHelper.ParseUri(target, UriKind.RelativeOrAbsolute);
		}
		catch (UriFormatException ex)
		{
			throw new ArgumentException("Invalid target - " + ex);
		}
		return _relationships.AddRelationship(targetUri, TargetMode.External, relationshipType, id);
	}

	public PackageRelationship AddRelationship(PackagePartName targetPartName, TargetMode targetMode, string relationshipType)
	{
		return AddRelationship(targetPartName, targetMode, relationshipType, null);
	}

	public PackageRelationship AddRelationship(PackagePartName targetPartName, TargetMode targetMode, string relationshipType, string id)
	{
		_container.ThrowExceptionIfReadOnly();
		if (targetPartName == null)
		{
			throw new ArgumentException("targetPartName");
		}
		if (relationshipType == null)
		{
			throw new ArgumentException("relationshipType");
		}
		if (IsRelationshipPart || targetPartName.IsRelationshipPartURI())
		{
			throw new InvalidOperationException("Rule M1.25: The Relationships part shall not have relationships to any other part.");
		}
		if (_relationships == null)
		{
			_relationships = new PackageRelationshipCollection();
		}
		return _relationships.AddRelationship(targetPartName.URI, targetMode, relationshipType, id);
	}

	public PackageRelationship AddRelationship(Uri targetURI, TargetMode targetMode, string relationshipType)
	{
		return AddRelationship(targetURI, targetMode, relationshipType, null);
	}

	public PackageRelationship AddRelationship(Uri targetURI, TargetMode targetMode, string relationshipType, string id)
	{
		_container.ThrowExceptionIfReadOnly();
		if (targetURI == null)
		{
			throw new ArgumentException("targetPartName");
		}
		if (relationshipType == null)
		{
			throw new ArgumentException("relationshipType");
		}
		if (IsRelationshipPart || PackagingUriHelper.IsRelationshipPartURI(targetURI))
		{
			throw new InvalidOperationException("Rule M1.25: The Relationships part shall not have relationships to any other part.");
		}
		if (_relationships == null)
		{
			_relationships = new PackageRelationshipCollection();
		}
		return _relationships.AddRelationship(targetURI, targetMode, relationshipType, id);
	}

	public void ClearRelationships()
	{
		if (_relationships != null)
		{
			_relationships.Clear();
		}
	}

	public void RemoveRelationship(string id)
	{
		_container.ThrowExceptionIfReadOnly();
		if (_relationships != null)
		{
			_relationships.RemoveRelationship(id);
		}
	}

	public PackageRelationship GetRelationship(string id)
	{
		return _relationships.GetRelationshipByID(id);
	}

	public PackageRelationshipCollection GetRelationshipsByType(string relationshipType)
	{
		_container.ThrowExceptionIfWriteOnly();
		return GetRelationshipsCore(relationshipType);
	}

	private PackageRelationshipCollection GetRelationshipsCore(string filter)
	{
		_container.ThrowExceptionIfWriteOnly();
		if (_relationships == null)
		{
			ThrowExceptionIfRelationship();
			_relationships = new PackageRelationshipCollection(this);
		}
		return new PackageRelationshipCollection(_relationships, filter);
	}

	public bool IsRelationshipExists(PackageRelationship rel)
	{
		try
		{
			foreach (PackageRelationship relationship in Relationships)
			{
				if (relationship == rel)
				{
					return true;
				}
			}
		}
		catch (InvalidFormatException)
		{
		}
		return false;
	}

	public PackagePart GetRelatedPart(PackageRelationship rel)
	{
		if (!IsRelationshipExists(rel))
		{
			throw new ArgumentException("Relationship " + rel?.ToString() + " doesn't start with this part " + _partName);
		}
		Uri uri = rel.TargetUri;
		if (uri.OriginalString.IndexOf('#') >= 0)
		{
			string text = uri.ToString();
			try
			{
				uri = PackagingUriHelper.ParseUri(text.Substring(0, text.IndexOf('#')), UriKind.Absolute);
			}
			catch (UriFormatException)
			{
				throw new InvalidFormatException("Invalid target URI: " + text);
			}
		}
		PackagePartName partName = PackagingUriHelper.CreatePartName(uri);
		return _container.GetPart(partName) ?? throw new ArgumentException("No part found for relationship " + rel);
	}

	public Stream GetStream(FileMode mode)
	{
		return GetStream(mode, FileAccess.Write);
	}

	public Stream GetStream(FileMode mode, FileAccess access)
	{
		if (mode == FileMode.Create && access == FileAccess.Write)
		{
			return GetOutputStream();
		}
		return GetInputStream();
	}

	public Stream GetInputStream()
	{
		Stream inputStreamImpl = GetInputStreamImpl();
		if (inputStreamImpl == null)
		{
			throw new IOException("Can't obtain the input stream from " + _partName.Name);
		}
		return inputStreamImpl;
	}

	public Stream GetOutputStream()
	{
		if (this is ZipPackagePart)
		{
			_container.RemovePart(_partName);
			PackagePart obj = _container.CreatePart(_partName, _contentType.ToString(), loadRelationships: false) ?? throw new InvalidOperationException("Can't create a temporary part !");
			obj._relationships = _relationships;
			return obj.GetOutputStreamImpl();
		}
		return GetOutputStreamImpl();
	}

	private void ThrowExceptionIfRelationship()
	{
		if (IsRelationshipPart)
		{
			throw new InvalidOperationException("Can do this operation on a relationship part !");
		}
	}

	private void LoadRelationships()
	{
		if (_relationships == null && !IsRelationshipPart)
		{
			ThrowExceptionIfRelationship();
			_relationships = new PackageRelationshipCollection(this);
		}
	}

	public override string ToString()
	{
		return "Name: " + _partName?.ToString() + " - Content Type: " + _contentType.ToString();
	}

	public int CompareTo(PackagePart other)
	{
		if (other == null)
		{
			return -1;
		}
		return PackagePartName.Compare(_partName, other._partName);
	}

	protected abstract Stream GetInputStreamImpl();

	protected abstract Stream GetOutputStreamImpl();

	public abstract bool Save(Stream zos);

	public abstract bool Load(Stream ios);

	public abstract void Close();

	public abstract void Flush();

	public virtual void Clear()
	{
	}
}
