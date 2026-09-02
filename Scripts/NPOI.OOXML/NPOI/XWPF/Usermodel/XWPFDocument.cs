using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NPOI.OOXML.XWPF.Util;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.Util;
using NPOI.XWPF.Model;

namespace NPOI.XWPF.UserModel;

public class XWPFDocument : POIXMLDocument, Document, IBody
{
	private CT_Document ctDocument;

	private XWPFSettings Settings;

	private IdentifierManager drawingIdManager = new IdentifierManager(0L, 4294967295L);

	protected List<XWPFFooter> footers = new List<XWPFFooter>();

	protected List<XWPFHeader> headers = new List<XWPFHeader>();

	protected List<XWPFComment> comments = new List<XWPFComment>();

	protected List<XWPFHyperlink> hyperlinks = new List<XWPFHyperlink>();

	protected List<XWPFParagraph> paragraphs = new List<XWPFParagraph>();

	protected List<XWPFTable> tables = new List<XWPFTable>();

	protected List<XWPFSDT> contentControls = new List<XWPFSDT>();

	protected List<IBodyElement> bodyElements = new List<IBodyElement>();

	protected List<XWPFPictureData> pictures = new List<XWPFPictureData>();

	protected Dictionary<long, List<XWPFPictureData>> packagePictures = new Dictionary<long, List<XWPFPictureData>>();

	protected Dictionary<int, XWPFFootnote> endnotes = new Dictionary<int, XWPFFootnote>();

	protected XWPFNumbering numbering;

	protected XWPFStyles styles;

	protected XWPFFootnotes footnotes;

	private XWPFHeaderFooterPolicy headerFooterPolicy;

	public CT_Document Document
	{
		get
		{
			return ctDocument;
		}
		set
		{
			ctDocument = value;
		}
	}

	public int ColumnCount
	{
		get
		{
			return int.Parse(ctDocument.body.sectPr.cols.num);
		}
		set
		{
			if (ctDocument != null)
			{
				ctDocument.body.sectPr.cols.num = value.ToString();
			}
		}
	}

	public ST_TextDirection TextDirection
	{
		get
		{
			return ctDocument.body.sectPr.textDirection.val;
		}
		set
		{
			if (ctDocument != null)
			{
				ctDocument.body.sectPr.textDirection.val = value;
			}
		}
	}

	internal IdentifierManager DrawingIdManager => drawingIdManager;

	public IList<IBodyElement> BodyElements => bodyElements.AsReadOnly();

	public IList<XWPFParagraph> Paragraphs => paragraphs.AsReadOnly();

	public IList<XWPFTable> Tables => tables.AsReadOnly();

	public IList<XWPFFooter> FooterList => footers.AsReadOnly();

	public IList<XWPFHeader> HeaderList => headers.AsReadOnly();

	public Dictionary<int, XWPFFootnote> Endnotes => endnotes;

	public bool IsTrackRevisions
	{
		get
		{
			return Settings.IsTrackRevisions;
		}
		set
		{
			Settings.IsTrackRevisions = value;
		}
	}

	public IList<XWPFPictureData> AllPictures => pictures.AsReadOnly();

	public IList<XWPFPictureData> AllPackagePictures
	{
		get
		{
			List<XWPFPictureData> list = new List<XWPFPictureData>();
			foreach (List<XWPFPictureData> value in packagePictures.Values)
			{
				list.AddRange(value);
			}
			return list.AsReadOnly();
		}
	}

	public POIXMLDocumentPart Part => this;

	public BodyType PartType => BodyType.DOCUMENT;

	public XWPFDocument(OPCPackage pkg)
		: base(pkg)
	{
		Load(XWPFFactory.GetInstance());
	}

	public XWPFDocument(Stream is1)
		: base(PackageHelper.Open(is1))
	{
		Load(XWPFFactory.GetInstance());
	}

	public XWPFDocument()
		: base(NewPackage())
	{
		OnDocumentCreate();
	}

	internal override void OnDocumentRead()
	{
		try
		{
			DocumentDocument documentDocument = DocumentDocument.Parse(DocumentHelper.LoadDocument(GetPackagePart().GetInputStream()), POIXMLDocumentPart.NamespaceManager);
			ctDocument = documentDocument.Document;
			InitFootnotes();
			foreach (object item4 in ctDocument.body.Items)
			{
				if (item4 is CT_P)
				{
					XWPFParagraph item = new XWPFParagraph((CT_P)item4, this);
					bodyElements.Add(item);
					paragraphs.Add(item);
				}
				else if (item4 is CT_Tbl)
				{
					XWPFTable item2 = new XWPFTable((CT_Tbl)item4, this);
					bodyElements.Add(item2);
					tables.Add(item2);
				}
				else if (item4 is CT_SdtBlock)
				{
					XWPFSDT item3 = new XWPFSDT((CT_SdtBlock)item4, this);
					bodyElements.Add(item3);
					contentControls.Add(item3);
				}
			}
			if (documentDocument.Document.body.sectPr != null)
			{
				headerFooterPolicy = new XWPFHeaderFooterPolicy(this);
			}
			foreach (RelationPart relationPart in base.RelationParts)
			{
				POIXMLDocumentPart documentPart = relationPart.DocumentPart;
				string relationshipType = relationPart.Relationship.RelationshipType;
				if (relationshipType.Equals(XWPFRelation.STYLES.Relation))
				{
					styles = (XWPFStyles)documentPart;
					styles.OnDocumentRead();
				}
				else if (relationshipType.Equals(XWPFRelation.NUMBERING.Relation))
				{
					numbering = (XWPFNumbering)documentPart;
					numbering.OnDocumentRead();
				}
				else if (relationshipType.Equals(XWPFRelation.FOOTER.Relation))
				{
					XWPFFooter xWPFFooter = (XWPFFooter)documentPart;
					footers.Add(xWPFFooter);
					xWPFFooter.OnDocumentRead();
				}
				else if (relationshipType.Equals(XWPFRelation.HEADER.Relation))
				{
					XWPFHeader xWPFHeader = (XWPFHeader)documentPart;
					headers.Add(xWPFHeader);
					xWPFHeader.OnDocumentRead();
				}
				else if (relationshipType.Equals(XWPFRelation.COMMENT.Relation))
				{
					foreach (CT_Comment item5 in CommentsDocument.Parse(POIXMLDocumentPart.ConvertStreamToXml(documentPart.GetPackagePart().GetInputStream()), POIXMLDocumentPart.NamespaceManager).Comments.comment)
					{
						comments.Add(new XWPFComment(item5, this));
					}
				}
				else if (relationshipType.Equals(XWPFRelation.SETTINGS.Relation))
				{
					Settings = (XWPFSettings)documentPart;
					Settings.OnDocumentRead();
				}
				else if (relationshipType.Equals(XWPFRelation.IMAGES.Relation))
				{
					XWPFPictureData xWPFPictureData = (XWPFPictureData)documentPart;
					xWPFPictureData.OnDocumentRead();
					RegisterPackagePictureData(xWPFPictureData);
					pictures.Add(xWPFPictureData);
				}
				else
				{
					if (!relationshipType.Equals(XWPFRelation.GLOSSARY_DOCUMENT.Relation))
					{
						continue;
					}
					foreach (POIXMLDocumentPart relation in documentPart.GetRelations())
					{
						try
						{
							relation.OnDocumentRead();
						}
						catch (Exception ex)
						{
							throw new POIXMLException(ex);
						}
					}
				}
			}
			InitHyperlinks();
		}
		catch (XmlException ex2)
		{
			throw new POIXMLException(ex2);
		}
	}

	private void InitHyperlinks()
	{
		try
		{
			IEnumerator<PackageRelationship> enumerator = GetPackagePart().GetRelationshipsByType(XWPFRelation.HYPERLINK.Relation).GetEnumerator();
			while (enumerator.MoveNext())
			{
				PackageRelationship current = enumerator.Current;
				hyperlinks.Add(new XWPFHyperlink(current.Id, current.TargetUri.ToString()));
			}
		}
		catch (InvalidDataException ex)
		{
			throw new POIXMLException(ex);
		}
	}

	private void InitFootnotes()
	{
		foreach (RelationPart relationPart in base.RelationParts)
		{
			POIXMLDocumentPart documentPart = relationPart.DocumentPart;
			string relationshipType = relationPart.Relationship.RelationshipType;
			if (relationshipType.Equals(XWPFRelation.FOOTNOTE.Relation))
			{
				footnotes = (XWPFFootnotes)documentPart;
				footnotes.OnDocumentRead();
			}
			if (!relationshipType.Equals(XWPFRelation.ENDNOTE.Relation))
			{
				continue;
			}
			foreach (CT_FtnEdn item in EndnotesDocument.Parse(POIXMLDocumentPart.ConvertStreamToXml(documentPart.GetPackagePart().GetInputStream()), POIXMLDocumentPart.NamespaceManager).Endnotes.endnote)
			{
				endnotes.Add(int.Parse(item.id), new XWPFFootnote(this, item));
			}
		}
	}

	protected static OPCPackage NewPackage()
	{
		try
		{
			OPCPackage oPCPackage = OPCPackage.Create(new MemoryStream());
			PackagePartName packagePartName = PackagingUriHelper.CreatePartName(XWPFRelation.DOCUMENT.DefaultFileName);
			oPCPackage.AddRelationship(packagePartName, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument");
			oPCPackage.CreatePart(packagePartName, XWPFRelation.DOCUMENT.ContentType);
			oPCPackage.GetPackageProperties().SetCreatorProperty(POIXMLDocument.DOCUMENT_CREATOR);
			return oPCPackage;
		}
		catch (Exception ex)
		{
			throw new POIXMLException(ex);
		}
	}

	internal override void OnDocumentCreate()
	{
		ctDocument = new CT_Document();
		ctDocument.AddNewBody();
		Settings = (XWPFSettings)CreateRelationship(XWPFRelation.SETTINGS, XWPFFactory.GetInstance());
		CreateStyles();
		GetProperties().ExtendedProperties.GetUnderlyingProperties().Application = POIXMLDocument.DOCUMENT_CREATOR;
	}

	public IEnumerator<IBodyElement> GetBodyElementsIterator()
	{
		return bodyElements.GetEnumerator();
	}

	public XWPFTable GetTableArray(int pos)
	{
		if (pos >= 0 && pos < tables.Count)
		{
			return tables[pos];
		}
		return null;
	}

	public XWPFFooter GetFooterArray(int pos)
	{
		if (pos >= 0 && pos < footers.Count)
		{
			return footers[pos];
		}
		return null;
	}

	public XWPFHeader GetHeaderArray(int pos)
	{
		if (pos >= 0 && pos < headers.Count)
		{
			return headers[pos];
		}
		return null;
	}

	public string GetTblStyle(XWPFTable table)
	{
		return table.StyleID;
	}

	public XWPFHyperlink GetHyperlinkByID(string id)
	{
		IEnumerator<XWPFHyperlink> enumerator = hyperlinks.GetEnumerator();
		while (enumerator.MoveNext())
		{
			XWPFHyperlink current = enumerator.Current;
			if (current.Id.Equals(id))
			{
				return current;
			}
		}
		return null;
	}

	public XWPFFootnote GetFootnoteByID(int id)
	{
		if (footnotes == null)
		{
			return null;
		}
		return footnotes.GetFootnoteById(id);
	}

	public XWPFFootnote GetEndnoteByID(int id)
	{
		if (endnotes == null || !endnotes.ContainsKey(id))
		{
			return null;
		}
		return endnotes[id];
	}

	public List<XWPFFootnote> GetFootnotes()
	{
		if (footnotes == null)
		{
			return new List<XWPFFootnote>();
		}
		return footnotes.GetFootnotesList();
	}

	public XWPFHyperlink[] GetHyperlinks()
	{
		return hyperlinks.ToArray();
	}

	public XWPFComment GetCommentByID(string id)
	{
		IEnumerator<XWPFComment> enumerator = comments.GetEnumerator();
		while (enumerator.MoveNext())
		{
			XWPFComment current = enumerator.Current;
			if (current.Id.Equals(id))
			{
				return current;
			}
		}
		return null;
	}

	public XWPFComment[] GetComments()
	{
		return comments.ToArray();
	}

	public PackagePart GetPartById(string id)
	{
		try
		{
			PackagePart corePart = base.CorePart;
			return corePart.GetRelatedPart(corePart.GetRelationship(id));
		}
		catch (Exception innerException)
		{
			throw new ArgumentException("GetTargetPart exception", innerException);
		}
	}

	public XWPFHeaderFooterPolicy GetHeaderFooterPolicy()
	{
		return headerFooterPolicy;
	}

	public XWPFHeaderFooterPolicy CreateHeaderFooterPolicy()
	{
		if (headerFooterPolicy == null)
		{
			if (!ctDocument.body.IsSetSectPr())
			{
				ctDocument.body.AddNewSectPr();
			}
			headerFooterPolicy = new XWPFHeaderFooterPolicy(this);
		}
		return headerFooterPolicy;
	}

	public CT_Styles GetCTStyle()
	{
		PackagePart[] relatedByType;
		try
		{
			relatedByType = GetRelatedByType(XWPFRelation.STYLES.Relation);
		}
		catch (Exception innerException)
		{
			throw new InvalidOperationException("get Style document part exception", innerException);
		}
		if (relatedByType.Length != 1)
		{
			throw new InvalidOperationException("Expecting one Styles document part, but found " + relatedByType.Length);
		}
		return StylesDocument.Parse(POIXMLDocumentPart.ConvertStreamToXml(relatedByType[0].GetInputStream()), POIXMLDocumentPart.NamespaceManager).Styles;
	}

	public override List<PackagePart> GetAllEmbedds()
	{
		List<PackagePart> list = new List<PackagePart>();
		PackagePart packagePart = GetPackagePart();
		foreach (PackageRelationship item in GetPackagePart().GetRelationshipsByType(POIXMLDocument.OLE_OBJECT_REL_TYPE))
		{
			list.Add(packagePart.GetRelatedPart(item));
		}
		foreach (PackageRelationship item2 in GetPackagePart().GetRelationshipsByType(POIXMLDocument.PACK_OBJECT_REL_TYPE))
		{
			list.Add(packagePart.GetRelatedPart(item2));
		}
		return list;
	}

	private int GetBodyElementSpecificPos(int pos, List<IBodyElement> list)
	{
		if (list.Count == 0)
		{
			return -1;
		}
		if (pos >= 0 && pos < bodyElements.Count)
		{
			IBodyElement bodyElement = bodyElements[pos];
			if (bodyElement.ElementType != list[0].ElementType)
			{
				return -1;
			}
			for (int num = Math.Min(pos, list.Count - 1); num >= 0; num--)
			{
				if (list[num] == bodyElement)
				{
					return num;
				}
			}
		}
		return -1;
	}

	public int GetParagraphPos(int pos)
	{
		List<IBodyElement> list = new List<IBodyElement>();
		foreach (XWPFParagraph paragraph in paragraphs)
		{
			list.Add(paragraph);
		}
		return GetBodyElementSpecificPos(pos, list);
	}

	public int GetTablePos(int pos)
	{
		List<IBodyElement> list = new List<IBodyElement>();
		foreach (XWPFTable table in tables)
		{
			list.Add(table);
		}
		return GetBodyElementSpecificPos(pos, list);
	}

	public XWPFParagraph InsertNewParagraph(XmlDocument cursor)
	{
		throw new NotImplementedException();
	}

	public XWPFTable InsertNewTbl(XmlDocument cursor)
	{
		throw new NotImplementedException();
	}

	private bool IsCursorInBody(XmlDocument cursor)
	{
		throw new NotImplementedException();
	}

	private int GetPosOfBodyElement(IBodyElement needle)
	{
		BodyElementType elementType = needle.ElementType;
		for (int i = 0; i < bodyElements.Count; i++)
		{
			IBodyElement bodyElement = bodyElements[i];
			if (bodyElement.ElementType == elementType && bodyElement.Equals(needle))
			{
				return i;
			}
		}
		return -1;
	}

	public int GetPosOfParagraph(XWPFParagraph p)
	{
		return GetPosOfBodyElement(p);
	}

	public int GetPosOfTable(XWPFTable t)
	{
		return GetPosOfBodyElement(t);
	}

	protected internal override void Commit()
	{
		using Stream stream = GetPackagePart().GetOutputStream();
		new DocumentDocument(ctDocument).Save(stream);
	}

	private int GetRelationIndex(XWPFRelation relation)
	{
		int num = 1;
		foreach (RelationPart relationPart in base.RelationParts)
		{
			if (relationPart.Relationship.RelationshipType.Equals(relation.Relation))
			{
				num++;
			}
		}
		return num;
	}

	public XWPFParagraph CreateParagraph()
	{
		XWPFParagraph xWPFParagraph = new XWPFParagraph(ctDocument.body.AddNewP(), this);
		bodyElements.Add(xWPFParagraph);
		paragraphs.Add(xWPFParagraph);
		return xWPFParagraph;
	}

	public XWPFNumbering CreateNumbering()
	{
		if (numbering == null)
		{
			NumberingDocument numberingDocument = new NumberingDocument();
			XWPFRelation nUMBERING = XWPFRelation.NUMBERING;
			int relationIndex = GetRelationIndex(nUMBERING);
			XWPFNumbering xWPFNumbering = (XWPFNumbering)CreateRelationship(nUMBERING, XWPFFactory.GetInstance(), relationIndex);
			xWPFNumbering.SetNumbering(numberingDocument.Numbering);
			numbering = xWPFNumbering;
		}
		return numbering;
	}

	public XWPFStyles CreateStyles()
	{
		if (styles == null)
		{
			StylesDocument stylesDocument = new StylesDocument();
			XWPFRelation sTYLES = XWPFRelation.STYLES;
			int relationIndex = GetRelationIndex(sTYLES);
			XWPFStyles xWPFStyles = (XWPFStyles)CreateRelationship(sTYLES, XWPFFactory.GetInstance(), relationIndex);
			xWPFStyles.SetStyles(stylesDocument.Styles);
			styles = xWPFStyles;
		}
		return styles;
	}

	public XWPFFootnotes CreateFootnotes()
	{
		if (footnotes == null)
		{
			FootnotesDocument footnotesDocument = new FootnotesDocument();
			XWPFRelation fOOTNOTE = XWPFRelation.FOOTNOTE;
			int relationIndex = GetRelationIndex(fOOTNOTE);
			XWPFFootnotes xWPFFootnotes = (XWPFFootnotes)CreateRelationship(fOOTNOTE, XWPFFactory.GetInstance(), relationIndex);
			xWPFFootnotes.SetFootnotes(footnotesDocument.Footnotes);
			footnotes = xWPFFootnotes;
		}
		return footnotes;
	}

	public XWPFFootnote AddFootnote(CT_FtnEdn note)
	{
		return footnotes.AddFootnote(note);
	}

	public XWPFFootnote AddEndnote(CT_FtnEdn note)
	{
		XWPFFootnote xWPFFootnote = new XWPFFootnote(this, note);
		endnotes.Add(int.Parse(note.id), xWPFFootnote);
		return xWPFFootnote;
	}

	public bool RemoveBodyElement(int pos)
	{
		if (pos >= 0 && pos < bodyElements.Count)
		{
			BodyElementType elementType = bodyElements[pos].ElementType;
			if (elementType == BodyElementType.TABLE)
			{
				int tablePos = GetTablePos(pos);
				tables.RemoveAt(tablePos);
				ctDocument.body.RemoveTbl(tablePos);
			}
			if (elementType == BodyElementType.PARAGRAPH)
			{
				int paragraphPos = GetParagraphPos(pos);
				paragraphs.RemoveAt(paragraphPos);
				ctDocument.body.RemoveP(paragraphPos);
			}
			bodyElements.RemoveAt(pos);
			return true;
		}
		return false;
	}

	public void SetParagraph(XWPFParagraph paragraph, int pos)
	{
		paragraphs[pos] = paragraph;
		ctDocument.body.SetPArray(pos, paragraph.GetCTP());
	}

	public XWPFParagraph GetLastParagraph()
	{
		int index = paragraphs.ToArray().Length - 1;
		return paragraphs[index];
	}

	public XWPFTable CreateTable()
	{
		XWPFTable xWPFTable = new XWPFTable(ctDocument.body.AddNewTbl(), this);
		bodyElements.Add(xWPFTable);
		tables.Add(xWPFTable);
		return xWPFTable;
	}

	public XWPFTable CreateTable(int rows, int cols)
	{
		XWPFTable xWPFTable = new XWPFTable(ctDocument.body.AddNewTbl(), this, rows, cols);
		bodyElements.Add(xWPFTable);
		tables.Add(xWPFTable);
		return xWPFTable;
	}

	public void CreateTOC()
	{
		CT_Styles cT_Styles = DocumentStylesBuilder.BuildStylesForTOC();
		styles.SetStyles(cT_Styles);
		new TOC(Document.body.AddNewSdt()).Build();
		EnforceUpdateFields();
	}

	public void SetTable(int pos, XWPFTable table)
	{
		tables[pos] = table;
		ctDocument.body.SetTblArray(pos, table.GetCTTbl());
	}

	public bool IsEnforcedProtection()
	{
		return Settings.IsEnforcedWith();
	}

	public bool IsEnforcedReadonlyProtection()
	{
		return Settings.IsEnforcedWith(ST_DocProtect.readOnly);
	}

	public bool IsEnforcedFillingFormsProtection()
	{
		return Settings.IsEnforcedWith(ST_DocProtect.forms);
	}

	public bool IsEnforcedCommentsProtection()
	{
		return Settings.IsEnforcedWith(ST_DocProtect.comments);
	}

	public bool IsEnforcedTrackedChangesProtection()
	{
		return Settings.IsEnforcedWith(ST_DocProtect.trackedChanges);
	}

	public bool IsEnforcedUpdateFields()
	{
		return Settings.IsUpdateFields();
	}

	public void EnforceReadonlyProtection()
	{
		Settings.SetEnforcementEditValue(ST_DocProtect.readOnly);
	}

	public void EnforceFillingFormsProtection()
	{
		Settings.SetEnforcementEditValue(ST_DocProtect.forms);
	}

	public void EnforceCommentsProtection()
	{
		Settings.SetEnforcementEditValue(ST_DocProtect.comments);
	}

	public void EnforceTrackedChangesProtection()
	{
		Settings.SetEnforcementEditValue(ST_DocProtect.trackedChanges);
	}

	public void RemoveProtectionEnforcement()
	{
		Settings.RemoveEnforcement();
	}

	public void EnforceUpdateFields()
	{
		Settings.SetUpdateFields();
	}

	public void InsertTable(int pos, XWPFTable table)
	{
		bodyElements.Insert(pos, table);
		CT_Tbl[] tblArray = ctDocument.body.GetTblArray();
		int i;
		for (i = 0; i < tblArray.Length && tblArray[i] != table.GetCTTbl(); i++)
		{
		}
		tables.Insert(i, table);
	}

	public void RegisterPackagePictureData(XWPFPictureData picData)
	{
		List<XWPFPictureData> list = null;
		if (packagePictures.ContainsKey(picData.Checksum))
		{
			list = packagePictures[picData.Checksum];
		}
		if (list == null)
		{
			list = new List<XWPFPictureData>(1);
			packagePictures.Add(picData.Checksum, list);
		}
		if (!list.Contains(picData))
		{
			list.Add(picData);
		}
	}

	public XWPFPictureData FindPackagePictureData(byte[] pictureData, int format)
	{
		long key = IOUtils.CalculateChecksum(pictureData);
		XWPFPictureData xWPFPictureData = null;
		List<XWPFPictureData> list = null;
		if (packagePictures.ContainsKey(key))
		{
			list = packagePictures[key];
		}
		if (list != null)
		{
			IEnumerator<XWPFPictureData> enumerator = list.GetEnumerator();
			while (enumerator.MoveNext() && xWPFPictureData == null)
			{
				XWPFPictureData current = enumerator.Current;
				if (Arrays.Equals(pictureData, current.Data))
				{
					xWPFPictureData = current;
				}
			}
		}
		return xWPFPictureData;
	}

	public string AddPictureData(byte[] pictureData, int format)
	{
		XWPFPictureData xWPFPictureData = FindPackagePictureData(pictureData, format);
		POIXMLRelation descriptor = XWPFPictureData.RELATIONS[format];
		if (xWPFPictureData == null)
		{
			int nextPicNameNumber = GetNextPicNameNumber(format);
			xWPFPictureData = (XWPFPictureData)CreateRelationship(descriptor, XWPFFactory.GetInstance(), nextPicNameNumber);
			PackagePart packagePart = xWPFPictureData.GetPackagePart();
			Stream stream = null;
			try
			{
				stream = packagePart.GetOutputStream();
				stream.Write(pictureData, 0, pictureData.Length);
			}
			catch (IOException ex)
			{
				throw new POIXMLException(ex);
			}
			finally
			{
				try
				{
					stream?.Close();
				}
				catch (IOException)
				{
				}
			}
			RegisterPackagePictureData(xWPFPictureData);
			pictures.Add(xWPFPictureData);
			return GetRelationId(xWPFPictureData);
		}
		if (!GetRelations().Contains(xWPFPictureData))
		{
			xWPFPictureData.GetPackagePart();
			return AddRelation(null, XWPFRelation.IMAGES, xWPFPictureData).Relationship.Id;
		}
		return GetRelationId(xWPFPictureData);
	}

	public string AddPictureData(Stream is1, int format)
	{
		try
		{
			byte[] pictureData = IOUtils.ToByteArray(is1);
			return AddPictureData(pictureData, format);
		}
		catch (IOException ex)
		{
			throw new POIXMLException(ex);
		}
	}

	public int GetNextPicNameNumber(int format)
	{
		int num = AllPackagePictures.Count + 1;
		PackagePartName partName = PackagingUriHelper.CreatePartName(XWPFPictureData.RELATIONS[format].GetFileName(num));
		while (base.Package.GetPart(partName) != null)
		{
			num++;
			partName = PackagingUriHelper.CreatePartName(XWPFPictureData.RELATIONS[format].GetFileName(num));
		}
		return num;
	}

	public XWPFPictureData GetPictureDataByID(string blipID)
	{
		POIXMLDocumentPart relationById = GetRelationById(blipID);
		if (relationById is XWPFPictureData)
		{
			return (XWPFPictureData)relationById;
		}
		return null;
	}

	public XWPFNumbering GetNumbering()
	{
		if (numbering == null)
		{
			numbering = new XWPFNumbering();
		}
		return numbering;
	}

	public XWPFStyles GetStyles()
	{
		return styles;
	}

	public XWPFParagraph GetParagraph(CT_P p)
	{
		for (int i = 0; i < Paragraphs.Count; i++)
		{
			if (Paragraphs[i].GetCTP() == p)
			{
				return Paragraphs[i];
			}
		}
		return null;
	}

	public XWPFTable GetTable(CT_Tbl ctTbl)
	{
		for (int i = 0; i < tables.Count; i++)
		{
			if (tables[i].GetCTTbl() == ctTbl)
			{
				return tables[i];
			}
		}
		return null;
	}

	public IEnumerator<XWPFTable> GetTablesEnumerator()
	{
		return tables.GetEnumerator();
	}

	public void ChangeOrientation(ST_PageOrientation orientation)
	{
		CT_Body body = Document.body;
		if (body.sectPr == null)
		{
			body.AddNewSectPr();
		}
		CT_SectPr sectPr = body.sectPr;
		CreateParagraph().GetCTP().AddNewPPr().sectPr = sectPr;
		CT_PageSz pgSz = sectPr.pgSz;
		pgSz.orient = orientation;
		if (orientation == ST_PageOrientation.landscape)
		{
			pgSz.w = 16840uL;
			pgSz.h = 11900uL;
		}
		else
		{
			pgSz.h = 16840uL;
			pgSz.w = 11900uL;
		}
	}

	public IEnumerator<XWPFParagraph> GetParagraphsEnumerator()
	{
		return paragraphs.GetEnumerator();
	}

	public XWPFParagraph GetParagraphArray(int pos)
	{
		if (pos >= 0 && pos < paragraphs.Count)
		{
			return paragraphs[pos];
		}
		return null;
	}

	public XWPFTableCell GetTableCell(CT_Tc cell)
	{
		if (cell == null || !(cell.Parent is CT_Row))
		{
			return null;
		}
		object obj = ((CT_Row)cell.Parent).Parent;
		if (obj == null || !(obj is CT_Tbl))
		{
			return null;
		}
		return GetTable((CT_Tbl)obj)?.GetRow((CT_Row)cell.Parent)?.GetTableCell(cell);
	}

	public XWPFDocument GetXWPFDocument()
	{
		return this;
	}
}
