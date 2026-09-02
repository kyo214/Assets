using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFFootnotes : POIXMLDocumentPart
{
	private List<XWPFFootnote> listFootnote = new List<XWPFFootnote>();

	private CT_Footnotes ctFootnotes;

	protected XWPFDocument document;

	public XWPFFootnotes(PackagePart part)
		: base(part)
	{
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public XWPFFootnotes(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	public XWPFFootnotes()
	{
	}

	internal override void OnDocumentRead()
	{
		Stream stream = null;
		try
		{
			stream = GetPackagePart().GetInputStream();
			FootnotesDocument footnotesDocument = FootnotesDocument.Parse(POIXMLDocumentPart.ConvertStreamToXml(stream), POIXMLDocumentPart.NamespaceManager);
			ctFootnotes = footnotesDocument.Footnotes;
		}
		catch (XmlException)
		{
			throw new POIXMLException();
		}
		finally
		{
			stream?.Close();
		}
		if (ctFootnotes.footnote == null)
		{
			return;
		}
		foreach (CT_FtnEdn item in ctFootnotes.footnote)
		{
			listFootnote.Add(new XWPFFootnote(item, this));
		}
	}

	protected internal override void Commit()
	{
		using Stream stream = GetPackagePart().GetOutputStream();
		new FootnotesDocument(ctFootnotes).Save(stream);
	}

	public List<XWPFFootnote> GetFootnotesList()
	{
		return listFootnote;
	}

	public XWPFFootnote GetFootnoteById(int id)
	{
		foreach (XWPFFootnote item in listFootnote)
		{
			if (item.GetCTFtnEdn().id == id.ToString())
			{
				return item;
			}
		}
		return null;
	}

	public void SetFootnotes(CT_Footnotes footnotes)
	{
		ctFootnotes = footnotes;
	}

	public void AddFootnote(XWPFFootnote footnote)
	{
		listFootnote.Add(footnote);
		ctFootnotes.AddNewFootnote().Set(footnote.GetCTFtnEdn());
	}

	public XWPFFootnote AddFootnote(CT_FtnEdn note)
	{
		CT_FtnEdn cT_FtnEdn = ctFootnotes.AddNewFootnote();
		cT_FtnEdn.Set(note);
		XWPFFootnote xWPFFootnote = new XWPFFootnote(cT_FtnEdn, this);
		listFootnote.Add(xWPFFootnote);
		return xWPFFootnote;
	}

	public void SetXWPFDocument(XWPFDocument doc)
	{
		document = doc;
	}

	public XWPFDocument GetXWPFDocument()
	{
		if (document != null)
		{
			return document;
		}
		return (XWPFDocument)GetParent();
	}
}
