namespace NPOI.POIFS.Crypt.Dsig.Services;

public interface ITimeStampService : ISignatureConfigurable
{
	byte[] TimeStamp(byte[] data, RevocationData revocationData);
}
