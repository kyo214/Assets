using System.IO;

namespace NPOI.HPSF;

public class MutableProperty : Property
{
	public MutableProperty()
	{
	}

	public MutableProperty(Property p)
	{
		ID = p.ID;
		Type = p.Type;
		Value = p.Value;
	}

	public int Write(Stream out1, int codepage)
	{
		long num = Type;
		if (codepage == 1200 && num == 30)
		{
			num = 31L;
		}
		return 0 + TypeWriter.WriteUIntToStream(out1, (uint)num) + VariantSupport.Write(out1, num, Value, codepage);
	}
}
