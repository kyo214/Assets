using System.Collections;
using System.Collections.Generic;
using NPOI.OpenXmlFormats.Shared;
using NPOI.XWPF.UserModel;

namespace NPOI.XWPF.Usermodel;

public abstract class MathContainer : IRunBody
{
	protected IRunBody parent;

	protected XWPFDocument document;

	protected IOMathContainer container;

	protected List<XWPFSharedRun> runs;

	protected List<XWPFNary> naries;

	protected List<XWPFAcc> accs;

	protected List<XWPFSSub> sSubs;

	protected List<XWPFSSup> sSups;

	protected List<XWPFF> fs;

	protected List<XWPFRad> rads;

	public XWPFDocument Document => document;

	public POIXMLDocumentPart Part => parent.Part;

	public IList<XWPFSharedRun> Runs => runs.AsReadOnly();

	public IList<XWPFAcc> Accs => accs.AsReadOnly();

	public IList<XWPFNary> Naries => naries.AsReadOnly();

	public IList<XWPFSSub> SSubs => sSubs.AsReadOnly();

	public IList<XWPFF> Fs => fs.AsReadOnly();

	public IList<XWPFRad> Rads => rads.AsReadOnly();

	public MathContainer(IOMathContainer c, IRunBody p)
	{
		container = c;
		parent = p;
		document = p.Document;
		FillLists(c.Items);
	}

	private void FillLists(ArrayList items)
	{
		runs = new List<XWPFSharedRun>();
		accs = new List<XWPFAcc>();
		naries = new List<XWPFNary>();
		sSubs = new List<XWPFSSub>();
		sSups = new List<XWPFSSup>();
		fs = new List<XWPFF>();
		rads = new List<XWPFRad>();
		BuildListsInOrderFromXml(items);
	}

	private void BuildListsInOrderFromXml(ArrayList items)
	{
		foreach (object item in items)
		{
			if (item is CT_R)
			{
				runs.Add(new XWPFSharedRun(item as CT_R, this));
			}
			if (item is CT_Acc)
			{
				accs.Add(new XWPFAcc(item as CT_Acc, this));
			}
			if (item is CT_Nary)
			{
				naries.Add(new XWPFNary(item as CT_Nary, this));
			}
			if (item is CT_SSub)
			{
				sSubs.Add(new XWPFSSub(item as CT_SSub, this));
			}
			if (item is CT_F)
			{
				fs.Add(new XWPFF(item as CT_F, this));
			}
			if (item is CT_Rad)
			{
				rads.Add(new XWPFRad(item as CT_Rad, this));
			}
		}
	}

	public XWPFSharedRun CreateRun()
	{
		XWPFSharedRun xWPFSharedRun = new XWPFSharedRun(container.AddNewR(), this);
		runs.Add(xWPFSharedRun);
		return xWPFSharedRun;
	}

	public XWPFAcc CreateAcc()
	{
		XWPFAcc xWPFAcc = new XWPFAcc(container.AddNewAcc(), this);
		accs.Add(xWPFAcc);
		return xWPFAcc;
	}

	public XWPFNary CreateNary()
	{
		XWPFNary xWPFNary = new XWPFNary(container.AddNewNary(), this);
		naries.Add(xWPFNary);
		return xWPFNary;
	}

	public XWPFSSub CreateSSub()
	{
		XWPFSSub xWPFSSub = new XWPFSSub(container.AddNewSSub(), this);
		sSubs.Add(xWPFSSub);
		return xWPFSSub;
	}

	public XWPFSSup CreateSSup()
	{
		XWPFSSup xWPFSSup = new XWPFSSup(container.AddNewSSup(), this);
		sSups.Add(xWPFSSup);
		return xWPFSSup;
	}

	public XWPFF CreateF()
	{
		XWPFF xWPFF = new XWPFF(container.AddNewF(), this);
		fs.Add(xWPFF);
		return xWPFF;
	}

	public XWPFRad CreateRad()
	{
		XWPFRad xWPFRad = new XWPFRad(container.AddNewRad(), this);
		rads.Add(xWPFRad);
		return xWPFRad;
	}
}
