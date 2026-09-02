using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.OpenXmlFormats.Spreadsheet.Document;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.Model;

public class ExternalLinksTable : POIXMLDocumentPart
{
	protected internal class ExternalName : IName
	{
		private ExternalLinksTable externalLinkTable;

		private CT_ExternalDefinedName name;

		public string NameName
		{
			get
			{
				return name.name;
			}
			set
			{
				name.name = value;
			}
		}

		public string SheetName
		{
			get
			{
				int sheetIndex = SheetIndex;
				if (sheetIndex >= 0)
				{
					return externalLinkTable.SheetNames[sheetIndex];
				}
				return null;
			}
		}

		public int SheetIndex
		{
			get
			{
				if (name.IsSetSheetId())
				{
					return (int)name.sheetId;
				}
				return -1;
			}
			set
			{
				name.sheetId = (uint)value;
			}
		}

		public string RefersToFormula
		{
			get
			{
				return name.refersTo.Substring(1);
			}
			set
			{
				name.refersTo = "=" + value;
			}
		}

		public bool IsFunctionName => false;

		public bool IsDeleted => false;

		public string Comment
		{
			get
			{
				return null;
			}
			set
			{
				throw new InvalidOperationException("Not Supported");
			}
		}

		protected internal ExternalName(CT_ExternalDefinedName name, ExternalLinksTable externalLinkTable)
		{
			this.name = name;
			this.externalLinkTable = externalLinkTable;
		}

		public void SetFunction(bool value)
		{
			throw new InvalidOperationException("Not Supported");
		}
	}

	private CT_ExternalLink link;

	public CT_ExternalLink CTExternalLink => link;

	public virtual string LinkedFileName
	{
		get
		{
			string id = link.externalBook.id;
			PackageRelationship relationship = GetPackagePart().GetRelationship(id);
			if (relationship != null && relationship.TargetMode == TargetMode.External)
			{
				return relationship.TargetUri.ToString();
			}
			return null;
		}
		set
		{
			string id = link.externalBook.id;
			if (!string.IsNullOrEmpty(id))
			{
				GetPackagePart().RemoveRelationship(id);
			}
			PackageRelationship packageRelationship = GetPackagePart().AddExternalRelationship(value, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/externalLinkPath");
			link.externalBook.id = packageRelationship.Id;
		}
	}

	public List<string> SheetNames
	{
		get
		{
			CT_ExternalSheetName[] sheetName = link.externalBook.sheetNames.sheetName;
			List<string> list = new List<string>(sheetName.Length);
			CT_ExternalSheetName[] array = sheetName;
			foreach (CT_ExternalSheetName cT_ExternalSheetName in array)
			{
				list.Add(cT_ExternalSheetName.val);
			}
			return list;
		}
	}

	public List<IName> DefinedNames
	{
		get
		{
			CT_ExternalDefinedName[] definedName = link.externalBook.definedNames.definedName;
			List<IName> list = new List<IName>(definedName.Length);
			CT_ExternalDefinedName[] array = definedName;
			foreach (CT_ExternalDefinedName name in array)
			{
				list.Add(new ExternalName(name, this));
			}
			return list;
		}
	}

	public ExternalLinksTable()
	{
		link = new CT_ExternalLink();
		link.AddNewExternalBook();
	}

	internal ExternalLinksTable(PackagePart part)
		: base(part)
	{
		ReadFrom(part.GetInputStream());
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public ExternalLinksTable(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	public void ReadFrom(Stream is1)
	{
		try
		{
			ExternalLinkDocument externalLinkDocument = ExternalLinkDocument.Parse(POIXMLDocumentPart.ConvertStreamToXml(is1), POIXMLDocumentPart.NamespaceManager);
			link = externalLinkDocument.ExternalLink;
		}
		catch (XmlException ex)
		{
			throw new IOException(ex.Message);
		}
	}

	public void WriteTo(Stream out1)
	{
		ExternalLinkDocument externalLinkDocument = new ExternalLinkDocument();
		externalLinkDocument.ExternalLink = link;
		externalLinkDocument.Save(out1);
	}

	protected internal override void Commit()
	{
		Stream outputStream = GetPackagePart().GetOutputStream();
		WriteTo(outputStream);
		outputStream.Close();
	}
}
