using System.Collections.Generic;
using Org.BouncyCastle.X509;

namespace NPOI.POIFS.Crypt.Dsig.Services;

public interface ITimeStampServiceValidator
{
	void Validate(List<X509Certificate> certificateChain, RevocationData revocationData);
}
