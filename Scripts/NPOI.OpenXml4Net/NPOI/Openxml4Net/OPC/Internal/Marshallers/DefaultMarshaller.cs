using System.IO;

namespace NPOI.OpenXml4Net.OPC.Internal.Marshallers;

public class DefaultMarshaller : PartMarshaller
{
	public bool Marshall(PackagePart part, Stream out1)
	{
		return part.Save(out1);
	}
}
