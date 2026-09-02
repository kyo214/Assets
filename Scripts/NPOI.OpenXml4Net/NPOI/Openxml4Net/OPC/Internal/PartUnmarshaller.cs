using System.IO;
using NPOI.OpenXml4Net.OPC.Internal.Unmarshallers;

namespace NPOI.OpenXml4Net.OPC.Internal;

public interface PartUnmarshaller
{
	PackagePart Unmarshall(UnmarshallContext context, Stream in1);
}
