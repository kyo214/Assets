namespace NPOI.DDF;

public interface IEscherRecordFactory
{
	EscherRecord CreateRecord(byte[] data, int offset);
}
