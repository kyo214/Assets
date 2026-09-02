namespace NPOI.Util;

public interface IDelayableLittleEndianOutput : ILittleEndianOutput
{
	ILittleEndianOutput CreateDelayedOutput(int size);
}
