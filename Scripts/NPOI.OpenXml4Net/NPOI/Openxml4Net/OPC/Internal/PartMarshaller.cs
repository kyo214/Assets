using System.IO;

namespace NPOI.OpenXml4Net.OPC.Internal;

public interface PartMarshaller
{
	bool Marshall(PackagePart part, Stream out1);
}
