namespace NPOI.POIFS.Crypt.Dsig.Services;

public interface ISignaturePolicyService
{
	string GetSignaturePolicyIdentifier();

	string GetSignaturePolicyDescription();

	string GetSignaturePolicyDownloadUrl();

	byte[] GetSignaturePolicyDocument();
}
