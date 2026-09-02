namespace NPOI.DDF;

public interface EscherSerializationListener
{
	void BeforeRecordSerialize(int offset, short recordId, EscherRecord record);

	void AfterRecordSerialize(int offset, short recordId, int size, EscherRecord record);
}
