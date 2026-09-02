using System;
using System.Collections.Generic;
using System.IO;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFNumbering : POIXMLDocumentPart
{
	protected List<XWPFAbstractNum> abstractNums = new List<XWPFAbstractNum>();

	protected List<XWPFNum> nums = new List<XWPFNum>();

	private CT_Numbering ctNumbering;

	private bool isNew;

	public XWPFNumbering(PackagePart part)
		: base(part)
	{
		isNew = true;
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public XWPFNumbering(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	public XWPFNumbering()
	{
		abstractNums = new List<XWPFAbstractNum>();
		nums = new List<XWPFNum>();
		isNew = true;
	}

	internal override void OnDocumentRead()
	{
		NumberingDocument numberingDocument = null;
		Stream inputStream = GetPackagePart().GetInputStream();
		try
		{
			numberingDocument = NumberingDocument.Parse(POIXMLDocumentPart.ConvertStreamToXml(inputStream), POIXMLDocumentPart.NamespaceManager);
			ctNumbering = numberingDocument.Numbering;
			foreach (CT_Num num in ctNumbering.GetNumList())
			{
				nums.Add(new XWPFNum(num, this));
			}
			foreach (CT_AbstractNum abstractNum in ctNumbering.GetAbstractNumList())
			{
				abstractNums.Add(new XWPFAbstractNum(abstractNum, this));
			}
			isNew = false;
		}
		catch (Exception ex)
		{
			throw new POIXMLException(ex);
		}
		finally
		{
			inputStream?.Close();
		}
	}

	protected internal override void Commit()
	{
		Stream outputStream = GetPackagePart().GetOutputStream();
		new NumberingDocument(ctNumbering).Save(outputStream);
		outputStream.Close();
	}

	public void SetNumbering(CT_Numbering numbering)
	{
		ctNumbering = numbering;
	}

	public bool NumExist(string numID)
	{
		foreach (XWPFNum num in nums)
		{
			if (num.GetCTNum().numId.Equals(numID))
			{
				return true;
			}
		}
		return false;
	}

	public string AddNum(XWPFNum num)
	{
		ctNumbering.AddNewNum();
		int pos = ctNumbering.GetNumList().Count - 1;
		ctNumbering.SetNumArray(pos, num.GetCTNum());
		nums.Add(num);
		return num.GetCTNum().numId;
	}

	public string AddNum(string abstractNumID)
	{
		CT_Num cT_Num = ctNumbering.AddNewNum();
		cT_Num.AddNewAbstractNumId();
		cT_Num.abstractNumId.val = abstractNumID;
		cT_Num.numId = (nums.Count + 1).ToString();
		XWPFNum item = new XWPFNum(cT_Num, this);
		nums.Add(item);
		return cT_Num.numId;
	}

	public void AddNum(string abstractNumID, string numID)
	{
		CT_Num cT_Num = ctNumbering.AddNewNum();
		cT_Num.AddNewAbstractNumId();
		cT_Num.abstractNumId.val = abstractNumID;
		cT_Num.numId = numID;
		XWPFNum item = new XWPFNum(cT_Num, this);
		nums.Add(item);
	}

	public XWPFNum GetNum(string numID)
	{
		foreach (XWPFNum num in nums)
		{
			if (num.GetCTNum().numId.Equals(numID))
			{
				return num;
			}
		}
		return null;
	}

	public XWPFAbstractNum GetAbstractNum(string abstractNumID)
	{
		foreach (XWPFAbstractNum abstractNum in abstractNums)
		{
			if (abstractNum.GetAbstractNum().abstractNumId.Equals(abstractNumID))
			{
				return abstractNum;
			}
		}
		return null;
	}

	public string GetIdOfAbstractNum(XWPFAbstractNum abstractNum)
	{
		XWPFAbstractNum xWPFAbstractNum = new XWPFAbstractNum(abstractNum.GetCTAbstractNum().Copy(), this);
		for (int i = 0; i < abstractNums.Count; i++)
		{
			xWPFAbstractNum.GetCTAbstractNum().abstractNumId = i.ToString();
			xWPFAbstractNum.SetNumbering(this);
			if (xWPFAbstractNum.GetCTAbstractNum().ValueEquals(abstractNums[i].GetCTAbstractNum()))
			{
				return xWPFAbstractNum.GetCTAbstractNum().abstractNumId;
			}
		}
		return null;
	}

	public string AddAbstractNum(XWPFAbstractNum abstractNum)
	{
		int count = abstractNums.Count;
		if (abstractNum.GetAbstractNum() != null)
		{
			CT_AbstractNum abstractNum2 = abstractNum.GetAbstractNum();
			abstractNum2.abstractNumId = count.ToString();
			ctNumbering.AddNewAbstractNum().Set(abstractNum2);
		}
		else
		{
			ctNumbering.AddNewAbstractNum();
			abstractNum.GetAbstractNum().abstractNumId = count.ToString();
			ctNumbering.SetAbstractNumArray(count, abstractNum.GetAbstractNum());
		}
		abstractNums.Add(abstractNum);
		return abstractNum.GetAbstractNum().abstractNumId;
	}

	public string AddAbstractNum()
	{
		XWPFAbstractNum xWPFAbstractNum = new XWPFAbstractNum(ctNumbering.AddNewAbstractNum(), this);
		xWPFAbstractNum.AbstractNumId = abstractNums.Count.ToString();
		xWPFAbstractNum.MultiLevelType = MultiLevelType.HybridMultilevel;
		xWPFAbstractNum.InitLvl();
		abstractNums.Add(xWPFAbstractNum);
		return xWPFAbstractNum.GetAbstractNum().abstractNumId;
	}

	public bool RemoveAbstractNum(string abstractNumID)
	{
		if (int.Parse(abstractNumID) < abstractNums.Count)
		{
			ctNumbering.RemoveAbstractNum(int.Parse(abstractNumID));
			abstractNums.RemoveAt(int.Parse(abstractNumID));
			return true;
		}
		return false;
	}

	public string GetAbstractNumID(string numID)
	{
		XWPFNum num = GetNum(numID);
		if (num == null)
		{
			return null;
		}
		if (num.GetCTNum() == null)
		{
			return null;
		}
		if (num.GetCTNum().abstractNumId == null)
		{
			return null;
		}
		return num.GetCTNum().abstractNumId.val;
	}
}
