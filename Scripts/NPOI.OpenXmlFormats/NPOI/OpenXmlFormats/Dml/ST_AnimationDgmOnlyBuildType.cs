using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_AnimationDgmOnlyBuildType
{
	one = 0,
	lvlOne = 1,
	lvlAtOnce = 2
}
