namespace NPOI.HSSF.Record;

public interface BiffHeaderInput
{
	int ReadRecordSID();

	int ReadDataSize();

	int Available();
}
