using System;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFSettings : POIXMLDocumentPart
{
	private CT_Settings ctSettings;

	public bool IsTrackRevisions
	{
		get
		{
			return ctSettings.IsSetTrackRevisions();
		}
		set
		{
			if (value)
			{
				if (!ctSettings.IsSetTrackRevisions())
				{
					ctSettings.AddNewTrackRevisions();
				}
			}
			else if (ctSettings.IsSetTrackRevisions())
			{
				ctSettings.UnsetTrackRevisions();
			}
		}
	}

	public XWPFSettings(PackagePart part)
		: base(part)
	{
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public XWPFSettings(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	public XWPFSettings()
	{
		ctSettings = new CT_Settings();
	}

	internal override void OnDocumentRead()
	{
		base.OnDocumentRead();
		ReadFrom(GetPackagePart().GetInputStream());
	}

	public long GetZoomPercent()
	{
		CT_Zoom zoom = ctSettings.zoom;
		zoom = (ctSettings.IsSetZoom() ? ctSettings.zoom : ctSettings.AddNewZoom());
		return long.Parse(zoom.percent);
	}

	public void SetZoomPercent(long zoomPercent)
	{
		if (!ctSettings.IsSetZoom())
		{
			ctSettings.AddNewZoom();
		}
		ctSettings.zoom.percent = zoomPercent.ToString();
	}

	public bool IsEnforcedWith()
	{
		return ctSettings.documentProtection?.enforcement.Equals(ST_OnOff.on) ?? false;
	}

	public bool IsEnforcedWith(ST_DocProtect editValue)
	{
		CT_DocProtect documentProtection = ctSettings.documentProtection;
		if (documentProtection == null)
		{
			return false;
		}
		if (documentProtection.enforcement.Equals(ST_OnOff.on))
		{
			return documentProtection.edit.Equals(editValue);
		}
		return false;
	}

	public void SetEnforcementEditValue(ST_DocProtect editValue)
	{
		SafeGetDocumentProtection().enforcement = ST_OnOff.on;
		SafeGetDocumentProtection().edit = editValue;
	}

	public void RemoveEnforcement()
	{
		SafeGetDocumentProtection().enforcement = ST_OnOff.off;
	}

	public void SetUpdateFields()
	{
		CT_OnOff cT_OnOff = new CT_OnOff();
		cT_OnOff.val = true;
		ctSettings.updateFields = cT_OnOff;
	}

	public bool IsUpdateFields()
	{
		if (ctSettings.IsSetUpdateFields())
		{
			return ctSettings.updateFields.val;
		}
		return false;
	}

	protected internal override void Commit()
	{
		if (ctSettings == null)
		{
			throw new InvalidOperationException("Unable to write out settings that were never read in!");
		}
		using Stream stream = GetPackagePart().GetOutputStream();
		new SettingsDocument(ctSettings).Save(stream);
	}

	private CT_DocProtect SafeGetDocumentProtection()
	{
		CT_DocProtect documentProtection = ctSettings.documentProtection;
		if (documentProtection == null)
		{
			documentProtection = new CT_DocProtect();
			ctSettings.documentProtection = documentProtection;
		}
		return ctSettings.documentProtection;
	}

	private void ReadFrom(Stream inputStream)
	{
		try
		{
			XmlDocument doc = POIXMLDocumentPart.ConvertStreamToXml(inputStream);
			ctSettings = SettingsDocument.Parse(doc, POIXMLDocumentPart.NamespaceManager).Settings;
		}
		catch (Exception innerException)
		{
			throw new Exception("SettingsDocument parse failed", innerException);
		}
	}
}
